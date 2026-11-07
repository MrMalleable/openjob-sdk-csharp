using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using openjob_sdk_csharp_agent.common.constant;
using openjob_sdk_csharp_agent.common.request;
using openjob_sdk_csharp_agent.common.request.cluster;
using openjob_sdk_csharp_agent.common.response;
using openjob_sdk_csharp_agent.config;

namespace openjob_sdk_csharp_agent.common.helper
{
    /// <summary>
    /// 集群通信客户端：与 OpenJob Server OpenAPI 通信
    /// </summary>
    public class ClusterApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ClusterApiClient> _logger;
        private readonly OpenJobConfig _config;

        public ClusterApiClient(
            HttpClient httpClient,
            ILogger<ClusterApiClient> logger,
            Microsoft.Extensions.Options.IOptions<OpenJobConfig> options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _config = options.Value;

            if (_httpClient.BaseAddress == null && !string.IsNullOrWhiteSpace(_config.ClusterHost))
            {
                _httpClient.BaseAddress = new Uri(_config.ClusterHost.TrimEnd('/') + "/");
            }

            if (!_httpClient.DefaultRequestHeaders.Contains("token")
                && !string.IsNullOrWhiteSpace(_config.Token))
            {
                _httpClient.DefaultRequestHeaders.Add("token", _config.Token);
            }

            if (_httpClient.DefaultRequestHeaders.Accept.Count == 0)
            {
                _httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            }

            if (_httpClient.Timeout == Timeout.InfiniteTimeSpan
                || _httpClient.Timeout == TimeSpan.FromSeconds(100))
            {
                _httpClient.Timeout = TimeSpan.FromSeconds(
                    Math.Max(1, _config.HttpTimeoutSeconds));
            }
        }

        /// <summary>
        /// 上线客户端
        /// </summary>
        public Task<OpenjobApiResponse<OpenjobApiClusterStartResponse>?> RegisterOnAsync(
            CancellationToken cancellationToken = default)
        {
            var requestBody = new
            {
                workerKey = _config.WorkerKey,
                appName = _config.AppName,
                address = _config.Address,
                version = _config.Version,
                protocolType = _config.ProtocolType
            };
            return PostJsonAsync<OpenjobApiClusterStartResponse>(
                ClusterApiConstant.START_API, requestBody, cancellationToken);
        }

        /// <summary>
        /// 客户端心跳
        /// </summary>
        public Task<OpenjobApiResponse<OpenjobApiClusterHeartBeatResponse>?> HeartBeatAsync(
            IList<long> runningJobInstanceIds,
            CancellationToken cancellationToken = default)
        {
            var requestBody = new
            {
                address = _config.Address,
                appId = _config.AppId,
                appName = _config.AppName,
                runningJobInstanceIds,
                version = _config.Version
            };
            return PostJsonAsync<OpenjobApiClusterHeartBeatResponse>(
                ClusterApiConstant.HEARTBEAT_API, requestBody, cancellationToken);
        }

        /// <summary>
        /// 下线客户端
        /// </summary>
        public Task<OpenjobApiResponse<object>?> RegisterOffAsync(
            CancellationToken cancellationToken = default)
        {
            var requestBody = new
            {
                address = _config.Address,
                appName = _config.AppName,
                workerKey = _config.WorkerKey
            };
            return PostJsonAsync<object>(
                ClusterApiConstant.STOP_API, requestBody, cancellationToken);
        }

        /// <summary>
        /// 上传任务日志
        /// </summary>
        public Task<OpenjobApiResponse<object>?> UploadTaskLogAsync(
            IList<UploadLogField> fieldList,
            CancellationToken cancellationToken = default)
        {
            var requestBody = new { fieldList };
            return PostJsonAsync<object>(
                ClusterApiConstant.UPLOAD_LOG_API, requestBody, cancellationToken);
        }

        /// <summary>
        /// 实例任务状态批量上报
        /// </summary>
        public Task<OpenjobApiResponse<object>?> HandleTasksAsync(
            ClusterHandleTaskRequest request,
            CancellationToken cancellationToken = default)
        {
            return PostJsonAsync<object>(
                ClusterApiConstant.HANDLE_TASKS_API, request, cancellationToken);
        }

        /// <summary>
        /// 上报实例状态
        /// </summary>
        public Task<OpenjobApiResponse<object>?> HandleStatusAsync(
            ClusterHandleStatusRequest request,
            CancellationToken cancellationToken = default)
        {
            return PostJsonAsync<object>(
                ClusterApiConstant.HANDLE_STATUS_API, request, cancellationToken);
        }

        private async Task<OpenjobApiResponse<T>?> PostJsonAsync<T>(
            string url,
            object body,
            CancellationToken cancellationToken)
        {
            try
            {
                var content = new StringContent(
                    JsonConvert.SerializeObject(body),
                    Encoding.UTF8,
                    "application/json");

                using var response = await _httpClient
                    .PostAsync(url.TrimStart('/'), content, cancellationToken)
                    .ConfigureAwait(false);
                var resultStr = await response.Content
                    .ReadAsStringAsync(cancellationToken)
                    .ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "OpenJob API non-success. url={Url}, statusCode={StatusCode}, body={Body}",
                        url, (int)response.StatusCode, Truncate(resultStr));
                }
                else
                {
                    _logger.LogDebug(
                        "OpenJob API success. url={Url}, body={Body}",
                        url, Truncate(resultStr));
                }

                return JsonConvert.DeserializeObject<OpenjobApiResponse<T>>(resultStr);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OpenJob API request failed. url={Url}", url);
                return default;
            }
        }

        private static string Truncate(string? value, int maxLength = 512)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            {
                return value ?? string.Empty;
            }

            return value[..maxLength] + "...";
        }
    }
}
