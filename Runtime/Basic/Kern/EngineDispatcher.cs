/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
    public static class EngineDispatcher
    {
        /// <summary>
        /// 应用程序响应回调句柄函数声明
        /// </summary>
        public delegate void ApplicationResponseHandler(NovaEngine.Application.ProtocolType protocolType);

        /// <summary>
        /// 应用程序响应回调句柄函数委托实例
        /// </summary>
        private static ApplicationResponseHandler s_applicationResponseHandler;

        /// <summary>
        /// 应用程序正式启动的状态标识
        /// </summary>
        private static bool s_isOnStartup = false;

        /// <summary>
        /// 获取当前应用程序正式启动的状态
        /// </summary>
        public static bool IsOnStartup => s_isOnStartup;

        /// <summary>
        /// 程序启动事件转发通知函数
        /// </summary>
        private static void OnDispatchingStartup()
        {
            s_isOnStartup = true;

            OnApplicationResponseCallback(NovaEngine.Application.ProtocolType.Startup);
        }

        /// <summary>
        /// 程序关闭事件转发通知函数
        /// </summary>
        private static void OnDispatchingShutdown()
        {
            OnApplicationResponseCallback(NovaEngine.Application.ProtocolType.Shutdown);

            s_isOnStartup = false;
        }

        /// <summary>
        /// 程序固定刷新事件转发通知函数
        /// </summary>
        internal static void OnDispatchingFixedUpdate() { OnApplicationResponseCallback(NovaEngine.Application.ProtocolType.FixedUpdate); }

        /// <summary>
        /// 程序刷新事件转发通知函数
        /// </summary>
        internal static void OnDispatchingUpdate() { OnApplicationResponseCallback(NovaEngine.Application.ProtocolType.Update); }

        /// <summary>
        /// 程序后置刷新事件转发通知函数
        /// </summary>
        internal static void OnDispatchingLateUpdate() { OnApplicationResponseCallback(NovaEngine.Application.ProtocolType.LateUpdate); }

        /// <summary>
        /// 关于定时调度事件的转发通知函数
        /// </summary>
        /// <param name="protocolType"></param>
        internal static void OnDispatchingForTimeScheduled(NovaEngine.Application.ProtocolType protocolType)
        {
            OnApplicationResponseCallback(protocolType);
        }

        /// <summary>
        /// 应用程序响应回调处理函数
        /// </summary>
        internal static void OnApplicationResponseCallback(NovaEngine.Application.ProtocolType protocolType)
        {
            // Debugger.Log("Call Application Protocol Type: {%s}", protocolType.ToString());

            s_applicationResponseHandler?.Invoke(protocolType);
        }

        /// <summary>
        /// 新增应用程序响应回调的函数接口
        /// </summary>
        private static void AddApplicationResponseHandler(ApplicationResponseHandler handler)
        {
            s_applicationResponseHandler += handler;
        }

        /// <summary>
        /// 移除应用程序响应回调的函数接口
        /// </summary>
        private static void RemoveApplicationResponseHandler(ApplicationResponseHandler handler)
        {
            s_applicationResponseHandler -= handler;
        }

        /// <summary>
        /// 应用层启动回调处理的函数接口
        /// </summary>
        /// <param name="handler">响应句柄</param>
        public static void OnApplicationStartup(ApplicationResponseHandler handler)
        {
            Debugger.Assert(false == s_isOnStartup);

            AddApplicationResponseHandler(handler);

            OnDispatchingStartup();
        }

        /// <summary>
        /// 应用层关闭回调处理的函数接口
        /// </summary>
        /// <param name="handler">响应句柄</param>
        public static void OnApplicationShutdown(ApplicationResponseHandler handler)
        {
            Debugger.Assert(s_isOnStartup);

            OnDispatchingShutdown();

            RemoveApplicationResponseHandler(handler);
        }
    }
}
