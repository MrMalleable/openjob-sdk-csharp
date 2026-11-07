using openjob_sdk_csharp_agent.common.helper;
using Xunit;

namespace openjob_sdk_csharp_agent.Tests;

public class ApiResponseHelperTests
{
    [Fact]
    public void BuildSuccessResponse_ShouldUseUnixMillisServerTime()
    {
        var before = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var response = ApiResponseHelper<string>.BuildSuccessResponse("ok");
        var after = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        Assert.Equal(200, response.Status);
        Assert.Equal(0, response.Code);
        Assert.Equal("ok", response.Data);
        Assert.InRange(response.ServerTime!.Value, before, after);
    }
}
