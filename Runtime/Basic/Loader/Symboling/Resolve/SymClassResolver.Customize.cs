/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using System.Collections.Generic;

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
                // 数据类的自动填充流程
                AutoFillEntityClassFeatures(symClass);
            }
        }

        /// <summary>
        /// 自动填充实体类型的标记类的特性
        /// </summary>
        /// <param name="symClass">类标记对象</param>
        private static void AutoFillEntityClassFeatures(SymClass symClass)
        {
        }

        /// <summary>
        /// 自动填充系统类型的标记类的特性
        /// </summary>
        /// <param name="symClass">类标记对象</param>
        private static void AutoFillSystemClassFeatures(SymClass symClass)
        {
            bool on_aspect_supported = false;
            bool on_extend_supported = false;

            // 对象分发
            bool on_input_activated = false;
            bool on_event_activated = false;
            bool on_message_activated = false;
            // 全局分发
            bool on_input_system = false;
            bool on_event_system = false;
            bool on_message_system = false;

            IList<SymMethod> methods = symClass.GetAllMethods();
            for (int n = 0; null != methods && n < methods.Count; ++n)
            {
                SymMethod method = methods[n];
                // 因为是静态类，所以内部的所有方法都必然为静态方法
                // 其实这里完全没有必要做检查，但作者高兴，而且这里的处理是一次性的，所以就做吧，开心最重要
                Debugger.Assert(method.IsStatic, "Invalid method type.");

                // 扩展类型函数，将扩展目标添加到符号类中
                if (method.IsExtension)
                {
                    // 当前符号类绑定扩展的目标对象类型
                    // 需要注意的是，同一个静态类中所有扩展方法必须指向同一个对象类型，所以此处多次重复绑定，也是在进行检查
                    symClass.RebindingExtensionTargetType(method.ExtensionParameterType);

                    if (!on_extend_supported)
                    {
                        if (method.HasAttribute(typeof(InputResponseBindingOfTargetAttribute)))
                        {
                            on_input_activated = true;
                            on_extend_supported = true;
                        }
                        if (method.HasAttribute(typeof(EventSubscribeBindingOfTargetAttribute)))
                        {
                            on_event_activated = true;
                            on_extend_supported = true;
                        }
                        if (method.HasAttribute(typeof(MessageListenerBindingOfTargetAttribute)))
                        {
                            on_message_activated = true;
                            on_extend_supported = true;
                        }
                    }

                    OnAspectCallAttribute aspectCallAttribute = method.GetAttribute<OnAspectCallAttribute>(true);
                    if (null != aspectCallAttribute)
                    {
                        AspectBehaviourType aspectBehaviourType = AspectController.Instance.GetAspectBehaviourTypeByName(aspectCallAttribute.MethodName);
                        Debugger.Assert(AspectBehaviourType.Unknown != aspectBehaviourType, "Invalid aspect method.");

                        // 通过扩展的目标类型，获取到对应的符号类实例
                        SymClass extensionTargetClass = CodeLoader.GetSymClassByType(symClass.ExtensionTargetType);
                        Debugger.Assert(null != extensionTargetClass, "Invalid extension target type.");

                        // 将切面行为类型附加到扩展的目标对象上
                        extensionTargetClass.AddAspectBehaviourType(aspectBehaviourType);

                        // Debugger.Info("新增切面行为类型‘{%s}’到目标对象实例‘{%s}’", aspectBehaviourType.ToString(), extensionTargetClass.ClassName);

                        on_aspect_supported = true;
                    }

                    // 以下代码只是临时使用，以后将废弃这个特性标签
                    OnAspectCallOfTargetAttribute aspectCallOfTargetAttribute = method.GetAttribute<OnAspectCallOfTargetAttribute>(true);
                    if (null != aspectCallOfTargetAttribute)
                    {
                        AspectBehaviourType aspectBehaviourType = AspectController.Instance.GetAspectBehaviourTypeByName(aspectCallOfTargetAttribute.MethodName);
                        Debugger.Assert(AspectBehaviourType.Unknown != aspectBehaviourType, "Invalid aspect method.");

                        // 通过扩展的目标类型，获取到对应的符号类实例
                        SymClass extensionTargetClass = CodeLoader.GetSymClassByType(symClass.ExtensionTargetType);
                        Debugger.Assert(null != extensionTargetClass, "Invalid extension target type.");

                        // 将切面行为类型附加到扩展的目标对象上
                        extensionTargetClass.AddAspectBehaviourType(aspectBehaviourType);

                        // Debugger.Info("新增切面行为类型‘{%s}’到目标对象实例‘{%s}’", aspectBehaviourType.ToString(), extensionTargetClass.ClassName);

                        on_aspect_supported = true;
                    }
                }
                else
                {
                    if (!on_input_system)
                    {
                        if (method.HasAttribute(typeof(OnInputDispatchCallAttribute)))
                        {
                            on_input_system = true;
                        }
                    }

                    if (!on_event_system)
                    {
                        if (method.HasAttribute(typeof(OnEventDispatchCallAttribute)))
                        {
                            on_event_system = true;
                        }
                    }

                    if (!on_message_system)
                    {
                        if (method.HasAttribute(typeof(OnMessageDispatchCallAttribute)))
                        {
                            on_message_system = true;
                        }
                    }
                }
            }

            if (on_aspect_supported)
            {
                // 装配切面支持
                symClass.AddFeatureType(typeof(AspectAttribute));
                Debugger.Info(LogGroupTag.CodeLoader, "Automatically adding aspect feature type to target symbol class '{%s}'.", symClass.ClassName);
            }

            if (on_extend_supported)
            {
                // 装配扩展支持
                symClass.AddFeatureType(typeof(ExtendSupportedAttribute));
                Debugger.Info(LogGroupTag.CodeLoader, "Automatically adding extend supported feature type to target symbol class '{%s}'.", symClass.ClassName);
            }

            if (on_input_activated)
            {
                // 通过扩展的目标类型，获取到对应的符号类实例
                SymClass extensionTargetClass = CodeLoader.GetSymClassByType(symClass.ExtensionTargetType);
                Debugger.Assert(null != extensionTargetClass, "Invalid extension target type.");

                // 激活对象的输入转发
                extensionTargetClass.AddFeatureType(typeof(InputActivationAttribute));
                Debugger.Info(LogGroupTag.CodeLoader, "Automatically adding input activation feature type to target symbol class '{%s}'.", extensionTargetClass.ClassName);
            }

            if (on_event_activated)
            {
                // 通过扩展的目标类型，获取到对应的符号类实例
                SymClass extensionTargetClass = CodeLoader.GetSymClassByType(symClass.ExtensionTargetType);
                Debugger.Assert(null != extensionTargetClass, "Invalid extension target type.");

                // 激活对象的事件转发
                extensionTargetClass.AddFeatureType(typeof(EventActivationAttribute));
                Debugger.Info(LogGroupTag.CodeLoader, "Automatically adding event activation feature type to target symbol class '{%s}'.", extensionTargetClass.ClassName);
            }

            if (on_message_activated)
            {
                // 通过扩展的目标类型，获取到对应的符号类实例
                SymClass extensionTargetClass = CodeLoader.GetSymClassByType(symClass.ExtensionTargetType);
                Debugger.Assert(null != extensionTargetClass, "Invalid extension target type.");

                // 激活对象的消息转发
                extensionTargetClass.AddFeatureType(typeof(MessageActivationAttribute));
                Debugger.Info(LogGroupTag.CodeLoader, "Automatically adding message activation feature type to target symbol class '{%s}'.", extensionTargetClass.ClassName);
            }

            if (on_input_system)
            {
                // 装配输入系统
                symClass.AddFeatureType(typeof(InputSystemAttribute));
                Debugger.Info(LogGroupTag.CodeLoader, "Automatically adding input system feature type to target symbol class '{%s}'.", symClass.ClassName);
            }

            if (on_event_system)
            {
                // 装配事件系统
                symClass.AddFeatureType(typeof(EventSystemAttribute));
                Debugger.Info(LogGroupTag.CodeLoader, "Automatically adding event system feature type to target symbol class '{%s}'.", symClass.ClassName);
            }

            if (on_message_system)
            {
                // 装配消息系统
                symClass.AddFeatureType(typeof(MessageSystemAttribute));
                Debugger.Info(LogGroupTag.CodeLoader, "Automatically adding message system feature type to target symbol class '{%s}'.", symClass.ClassName);
            }
        }
    }
}
