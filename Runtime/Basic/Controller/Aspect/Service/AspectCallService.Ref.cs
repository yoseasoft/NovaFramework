/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
    /// 提供切面访问接口的服务类
    public static partial class AspectCallService
    {
        [OnServiceProcessRegisterOfTarget(typeof(CRef), AspectBehaviourType.Initialize)]
        private static void CallServiceProcessOfRefInitialize(CRef obj, bool reload)
        {
            // Debugger.Info(LogGroupTag.Controller, "Register ref '{%t}' dispatch call with target behaviour type '{%i}'.", obj.BeanType, AspectBehaviourType.Initialize);

            RegRefDispatchCallByTargetType(obj, AspectBehaviourType.Initialize, reload);
        }

        [OnServiceProcessRegisterOfTarget(typeof(CRef), AspectBehaviourType.Startup)]
        private static void CallServiceProcessOfRefStartup(CRef obj, bool reload)
        {
            // Debugger.Info(LogGroupTag.Controller, "Register ref '{%t}' dispatch call with target behaviour type '{%i}'.", obj.BeanType, AspectBehaviourType.Startup);

            RegRefDispatchCallByTargetType(obj, AspectBehaviourType.Startup, reload);
        }

        [OnServiceProcessRegisterOfTarget(typeof(CRef), AspectBehaviourType.Awake)]
        private static void CallServiceProcessOfRefAwake(CRef obj, bool reload)
        {
            // Debugger.Info(LogGroupTag.Controller, "Register ref '{%t}' dispatch call with target behaviour type '{%i}'.", obj.BeanType, AspectBehaviourType.Awake);

            RegRefDispatchCallByTargetType(obj, AspectBehaviourType.Awake, reload);
        }

        [OnServiceProcessRegisterOfTarget(typeof(CRef), AspectBehaviourType.Start)]
        private static void CallServiceProcessOfRefStart(CRef obj, bool reload)
        {
            // Debugger.Info(LogGroupTag.Controller, "Register ref '{%t}' dispatch call with target behaviour type '{%i}'.", obj.BeanType, AspectBehaviourType.Start);

            RegRefDispatchCallByTargetType(obj, AspectBehaviourType.Start, reload);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RegRefDispatchCallByTargetType(CRef obj, AspectBehaviourType behaviourType, bool reload)
        {
            RegRefDispatchCallByTargetType(obj, obj.BeanType, behaviourType, reload);
        }

        private static void RegRefDispatchCallByTargetType(CRef obj, Type targetType, AspectBehaviourType behaviourType, bool reload)
        {
            Loader.Structuring.GeneralCodeInfo codeInfo = Loader.CodeLoader.LookupGeneralCodeInfo(targetType, typeof(CRef));
            if (null == codeInfo)
            {
                Debugger.Warn(LogGroupTag.Controller, "Could not found any aspect call ref service process with target type '{%t}', called it failed.", targetType);
                return;
            }

            if (codeInfo is not Loader.Structuring.RefCodeInfo refCodeInfo)
            {
                Debugger.Warn(LogGroupTag.Controller, "The aspect call ref service process getting error code info '{%t}' with target type '{%t}', called it failed.", codeInfo, targetType);
                return;
            }

            // 输入响应信息
            for (int n = 0; n < refCodeInfo.GetInputResponsingMethodTypeCount(); ++n)
            {
                Loader.Structuring.InputResponsingMethodTypeCodeInfo methodTypeCodeInfo = refCodeInfo.GetInputResponsingMethodType(n);
                if (methodTypeCodeInfo.BehaviourType != behaviourType) continue;

                if (false == NovaEngine.Utility.Reflection.IsTypeOfExtension(methodTypeCodeInfo.Method) && reload)
                {
                    // 针对对象内部的成员函数，在重载模式下不能对其撤销后再次注册
                    continue;
                }

                // Delegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(targetObject, methodTypeCodeInfo.Method);
                // Debugger.Assert(callback, "Invalid method type.");

                Debugger.Info(LogGroupTag.Controller, "Register ref '{%t}' input listener with target method '{%t}'.", targetType, methodTypeCodeInfo.Method);

                if (methodTypeCodeInfo.InputCode > 0)
                {
                    // if (reload) { obj.RemoveInputResponse(methodTypeCodeInfo.Method, methodTypeCodeInfo.InputCode, (int) methodTypeCodeInfo.OperationType); }

                    obj.AddInputResponse(methodTypeCodeInfo.Fullname, methodTypeCodeInfo.Method, methodTypeCodeInfo.InputCode, (int) methodTypeCodeInfo.OperationType, true);
                }
                else
                {
                    // if (reload) { obj.RemoveInputResponse(methodTypeCodeInfo.Method, methodTypeCodeInfo.InputDataType); }

                    obj.AddInputResponse(methodTypeCodeInfo.Fullname, methodTypeCodeInfo.Method, methodTypeCodeInfo.InputDataType, true);
                }
            }

            // 事件订阅信息
            for (int n = 0; n < refCodeInfo.GetEventSubscribingMethodTypeCount(); ++n)
            {
                Loader.Structuring.EventSubscribingMethodTypeCodeInfo methodTypeCodeInfo = refCodeInfo.GetEventSubscribingMethodType(n);
                if (methodTypeCodeInfo.BehaviourType != behaviourType) continue;

                if (false == NovaEngine.Utility.Reflection.IsTypeOfExtension(methodTypeCodeInfo.Method) && reload)
                {
                    // 针对对象内部的成员函数，在重载模式下不能对其撤销后再次注册
                    continue;
                }

                // Delegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(targetObject, methodTypeCodeInfo.Method);
                // Debugger.Assert(callback, "Invalid method type.");

                Debugger.Info(LogGroupTag.Controller, "Register ref '{%t}' event listener with target method '{%t}'.", targetType, methodTypeCodeInfo.Method);

                if (methodTypeCodeInfo.EventID > 0)
                {
                    // if (reload) { obj.Unsubscribe(methodTypeCodeInfo.Method, methodTypeCodeInfo.EventID); }

                    obj.Subscribe(methodTypeCodeInfo.Fullname, methodTypeCodeInfo.Method, methodTypeCodeInfo.EventID, true);
                }
                else
                {
                    // if (reload) { obj.Unsubscribe(methodTypeCodeInfo.Method, methodTypeCodeInfo.EventDataType); }

                    obj.Subscribe(methodTypeCodeInfo.Fullname, methodTypeCodeInfo.Method, methodTypeCodeInfo.EventDataType, true);
                }
            }

            // 消息派发信息
            for (int n = 0; n < refCodeInfo.GetMessageBindingMethodTypeCount(); ++n)
            {
                Loader.Structuring.MessageBindingMethodTypeCodeInfo methodTypeCodeInfo = refCodeInfo.GetMessageBindingMethodType(n);
                if (methodTypeCodeInfo.BehaviourType != behaviourType) continue;

                if (false == NovaEngine.Utility.Reflection.IsTypeOfExtension(methodTypeCodeInfo.Method) && reload)
                {
                    // 针对对象内部的成员函数，在重载模式下不能对其撤销后再次注册
                    continue;
                }

                // Delegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(targetObject, methodTypeCodeInfo.Method);
                // Debugger.Assert(callback, "Invalid method type.");

                Debugger.Info(LogGroupTag.Controller, "Register ref '{%t}' message listener with target method '{%t}'.", targetType, methodTypeCodeInfo.Method);

                if (methodTypeCodeInfo.Opcode > 0)
                {
                    // if (reload) { obj.RemoveMessageListener(methodTypeCodeInfo.Method, methodTypeCodeInfo.Opcode); }

                    obj.AddMessageListener(methodTypeCodeInfo.Fullname, methodTypeCodeInfo.Method, methodTypeCodeInfo.Opcode, true);
                }
                else
                {
                    // if (reload) { obj.RemoveMessageListener(methodTypeCodeInfo.Method, methodTypeCodeInfo.MessageType); }

                    obj.AddMessageListener(methodTypeCodeInfo.Fullname, methodTypeCodeInfo.Method, methodTypeCodeInfo.MessageType, true);
                }
            }

            Type baseType = targetType.BaseType;
            if (NovaEngine.Utility.Reflection.IsTypeOfInstantiableClass(baseType))
            {
                RegRefDispatchCallByTargetType(obj, baseType, behaviourType, reload);
            }
        }
    }
}
