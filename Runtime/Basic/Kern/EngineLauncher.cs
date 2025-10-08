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

using System.Collections.Generic;

namespace GameEngine
{
    /// <summary>
    /// 游戏世界总控管理器对象类
    /// </summary>
    internal static class EngineLauncher
    {
        /// <summary>
        /// 世界对象构建函数
        /// </summary>
        public static void OnCreate(object controller, IReadOnlyDictionary<string, string> variables)
        {
            // 初始化环境配置
            EngineConfigure.InitGameConfig(variables);

            // 创建程序总控实例
            if (typeof(UnityEngine.MonoBehaviour).IsAssignableFrom(controller.GetType()))
            {
                NovaEngine.AppEntry.Create(controller as UnityEngine.MonoBehaviour);
            }
            else
            {
                NovaEngine.AppEntry.Create();
            }
        }

        /// <summary>
        /// 世界对象销毁函数
        /// </summary>
        private static void OnDestroy()
        {
            // 销毁程序总控实例
            NovaEngine.AppEntry.Destroy();

            // 清理环境配置
            EngineConfigure.CleanupGameConfig();
        }

        /// <summary>
        /// 世界对象重载函数
        /// </summary>
        /// <param name="type">类型标识</param>
        public static void OnReload(int type)
        {
            // 重载业务层模块
            GameCall.Reload(type);
        }

        /// <summary>
        /// 世界对象启动函数
        /// </summary>
        private static void OnStart()
        {
            OnApplicationLaunching();

            // 总控实例启动
            NovaEngine.AppEntry.Startup();

            // 控制器启动
            ControllerManagement.Startup();

            // 句柄管理器启动
            HandlerManagement.Startup();

            // 应用上下文启动
            ApplicationContext.Startup();

            // 启动游戏业务层
            GameCall.Startup();

            // 广播应用启动的事件通知
            // 已调整到业务层主动调用，因为其内部绑定的回调监听是在业务层启动时绑定的
            // EngineDispatcher.OnDispatchingStartup();
        }

        /// <summary>
        /// 世界对象关闭函数
        /// </summary>
        private static void OnShutdown()
        {
            // 广播应用关闭的事件通知
            // 已调整到业务层主动调用，因为其内部绑定的回调监听是在业务层关闭时卸载的
            // EngineDispatcher.OnDispatchingShutdown();

            // 关闭游戏业务层
            GameCall.Shutdown();

            // 应用上下文关闭
            ApplicationContext.Shutdown();

            // 句柄管理器关闭
            HandlerManagement.Shutdown();

            // 控制器关闭
            ControllerManagement.Shutdown();

            // 总控实例关闭
            NovaEngine.AppEntry.Shutdown();
        }

        /// <summary>
        /// 世界对象固定刷新函数
        /// </summary>
        private static void OnFixedUpdate()
        {
            InternalFixedUpdate();
        }

        /// <summary>
        /// 世界对象刷新函数
        /// </summary>
        private static void OnUpdate()
        {
            InternalUpdate();
        }

        /// <summary>
        /// 世界对象延迟刷新函数
        /// </summary>
        private static void OnLateUpdate()
        {
            InternalLateUpdate();
        }

        #region 管理器内部调用的执行/刷新回调接口

        /// <summary>
        /// 管理器内部固定执行函数
        /// </summary>
        private static void InternalFixedExecute()
        {
        }

        /// <summary>
        /// 管理器内部执行函数
        /// </summary>
        private static void InternalExecute()
        {
            // 总控实例执行
            NovaEngine.AppEntry.Execute();

            // 控制器管理对象执行
            ControllerManagement.Execute();
            // 管理句柄执行
            HandlerManagement.Execute();

            // 外部通知执行
            EngineDispatcher.OnDispatchingExecute();
        }

        /// <summary>
        /// 管理器内部延迟执行函数
        /// </summary>
        private static void InternalLateExecute()
        {
            // 总控实例后置执行
            NovaEngine.AppEntry.LateExecute();

            // 控制器管理对象后置执行
            ControllerManagement.LateExecute();
            // 管理句柄后置执行
            HandlerManagement.LateExecute();

            // 外部通知后置执行
            EngineDispatcher.OnDispatchingLateExecute();
        }

        /// <summary>
        /// 管理器内部固定刷新函数
        /// </summary>
        private static void InternalFixedUpdate()
        {
        }

        /// <summary>
        /// 管理器内部刷新函数
        /// </summary>
        private static void InternalUpdate()
        {
            // 总控实例刷新
            NovaEngine.AppEntry.Update();

            // 控制器管理对象刷新
            ControllerManagement.Update();
            // 管理句柄刷新
            HandlerManagement.Update();

            // 外部通知刷新
            EngineDispatcher.OnDispatchingUpdate();
        }

        /// <summary>
        /// 管理器内部延迟刷新函数
        /// </summary>
        private static void InternalLateUpdate()
        {
            // 总控实例后置刷新
            NovaEngine.AppEntry.LateUpdate();

            // 控制器管理对象后置刷新
            ControllerManagement.LateUpdate();
            // 管理句柄后置刷新
            HandlerManagement.LateUpdate();

            // 外部通知后置刷新
            EngineDispatcher.OnDispatchingLateUpdate();

            // 控制器管理对象倾泻
            ControllerManagement.Dump();
        }

        #endregion

        private static void OnApplicationFocus(bool focus)
        {
            NovaEngine.CFrameworkController.OnApplicationFocus(focus);
        }

        private static void OnApplicationPause(bool pause)
        {
            NovaEngine.CFrameworkController.OnApplicationPause(pause);
        }

        private static void OnApplicationLaunching()
        {
            NovaEngine.CFrameworkController.OnApplicationLaunching();
        }

        private static void OnApplicationQuit()
        {
            // 引擎实例关闭
            OnShutdown();

            NovaEngine.CFrameworkController.OnApplicationQuit();
        }
    }
}
