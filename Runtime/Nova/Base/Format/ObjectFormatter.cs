/// -------------------------------------------------------------------------------
/// NovaEngine Framework
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

using SystemType = System.Type;

namespace NovaEngine
{
    /// <summary>
    /// 格式化接口集合工具类
    /// </summary>
    internal static partial class ObjectFormatter
    {
        /// <summary>
        /// 对象实例格式化字符串信息接口定义
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <returns>返回格式化字符串信息</returns>
        private delegate string FormatToStringCallback(object obj);

        /// <summary>
        /// 对象实例格式化字符串信息获取函数
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <returns>返回对象实例的字符串信息</returns>
        public static string ToString(object obj)
        {
            return ToVerboseInfo(obj);
        }

        /// <summary>
        /// 检测目标对象类型是否为基础数据类型<br/>
        /// 这里将所有的值类型和字符串类型作为基础数据类型；<br/>
        /// 而除字符串以外的所有引用类型，均不认定为基础数据类型；
        /// </summary>
        /// <param name="targetType">目标对象类型</param>
        /// <returns>若目标对象类型为基础数据类型则返回true，否则返回false</returns>
        private static bool IsBasicDataType(SystemType targetType)
        {
            // 值类型
            if (targetType.IsValueType)
            {
                // 暂时先把结构体也统一当做值类型的流程处理
                return true;
            }

            // 此处将字符串也认定为值类型，因为它们的字符串输出流程是一致的
            if (typeof(string) == targetType)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检测目标对象类型是否为系统库定义的对象类型<br/>
        /// 系统库包括基础的DotNet库及Unity引擎库
        /// </summary>
        /// <param name="targetType">目标对象类型</param>
        /// <returns>若目标对象类型为系统库定义对象类型则返回true，否则返回false</returns>
        private static bool IsCoreSystemObjectType(SystemType targetType)
        {
            string ns = targetType.Namespace;

            if (ns.StartsWith("System") || ns.StartsWith("Unity"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检测目标对象类型是否为容器类型<br/>
        /// 容器类型包括：Array,List,Set,Dictionary,Queue,Stack等
        /// </summary>
        /// <param name="targetType">目标对象类型</param>
        /// <returns>若目标对象类型为容器类型则返回true，否则返回false</returns>
        private static bool IsContainerObjectType(SystemType targetType)
        {
            // 数组类型
            if (targetType.IsArray)
            {
                return true;
            }

            // 检查是否为泛型容器
            if (targetType.IsGenericType)
            {
                if (targetType.GetGenericTypeDefinition() == typeof(List<>) ||
                    targetType.GetGenericTypeDefinition() == typeof(Dictionary<,>) ||
                    targetType.GetGenericTypeDefinition() == typeof(HashSet<>) ||
                    targetType.GetGenericTypeDefinition() == typeof(Queue<>) ||
                    targetType.GetGenericTypeDefinition() == typeof(Stack<>))
                {
                    return true;
                }
            }

            if (typeof(System.Collections.ICollection).IsAssignableFrom(targetType) ||
                typeof(System.Collections.IList).IsAssignableFrom(targetType) ||
                typeof(System.Collections.IDictionary).IsAssignableFrom(targetType))
            {
                return true;
            }

            return false;
        }
    }
}
