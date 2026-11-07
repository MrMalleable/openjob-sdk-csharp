using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using openjob_sdk_csharp_agent.common.constant;
using openjob_sdk_csharp_agent.common.request.agent;
using openjob_sdk_csharp_agent.config;
using openjob_sdk_csharp_agent.core;
using openjob_sdk_csharp_agent.core.processor;
using openjob_sdk_csharp_agent.Tests.Fakes;
using Xunit;

namespace openjob_sdk_csharp_agent.Tests;

public class JobInstanceServiceTests
{
    [Fact]
    public async Task RegisterOnAsync_ShouldFillAppId_WhenServerReturnsSuccess()
    {
        var (service, config, fake) = CreateService();

        var ok = await service.RegisterOnAsync();

        Assert.True(ok);
        Assert.Equal(100, config.AppId);
        Assert.Equal(1, fake.RegisterOnCount);
        Assert.True(service.IsRegistered);
    }

    [Fact]
    public async Task SubmitAsync_ShouldReject_WhenProcessorMissing()
    {
        var (service, _, _) = CreateService();

        var (accepted, message) = await service.SubmitAsync(new AgentSubmitRequest
        {
            JobId = 1,
            JobInstanceId = 11,
            ProcessorInfo = "unknownProcessor"
        });

        Assert.False(accepted);
        Assert.Contains("processor not found", message);
        Assert.Empty(service.GetJobInstanceIds());
    }

    [Fact]
    public async Task SubmitAsync_ShouldAcceptAndReportSuccess()
    {
        var (service, _, fake) = CreateService();
        var request = new AgentSubmitRequest
        {
            JobId = 1,
            JobInstanceId = 12,
            ProcessorInfo = "defaultEchoProcessor",
            JobParams = "payload"
        };

        var (accepted, message) = await service.SubmitAsync(request);

        Assert.True(accepted);
        Assert.Equal("accepted", message);

        await WaitUntilAsync(() => !service.Check(12), TimeSpan.FromSeconds(3));

        Assert.False(service.Check(12));
        Assert.Contains(fake.StatusReports, x => x.Status == InstanceStatusConstant.Running);
        Assert.Contains(fake.StatusReports, x =>
            x.Status == InstanceStatusConstant.Success &&
            x.JobInstanceId == 12 &&
            (x.Result ?? string.Empty).Contains("payload"));
    }

    [Fact]
    public async Task Stop_ShouldCancelRunningJob_AndReportStop()
    {
        var slowProcessor = new SlowProcessor();
        var (service, _, fake) = CreateService(slowProcessor);

        var (accepted, _) = await service.SubmitAsync(new AgentSubmitRequest
        {
            JobId = 2,
            JobInstanceId = 21,
            ProcessorInfo = slowProcessor.Name
        });
        Assert.True(accepted);

        await WaitUntilAsync(() => service.Check(21), TimeSpan.FromSeconds(2));
        Assert.True(service.Stop(new AgentStopRequest { JobId = 2, JobInstanceId = 21 }));

        await WaitUntilAsync(() => !service.Check(21), TimeSpan.FromSeconds(3));
        Assert.Contains(fake.StatusReports, x => x.Status == InstanceStatusConstant.Stop && x.JobInstanceId == 21);
    }

    [Fact]
    public async Task SendHeartBeatAsync_ShouldSkip_WhenAppIdMissing()
    {
        var (service, _, fake) = CreateService();

        await service.SendHeartBeatAsync();

        Assert.Equal(0, fake.HeartBeatCount);
    }

    [Fact]
    public async Task SendHeartBeatAsync_ShouldCallClient_WhenRegistered()
    {
        var (service, _, fake) = CreateService();
        await service.RegisterOnAsync();

        await service.SendHeartBeatAsync();

        Assert.Equal(1, fake.HeartBeatCount);
    }

    private static (JobInstanceService Service, OpenJobConfig Config, FakeClusterApiClient Fake)
        CreateService(params IJobProcessor[] extraProcessors)
    {
        var config = new OpenJobConfig
        {
            ClusterHost = "http://localhost:8080",
            Host = "127.0.0.1",
            Port = 8081,
            AppName = "test-openjob",
            Token = "token",
            Version = "1.0.0"
        };
        config.Normalize();

        var processors = new List<IJobProcessor> { new DefaultEchoProcessor() };
        processors.AddRange(extraProcessors);

        var fake = new FakeClusterApiClient();
        var service = new JobInstanceService(
            NullLogger<JobInstanceService>.Instance,
            Options.Create(config),
            fake,
            new JobProcessorRegistry(processors));

        return (service, config, fake);
    }

    private static async Task WaitUntilAsync(Func<bool> condition, TimeSpan timeout)
    {
        var start = DateTime.UtcNow;
        while (DateTime.UtcNow - start < timeout)
        {
            if (condition())
            {
                return;
            }

            await Task.Delay(20);
        }

        Assert.True(condition(), "condition not met before timeout");
    }

    private sealed class SlowProcessor : IJobProcessor
    {
        public string Name => "slowProcessor";

        public async Task<ProcessResult> ProcessAsync(JobContext context)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), context.CancellationToken);
            return ProcessResult.Ok("done");
        }
    }
}
