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

using System.Collections.Generic;

using SystemEnum = System.Enum;
using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemBindingFlags = System.Reflection.BindingFlags;

namespace GameEngine.Profiler.Statistics
{
    /// <summary>
    /// 统计模块中心管理工具类，用于对运行时数据进行统计分析接口调度
    /// </summary>
    internal static class StatisticalCenter
    {
        /// <summary>
        /// 统计模块类的统一规范名称定义
        /// </summary>
        private const string StatClassUnifiedStandardName = "Stat";

        /// <summary>
        /// 单例类的创建函数句柄
        /// </summary>
        private delegate object StatCreateHandler(int statType);

        /// <summary>
        /// 单例类的销毁函数句柄
        /// </summary>
        private delegate void StatDestroyHandler();

        /// <summary>
        /// 单例类的调用函数句柄
        /// </summary>
        private delegate void StatProcessHandler(int funcType, params object[] args);

        /// <summary>
        /// 模块统计对象类的启动状态标识
        /// </summary>
        private static bool _isOnStarting = false;

        /// <summary>
        /// 模块统计对象类的类型映射管理容器
        /// </summary>
        private static IDictionary<int, SystemType> _statClassTypes;

        /// <summary>
        /// 模块统计对象类的实例映射容器
        /// </summary>
        private static IDictionary<int, IStat> _statObjects;

        /// <summary>
        /// 模块统计对象类的创建函数回调句柄管理容器
        /// </summary>
        private static IDictionary<int, StatCreateHandler> _statCreateCallbacks;
        /// <summary>
        /// 模块统计对象类的销毁函数回调句柄管理容器
        /// </summary>
        private static IDictionary<int, StatDestroyHandler> _statDestroyCallbacks;
        /// <summary>
        /// 模块统计对象类的调用函数回调句柄管理容器
        /// </summary>
        private static IDictionary<int, StatProcessHandler> _statProcessCallbacks;

        /// <summary>
        /// 模块统计对象类的编码映射管理容器
        /// </summary>
        private static IDictionary<int, SystemType> _statCodeTypes;

        /// <summary>
        /// 初始化句柄对象的全部统计模块
        /// </summary>
        public static void Startup()
        {
            // 管理容器初始化
            _statClassTypes = new Dictionary<int, SystemType>();
            _statObjects = new Dictionary<int, IStat>();
            _statCreateCallbacks = new Dictionary<int, StatCreateHandler>();
            _statDestroyCallbacks = new Dictionary<int, StatDestroyHandler>();
            _statProcessCallbacks = new Dictionary<int, StatProcessHandler>();
            _statCodeTypes = new Dictionary<int, SystemType>();

            // 加载全部统计模块
            LoadAllStats();

            foreach (KeyValuePair<int, StatCreateHandler> pair in _statCreateCallbacks)
            {
                // 创建统计模块实例
                object stat_module = pair.Value(pair.Key);

                if (false == _statObjects.ContainsKey(pair.Key))
                {
                    Debugger.Warn("Could not found any stat class '{0}' from manager list, created it failed.", pair.Key);

                    // _statObjects.Add(pair.Key, stat_module as IStat);
                }
            }

            // 开启统计功能
            _isOnStarting = true;
        }

        /// <summary>
        /// 清理句柄对象的全部统计模块
        /// </summary>
        public static void Shutdown()
        {
            // 关闭统计功能
            _isOnStarting = false;

            foreach (KeyValuePair<int, StatDestroyHandler> pair in _statDestroyCallbacks)
            {
                // 销毁统计模块实例
                pair.Value();
            }

            // 容器清理
            _statClassTypes.Clear();
            _statClassTypes = null;
            _statObjects.Clear();
            _statObjects = null;

            _statCreateCallbacks.Clear();
            _statCreateCallbacks = null;
            _statDestroyCallbacks.Clear();
            _statDestroyCallbacks = null;
            _statProcessCallbacks.Clear();
            _statProcessCallbacks = null;

            _statCodeTypes.Clear();
            _statCodeTypes = null;
        }

        /// <summary>
        /// 加载全部统计模块
        /// 需要注意的是，若调试模式未开启，将跳过加载逻辑
        /// </summary>
        private static void LoadAllStats()
        {
            string namespaceTag = typeof(StatisticalCenter).Namespace;

            foreach (StatType enumType in SystemEnum.GetValues(typeof(StatType)))
            {
                if (StatType.Unknown == enumType)
                {
                    continue;
                }

                string enumName = enumType.ToString();

                // 类名反射时需要包含命名空间前缀
                string statName = NovaEngine.Utility.Text.Format("{0}.{1}{2}", namespaceTag, enumName, StatClassUnifiedStandardName);

                SystemType statType = SystemType.GetType(statName);
                if (null == statType)
                {
                    Debugger.Info(LogGroupTag.Profiler, "Could not found any stat class with target name {0}.", statName);
                    continue;
                }

                if (false == typeof(IStat).IsAssignableFrom(statType))
                {
                    Debugger.Warn("The stat type {0} must be inherited from 'IStat' interface.", statName);
                    continue;
                }

                SystemType singletonType = typeof(StatSingleton<>);
                SystemType statGenericType = singletonType.MakeGenericType(new SystemType[] { statType });

                SystemMethodInfo statCreateMethod = statGenericType.GetMethod("Create", SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Static);
                SystemMethodInfo statDestroyMethod = statGenericType.GetMethod("Destroy", SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Static);
                SystemMethodInfo statProcessMethod = statGenericType.GetMethod("Process", SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Static);
                Debugger.Assert(null != statCreateMethod && null != statDestroyMethod && null != statProcessMethod, "Invalid stat type.");

                StatCreateHandler statCreateCallback = statCreateMethod.CreateDelegate(typeof(StatCreateHandler)) as StatCreateHandler;
                StatDestroyHandler statDestroyCallback = statDestroyMethod.CreateDelegate(typeof(StatDestroyHandler)) as StatDestroyHandler;
                StatProcessHandler statProcessCallback = statProcessMethod.CreateDelegate(typeof(StatProcessHandler)) as StatProcessHandler;
                Debugger.Assert(null != statCreateCallback && null != statDestroyCallback && null != statProcessCallback, "Invalid method type.");

                Debugger.Log(LogGroupTag.Profiler, "Load stat type '{0}' succeed.", statType.FullName);

                _statClassTypes.Add((int) enumType, statType);
                _statCreateCallbacks.Add((int) enumType, statCreateCallback);
                _statDestroyCallbacks.Add((int) enumType, statDestroyCallback);
                _statProcessCallbacks.Add((int) enumType, statProcessCallback);
            }
        }

        /// <summary>
        /// 调用统计模块处理函数
        /// </summary>
        /// <param name="funcType">功能类型</param>
        /// <param name="args">参数列表</param>
        public static void Call(int funcType, params object[] args)
        {
            if (!_isOnStarting)
            {
                Debugger.Warn(LogGroupTag.Profiler, "调用统计模块处理函数异常：当前统计模块尚未启动，执行统计调用操作失败！");
                return;
            }

            if (false == _statCodeTypes.TryGetValue(funcType, out SystemType targetType))
            {
                Debugger.Warn(LogGroupTag.Profiler, "调用统计模块处理函数异常：当前控制中心的编码映射容器中无法找到匹配指定功能编码‘{%d}’的统计类型，移除该统计编码操作失败！", funcType);
                return;
            }

            IStat stat = GetStat(targetType);
            Debugger.Assert(stat, "Invalid arguments.");

            if (false == _statProcessCallbacks.TryGetValue(stat.StatType, out StatProcessHandler callback))
            {
                Debugger.Warn(LogGroupTag.Profiler, "调用统计模块处理函数异常：目标统计模块‘{%d}’下的处理函数句柄缺失，无法正确执行统计调用操作！", stat.StatType);
                return;
            }

            callback(funcType, args);
        }

        #region 统计模块编码注册访问管理接口函数

        /// <summary>
        /// 注册指定对象类型的所有统计功能编码
        /// </summary>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="statCodeList">功能编码列表</param>
        private static void RegisterStatCodeTypes(SystemType targetType, IList<int> statCodeList)
        {
            if (null == statCodeList)
                return;

            for (int n = 0; n < statCodeList.Count; ++n)
            {
                RegisterStatCodeTypes(targetType, statCodeList[n]);
            }
        }

        /// <summary>
        /// 注册指定对象类型的指定功能编码
        /// </summary>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="statCode">功能编码</param>
        private static void RegisterStatCodeTypes(SystemType targetType, int statCode)
        {
            if (_statCodeTypes.ContainsKey(statCode))
            {
                Debugger.Warn(LogGroupTag.Profiler, "注册统计编码类型异常：目标统计对象‘{%t}’的指定功能编码‘{%d}’已被注册到当前的访问列表中，重复注册相同编码将覆盖原有数据！", targetType, statCode);
                _statCodeTypes.Remove(statCode);
            }

            _statCodeTypes.Add(statCode, targetType);
        }

        /// <summary>
        /// 移除指定对象类型的所有统计功能编码
        /// </summary>
        /// <param name="targetType">目标对象类型</param>
        private static void RemoveStatCode(SystemType targetType)
        {
            IList<int> codes = NovaEngine.Utility.Collection.ToList<int>(_statCodeTypes.Keys);
            for (int n = 0; n < codes.Count; ++n)
            {
                int k = codes[n];
                if (false == _statCodeTypes.TryGetValue(k, out SystemType classType))
                {
                    Debugger.Warn(LogGroupTag.Profiler, "移除统计编码类型异常：当前控制中心的编码映射容器中无法找到匹配指定功能编码‘{%d}’的统计类型，移除该统计编码操作失败！", k);
                    continue;
                }

                if (classType == targetType)
                {
                    _statCodeTypes.Remove(k);
                }
            }
        }

        #endregion

        #region 统计模块对象实例管理控制接口函数

        /// <summary>
        /// 添加新的统计模块实例
        /// </summary>
        /// <param name="statType">统计模块对象类型</param>
        /// <param name="stat">统计模块对象实例</param>
        /// <param name="statCodeList">统计模块对象编码列表</param>
        internal static void RegisterStat(int statType, IStat stat, IList<int> statCodeList)
        {
            if (_statObjects.ContainsKey(statType))
            {
                Debugger.Warn("The stat object '{0}' is already exist, cannot repeat register it.", statType);
                return;
            }

            _statObjects.Add(statType, stat);

            // 注册编码列表
            RegisterStatCodeTypes(stat.GetType(), statCodeList);
        }

        /// <summary>
        /// 移除指定类型的统计模块实例
        /// </summary>
        /// <param name="statType">统计模块对象类型</param>
        internal static void UnregisterStat(int statType)
        {
            if (false == _statObjects.ContainsKey(statType))
            {
                Debugger.Warn("Could not found any stat module object '{0}' in this container, unregister it failed.", statType);
                return;
            }

            IStat stat = GetStat(statType);
            // 移除编码列表
            RemoveStatCode(stat.GetType());

            _statObjects.Remove(statType);
        }

        /// <summary>
        /// 通过指定统计模块类型获取对应的统计模块对象实例
        /// </summary>
        /// <typeparam name="T">统计模块类型</typeparam>
        /// <returns>返回类型获取对应的对象实例</returns>
        public static T GetStat<T>() where T : IStat
        {
            return (T) GetStat(typeof(T));
        }

        /// <summary>
        /// 通过指定统计模块类型名称获取对应的统计模块对象实例
        /// </summary>
        /// <param name="clsType">统计模块类型</param>
        /// <returns>返回类型名称获取对应的对象实例</returns>
        public static IStat GetStat(SystemType clsType)
        {
            foreach (KeyValuePair<int, IStat> pair in _statObjects)
            {
                if (clsType == pair.Value.GetType())
                    return pair.Value;
            }

            return null;
        }

        /// <summary>
        /// 通过指定统计模块类型获取对应的统计模块对象实例
        /// </summary>
        /// <param name="statType">统计模块对象类型</param>
        /// <returns>返回类型获取对应的对象实例</returns>
        public static IStat GetStat(int statType)
        {
            if (_statObjects.ContainsKey(statType))
            {
                return _statObjects[statType];
            }

            return null;
        }

        #endregion
    }
}
