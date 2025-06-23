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
        private string m_methodName;
        /// <summary>
        /// 函数的完整名称
        /// </summary>
        private string m_fullName;
        /// <summary>
        /// 函数的返回类型
        /// </summary>
        private SystemType m_returnType;
        /// <summary>
        /// 函数对象实例
        /// </summary>
        private MethodInfo m_methodInfo;

        /// <summary>
        /// 函数参数列表
        /// </summary>
        private ParameterInfo[] m_parameters;

        /// <summary>
        /// 函数是否为静态类型
        /// </summary>
        private bool m_isStatic;

        /// <summary>
        /// 函数是否为扩展类型
        /// </summary>
        private bool m_isExtense;

        public MethodInfo MethodInfo
        {
            get { return m_methodInfo; }
            internal set
            {
                m_methodInfo = value;

                m_methodName = m_methodInfo.Name;
                m_fullName = NovaEngine.Utility.Text.GetFullName(m_methodInfo);
                m_returnType = m_methodInfo.ReturnType;

                ParameterInfo[] parameters = m_methodInfo.GetParameters();
                m_parameters = new ParameterInfo[parameters.Length];
                for (int n = 0; n < parameters.Length; ++n)
                {
                    m_parameters[n] = parameters[n];
                }

                m_isStatic = m_methodInfo.IsStatic;
                m_isExtense = NovaEngine.Utility.Reflection.IsTypeOfExtension(m_methodInfo);
            }
        }

        public string MethodName => m_methodName;
        public string FullName => m_fullName;
        public SystemType ReturnType => m_returnType;

        public ParameterInfo[] Parameters => m_parameters;

        public bool IsStatic => m_isStatic;
        public bool IsExtense => m_isExtense;

        public SymMethod() { }

        ~SymMethod()
        {
            m_methodInfo = null;
        }

        /// <summary>
        /// 获取指定索引指向的参数信息
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引的参数信息，若不存在返回null</returns>
        public ParameterInfo GetParameter(int index)
        {
            if (index < 0 || index >= m_parameters.Length)
            {
                Debugger.Warn("The method parameter search index '{0}' out of the range, getted it failed.", index);
                return null;
            }

            return m_parameters[index];
        }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.AppendFormat("{0}, ", base.ToString());
            sb.AppendFormat("MethodName = {0}, ", m_methodName);
            sb.AppendFormat("FullName = {0}, ", m_fullName);
            sb.AppendFormat("ReturnType = {0}, ", NovaEngine.Utility.Text.ToString(m_returnType));
            sb.AppendFormat("MethodInfo = {0}, ", NovaEngine.Utility.Text.ToString(m_methodInfo));
            return sb.ToString();
        }
    }
}
