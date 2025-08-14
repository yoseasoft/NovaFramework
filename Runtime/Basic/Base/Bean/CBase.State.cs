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

using System.Collections.Generic;

using SystemType = System.Type;
using SystemDelegate = System.Delegate;
using SystemMethodInfo = System.Reflection.MethodInfo;

namespace GameEngine
{
    /// <summary>
    /// 基础对象抽象类，对需要进行对象定义的场景提供一个通用的基类
    /// </summary>
    public abstract partial class CBase
    {
        /// <summary>
        /// 基础对象的状态轮询初始化函数接口
        /// </summary>
        private void OnStatePollInitialize()
        {
        }

        /// <summary>
        /// 基础对象的状态轮询清理函数接口
        /// </summary>
        private void OnStatePollCleanup()
        {
        }

        #region 基础轮询接口包装结构及处理函数声明（包含轮询接口函数的通用数据结构）

        protected class BasePollSyntaxInfo
        {
            /// <summary>
            /// 轮询函数的目标对象实例
            /// </summary>
            protected readonly CBase _targetObject;
            /// <summary>
            /// 轮询函数的完整名称
            /// </summary>
            protected readonly string _fullname;
            /// <summary>
            /// 轮询函数的函数信息实例
            /// </summary>
            protected readonly SystemMethodInfo _methodInfo;
            /// <summary>
            /// 轮询函数的动态构建回调句柄
            /// </summary>
            protected readonly SystemDelegate _callback;
            /// <summary>
            /// 轮询函数的自动注册状态标识
            /// </summary>
            protected readonly bool _automatically;
            /// <summary>
            /// 轮询函数的扩展定义状态标识
            /// </summary>
            protected readonly bool _isExtensionType;
            /// <summary>
            /// 轮询函数的无参状态标识
            /// </summary>
            protected readonly bool _isNullParameterType;

            public string Fullname => _fullname;
            public SystemMethodInfo MethodInfo => _methodInfo;
            public SystemDelegate Callback => _callback;
            public bool Automatically => _automatically;
            public bool IsExtensionType => _isExtensionType;
            public bool IsNullParameterType => _isNullParameterType;

            protected BasePollSyntaxInfo(CBase targetObject, SystemMethodInfo methodInfo, bool automatically)
            {
                _targetObject = targetObject;
                _methodInfo = methodInfo;
                _automatically = automatically;
                _isExtensionType = NovaEngine.Utility.Reflection.IsTypeOfExtension(methodInfo);
                _isNullParameterType = Loader.Inspecting.CodeInspector.IsNullParameterTypeOfMessageCallFunction(methodInfo);

                object obj = targetObject;
                if (_isExtensionType)
                {
                    // 扩展函数在构建委托时不需要传入运行时对象实例，而是在调用时传入
                    obj = null;
                }

                string fullname = GenCallName(methodInfo);

                SystemDelegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(obj, methodInfo);
                Debugger.Assert(null != callback, "Invalid method type.");

                _fullname = fullname;
                _callback = callback;
            }

            /// <summary>
            /// 根据函数信息生成事件回调的名字标签
            /// </summary>
            /// <param name="methodInfo">函数对象信息</param>
            /// <returns>返回通过函数信息生成的名字标签</returns>
            protected internal static string GenCallName(SystemMethodInfo methodInfo)
            {
                return NovaEngine.Utility.Text.GetFullName(methodInfo);
            }
        }

        #endregion
    }
}
