using Newtonsoft.Json;
using openjob_sdk_csharp_agent.common.helper;
using openjob_sdk_csharp_agent.config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

OpenJobConfig? openJobConfig = OpenJobConfig.GetConfiguration(app.Configuration);

if (openJobConfig == null)
{
    app.Logger.LogError("The configuration of Openjob is not found, application is not started");
    return;
}

// 集群地址必须配置，否则不启动服务
if (string.IsNullOrEmpty(openJobConfig.ClusterHost))
{
    app.Logger.LogError("ClusterHost is not configured, application is not started");
    return;
}

// 请求Token必须配置，否则不启动服务
if (string.IsNullOrEmpty(openJobConfig.AppName))
{
    app.Logger.LogError("AppName is not configured, application is not started");
    return;
}

// 如果请求Token必须配置，否则不启动服务
if (string.IsNullOrEmpty(openJobConfig.Token))
{
    app.Logger.LogError("Token is not configured, application is not started");
    return;
}

// 如果IP没有配置并且本地没有获取到，则不启动服务
if (string.IsNullOrEmpty(openJobConfig.Host))
{
    app.Logger.LogError("GetLocalIp error, application is not started");
    return;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// 指定端口启动
app.Urls.Add("http://*:" + openJobConfig?.Port?? "25588");

app.Run();
