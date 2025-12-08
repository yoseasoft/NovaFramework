/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

namespace GameEngine
{
    /// 提供切面访问接口的服务类
    public static partial class AspectCallService
    {
        [OnServiceProcessRegisterOfTarget(typeof(CView), AspectBehaviourType.Initialize)]
        private static void CallServiceProcessOfViewInitialize(CView obj, bool reload)
        {
            // Debugger.Info("Register view '{%t}' notice call with target behaviour type '{%i}'.", obj.BeanType, AspectBehaviourType.Initialize);

            RegViewNoticeCallByTargetType(obj, AspectBehaviourType.Initialize, reload);
        }

        [OnServiceProcessRegisterOfTarget(typeof(CView), AspectBehaviourType.Startup)]
        private static void CallServiceProcessOfViewStartup(CView obj, bool reload)
        {
            // Debugger.Info("Register view '{%t}' notice call with target behaviour type '{%i}'.", obj.BeanType, AspectBehaviourType.Startup);

            RegViewNoticeCallByTargetType(obj, AspectBehaviourType.Startup, reload);
        }

        [OnServiceProcessRegisterOfTarget(typeof(CView), AspectBehaviourType.Awake)]
        private static void CallServiceProcessOfViewAwake(CView obj, bool reload)
        {
            // Debugger.Info("Register view '{%t}' notice call with target behaviour type '{%i}'.", obj.BeanType, AspectBehaviourType.Awake);

            RegViewNoticeCallByTargetType(obj, AspectBehaviourType.Awake, reload);
        }

        [OnServiceProcessRegisterOfTarget(typeof(CView), AspectBehaviourType.Start)]
        private static void CallServiceProcessOfViewStart(CView obj, bool reload)
        {
            // Debugger.Info("Register view '{%t}' notice call with target behaviour type '{%i}'.", obj.BeanType, AspectBehaviourType.Start);

            RegViewNoticeCallByTargetType(obj, AspectBehaviourType.Start, reload);
        }

        private static void RegViewNoticeCallByTargetType(CView obj, AspectBehaviourType behaviourType, bool reload)
        {
            Type targetType = obj.BeanType;
            Loader.Structuring.GeneralCodeInfo codeInfo = Loader.CodeLoader.LookupGeneralCodeInfo(targetType, typeof(CEntity));
            if (null == codeInfo)
            {
                Debugger.Warn("Could not found any aspect call ref service process with target type '{%t}', called it failed.", targetType);
                return;
            }

            Loader.Structuring.ViewCodeInfo viewCodeInfo = codeInfo as Loader.Structuring.ViewCodeInfo;
            if (null == viewCodeInfo)
            {
                Debugger.Warn("The aspect call ref service process getting error code info '{%t}' with target type '{%t}', called it failed.", codeInfo.GetType(), targetType);
                return;
            }

            // 通知接口信息
            for (int n = 0; n < viewCodeInfo.GetNoticeMethodTypeCount(); ++n)
            {
                Loader.Structuring.CViewNoticeMethodTypeCodeInfo methodTypeCodeInfo = viewCodeInfo.GetNoticeMethodType(n);
                if (methodTypeCodeInfo.BehaviourType != behaviourType) continue;

                if (false == NovaEngine.Utility.Reflection.IsTypeOfExtension(methodTypeCodeInfo.Method) && reload)
                {
                    // 针对对象内部的成员函数，在重载模式下不能对其撤销后再次注册
                    continue;
                }

                // Delegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(targetObject, methodTypeCodeInfo.Method);
                // Debugger.Assert(null != callback, "Invalid method type.");

                Debugger.Info(LogGroupTag.Controller, "Register view '{%t}' notice call with target method '{%t}'.", targetType, methodTypeCodeInfo.Method);

                // if (reload) { obj.RemoveNoticeProcess(methodTypeCodeInfo.Fullname, methodTypeCodeInfo.NoticeType); }

                obj.AddNoticeProcess(methodTypeCodeInfo.Fullname, methodTypeCodeInfo.Method, methodTypeCodeInfo.NoticeType);
            }
        }
    }
}
