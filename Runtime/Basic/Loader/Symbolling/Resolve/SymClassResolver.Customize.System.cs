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

using System.Collections.Generic;

namespace GameEngine.Loader.Symbolling
{
    /// 标记对象的解析类
    internal static partial class SymClassResolver
    {
        /// <summary>
        /// 自动填充系统对象类型的标记类的特性
        /// </summary>
        /// <param name="symClass">类标记对象</param>
        private static void AutoFillSystemClassFeatures(SymClass symClass)
        {
            bool on_aspect_supported = false;
            bool on_extend_supported = false;

            // 全局分发
            bool on_input_system = false;
            bool on_event_system = false;
            bool on_message_system = false;
            bool on_api_system = false;

            IList<SymMethod> methods = symClass.GetAllMethods();
            if (null == methods)
                return;

            for (int n = 0; n < methods.Count; ++n)
            {
                SymMethod method = methods[n];
                // 因为是静态类，所以内部的所有方法都必然为静态方法
                // 其实这里完全没有必要做检查，但作者高兴，而且这里的处理是一次性的，所以就做吧，开心最重要
                Debugger.Assert(method.IsStatic, "Invalid method type.");

                // 扩展类型函数，将扩展目标添加到符号类中
                if (method.IsExtension)
                {
                    OnAspectCallAttribute aspectCallAttribute = method.GetAttribute<OnAspectCallAttribute>(true);
                    if (null != aspectCallAttribute)
                    {
                        AspectBehaviourType aspectBehaviourType = AspectController.Instance.GetAspectBehaviourTypeByName(aspectCallAttribute.MethodName);

                        /**
                         * 2025-12-02：
                         * 扩展注入方式，由原本针对指定类型的注入，修改为
                         * 在行为注册时，若目标对象为抽象父类或接口，则其所有的子类或实现类，都将注入该切面行为
                         *
                         * // 通过扩展的目标类型，获取到对应的符号类实例
                         * SymClass extensionTargetClass = CodeLoader.GetSymClassByType(method.ExtensionParameterType);
                         * Debugger.Assert(null != extensionTargetClass, "Invalid extension target type.");
                         *
                         * // 将切面行为类型附加到扩展的目标对象上
                         * extensionTargetClass.AddAspectBehaviourType(aspectBehaviourType);
                         */

                        // Extend访问类型的切面函数，将返回Unknown类型
                        if (AspectBehaviourType.Unknown != aspectBehaviourType)
                        {
                            if (false == NovaEngine.Utility.Reflection.IsTypeOfInstantiableClass(method.ExtensionParameterType))
                            {
                                IList<SymClass> extensionTargetClasses = CodeLoader.FindAllSymClassesByInterfaceType(method.ExtensionParameterType);

                                for (int k = 0; null != extensionTargetClasses && k < extensionTargetClasses.Count; ++k)
                                {
                                    SymClass extensionTargetClass = extensionTargetClasses[k];

                                    // 将切面行为类型附加到扩展的目标对象上
                                    extensionTargetClass.AddAspectBehaviourType(aspectBehaviourType);
                                }
                            }
                            else
                            {
                                // 通过扩展的目标类型，获取到对应的符号类实例
                                SymClass extensionTargetClass = CodeLoader.GetSymClassByType(method.ExtensionParameterType);
                                Debugger.Assert(null != extensionTargetClass, "Invalid extension target type.");

                                // 将切面行为类型附加到扩展的目标对象上
                                extensionTargetClass.AddAspectBehaviourType(aspectBehaviourType);
                            }
                        }
                        else
                        {
                            Debugger.Log(LogGroupTag.CodeLoader, "新增扩展切面函数‘{%s}’给目标对象实例‘{%t}’！", aspectCallAttribute.MethodName, method.ExtensionParameterType);
                        }

                        on_aspect_supported = true;
                    }

                    else if (method.HasAttribute(typeof(InputResponseBindingOfTargetAttribute), true))
                    {
                        // 激活扩展的目标类型的输入转发特性
                        AutobindFeatureTypeForTargetSymbol(method.ExtensionParameterType, typeof(InputActivationAttribute));

                        on_extend_supported = true;
                    }

                    else if (method.HasAttribute(typeof(EventSubscribeBindingOfTargetAttribute), true))
                    {
                        // 激活扩展的目标类型的事件转发特性
                        AutobindFeatureTypeForTargetSymbol(method.ExtensionParameterType, typeof(EventActivationAttribute));

                        on_extend_supported = true;
                    }

                    else if (method.HasAttribute(typeof(MessageListenerBindingOfTargetAttribute), true))
                    {
                        // 激活扩展的目标类型的消息转发特性
                        AutobindFeatureTypeForTargetSymbol(method.ExtensionParameterType, typeof(MessageActivationAttribute));

                        on_extend_supported = true;
                    }

                    else
                    {
                        AutoFillOtherExtensionMethodFeatures(symClass, method);
                    }
                } // method.IsExtension
                else
                {
                    if (!on_input_system)
                    {
                        on_input_system |= method.HasAttribute(typeof(OnInputDispatchCallAttribute), true);
                    }

                    if (!on_event_system)
                    {
                        on_event_system |= method.HasAttribute(typeof(OnEventDispatchCallAttribute), true);
                    }

                    if (!on_message_system)
                    {
                        on_message_system |= method.HasAttribute(typeof(OnMessageDispatchCallAttribute), true);
                    }
                }

                if (!on_api_system)
                {
                    on_api_system |= method.HasAttribute(typeof(OnApiDispatchCallAttribute), true);
                    // on_api_system |= method.HasAttribute(typeof(ApiFunctionBindingOfTargetAttribute), true);
                }
            }

            if (on_aspect_supported)
            {
                // 装配切面支持
                AutobindFeatureTypeForTargetSymbol(symClass, typeof(AspectAttribute));
            }

            if (on_extend_supported)
            {
                // 装配扩展支持
                AutobindFeatureTypeForTargetSymbol(symClass, typeof(ExtendSupportedAttribute));
            }

            if (on_input_system)
            {
                // 装配输入系统
                AutobindFeatureTypeForTargetSymbol(symClass, typeof(InputSystemAttribute));
            }

            if (on_event_system)
            {
                // 装配事件系统
                AutobindFeatureTypeForTargetSymbol(symClass, typeof(EventSystemAttribute));
            }

            if (on_message_system)
            {
                // 装配消息系统
                AutobindFeatureTypeForTargetSymbol(symClass, typeof(MessageSystemAttribute));
            }

            if (on_api_system)
            {
                // 装配API调度系统
                AutobindFeatureTypeForTargetSymbol(symClass, typeof(ApiSystemAttribute));
            }
        }

        /// <summary>
        /// 自动填充其它系统对象类型扩展函数的标记类的特性
        /// </summary>
        /// <param name="symClass">类标记对象</param>
        private static void AutoFillOtherExtensionMethodFeatures(SymClass symClass, SymMethod symMethod)
        {
            if (!symMethod.IsExtension)
            {
                // 该接口仅服务于扩展函数
                return;
            }

            // 服务于‘CEntity’的扩展函数解析
            if (typeof(CEntity).IsAssignableFrom(symMethod.ExtensionParameterType))
            {
                AutoFillEntityExtensionMethodFeatures(symClass, symMethod);
            }
        }
    }
}
