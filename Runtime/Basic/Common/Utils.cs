/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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
using System.Reflection;

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemDelegate = System.Delegate;

namespace GameEngine
{
    /// <summary>
    /// 框架内部的辅助工具类，用于实现一些通用的辅助接口函数
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// 获取目标对象类型下所有指定行为模式的函数信息，合并创建一个回调句柄实例
        /// </summary>
        /// <param name="targetObject">目标对象实例</param>
        /// <param name="classType">目标对象类型</param>
        /// <param name="attributeType">行为属性类型</param>
        /// <returns>返回创建的回调句柄实例，若不存在该行为模式的函数信息，则返回null</returns>
        public static SystemDelegate CreateSubmoduleBehaviourCallback(object targetObject, SystemType classType, SystemType attributeType)
        {
            IList<SystemDelegate> list = new List<SystemDelegate>();
            MethodInfo[] methods = classType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for (int n = 0; n < methods.Length; ++n)
            {
                MethodInfo method = methods[n];
                SystemAttribute attr = method.GetCustomAttribute(attributeType);
                if (null != attr)
                {
                    SystemDelegate c;
                    if (null == targetObject)
                    {
                        // 这里可以做一个检查，要求函数必须为静态函数
                        c = method.CreateDelegate(typeof(NovaEngine.Definition.Delegate.EmptyFunctionHandler));
                    }
                    else
                    {
                        c = method.CreateDelegate(typeof(NovaEngine.Definition.Delegate.EmptyFunctionHandler), targetObject);
                    }

                    list.Add(c);
                }
            }

            // 先重置回调
            SystemDelegate callback = null;

            if (list.Count > 0)
            {
                for (int n = 0; n < list.Count; ++n)
                {
                    if (null == callback)
                    {
                        callback = list[n];
                    }
                    else
                    {
                        callback = SystemDelegate.Combine(list[n], callback);
                    }
                }
            }

            return callback;
        }
    }
}
