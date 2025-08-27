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
using SystemDelegate = System.Delegate;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemStringBuilder = System.Text.StringBuilder;

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
        private IList<StateTransitioningMethodTypeCodeInfo> _stateTransitioningMethodTypes;

        #region 状态转换类结构信息操作函数

        /// <summary>
        /// 新增指定状态转换函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        internal void AddStateTransitioningMethodType(StateTransitioningMethodTypeCodeInfo codeInfo)
        {
            if (null == _stateTransitioningMethodTypes)
            {
                _stateTransitioningMethodTypes = new List<StateTransitioningMethodTypeCodeInfo>();
            }

            if (_stateTransitioningMethodTypes.Contains(codeInfo))
            {
                Debugger.Warn("The state transitioning class type '{0}' was already registed target method '{1}', repeat added it failed.",
                        _classType.FullName, NovaEngine.Utility.Text.ToString(codeInfo.Method));
                return;
            }

            _stateTransitioningMethodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有状态转换函数的回调句柄相关的结构信息
        /// </summary>
        internal void RemoveAllStateTransitioningMethodTypes()
        {
            _stateTransitioningMethodTypes?.Clear();
            _stateTransitioningMethodTypes = null;
        }

        /// <summary>
        /// 获取当前状态转换函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        internal int GetStateTransitioningMethodTypeCount()
        {
            if (null != _stateTransitioningMethodTypes)
            {
                return _stateTransitioningMethodTypes.Count;
            }

            return 0;
        }

        /// <summary>
        /// 获取当前状态转换函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        internal StateTransitioningMethodTypeCodeInfo GetStateTransitioningMethodType(int index)
        {
            if (null == _stateTransitioningMethodTypes || index < 0 || index >= _stateTransitioningMethodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for state transitioning method type code info list.", index);
                return null;
            }

            return _stateTransitioningMethodTypes[index];
        }

        #endregion

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());
            sb.AppendFormat("StateTransitioningMethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<StateTransitioningMethodTypeCodeInfo>(_stateTransitioningMethodTypes));
            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 标准状态转换函数结构信息
    /// </summary>
    internal class StateTransitioningMethodTypeCodeInfo
    {
        /// <summary>
        /// 状态转换类的完整名称
        /// </summary>
        private string _fullname;
        /// <summary>
        /// 状态转换类的目标对象类型
        /// </summary>
        private SystemType _targetType;
        /// <summary>
        /// 状态转换类的状态名称
        /// </summary>
        private string _stateName;
        /// <summary>
        /// 状态转换的访问类型
        /// </summary>
        private StateAccessType _accessType;
        /// <summary>
        /// 状态转换的观察行为类型
        /// </summary>
        private AspectBehaviourType _behaviourType;
        /// <summary>
        /// 状态转换类的回调函数
        /// </summary>
        private SystemMethodInfo _method;

        public string Fullname { get { return _fullname; } internal set { _fullname = value; } }
        public SystemType TargetType { get { return _targetType; } internal set { _targetType = value; } }
        public string StateName { get { return _stateName; } internal set { _stateName = value; } }
        public StateAccessType AccessType { get { return _accessType; } internal set { _accessType = value; } }
        public AspectBehaviourType BehaviourType { get { return _behaviourType; } internal set { _behaviourType = value; } }
        public SystemMethodInfo Method { get { return _method; } internal set { _method = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("Fullname = {0}, ", _fullname);
            sb.AppendFormat("TargetType = {0}, ", NovaEngine.Utility.Text.ToString(_targetType));
            sb.AppendFormat("StateName = {0}, ", _stateName);
            sb.AppendFormat("AccessType = {0}, ", _accessType.ToString());
            sb.AppendFormat("BehaviourType = {0}, ", _behaviourType.ToString());
            sb.AppendFormat("Method = {0}, ", NovaEngine.Utility.Text.ToString(_method));
            sb.Append("}");
            return sb.ToString();
        }
    }
}
