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

using SystemType = System.Type;

namespace GameEngine.Loader.Structuring
{
    /// <summary>
    /// 事件模块的编码结构信息对象类
    /// </summary>
    internal abstract class EventCodeInfo : GeneralCodeInfo
    { }

    /// <summary>
    /// 事件调用模块的编码结构信息对象类
    /// </summary>
    internal class EventCallCodeInfo : EventCodeInfo
    {
        /// <summary>
        /// 事件调用模块的数据引用对象
        /// </summary>
        private MethodTypeList<EventCallMethodTypeCodeInfo> _methodTypes;

        internal MethodTypeList<EventCallMethodTypeCodeInfo> MethodTypes => _methodTypes;

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="invoke">函数的结构信息</param>
        public void AddMethodType(EventCallMethodTypeCodeInfo invoke)
        {
            if (null == _methodTypes)
            {
                _methodTypes = new MethodTypeList<EventCallMethodTypeCodeInfo>();
            }

            _methodTypes.Add(invoke);
        }

        /// <summary>
        /// 移除所有函数的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllMethodTypes()
        {
            _methodTypes?.Clear();
            _methodTypes = null;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        public int GetMethodTypeCount()
        {
            return _methodTypes?.Count() ?? 0;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public EventCallMethodTypeCodeInfo GetMethodType(int index)
        {
            return _methodTypes?.Get(index);
        }
    }

    /// <summary>
    /// 事件调用模块的函数结构信息
    /// </summary>
    internal class EventCallMethodTypeCodeInfo : MethodTypeCodeInfo
    {
        /// <summary>
        /// 事件调用模块的监听事件标识
        /// </summary>
        public int EventID { get; internal set; }
        /// <summary>
        /// 事件调用模块的监听事件数据类型
        /// </summary>
        public SystemType EventDataType { get; internal set; }
    }

    /// <summary>
    /// 切面管理的订阅事件函数结构信息
    /// </summary>
    internal sealed class EventSubscribingMethodTypeCodeInfo : EventCallMethodTypeCodeInfo
    {
        /// <summary>
        /// 订阅绑定的观察行为类型
        /// </summary>
        public AspectBehaviourType BehaviourType { get; internal set; }
    }
}
