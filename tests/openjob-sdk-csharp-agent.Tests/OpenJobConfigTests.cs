using openjob_sdk_csharp_agent.config;
using Xunit;

namespace openjob_sdk_csharp_agent.Tests;

public class OpenJobConfigTests
{
    [Fact]
    public void Normalize_ShouldFillDefaults_AndBuildAddress()
    {
        var config = new OpenJobConfig
        {
            ClusterHost = "http://localhost:8080/",
            AppName = "demo",
            Token = "token",
            Host = "127.0.0.1",
            Port = 0,
            HeartBeatInterval = 0
        };

        config.Normalize();

        Assert.Equal(5, config.HeartBeatInterval);
        Assert.Equal(25588, config.Port);
        Assert.Equal("http://localhost:8080", config.ClusterHost);
        Assert.False(string.IsNullOrWhiteSpace(config.WorkerKey));
        Assert.Equal("127.0.0.1:25588", config.Address);
        Assert.Null(config.Validate());
    }

    [Fact]
    public void Validate_ShouldFail_WhenRequiredMissing()
    {
        var config = new OpenJobConfig();
        config.Normalize();
        Assert.Equal("ClusterHost is not configured", config.Validate());
    }

    [Fact]
    public void Validate_ShouldFail_WhenTokenMissing()
    {
        var config = new OpenJobConfig
        {
            ClusterHost = "http://localhost:8080",
            AppName = "demo",
            Host = "127.0.0.1",
            Port = 8081
        };
        config.Normalize();
        Assert.Equal("Token is not configured", config.Validate());
    }
}
