/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemParameterInfo = System.Reflection.ParameterInfo;

namespace GameEngine.Loader.Inspecting
{
    /// <summary>
    /// 程序集的安全检查类，对业务层载入的所有对象类进行安全检查的分析处理，确保代码的正确运行
    /// </summary>
    internal static partial class CodeInspector
    {
        /// <summary>
        /// 检查指定回调函数的格式是否正确
        /// </summary>
        /// <param name="methodInfo">函数类型</param>
        /// <returns>若格式正确则返回true，否则返回false</returns>
        public static bool CheckFunctionFormatOfTarget(SystemMethodInfo methodInfo)
        {
            return false;
        }

        /// <summary>
        /// 检测目标函数是否为无参的函数类型
        /// </summary>
        /// <param name="methodInfo">函数类型</param>
        /// <returns>若为无参格式则返回true，否则返回false</returns>
        public static bool CheckFunctionFormatOfTargetWithNullParameterType(SystemMethodInfo methodInfo)
        {
            // 无参类型的函数有三种格式:
            // 1. [static] void OnFunctionCall();
            // 2. static void OnFunctionCall(IBean obj);
            // 2. static void OnFunctionCall(this IBean obj);
            SystemParameterInfo[] paramInfos = methodInfo.GetParameters();
            if (null == paramInfos || paramInfos.Length <= 0)
            {
                return true;
            }

            if (paramInfos.Length == 1 && methodInfo.IsStatic) // 无参类型消息如果存在一个参数，那必然是静态函数
            {
                // 扩展函数存在一个参数，就是扩展对象自身
                // 静态函数也允许指定一个Bean对象的参数
                if (typeof(IBean).IsAssignableFrom(paramInfos[0].ParameterType))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
