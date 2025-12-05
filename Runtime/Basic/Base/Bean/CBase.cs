/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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
using System.Runtime.CompilerServices;

namespace GameEngine
{
    /// <summary>
    /// 基础对象抽象类，对需要进行对象定义的场景提供一个通用的基类
    /// </summary>
    public abstract partial class CBase : CBean,
            NovaEngine.IReference, NovaEngine.ILaunchable, IInputDispatch, IEventDispatch, IMessageDispatch
    {
        /// <summary>
        /// 对象当前生命周期类型
        /// </summary>
        private AspectBehaviourType _currentLifecycleType = AspectBehaviourType.Unknown;

        /// <summary>
        /// 对象当前生命周期的运行状态标识
        /// </summary>
        private bool _isCurrentLifecycleTypeRunning = false;

        /// <summary>
        /// 获取对象当前生命周期类型
        /// </summary>
        public AspectBehaviourType CurrentLifecycleType => _currentLifecycleType;

        /// <summary>
        /// 获取对象当前生命周期的运行状态
        /// </summary>
        public bool IsCurrentLifecycleTypeRunning => _isCurrentLifecycleTypeRunning;

        /// <summary>
        /// 对象初始化函数接口
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // 初始化转发回调
            OnDispatchCallInitialize();
        }

        /// <summary>
        /// 对象清理函数接口
        /// </summary>
        public override void Cleanup()
        {
            // 清理转发回调
            OnDispatchCallCleanup();

            base.Cleanup();
        }

        /// <summary>
        /// 对象重载函数接口
        /// </summary>
        public override void Reload()
        {
            base.Reload();

            // 重载转发回调
            OnDispatchCallReload();
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal bool IsOnTargetLifecycle(AspectBehaviourType lifecycleType)
        {
            Debugger.Assert(AspectBehaviourType.Unknown != _currentLifecycleType, "Invalid current lifecycle value.");

            if (_currentLifecycleType == lifecycleType)
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal bool IsOnProcessingForTargetLifecycle(AspectBehaviourType lifecycleType)
        {
            Debugger.Assert(AspectBehaviourType.Unknown != _currentLifecycleType, "Invalid current lifecycle value.");

            if (_currentLifecycleType == lifecycleType && _isCurrentLifecycleTypeRunning)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检测当前对象的生命周期是否处于指定的范围区间内<br/>
        /// 区间范围的取值为大于等于起始范围类型，小于结束范围类型
        /// </summary>
        /// <param name="began">开始范围类型</param>
        /// <param name="ended">结束范围类型</param>
        /// <returns>若对象处于给定生命周期范围内则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsTheLifecycleTypeBetweenOfTargetRanges(AspectBehaviourType began, AspectBehaviourType ended)
        {
            Debugger.Assert(AspectBehaviourType.Unknown != _currentLifecycleType, "Invalid current lifecycle value.");

            if (_currentLifecycleType >= began && _currentLifecycleType < ended)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检测当前对象的生命周期是否处于唤醒激活状态
        /// </summary>
        /// <returns>若对象处于给定生命周期状态则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal bool IsOnAwakingStatus()
        {
            return IsTheLifecycleTypeBetweenOfTargetRanges(AspectBehaviourType.Awake, AspectBehaviourType.Destroy);
        }

        /// <summary>
        /// 检测当前对象的生命周期是否处于开始激活状态
        /// </summary>
        /// <returns>若对象处于给定生命周期状态则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal bool IsOnStartingStatus()
        {
            return IsTheLifecycleTypeBetweenOfTargetRanges(AspectBehaviourType.Start, AspectBehaviourType.Destroy);
        }

        /// <summary>
        /// 检测当前对象的生命周期是否处于开始激活状态
        /// </summary>
        /// <returns>若对象处于给定生命周期状态则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal bool IsOnDestroyingStatus()
        {
            return IsTheLifecycleTypeBetweenOfTargetRanges(AspectBehaviourType.Destroy, AspectBehaviourType.Free);
        }

        /// <summary>
        /// 检测当前对象的生命周期是否处于刷新激活状态
        /// </summary>
        /// <returns>若对象处于给定生命周期状态则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal bool IsOnWorkingStatus()
        {
            // return IsOnTargetLifecycle(AspectBehaviourType.Execute) ||
            //        IsOnTargetLifecycle(AspectBehaviourType.LateExecute) ||
            //        IsOnTargetLifecycle(AspectBehaviourType.Update) ||
            //        IsOnTargetLifecycle(AspectBehaviourType.LateUpdate);
            return IsTheLifecycleTypeBetweenOfTargetRanges(AspectBehaviourType.WorkingBegan, AspectBehaviourType.WorkingEnded);
        }

        /// <summary>
        /// 通过指定的函数名称，获取对应的生命周期步骤
        /// </summary>
        /// <param name="methodName">函数名称</param>
        /// <returns>返回生命周期步骤类型，若不存在对应步骤则返回unknown状态</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static AspectBehaviourType GetLifecycleTypeByMethodName(string methodName)
        {
            return NovaEngine.Utility.Convertion.GetEnumFromName<AspectBehaviourType>(methodName);
        }

        #endregion

        #region 切面控制层提供的服务回调函数

        /// <summary>
        /// 支持切面控制的函数调用接口
        /// </summary>
        /// <param name="methodName">函数名称</param>
        public void Call(string methodName)
        {
            AspectBehaviourType lifecycleType = GetLifecycleTypeByMethodName(methodName);
            if (AspectBehaviourType.Unknown != lifecycleType)
            {
                _currentLifecycleType = lifecycleType;
                _isCurrentLifecycleTypeRunning = true;
            }

            AspectController.Instance.Call(this, methodName);

            _isCurrentLifecycleTypeRunning = false;
        }

        /// <summary>
        /// 进行异常拦截的切面控制函数调用接口
        /// </summary>
        /// <param name="methodName">函数名称</param>
        /// <returns>正常调用结束返回true，发生异常返回false</returns>
        public bool Pcall(string methodName)
        {
            try { Call(methodName); } catch (Exception e) { Debugger.Error(e); return false; }

            return true;
        }

        /// <summary>
        /// 支持切面控制的函数调用接口
        /// </summary>
        /// <param name="f">目标函数</param>
        protected internal void Call(Action f)
        {
            AspectBehaviourType lifecycleType = GetLifecycleTypeByMethodName(f.Method.Name);

            CBase obj = f.Target as CBase;
            Debugger.Assert(obj, NovaEngine.ErrorText.InvalidArguments);

            if (AspectBehaviourType.Unknown != lifecycleType)
            {
                obj._currentLifecycleType = lifecycleType;
                obj._isCurrentLifecycleTypeRunning = true;
            }

            AspectController.Instance.Call(f);

            obj._isCurrentLifecycleTypeRunning = false;
        }

        /// <summary>
        /// 支持切面控制的函数调用接口
        /// </summary>
        /// <param name="f">目标函数</param>
        /// <param name="lifecycleType">生命周期类型</param>
        protected internal void Call(Action f, AspectBehaviourType lifecycleType)
        {
            Debugger.Assert(AspectBehaviourType.Unknown != lifecycleType, NovaEngine.ErrorText.InvalidArguments);

            CBase obj = f.Target as CBase;
            Debugger.Assert(obj, NovaEngine.ErrorText.InvalidArguments);

            obj._currentLifecycleType = lifecycleType;
            obj._isCurrentLifecycleTypeRunning = true;

            AspectController.Instance.Call(f);

            obj._isCurrentLifecycleTypeRunning = false;
        }

        /// <summary>
        /// 进行异常拦截的切面控制函数调用接口
        /// </summary>
        /// <param name="f">目标函数</param>
        /// <returns>正常调用结束返回true，发生异常返回false</returns>
        protected internal bool Pcall(Action f)
        {
            try { Call(f); } catch (Exception e) { Debugger.Error(e); return false; }

            return true;
        }

        /// <summary>
        /// 支持切面控制的函数调用接口
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="func">目标函数</param>
        /// <param name="arg0">参数值</param>
        protected internal void Call<T>(Action<T> func, T arg0)
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
        protected internal bool Pcall<T>(Action<T> func, T arg0)
        {
            try { Call<T>(func, arg0); } catch (Exception e) { Debugger.Error(e); return false; }

            return true;
        }

        /// <summary>
        /// 支持切面控制的带返回值的函数调用接口
        /// </summary>
        /// <typeparam name="V">返回值类型</typeparam>
        /// <param name="func">目标函数</param>
        /// <returns>返回目标函数调用后的返回结果</returns>
        protected internal V Call<V>(Func<V> func)
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
        protected internal bool Pcall<V>(Func<V> func, out V value)
        {
            try { value = Call<V>(func); } catch (Exception e) { Debugger.Error(e); value = default(V); return false; }

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
        protected internal V Call<T, V>(Func<T, V> func, T arg0)
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
        protected internal bool Pcall<T, V>(Func<T, V> func, T arg0, out V value)
        {
            try { value = Call<T, V>(func, arg0); } catch (Exception e) { Debugger.Error(e); value = default(V); return false; }

            return true;
        }

        #endregion
    }
}
