/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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

using System;
using System.Customize.Extension;
using System.Reflection;

namespace GameEngine.Loader.Inspecting
{
    /// 程序集的安全检查类
    internal static partial class CodeInspector
    {
        /// <summary>
        /// 检查同步回调函数的格式是否正确
        /// </summary>
        /// <param name="methodInfo">函数类型</param>
        /// <returns>若格式正确则返回true，否则返回false</returns>
        public static bool CheckFunctionFormatOfReplicateCall(MethodInfo methodInfo)
        {
            // 函数返回值必须为“void”
            if (typeof(void) != methodInfo.ReturnType)
            {
                return false;
            }

            ParameterInfo[] paramInfos = methodInfo.GetParameters();
            if (null == paramInfos || paramInfos.Length <= 0)
            {
                // 可能存在无参的情况
                return true;
            }

            // 同步侦听函数有四种格式:
            // 1. [static] void OnReplicate();
            // 2. static void OnReplicate(IBean obj);
            // 3. [static] void OnReplicate(string tags, ReplicateAnnounceType announceType);
            // 4. static void OnReplicate(IBean obj, string tags, ReplicateAnnounceType announceType);
            if (paramInfos.Length == 1)
            {
                if (paramInfos[0].ParameterType.Is<IBean>())
                {
                    return true;
                }
                else if (paramInfos[0].ParameterType.Is<string>())
                {
                    return true;
                }
            }
            else if (paramInfos.Length == 2)
            {
                if (paramInfos[0].ParameterType.Is<string>() && // 第一个参数为数据标签
                    paramInfos[1].ParameterType.Is<ReplicateAnnounceType>()) // 第二个参数为数据播报类型
                {
                    return true;
                }
                else if (paramInfos[0].ParameterType.Is<IBean>() && // 第一个参数为Bean对象
                         paramInfos[1].ParameterType.Is<string>()) // 第二个参数为数据标签
                {
                    return true;
                }
            }
            else if (paramInfos.Length == 3)
            {
                if (paramInfos[0].ParameterType.Is<IBean>() && // 第一个参数为Bean对象
                    paramInfos[1].ParameterType.Is<string>() && // 第二个参数为数据标签
                    paramInfos[2].ParameterType.Is<ReplicateAnnounceType>()) // 第三个参数为数据播报类型
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检测目标函数是否为无参的同步回调函数类型
        /// </summary>
        /// <param name="methodInfo">函数类型</param>
        /// <returns>若为无参格式则返回true，否则返回false</returns>
        public static bool CheckFunctionFormatOfReplicateCallWithNullParameterType(MethodInfo methodInfo)
        {
            // 无参类型的同步侦听函数有两种格式:
            // 1. [static] void OnReplicate();
            // 2. static void OnReplicate(IBean obj);
            ParameterInfo[] paramInfos = methodInfo.GetParameters();
            if (null == paramInfos || paramInfos.Length <= 0)
            {
                return true;
            }

            if (paramInfos.Length == 1 && methodInfo.IsStatic) // 无参类型同步函数如果存在一个参数，那必然是静态函数
            {
                if (paramInfos[0].ParameterType.Is<IBean>())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检查原型对象扩展同步回调函数的格式是否正确
        /// </summary>
        /// <param name="methodInfo">函数类型</param>
        /// <returns>若格式正确则返回true，否则返回false</returns>
        public static bool CheckFunctionFormatOfReplicateCallWithBeanExtensionType(MethodInfo methodInfo)
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

            ParameterInfo[] paramInfos = methodInfo.GetParameters();
            if (null == paramInfos || paramInfos.Length <= 0)
            {
                return false;
            }

            // 同步侦听函数有两种格式
            // 1. static void OnReplicate(this IBean self);
            // 1. static void OnReplicate(this IBean self, string tags, ReplicateAnnounceType announceType);

            // 第一个参数必须为原型类的子类，且必须是可实例化的类
            if (false == paramInfos[0].ParameterType.Is<IBean>() ||
                false == NovaEngine.Utility.Reflection.IsTypeOfInstantiableClass(paramInfos[0].ParameterType))
            {
                return false;
            }

            if (paramInfos.Length == 1)
            {
                return true;
            }
            else if (paramInfos.Length == 2)
            {
                if (paramInfos[1].ParameterType.Is<string>()) // 第二个参数为数据标签
                {
                    return true;
                }
            }
            else if (paramInfos.Length == 3)
            {
                if (paramInfos[1].ParameterType.Is<string>() && // 第二个参数为数据标签
                    paramInfos[2].ParameterType.Is<ReplicateAnnounceType>()) // 第三个参数为数据播报类型
                {
                    return true;
                }
            }

            return false;
        }
    }
}
