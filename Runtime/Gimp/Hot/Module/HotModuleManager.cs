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

using System.Collections.Generic;

using SystemType = System.Type;

namespace GameEngine
{
    /// <summary>
    /// 模块热加载流程的管理器对象类，通过该类对所有热加载模块进行调度管理
    /// </summary>
    internal static class HotModuleManager
    {
        /// <summary>
        /// 热加载模块管理器当前是否运行的状态标识
        /// </summary>
        private static bool _isRunning;

        /// <summary>
        /// 热加载模块对象实例的管理容器
        /// </summary>
        private static IDictionary<SystemType, IHotModule> _hotModules;

        /// <summary>
        /// 热加载模块管理器启动函数
        /// </summary>
        public static void Startup()
        {
            // 热加载模块对象的管理容器初始化
            _hotModules = new Dictionary<SystemType, IHotModule>();

            _isRunning = true;
        }

        /// <summary>
        /// 热加载模块管理器停止函数
        /// </summary>
        public static void Shutdown()
        {
            // 热加载模块对象实例的管理容器注销
            UnregisterAllHotModules();
            _hotModules = null;

            _isRunning = false;
        }

        #region 外部热加载模块的接入/注销相关的操作接口

        /// <summary>
        /// 注册指定类型的热加载模块对象到当前管理句柄中
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void RegisterHotModule<T>() where T : IHotModule, new()
        {
            RegisterHotModule(typeof(T));
        }

        /// <summary>
        /// 注册指定类型的热加载模块对象到当前管理句柄中
        /// </summary>
        /// <param name="type">对象类型</param>
        public static void RegisterHotModule(SystemType type)
        {
            Debugger.Assert(_isRunning, NovaEngine.ErrorText.InvalidOperation);

            if (_hotModules.ContainsKey(type))
            {
                Debugger.Error("The hot module object '{%t}' was already exists, repeat registed it failed.", type);
                return;
            }

            IHotModule module = System.Activator.CreateInstance(type) as IHotModule;
            module.Startup();

            _hotModules.Add(type, module);
        }

        /// <summary>
        /// 从当前管理句柄中注销指定类型的热加载模块对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void UnregisterHotModule<T>() where T : IHotModule
        {
            UnregisterHotModule(typeof(T));
        }

        /// <summary>
        /// 从当前管理句柄中注销指定类型的热加载模块对象实例
        /// </summary>
        /// <param name="type">对象类型</param>
        public static void UnregisterHotModule(SystemType type)
        {
            Debugger.Assert(_isRunning, NovaEngine.ErrorText.InvalidOperation);

            if (false == _hotModules.TryGetValue(type, out IHotModule module))
            {
                Debugger.Error("Could not found target hot module object with class type '{%t}', unregisted it failed.", type);
                return;
            }

            module.Shutdown();

            _hotModules.Remove(type);
        }

        /// <summary>
        /// 注销当前管理器中的所有热加载模块对象实例
        /// </summary>
        private static void UnregisterAllHotModules()
        {
            IList<SystemType> keys = NovaEngine.Utility.Collection.ToListForKeys(_hotModules);
            for (int n = 0; n < keys.Count; ++n)
            {
                UnregisterHotModule(keys[n]);
            }

            _hotModules.Clear();
        }

        #endregion
    }
}
