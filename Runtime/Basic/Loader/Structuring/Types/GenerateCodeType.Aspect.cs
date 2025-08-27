/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemAction_object = System.Action<object>;

namespace GameEngine.Loader.Structuring
{
    /// <summary>
    /// 切面控制模块的编码结构信息对象类
    /// </summary>
    internal abstract class AspectCodeInfo : GeneralCodeInfo
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
    /// 切面调用模块的编码结构信息对象类
    /// </summary>
    internal class AspectCallCodeInfo : AspectCodeInfo
    {
        /// <summary>
        /// 切面调用模块的函数结构信息管理容器
        /// </summary>
        private IList<AspectCallMethodTypeCodeInfo> _methodTypes;

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        internal void AddMethodType(AspectCallMethodTypeCodeInfo codeInfo)
        {
            if (null == _methodTypes)
            {
                _methodTypes = new List<AspectCallMethodTypeCodeInfo>();
            }

            if (_methodTypes.Contains(codeInfo))
            {
                Debugger.Warn("The aspect call class type '{0}' was already registed target method '{1}', repeat added it failed.",
                        NovaEngine.Utility.Text.ToString(_classType), codeInfo.MethodName);
                return;
            }

            _methodTypes.Add(codeInfo);
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
        internal AspectCallMethodTypeCodeInfo GetMethodType(int index)
        {
            if (null == _methodTypes || index < 0 || index >= _methodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for aspect call method type code info list.", index);
                return null;
            }

            return _methodTypes[index];
        }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("AspectCall = { ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());

            sb.AppendFormat("MethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<AspectCallMethodTypeCodeInfo>(_methodTypes));

            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 切面调用模块的函数结构信息
    /// </summary>
    internal class AspectCallMethodTypeCodeInfo
    {
        /// <summary>
        /// 切面调用模块的完整名称
        /// </summary>
        private string _fullname;
        /// <summary>
        /// 切面调用模块的目标对象类型
        /// </summary>
        private SystemType _targetType;
        /// <summary>
        /// 切面调用模块的目标函数名称
        /// </summary>
        private string _methodName;
        /// <summary>
        /// 切面调用模块的接入访问方式
        /// </summary>
        private AspectAccessType _accessType;
        /// <summary>
        /// 切面调用模块的回调函数信息
        /// </summary>
        private SystemMethodInfo _methodInfo;
        /// <summary>
        /// 切面调用模块的回调句柄实例
        /// </summary>
        private SystemAction_object _callback;

        public string Fullname { get { return _fullname; } internal set { _fullname = value; } }
        public SystemType TargetType { get { return _targetType; } internal set { _targetType = value; } }
        public string MethodName { get { return _methodName; } internal set { _methodName = value; } }
        public AspectAccessType AccessType { get { return _accessType; } internal set { _accessType = value; } }
        public SystemMethodInfo MethodInfo { get { return _methodInfo; } internal set { _methodInfo = value; } }
        public SystemAction_object Callback { get { return _callback; } internal set { _callback = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("Fullname = {0}, ", _fullname);
            sb.AppendFormat("TargetType = {0}, ", NovaEngine.Utility.Text.ToString(_targetType));
            sb.AppendFormat("MethodName = {0}, ", _methodName ?? NovaEngine.Definition.CString.Unknown);
            sb.AppendFormat("AccessType = {0}, ", _accessType.ToString());
            sb.AppendFormat("MethodInfo = {0}, ", NovaEngine.Utility.Text.ToString(_methodInfo));
            sb.Append("}");
            return sb.ToString();
        }
    }
}
