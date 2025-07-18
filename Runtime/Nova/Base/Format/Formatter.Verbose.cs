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

using SystemType = System.Type;

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
        private static string ToVerboseInfo(object obj)
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

            // 容器检测之所以要放在系统类检测的前面，就是因为容器也属于系统类中的一种
            // 但也有部分自定义容器类，不属于系统类，所以没法放在系统类中统一进行检测
            if (IsContainerObjectType(classType))
            {
                return GetContainerObjectInfo(obj, ToVerboseInfo);
            }

            if (IsCoreSystemObjectType(classType))
            {
                return GetCoreSystemObjectInfo(obj);
            }

            return GetCustomObjectInfo(obj, ToSummaryInfo);
        }
    }
}
