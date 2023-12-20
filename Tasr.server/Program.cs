using Microsoft.AspNetCore.Http.Features;
using Tasr.Server.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddScoped<IMemoryFactory, AzureOpenAiMemFactory>();
//builder.Services.AddScoped<ISemanticKercelFactory, AzureOpenAiKernelFactory>();
//builder.Services.AddScoped<ISummaryService, SummaryService>();
builder.Services.AddScoped<IChatCompletionFactory, AzureChatCompletionFactory>();
builder.Services.AddScoped<IMeetingSummaryService, MeetingSummaryService>();
// 设置上传文件大小限制，几乎无限制
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue;
});
builder.Services.Configure<FormOptions>(option =>
{
    option.MultipartBodyLengthLimit = int.MaxValue;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
