using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Tasr.server.Commands;
using TheSalLab.GeneralReturnValues;

namespace Tasr.server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController
    {
        [HttpPost]
        [Route("FileToText")]
        public async Task<ServiceResultViewModel<string>> FileToTextAsync([FromForm] AudioToTextCommand command)
        {
            Console.WriteLine("收到请求");
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
    }
}
