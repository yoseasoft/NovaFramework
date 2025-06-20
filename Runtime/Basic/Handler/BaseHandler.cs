/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;
using SystemDelegate = System.Delegate;
using SystemActivator = System.Activator;
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
        /// 句柄对象的类型标识
        /// </summary>
        private int m_handlerType = 0;

        /// <summary>
        /// 句柄对象当前是否处于刷新逻辑的状态标识
        /// </summary>
        private bool m_isOnUpdatingStatus = false;

        /// <summary>
        /// 定时器模块的实例引用
        /// </summary>
        private NovaEngine.TimerModule m_timerModule = null;
        /// <summary>
        /// 线程模块的实例引用
        /// </summary>
        private NovaEngine.ThreadModule m_threadModule = null;
        /// <summary>
        /// 任务模块的实例引用
        /// </summary>
        private NovaEngine.TaskModule m_taskModule = null;
        /// <summary>
        /// 网络模块的实例引用
        /// </summary>
        private NovaEngine.NetworkModule m_networkModule = null;
        /// <summary>
        /// 资源模块的实例引用
        /// </summary>
        private NovaEngine.ResourceModule m_resourceModule;
        /// <summary>
        /// 文件模块的实例引用
        /// </summary>
        private NovaEngine.FileModule m_fileModule = null;
        /// <summary>
        /// 输入模块的实例引用
        /// </summary>
        private NovaEngine.InputModule m_inputModule = null;
        /// <summary>
        /// 场景模块的实例引用
        /// </summary>
        private NovaEngine.SceneModule m_sceneModule = null;

        /// <summary>
        /// 句柄子模块行为流程回调的缓存队列
        /// </summary>
        private IDictionary<SystemType, SystemDelegate> m_cachedSubmoduleBehaviourCallbacks = null;

        /// <summary>
        /// 设置句柄的类型标识
        /// </summary>
        public int HandlerType { get { return m_handlerType; } internal set { m_handlerType = value; } }

        /// <summary>
        /// 获取句柄对象当前的刷新状态标识
        /// </summary>
        protected internal bool IsOnUpdatingStatus => m_isOnUpdatingStatus;

        /// <summary>
        /// 获取定时器模块的对象实例
        /// </summary>
        public NovaEngine.TimerModule TimerModule => m_timerModule;
        /// <summary>
        /// 获取线程模块的对象实例
        /// </summary>
        public NovaEngine.ThreadModule ThreadModule => m_threadModule;
        /// <summary>
        /// 获取任务模块的对象实例
        /// </summary>
        public NovaEngine.TaskModule TaskModule => m_taskModule;
        /// <summary>
        /// 获取网络模块的对象实例
        /// </summary>
        public NovaEngine.NetworkModule NetworkModule => m_networkModule;
        /// <summary>
        /// 获取输入模块的对象实例
        /// </summary>
        public NovaEngine.InputModule InputModule => m_inputModule;
        /// <summary>
        /// 获取资源模块的对象实例
        /// </summary>
        public NovaEngine.ResourceModule ResourceModule => m_resourceModule;
        /// <summary>
        /// 获取文件模块的对象实例
        /// </summary>
        public NovaEngine.FileModule FileModule => m_fileModule;
        /// <summary>
        /// 获取场景模块的对象实例
        /// </summary>
        public NovaEngine.SceneModule SceneModule => m_sceneModule;

        /// <summary>
        /// 句柄对象初始化接口函数
        /// </summary>
        /// <returns>若句柄对象初始化成功则返回true，否则返回false</returns>
        public bool Initialize()
        {
            // 初始化模块实例
            m_timerModule = NovaEngine.Facade.Instance.GetModule<NovaEngine.TimerModule>();
            m_threadModule = NovaEngine.Facade.Instance.GetModule<NovaEngine.ThreadModule>();
            m_taskModule = NovaEngine.Facade.Instance.GetModule<NovaEngine.TaskModule>();
            m_networkModule = NovaEngine.Facade.Instance.GetModule<NovaEngine.NetworkModule>();
            m_inputModule = NovaEngine.Facade.Instance.GetModule<NovaEngine.InputModule>();
            m_resourceModule = NovaEngine.Facade.Instance.GetModule<NovaEngine.ResourceModule>();
            m_fileModule = NovaEngine.Facade.Instance.GetModule<NovaEngine.FileModule>();
            m_sceneModule = NovaEngine.Facade.Instance.GetModule<NovaEngine.SceneModule>();

            // 初始化子模块行为流程缓存队列
            m_cachedSubmoduleBehaviourCallbacks = new Dictionary<SystemType, SystemDelegate>();

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
            m_cachedSubmoduleBehaviourCallbacks.Clear();
            m_cachedSubmoduleBehaviourCallbacks = null;

            // 清理模块实例
            m_timerModule = null;
            m_threadModule = null;
            m_taskModule = null;
            m_networkModule = null;
            m_inputModule = null;
            m_resourceModule = null;
            m_fileModule = null;
            m_sceneModule = null;
        }

        /// <summary>
        /// 句柄对象刷新接口
        /// </summary>
        public void Update()
        {
            m_isOnUpdatingStatus = true;

            OnUpdate();

            m_isOnUpdatingStatus = false;
        }

        /// <summary>
        /// 句柄对象延迟刷新接口
        /// </summary>
        public void LateUpdate()
        {
            m_isOnUpdatingStatus = true;

            OnLateUpdate();

            m_isOnUpdatingStatus = false;
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
            if (m_cachedSubmoduleBehaviourCallbacks.TryGetValue(targetType, out handler))
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

                m_cachedSubmoduleBehaviourCallbacks.Add(targetType, callback);
                return true;
            }

            m_cachedSubmoduleBehaviourCallbacks.Add(targetType, callback);
            return false;
        }

        #endregion

        #region 切面控制层提供的服务回调函数

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
