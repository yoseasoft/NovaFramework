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

using System;
using System.Collections.Generic;
using System.Reflection;

namespace GameEngine
{
    /// <summary>
    /// 切面访问接口的控制器类，对整个程序所有切面访问函数进行统一的整合和管理
    /// </summary>
    internal sealed partial class AspectController : BaseController<AspectController>
    {
        /// <summary>
        /// 切面标准调用函数的句柄定义
        /// </summary>
        /// <param name="self">调用对象实例</param>
        // public delegate void OnAspectStandardCallHandler(object self);

        /// <summary>
        /// 切面管理对象初始化通知接口函数
        /// </summary>
        protected override sealed void OnInitialize()
        {
            // 切面服务的初始化
            AspectCallService.InitAllServiceProcessingCallbacks();
        }

        /// <summary>
        /// 切面管理对象清理通知接口函数
        /// </summary>
        protected override sealed void OnCleanup()
        {
            // 切面服务的清理
            AspectCallService.CleanupAllServiceProcessingCallbacks();
        }

        /// <summary>
        /// 切面管理对象刷新调度函数接口
        /// </summary>
        protected override sealed void OnUpdate()
        {
        }

        /// <summary>
        /// 切面管理对象后置刷新调度函数接口
        /// </summary>
        protected override sealed void OnLateUpdate()
        {
        }

        /// <summary>
        /// 切面管理对象重载调度函数接口
        /// </summary>
        protected override sealed void OnReload()
        {
        }

        /// <summary>
        /// 切面管理对象倾泻调度函数接口
        /// </summary>
        protected override sealed void OnDump()
        {
        }

        #region 切面访问函数调用接口

        /// <summary>
        /// 指定切入点的函数调用接口
        /// </summary>
        /// <param name="obj">目标对象实例</param>
        /// <param name="methodName">函数名称</param>
        public void Call(object obj, string methodName)
        {
            MethodInfo methodInfo = obj.GetType().GetMethod(methodName);
            if (null == methodInfo)
            {
                CallExtend(obj, methodName);
            }
            else
            {
                if (typeof(void) == methodInfo.ReturnType)
                {
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    if (null != parameters && parameters.Length > 0)
                    {
                        Debugger.Warn("The method '{%t}.{%s}' has multiple params '{%d}', cannot invoke with current process.",
                                      obj.GetType(), methodInfo.Name, null == parameters ? 0 : parameters.Length);
                        return;
                    }

                    Action action = (Action) Delegate.CreateDelegate(typeof(Action), obj, methodInfo);
                    Call(action);
                }
                else
                {
                    Debugger.Warn("The method '{%t}.{%s}' has return value '{%t}', cannot invoke with current process.",
                                  obj.GetType(), methodInfo.Name, methodInfo.ReturnType);
                }
            }
        }

        /// <summary>
        /// 指定切入点的函数调用接口
        /// </summary>
        /// <param name="func">目标函数</param>
        public void Call(Action func)
        {
            bool isException = false;

            CallBefore(func.Target, func.Method.Name);

            if (false == CallAround(func.Target, func.Method.Name))
            {
                try { func(); } catch (Exception e) { isException = true; Debugger.Error(e); }
            }

            CallAfter(func.Target, func.Method.Name, isException);
        }

        /// <summary>
        /// 指定切入点的函数调用接口
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="func">目标函数</param>
        /// <param name="arg1">参数值</param>
        public void Call<T>(Action<T> func, T arg1)
        {
            bool isException = false;

            CallBefore(func.Target, func.Method.Name);

            if (false == CallAround(func.Target, func.Method.Name))
            {
                try { func(arg1); } catch (Exception e) { isException = true; Debugger.Error(e); }
            }

            CallAfter(func.Target, func.Method.Name, isException);
        }

        /// <summary>
        /// 指定切入点的函数调用接口
        /// </summary>
        /// <typeparam name="T1">参数类型</typeparam>
        /// <typeparam name="T2">参数类型</typeparam>
        /// <param name="func">目标函数</param>
        /// <param name="arg1">参数值</param>
        /// <param name="arg2">参数值</param>
        public void Call<T1, T2>(Action<T1, T2> func, T1 arg1, T2 arg2)
        {
            bool isException = false;

            CallBefore(func.Target, func.Method.Name);

            if (false == CallAround(func.Target, func.Method.Name))
            {
                try { func(arg1, arg2); } catch (Exception e) { isException = true; Debugger.Error(e); }
            }

            CallAfter(func.Target, func.Method.Name, isException);
        }

        /// <summary>
        /// 指定切入点的带返回值的函数调用接口
        /// </summary>
        /// <typeparam name="V">返回值类型</typeparam>
        /// <param name="func">目标函数</param>
        /// <returns>返回目标函数调用后的返回结果</returns>
        public V Call<V>(Func<V> func)
        {
            bool isException = false;

            CallBefore(func.Target, func.Method.Name);

            V result = default;
            if (false == CallAround(func.Target, func.Method.Name))
            {
                try { result = func(); } catch (Exception e) { isException = true; Debugger.Error(e); }
            }

            CallAfter(func.Target, func.Method.Name, isException);

            return result;
        }

        /// <summary>
        /// 指定切入点的带返回值的函数调用接口
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <typeparam name="V">返回值类型</typeparam>
        /// <param name="func">目标函数</param>
        /// <param name="arg1">参数值</param>
        /// <returns>返回目标函数调用后的返回结果</returns>
        public V Call<T, V>(Func<T, V> func, T arg1)
        {
            bool isException = false;

            CallBefore(func.Target, func.Method.Name);

            V result = default;
            if (false == CallAround(func.Target, func.Method.Name))
            {
                try { result = func(arg1); } catch (Exception e) { isException = true; Debugger.Error(e); }
            }

            CallAfter(func.Target, func.Method.Name, isException);

            return result;
        }

        /// <summary>
        /// 指定切入点的带返回值的函数调用接口
        /// </summary>
        /// <typeparam name="T1">参数类型</typeparam>
        /// <typeparam name="T2">参数类型</typeparam>
        /// <typeparam name="V">返回值类型</typeparam>
        /// <param name="func">目标函数</param>
        /// <param name="arg1">参数值</param>
        /// <param name="arg2">参数值</param>
        /// <returns>返回目标函数调用后的返回结果</returns>
        public V Call<T1, T2, V>(Func<T1, T2, V> func, T1 arg1, T2 arg2)
        {
            bool isException = false;

            CallBefore(func.Target, func.Method.Name);

            V result = default;
            if (false == CallAround(func.Target, func.Method.Name))
            {
                try { result = func(arg1, arg2); } catch (Exception e) { isException = true; Debugger.Error(e); }
            }

            CallAfter(func.Target, func.Method.Name, isException);

            return result;
        }

        #endregion
    }
}
