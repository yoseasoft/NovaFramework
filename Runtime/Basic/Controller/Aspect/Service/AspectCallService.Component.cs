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

using SystemType = System.Type;

namespace GameEngine
{
    /// <summary>
    /// 提供切面访问接口的服务类，对整个程序内部的对象实例提供切面访问的服务逻辑处理
    /// </summary>
    public static partial class AspectCallService
    {
        [OnServiceProcessRegisterOfTarget(typeof(CComponent), AspectBehaviourType.Initialize)]
        private static void CallServiceProcessOfComponentInitialize(CComponent component, bool reload)
        {
            // Debugger.Info("Register component '{%t}' dispatch call with target behaviour type '{%i}'.", component.BeanType, AspectBehaviourType.Initialize);

            RegComponentDispatchCallByTargetType(component, AspectBehaviourType.Initialize, reload);
        }

        [OnServiceProcessRegisterOfTarget(typeof(CComponent), AspectBehaviourType.Startup)]
        private static void CallServiceProcessOfComponentStartup(CComponent component, bool reload)
        {
            // Debugger.Info("Register component '{%t}' dispatch call with target behaviour type '{%i}'.", component.BeanType, AspectBehaviourType.Startup);

            RegComponentDispatchCallByTargetType(component, AspectBehaviourType.Startup, reload);
        }

        [OnServiceProcessRegisterOfTarget(typeof(CComponent), AspectBehaviourType.Awake)]
        private static void CallServiceProcessOfComponentAwake(CComponent component, bool reload)
        {
            // Debugger.Info("Register component '{%t}' dispatch call with target behaviour type '{%i}'.", component.BeanType, AspectBehaviourType.Awake);

            RegComponentDispatchCallByTargetType(component, AspectBehaviourType.Awake, reload);
        }

        [OnServiceProcessRegisterOfTarget(typeof(CComponent), AspectBehaviourType.Start)]
        private static void CallServiceProcessOfComponentStart(CComponent component, bool reload)
        {
            // Debugger.Info("Register component '{%t}' dispatch call with target behaviour type '{%i}'.", component.BeanType, AspectBehaviourType.Start);

            RegComponentDispatchCallByTargetType(component, AspectBehaviourType.Start, reload);
        }

        private static void RegComponentDispatchCallByTargetType(CComponent component, AspectBehaviourType behaviourType, bool reload)
        {
            SystemType targetType = component.BeanType;
            Loader.Structuring.GeneralCodeInfo codeInfo = Loader.CodeLoader.LookupGeneralCodeInfo(targetType, typeof(CComponent));
            if (null == codeInfo)
            {
                Debugger.Warn("Could not found any aspect call component service process with target type '{%t}', called it failed.", targetType);
                return;
            }

            Loader.Structuring.ComponentCodeInfo componentCodeInfo = codeInfo as Loader.Structuring.ComponentCodeInfo;
            if (null == componentCodeInfo)
            {
                Debugger.Warn("The aspect call component service process getting error code info '{%t}' with target type '{%t}', called it failed.", codeInfo.GetType(), targetType);
                return;
            }

            // 输入响应信息
            for (int n = 0; n < componentCodeInfo.GetInputResponsingMethodTypeCount(); ++n)
            {
                Loader.Structuring.InputResponsingMethodTypeCodeInfo methodTypeCodeInfo = componentCodeInfo.GetInputResponsingMethodType(n);
                if (methodTypeCodeInfo.BehaviourType != behaviourType) continue;

                if (false == NovaEngine.Utility.Reflection.IsTypeOfExtension(methodTypeCodeInfo.Method) && reload)
                {
                    // 针对对象内部的成员函数，在重载模式下不能对其撤销后再次注册
                    continue;
                }

                // SystemDelegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(targetObject, methodTypeCodeInfo.Method);
                // Debugger.Assert(null != callback, "Invalid method type.");

                Debugger.Info(LogGroupTag.Controller, "Register component '{%t}' input listener with target method '{%t}'.", targetType, methodTypeCodeInfo.Method);

                if (methodTypeCodeInfo.InputCode > 0)
                {
                    // if (reload) { component.RemoveInputResponse(methodTypeCodeInfo.InputCode, (int) methodTypeCodeInfo.OperationType); }

                    component.AddInputResponse(methodTypeCodeInfo.Fullname, methodTypeCodeInfo.Method, methodTypeCodeInfo.InputCode, (int) methodTypeCodeInfo.OperationType, true);
                }
                else
                {
                    // if (reload) { component.RemoveInputResponse(methodTypeCodeInfo.InputDataType); }

                    component.AddInputResponse(methodTypeCodeInfo.Fullname, methodTypeCodeInfo.Method, methodTypeCodeInfo.InputDataType, true);
                }
            }

            // 订阅事件信息
            for (int n = 0; n < componentCodeInfo.GetEventSubscribingMethodTypeCount(); ++n)
            {
                Loader.Structuring.EventSubscribingMethodTypeCodeInfo methodTypeCodeInfo = componentCodeInfo.GetEventSubscribingMethodType(n);
                if (methodTypeCodeInfo.BehaviourType != behaviourType) continue;

                if (false == NovaEngine.Utility.Reflection.IsTypeOfExtension(methodTypeCodeInfo.Method) && reload)
                {
                    // 针对对象内部的成员函数，在重载模式下不能对其撤销后再次注册
                    continue;
                }

                // SystemDelegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(targetObject, methodTypeCodeInfo.Method);
                // Debugger.Assert(null != callback, "Invalid method type.");

                Debugger.Info(LogGroupTag.Controller, "Register component '{%t}' event listener with target method '{%t}'.", targetType, methodTypeCodeInfo.Method);

                if (methodTypeCodeInfo.EventID > 0)
                {
                    // if (reload) { component.Unsubscribe(methodTypeCodeInfo.EventID); }

                    component.Subscribe(methodTypeCodeInfo.Fullname, methodTypeCodeInfo.Method, methodTypeCodeInfo.EventID, true);
                }
                else
                {
                    // if (reload) { component.Unsubscribe(methodTypeCodeInfo.EventDataType); }

                    component.Subscribe(methodTypeCodeInfo.Fullname, methodTypeCodeInfo.Method, methodTypeCodeInfo.EventDataType, true);
                }
            }

            // 消息派发信息
            for (int n = 0; n < componentCodeInfo.GetMessageBindingMethodTypeCount(); ++n)
            {
                Loader.Structuring.MessageBindingMethodTypeCodeInfo methodTypeCodeInfo = componentCodeInfo.GetMessageBindingMethodType(n);
                if (methodTypeCodeInfo.BehaviourType != behaviourType) continue;

                if (false == NovaEngine.Utility.Reflection.IsTypeOfExtension(methodTypeCodeInfo.Method) && reload)
                {
                    // 针对对象内部的成员函数，在重载模式下不能对其撤销后再次注册
                    continue;
                }

                // SystemDelegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(targetObject, methodTypeCodeInfo.Method);
                // Debugger.Assert(null != callback, "Invalid method type.");

                Debugger.Info(LogGroupTag.Controller, "Register component '{%t}' message listener with target method '{%t}'.", targetType, methodTypeCodeInfo.Method);

                if (methodTypeCodeInfo.Opcode > 0)
                {
                    // if (reload) { component.RemoveMessageListener(methodTypeCodeInfo.Opcode); }

                    component.AddMessageListener(methodTypeCodeInfo.Fullname, methodTypeCodeInfo.Method, methodTypeCodeInfo.Opcode, true);
                }
                else
                {
                    // if (reload) { component.RemoveMessageListener(methodTypeCodeInfo.MessageType); }

                    component.AddMessageListener(methodTypeCodeInfo.Fullname, methodTypeCodeInfo.Method, methodTypeCodeInfo.MessageType, true);
                }
            }
        }
    }
}
