/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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

using RecyclableMemoryStreamManager = Microsoft.IO.RecyclableMemoryStreamManager;

namespace NovaEngine.Module
{
    /// <summary>
    /// 网络服务接口管理基类
    /// </summary>
    internal abstract class NetworkService
    {
        /// <summary>
        /// 循环数据流管理对象
        /// </summary>
        private readonly RecyclableMemoryStreamManager _memoryStreamManager = new RecyclableMemoryStreamManager();

        /// <summary>
        /// 获取网络通道的服务类型
        /// </summary>
        public abstract int ServiceType { get; }

        /// <summary>
        /// 获取循环数据流管理对象
        /// </summary>
        public RecyclableMemoryStreamManager MemoryStreamManager
        {
            get { return _memoryStreamManager; }
        }

        /// <summary>
        /// 创建一个默认名称的通道对象实例
        /// </summary>
        /// <returns>返回新创建的通道实例，若实例创建失败则返回null</returns>
        public NetworkChannel CreateChannel()
        {
            return CreateChannel(null, null);
        }

        /// <summary>
        /// 创建一个指定名称和地址的通道对象实例
        /// </summary>
        /// <param name="name">通道名称</param>
        /// <param name="url">通道地址</param>
        /// <returns>返回新创建的通道实例，若实例创建失败则返回null</returns>
        public abstract NetworkChannel CreateChannel(string name, string url);

        /// <summary>
        /// 释放指定标识对应的通道对象实例
        /// </summary>
        /// <param name="channelID">通道标识</param>
        public abstract void ReleaseChannel(int channelID);

        public abstract void Update();

        /// <summary>
        /// 通过指定的网络通道标识在管理器中查找对应的实例
        /// </summary>
        /// <param name="channelID">网络通道对象标识</param>
        /// <returns>返回网络通道对象实例，若查找失败则返回null</returns>
        public NetworkChannel GetChannel(int channelID)
        {
            return NetworkAdapter.GetChannel(channelID);
        }
    }
}
