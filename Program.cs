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

// ��Ⱥ��ַ�������ã�������������
if (string.IsNullOrEmpty(openJobConfig.ClusterHost))
{
    app.Logger.LogError("ClusterHost is not configured, application is not started");
    return;
}

// ����Token�������ã�������������
if (string.IsNullOrEmpty(openJobConfig.AppName))
{
    app.Logger.LogError("AppName is not configured, application is not started");
    return;
}

// �������Token�������ã�������������
if (string.IsNullOrEmpty(openJobConfig.Token))
{
    app.Logger.LogError("Token is not configured, application is not started");
    return;
}

// ���IPû�����ò��ұ���û�л�ȡ��������������
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

// ����һ��CancellationTokenSource����ȡ������
var cts = new CancellationTokenSource();

// ����Console.CancelKeyPress�¼�
Console.CancelKeyPress += async (sender, e) =>
{
    // ��ֹ������ֹ
    e.Cancel = true;

    var heartBeatService = app.Services.GetService<HeartBeatService>();

    Console.WriteLine(heartBeatService);
    // ȡ������
    cts.Cancel();
};

// ָ���˿�����
app.Urls.Add("http://*:" + openJobConfig?.Port?? "25588");

app.Run();
