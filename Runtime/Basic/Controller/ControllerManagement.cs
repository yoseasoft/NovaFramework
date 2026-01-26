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

using System;
using System.Collections.Generic;
using System.Customize.Extension;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GameEngine
{
    /// <summary>
    /// 控制器的管理对象类，负责对所有的控制器对象实例进行统一的调度管理
    /// </summary>
    public static partial class ControllerManagement
    {
        /// <summary>
        /// 控制器管理类的后缀名称常量定义
        /// </summary>
        private const string ControllerClassUnifiedStandardName = "Controller";

        /// <summary>
        /// 控制器对象类的类型映射管理容器
        /// </summary>
        private static IDictionary<int, Type> _controllerClassTypes;
        /// <summary>
        /// 控制器对象实例的映射管理容器
        /// </summary>
        private static IDictionary<int, IController> _controllerObjects;

        /// <summary>
        /// 控制器对象类的创建函数回调句柄管理容器
        /// </summary>
        private static IDictionary<int, NovaEngine.ISingleton.SingletonCreateHandler> _controllerCreateCallbacks;
        /// <summary>
        /// 控制器对象类的销毁函数回调句柄管理容器
        /// </summary>
        private static IDictionary<int, NovaEngine.ISingleton.SingletonDestroyHandler> _controllerDestroyCallbacks;

        /// <summary>
        /// 控制器管理类的启动函数
        /// </summary>
        public static void Startup()
        {
            string namespaceTag = typeof(ControllerManagement).Namespace;

            // 管理容器初始化
            _controllerClassTypes = new Dictionary<int, Type>();
            _controllerObjects = new Dictionary<int, IController>();
            _controllerCreateCallbacks = new Dictionary<int, NovaEngine.ISingleton.SingletonCreateHandler>();
            _controllerDestroyCallbacks = new Dictionary<int, NovaEngine.ISingleton.SingletonDestroyHandler>();

            foreach (string enumName in Enum.GetNames(typeof(ModuleType)))
            {
                if (enumName.Equals(ModuleType.Unknown.ToString()))
                {
                    continue;
                }

                // 类名反射时需要包含命名空间前缀
                string controllerName = NovaEngine.FormatString.Format("{%s}.{%s}{%s}", namespaceTag, enumName, ControllerClassUnifiedStandardName);

                Type controllerType = Type.GetType(controllerName);
                if (null == controllerType)
                {
                    Debugger.Info(LogGroupTag.Controller, "Could not found any controller class with target name {%s}.", controllerName);
                    continue;
                }

                if (false == controllerType.Is<IController>())
                {
                    Debugger.Warn(LogGroupTag.Controller, "The controller type {%s} must be inherited from 'IController' interface.", controllerName);
                    continue;
                }

                int enumType = (int) NovaEngine.Utility.Convertion.GetEnumFromName<ModuleType>(enumName);
                Type singletonType = typeof(NovaEngine.Singleton<>);
                Type controllerGenericType = singletonType.MakeGenericType(new Type[] { controllerType });

                MethodInfo controllerCreateMethod = controllerGenericType.GetMethod("Create", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                MethodInfo controllerDestroyMethod = controllerGenericType.GetMethod("Destroy", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                Debugger.Assert(null != controllerCreateMethod && null != controllerDestroyMethod, "Invalid controller type.");

                NovaEngine.ISingleton.SingletonCreateHandler controllerCreateCallback = controllerCreateMethod.CreateDelegate(typeof(NovaEngine.ISingleton.SingletonCreateHandler)) as NovaEngine.ISingleton.SingletonCreateHandler;
                NovaEngine.ISingleton.SingletonDestroyHandler controllerDestroyCallback = controllerDestroyMethod.CreateDelegate(typeof(NovaEngine.ISingleton.SingletonDestroyHandler)) as NovaEngine.ISingleton.SingletonDestroyHandler;
                Debugger.Assert(null != controllerCreateCallback && null != controllerDestroyCallback, "Invalid method type.");

                // Debugger.Log(LogGroupTag.Controller, "Load controller type '{%t}' succeed.", controllerType);

                _controllerClassTypes.Add(enumType, controllerType);
                _controllerCreateCallbacks.Add(enumType, controllerCreateCallback);
                _controllerDestroyCallbacks.Add(enumType, controllerDestroyCallback);
            }

            foreach (KeyValuePair<int, NovaEngine.ISingleton.SingletonCreateHandler> pair in _controllerCreateCallbacks)
            {
                // 创建控制器实例
                object controller = pair.Value();
                if (_controllerObjects.ContainsKey(pair.Key))
                {
                    Debugger.Warn(LogGroupTag.Controller, "The controller of type '{%d}' was already exist, repeat created it will be removed old type.", pair.Key);

                    _controllerObjects.Remove(pair.Key);
                }

                _controllerObjects.Add(pair.Key, controller as IController);
            }
        }

        /// <summary>
        /// 控制器管理类的关闭函数
        /// </summary>
        public static void Shutdown()
        {
            foreach (KeyValuePair<int, NovaEngine.ISingleton.SingletonDestroyHandler> pair in _controllerDestroyCallbacks)
            {
                // 销毁控制器实例
                pair.Value();
            }

            // 容器清理
            _controllerClassTypes.Clear();
            _controllerClassTypes = null;
            _controllerObjects.Clear();
            _controllerObjects = null;

            _controllerCreateCallbacks.Clear();
            _controllerCreateCallbacks = null;
            _controllerDestroyCallbacks.Clear();
            _controllerDestroyCallbacks = null;
        }

        /// <summary>
        /// 控制器管理类执行通知接口函数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Execute()
        {
        }

        /// <summary>
        /// 控制器管理类后置执行通知接口函数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LateExecute()
        {
        }

        /// <summary>
        /// 控制器管理类刷新通知接口函数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Update()
        {
            foreach (KeyValuePair<int, IController> pair in _controllerObjects)
            {
                pair.Value.Update();
            }
        }

        /// <summary>
        /// 控制器管理类后置刷新通知接口函数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LateUpdate()
        {
            foreach (KeyValuePair<int, IController> pair in _controllerObjects)
            {
                pair.Value.LateUpdate();
            }
        }

        /// <summary>
        /// 控制器管理类重载通知接口函数
        /// </summary>
        public static void Reload()
        {
            foreach (KeyValuePair<int, IController> pair in _controllerObjects)
            {
                pair.Value.Reload();
            }
        }

        /// <summary>
        /// 控制器管理类倾泻通知接口函数
        /// </summary>
        public static void Dump()
        {
            foreach (KeyValuePair<int, IController> pair in _controllerObjects)
            {
                pair.Value.Dump();
            }
        }
    }
}
