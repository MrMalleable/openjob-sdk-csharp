using openjob_sdk_csharp_agent.common.helper;

namespace openjob_sdk_csharp_agent.config
{
    /// <summary>
    /// Openjob相关配置
    /// </summary>
    public class OpenJobConfig
    {
        /// <summary>
        /// 集群地址
        /// </summary>
        public string? ClusterHost { get; set; }
        /// <summary>
        /// 心跳间隔
        /// </summary>
        public int? HeartBeatInterval { get; set; }
        /// <summary>
        /// 运行Ip
        /// </summary>
        public string? Host { get; set; }

        /// <summary>
        /// 启动端口
        /// </summary>
        public string? Port { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string? AppName { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        public string? AppId { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// 请求token
        /// </summary>
        public string? Token { get; set; }

        public static void GetConfiguration(IConfiguration configuration, OpenJobConfig config)
        {
            if(configuration == null)
            {
                return;
            }

            string? clusterHost = configuration.GetValue<string>("OpenJob:ClusterHost");
            int? heartBeatInterval = configuration.GetValue<int>("OpenJob:HeartBeatInterval");
            if(!heartBeatInterval.HasValue || heartBeatInterval.Value < 0)
            {
                heartBeatInterval = 5;
            }
            string? host = configuration.GetValue<string>("OpenJob:Host");
            if (string.IsNullOrEmpty(host))
            {
                host = NetHelper.GetLocalIp();
            }
            string? port = configuration.GetValue<string>("OpenJob:Port");
            string? appName = configuration.GetValue<string>("OpenJob:AppName");
            string? appId = configuration.GetValue<string>("OpenJob:AppId");
            string? version = configuration.GetValue<string>("OpenJob:Version");
            string? token = configuration.GetValue<string>("OpenJob:Token");

            config.ClusterHost = clusterHost;
            config.HeartBeatInterval = heartBeatInterval;
            config.Host = host;
            config.Port = port;
            config.AppName = appName;
            config.AppId = appId;
            config.Version = version;
            config.Token = token;

        }
    }
}
