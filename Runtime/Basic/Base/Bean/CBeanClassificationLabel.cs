/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
/// Copyright (C) 2026, Hurley, Independent Studio.
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

namespace GameEngine
{
    /// <summary>
    /// 实体对象的分类标签枚举定义<br/>
    /// 此类型主要用于快速进行类型判断，避免使用类似<see cref="System.Type.IsAssignableFrom(System.Type)"/>之类的方式导致的性能消耗
    /// </summary>
    public enum CBeanClassificationLabel : byte
    {
        Unknown = 0,

        /// <summary>
        /// 通用对象类型
        /// </summary>
        Object,
        /// <summary>
        /// 场景对象类型
        /// </summary>
        Scene,
        /// <summary>
        /// 角色对象类型
        /// </summary>
        Actor,
        /// <summary>
        /// 视图对象类型
        /// </summary>
        View,
        /// <summary>
        /// 组件对象类型
        /// </summary>
        Component,
    }
}
