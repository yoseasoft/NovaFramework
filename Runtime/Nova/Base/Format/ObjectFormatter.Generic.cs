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

using System.Text;
using System.Reflection;

using SystemType = System.Type;

namespace NovaEngine
{
    /// 格式化接口集合工具类
    internal static partial class ObjectFormatter
    {
        /// <summary>
        /// 获取指定对象实例的字符串信息<br/>
        /// 请注意，这里的对象必须为非字符串类型的引用对象实例
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <returns>返回对象实例的字符串信息</returns>
        private static string GetCoreSystemObjectInfo(object obj)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(obj.ToString());

            SystemType classType = obj.GetType();
            sb.Append(Definition.CCharacter.LeftParen);
            sb.Append(Utility.Text.GetFullName(classType));
            sb.Append(Definition.CCharacter.RightParen);

            return sb.ToString();
        }

        /// <summary>
        /// 获取指定容器对象实例的字符串信息
        /// </summary>
        /// <param name="obj">容器对象实例</param>
        /// <param name="callback">回调句柄</param>
        /// <returns>返回容器对象的字符串信息</returns>
        private static string GetContainerObjectInfo(object obj, FormatToStringCallback callback)
        {
            SystemType targetType = obj.GetType();

            // 数组类型
            if (targetType.IsArray)
            {
                // 获取数组元素类型
                // SystemType elementType = classType.GetElementType();
                // 获取泛型方法
                // MethodInfo methodInfo = typeof(Formatter).GetMethod("ToString_Array").MakeGenericMethod(elementType);
                // 调用方法
                // return methodInfo.Invoke(null, new object[] { obj }) as string;

                return Utility.Text.ToString(obj as System.Array, (n, element) =>
                {
                    return $"[{n}]={callback(element)}";
                });
            }

            if (obj is System.Collections.IList __list)
            {
                return Utility.Text.ToString(__list, (n, element) =>
                {
                    return $"[{n}]={callback(element)}";
                });
            }

            if (obj is System.Collections.IDictionary __dictionary)
            {
                return Utility.Text.ToString(__dictionary, (key, value) =>
                {
                    return $"[{callback(key)}]={callback(value)}";
                });
            }

            if (obj is System.Collections.ICollection __collection)
            {
                return Utility.Text.ToString(__collection, (n, element) =>
                {
                    return $"[{n}]={callback(element)}";
                });
            }

            Debugger.Warn("Could not found any container formatter with target object type {%s}.", Utility.Text.GetFullName(targetType));

            return Definition.CString.Null;
        }

        /// <summary>
        /// 获取指定自定义对象实例的字符串信息
        /// </summary>
        /// <param name="obj">自定义对象实例</param>
        /// <param name="callback">回调句柄</param>
        /// <returns>返回自定义对象的字符串信息</returns>
        private static string GetCustomObjectInfo(object obj, FormatToStringCallback callback)
        {
            StringBuilder sb = new StringBuilder();

            SystemType targetType = obj.GetType();
            sb.AppendFormat("Class={0},", NovaEngine.Utility.Text.GetFullName(targetType));

            FieldInfo[] fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (null != fields && fields.Length > 0)
            {
                sb.Append("Field={");
                for (int n = 0; null != fields && n < fields.Length; ++n)
                {
                    if (n > 0) sb.Append(Definition.CCharacter.Comma);

                    FieldInfo fieldInfo = fields[n];
                    sb.AppendFormat("[{0}]=\"{1}\"", fieldInfo.Name, callback(fieldInfo.GetValue(obj)));
                }
                sb.Append("},");
            }

            PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (null != properties && properties.Length > 0)
            {
                sb.Append("Property={");
                for (int n = 0; null != properties && n < properties.Length; ++n)
                {
                    // 你可以通过检查方法的声明类型来判断该方法是否属于匿名类型。
                    // 在 C# 中，匿名类型通常由编译器生成，其类型名称会以 < > 开头。
                    // 例如，匿名类型的名称可能看起来像这样：<>f__AnonymousType0。
                    // bool isAnonymousType = getMethod.DeclaringType.Name.StartsWith("<") && getMethod.DeclaringType.Name.EndsWith(">");

                    if (n > 0) sb.Append(Definition.CCharacter.Comma);

                    PropertyInfo propertyInfo = properties[n];
                    sb.AppendFormat("[{0}]=\"{1}\"", propertyInfo.Name, callback(propertyInfo.GetValue(obj)));
                }
                sb.Append("},");
            }

            return sb.ToString();
        }
    }
}
