/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemType = System.Type;

using UnityGameObject = UnityEngine.GameObject;
using UnityComponent = UnityEngine.Component;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

namespace NovaEngine
{
    /// <summary>
    /// 应用程序的总入口，提供引擎中绑定组件或调度器的统一管理类<br/>
    /// 建议所有的GameObject实例化都通过该管理类实现，这样可以使用包装后的接口来访问所有的组件实例<br/>
    /// 同时程序中的管理模块，也可以通过该入口进行统一调度规划
    /// </summary>
    public static partial class AppEntry
    {
        /// <summary>
        /// 程序管理器的实例运行状态标识
        /// </summary>
        private static bool s_isRunning = false;

        /// <summary>
        /// 程序调度的根节点对象实例
        /// </summary>
        private static UnityGameObject s_rootGameObject = null;
        /// <summary>
        /// 程序调度的根控制器对象实例
        /// </summary>
        private static UnityMonoBehaviour s_rootController = null;

        /// <summary>
        /// 引擎对象实例
        /// </summary>
        private static Engine s_engine = null;

        /// <summary>
        /// 获取当前调度器运行状态标识
        /// </summary>
        public static bool IsRunning => s_isRunning;
        /// <summary>
        /// 获取当前调度器的根节点对象实例
        /// </summary>
        public static UnityGameObject RootGameObject => s_rootGameObject;
        /// <summary>
        /// 获取当前调度器的根控制器对象实例
        /// </summary>
        public static UnityMonoBehaviour RootController => s_rootController;
        /// <summary>
        /// 获取当前程序的引擎对象实例
        /// </summary>
        public static Engine Engine => s_engine;

        /// <summary>
        /// 总控对象实例创建函数
        /// </summary>
        /// <returns>若实例创建成功返回true，否则返回false</returns>
        public static bool Create()
        {
            Logger.Assert(null == s_engine, "Cannot created one more time.");

            // 创建引擎实例
            s_engine = Engine.Create();

            return true;
        }

        /// <summary>
        /// 总控对象实例创建函数
        /// </summary>
        /// <param name="controller">控制器实例</param>
        /// <returns>若实例创建成功返回true，否则返回false</returns>
        public static bool Create(UnityMonoBehaviour controller)
        {
            if (null != controller)
            {
                UnityGameObject gameObject = controller.gameObject;
                if (null == gameObject)
                {
                    Logger.Error("The root game object must be non-null.");
                    return false;
                }

                UnityComponent[] components = gameObject.GetComponents<UnityComponent>();
                for (int n = 0; null != s_rootController && n < components.Length; ++n)
                {
                    if (s_rootController == components[n])
                    {
                        controller = components[n] as UnityMonoBehaviour;
                        break;
                    }
                }

                if (null == controller)
                {
                    Logger.Error("Could not found any framework controller component in this game object '{0}', register root controller failed.", gameObject.name);
                    return false;
                }

                // 初始化根节点实例
                s_rootGameObject = gameObject;
                // 初始化根控制器实例
                s_rootController = controller;
            }

            return Create();
        }

        /// <summary>
        /// 总控对象实例销毁函数
        /// </summary>
        public static void Destroy()
        {
            if (s_isRunning)
            {
                // 如果处于运行状态，则需要先关闭调度器
                Shutdown();
            }

            // 销毁引擎实例
            Engine.Destroy();
            s_engine = null;

            s_rootController = null;
            s_rootGameObject = null;
        }

        /// <summary>
        /// 总控对象的初始启动接口函数
        /// </summary>
        public static void Startup()
        {
            Logger.Assert(null != s_engine, "Invalid arguments.");

            if (s_engine.IsOnStartup)
            {
                Logger.Warn("The App entry is already running, cannot repeat start it.");
                return;
            }

            // 引擎实例启动
            s_engine.Startup();

            s_isRunning = true;
        }

        /// <summary>
        /// 总控对象的结束关闭接口函数
        /// </summary>
        public static void Shutdown()
        {
            if (null == s_engine || false == s_engine.IsOnStartup)
            {
                return;
            }

            // 移除所有管理器实例
            RemoveAllManagers();
            // 移除所有组件实例
            RemoveAllComponents();

            // 引擎实例关闭
            s_engine.Shutdown();

            s_isRunning = false;
        }

        /// <summary>
        ///总控对象的刷新回调函数
        /// </summary>
        public static void Update()
        {
            if (s_isRunning)
            {
                s_engine.Update();
            }

            // 管理器刷新通知
            UpdateAllManagers();
        }

        /// <summary>
        ///总控对象的后置刷新回调函数
        /// </summary>
        public static void LateUpdate()
        {
            if (s_isRunning)
            {
                s_engine.LateUpdate();
            }

            // 管理器后置刷新通知
            LateUpdateAllManagers();
        }
    }
}
