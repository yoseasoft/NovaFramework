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

using SystemType = System.Type;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader.Structuring
{
    /// <summary>
    /// 网络消息模块的编码结构信息对象类
    /// </summary>
    internal abstract class NetworkCodeInfo : GeneralCodeInfo
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
    /// 网络消息对象模块的结构信息
    /// </summary>
    internal sealed class NetworkMessageCodeInfo : NetworkCodeInfo
    {
        /// <summary>
        /// 网络消息对象模块的协议编码
        /// </summary>
        private int _opcode;
        /// <summary>
        /// 网络消息对象模块的响应编码
        /// </summary>
        private int _responseCode;

        public int Opcode { get { return _opcode; } internal set { _opcode = value; } }
        public int ResponseCode { get { return _responseCode; } internal set { _responseCode = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("NetworkMessage = { ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());
            sb.AppendFormat("Opcode = {0}, ", _opcode);
            sb.AppendFormat("ResponseCode = {0}, ", _responseCode);
            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 网络消息接收模块的结构信息
    /// </summary>
    internal sealed class MessageCallCodeInfo : NetworkCodeInfo
    {
        /// <summary>
        /// 网络消息接收模块的数据引用对象
        /// </summary>
        private IList<MessageCallMethodTypeCodeInfo> _methodTypes;

        /// <summary>
        /// 新增接收消息的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">接收回调的结构信息</param>
        internal void AddMethodType(MessageCallMethodTypeCodeInfo codeInfo)
        {
            if (null == _methodTypes)
            {
                _methodTypes = new List<MessageCallMethodTypeCodeInfo>();
            }

            if (_methodTypes.Contains(codeInfo))
            {
                Debugger.Warn("The message call class type '{0}' was already registed target code '{1}', repeat added it failed.", _classType.FullName, codeInfo.Opcode);
                return;
            }

            _methodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有接收消息的回调句柄相关的结构信息
        /// </summary>
        internal void RemoveAllMethodTypes()
        {
            _methodTypes?.Clear();
            _methodTypes = null;
        }

        /// <summary>
        /// 获取当前接收消息回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回接收消息回调句柄的结构信息数量</returns>
        internal int GetMethodTypeCount()
        {
            if (null != _methodTypes)
            {
                return _methodTypes.Count;
            }

            return 0;
        }

        /// <summary>
        /// 获取当前接收消息回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        internal MessageCallMethodTypeCodeInfo GetMethodType(int index)
        {
            if (null == _methodTypes || index < 0 || index >= _methodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for message call method type code info list.", index);
                return null;
            }

            return _methodTypes[index];
        }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("MessageCall = { ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());

            sb.AppendFormat("MethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<MessageCallMethodTypeCodeInfo>(_methodTypes));

            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 消息接收模块的结构信息
    /// </summary>
    internal class MessageCallMethodTypeCodeInfo
    {
        /// <summary>
        /// 消息调用模块的完整名称
        /// </summary>
        private string _fullname;
        /// <summary>
        /// 消息调用模块的目标对象类型
        /// </summary>
        private SystemType _targetType;
        /// <summary>
        /// 消息处理模块的协议编码
        /// </summary>
        private int _opcode;
        /// <summary>
        /// 消息处理模块的消息对象类型
        /// </summary>
        private SystemType _messageType;
        /// <summary>
        /// 消息处理模块的回调函数
        /// </summary>
        private SystemMethodInfo _method;

        public string Fullname { get { return _fullname; } internal set { _fullname = value; } }
        public SystemType TargetType { get { return _targetType; } internal set { _targetType = value; } }
        public int Opcode { get { return _opcode; } internal set { _opcode = value; } }
        public SystemType MessageType { get { return _messageType; } internal set { _messageType = value; } }
        public SystemMethodInfo Method { get { return _method; } internal set { _method = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("Fullname = {0}, ", _fullname);
            sb.AppendFormat("TargetType = {0}, ", NovaEngine.Utility.Text.ToString(_targetType));
            sb.AppendFormat("Opcode = {0}, ", _opcode);
            sb.AppendFormat("MessageType = {0}, ", NovaEngine.Utility.Text.ToString(_messageType));
            sb.AppendFormat("Method = {0}, ", NovaEngine.Utility.Text.ToString(_method));
            sb.Append("}");
            return sb.ToString();
        }
    }
}
