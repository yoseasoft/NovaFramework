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

using System.Collections.Generic;
using System.Reflection;

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;
using SystemDelegate = System.Delegate;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemBindingFlags = System.Reflection.BindingFlags;

namespace GameEngine
{
    /// <summary>
    /// 用于定义句柄基类的抽象对象类，提供一些针对句柄对象操作的通用函数<br/>
    /// 您可以通过继承该类，实现自定义的句柄对象
    /// </summary>
    public abstract class BaseHandler : IHandler
    {
        /// <summary>
        /// 句柄对象类的子模块初始化回调句柄的声明属性类型定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        protected sealed class OnSubmoduleInitCallbackAttribute : SystemAttribute
        {
            public OnSubmoduleInitCallbackAttribute() : base() { }
        }

        /// <summary>
        /// 句柄对象类的子模块清理回调句柄的声明属性类型定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        protected sealed class OnSubmoduleCleanupCallbackAttribute : SystemAttribute
        {
            public OnSubmoduleCleanupCallbackAttribute() : base() { }
        }

        /// <summary>
        /// 句柄对象类的子模块清理回调句柄的声明属性类型定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        protected sealed class OnSubmoduleReloadCallbackAttribute : SystemAttribute
        {
            public OnSubmoduleReloadCallbackAttribute() : base() { }
        }

        /// <summary>
        /// 句柄对象的类型标识
        /// </summary>
        private int _handlerType = 0;

        /// <summary>
        /// 句柄对象当前是否处于任务调度中的状态标识
        /// </summary>
        private bool _isOnWorkingStatus = false;

        /// <summary>
        /// 定时器模块的实例引用
        /// </summary>
        private NovaEngine.TimerModule _timerModule = null;
        /// <summary>
        /// 线程模块的实例引用
        /// </summary>
        private NovaEngine.ThreadModule _threadModule = null;
        /// <summary>
        /// 任务模块的实例引用
        /// </summary>
        private NovaEngine.TaskModule _taskModule = null;
        /// <summary>
        /// 网络模块的实例引用
        /// </summary>
        private NovaEngine.NetworkModule _networkModule = null;
        /// <summary>
        /// 资源模块的实例引用
        /// </summary>
        private NovaEngine.ResourceModule _resourceModule;
        /// <summary>
        /// 文件模块的实例引用
        /// </summary>
        private NovaEngine.FileModule _fileModule = null;
        /// <summary>
        /// 输入模块的实例引用
        /// </summary>
        private NovaEngine.InputModule _inputModule = null;
        /// <summary>
        /// 场景模块的实例引用
        /// </summary>
        private NovaEngine.SceneModule _sceneModule = null;

        /// <summary>
        /// 句柄子模块行为流程回调的缓存队列
        /// </summary>
        private IDictionary<SystemType, SystemDelegate> _cachedSubmoduleBehaviourCallbacks = null;

        /// <summary>
        /// 设置句柄的类型标识
        /// </summary>
        public int HandlerType { get { return _handlerType; } internal set { _handlerType = value; } }

        /// <summary>
        /// 获取句柄对象当前的任务调度状态标识
        /// </summary>
        protected internal bool IsOnWorkingStatus => _isOnWorkingStatus;

        /// <summary>
        /// 获取定时器模块的对象实例
        /// </summary>
        internal NovaEngine.TimerModule TimerModule => _timerModule;
        /// <summary>
        /// 获取线程模块的对象实例
        /// </summary>
        internal NovaEngine.ThreadModule ThreadModule => _threadModule;
        /// <summary>
        /// 获取任务模块的对象实例
        /// </summary>
        internal NovaEngine.TaskModule TaskModule => _taskModule;
        /// <summary>
        /// 获取网络模块的对象实例
        /// </summary>
        internal NovaEngine.NetworkModule NetworkModule => _networkModule;
        /// <summary>
        /// 获取输入模块的对象实例
        /// </summary>
        internal NovaEngine.InputModule InputModule => _inputModule;
        /// <summary>
        /// 获取资源模块的对象实例
        /// </summary>
        internal NovaEngine.ResourceModule ResourceModule => _resourceModule;
        /// <summary>
        /// 获取文件模块的对象实例
        /// </summary>
        internal NovaEngine.FileModule FileModule => _fileModule;
        /// <summary>
        /// 获取场景模块的对象实例
        /// </summary>
        internal NovaEngine.SceneModule SceneModule => _sceneModule;

        /// <summary>
        /// 句柄对象初始化接口函数
        /// </summary>
        /// <returns>若句柄对象初始化成功则返回true，否则返回false</returns>
        public bool Initialize()
        {
            // 初始化模块实例
            _timerModule = NovaEngine.Facade.Instance.GetModule<NovaEngine.TimerModule>();
            _threadModule = NovaEngine.Facade.Instance.GetModule<NovaEngine.ThreadModule>();
            _taskModule = NovaEngine.Facade.Instance.GetModule<NovaEngine.TaskModule>();
            _networkModule = NovaEngine.Facade.Instance.GetModule<NovaEngine.NetworkModule>();
            _inputModule = NovaEngine.Facade.Instance.GetModule<NovaEngine.InputModule>();
            _resourceModule = NovaEngine.Facade.Instance.GetModule<NovaEngine.ResourceModule>();
            _fileModule = NovaEngine.Facade.Instance.GetModule<NovaEngine.FileModule>();
            _sceneModule = NovaEngine.Facade.Instance.GetModule<NovaEngine.SceneModule>();

            // 初始化子模块行为流程缓存队列
            _cachedSubmoduleBehaviourCallbacks = new Dictionary<SystemType, SystemDelegate>();

            if (false == OnInitialize()) { return false; }

            // 子模块初始化
            OnSubmoduleInitCallback();

            return true;
        }

        /// <summary>
        /// 句柄对象清理接口函数
        /// </summary>
        public void Cleanup()
        {
            // 子模块清理
            OnSubmoduleCleanupCallback();

            OnCleanup();

            // 清理子模块行为流程缓存队列
            _cachedSubmoduleBehaviourCallbacks.Clear();
            _cachedSubmoduleBehaviourCallbacks = null;

            // 清理模块实例
            _timerModule = null;
            _threadModule = null;
            _taskModule = null;
            _networkModule = null;
            _inputModule = null;
            _resourceModule = null;
            _fileModule = null;
            _sceneModule = null;
        }

        /// <summary>
        /// 句柄对象重载接口函数
        /// </summary>
        public void Reload()
        {
            // 子模块重载
            OnSubmoduleReloadCallback();

            OnReload();
        }

        /// <summary>
        /// 句柄对象执行接口
        /// </summary>
        public void Execute()
        {
            _isOnWorkingStatus = true;

            OnExecute();

            _isOnWorkingStatus = false;
        }

        /// <summary>
        /// 句柄对象延迟执行接口
        /// </summary>
        public void LateExecute()
        {
            _isOnWorkingStatus = true;

            OnLateExecute();

            _isOnWorkingStatus = false;
        }

        /// <summary>
        /// 句柄对象刷新接口
        /// </summary>
        public void Update()
        {
            _isOnWorkingStatus = true;

            OnUpdate();

            _isOnWorkingStatus = false;
        }

        /// <summary>
        /// 句柄对象延迟刷新接口
        /// </summary>
        public void LateUpdate()
        {
            _isOnWorkingStatus = true;

            OnLateUpdate();

            _isOnWorkingStatus = false;
        }

        /// <summary>
        /// 句柄对象内置初始化接口函数
        /// </summary>
        /// <returns>若句柄对象初始化成功则返回true，否则返回false</returns>
        protected abstract bool OnInitialize();

        /// <summary>
        /// 句柄对象内置清理接口函数
        /// </summary>
        protected abstract void OnCleanup();

        /// <summary>
        /// 句柄对象内置重载接口函数
        /// </summary>
        protected abstract void OnReload();

        /// <summary>
        /// 句柄对象内置执行接口
        /// </summary>
        protected abstract void OnExecute();

        /// <summary>
        /// 句柄对象内置延迟执行接口
        /// </summary>
        protected abstract void OnLateExecute();

        /// <summary>
        /// 句柄对象内置刷新接口
        /// </summary>
        protected abstract void OnUpdate();

        /// <summary>
        /// 句柄对象内置延迟刷新接口
        /// </summary>
        protected abstract void OnLateUpdate();

        /// <summary>
        /// 句柄对象的模块事件转发回调接口
        /// </summary>
        /// <param name="e">模块事件参数</param>
        public abstract void OnEventDispatch(NovaEngine.ModuleEventArgs e);

        #region 句柄子模块调度管理接口函数

        /// <summary>
        /// 句柄对象子模块初始化回调处理接口函数
        /// </summary>
        private void OnSubmoduleInitCallback()
        {
            OnSubmoduleActionCallbackOfTargetAttribute(typeof(OnSubmoduleInitCallbackAttribute));
        }

        /// <summary>
        /// 句柄对象子模块清理回调处理接口函数
        /// </summary>
        private void OnSubmoduleCleanupCallback()
        {
            OnSubmoduleActionCallbackOfTargetAttribute(typeof(OnSubmoduleCleanupCallbackAttribute));
        }

        /// <summary>
        /// 句柄对象子模块重载回调处理接口函数
        /// </summary>
        private void OnSubmoduleReloadCallback()
        {
            OnSubmoduleActionCallbackOfTargetAttribute(typeof(OnSubmoduleReloadCallbackAttribute));
        }

        /// <summary>
        /// 句柄对象子模块指定类型的回调函数触发处理接口
        /// </summary>
        /// <param name="attrType">属性类型</param>
        private void OnSubmoduleActionCallbackOfTargetAttribute(SystemType attrType)
        {
            SystemDelegate callback;
            if (TryGetSubmoduleBehaviourCallback(attrType, out callback))
            {
                callback.DynamicInvoke();
            }
        }

        private bool TryGetSubmoduleBehaviourCallback(SystemType targetType, out SystemDelegate callback)
        {
            SystemDelegate handler;
            if (_cachedSubmoduleBehaviourCallbacks.TryGetValue(targetType, out handler))
            {
                callback = handler;
                return null == callback ? false : true;
            }

            IList<SystemDelegate> list = new List<SystemDelegate>();
            SystemType classType = GetType();
            SystemMethodInfo[] methods = classType.GetMethods(SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Instance);
            for (int n = 0; n < methods.Length; ++n)
            {
                SystemMethodInfo method = methods[n];
                IEnumerable<SystemAttribute> e = method.GetCustomAttributes();
                SystemAttribute attr = method.GetCustomAttribute(targetType);
                if (null != attr)
                {
                    SystemDelegate c = method.CreateDelegate(typeof(NovaEngine.Definition.Delegate.EmptyFunctionHandler), this);
                    list.Add(c);
                }
            }

            // 先重置回调
            callback = null;

            if (list.Count > 0)
            {
                for (int n = 0; n < list.Count; ++n)
                {
                    if (null == callback)
                    {
                        callback = list[n];
                    }
                    else
                    {
                        callback = SystemDelegate.Combine(list[n], callback);
                    }
                }

                _cachedSubmoduleBehaviourCallbacks.Add(targetType, callback);
                return true;
            }

            _cachedSubmoduleBehaviourCallbacks.Add(targetType, callback);
            return false;
        }

        #endregion

        #region 切面控制层提供的服务回调函数

        /// <summary>
        /// 支持切面控制的函数调用接口
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="f">目标函数</param>
        protected void Call(CBase obj, System.Action f)
        {
            Debugger.Assert(null != obj && obj == f.Target, NovaEngine.ErrorText.InvalidArguments);

            obj.Call(f);
        }

        /// <summary>
        /// 支持切面控制的函数调用接口
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="f">目标函数</param>
        /// <param name="lifecycleType">生命周期类型</param>
        protected void Call(CBase obj, System.Action f, AspectBehaviourType lifecycleType)
        {
            Debugger.Assert(null != obj && obj == f.Target, NovaEngine.ErrorText.InvalidArguments);

            obj.Call(f, lifecycleType);
        }

        /// <summary>
        /// 支持切面控制的函数调用接口
        /// </summary>
        /// <param name="f">目标函数</param>
        protected void Call(System.Action f)
        {
            if (null != f.Target && typeof(CBase).IsAssignableFrom(f.Target.GetType()))
            {
                (f.Target as CBase).Call(f);
            }
            else
            {
                AspectController.Instance.Call(f);
            }
        }

        /// <summary>
        /// 支持切面控制的函数调用接口
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="func">目标函数</param>
        /// <param name="arg0">参数值</param>
        protected void Call<T>(System.Action<T> func, T arg0)
        {
            if (null != func.Target && typeof(CBase).IsAssignableFrom(func.Target.GetType()))
            {
                (func.Target as CBase).Call(func, arg0);
            }
            else
            {
                AspectController.Instance.Call<T>(func, arg0);
            }
        }

        /// <summary>
        /// 支持切面控制的带返回值的函数调用接口
        /// </summary>
        /// <typeparam name="V">返回值类型</typeparam>
        /// <param name="func">目标函数</param>
        /// <returns>返回目标函数调用后的返回结果</returns>
        protected V Call<V>(System.Func<V> func)
        {
            if (null != func.Target && typeof(CBase).IsAssignableFrom(func.Target.GetType()))
            {
                return (func.Target as CBase).Call<V>(func);
            }
            else
            {
                return AspectController.Instance.Call<V>(func);
            }
        }

        /// <summary>
        /// 支持切面控制的带返回值的函数调用接口
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <typeparam name="V">返回值类型</typeparam>
        /// <param name="func">目标函数</param>
        /// <param name="arg0">参数值</param>
        /// <returns>返回目标函数调用后的返回结果</returns>
        protected V Call<T, V>(System.Func<T, V> func, T arg0)
        {
            if (null != func.Target && typeof(CBase).IsAssignableFrom(func.Target.GetType()))
            {
                return (func.Target as CBase).Call<T, V>(func, arg0);
            }
            else
            {
                return AspectController.Instance.Call<T, V>(func, arg0);
            }
        }

        #endregion

        #region 类的实例化提供的操作处理函数

        /// <summary>
        /// 通过指定的对象类型，创建一个其对应的对象实例
        /// </summary>
        /// <param name="classType">对象类型</param>
        /// <returns>返回给定类型生成的对象实例，若实例生成失败则返回null</returns>
        protected internal static object CreateInstance(SystemType classType)
        {
            return PoolController.Instance.CreateObject(classType);
        }

        /// <summary>
        /// 释放指定的对象实例
        /// </summary>
        /// <param name="instance">对象实例</param>
        protected internal static void ReleaseInstance(object instance)
        {
            PoolController.Instance.ReleaseObject(instance);
        }

        #endregion
    }
}
