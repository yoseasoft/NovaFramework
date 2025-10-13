/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using System.Collections.Generic;

using SystemEnum = System.Enum;

namespace GameEngine
{
    /// <summary>
    /// 切面访问接口的控制器类，对整个程序所有切面访问函数进行统一的整合和管理
    /// </summary>
    internal sealed partial class AspectController
    {
        /// <summary>
        /// 切面行为的名称与类型关联映射容器
        /// </summary>
        private IDictionary<string, AspectBehaviourType> _behaviourNameTypes = null;

        /// <summary>
        /// 切面行为相关内容的初始化回调函数
        /// </summary>
        [OnControllerSubmoduleInitCallback]
        private void InitializeForAspectBehaviour()
        {
            // 数据容器初始化
            _behaviourNameTypes = new Dictionary<string, AspectBehaviourType>();

            _InitBehaviourNameTypes();
        }

        /// <summary>
        /// 切面行为相关内容的清理回调函数
        /// </summary>
        [OnControllerSubmoduleCleanupCallback]
        private void CleanupForAspectBehaviour()
        {
            // 移除数据容器
            _behaviourNameTypes.Clear();
            _behaviourNameTypes = null;
        }

        /// <summary>
        /// 初始化行为类型的名称与值映射数据
        /// </summary>
        private void _InitBehaviourNameTypes()
        {
            foreach (AspectBehaviourType aspectBehaviourType in SystemEnum.GetValues(typeof(AspectBehaviourType)))
            {
                // if (AspectBehaviourType.Unknown == aspectBehaviourType)
                if ((int) AspectBehaviourType.Unknown == ((int) aspectBehaviourType & 0xff))
                {
                    // 未知类型直接忽略
                    // 2025-10-13：
                    // 将边界定义的类型也一起包含进来，所有边界类型的低8位都为0
                    continue;
                }

                // 通过枚举类型获取其对应的名称
                string aspectBehaviourName = SystemEnum.GetName(typeof(AspectBehaviourType), aspectBehaviourType);

                Debugger.Assert(false == _behaviourNameTypes.ContainsKey(aspectBehaviourName));

                _behaviourNameTypes.Add(aspectBehaviourName, aspectBehaviourType);
            }
        }

        /// <summary>
        /// 通过指定的行为名称，获取对应的行为类型定义
        /// </summary>
        /// <param name="name">行为名称</param>
        /// <returns>返回行为类型定义，若名称无效则返回Unknown值</returns>
        public AspectBehaviourType GetAspectBehaviourTypeByName(string name)
        {
            if (_behaviourNameTypes.TryGetValue(name, out AspectBehaviourType type))
            {
                return type;
            }

            return AspectBehaviourType.Unknown;
        }
    }
}
