/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemType = System.Type;
using SystemDelegate = System.Delegate;
using SystemMethodInfo = System.Reflection.MethodInfo;

namespace GameEngine
{
    /// <summary>
    /// 基础对象抽象类，对需要进行对象定义的场景提供一个通用的基类
    /// </summary>
    public abstract partial class CBase : CBean, NovaEngine.IReference, NovaEngine.ILaunchable, IInputDispatch, IEventDispatch, IMessageDispatch
    {
        /// <summary>
        /// 对象生命周期关键点类型的枚举定义
        /// </summary>
        public enum LifecycleKeypointType : byte
        {
            /// <summary>
            /// 未知
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// 初始化节点
            /// </summary>
            Initialize,

            /// <summary>
            /// 启动节点
            /// </summary>
            Startup,

            /// <summary>
            /// 唤醒节点
            /// </summary>
            Awake,

            /// <summary>
            /// 开始节点
            /// </summary>
            Start,

            /// <summary>
            /// 闲置节点
            /// </summary>
            Idle,

            /// <summary>
            /// 工作节点
            /// </summary>
            Work,

            /// <summary>
            /// 销毁节点
            /// </summary>
            Destroy,

            /// <summary>
            /// 关闭节点
            /// </summary>
            Shutdown,

            /// <summary>
            /// 清理节点
            /// </summary>
            Cleanup,

            /// <summary>
            /// 释放节点
            /// </summary>
            Free,
        }

        /// <summary>
        /// 对象当前生命周期的运行步骤
        /// </summary>
        private LifecycleKeypointType _currentLifecycleRunningStep = LifecycleKeypointType.Unknown;

        /// <summary>
        /// 对象当前生命周期的调度状态标识
        /// </summary>
        private bool _currentLifecycleSchedulingStatus = false;

        /// <summary>
        /// 获取对象当前生命周期的运行步骤
        /// </summary>
        public LifecycleKeypointType CurrentLifecycleRunningStep => _currentLifecycleRunningStep;

        /// <summary>
        /// 获取对象当前生命周期的调度状态
        /// </summary>
        public bool CurrentLifecycleScheduleRunning => _currentLifecycleSchedulingStatus;

        /// <summary>
        /// 对象初始化函数接口
        /// </summary>
        public override void Initialize()
        {
            // 初始化通信工具包
            OnCommuPackageInitialize();
        }

        /// <summary>
        /// 对象清理函数接口
        /// </summary>
        public override void Cleanup()
        {
            // 清理通信工具包
            OnCommuPackageCleanup();
        }

        /// <summary>
        /// 对象重载函数接口
        /// </summary>
        internal override void Reload()
        {
            base.Reload();

            // 卸载自动绑定的扩展通知接口
            this.RemoveAllAutomaticallyInputResponses();
            this.UnsubscribeAllAutomaticallyEvents();
            this.RemoveAllAutomaticallyMessageListeners();
        }

        /// <summary>
        /// 对象启动通知函数接口
        /// </summary>
        public abstract void Startup();

        /// <summary>
        /// 对象关闭通知函数接口
        /// </summary>
        public abstract void Shutdown();

        #region 对象生命周期阶段管理相关接口函数

        /// <summary>
        /// 检测当前对象是否处于目标生命周期类型的状态中
        /// </summary>
        /// <param name="lifecycleType">生命周期类型</param>
        /// <returns>若对象处于给定生命周期类型的状态中则返回true，否则返回false</returns>
        protected internal bool IsOnTargetLifecycle(LifecycleKeypointType lifecycleType)
        {
            Debugger.Assert(LifecycleKeypointType.Unknown != _currentLifecycleRunningStep, "Invalid current lifecycle value.");

            if (_currentLifecycleRunningStep == lifecycleType)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检测当前对象是否处于目标生命周期的调度过程中
        /// </summary>
        /// <param name="lifecycleType">生命周期类型</param>
        /// <returns>若对象处于给定生命周期类型的调度中则返回true，否则返回false</returns>
        protected internal bool IsOnSchedulingProcessForTargetLifecycle(LifecycleKeypointType lifecycleType)
        {
            Debugger.Assert(LifecycleKeypointType.Unknown != _currentLifecycleRunningStep, "Invalid current lifecycle value.");

            if (_currentLifecycleRunningStep == lifecycleType && _currentLifecycleSchedulingStatus)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检测指定的生命周期对于当前对象是否处于完成阶段
        /// </summary>
        /// <param name="lifecycleType">生命周期类型</param>
        /// <returns>若对象已完成给定生命周期类型则返回true，否则返回false</returns>
        protected internal bool IsOnTargetLifecycleSchedulingCompleted(LifecycleKeypointType lifecycleType)
        {
            Debugger.Assert(LifecycleKeypointType.Unknown != _currentLifecycleRunningStep, "Invalid current lifecycle value.");

            if (_currentLifecycleRunningStep == lifecycleType && false == _currentLifecycleSchedulingStatus)
            {
                return true;
            }

            if (_currentLifecycleRunningStep > lifecycleType)
            {
                // 除了work和idle，正常情况下生命周期的调用步骤的按序增长的
                if (LifecycleKeypointType.Work != lifecycleType && LifecycleKeypointType.Idle != lifecycleType)
                {
                    return true;
                }
                else if (false == _currentLifecycleSchedulingStatus)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检测当前对象的生命周期是否处于指定的范围区间内<br/>
        /// 区间范围的取值为大于等于起始范围类型，小于结束范围类型
        /// </summary>
        /// <param name="beginType">开始范围类型</param>
        /// <param name="endType">结束范围类型</param>
        /// <returns>若对象处于给定生命周期范围内则返回true，否则返回false</returns>
        private bool IsInTargetLifecycleRange(LifecycleKeypointType beginType, LifecycleKeypointType endType)
        {
            Debugger.Assert(LifecycleKeypointType.Unknown != _currentLifecycleRunningStep, "Invalid current lifecycle value.");

            if (_currentLifecycleRunningStep >= beginType && _currentLifecycleRunningStep < endType)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检测当前对象的生命周期是否处于唤醒激活状态
        /// </summary>
        /// <returns>若对象处于给定生命周期状态则返回true，否则返回false</returns>
        protected internal bool IsOnAwakingStatus()
        {
            return IsInTargetLifecycleRange(LifecycleKeypointType.Awake, LifecycleKeypointType.Destroy);
        }

        /// <summary>
        /// 检测当前对象的生命周期是否处于开始激活状态
        /// </summary>
        /// <returns>若对象处于给定生命周期状态则返回true，否则返回false</returns>
        protected internal bool IsOnStartingStatus()
        {
            return IsInTargetLifecycleRange(LifecycleKeypointType.Start, LifecycleKeypointType.Destroy);
        }

        /// <summary>
        /// 检测当前对象的生命周期是否处于开始激活状态
        /// </summary>
        /// <returns>若对象处于给定生命周期状态则返回true，否则返回false</returns>
        protected internal bool IsOnDestroyingStatus()
        {
            return IsInTargetLifecycleRange(LifecycleKeypointType.Destroy, LifecycleKeypointType.Free);
        }

        /// <summary>
        /// 检测当前对象的生命周期是否处于刷新激活状态
        /// </summary>
        /// <returns>若对象处于给定生命周期状态则返回true，否则返回false</returns>
        protected internal bool IsOnWorkingStatus()
        {
            return IsOnTargetLifecycle(LifecycleKeypointType.Work);
        }

        /// <summary>
        /// 在函数调用前的对象生命周期改变处理的操作函数
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="method">函数实例</param>
        protected static void OnLifecycleChangedBeforeMethodInvoke(object obj, SystemMethodInfo method)
        {
            if (null == method)
            {
                Debugger.Warn("The lifecycle changed method must be non-null.");
                return;
            }

            OnLifecycleChangedBeforeMethodInvoke(obj, method.Name);
        }

        /// <summary>
        /// 在函数调用前的对象生命周期改变处理的操作函数
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="methodName">函数名称</param>
        protected static void OnLifecycleChangedBeforeMethodInvoke(object obj, string methodName)
        {
            if (NovaEngine.Environment.IsDevelopmentState())
            {
                if (false == typeof(CBase).IsAssignableFrom(obj.GetType()))
                {
                    Debugger.Warn("The lifecycle changed target object '{0}' must be inherited from 'CBase'.", obj.GetType().FullName);
                    return;
                }
            }

            OnLifecycleChangedByMethodName(obj as CBase, methodName, true);
        }

        /// <summary>
        /// 在函数调用前的对象生命周期改变处理的操作函数
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="method">函数实例</param>
        /// <param name="lifecycleType">生命周期类型</param>
        protected static void OnLifecycleChangedBeforeMethodInvoke(object obj, SystemMethodInfo method, LifecycleKeypointType lifecycleType)
        {
            if (null == method)
            {
                Debugger.Warn("The lifecycle changed method must be non-null.");
                return;
            }

            OnLifecycleChangedBeforeMethodInvoke(obj, method.Name, lifecycleType);
        }

        /// <summary>
        /// 在函数调用前的对象生命周期改变处理的操作函数
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="methodName">函数名称</param>
        /// <param name="lifecycleType">生命周期类型</param>
        protected static void OnLifecycleChangedBeforeMethodInvoke(object obj, string methodName, LifecycleKeypointType lifecycleType)
        {
            if (NovaEngine.Environment.IsDevelopmentState())
            {
                if (false == typeof(CBase).IsAssignableFrom(obj.GetType()))
                {
                    Debugger.Warn("The lifecycle changed target object '{0}' must be inherited from 'CBase'.", obj.GetType().FullName);
                    return;
                }
            }

            OnLifecycleChangedByMethodName(obj as CBase, methodName, lifecycleType, true);
        }

        /// <summary>
        /// 在函数调用后的对象生命周期改变处理的操作函数
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="method">函数实例</param>
        protected static void OnLifecycleChangedAfterMethodInvoke(object obj, SystemMethodInfo method)
        {
            if (null == method)
            {
                Debugger.Warn("The lifecycle changed method must be non-null.");
                return;
            }

            OnLifecycleChangedAfterMethodInvoke(obj, method.Name);
        }

        /// <summary>
        /// 在函数调用前的对象生命周期改变处理的操作函数
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="methodName">函数名称</param>
        protected static void OnLifecycleChangedAfterMethodInvoke(object obj, string methodName)
        {
            if (NovaEngine.Environment.IsDevelopmentState())
            {
                if (false == typeof(CBase).IsAssignableFrom(obj.GetType()))
                {
                    Debugger.Warn("The lifecycle changed target object '{0}' must be inherited from 'CBase'.", obj.GetType().FullName);
                    return;
                }
            }

            OnLifecycleChangedByMethodName(obj as CBase, methodName, false);
        }

        /// <summary>
        /// 在函数调用后的对象生命周期改变处理的操作函数
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="method">函数实例</param>
        /// <param name="lifecycleType">生命周期类型</param>
        protected static void OnLifecycleChangedAfterMethodInvoke(object obj, SystemMethodInfo method, LifecycleKeypointType lifecycleType)
        {
            if (null == method)
            {
                Debugger.Warn("The lifecycle changed method must be non-null.");
                return;
            }

            OnLifecycleChangedAfterMethodInvoke(obj, method.Name, lifecycleType);
        }

        /// <summary>
        /// 在函数调用前的对象生命周期改变处理的操作函数
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="methodName">函数名称</param>
        /// <param name="lifecycleType">生命周期类型</param>
        protected static void OnLifecycleChangedAfterMethodInvoke(object obj, string methodName, LifecycleKeypointType lifecycleType)
        {
            if (NovaEngine.Environment.IsDevelopmentState())
            {
                if (false == typeof(CBase).IsAssignableFrom(obj.GetType()))
                {
                    Debugger.Warn("The lifecycle changed target object '{0}' must be inherited from 'CBase'.", obj.GetType().FullName);
                    return;
                }
            }

            OnLifecycleChangedByMethodName(obj as CBase, methodName, lifecycleType, false);
        }

        /// <summary>
        /// 通过指定的函数名称和调用位置，改变目标对象的生命周期步骤
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="methodName">函数名称</param>
        /// <param name="isBefore">调用位置标识</param>
        private static void OnLifecycleChangedByMethodName(CBase obj, string methodName, bool isBefore)
        {
            LifecycleKeypointType lifecycleType = GetLifecycleTypeByMethodName(methodName, isBefore);

            OnLifecycleChangedByMethodName(obj, methodName, lifecycleType, isBefore);
        }

        /// <summary>
        /// 通过指定的函数名称和调用位置，改变目标对象的生命周期步骤
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="methodName">函数名称</param>
        /// <param name="lifecycleType">生命周期类型</param>
        /// <param name="isBefore">调用位置标识</param>
        private static void OnLifecycleChangedByMethodName(CBase obj, string methodName, LifecycleKeypointType lifecycleType, bool isBefore)
        {
            if (LifecycleKeypointType.Unknown == lifecycleType)
            {
                // 扩展函数调用，不需要进行生命周期变更
                return;
            }

            if (NovaEngine.Environment.IsDevelopmentState())
            {
                if (obj._currentLifecycleRunningStep > lifecycleType)
                {
                    // 除了work和idle，正常情况下生命周期的调用步骤的按序增长的
                    if (LifecycleKeypointType.Work != lifecycleType && LifecycleKeypointType.Idle != lifecycleType)
                    {
                        Debugger.Error("Invalid object lifecycle changed from '{0}' to '{1}', please checked target method '{2}.{3}' invoke is legal.",
                                obj._currentLifecycleRunningStep, lifecycleType, obj.GetType().FullName, methodName);
                        return;
                    }
                }
            }

            // cleanup函数调用结束后，将生命周期步骤重置为unknown状态
            if (LifecycleKeypointType.Cleanup == lifecycleType && !isBefore)
            {
                lifecycleType = LifecycleKeypointType.Unknown;
            }

            // Debugger.Info("Changed object '{0}' lifecycle type from '{1}' to '{2}' with target method '{3}'.", obj.GetType().FullName, obj.m_currentLifecycleRunningStep, lifecycleType, methodName);

            obj._currentLifecycleRunningStep = lifecycleType;
            obj._currentLifecycleSchedulingStatus = isBefore;
        }

        /// <summary>
        /// 通过指定的函数名称和调用位置，获取对应的生命周期步骤
        /// </summary>
        /// <param name="methodName">函数名称</param>
        /// <param name="isBefore">调用位置标识</param>
        /// <returns>返回生命周期步骤类型，若不存在对应步骤则返回unknown状态</returns>
        private static LifecycleKeypointType GetLifecycleTypeByMethodName(string methodName, bool isBefore)
        {
            LifecycleKeypointType lifecycleType;
            if (ProtoController.Instance.TryGetProtoMethodLifecycleType(methodName, isBefore, out lifecycleType))
            {
                return lifecycleType;
            }

            AspectBehaviourType behaviourType = NovaEngine.Utility.Convertion.GetEnumFromString<AspectBehaviourType>(methodName);
            lifecycleType = LifecycleKeypointType.Unknown;

            switch (behaviourType)
            {
                case AspectBehaviourType.Initialize:
                    if (isBefore) lifecycleType = LifecycleKeypointType.Initialize;
                    break;
                case AspectBehaviourType.Startup:
                    if (isBefore) lifecycleType = LifecycleKeypointType.Startup;
                    break;
                case AspectBehaviourType.Awake:
                    if (isBefore) lifecycleType = LifecycleKeypointType.Awake;
                    break;
                case AspectBehaviourType.Start:
                    if (isBefore) lifecycleType = LifecycleKeypointType.Start;
                    break;
                case AspectBehaviourType.Update:
                case AspectBehaviourType.LateUpdate:
                    if (isBefore) lifecycleType = LifecycleKeypointType.Work;
                    else lifecycleType = LifecycleKeypointType.Idle;
                    break;
                case AspectBehaviourType.Destroy:
                    if (isBefore) lifecycleType = LifecycleKeypointType.Destroy;
                    break;
                case AspectBehaviourType.Shutdown:
                    if (isBefore) lifecycleType = LifecycleKeypointType.Shutdown;
                    break;
                case AspectBehaviourType.Cleanup:
                    lifecycleType = LifecycleKeypointType.Cleanup;
                    break;
            }

            // 缓存对应类型
            ProtoController.Instance.AddProtoMethodLifecycleType(methodName, isBefore, lifecycleType);

            return lifecycleType;
        }

        #endregion

        #region 切面控制层提供的服务回调函数

        /// <summary>
        /// 支持切面控制的函数调用接口
        /// </summary>
        /// <param name="methodName">函数名称</param>
        public void Call(string methodName)
        {
            OnLifecycleChangedBeforeMethodInvoke(this, methodName);

            AspectController.Instance.Call(this, methodName);

            OnLifecycleChangedAfterMethodInvoke(this, methodName);
        }

        /// <summary>
        /// 进行异常拦截的切面控制函数调用接口
        /// </summary>
        /// <param name="methodName">函数名称</param>
        /// <returns>正常调用结束返回true，发生异常返回false</returns>
        public bool Pcall(string methodName)
        {
            try { Call(methodName); } catch (System.Exception e) { Debugger.Error(e); return false; }

            return true;
        }

        /// <summary>
        /// 支持切面控制的函数调用接口
        /// </summary>
        /// <param name="f">目标函数</param>
        protected internal void Call(System.Action f)
        {
            OnLifecycleChangedBeforeMethodInvoke(f.Target, f.Method);

            AspectController.Instance.Call(f);

            OnLifecycleChangedAfterMethodInvoke(f.Target, f.Method);
        }

        /// <summary>
        /// 支持切面控制的函数调用接口
        /// </summary>
        /// <param name="f">目标函数</param>
        /// <param name="lifecycleType">生命周期类型</param>
        protected internal void Call(System.Action f, LifecycleKeypointType lifecycleType)
        {
            OnLifecycleChangedBeforeMethodInvoke(f.Target, f.Method, lifecycleType);

            AspectController.Instance.Call(f);

            OnLifecycleChangedAfterMethodInvoke(f.Target, f.Method, lifecycleType);
        }

        /// <summary>
        /// 进行异常拦截的切面控制函数调用接口
        /// </summary>
        /// <param name="f">目标函数</param>
        /// <returns>正常调用结束返回true，发生异常返回false</returns>
        protected internal bool Pcall(System.Action f)
        {
            try { Call(f); } catch (System.Exception e) { Debugger.Error(e); return false; }

            return true;
        }

        /// <summary>
        /// 支持切面控制的函数调用接口
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="func">目标函数</param>
        /// <param name="arg0">参数值</param>
        protected internal void Call<T>(System.Action<T> func, T arg0)
        {
            AspectController.Instance.Call<T>(func, arg0);
        }

        /// <summary>
        /// 进行异常拦截的切面控制函数调用接口
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="func">目标函数</param>
        /// <param name="arg0">参数值</param>
        /// <returns>正常调用结束返回true，发生异常返回false</returns>
        protected internal bool Pcall<T>(System.Action<T> func, T arg0)
        {
            try { Call<T>(func, arg0); } catch (System.Exception e) { Debugger.Error(e); return false; }

            return true;
        }

        /// <summary>
        /// 支持切面控制的带返回值的函数调用接口
        /// </summary>
        /// <typeparam name="V">返回值类型</typeparam>
        /// <param name="func">目标函数</param>
        /// <returns>返回目标函数调用后的返回结果</returns>
        protected internal V Call<V>(System.Func<V> func)
        {
            return AspectController.Instance.Call<V>(func);
        }

        /// <summary>
        /// 进行异常拦截的切面控制的带返回值函数调用接口
        /// </summary>
        /// <typeparam name="V">返回值类型</typeparam>
        /// <param name="func">目标函数</param>
        /// <param name="value">目标函数返回值</param>
        /// <returns>正常调用结束返回true，发生异常返回false</returns>
        protected internal bool Pcall<V>(System.Func<V> func, out V value)
        {
            try { value = Call<V>(func); } catch (System.Exception e) { Debugger.Error(e); value = default(V); return false; }

            return true;
        }

        /// <summary>
        /// 支持切面控制的带返回值的函数调用接口
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <typeparam name="V">返回值类型</typeparam>
        /// <param name="func">目标函数</param>
        /// <param name="arg0">参数值</param>
        /// <returns>返回目标函数调用后的返回结果</returns>
        protected internal V Call<T, V>(System.Func<T, V> func, T arg0)
        {
            return AspectController.Instance.Call<T, V>(func, arg0);
        }

        /// <summary>
        /// 进行异常拦截的切面控制的带返回值函数调用接口
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <typeparam name="V">返回值类型</typeparam>
        /// <param name="func">目标函数</param>
        /// <param name="arg0">参数值</param>
        /// <param name="value">目标函数返回值</param>
        /// <returns>正常调用结束返回true，发生异常返回false</returns>
        protected internal bool Pcall<T, V>(System.Func<T, V> func, T arg0, out V value)
        {
            try { value = Call<T, V>(func, arg0); } catch (System.Exception e) { Debugger.Error(e); value = default(V); return false; }

            return true;
        }

        #endregion

        #region 事件分发层提供的服务回调函数

        /// <summary>
        /// 发送事件消息到事件管理器中等待派发
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件参数列表</param>
        public void Send(int eventID, params object[] args)
        {
            EventController.Instance.Send(eventID, args);
        }

        /// <summary>
        /// 发送事件消息到事件管理器中等待派发
        /// </summary>
        /// <param name="arg">事件数据</param>
        public void Send<T>(T arg) where T : struct
        {
            EventController.Instance.Send<T>(arg);
        }

        /// <summary>
        /// 发送事件消息到事件管理器中并立即处理掉它
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件参数列表</param>
        public void Fire(int eventID, params object[] args)
        {
            EventController.Instance.Fire(eventID, args);
        }

        /// <summary>
        /// 发送事件消息到事件管理器中并立即处理掉它
        /// </summary>
        /// <param name="arg">事件数据</param>
        public void Fire<T>(T arg) where T : struct
        {
            EventController.Instance.Fire<T>(arg);
        }

        #endregion
    }
}
