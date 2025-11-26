/// -------------------------------------------------------------------------------
/// NovaEngine Framework
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

using System.Reflection;

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemDelegate = System.Delegate;

namespace NovaEngine
{
    /// <summary>
    /// 格式化接口集合工具类
    /// </summary>
    internal static partial class ObjectFormatter
    {
        /// <summary>
        /// 尝试获取系统反射对象相关的字符串信息
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <param name="text">输出文本信息</param>
        /// <returns>若获取对象实例的字符串信息成功则返回true，否则返回false</returns>
        private static bool TryGetSystemReflectionObjectInfo(object obj, out string text)
        {
            // 处理类型对象的情况
            if (obj is SystemType __type)
            {
                text = Utility.Text.GetFullName(__type);
                return true;
            }

            // 处理特性对象的情况
            if (obj is SystemAttribute __attribute)
            {
                text = Utility.Text.GetFullName(__attribute);
                return true;
            }

            // 处理委托对象的情况
            if (obj is SystemDelegate __delegate)
            {
                text = Utility.Text.GetFullName(__delegate);
                return true;
            }

            // 处理字段对象的情况
            if (obj is FieldInfo __field)
            {
                text = Utility.Text.GetFullName(__field);
                return true;
            }

            // 处理属性对象的情况
            if (obj is PropertyInfo __property)
            {
                text = Utility.Text.GetFullName(__property);
                return true;
            }

            // 处理方法对象的情况
            if (obj is MethodInfo __method)
            {
                text = Utility.Text.GetFullName(__method);
                return true;
            }

            // 处理参数对象的情况
            if (obj is ParameterInfo __parameter)
            {
                text = Utility.Text.GetFullName(__parameter);
                return true;
            }

            text = string.Empty;
            return false;
        }
    }
}
