/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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

namespace GameEngine.Loader.Structuring
{
    /// <summary>
    /// 网络消息模块的编码结构信息对象类
    /// </summary>
    internal abstract class NetworkCodeInfo : GeneralCodeInfo
    { }

    /// <summary>
    /// 网络消息对象模块的结构信息
    /// </summary>
    internal sealed class NetworkMessageCodeInfo : NetworkCodeInfo
    {
        /// <summary>
        /// 网络消息对象模块的协议编码
        /// </summary>
        public int Opcode { get; internal set; }
        /// <summary>
        /// 网络消息对象模块的响应编码
        /// </summary>
        public int ResponseCode { get; internal set; }
    }

    /// <summary>
    /// 网络消息接收模块的结构信息
    /// </summary>
    internal sealed class MessageCallCodeInfo : NetworkCodeInfo
    {
        /// <summary>
        /// 网络消息接收模块的数据引用对象
        /// </summary>
        private MethodTypeList<MessageCallMethodTypeCodeInfo> _methodTypes;

        internal MethodTypeList<MessageCallMethodTypeCodeInfo> MethodTypes => _methodTypes;

        /// <summary>
        /// 新增接收消息的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">接收回调的结构信息</param>
        public void AddMethodType(MessageCallMethodTypeCodeInfo codeInfo)
        {
            if (null == _methodTypes)
            {
                _methodTypes = new MethodTypeList<MessageCallMethodTypeCodeInfo>();
            }

            _methodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有接收消息的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllMethodTypes()
        {
            _methodTypes?.Clear();
            _methodTypes = null;
        }

        /// <summary>
        /// 获取当前接收消息回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回接收消息回调句柄的结构信息数量</returns>
        public int GetMethodTypeCount()
        {
            return _methodTypes?.Count() ?? 0;
        }

        /// <summary>
        /// 获取当前接收消息回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public MessageCallMethodTypeCodeInfo GetMethodType(int index)
        {
            return _methodTypes?.Get(index);
        }
    }

    /// <summary>
    /// 消息接收模块的结构信息
    /// </summary>
    internal class MessageCallMethodTypeCodeInfo : MethodTypeCodeInfo
    {
        /// <summary>
        /// 消息处理模块的协议编码
        /// </summary>
        public int Opcode { get; internal set; }
        /// <summary>
        /// 消息处理模块的消息对象类型
        /// </summary>
        public Type MessageType { get; internal set; }
    }

    /// <summary>
    /// 切面管理的消息绑定函数结构信息
    /// </summary>
    internal sealed class MessageBindingMethodTypeCodeInfo : MessageCallMethodTypeCodeInfo
    {
        /// <summary>
        /// 消息绑定的观察行为类型
        /// </summary>
        public AspectBehaviourType BehaviourType { get; internal set; }
    }
}
