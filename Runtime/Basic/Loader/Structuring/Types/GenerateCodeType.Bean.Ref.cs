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
    /// 引用对象模块的结构信息
    /// </summary>
    internal abstract class RefCodeInfo : BaseBeanCodeInfo
    {
        /// <summary>
        /// 状态转换类的函数结构信息管理容器
        /// </summary>
        private MethodTypeList<StateTransitioningMethodTypeCodeInfo> _stateTransitioningMethodTypes;

        internal MethodTypeList<StateTransitioningMethodTypeCodeInfo> StateTransitioningMethodTypes => _stateTransitioningMethodTypes;

        #region 状态转换类结构信息操作函数

        /// <summary>
        /// 新增指定状态转换函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        public void AddStateTransitioningMethodType(StateTransitioningMethodTypeCodeInfo codeInfo)
        {
            if (null == _stateTransitioningMethodTypes)
            {
                _stateTransitioningMethodTypes = new MethodTypeList<StateTransitioningMethodTypeCodeInfo>();
            }

            _stateTransitioningMethodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有状态转换函数的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllStateTransitioningMethodTypes()
        {
            _stateTransitioningMethodTypes?.Clear();
            _stateTransitioningMethodTypes = null;
        }

        /// <summary>
        /// 获取当前状态转换函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        public int GetStateTransitioningMethodTypeCount()
        {
            return _stateTransitioningMethodTypes?.Count() ?? 0;
        }

        /// <summary>
        /// 获取当前状态转换函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public StateTransitioningMethodTypeCodeInfo GetStateTransitioningMethodType(int index)
        {
            return _stateTransitioningMethodTypes?.Get(index);
        }

        #endregion
    }
}
