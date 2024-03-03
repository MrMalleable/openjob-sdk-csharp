using openjob_sdk_csharp_agent.common.helper;
using openjob_sdk_csharp_agent.config;

namespace openjob_sdk_csharp_agent.core
{
    /// <summary>
    /// 核心处理类
    /// </summary>
    public class JobInstanceService
    {
        private ILogger<JobInstanceService> _logger;

        private OpenJobConfig _openJobConfig;

        private ClusterApiClient _clusterApiClient;

        public JobInstanceService(ILogger<JobInstanceService> _logger, ILogger<ClusterApiClient> _clusterLogger,
            OpenJobConfig openJobConfig)
        {
            this._logger = _logger;
            this._openJobConfig = openJobConfig;
            this._clusterApiClient = new ClusterApiClient(_clusterLogger, this._openJobConfig.Host, 
                this._openJobConfig.Port);
            // 打印日志
            this._logger.LogInformation("Initial Success!");
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public OpenJobConfig GetConfiguration()
        {
            return this._openJobConfig;
        }

        /// <summary>
        /// 发送心跳
        /// </summary>
        public void SendHeartBeat()
        {
            _clusterApiClient.HeartBeat(_openJobConfig.Host,
                _openJobConfig.AppId,
                _openJobConfig.AppName,
                GetJobInstanceIds(),
                _openJobConfig.Version);
        }

        public IList<long> GetJobInstanceIds()
        {
            // todo
            return new List<long>(0);
        }
    }
}
