/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2025, Hurley, Independent Studio.
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

namespace GameEngine
{
    /// <summary>
    /// 状态管理对象类，用于对场景上下文中的所有引用对象的状态进行集中管理及分发
    /// </summary>
    public sealed partial class FsmController : BaseController<FsmController>
    {
        /// <summary>
        /// 状态信息记录对象类，用于记录状态类的基础信息
        /// </summary>
        private sealed class StateInfo
        {
            private string _name;
            private SystemType _classType;
            private int _priority;
            private bool _isInstanceType;
            private bool _isStaticType;

            internal StateInfo(string name, SystemType classType) : this(name, 0, false, classType)
            { }

            internal StateInfo(string name, int priority, SystemType classType) : this(name, priority, false, classType)
            { }

            internal StateInfo(string name, bool isInstanceType, SystemType classType) : this(name, 0, isInstanceType, classType)
            { }

            internal StateInfo(string name, int priority, bool isInstanceType, SystemType classType)
            {
                _name = name;
                _priority = priority;
                _isInstanceType = isInstanceType;

                _classType = classType;

                _isStaticType = NovaEngine.Utility.Reflection.IsTypeOfStaticClass(classType);
            }
        }
    }
}
