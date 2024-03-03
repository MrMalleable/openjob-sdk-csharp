using System.Net;

namespace openjob_sdk_csharp_agent.common.helper
{
    /// <summary>
    /// 网络工具类
    /// </summary>
    public class NetHelper
    {
        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIp()
        {
            // Get the Name of HOST   
            string hostName = Dns.GetHostName();

            // Get the IP from GetHostByName method of dns class. 
            return Dns.GetHostEntry(hostName).AddressList[0].ToString();
        }
    }
}
