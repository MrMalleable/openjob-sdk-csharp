namespace openjob_sdk_csharp_agent.common.constant
{
    public class ClusterApiConstant
    {
        // 上线客户端接口
        public static readonly string START_API = "/openapi/worker/start";

        // 上线客户端接口
        public static readonly string HEARTBEAT_API = "/openapi/worker/heartbeat";

        // 上线客户端接口
        public static readonly string STOP_API = "/openapi/worker/stop";

        // 上线客户端接口
        public static readonly string UPLOAD_LOG_API = "/openapi/task-log/batch-add";

        // 上报实例状态接口
        public static readonly string HANDLE_STATUS_API = "/openapi/job-instance/handle-status";

        // 实例任务状态接口
        public static readonly string HANDLE_TASKS_API = "/openapi/job-instance/handle-tasks";

    }
}
