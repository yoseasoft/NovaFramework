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

using System;

namespace GameEngine.Loader.Structuring
{
    /// <summary>
    /// 基础对象模块的结构信息
    /// </summary>
    internal abstract class BaseBeanCodeInfo : BeanCodeInfo
    {
        /// <summary>
        /// 输入转发类的函数结构信息管理容器
        /// </summary>
        private MethodTypeList<InputCallMethodTypeCodeInfo> _inputDispatchingMethodTypes;

        /// <summary>
        /// 事件转发类的函数结构信息管理容器
        /// </summary>
        private MethodTypeList<EventCallMethodTypeCodeInfo> _eventDispatchingMethodTypes;

        /// <summary>
        /// 消息转发类的函数结构信息管理容器
        /// </summary>
        private MethodTypeList<MessageCallMethodTypeCodeInfo> _messageDispatchingMethodTypes;

        /// <summary>
        /// 同步转发类的函数结构信息管理容器
        /// </summary>
        private MethodTypeList<ReplicateCallMethodTypeCodeInfo> _replicateDispatchingMethodTypes;

        /// <summary>
        /// 获取输入转发函数列表
        /// </summary>
        internal MethodTypeList<InputCallMethodTypeCodeInfo> InputDispatchingMethodTypes => _inputDispatchingMethodTypes;
        /// <summary>
        /// 获取事件转发函数列表
        /// </summary>
        internal MethodTypeList<EventCallMethodTypeCodeInfo> EventDispatchingMethodTypes => _eventDispatchingMethodTypes;
        /// <summary>
        /// 获取消息转发函数列表
        /// </summary>
        internal MethodTypeList<MessageCallMethodTypeCodeInfo> MessageDispatchingMethodTypes => _messageDispatchingMethodTypes;
        /// <summary>
        /// 获取同步转发函数列表
        /// </summary>
        internal MethodTypeList<ReplicateCallMethodTypeCodeInfo> ReplicateDispatchingMethodTypes => _replicateDispatchingMethodTypes;

        #region 输入转发类结构信息操作函数

        /// <summary>
        /// 新增指定输入转发函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        public void AddInputDispatchingMethodType(InputCallMethodTypeCodeInfo codeInfo)
        {
            if (null == _inputDispatchingMethodTypes)
            {
                _inputDispatchingMethodTypes = new MethodTypeList<InputCallMethodTypeCodeInfo>();
            }

            _inputDispatchingMethodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有输入转发函数的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllInputDispatchingMethodTypes()
        {
            _inputDispatchingMethodTypes?.Clear();
            _inputDispatchingMethodTypes = null;
        }

        /// <summary>
        /// 获取当前输入转发函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        public int GetInputDispatchingMethodTypeCount()
        {
            return _inputDispatchingMethodTypes?.Count() ?? 0;
        }

        /// <summary>
        /// 获取当前输入转发函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public InputCallMethodTypeCodeInfo GetInputDispatchingMethodType(int index)
        {
            return _inputDispatchingMethodTypes?.Get(index);
        }

        #endregion

        #region 事件转发类结构信息操作函数

        /// <summary>
        /// 新增指定事件转发函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        public void AddEventDispatchingMethodType(EventCallMethodTypeCodeInfo codeInfo)
        {
            if (null == _eventDispatchingMethodTypes)
            {
                _eventDispatchingMethodTypes = new MethodTypeList<EventCallMethodTypeCodeInfo>();
            }

            _eventDispatchingMethodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有事件转发函数的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllEventDispatchingMethodTypes()
        {
            _eventDispatchingMethodTypes?.Clear();
            _eventDispatchingMethodTypes = null;
        }

        /// <summary>
        /// 获取当前事件转发函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        public int GetEventDispatchingMethodTypeCount()
        {
            return _eventDispatchingMethodTypes?.Count() ?? 0;
        }

        /// <summary>
        /// 获取当前事件转发函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public EventCallMethodTypeCodeInfo GetEventDispatchingMethodType(int index)
        {
            return _eventDispatchingMethodTypes?.Get(index);
        }

        #endregion

        #region 消息转发类结构信息操作函数

        /// <summary>
        /// 新增指定消息转发函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        public void AddMessageDispatchingMethodType(MessageCallMethodTypeCodeInfo codeInfo)
        {
            if (null == _messageDispatchingMethodTypes)
            {
                _messageDispatchingMethodTypes = new MethodTypeList<MessageCallMethodTypeCodeInfo>();
            }

            _messageDispatchingMethodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有消息转发函数的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllMessageDispatchingMethodTypes()
        {
            _messageDispatchingMethodTypes?.Clear();
            _messageDispatchingMethodTypes = null;
        }

        /// <summary>
        /// 获取当前消息转发函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        public int GetMessageDispatchingMethodTypeCount()
        {
            return _messageDispatchingMethodTypes?.Count() ?? 0;
        }

        /// <summary>
        /// 获取当前消息转发函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public MessageCallMethodTypeCodeInfo GetMessageDispatchingMethodType(int index)
        {
            return _messageDispatchingMethodTypes?.Get(index);
        }

        #endregion

        #region 同步转发类结构信息操作函数

        /// <summary>
        /// 新增指定同步转发函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        public void AddReplicateDispatchingMethodType(ReplicateCallMethodTypeCodeInfo codeInfo)
        {
            if (null == _replicateDispatchingMethodTypes)
            {
                _replicateDispatchingMethodTypes = new MethodTypeList<ReplicateCallMethodTypeCodeInfo>();
            }

            _replicateDispatchingMethodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有同步转发函数的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllReplicateDispatchingMethodTypes()
        {
            _replicateDispatchingMethodTypes?.Clear();
            _replicateDispatchingMethodTypes = null;
        }

        /// <summary>
        /// 获取当前同步转发函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        public int GetReplicateDispatchingMethodTypeCount()
        {
            return _replicateDispatchingMethodTypes?.Count() ?? 0;
        }

        /// <summary>
        /// 获取当前同步转发函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public ReplicateCallMethodTypeCodeInfo GetReplicateDispatchingMethodType(int index)
        {
            return _replicateDispatchingMethodTypes?.Get(index);
        }

        #endregion
    }
}
