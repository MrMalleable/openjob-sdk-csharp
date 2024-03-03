namespace openjob_sdk_csharp_agent.common.request.cluster
{
    public class ClusterHandleTaskRequest
    {
        /// <summary>
        /// 实例任务状态列表
        /// </summary>
        public IList<HandleTaskItem>? TaskRequestList { get; set; }
    }

    /// <summary>
    /// 实例任务状态
    /// </summary>
    public class HandleTaskItem
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public long? JobId { get; set; }
        /// <summary>
        /// 任务实例ID
        /// </summary>
        public long? JobInstanceId { get; set; }
        /// <summary>
        /// 任务调度版本
        /// </summary>
        public long? DispatchVersion { get; set; }
        /// <summary>
        /// 实例任务唯一标识
        /// </summary>
        public string? TaskId { get; set; }
        /// <summary>
        /// 父任务，默认传 0_0_0_0_0
        /// </summary>
        public string? ParentTaskId { get; set; }
        /// <summary>
        /// 自定义任务名称，可为空
        /// </summary>
        public string? TaskName { get; set; }
        /// <summary>
        /// 实例任务状态
        /// </summary>
        public long? Status { get; set; }
        /// <summary>
        /// 任务成功或失败信息，可为空
        /// </summary>
        public string? Result { get; set; }
        /// <summary>
        /// 客户端地址
        /// </summary>
        public string? WorkerAddress { get; set; }
        /// <summary>
        /// 任务创建时间,时间戳
        /// </summary>
        public long? CreateTime { get; set; }
        /// <summary>
        /// 任务更新时间,时间戳
        /// </summary>
        public long? UpdateTime { get; set; }
    }
}
