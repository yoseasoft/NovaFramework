/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
///
/// The above copyright notice and this permission notice shall be included in
/// all copies or substantial portions of the Software.
///
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
/// -------------------------------------------------------------------------------

using System.Net;
using System.Net.NetworkInformation;

using SystemStringBuilder = System.Text.StringBuilder;

namespace NovaEngine
{
    /// <summary>
    /// 实用函数集合工具类
    /// </summary>
    public static partial class Utility
    {
        /// <summary>
        /// 网络相关实用函数集合
        /// </summary>
        public static class Network
        {
            /// <summary>
            /// 检查当前系统的网络是否已处于不可用状态
            /// </summary>
            /// <returns>若当前系统的网络不可用则返回true，否则返回false</returns>
            public static bool IsNotReachableConnectionState()
            {
                return (UnityEngine.Application.internetReachability == UnityEngine.NetworkReachability.NotReachable);
            }

            /// <summary>
            /// 检查当前系统的网络是否处于运营商数据网络的连接模式
            /// </summary>
            /// <returns>若当前系统的网络是处于运营商数据网络的连接模式则返回true，否则返回false</returns>
            public static bool IsCarrierDataConnectionState()
            {
                return (UnityEngine.Application.internetReachability == UnityEngine.NetworkReachability.ReachableViaCarrierDataNetwork);
            }

            /// <summary>
            /// 检查当前系统的网络是否处于WiFi或者有线网络的连接模式
            /// </summary>
            /// <returns>若当前系统的网络是处于WiFi或者有线网络的连接模式则返回true，否则返回false</returns>
            public static bool IsLocalAreaConnectionState()
            {
                return (UnityEngine.Application.internetReachability == UnityEngine.NetworkReachability.ReachableViaLocalAreaNetwork);
            }

            /// <summary>
            /// 通过主机IP和端口信息获取其对应的结构对象
            /// </summary>
            /// <param name="host">主机地址</param>
            /// <param name="port">主机端口</param>
            /// <returns>返回主机结构对象</returns>
            public static IPEndPoint ToIPEndPoint(string host, int port)
            {
                return new IPEndPoint(IPAddress.Parse(host), port);
            }

            /// <summary>
            /// 通过主机IP和端口信息获取其对应的结构对象
            /// </summary>
            /// <param name="address">主机地址信息</param>
            /// <returns>返回主机结构对象</returns>
            public static IPEndPoint ToIPEndPoint(string address)
            {
                int index = address.LastIndexOf(Definition.CCharacter.Colon);
                string host = address.Substring(0, index);
                string p = address.Substring(index + 1);
                int port = int.Parse(p);
                return ToIPEndPoint(host, port);
            }

            /// <summary>
            /// 获取当前网络设备的物理MAC地址
            /// </summary>
            /// <returns>返回当前网络设备的物理MAC地址</returns>
            public static string GetMacAddress()
            {
                string physicalAddress = "";
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                if (null != interfaces)
                {
                    for (int n = 0; n < interfaces.Length; ++n)
                    {
                        NetworkInterface adaper = interfaces[n];
                        if (adaper.Description == "en0")
                        {
                            physicalAddress = adaper.GetPhysicalAddress().ToString();
                            break;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(adaper.GetPhysicalAddress().ToString()))
                            {
                                physicalAddress = adaper.GetPhysicalAddress().ToString();
                                break;
                            };
                        }
                    }
                }

                SystemStringBuilder sb = new SystemStringBuilder();
                for (int n = 0; n < physicalAddress.Length; ++n)
                {
                    sb.Append(physicalAddress[n]);
                    if (n % 2 == 1 && n < (physicalAddress.Length - 1))
                    {
                        sb.Append(Definition.CString.Colon);
                    }
                }

                return sb.ToString();
            }
        }
    }
}
