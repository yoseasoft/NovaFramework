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
using SystemStringBuilder = System.Text.StringBuilder;
using SystemFieldInfo = System.Reflection.FieldInfo;
using SystemPropertyInfo = System.Reflection.PropertyInfo;
using SystemBindingFlags = System.Reflection.BindingFlags;

namespace NovaEngine
{
    /// <summary>
    /// 格式化接口集合工具类
    /// </summary>
    public static partial class Formatter
    {
        /// <summary>
        /// 详细模式的对象字符串信息输出接口函数
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <returns>返回对象实例的详细字符串信息</returns>
        public static string ToVerboseInfo(object obj)
        {
            if (null == obj)
            {
                return Definition.CString.Null;
            }

            SystemType classType = obj.GetType();

            if (IsBasicDataType(classType))
            {
                return obj.ToString();
            }

            if (IsContainerType(classType))
            {
                return GetContainerObjectVerboseInfo(obj);
            }

            if (IsCoreSystemLibraryType(classType))
            {
                return GetCoreSystemObjectVerboseInfo(obj);
            }

            return GetCustomObjectVerboseInfo(obj);
        }

        /// <summary>
        /// 获取指定对象实例的简略字符串信息<br/>
        /// 请注意，这里的对象必须为非字符串类型的引用对象实例
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <returns>返回对象实例的简略字符串信息</returns>
        private static string GetCoreSystemObjectVerboseInfo(object obj)
        {
            SystemStringBuilder sb = new SystemStringBuilder();

            SystemType classType = obj.GetType();
            sb.Append(Definition.CCharacter.LeftParen);
            sb.Append(Utility.Text.GetFullName(classType));
            sb.Append(Definition.CCharacter.RightParen);
            sb.Append(obj.ToString());

            return sb.ToString();
        }

        private static string GetContainerObjectVerboseInfo(object obj)
        {
            SystemType classType = obj.GetType();

            if (classType.IsArray)
            {
                // 获取数组元素类型
                // SystemType elementType = classType.GetElementType();
                // 获取泛型方法
                // SystemMethodInfo methodInfo = typeof(Formatter).GetMethod("ToString_Array").MakeGenericMethod(elementType);
                // 调用方法
                // return methodInfo.Invoke(null, new object[] { obj }) as string;

                return ToString_Array((System.Array) obj);
            }

            return null;
        }

        private static string GetCustomObjectVerboseInfo(object obj)
        {
            SystemStringBuilder sb = new SystemStringBuilder();

            SystemType targetType = obj.GetType();
            sb.AppendFormat("Class={0}", NovaEngine.Utility.Text.GetFullName(targetType));
            sb.Append("{");

            sb.Append("Field={");
            SystemFieldInfo[] fields = targetType.GetFields(SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Instance | SystemBindingFlags.Static);
            for (int n = 0; null != fields && n < fields.Length; ++n)
            {
                if (n > 0) sb.Append(Definition.CCharacter.Comma);

                SystemFieldInfo fieldInfo = fields[n];
                sb.AppendFormat("[{0}]=\"{1}\"", fieldInfo.Name, fieldInfo.GetValue(obj));
            }
            sb.Append("},");

            sb.Append("Property={");
            SystemPropertyInfo[] properties = targetType.GetProperties(SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Instance | SystemBindingFlags.Static);
            for (int n = 0; null != properties && n < properties.Length; ++n)
            {
                // 你可以通过检查方法的声明类型来判断该方法是否属于匿名类型。
                // 在 C# 中，匿名类型通常由编译器生成，其类型名称会以 < > 开头。
                // 例如，匿名类型的名称可能看起来像这样：<>f__AnonymousType0。
                // bool isAnonymousType = getMethod.DeclaringType.Name.StartsWith("<") && getMethod.DeclaringType.Name.EndsWith(">");

                if (n > 0) sb.Append(Definition.CCharacter.Comma);

                SystemPropertyInfo propertyInfo = properties[n];
                sb.AppendFormat("[{0}]=\"{1}\"", propertyInfo.Name, propertyInfo.GetValue(obj));
            }
            sb.Append("},");

            sb.Append("}");

            return sb.ToString();
        }
    }
}
