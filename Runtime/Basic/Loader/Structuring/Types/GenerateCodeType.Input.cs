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
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader.Structuring
{
    /// <summary>
    /// 输入响应模块的编码结构信息对象类
    /// </summary>
    internal abstract class InputCodeInfo : GeneralCodeInfo
    {
        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("Class = {0}, ", _classType.FullName);
            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 输入响应类的结构信息
    /// </summary>
    internal class InputCallCodeInfo : InputCodeInfo
    {
        /// <summary>
        /// 事件调用类的数据引用对象
        /// </summary>
        private IList<InputCallMethodTypeCodeInfo> _methodTypes;

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="invoke">函数的结构信息</param>
        internal void AddMethodType(InputCallMethodTypeCodeInfo invoke)
        {
            if (null == _methodTypes)
            {
                _methodTypes = new List<InputCallMethodTypeCodeInfo>();
            }

            if (_methodTypes.Contains(invoke))
            {
                Debugger.Warn("The input call class type '{0}' was already registed target keycode '{1}', repeat added it failed.", _classType.FullName, invoke.InputCode);
                return;
            }

            _methodTypes.Add(invoke);
        }

        /// <summary>
        /// 移除所有函数的回调句柄相关的结构信息
        /// </summary>
        internal void RemoveAllMethodTypes()
        {
            _methodTypes?.Clear();
            _methodTypes = null;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        internal int GetMethodTypeCount()
        {
            if (null != _methodTypes)
            {
                return _methodTypes.Count;
            }

            return 0;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        internal InputCallMethodTypeCodeInfo GetMethodType(int index)
        {
            if (null == _methodTypes || index < 0 || index >= _methodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for input call method type code info list.", index);
                return null;
            }

            return _methodTypes[index];
        }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("InputCall = { ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());

            sb.AppendFormat("MethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<InputCallMethodTypeCodeInfo>(_methodTypes));

            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 输入响应类的函数结构信息
    /// </summary>
    internal class InputCallMethodTypeCodeInfo
    {
        /// <summary>
        /// 输入响应类的完整名称
        /// </summary>
        private string _fullname;
        /// <summary>
        /// 输入响应类的目标对象类型
        /// </summary>
        private SystemType _targetType;
        /// <summary>
        /// 输入响应类的监听键码标识
        /// </summary>
        private int _inputCode;
        /// <summary>
        /// 输入响应类的监听操作数据类型
        /// </summary>
        private InputOperationType _operationType;
        /// <summary>
        /// 输入响应类的监听键码集合数据类型
        /// </summary>
        private SystemType _inputDataType;
        /// <summary>
        /// 输入响应类的回调函数
        /// </summary>
        private SystemMethodInfo _method;

        public string Fullname { get { return _fullname; } internal set { _fullname = value; } }
        public SystemType TargetType { get { return _targetType; } internal set { _targetType = value; } }
        public int InputCode { get { return _inputCode; } internal set { _inputCode = value; } }
        public InputOperationType OperationType { get { return _operationType; } internal set { _operationType = value; } }
        public SystemType InputDataType { get { return _inputDataType; } internal set { _inputDataType = value; } }
        public SystemMethodInfo Method { get { return _method; } internal set { _method = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("Fullname = {0}, ", _fullname);
            sb.AppendFormat("TargetType = {0}, ", NovaEngine.Utility.Text.ToString(_targetType));
            sb.AppendFormat("InputCode = {0}, ", _inputCode);
            sb.AppendFormat("OperationType = {0}, ", _operationType);
            sb.AppendFormat("InputDataType = {0}, ", NovaEngine.Utility.Text.ToString(_inputDataType));
            sb.AppendFormat("Method = {0}, ", NovaEngine.Utility.Text.ToString(_method));
            sb.Append("}");
            return sb.ToString();
        }
    }
}
