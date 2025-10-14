/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
        /// 开启标签
        /// </summary>
        OpeningBegan = 0x0100,

        /// <summary>
        /// 初始化服务节点<br/>
        /// 供引擎内部流程使用的调度行为
        /// </summary>
        Initialize = 0x0101,

        /// <summary>
        /// 启动服务节点<br/>
        /// 供引擎内部流程使用的调度行为
        /// </summary>
        Startup = 0x0102,

        /// <summary>
        /// 唤醒服务节点<br/>
        /// 供业务层使用的调度行为
        /// </summary>
        Awake = 0x0104,

        /// <summary>
        /// 开始服务节点<br/>
        /// 供业务层使用的调度行为
        /// </summary>
        Start = 0x0108,

        /// <summary>
        /// 打开结束标签
        /// </summary>
        OpeningEnded = 0x0200,

        /// <summary>
        /// 工作标签
        /// </summary>
        WorkingBegan = 0x0200,

        /// <summary>
        /// 逻辑服务节点
        /// </summary>
        Execute = 0x0201,

        /// <summary>
        /// 延迟逻辑服务节点
        /// </summary>
        LateExecute = 0x0202,

        /// <summary>
        /// 更新服务节点
        /// </summary>
        Update = 0x0204,

        /// <summary>
        /// 延迟更新服务节点
        /// </summary>
        LateUpdate = 0x0208,

        /// <summary>
        /// 工作结束标签
        /// </summary>
        WorkingEnded = 0x0300,

        /// <summary>
        /// 关闭标签
        /// </summary>
        ClosingBegan = 0x0400,

        /// <summary>
        /// 销毁服务节点<br/>
        /// 供业务层使用的调度行为
        /// </summary>
        Destroy = 0x0401,

        /// <summary>
        /// 关闭服务节点<br/>
        /// 供引擎内部流程使用的调度行为
        /// </summary>
        Shutdown = 0x0402,

        /// <summary>
        /// 清理服务节点<br/>
        /// 供引擎内部流程使用的调度行为
        /// </summary>
        Cleanup = 0x0404,

        /// <summary>
        /// 关闭结束标签
        /// </summary>
        ClosingEnded = 0x0500,

        /// <summary>
        /// 释放服务节点
        /// </summary>
        Free = 0x1000,
    }
}
