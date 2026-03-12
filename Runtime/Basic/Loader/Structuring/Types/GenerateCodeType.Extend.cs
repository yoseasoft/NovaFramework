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

using System;

namespace GameEngine.Loader.Structuring
{
    /// <summary>
    /// 扩展定义模块的编码结构信息对象类
    /// </summary>
    internal abstract class ExtendCodeInfo : GeneralCodeInfo
    { }

    /// <summary>
    /// 扩展定义调用模块的结构信息
    /// </summary>
    internal sealed class ExtendCallCodeInfo : ExtendCodeInfo
    {
        /// <summary>
        /// 原型对象输入响应的扩展定义调用模块的数据管理容器
        /// </summary>
        private MethodTypeList<InputResponsingMethodTypeCodeInfo> _inputCallMethodTypes;

        /// <summary>
        /// 原型对象事件订阅的扩展定义调用模块的数据管理容器
        /// </summary>
        private MethodTypeList<EventSubscribingMethodTypeCodeInfo> _eventCallMethodTypes;

        /// <summary>
        /// 原型对象消息监听的扩展定义调用模块的数据管理容器
        /// </summary>
        private MethodTypeList<MessageListeningMethodTypeCodeInfo> _messageCallMethodTypes;

        /// <summary>
        /// 原型对象同步传输的扩展定义调用模块的数据管理容器
        /// </summary>
        private MethodTypeList<ReplicateCommunicatingMethodTypeCodeInfo> _replicateCallMethodTypes;

        /// <summary>
        /// 获取原型对象输入响应函数列表
        /// </summary>
        internal MethodTypeList<InputResponsingMethodTypeCodeInfo> InputCallMethodTypes => _inputCallMethodTypes;
        /// <summary>
        /// 获取原型对象事件订阅函数列表
        /// </summary>
        internal MethodTypeList<EventSubscribingMethodTypeCodeInfo> EventCallMethodTypes => _eventCallMethodTypes;
        /// <summary>
        /// 获取原型对象消息监听函数列表
        /// </summary>
        internal MethodTypeList<MessageListeningMethodTypeCodeInfo> MessageCallMethodTypes => _messageCallMethodTypes;
        /// <summary>
        /// 获取原型对象同步传输函数列表
        /// </summary>
        internal MethodTypeList<ReplicateCommunicatingMethodTypeCodeInfo> ReplicateCallMethodTypes => _replicateCallMethodTypes;

        #region 扩展输入调用模块结构信息操作函数

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        public void AddInputCallMethodType(InputResponsingMethodTypeCodeInfo codeInfo)
        {
            if (null == _inputCallMethodTypes)
            {
                _inputCallMethodTypes = new MethodTypeList<InputResponsingMethodTypeCodeInfo>();
            }

            _inputCallMethodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有函数的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllInputCallMethodTypes()
        {
            _inputCallMethodTypes?.Clear();
            _inputCallMethodTypes = null;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        public int GetInputCallMethodTypeCount()
        {
            return _inputCallMethodTypes?.Count() ?? 0;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public InputResponsingMethodTypeCodeInfo GetInputCallMethodType(int index)
        {
            return _inputCallMethodTypes?.Get(index);
        }

        #endregion

        #region 扩展事件调用模块结构信息操作函数

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        public void AddEventCallMethodType(EventSubscribingMethodTypeCodeInfo codeInfo)
        {
            if (null == _eventCallMethodTypes)
            {
                _eventCallMethodTypes = new MethodTypeList<EventSubscribingMethodTypeCodeInfo>();
            }

            _eventCallMethodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有函数的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllEventCallMethodTypes()
        {
            _eventCallMethodTypes?.Clear();
            _eventCallMethodTypes = null;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        public int GetEventCallMethodTypeCount()
        {
            return _eventCallMethodTypes?.Count() ?? 0;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public EventSubscribingMethodTypeCodeInfo GetEventCallMethodType(int index)
        {
            return _eventCallMethodTypes?.Get(index);
        }

        #endregion

        #region 扩展消息调用模块结构信息操作函数

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        public void AddMessageCallMethodType(MessageListeningMethodTypeCodeInfo codeInfo)
        {
            if (null == _messageCallMethodTypes)
            {
                _messageCallMethodTypes = new MethodTypeList<MessageListeningMethodTypeCodeInfo>();
            }

            _messageCallMethodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有函数的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllMessageCallMethodTypes()
        {
            _messageCallMethodTypes?.Clear();
            _messageCallMethodTypes = null;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        public int GetMessageCallMethodTypeCount()
        {
            return _messageCallMethodTypes?.Count() ?? 0;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public MessageListeningMethodTypeCodeInfo GetMessageCallMethodType(int index)
        {
            return _messageCallMethodTypes?.Get(index);
        }

        #endregion

        #region 扩展同步调用模块结构信息操作函数

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        public void AddReplicateCallMethodType(ReplicateCommunicatingMethodTypeCodeInfo codeInfo)
        {
            if (null == _replicateCallMethodTypes)
            {
                _replicateCallMethodTypes = new MethodTypeList<ReplicateCommunicatingMethodTypeCodeInfo>();
            }

            _replicateCallMethodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有函数的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllReplicateCallMethodTypes()
        {
            _replicateCallMethodTypes?.Clear();
            _replicateCallMethodTypes = null;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        public int GetReplicateCallMethodTypeCount()
        {
            return _replicateCallMethodTypes?.Count() ?? 0;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public ReplicateCommunicatingMethodTypeCodeInfo GetReplicateCallMethodType(int index)
        {
            return _replicateCallMethodTypes?.Get(index);
        }

        #endregion
    }
}
