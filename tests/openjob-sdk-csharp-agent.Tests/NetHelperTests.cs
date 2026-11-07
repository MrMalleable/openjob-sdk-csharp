using openjob_sdk_csharp_agent.common.helper;
using Xunit;

namespace openjob_sdk_csharp_agent.Tests;

public class NetHelperTests
{
    [Fact]
    public void GetLocalIp_ShouldNotThrow_AndPreferIpv4WhenPresent()
    {
        var ip = NetHelper.GetLocalIp();

        if (ip != null)
        {
            Assert.DoesNotContain(':', ip); // IPv4 不含冒号
            Assert.False(string.IsNullOrWhiteSpace(ip));
        }
    }
}
