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

using System.Collections.Generic;
using System.Reflection;

using SystemEnum = System.Enum;
using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemDelegate = System.Delegate;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemBindingFlags = System.Reflection.BindingFlags;

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
        private const string CONTROLLER_CLASS_SUFFIX_NAME = "Controller";

        /// <summary>
        /// 控制器对象类的类型映射管理容器
        /// </summary>
        private static IDictionary<int, SystemType> s_controllerClassTypes;
        /// <summary>
        /// 控制器对象实例的映射管理容器
        /// </summary>
        private static IDictionary<int, IController> s_controllerObjects;

        /// <summary>
        /// 控制器对象类的创建函数回调句柄管理容器
        /// </summary>
        private static IDictionary<int, NovaEngine.ISingleton.SingletonCreateHandler> s_controllerCreateCallbacks;
        /// <summary>
        /// 控制器对象类的销毁函数回调句柄管理容器
        /// </summary>
        private static IDictionary<int, NovaEngine.ISingleton.SingletonDestroyHandler> s_controllerDestroyCallbacks;

        /// <summary>
        /// 控制器管理类的启动函数
        /// </summary>
        public static void Startup()
        {
            string namespaceTag = typeof(ControllerManagement).Namespace;

            // 管理容器初始化
            s_controllerClassTypes = new Dictionary<int, SystemType>();
            s_controllerObjects = new Dictionary<int, IController>();
            s_controllerCreateCallbacks = new Dictionary<int, NovaEngine.ISingleton.SingletonCreateHandler>();
            s_controllerDestroyCallbacks = new Dictionary<int, NovaEngine.ISingleton.SingletonDestroyHandler>();

            foreach (string enumName in SystemEnum.GetNames(typeof(ModuleType)))
            {
                if (enumName.Equals(ModuleType.Unknown.ToString()))
                {
                    continue;
                }

                // 类名反射时需要包含命名空间前缀
                string controllerName = NovaEngine.Utility.Text.Format("{%s}.{%s}{%s}", namespaceTag, enumName, CONTROLLER_CLASS_SUFFIX_NAME);

                SystemType controllerType = SystemType.GetType(controllerName);
                if (null == controllerType)
                {
                    Debugger.Info("Could not found any controller class with target name {%s}.", controllerName);
                    continue;
                }

                if (false == typeof(IController).IsAssignableFrom(controllerType))
                {
                    Debugger.Warn("The controller type {%s} must be inherited from 'IController' interface.", controllerName);
                    continue;
                }

                int enumType = (int) NovaEngine.Utility.Convertion.GetEnumFromString<ModuleType>(enumName);
                SystemType singletonType = typeof(NovaEngine.Singleton<>);
                SystemType controllerGenericType = singletonType.MakeGenericType(new SystemType[] { controllerType });

                SystemMethodInfo controllerCreateMethod = controllerGenericType.GetMethod("Create", SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Static);
                SystemMethodInfo controllerDestroyMethod = controllerGenericType.GetMethod("Destroy", SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Static);
                Debugger.Assert(null != controllerCreateMethod && null != controllerDestroyMethod, "Invalid controller type.");

                NovaEngine.ISingleton.SingletonCreateHandler controllerCreateCallback = controllerCreateMethod.CreateDelegate(typeof(NovaEngine.ISingleton.SingletonCreateHandler)) as NovaEngine.ISingleton.SingletonCreateHandler;
                NovaEngine.ISingleton.SingletonDestroyHandler controllerDestroyCallback = controllerDestroyMethod.CreateDelegate(typeof(NovaEngine.ISingleton.SingletonDestroyHandler)) as NovaEngine.ISingleton.SingletonDestroyHandler;
                Debugger.Assert(null != controllerCreateCallback && null != controllerDestroyCallback, "Invalid method type.");

                Debugger.Log(LogGroupTag.Controller, "Load controller type '{%f}' succeed.", controllerType);

                s_controllerClassTypes.Add(enumType, controllerType);
                s_controllerCreateCallbacks.Add(enumType, controllerCreateCallback);
                s_controllerDestroyCallbacks.Add(enumType, controllerDestroyCallback);
            }

            foreach (KeyValuePair<int, NovaEngine.ISingleton.SingletonCreateHandler> pair in s_controllerCreateCallbacks)
            {
                // 创建控制器实例
                object controller = pair.Value();
                if (s_controllerObjects.ContainsKey(pair.Key))
                {
                    Debugger.Warn("The controller of type '{%d}' was already exist, repeat created it will be removed old type.", pair.Key);

                    s_controllerObjects.Remove(pair.Key);
                }

                s_controllerObjects.Add(pair.Key, controller as IController);
            }
        }

        /// <summary>
        /// 控制器管理类的关闭函数
        /// </summary>
        public static void Shutdown()
        {
            foreach (KeyValuePair<int, NovaEngine.ISingleton.SingletonDestroyHandler> pair in s_controllerDestroyCallbacks)
            {
                // 销毁控制器实例
                pair.Value();
            }

            // 容器清理
            s_controllerClassTypes.Clear();
            s_controllerClassTypes = null;
            s_controllerObjects.Clear();
            s_controllerObjects = null;

            s_controllerCreateCallbacks.Clear();
            s_controllerCreateCallbacks = null;
            s_controllerDestroyCallbacks.Clear();
            s_controllerDestroyCallbacks = null;
        }

        /// <summary>
        /// 控制器管理类刷新通知接口函数
        /// </summary>
        public static void Update()
        {
            foreach (KeyValuePair<int, IController> pair in s_controllerObjects)
            {
                pair.Value.Update();
            }
        }

        /// <summary>
        /// 控制器管理类后置刷新通知接口函数
        /// </summary>
        public static void LateUpdate()
        {
            foreach (KeyValuePair<int, IController> pair in s_controllerObjects)
            {
                pair.Value.LateUpdate();
            }
        }

        /// <summary>
        /// 控制器管理类倾泻通知接口函数
        /// </summary>
        public static void Dump()
        {
            foreach (KeyValuePair<int, IController> pair in s_controllerObjects)
            {
                pair.Value.Dump();
            }
        }
    }
}
