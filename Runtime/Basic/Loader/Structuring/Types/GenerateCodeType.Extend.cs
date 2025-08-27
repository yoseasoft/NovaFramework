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
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader.Structuring
{
    /// <summary>
    /// 扩展定义模块的编码结构信息对象类
    /// </summary>
    internal abstract class ExtendCodeInfo : GeneralCodeInfo
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
    /// 扩展定义调用模块的结构信息
    /// </summary>
    internal class ExtendCallCodeInfo : ExtendCodeInfo
    {
        /// <summary>
        /// 原型对象输入响应的扩展定义调用模块的数据管理容器
        /// </summary>
        private IList<InputResponsingMethodTypeCodeInfo> _inputCallMethodTypes;

        /// <summary>
        /// 原型对象事件订阅的扩展定义调用模块的数据管理容器
        /// </summary>
        private IList<EventSubscribingMethodTypeCodeInfo> _eventCallMethodTypes;

        /// <summary>
        /// 原型对象消息处理的扩展定义调用模块的数据管理容器
        /// </summary>
        private IList<MessageBindingMethodTypeCodeInfo> _messageCallMethodTypes;

        /// <summary>
        /// 原型对象状态监控的扩展定义调用模块的数据管理容器
        /// </summary>
        private IList<StateTransitioningMethodTypeCodeInfo> _stateCallMethodTypes;

        #region 扩展输入调用模块结构信息操作函数

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="invoke">函数的结构信息</param>
        public void AddInputCallMethodType(InputResponsingMethodTypeCodeInfo invoke)
        {
            if (null == _inputCallMethodTypes)
            {
                _inputCallMethodTypes = new List<InputResponsingMethodTypeCodeInfo>();
            }

            if (_inputCallMethodTypes.Contains(invoke))
            {
                Debugger.Warn("The extend input call class type '{0}' was already registed target input '{1}', repeat added it failed.", _classType.FullName, invoke.InputCode);
                return;
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
            if (null != _inputCallMethodTypes)
            {
                return _inputCallMethodTypes.Count;
            }

            return 0;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public InputResponsingMethodTypeCodeInfo GetInputCallMethodType(int index)
        {
            if (null == _inputCallMethodTypes || index < 0 || index >= _inputCallMethodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for extend input call method type code info list.", index);
                return null;
            }

            return _inputCallMethodTypes[index];
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
                _eventCallMethodTypes = new List<EventSubscribingMethodTypeCodeInfo>();
            }

            if (_eventCallMethodTypes.Contains(invoke))
            {
                Debugger.Warn("The extend event call class type '{0}' was already registed target event '{1}', repeat added it failed.", _classType.FullName, invoke.EventID);
                return;
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
            if (null != _eventCallMethodTypes)
            {
                return _eventCallMethodTypes.Count;
            }

            return 0;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public EventSubscribingMethodTypeCodeInfo GetEventCallMethodType(int index)
        {
            if (null == _eventCallMethodTypes || index < 0 || index >= _eventCallMethodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for extend event call method type code info list.", index);
                return null;
            }

            return _eventCallMethodTypes[index];
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
                _messageCallMethodTypes = new List<MessageBindingMethodTypeCodeInfo>();
            }

            if (_messageCallMethodTypes.Contains(invoke))
            {
                Debugger.Warn("The extend message call class type '{0}' was already registed target event '{1}', repeat added it failed.", _classType.FullName, invoke.Opcode);
                return;
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
            if (null != _messageCallMethodTypes)
            {
                return _messageCallMethodTypes.Count;
            }

            return 0;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public MessageBindingMethodTypeCodeInfo GetMessageCallMethodType(int index)
        {
            if (null == _messageCallMethodTypes || index < 0 || index >= _messageCallMethodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for extend message call method type code info list.", index);
                return null;
            }

            return _messageCallMethodTypes[index];
        }

        #endregion

        #region 扩展状态调用模块结构信息操作函数

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="invoke">函数的结构信息</param>
        public void AddStateCallMethodType(StateTransitioningMethodTypeCodeInfo invoke)
        {
            if (null == _stateCallMethodTypes)
            {
                _stateCallMethodTypes = new List<StateTransitioningMethodTypeCodeInfo>();
            }

            if (_stateCallMethodTypes.Contains(invoke))
            {
                Debugger.Warn("The extend state call class type '{0}' was already registed target state '{1}', repeat added it failed.", _classType.FullName, invoke.StateName);
                return;
            }

            _stateCallMethodTypes.Add(invoke);
        }

        /// <summary>
        /// 移除所有函数的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllStateCallMethodTypes()
        {
            _stateCallMethodTypes?.Clear();
            _stateCallMethodTypes = null;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        public int GetStateCallMethodTypeCount()
        {
            if (null != _stateCallMethodTypes)
            {
                return _stateCallMethodTypes.Count;
            }

            return 0;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public StateTransitioningMethodTypeCodeInfo GetStateCallMethodType(int index)
        {
            if (null == _stateCallMethodTypes || index < 0 || index >= _stateCallMethodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for extend state call method type code info list.", index);
                return null;
            }

            return _stateCallMethodTypes[index];
        }

        #endregion

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("ExtendCall = { ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());

            sb.AppendFormat("InputCallMethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<InputResponsingMethodTypeCodeInfo>(_inputCallMethodTypes));
            sb.AppendFormat("EventCallMethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<EventSubscribingMethodTypeCodeInfo>(_eventCallMethodTypes));
            sb.AppendFormat("MessageCallMethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<MessageBindingMethodTypeCodeInfo>(_messageCallMethodTypes));
            sb.AppendFormat("StateCallMethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<StateTransitioningMethodTypeCodeInfo>(_stateCallMethodTypes));

            sb.Append("}");
            return sb.ToString();
        }
    }
}
