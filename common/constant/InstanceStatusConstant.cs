namespace openjob_sdk_csharp_agent.common.constant
{
    /// <summary>
    /// 任务实例状态，对齐 OpenJob InstanceStatusEnum
    /// </summary>
    public static class InstanceStatusConstant
    {
        public const int Waiting = 1;
        public const int Running = 5;
        public const int Success = 10;
        public const int Fail = 15;
        public const int Stop = 20;
        public const int Cancel = 25;
    }
}
