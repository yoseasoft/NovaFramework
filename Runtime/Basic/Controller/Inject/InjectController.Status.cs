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

namespace GameEngine
{
    /// <summary>
    /// 反射注入接口的控制器类，对整个程序所有反射注入函数进行统一的整合和管理
    /// </summary>
    internal partial class InjectController
    {
        /// <summary>
        /// 对象激活状态管理容器
        /// </summary>
        private IDictionary<SystemType, AspectBehaviourType> _objectActivationStatus = null;

        /// <summary>
        /// 对象注入状态标识的初始化回调函数
        /// </summary>
        [OnControllerSubmoduleInitCallback]
        private void InitInjectObjectStatuss()
        {
            // 对象激活状态管理容器初始化
            _objectActivationStatus = new Dictionary<SystemType, AspectBehaviourType>();
        }

        /// <summary>
        /// 对象注入状态标识的清理回调函数
        /// </summary>
        [OnControllerSubmoduleCleanupCallback]
        private void CleanupInjectObjectStatus()
        {
            // 移除全部对象的激活状态信息
            RemoveAllObjectActivationStatuses();

            _objectActivationStatus = null;
        }

        #region 对象类注入状态标识激活管理接口函数

        /// <summary>
        /// 设置指定对象类型的激活状态信息
        /// </summary>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="behaviourType">行为类型</param>
        private void SetObjectActivationStatus(SystemType targetType, AspectBehaviourType behaviourType)
        {
            if (_objectActivationStatus.ContainsKey(targetType))
            {
                Debugger.Warn("The target object status '{0}' was already exist, repeat added it will be override old value.", NovaEngine.Utility.Text.ToString(targetType));

                _objectActivationStatus.Remove(targetType);
            }

            _objectActivationStatus.Add(targetType, behaviourType);
        }

        /// <summary>
        /// 通过指定的对象类型获取其行为标识
        /// </summary>
        /// <param name="targetType">目标对象类型</param>
        /// <returns>返回对象的行为标识，若该对象为注册则返回默认行为标识</returns>
        public AspectBehaviourType GetObjectActivationBehaviourByType(SystemType targetType)
        {
            if (_objectActivationStatus.TryGetValue(targetType, out AspectBehaviourType result))
            {
                return result;
            }

            return AspectBehaviourType.Unknown;
        }

        /// <summary>
        /// 移除指定对象类型的激活状态信息
        /// </summary>
        /// <param name="targetType">目标对象类型</param>
        private void RemoveObjectActivationStatus(SystemType targetType)
        {
            if (false == _objectActivationStatus.ContainsKey(targetType))
            {
                Debugger.Warn("Could not found any activation status with target type '{0}', removed it failed.", NovaEngine.Utility.Text.ToString(targetType));
                return;
            }

            _objectActivationStatus.Remove(targetType);
        }

        /// <summary>
        /// 移除全部对象的激活状态信息
        /// </summary>
        private void RemoveAllObjectActivationStatuses()
        {
            _objectActivationStatus.Clear();
        }

        #endregion
    }
}
