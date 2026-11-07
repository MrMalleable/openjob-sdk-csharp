using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace openjob_sdk_csharp_agent.common.helper
{
    /// <summary>
    /// 网络工具类
    /// </summary>
    public static class NetHelper
    {
        /// <summary>
        /// 获取本机可用 IPv4 地址，优先非回环网卡
        /// </summary>
        public static string? GetLocalIp()
        {
            try
            {
                foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (networkInterface.OperationalStatus != OperationalStatus.Up)
                    {
                        continue;
                    }

                    if (networkInterface.NetworkInterfaceType is NetworkInterfaceType.Loopback
                        or NetworkInterfaceType.Tunnel)
                    {
                        continue;
                    }

                    var properties = networkInterface.GetIPProperties();
                    foreach (var address in properties.UnicastAddresses)
                    {
                        if (address.Address.AddressFamily == AddressFamily.InterNetwork
                            && !IPAddress.IsLoopback(address.Address))
                        {
                            return address.Address.ToString();
                        }
                    }
                }

                var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
                return hostEntry.AddressList
                    .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork
                                          && !IPAddress.IsLoopback(ip))
                    ?.ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}
