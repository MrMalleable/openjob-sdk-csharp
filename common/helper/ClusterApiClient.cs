using openjob_sdk_csharp_agent.common.request;
using openjob_sdk_csharp_agent.common.response;
using System.Text;
using Newtonsoft.Json;
using openjob_sdk_csharp_agent.common.constant;
using openjob_sdk_csharp_agent.common.request.cluster;

namespace openjob_sdk_csharp_agent.common.helper
{
    /// <summary>
    /// 集群通信客户端
    /// 描述：和Server进行通信
    /// </summary>
    public class ClusterApiClient
    {
        // 服务端请求地址
        private string? host { get; set; }

        // 请求token
        // 管理后台 => 个人信息 查看当前登录账号安全秘钥。
        // 请求 API 时，头部新增 Header(Token:安全秘钥)

        private HttpClient httpClient;

        private readonly ILogger<ClusterApiClient> _logger;

        public ClusterApiClient(ILogger<ClusterApiClient> _logger, string? host, string? token)
        {
            Console.WriteLine("ClusterApiClient初始化");
            this._logger = _logger;
            this.host = host;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("token", token);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        /// <summary>
        /// 上线客户端
        /// </summary>
        /// <param name="address">客户端HTTP地址</param>
        /// <param name="appName">应用名称</param>
        /// <param name="version">当前客户端版本</param>
        /// <param name="workerKey">客户端唯一标识</param>
        /// <returns></returns>
        public OpenjobApiResponse<OpenjobApiClusterStartResponse>? RegisterOn(
            string address, 
            string appName, 
            string version, 
            string workerKey)
        {
            try
            {
                var requestBody = new
                {
                    address,
                    appName,
                    version,
                    workerKey
                };
                var response = PostJson(ClusterApiConstant.START_API, requestBody);
                return JsonConvert.DeserializeObject<OpenjobApiResponse<OpenjobApiClusterStartResponse>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegisterOnError");
                return default;
            }
        }


        /// <summary>
        /// 客户端心跳
        /// </summary>
        /// <param name="address">客户端HTTP地址</param>
        /// <param name="appId">应用编号ID</param>
        /// <param name="appName">应用名称</param>
        /// <param name="runningJobInstanceIds">所有正在运行的任务实例ID</param>
        /// <param name="version">当前客户端版本</param>
        /// <returns></returns>
        public OpenjobApiResponse<OpenjobApiClusterHeartBeatResponse>? HeartBeat(
            string? address,
            string? appId,
            string? appName,
            IList<long>? runningJobInstanceIds,
            string? version
            )
        {
            try
            {
                var requestBody = new
                {
                    address,
                    appId,
                    appName,
                    runningJobInstanceIds,
                    version
                };
                var response = PostJson(ClusterApiConstant.HEARTBEAT_API, requestBody);
                return JsonConvert.DeserializeObject<OpenjobApiResponse<OpenjobApiClusterHeartBeatResponse>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HeartBeatError");
                return default;
            }
        }

        /// <summary>
        /// 下线客户端
        /// </summary>
        /// <param name="address">客户端HTTP地址</param>
        /// <param name="appName">应用名称</param>
        /// <param name="workerKey">客户端唯一标识</param>
        /// <returns></returns>
        public OpenjobApiResponse<object>? RegisterOff(
            string address,
            string appName,
            string workerKey)
        {
            try
            {
                var requestBody = new
                {
                    address,
                    appName,
                    workerKey
                };
                var response = PostJson(ClusterApiConstant.STOP_API, requestBody);
                return JsonConvert.DeserializeObject<OpenjobApiResponse<object>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegisterOffError");
                return default;
            }
        }

        /// <summary>
        /// 上传任务日志
        /// 日志选项列表:
        ///     time 格式化时间 yyyy-MM-dd HH:mm:ss.SSS
        ///     time_stamp 毫秒时间戳
        ///     message 日志内容
        ///     level 日志级别(INFO/WARN/ERROR)
        ///     location 日志产生位置(org.springframework.web.servlet.DispatcherServlet.doService(DispatcherServlet.java:943))
        ///     throwable 错误日志堆栈信息，可为空
        ///     jobId 日志任务ID
        ///     job_instance_id 日志任务实例ID
        ///     job_dispatch_version 任务实例调度执行版本号
        ///     job_instance_taskId 任务实例执行唯一标识(实例执行任务ID)
        /// </summary>
        /// <param name="fieldList">任务日志选项列表</param>
        /// <returns></returns>
        public OpenjobApiResponse<OpenjobApiClusterStartResponse>? UploadTaskLog(IList<UploadLogField> fieldList)
        {
            try
            {
                var requestBody = new
                {
                    fieldList,
                };
                var response = PostJson(ClusterApiConstant.UPLOAD_LOG_API, requestBody);
                return JsonConvert.DeserializeObject<OpenjobApiResponse<OpenjobApiClusterStartResponse>>(response);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "UploadTaskLogError");
                return default;
            }
        }

        /// <summary>
        /// 实例任务状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public OpenjobApiResponse<object>? HandleTasks(ClusterHandleTaskRequest request)
        {
            try
            {
                var response = PostJson(ClusterApiConstant.HANDLE_TASKS_API, request);
                return JsonConvert.DeserializeObject<OpenjobApiResponse<object>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HandleTasksError");
                return default;
            }
        }

        /// <summary>
        /// 上报实例状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public OpenjobApiResponse<object>? HandleStatus(ClusterHandleStatusRequest request)
        {
            try
            {
                var response = PostJson(ClusterApiConstant.HANDLE_STATUS_API, request);
                return JsonConvert.DeserializeObject<OpenjobApiResponse<object>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HandleStatusError");
                return default;
            }
        }

        // 发送post请求
        private string PostJson(string url, object body)
        {
            var requestUrl = this.host + url;
            Console.WriteLine("请求地址为：" + requestUrl);
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(requestUrl, content).Result;
            var resultStr = response.Content.ReadAsStringAsync().Result;
            _logger.LogInformation("requestUrl: "+requestUrl+", response: "+ resultStr);
            return resultStr;
        }
    }
}
