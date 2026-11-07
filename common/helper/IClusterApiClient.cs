using openjob_sdk_csharp_agent.common.request;
using openjob_sdk_csharp_agent.common.request.cluster;
using openjob_sdk_csharp_agent.common.response;

namespace openjob_sdk_csharp_agent.common.helper
{
    /// <summary>
    /// 集群通信客户端抽象，便于测试与替换实现
    /// </summary>
    public interface IClusterApiClient
    {
        Task<OpenjobApiResponse<OpenjobApiClusterStartResponse>?> RegisterOnAsync(
            CancellationToken cancellationToken = default);

        Task<OpenjobApiResponse<OpenjobApiClusterHeartBeatResponse>?> HeartBeatAsync(
            IList<long> runningJobInstanceIds,
            CancellationToken cancellationToken = default);

        Task<OpenjobApiResponse<object>?> RegisterOffAsync(
            CancellationToken cancellationToken = default);

        Task<OpenjobApiResponse<object>?> UploadTaskLogAsync(
            IList<UploadLogField> fieldList,
            CancellationToken cancellationToken = default);

        Task<OpenjobApiResponse<object>?> HandleTasksAsync(
            ClusterHandleTaskRequest request,
            CancellationToken cancellationToken = default);

        Task<OpenjobApiResponse<object>?> HandleStatusAsync(
            ClusterHandleStatusRequest request,
            CancellationToken cancellationToken = default);
    }
}
