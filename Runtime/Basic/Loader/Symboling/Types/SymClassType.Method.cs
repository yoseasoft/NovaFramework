/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
using System.Reflection;

using SystemType = System.Type;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader.Symboling
{
    /// <summary>
    /// 通用对象函数的标记数据的结构信息
    /// </summary>
    public class SymMethod : SymBase
    {
        /// <summary>
        /// 函数的名称
        /// </summary>
        private string _methodName;
        /// <summary>
        /// 函数的完整名称
        /// </summary>
        private string _fullName;
        /// <summary>
        /// 函数对象实例
        /// </summary>
        private MethodInfo _methodInfo;

        /// <summary>
        /// 函数参数列表
        /// </summary>
        private ParameterInfo[] _parameters;

        /// <summary>
        /// 函数参数类型
        /// </summary>
        private SystemType[] _parameterTypes;

        /// <summary>
        /// 函数的返回类型
        /// </summary>
        private SystemType _returnType;

        /// <summary>
        /// 函数扩展参数类型
        /// 该参数仅当函数为扩展函数类型时有效
        /// </summary>
        private SystemType _extensionParameterType;

        /// <summary>
        /// 函数是否为静态类型
        /// </summary>
        private bool _isStatic;

        /// <summary>
        /// 函数是否为扩展类型
        /// </summary>
        private bool _isExtension;

        public MethodInfo MethodInfo
        {
            get { return _methodInfo; }
            internal set
            {
                _methodInfo = value;

                _methodName = _methodInfo.Name;
                _fullName = NovaEngine.Utility.Text.GetFullName(_methodInfo);

                ParameterInfo[] parameters = _methodInfo.GetParameters();
                _parameters = new ParameterInfo[parameters.Length];
                _parameterTypes = new SystemType[parameters.Length];
                _extensionParameterType = null;
                for (int n = 0; n < parameters.Length; ++n)
                {
                    _parameters[n] = parameters[n];
                    _parameterTypes[n] = parameters[n].ParameterType;
                }
                _returnType = _methodInfo.ReturnType;

                if (typeof(void) == _returnType)
                {
                    _returnType = null;
                }

                _isStatic = _methodInfo.IsStatic;
                _isExtension = NovaEngine.Utility.Reflection.IsTypeOfExtension(_methodInfo);

                if (_isExtension)
                {
                    Debugger.Assert(_parameterTypes.Length > 0, "Parameters index out of range.");

                    // 扩展的this参数固定为函数的第一个参数
                    _extensionParameterType = _parameterTypes[0];
                }
            }
        }

        public string MethodName => _methodName;
        public string FullName => _fullName;

        public ParameterInfo[] Parameters => _parameters;
        public SystemType[] ParameterTypes => _parameterTypes;
        public SystemType ReturnType => _returnType;
        public SystemType ExtensionParameterType => _extensionParameterType;

        public bool IsStatic => _isStatic;
        public bool IsExtension => _isExtension;

        public SymMethod() : base() { }

        ~SymMethod()
        {
            _methodInfo = null;

            _parameters = null;
            _parameterTypes = null;
            _returnType = null;
            _extensionParameterType = null;
        }

        /// <summary>
        /// 获取指定索引指向的参数信息
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引的参数信息，若不存在返回null</returns>
        public ParameterInfo GetParameter(int index)
        {
            if (index < 0 || index >= _parameters.Length)
            {
                Debugger.Warn("The method parameter search index '{0}' out of the range, getted it failed.", index);
                return null;
            }

            return _parameters[index];
        }

        /// <summary>
        /// 获取指定索引指向的参数类型
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引的参数类型，若不存在返回null</returns>
        public SystemType GetParameterType(int index)
        {
            if (index < 0 || index >= _parameterTypes.Length)
            {
                Debugger.Warn("The method parameter type search index '{0}' out of the range, getted it failed.", index);
                return null;
            }

            return _parameterTypes[index];
        }
    }
}
