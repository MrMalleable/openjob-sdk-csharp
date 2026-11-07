using Microsoft.Extensions.Options;
using openjob_sdk_csharp_agent.common.helper;
using openjob_sdk_csharp_agent.config;
using openjob_sdk_csharp_agent.core;
using openjob_sdk_csharp_agent.core.processor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<OpenJobConfig>(builder.Configuration.GetSection(OpenJobConfig.SectionName));
builder.Services.PostConfigure<OpenJobConfig>(config => config.Normalize());

builder.Services.AddSingleton<JobProcessorRegistry>();
builder.Services.AddSingleton<IJobProcessor, DefaultEchoProcessor>();
builder.Services.AddSingleton<JobInstanceService>();
builder.Services.AddHttpClient<IClusterApiClient, ClusterApiClient>((sp, client) =>
{
    var config = sp.GetRequiredService<IOptions<OpenJobConfig>>().Value;
    if (!string.IsNullOrWhiteSpace(config.ClusterHost))
    {
        client.BaseAddress = new Uri(config.ClusterHost.TrimEnd('/') + "/");
    }

    if (!string.IsNullOrWhiteSpace(config.Token))
    {
        client.DefaultRequestHeaders.Add("token", config.Token);
    }

    client.Timeout = TimeSpan.FromSeconds(Math.Max(1, config.HttpTimeoutSeconds));
});
builder.Services.AddHostedService<HeartBeatService>();

var app = builder.Build();

var openJobConfig = app.Services.GetRequiredService<IOptions<OpenJobConfig>>().Value;
var configError = openJobConfig.Validate();
if (configError != null)
{
    app.Logger.LogError("{Error}, application is not started", configError);
    return;
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Urls.Add($"http://*:{openJobConfig.Port}");

app.Logger.LogInformation(
    "OpenJob worker starting. address={Address}, appName={AppName}, cluster={ClusterHost}",
    openJobConfig.Address, openJobConfig.AppName, openJobConfig.ClusterHost);

app.Run();
