namespace openjob_sdk_csharp_agent.core
{
    /// <summary>
    /// 心跳线程
    /// </summary>
    public class HeartBeatThread
    {
        private ILogger<HeartBeatThread> _logger;

        private JobInstanceService _jobInstanceService;

        public HeartBeatThread(ILogger<HeartBeatThread> _logger, JobInstanceService jobInstanceService)
        {
            this._logger = _logger;
            this._jobInstanceService = jobInstanceService;
        }

        // 控制线程是否停止
        private bool _isRunning = false;

        /// <summary>
        /// 开启心跳线程
        /// </summary>
        public void Start()
        {
            _isRunning = true;
            while (true)
            {
                if (_isRunning)
                {
                    break;
                }
                int heartBeatInternal = _jobInstanceService.GetConfiguration().HeartBeatInterval ?? 5;
                // 向集群发送心跳
                _jobInstanceService.SendHeartBeat();
                
                Thread.Sleep(1000 * heartBeatInternal);
            }
            _logger.LogInformation("HeartBeatThread is ended!");
        }

        /// <summary>
        /// 停止线程
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
            _logger.LogInformation("HeartBeatThread is stopped!");
        }
    }
}
