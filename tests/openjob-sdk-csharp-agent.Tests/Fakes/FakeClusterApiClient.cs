using openjob_sdk_csharp_agent.common.helper;
using openjob_sdk_csharp_agent.common.request;
using openjob_sdk_csharp_agent.common.request.cluster;
using openjob_sdk_csharp_agent.common.response;

namespace openjob_sdk_csharp_agent.Tests.Fakes;

public sealed class FakeClusterApiClient : IClusterApiClient
{
    public List<ClusterHandleStatusRequest> StatusReports { get; } = new();
    public int HeartBeatCount { get; private set; }
    public int RegisterOnCount { get; private set; }
    public int RegisterOffCount { get; private set; }
    public bool RegisterOnSuccess { get; set; } = true;
    public long RegisterAppId { get; set; } = 100;

    public Task<OpenjobApiResponse<OpenjobApiClusterStartResponse>?> RegisterOnAsync(
        CancellationToken cancellationToken = default)
    {
        RegisterOnCount++;
        if (!RegisterOnSuccess)
        {
            return Task.FromResult<OpenjobApiResponse<OpenjobApiClusterStartResponse>?>(
                ApiFail<OpenjobApiClusterStartResponse>("register failed"));
        }

        return Task.FromResult<OpenjobApiResponse<OpenjobApiClusterStartResponse>?>(
            new OpenjobApiResponse<OpenjobApiClusterStartResponse>
            {
                Status = 200,
                Code = 0,
                Message = "OK",
                Data = new OpenjobApiClusterStartResponse
                {
                    AppId = RegisterAppId,
                    AppName = "test-openjob"
                }
            });
    }

    public Task<OpenjobApiResponse<OpenjobApiClusterHeartBeatResponse>?> HeartBeatAsync(
        IList<long> runningJobInstanceIds,
        CancellationToken cancellationToken = default)
    {
        HeartBeatCount++;
        return Task.FromResult<OpenjobApiResponse<OpenjobApiClusterHeartBeatResponse>?>(
            new OpenjobApiResponse<OpenjobApiClusterHeartBeatResponse>
            {
                Status = 200,
                Code = 0,
                Message = "OK",
                Data = new OpenjobApiClusterHeartBeatResponse()
            });
    }

    public Task<OpenjobApiResponse<object>?> RegisterOffAsync(
        CancellationToken cancellationToken = default)
    {
        RegisterOffCount++;
        return Task.FromResult<OpenjobApiResponse<object>?>(
            new OpenjobApiResponse<object> { Status = 200, Code = 0, Message = "OK" });
    }

    public Task<OpenjobApiResponse<object>?> UploadTaskLogAsync(
        IList<UploadLogField> fieldList,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<OpenjobApiResponse<object>?>(
            new OpenjobApiResponse<object> { Status = 200, Code = 0, Message = "OK" });
    }

    public Task<OpenjobApiResponse<object>?> HandleTasksAsync(
        ClusterHandleTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<OpenjobApiResponse<object>?>(
            new OpenjobApiResponse<object> { Status = 200, Code = 0, Message = "OK" });
    }

    public Task<OpenjobApiResponse<object>?> HandleStatusAsync(
        ClusterHandleStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        StatusReports.Add(request);
        return Task.FromResult<OpenjobApiResponse<object>?>(
            new OpenjobApiResponse<object> { Status = 200, Code = 0, Message = "OK" });
    }

    private static OpenjobApiResponse<T> ApiFail<T>(string message) => new()
    {
        Status = 500,
        Code = 0,
        Message = message
    };
}
