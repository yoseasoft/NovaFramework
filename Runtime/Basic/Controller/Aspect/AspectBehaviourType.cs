/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

namespace GameEngine
{
    /// <summary>
    /// 可用切点访问的行为类型函数的枚举定义
    /// </summary>
    public enum AspectBehaviourType : uint
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = 0x00,

        /// <summary>
        /// 初始化服务节点<br/>
        /// 供引擎内部流程使用的调度行为
        /// </summary>
        Initialize = 0x0001,

        /// <summary>
        /// 启动服务节点<br/>
        /// 供引擎内部流程使用的调度行为
        /// </summary>
        Startup = 0x0002,

        /// <summary>
        /// 唤醒服务节点<br/>
        /// 供业务层使用的调度行为
        /// </summary>
        Awake = 0x0004,

        /// <summary>
        /// 开始服务节点<br/>
        /// 供业务层使用的调度行为
        /// </summary>
        Start = 0x0008,

        /// <summary>
        /// 更新服务节点
        /// </summary>
        Update = 0x0010,

        /// <summary>
        /// 延迟更新服务节点
        /// </summary>
        LateUpdate = 0x0020,

        /// <summary>
        /// 销毁服务节点<br/>
        /// 供业务层使用的调度行为
        /// </summary>
        Destroy = 0x0100,

        /// <summary>
        /// 关闭服务节点<br/>
        /// 供引擎内部流程使用的调度行为
        /// </summary>
        Shutdown = 0x0200,

        /// <summary>
        /// 清理服务节点<br/>
        /// 供引擎内部流程使用的调度行为
        /// </summary>
        Cleanup = 0x0400,
    }
}
