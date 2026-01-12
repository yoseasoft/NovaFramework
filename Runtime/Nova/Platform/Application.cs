/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using UnityApplication = UnityEngine.Application;

namespace NovaEngine
{
    /// <summary>
    /// 系统级的应用管理类，用于定义应用程序的系统级访问接口
    /// </summary>
    public partial class Application : Singleton<Application>
    {
        /// <summary>
        /// 应用程序协议转换的处理函数句柄定义
        /// </summary>
        /// <param name="protocolType">应用程序协议类型</param>
        public delegate void ApplicationProtocolTransformationHandler(ProtocolType protocolType);

        /// <summary>
        /// 应用程序协议转换的处理回调函数
        /// </summary>
        private static ApplicationProtocolTransformationHandler _protocolTransformationCallback;

        /// <summary>
        /// 应用管理对象实例的初始化回调接口
        /// </summary>
        protected override void Initialize()
        {
        }

        /// <summary>
        /// 应用管理对象实例的清理回调接口
        /// </summary>
        protected override void Cleanup()
        {
        }

        /// <summary>
        /// 应用管理对象实例启动函数
        /// </summary>
        /// <returns>当对象实例启动成功时返回true，否则返回false</returns>
        public bool Startup()
        {
            Platform.PlatformType platformType = Platform.Instance.TargetPlatformType();
            if (Platform.PlatformType.Unknown == platformType)
            {
                Logger.Error("Could not found any available platform on target application.");
                return false;
            }

            // 设备模块启动
            if (false == Device.Instance.Startup())
            {
                Logger.Error("The device instance startup failed.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 应用管理对象实例关闭函数
        /// </summary>
        public void Shutdown()
        {
            // 设备模块关闭
            Device.Instance.Shutdown();

            Platform.Destroy();
        }

        #region 应用程序运行环境相关处理操作接口

        /// <summary>
        /// 检查当前运行程序是否处于真实的设备环境
        /// </summary>
        /// <returns>若程序运行在真实设备上则返回true，否则返回false</returns>
        public static bool IsPlayer()
        {
            return !UnityApplication.isEditor;
        }

        /// <summary>
        /// 检查当前运行程序是否处于编辑器环境
        /// </summary>
        /// <returns>若程序运行在编辑器上则返回true，否则返回false</returns>
        public static bool IsEditor()
        {
            return UnityApplication.isEditor;
        }

        #endregion

        #region 应用程序激活/失效相关通知回调接口

        //
        // 正常进入游戏：
        // OnApplicationDidFinishLaunching
        // OnApplicationDidBecomeActive
        //
        // 正常退出游戏：
        // OnApplicationWillTerminate
        //
        // Home键退出游戏：
        // OnApplicationWillResignActive
        // OnApplicationDidEnterBackground
        //
        // Home键返回游戏：
        // OnApplicationWillEnterForeground
        // OnApplicationDidBecomeActive
        //

        /// <summary>
        /// 当应用程序进入非活动状态执行，在此期间应用程序不接收消息或事件
        /// </summary>
        internal void OnApplicationWillResignActive()
        {
            OnProtocolTransformationDispatch(ProtocolType.ResignActive);
        }

        /// <summary>
        /// 当应用程序进入活动状态执行，在此期间应用程序恢复接收消息或事件功能
        /// </summary>
        internal void OnApplicationDidBecomeActive()
        {
            OnProtocolTransformationDispatch(ProtocolType.BecomeActive);
        }

        /// <summary>
        /// 当程序被推送到后台的时候调用
        /// </summary>
        internal void OnApplicationDidEnterBackground()
        {
            OnProtocolTransformationDispatch(ProtocolType.EnterBackground);
        }

        /// <summary>
        /// 当程序从后台重新回到前台时候调用
        /// </summary>
        internal void OnApplicationWillEnterForeground()
        {
            OnProtocolTransformationDispatch(ProtocolType.EnterForeground);
        }

        /// <summary>
        /// 当程序正确载入时被调用
        /// </summary>
        internal void OnApplicationDidFinishLaunching()
        {
            OnProtocolTransformationDispatch(ProtocolType.FinishLaunching);
        }

        /// <summary>
        /// 当程序将要退出时被调用，通常是用来保存数据和一些退出前的清理工作
        /// </summary>
        internal void OnApplicationWillTerminate()
        {
            OnProtocolTransformationDispatch(ProtocolType.Terminate);
        }

        #endregion

        #region 应用程序协议转换通知转发相关函数接口

        /// <summary>
        /// 新增应用程序协议转换的处理回调接口
        /// </summary>
        /// <param name="handler">协议转换句柄函数</param>
        public void AddProtocolTransformationHandler(ApplicationProtocolTransformationHandler handler)
        {
            _protocolTransformationCallback += handler;
        }

        /// <summary>
        /// 移除指定的应用程序协议转换的处理回调接口
        /// </summary>
        /// <param name="handler">协议转换句柄函数</param>
        public void RemoveProtocolTransformationHandler(ApplicationProtocolTransformationHandler handler)
        {
            _protocolTransformationCallback -= handler;
        }

        /// <summary>
        /// 通过当前应用对象已经注册的所有回调处理句柄函数，对指定的协议类型进行转发通知
        /// </summary>
        /// <param name="protocolType">协议类型</param>
        private void OnProtocolTransformationDispatch(ProtocolType protocolType)
        {
            _protocolTransformationCallback?.Invoke(protocolType);
        }

        #endregion

        /// <summary>
        /// 使用当前应用对象打开指定的URL网络链接
        /// </summary>
        /// <param name="url">URL地址</param>
        /// <returns>若打开网络链接成功返回true，否则返回false</returns>
        public bool OpenURL(string url)
        {
            return false;
        }
    }
}
