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
            bool on_extend_supported = false;
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

                if (!on_extend_supported)
                {
                    if (method.IsExtension && (
                        method.HasAttribute(typeof(InputResponseBindingOfTargetAttribute)) ||
                        method.HasAttribute(typeof(EventSubscribeBindingOfTargetAttribute)) ||
                        method.HasAttribute(typeof(MessageListenerBindingOfTargetAttribute))))
                    {
                        on_extend_supported = true;
                    }
                }

                if (!on_input_system)
                {
                    if (false == method.IsExtension && method.HasAttribute(typeof(OnInputDispatchCallAttribute)))
                    {
                        on_input_system = true;
                    }
                }

                if (!on_event_system)
                {
                    if (false == method.IsExtension && method.HasAttribute(typeof(OnEventDispatchCallAttribute)))
                    {
                        on_event_system = true;
                    }
                }

                if (!on_message_system)
                {
                    if (false == method.IsExtension && method.HasAttribute(typeof(OnMessageDispatchCallAttribute)))
                    {
                        on_message_system = true;
                    }
                }
            }

            if (on_extend_supported)
            {
                // 装配扩展支持
                symClass.AddFeatureType(typeof(ExtendSupportedAttribute));
                Debugger.Info(LogGroupTag.CodeLoader, "Automatically adding extend supported feature type to target symbol class '{%s}'.", symClass.ClassName);
            }

            if (on_input_system)
            {
                // 装配编码系统
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
