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

using SystemType = System.Type;

namespace GameEngine.Loader.Symboling
{
    /// <summary>
    /// 标记对象的解析类，对基础对象类的注入标记进行解析和构建
    /// </summary>
    internal static partial class SymClassResolver
    {
        /// <summary>
        /// 对标记类进行个性化定制
        /// </summary>
        /// <param name="symClass">类标记对象</param>
        static void DoPersonalizedCustomizationOfClass(SymClass symClass)
        {
            // 特性定制
            AutoFillClassFeatures(symClass);
        }

        /// <summary>
        /// 自动填充标记类的特性
        /// </summary>
        /// <param name="symClass">类标记对象</param>
        static void AutoFillClassFeatures(SymClass symClass)
        {
            if (symClass.IsStatic)
            {
                // 逻辑类的自动填充流程
                AutoFillSystemClassFeatures(symClass);
            }
            else
            {
                // 对象类的自动填充流程
                AutoFillInstantiationClassFeatures(symClass);
            }
        }

        /// <summary>
        /// 自动填充实例对象类型的标记类的特性
        /// </summary>
        /// <param name="symClass">类标记对象</param>
        private static void AutoFillInstantiationClassFeatures(SymClass symClass)
        {
        }

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

                        // Extend访问类型的切面函数，将返回Unknown类型
                        if (AspectBehaviourType.Unknown != aspectBehaviourType)
                        {
                            // 通过扩展的目标类型，获取到对应的符号类实例
                            SymClass extensionTargetClass = CodeLoader.GetSymClassByType(method.ExtensionParameterType);
                            Debugger.Assert(null != extensionTargetClass, "Invalid extension target type.");

                            // 将切面行为类型附加到扩展的目标对象上
                            extensionTargetClass.AddAspectBehaviourType(aspectBehaviourType);

                            // Debugger.Info("新增切面行为类型‘{%s}’到目标对象实例‘{%s}’", aspectBehaviourType.ToString(), extensionTargetClass.ClassName);
                        }
                        else
                        {
                            Debugger.Info("新增扩展切面函数‘{%s}’给目标对象实例‘{%s}’！", aspectCallAttribute.MethodName, NovaEngine.Utility.Text.ToString(method.ExtensionParameterType));
                        }

                        on_aspect_supported = true;
                    }

                    else if (method.HasAttribute(typeof(InputResponseBindingOfTargetAttribute)))
                    {
                        // 激活扩展的目标类型的输入转发特性
                        AutobindFeatureTypeForTargetSymbol(method.ExtensionParameterType, typeof(InputActivationAttribute));

                        on_extend_supported = true;
                    }

                    else if (method.HasAttribute(typeof(EventSubscribeBindingOfTargetAttribute)))
                    {
                        // 激活扩展的目标类型的事件转发特性
                        AutobindFeatureTypeForTargetSymbol(method.ExtensionParameterType, typeof(EventActivationAttribute));

                        on_extend_supported = true;
                    }

                    else if (method.HasAttribute(typeof(MessageListenerBindingOfTargetAttribute)))
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
                        on_input_system |= method.HasAttribute(typeof(OnInputDispatchCallAttribute));
                    }

                    if (!on_event_system)
                    {
                        on_event_system |= method.HasAttribute(typeof(OnEventDispatchCallAttribute));
                    }

                    if (!on_message_system)
                    {
                        on_message_system |= method.HasAttribute(typeof(OnMessageDispatchCallAttribute));
                    }
                }

                if (!on_api_system)
                {
                    on_api_system |= method.HasAttribute(typeof(OnApiDispatchCallAttribute));
                    // on_api_system |= method.HasAttribute(typeof(ApiFunctionBindingOfTargetAttribute));
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

        /// <summary>
        /// 给目标符号类绑定指定的特性标签
        /// </summary>
        /// <param name="targetType">符号类型</param>
        /// <param name="featureType">特性类型</param>
        private static void AutobindFeatureTypeForTargetSymbol(SystemType targetType, SystemType featureType)
        {
            // 通过目标类型，获取到对应的符号类实例
            SymClass symClass = CodeLoader.GetSymClassByType(targetType);
            Debugger.Assert(null != symClass, "Invalid target symbol type.");

            AutobindFeatureTypeForTargetSymbol(symClass, featureType);
        }

        /// <summary>
        /// 给目标符号类绑定指定的特性标签
        /// </summary>
        /// <param name="symClass">符号实例</param>
        /// <param name="featureType">特性类型</param>
        private static void AutobindFeatureTypeForTargetSymbol(SymClass symClass, SystemType featureType)
        {
            Debugger.Info(LogGroupTag.CodeLoader, "对象类型解析：目标符号类型‘{%s}’动态绑定新的特性‘{%s}’成功。",
                symClass.ClassName, NovaEngine.Utility.Text.ToString(featureType));

            // 为符号类添加特性
            symClass.AddFeatureType(featureType);
        }
    }
}
