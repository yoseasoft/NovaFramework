/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace GameEngine.Debug
{
    /// <summary>
    /// 游戏调试器组件对象类，用于定义调试器对象的基础属性及访问操作函数
    /// </summary>
    public sealed partial class DebuggerComponent
    {
        /// <summary>
        /// 调试环境下的帧率计数器对象类，封装了当前环境上下文的帧率计数及访问接口函数
        /// </summary>
        public sealed class FpsCounter
        {
            private float _updateInterval;
            private float _currentFps;
            private int _frames;
            private float _accumulator;
            private float _timeLeft;

            /// <summary>
            /// 获取或设置当前帧率统计的刷新间隔值
            /// </summary>
            public float UpdateInterval
            {
                get { return _updateInterval; }
                set
                {
                    if (value <= 0)
                    {
                        Debugger.Error("Update interval is invalid.");
                        return;
                    }

                    _updateInterval = value;
                    Reset();
                }
            }

            /// <summary>
            /// 获取当前的帧率计数值
            /// </summary>
            public float CurrentFps
            {
                get { return _currentFps; }
            }

            public FpsCounter(float updateInterval)
            {
                if (updateInterval <= 0)
                {
                    Debugger.Error("Update interval is invalid.");
                    return;
                }

                _updateInterval = updateInterval;
                Reset();
            }

            public void Update(float elapseSeconds, float realElapseSeconds)
            {
                ++_frames;
                _accumulator += realElapseSeconds;
                _timeLeft -= realElapseSeconds;

                if (_timeLeft <= 0f)
                {
                    _currentFps = _accumulator > 0f ? _frames / _accumulator : 0f;
                    _frames = 0;
                    _accumulator = 0f;
                    _timeLeft += _updateInterval;
                }
            }

            /// <summary>
            /// 计数信息重置函数
            /// </summary>
            private void Reset()
            {
                _currentFps = 0f;
                _frames = 0;
                _accumulator = 0f;
                _timeLeft = 0f;
            }
        }
    }
}
