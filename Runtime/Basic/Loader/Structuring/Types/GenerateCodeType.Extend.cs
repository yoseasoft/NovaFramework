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
        /// 原型对象消息处理的扩展定义调用模块的数据管理容器
        /// </summary>
        private MethodTypeList<MessageBindingMethodTypeCodeInfo> _messageCallMethodTypes;

        internal MethodTypeList<InputResponsingMethodTypeCodeInfo> InputCallMethodTypes => _inputCallMethodTypes;

        internal MethodTypeList<EventSubscribingMethodTypeCodeInfo> EventCallMethodTypes => _eventCallMethodTypes;

        internal MethodTypeList<MessageBindingMethodTypeCodeInfo> MessageCallMethodTypes => _messageCallMethodTypes;

        #region 扩展输入调用模块结构信息操作函数

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="invoke">函数的结构信息</param>
        public void AddInputCallMethodType(InputResponsingMethodTypeCodeInfo invoke)
        {
            if (null == _inputCallMethodTypes)
            {
                _inputCallMethodTypes = new MethodTypeList<InputResponsingMethodTypeCodeInfo>();
            }

            _inputCallMethodTypes.Add(invoke);
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
        /// <param name="invoke">函数的结构信息</param>
        public void AddEventCallMethodType(EventSubscribingMethodTypeCodeInfo invoke)
        {
            if (null == _eventCallMethodTypes)
            {
                _eventCallMethodTypes = new MethodTypeList<EventSubscribingMethodTypeCodeInfo>();
            }

            _eventCallMethodTypes.Add(invoke);
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
        /// <param name="invoke">函数的结构信息</param>
        public void AddMessageCallMethodType(MessageBindingMethodTypeCodeInfo invoke)
        {
            if (null == _messageCallMethodTypes)
            {
                _messageCallMethodTypes = new MethodTypeList<MessageBindingMethodTypeCodeInfo>();
            }

            _messageCallMethodTypes.Add(invoke);
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
        public MessageBindingMethodTypeCodeInfo GetMessageCallMethodType(int index)
        {
            return _messageCallMethodTypes?.Get(index);
        }

        #endregion

    }
}
