using Newtonsoft.Json;

namespace openjob_sdk_csharp_agent.common.request.cluster
{
    public class ClusterHandleTaskRequest
    {
        /// <summary>
        /// 实例任务状态列表
        /// </summary>
        [JsonProperty("taskRequestList")]
        public IList<HandleTaskItem>? TaskRequestList { get; set; }
    }

    /// <summary>
    /// 实例任务状态
    /// </summary>
    public class HandleTaskItem
    {
        [JsonProperty("jobId")]
        public long? JobId { get; set; }

        [JsonProperty("jobInstanceId")]
        public long? JobInstanceId { get; set; }

        [JsonProperty("dispatchVersion")]
        public long? DispatchVersion { get; set; }

        [JsonProperty("taskId")]
        public string? TaskId { get; set; }

        [JsonProperty("parentTaskId")]
        public string? ParentTaskId { get; set; }

        [JsonProperty("taskName")]
        public string? TaskName { get; set; }

        [JsonProperty("status")]
        public long? Status { get; set; }

        [JsonProperty("result")]
        public string? Result { get; set; }

        [JsonProperty("workerAddress")]
        public string? WorkerAddress { get; set; }

        [JsonProperty("createTime")]
        public long? CreateTime { get; set; }

        [JsonProperty("updateTime")]
        public long? UpdateTime { get; set; }
    }
}
