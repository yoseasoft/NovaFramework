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
using SystemAttribute = System.Attribute;
using SystemDelegate = System.Delegate;
using SystemStringBuilder = System.Text.StringBuilder;
using SystemFieldInfo = System.Reflection.FieldInfo;
using SystemPropertyInfo = System.Reflection.PropertyInfo;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemParameterInfo = System.Reflection.ParameterInfo;
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

            if (IsCoreSystemLibraryType(classType))
            {
                return GetCoreSystemObjectVerboseInfo(obj);
            }

            if (IsContainerType(classType))
            {
                return GetCustomObjectVerboseInfo(obj);
            }

            return obj.ToString();
        }

        /// <summary>
        /// 获取指定对象实例的简略字符串信息<br/>
        /// 请注意，这里的对象必须为非字符串类型的引用对象实例
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <returns>返回对象实例的简略字符串信息</returns>
        private static string GetCoreSystemObjectVerboseInfo(object obj)
        {
            SystemType classType = obj.GetType();

            if (obj is SystemType targetType)
            {
                return Utility.Text.GetFullName(targetType);
            }
            else if (obj is SystemAttribute attribute)
            {
                return Utility.Text.GetFullName(attribute);
            }
            else if (obj is SystemFieldInfo fieldInfo)
            {
                return Utility.Text.GetFullName(fieldInfo) + Definition.CCharacter.Equal + ToString(fieldInfo.GetValue(obj));
            }
            else if (obj is SystemPropertyInfo propertyInfo)
            {
                return Utility.Text.GetFullName(propertyInfo) + Definition.CCharacter.Equal + ToString(propertyInfo.GetValue(obj));
            }
            else if (obj is SystemDelegate callback)
            {
                return Utility.Text.GetFullName(callback);
            }
            else if (obj is SystemMethodInfo methodInfo)
            {
                return Utility.Text.GetFullName(methodInfo);
            }
            else if (obj is SystemParameterInfo parameterInfo)
            {
                return Utility.Text.GetFullName(parameterInfo);
            }

            return null;
        }

        private static string GetCustomObjectVerboseInfo(object obj)
        {
            SystemStringBuilder sb = new SystemStringBuilder();

            SystemType targetType = obj.GetType();
            sb.AppendFormat("Class={0}", NovaEngine.Utility.Text.GetFullName(targetType));
            sb.Append("{");

            sb.Append("}");

            return sb.ToString();
        }
    }
}
