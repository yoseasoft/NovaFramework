/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemType = System.Type;

namespace GameEngine.Loader.Structuring
{
    /// <summary>
    /// 基础对象模块的结构信息
    /// </summary>
    internal abstract class BaseBeanCodeInfo : BeanCodeInfo
    {
        /// <summary>
        /// 输入响应类的函数结构信息管理容器
        /// </summary>
        private MethodTypeList<InputResponsingMethodTypeCodeInfo> _inputResponsingMethodTypes;

        /// <summary>
        /// 事件订阅类的函数结构信息管理容器
        /// </summary>
        private MethodTypeList<EventSubscribingMethodTypeCodeInfo> _eventSubscribingMethodTypes;

        /// <summary>
        /// 消息绑定类的函数结构信息管理容器
        /// </summary>
        private MethodTypeList<MessageBindingMethodTypeCodeInfo> _messageBindingMethodTypes;

        internal MethodTypeList<InputResponsingMethodTypeCodeInfo> InputResponsingMethodTypes => _inputResponsingMethodTypes;

        internal MethodTypeList<EventSubscribingMethodTypeCodeInfo> EventSubscribingMethodTypes => _eventSubscribingMethodTypes;

        internal MethodTypeList<MessageBindingMethodTypeCodeInfo> MessageBindingMethodTypes => _messageBindingMethodTypes;

        #region 输入响应类结构信息操作函数

        /// <summary>
        /// 新增指定响应输入函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        public void AddInputResponsingMethodType(InputResponsingMethodTypeCodeInfo codeInfo)
        {
            if (null == _inputResponsingMethodTypes)
            {
                _inputResponsingMethodTypes = new MethodTypeList<InputResponsingMethodTypeCodeInfo>();
            }

            _inputResponsingMethodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有响应输入函数的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllInputResponsingMethodTypes()
        {
            _inputResponsingMethodTypes?.Clear();
            _inputResponsingMethodTypes = null;
        }

        /// <summary>
        /// 获取当前响应输入函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        public int GetInputResponsingMethodTypeCount()
        {
            return _inputResponsingMethodTypes?.Count() ?? 0;
        }

        /// <summary>
        /// 获取当前响应输入函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public InputResponsingMethodTypeCodeInfo GetInputResponsingMethodType(int index)
        {
            return _inputResponsingMethodTypes?.Get(index);
        }

        #endregion

        #region 事件订阅类结构信息操作函数

        /// <summary>
        /// 新增指定订阅事件函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        public void AddEventSubscribingMethodType(EventSubscribingMethodTypeCodeInfo codeInfo)
        {
            if (null == _eventSubscribingMethodTypes)
            {
                _eventSubscribingMethodTypes = new MethodTypeList<EventSubscribingMethodTypeCodeInfo>();
            }

            _eventSubscribingMethodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有订阅事件函数的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllEventSubscribingMethodTypes()
        {
            _eventSubscribingMethodTypes?.Clear();
            _eventSubscribingMethodTypes = null;
        }

        /// <summary>
        /// 获取当前订阅事件函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        public int GetEventSubscribingMethodTypeCount()
        {
            return _eventSubscribingMethodTypes?.Count() ?? 0;
        }

        /// <summary>
        /// 获取当前订阅事件函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public EventSubscribingMethodTypeCodeInfo GetEventSubscribingMethodType(int index)
        {
            return _eventSubscribingMethodTypes?.Get(index);
        }

        #endregion

        #region 消息绑定类结构信息操作函数

        /// <summary>
        /// 新增指定消息绑定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        public void AddMessageBindingMethodType(MessageBindingMethodTypeCodeInfo codeInfo)
        {
            if (null == _messageBindingMethodTypes)
            {
                _messageBindingMethodTypes = new MethodTypeList<MessageBindingMethodTypeCodeInfo>();
            }

            _messageBindingMethodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有消息绑定函数的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllMessageBindingMethodTypes()
        {
            _messageBindingMethodTypes?.Clear();
            _messageBindingMethodTypes = null;
        }

        /// <summary>
        /// 获取当前消息绑定函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        public int GetMessageBindingMethodTypeCount()
        {
            return _messageBindingMethodTypes?.Count() ?? 0;
        }

        /// <summary>
        /// 获取当前消息绑定函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public MessageBindingMethodTypeCodeInfo GetMessageBindingMethodType(int index)
        {
            return _messageBindingMethodTypes?.Get(index);
        }

        #endregion

    }
}
