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
        /// 检查原型对象扩展编程接口回调函数的格式是否正确
        /// </summary>
        /// <param name="methodInfo">函数类型</param>
        /// <returns>若格式正确则返回true，否则返回false</returns>
        public static bool CheckFunctionFormatOfApiCallWithBeanExtensionType(SystemMethodInfo methodInfo)
        {
            // 函数返回值必须为“void”
            if (typeof(void) != methodInfo.ReturnType)
            {
                return false;
            }

            // 扩展函数必须为静态类型
            if (false == methodInfo.IsStatic)
            {
                return false;
            }

            SystemParameterInfo[] paramInfos = methodInfo.GetParameters();
            if (null == paramInfos || paramInfos.Length <= 0)
            {
                return false;
            }

            // 编程接口函数格式：
            // static void OnApi(this IBean self, params object[] args);

            // 第一个参数必须为原型类的子类，且必须是可实例化的类
            if (false == typeof(IBean).IsAssignableFrom(paramInfos[0].ParameterType) ||
                false == NovaEngine.Utility.Reflection.IsTypeOfInstantiableClass(paramInfos[0].ParameterType))
            {
                return false;
            }

            // 后面有任何类型的参数都是正常的
            return true;
        }
    }
}
