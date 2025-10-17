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

using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;

namespace GameEngine
{
    /// <summary>
    /// 场景实现类声明属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DeclareSceneClassAttribute : DeclareEntityClassAttribute
    {
        public DeclareSceneClassAttribute(string sceneName) : this(sceneName, 0)
        { }

        public DeclareSceneClassAttribute(int priority) : this(string.Empty, priority)
        { }

        public DeclareSceneClassAttribute(string sceneName, int priority) : base(sceneName, priority)
        {
        }
    }

    /// <summary>
    /// 场景中自动打开的目标视图的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class SceneAutoDisplayOnTargetViewAttribute : SystemAttribute
    {
        /// <summary>
        /// 场景名称标识
        /// </summary>
        private readonly string _viewName;

        /// <summary>
        /// 场景名称获取函数
        /// </summary>
        public string ViewName => _viewName;

        public SceneAutoDisplayOnTargetViewAttribute(string viewName) : base()
        {
            _viewName = viewName;
        }
    }
}
