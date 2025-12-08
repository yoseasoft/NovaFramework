/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using System;
using System.Runtime.CompilerServices;

namespace GameEngine
{
    // API接口管理工具类
    public static partial class GameApi
    {
        #region 热加载模块动态绑定相关的接口函数

        /// <summary>
        /// 注册指定类型的热加载模块对象到当前管理句柄中
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        [Obsolete]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RegisterHotModule<T>() where T : IHotModule, new()
        {
            HotModuleManager.RegisterHotModule<T>();
        }

        /// <summary>
        /// 注册指定类型的热加载模块对象到当前管理句柄中
        /// </summary>
        /// <param name="type">对象类型</param>
        [Obsolete]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RegisterHotModule(Type type)
        {
            HotModuleManager.RegisterHotModule(type);
        }

        /// <summary>
        /// 自动注册应用上下文配置的所有热加载模块对象
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AutoRegisterAllHotModulesOfContextConfigure()
        {
            HotModuleManager.AutoRegisterAllHotModulesOfContextConfigure();
        }

        /// <summary>
        /// 从当前管理句柄中注销指定类型的热加载模块对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        [Obsolete]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnregisterHotModule<T>() where T : IHotModule
        {
            HotModuleManager.UnregisterHotModule<T>();
        }

        /// <summary>
        /// 从当前管理句柄中注销指定类型的热加载模块对象实例
        /// </summary>
        /// <param name="type">对象类型</param>
        [Obsolete]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnregisterHotModule(Type type)
        {
            HotModuleManager.UnregisterHotModule(type);
        }

        /// <summary>
        /// 自动注销应用上下文配置的所有热加载模块对象
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AutoUnregisterAllHotModulesOfContextConfigure()
        {
            HotModuleManager.AutoUnregisterAllHotModulesOfContextConfigure();
        }

        #endregion

        #region 框架层的网络模块相关的注册绑定接口

        /// <summary>
        /// 注册指定服务类型对应的消息解析器对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="serviceType">服务类型</param>
        /// <returns>若注册解析器对象实例成功则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RegisterMessageTranslator<T>(int serviceType) where T : IMessageTranslator, new()
        {
            return NetworkHandler.Instance.RegMessageTranslator<T>(serviceType);
        }

        /// <summary>
        /// 注册指定服务类型对应的消息解析器对象实例
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="classType">对象类型</param>
        /// <returns>若注册解析器对象实例成功则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RegisterMessageTranslator(int serviceType, Type classType)
        {
            return NetworkHandler.Instance.RegMessageTranslator(serviceType, classType);
        }

        /// <summary>
        /// 删除指定服务类型对应的消息解析器对象实例
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnregisterMessageTranslator(int serviceType)
        {
            NetworkHandler.Instance.UnregMessageTranslator(serviceType);
        }

        /// <summary>
        /// 设置网络消息的协议对象类型
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetMessageProtocolType<T>() where T : class
        {
            NetworkHandler.Instance.SetMessageProtocolType<T>();
        }

        /// <summary>
        /// 设置网络消息的协议对象类型
        /// </summary>
        /// <param name="classType">对象类型</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetMessageProtocolType(Type classType)
        {
            NetworkHandler.Instance.SetMessageProtocolType(classType);
        }

        #endregion

        #region 框架层的视图模块相关的注册绑定接口

        /// <summary>
        /// 注册指定的窗口管理器到当前的主控制器中
        /// </summary>
        /// <typeparam name="T">管理器类型</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RegisterFormManager<T>() where T : IFormManager, new()
        {
            GuiHandler.Instance.RegisterFormManager<T>();
        }

        /// <summary>
        /// 注册指定的窗口管理器到当前的主控制器中
        /// </summary>
        /// <param name="classType">管理器类型</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RegisterFormManager(Type classType)
        {
            GuiHandler.Instance.RegisterFormManager(classType);
        }

        /// <summary>
        /// 注册指定的窗口管理器到当前的主控制器中
        /// </summary>
        /// <param name="formManager">管理器对象</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RegisterFormManager(IFormManager formManager)
        {
            GuiHandler.Instance.RegisterFormManager(formManager);
        }

        /// <summary>
        /// 从主控制器中移除当前实施的
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnregisterCurrentFormManager()
        {
            GuiHandler.Instance.UnregisterCurrentFormManager();
        }

        #endregion
    }
}
