/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
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
    /// 游戏引擎对外提供的API服务统一接口调用封装类，用于对远程游戏业务提供底层API接口封装<br/>
    /// 外部可以通过统一使用该类提供的API进行逻辑处理，避免内部复杂的层次结构导致其它开发人员上手门槛过高<br/>
    /// 同时因为API封装类的存在，内部的具体实现类也可以更好的进行访问权限的控制管理<br/>
    /// 
    /// 该类基本涵盖了所有Handler及Controller提供的服务接口，若有缺失可联系开发人员进行补充
    /// </summary>
    public static partial class GameApi
    {
        /// <summary>
        /// 发送事件消息到事件管理器中等待派发
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件参数列表</param>
        public static void Send(int eventID, params object[] args)
        {
            EventController.Instance.Send(eventID, args);
        }

        /// <summary>
        /// 发送事件消息到事件管理器中等待派发
        /// </summary>
        /// <param name="arg">事件数据</param>
        public static void Send<T>(T arg) where T : struct
        {
            EventController.Instance.Send<T>(arg);
        }

        /// <summary>
        /// 发送事件消息到事件管理器中并立即处理掉它
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件参数列表</param>
        public static void Fire(int eventID, params object[] args)
        {
            EventController.Instance.Fire(eventID, args);
        }

        /// <summary>
        /// 发送事件消息到事件管理器中并立即处理掉它
        /// </summary>
        /// <param name="arg">事件数据</param>
        public static void Fire<T>(T arg) where T : struct
        {
            EventController.Instance.Fire<T>(arg);
        }
    }
}
