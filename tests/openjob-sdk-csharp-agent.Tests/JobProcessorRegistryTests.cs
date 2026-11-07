using openjob_sdk_csharp_agent.core.processor;
using Xunit;

namespace openjob_sdk_csharp_agent.Tests;

public class JobProcessorRegistryTests
{
    [Fact]
    public void TryGet_ShouldResolveRegisteredProcessor()
    {
        var registry = new JobProcessorRegistry(new[] { new DefaultEchoProcessor() });

        var found = registry.TryGet("defaultEchoProcessor", out var processor);

        Assert.True(found);
        Assert.NotNull(processor);
        Assert.Equal("defaultEchoProcessor", processor!.Name);
    }

    [Fact]
    public async Task DefaultEchoProcessor_ShouldReturnSuccess()
    {
        var processor = new DefaultEchoProcessor();
        var result = await processor.ProcessAsync(new JobContext
        {
            JobId = 1,
            JobInstanceId = 2,
            JobParams = "hello",
            CancellationToken = CancellationToken.None
        });

        Assert.True(result.Success);
        Assert.Contains("hello", result.Result);
    }
}
