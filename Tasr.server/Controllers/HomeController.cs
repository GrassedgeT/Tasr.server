using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tasr.Models;
using Tasr.server.Commands;
using Tasr.Server.Commands;
using Tasr.Server.Services;
using TheSalLab.GeneralReturnValues;

namespace Tasr.server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController
    {
        private IMeetingSummaryService _meetingSummaryService;
        //private ISummaryService _summaryService;

        public HomeController(IMeetingSummaryService summaryService)
        {
            _meetingSummaryService = summaryService;
        }

        [HttpPost]
        [Route("FileToText")]
        public async Task<ServiceResultViewModel<string>> FileToTextAsync([FromForm] AudioToTextCommand command)
        {
            //Console.WriteLine("收到请求");
            using var httpclient = new HttpClient();
            using var formData = new MultipartFormDataContent();
            formData.Add(new StreamContent(command.File.OpenReadStream()),"File","audio");
            HttpResponseMessage response;
            try
            {
                response = await httpclient.PostAsync("http://localhost:8000/audio2text", formData);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                return ServiceResult<string>.CreateExceptionResult(e, e.Message)
                    .ToServiceResultViewModel();
            }
      

            return ServiceResult<string>.CreateSucceededResult(await response.Content.ReadAsStringAsync())
                .ToServiceResultViewModel();
        }

        [HttpPost]
        [Route("summary")]
        public async Task<ServiceResultViewModel<string>> SummaryAsync([FromForm] TextContentCommand command)
        {
            string summary;
            try
            {
                summary = await _meetingSummaryService.SummaryAsync(command.Content);
            }
            catch (Exception e)
            {
                return ServiceResult<string>.CreateExceptionResult(e, e.Message).ToServiceResultViewModel();
            }

            return ServiceResult<string>.CreateSucceededResult(summary).ToServiceResultViewModel();
        }
    }
}
