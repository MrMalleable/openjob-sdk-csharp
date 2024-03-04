using openjob_sdk_csharp_agent.config;
using openjob_sdk_csharp_agent.core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

OpenJobConfig? openJobConfig = new OpenJobConfig();
builder.Services.AddSingleton(openJobConfig);
builder.Services.AddSingleton<JobInstanceService>();
builder.Services.AddHostedService<HeartBeatService>();

var app = builder.Build();

OpenJobConfig.GetConfiguration(app.Configuration, openJobConfig);

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

// 创建一个CancellationTokenSource用于取消操作
var cts = new CancellationTokenSource();

// 订阅Console.CancelKeyPress事件
Console.CancelKeyPress += async (sender, e) =>
{
    // 防止程序终止
    e.Cancel = true;

    var heartBeatService = app.Services.GetService<HeartBeatService>();

    Console.WriteLine(heartBeatService);
    // 取消操作
    cts.Cancel();
};

// 指定端口启动
app.Urls.Add("http://*:" + openJobConfig?.Port?? "25588");

app.Run();
