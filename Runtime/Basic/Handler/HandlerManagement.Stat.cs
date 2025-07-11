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

using System.Collections.Generic;

using SystemEnum = System.Enum;
using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemDelegate = System.Delegate;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemBindingFlags = System.Reflection.BindingFlags;

namespace GameEngine
{
    /// <summary>
    /// 句柄对象的封装管理类，对已定义的所有句柄类进行统一的调度派发操作
    /// </summary>
    internal static partial class HandlerManagement
    {
        /// <summary>
        /// 统计模块类的后缀名称常量定义
        /// </summary>
        private const string STAT_MODULE_CLASS_SUFFIX_NAME = "StatModule";

        /// <summary>
        /// 单例类的创建函数句柄
        /// </summary>
        private delegate object StatModuleCreateHandler(IHandler handler);

        /// <summary>
        /// 单例类的销毁函数句柄
        /// </summary>
        private delegate void StatModuleDestroyHandler();

        /// <summary>
        /// 模块统计对象类的类型映射管理容器
        /// </summary>
        private static IDictionary<int, SystemType> _statModuleClassTypes;

        /// <summary>
        /// 模块统计对象实例的映射容器
        /// </summary>
        private static IDictionary<int, IStatModule> _statModuleObjects;

        /// <summary>
        /// 模块统计对象类的创建函数回调句柄管理容器
        /// </summary>
        private static IDictionary<int, StatModuleCreateHandler> _statModuleCreateCallbacks;
        /// <summary>
        /// 模块统计对象类的销毁函数回调句柄管理容器
        /// </summary>
        private static IDictionary<int, StatModuleDestroyHandler> _statModuleDestroyCallbacks;

        /// <summary>
        /// 初始化句柄对象的全部统计模块
        /// </summary>
        private static void InitAllStatModules()
        {
            // 管理容器初始化
            _statModuleClassTypes = new Dictionary<int, SystemType>();
            _statModuleObjects = new Dictionary<int, IStatModule>();
            _statModuleCreateCallbacks = new Dictionary<int, StatModuleCreateHandler>();
            _statModuleDestroyCallbacks = new Dictionary<int, StatModuleDestroyHandler>();

            // 加载全部统计模块
            LoadAllStatModules();

            foreach (KeyValuePair<int, StatModuleCreateHandler> pair in _statModuleCreateCallbacks)
            {
                // 创建统计模块实例
                /*object stat_module = */
                pair.Value(GetHandler(pair.Key));
                if (false == _statModuleObjects.ContainsKey(pair.Key))
                {
                    Debugger.Warn("Could not found any stat module class '{0}' from manager list, created it failed.", pair.Key);

                    // s_statModuleObjects.Add(pair.Key, stat_module as IStatModule);
                }
            }
        }

        /// <summary>
        /// 清理句柄对象的全部统计模块
        /// </summary>
        private static void CleanupAllStatModules()
        {
            foreach (KeyValuePair<int, StatModuleDestroyHandler> pair in _statModuleDestroyCallbacks)
            {
                // 销毁统计模块实例
                pair.Value();
            }

            // 容器清理
            _statModuleClassTypes.Clear();
            _statModuleClassTypes = null;
            _statModuleObjects.Clear();
            _statModuleObjects = null;

            _statModuleCreateCallbacks.Clear();
            _statModuleCreateCallbacks = null;
            _statModuleDestroyCallbacks.Clear();
            _statModuleDestroyCallbacks = null;
        }

        /// <summary>
        /// 加载全部统计模块
        /// 需要注意的是，若调试模式未开启，将跳过加载逻辑
        /// </summary>
        private static void LoadAllStatModules()
        {
            string namespaceTag = typeof(HandlerManagement).Namespace;

            foreach (string enumName in SystemEnum.GetNames(typeof(NovaEngine.ModuleObject.EEventType)))
            {
                if (enumName.Equals(NovaEngine.ModuleObject.EEventType.Default.ToString()) || enumName.Equals(NovaEngine.ModuleObject.EEventType.User.ToString()))
                {
                    continue;
                }

                // 类名反射时需要包含命名空间前缀
                string statModuleName = NovaEngine.Utility.Text.Format("{0}.{1}{2}", namespaceTag, enumName, STAT_MODULE_CLASS_SUFFIX_NAME);

                SystemType statModuleType = SystemType.GetType(statModuleName);
                if (null == statModuleType)
                {
                    Debugger.Info(LogGroupTag.Module, "Could not found any stat module class with target name {0}.", statModuleName);
                    continue;
                }

                if (false == typeof(IStatModule).IsAssignableFrom(statModuleType))
                {
                    Debugger.Warn("The stat module type {0} must be inherited from 'IStatModule' interface.", statModuleName);
                    continue;
                }

                int enumType = (int) NovaEngine.Utility.Convertion.GetEnumFromString<NovaEngine.ModuleObject.EEventType>(enumName);
                if (null == GetHandler(enumType))
                {
                    Debugger.Warn("Could not found any handler class with target type '{0}', created stat module failed.", enumName);
                    continue;
                }

                SystemType singletonType = typeof(HandlerStatSingleton<>);
                SystemType statModuleGenericType = singletonType.MakeGenericType(new SystemType[] { statModuleType });

                SystemMethodInfo statModuleCreateMethod = statModuleGenericType.GetMethod("Create", SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Static);
                SystemMethodInfo statModuleDestroyMethod = statModuleGenericType.GetMethod("Destroy", SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Static);
                Debugger.Assert(null != statModuleCreateMethod && null != statModuleDestroyMethod, "Invalid stat module type.");

                StatModuleCreateHandler statModuleCreateCallback = statModuleCreateMethod.CreateDelegate(typeof(StatModuleCreateHandler)) as StatModuleCreateHandler;
                StatModuleDestroyHandler statModuleDestroyCallback = statModuleDestroyMethod.CreateDelegate(typeof(StatModuleDestroyHandler)) as StatModuleDestroyHandler;
                Debugger.Assert(null != statModuleCreateCallback && null != statModuleDestroyCallback, "Invalid method type.");

                Debugger.Log(LogGroupTag.Module, "Load handler stat module type '{0}' succeed.", statModuleType.FullName);

                _statModuleClassTypes.Add(enumType, statModuleType);
                _statModuleCreateCallbacks.Add(enumType, statModuleCreateCallback);
                _statModuleDestroyCallbacks.Add(enumType, statModuleDestroyCallback);
            }
        }

        #region 模块统计对象实例管理控制接口函数

        /// <summary>
        /// 添加新的统计模块实例
        /// </summary>
        /// <param name="moduleType">统计模块对象类型</param>
        /// <param name="statModule">统计模块对象实例</param>
        internal static void RegisterStatModule(int moduleType, IStatModule statModule)
        {
            if (_statModuleObjects.ContainsKey(moduleType))
            {
                Debugger.Warn("The stat module object '{0}' is already exist, cannot repeat register it.", moduleType);
                return;
            }

            _statModuleObjects.Add(moduleType, statModule);
        }

        /// <summary>
        /// 移除指定类型的统计模块实例
        /// </summary>
        /// <param name="moduleType">统计模块对象类型</param>
        internal static void UnregisterStatModule(int moduleType)
        {
            if (false == _statModuleObjects.ContainsKey(moduleType))
            {
                Debugger.Warn("Could not found any stat module object '{0}' in this container, unregister it failed.", moduleType);
                return;
            }

            _statModuleObjects.Remove(moduleType);
        }

        /// <summary>
        /// 通过指定统计模块类型获取对应的统计模块对象实例
        /// </summary>
        /// <typeparam name="T">统计模块类型</typeparam>
        /// <returns>返回类型获取对应的对象实例</returns>
        public static T GetStatModule<T>() where T : IStatModule
        {
            return (T) GetStatModule(typeof(T));
        }

        /// <summary>
        /// 通过指定统计模块类型名称获取对应的统计模块对象实例
        /// </summary>
        /// <param name="clsType">统计模块类型</param>
        /// <returns>返回类型名称获取对应的对象实例</returns>
        public static IStatModule GetStatModule(SystemType clsType)
        {
            foreach (KeyValuePair<int, IStatModule> pair in _statModuleObjects)
            {
                if (clsType.IsAssignableFrom(pair.Value.GetType()))
                    return pair.Value;
            }

            return null;
        }

        /// <summary>
        /// 通过指定统计模块类型获取对应的统计模块对象实例
        /// </summary>
        /// <param name="moduleType">统计模块对象类型</param>
        /// <returns>返回类型获取对应的对象实例</returns>
        public static IStatModule GetStatModule(int moduleType)
        {
            if (_statModuleObjects.ContainsKey(moduleType))
            {
                return _statModuleObjects[moduleType];
            }

            return null;
        }

        #endregion
    }
}
