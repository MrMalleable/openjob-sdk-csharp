namespace openjob_sdk_csharp_agent.common.constant
{
    /// <summary>
    /// 失败状态，对齐 OpenJob FailStatusEnum
    /// </summary>
    public static class FailStatusConstant
    {
        public const int None = 0;
        public const int ExecuteFail = 1;
        public const int ExecuteTimeout = 5;
        public const int ExecuteDiscard = 10;
        public const int ExecuteRetryTimes = 15;
        public const int ExecuteIgnore = 20;
    }
}
