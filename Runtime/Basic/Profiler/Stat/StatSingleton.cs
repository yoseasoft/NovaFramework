/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GameEngine.Profiler.Statistics
{
    /// <summary>
    /// 统计模块对象的单例构建类，该类以单例的形式对目标统计对象进行实例化管理
    /// </summary>
    public abstract class StatSingleton<TObject, TRecord>
        where TObject : class, IStat, new() // IStat<TRecord>
        where TRecord : StatInfo
    {
        /// <summary>
        /// 统计信息对象实例的缓存最大容量
        /// </summary>
        private const int StatInfoObjectCachesMaxSize = 100;

        private static TObject _instance = null;

        /// <summary>
        /// 统计模块的模块类型标识
        /// </summary>
        private int _statType;

        /// <summary>
        /// 统计模块对象启动状态标识
        /// </summary>
        // private bool _enabled = false;

        private IList<TRecord> _statInfoRecords;

        private IDictionary<int, MethodInfo> _regStatMethodTypes;

        /// <summary>
        /// 获取统计模块的模块类型标识
        /// </summary>
        public int StatType => _statType;

        /// <summary>
        /// 获取统计模块对象的启动状态
        /// </summary>
        // public bool Enabled => _enabled;

        /// <summary>
        /// 单例对象的默认构造函数<br/>
        /// 此处将函数的作用域声明为‘protected’，需要在自定义子类时实现该默认构造函数，且打开其访问作用域
        /// </summary>
        protected StatSingleton()
        {
        }

        /// <summary>
        /// 单例对象的默认析构函数
        /// </summary>
        ~StatSingleton()
        {
        }

        /// <summary>
        /// 获取单例类当前的有效实例
        /// </summary>
        public static TObject Instance
        {
            get
            {
                // 禁止被动创建实例，必须通过主动调用“Create”进行实例创建
                // if (null == StatSingleton<TObject, TRecord>._instance) { StatSingleton<TObject, TRecord>.Create(); }

                return StatSingleton<TObject, TRecord>._instance;
            }
        }

        /// <summary>
        /// 单例类的实例创建接口
        /// </summary>
        /// <param name="statType">统计类型</param>
        internal static TObject Create(int statType)
        {
            // 仅在调试模式下才创建统计模块实例
            if (NovaEngine.Environment.debugMode)
            {
                if (null == StatSingleton<TObject, TRecord>._instance)
                {
                    StatSingleton<TObject, TRecord>._instance = Activator.CreateInstance<TObject>();
                    (StatSingleton<TObject, TRecord>._instance as StatSingleton<TObject, TRecord>).Initialize(statType);
                }
            }

            return StatSingleton<TObject, TRecord>._instance;
        }

        /// <summary>
        /// 单例类的实例销毁接口
        /// </summary>
        internal static void Destroy()
        {
            if (null != StatSingleton<TObject, TRecord>._instance)
            {
                (StatSingleton<TObject, TRecord>._instance as StatSingleton<TObject, TRecord>).Cleanup();
                StatSingleton<TObject, TRecord>._instance = (TObject) ((object) null);
            }
        }

        /// <summary>
        /// 单例类初始化回调接口
        /// </summary>
        /// <param name="statType">统计类型</param>
        private void Initialize(int statType)
        {
            _statType = statType;
            _statInfoRecords = new List<TRecord>();
            _regStatMethodTypes = new Dictionary<int, MethodInfo>();

            // 仅在调试模块下开启统计功能
            // 2025-09-12：
            // 由于整合的统计模块外部访问接口，所以统计模块的可用状态，统一在控制中心管理
            // 无需在每个单独的实例内部再做处理
            // if (NovaEngine.Environment.debugMode) { _enabled = true; }

            // 统计接口初始化绑定
            InitAllStatMethods();

            // 注册统计模块实例
            Statistician.RegisterStat(_statType, _instance, GetAllStatMethodTypes());

            OnInitialize();
        }

        /// <summary>
        /// 单例类清理回调接口
        /// </summary>
        private void Cleanup()
        {
            OnCleanup();

            // 注销统计模块实例
            Statistician.UnregisterStat(_statType);

            // 注销所有统计回调函数
            UnregisterAllStatMethods();
            _regStatMethodTypes = null;

            _statInfoRecords.Clear();
            _statInfoRecords = null;
        }

        /// <summary>
        /// 单例类垃圾卸载回调接口
        /// </summary>
        private void Dump()
        {
            OnDump();

            _statInfoRecords.Clear();
        }

        /// <summary>
        /// 初始化引擎框架模块实例的回调接口
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        /// 清理引擎框架模块实例的回调接口
        /// </summary>
        protected virtual void OnCleanup() { }

        /// <summary>
        /// 回收引擎框架模块实例的回调接口
        /// </summary>
        protected virtual void OnDump() { }

        /// <summary>
        /// 检测指定的统计模块对象实例是否处于激活状态
        /// </summary>
        /// <returns>返回统计模块实例的激活状态</returns>
        // internal static bool IsActivated()
        // {
        //     if (null == _instance) return false;
        //     return (StatSingleton<T>._instance as StatSingleton<T>).Enabled;
        // }

        /// <summary>
        /// 清理统计模块对象实例中的所有临时记录数据
        /// </summary>
        internal static void Clear()
        {
            if (null == StatSingleton<TObject, TRecord>._instance) return;

            (StatSingleton<TObject, TRecord>._instance as StatSingleton<TObject, TRecord>).Dump();
        }

        #region 模块内部统计函数相关的管理接口函数

        /// <summary>
        /// 对模块内部所有的统计函数进行初始化绑定操作
        /// </summary>
        private void InitAllStatMethods()
        {
            Type classType = GetType();
            MethodInfo[] methods = classType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for (int n = 0; n < methods.Length; ++n)
            {
                MethodInfo method = methods[n];
                Attribute attr = method.GetCustomAttribute(typeof(IStat.OnStatFunctionRegisterAttribute));
                if (null != attr)
                {
                    IStat.OnStatFunctionRegisterAttribute _attr = (IStat.OnStatFunctionRegisterAttribute) attr;
                    RegisterStatMethod(_attr.FuncType, method);
                }
            }
        }

        /// <summary>
        /// 注册指定类型的统计函数
        /// </summary>
        /// <param name="funcType">统计类型</param>
        /// <param name="methodName">函数名称</param>
        protected void RegisterStatMethod(int funcType, string methodName)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                throw new NovaEngine.CFrameworkException("The method name is invalid.");
            }

            MethodInfo method = this.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (null == method)
            {
                Debugger.Error(LogGroupTag.Profiler, "Could not found any method info with name '{%s}' in current class '{%t}', register that method info failed.", methodName, this);
                return;
            }

            RegisterStatMethod(funcType, method);
        }

        /// <summary>
        /// 注册指定类型的统计函数
        /// </summary>
        /// <param name="funcType">统计类型</param>
        /// <param name="method">函数信息</param>
        protected void RegisterStatMethod(int funcType, MethodInfo method)
        {
            Debugger.Assert(null != _regStatMethodTypes, "The register method container must be non-null.");

            if (_regStatMethodTypes.ContainsKey(funcType))
            {
                Debugger.Warn(LogGroupTag.Profiler, "The stat method type '{%d}' was already register, repeat do it will be override old value.", funcType);
                _regStatMethodTypes.Remove(funcType);
            }

            _regStatMethodTypes.Add(funcType, method);
        }

        /// <summary>
        /// 注销当前已注册的所有统计函数
        /// </summary>
        private void UnregisterAllStatMethods()
        {
            // 清空容器
            _regStatMethodTypes.Clear();
        }

        /// <summary>
        /// 获取当前已注册的所有统计函数的编码清单
        /// </summary>
        /// <returns>返回所有统计函数的编码清单</returns>
        private IList<int> GetAllStatMethodTypes()
        {
            return NovaEngine.Utility.Collection.ToList(_regStatMethodTypes.Keys);
        }

        #endregion

        /// <summary>
        /// 调用指定类型的统计函数
        /// </summary>
        /// <param name="funcType">统计类型</param>
        /// <param name="args">参数列表</param>
        internal static void Process(int funcType, params object[] args)
        {
            // if (!IsActivated()) return;

            if (false == (StatSingleton<TObject, TRecord>._instance as StatSingleton<TObject, TRecord>)._regStatMethodTypes.TryGetValue(funcType, out MethodInfo method))
            {
                Debugger.Warn(LogGroupTag.Profiler, "Could not found any register stat method with type '{%d}', invoke it failed.", funcType);
                return;
            }

            method.Invoke(StatSingleton<TObject, TRecord>._instance as StatSingleton<TObject, TRecord>, args);
        }

        #region 统计模块内部容器管理的辅助接口函数

        /// <summary>
        /// 添加统计信息实例到指定列表中
        /// 这里会对列表长度进行限制，若超出范围将移除访问间隔时间最长数据
        /// </summary>
        /// <param name="targetObject">统计信息实例</param>
        protected void TryAddValue(TRecord targetObject)
        {
            while (_statInfoRecords.Count > StatInfoObjectCachesMaxSize)
            {
                _statInfoRecords.RemoveAt(0);
            }

            // 理论上不会有重复ID的情况，这里暂时先检测一下，后面再考虑去除掉此处代码
            for (int n = 0; n < _statInfoRecords.Count; ++n)
            {
                if (_statInfoRecords[n].Uid == targetObject.Uid)
                {
                    Debugger.Warn(LogGroupTag.Profiler, "目标统计模块对象‘{%t}’的信息记录列表中已存在ID为‘{%d}’的信息对象实例，请勿重复添加相同ID的对象实例！", typeof(TObject), targetObject.Uid);
                    return;
                }
            }

            _statInfoRecords.Add(targetObject);
        }

        /// <summary>
        /// 在当前列表中对指定ID的统计信息实例进行访问
        /// 此方法会自动将访问时间更新，同时调整该信息对象在列表中的位置
        /// </summary>
        /// <param name="uid">统计信息标识</param>
        /// <returns>返回对应的统计信息对象实例</returns>
        protected TRecord TryGetValue(int uid)
        {
            for (int n = _statInfoRecords.Count - 1; n >= 0; --n)
            {
                TRecord found = _statInfoRecords[n];
                if (found.Uid == uid)
                {
                    found.Access();
                    return found;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取当前统计信息对象列表
        /// </summary>
        /// <returns>返回当前统计信息对象列表</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected IReadOnlyList<TRecord> TryGetAllValues()
        {
            return (List<TRecord>) _statInfoRecords;
        }

        /// <summary>
        /// 在当前列表中对指定ID的统计信息实例进行释放
        /// 此方法会更新对应信息实例的释放时间
        /// </summary>
        /// <param name="uid">统计信息标识</param>
        protected void TryCloseValue(int uid)
        {
            for (int n = _statInfoRecords.Count - 1; n >= 0; --n)
            {
                TRecord found = _statInfoRecords[n];
                if (found.Uid == uid)
                {
                    found.Release();
                    return;
                }
            }

            Debugger.Warn(LogGroupTag.Profiler, "目标统计模块对象‘{%t}’的信息记录列表中无法查找到ID为‘{%d}’的信息对象实例，请检查该实例是否已被提前销毁！", typeof(TObject), uid);
        }

        #endregion
    }
}
