/// -------------------------------------------------------------------------------
/// NovaEngine Framework
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

using UnityTime = UnityEngine.Time;

namespace NovaEngine
{
    /// <summary>
    /// 引擎运行期间的时间戳记录封装类，用于提供运行期间所有时间统计相关数据的访问接口
    /// </summary>
    public static class Timestamp
    {
        /// <summary>
        /// 从程序开始后所运行的时间，会受时间缩放比例的影响
        /// </summary>
        private static float _time;

        /// <summary>
        /// 从程序开始后所运行的时间（以毫秒为单位），会受时间缩放比例的影响
        /// </summary>
        private static int _timeOfMilliseconds;

        /// <summary>
        /// 从程序开始后所运行的时间，不受时间缩放比例的影响
        /// </summary>
        private static float _unscaledTime;

        /// <summary>
        /// 从程序开始后所运行的时间（以毫秒为单位），不受时间缩放比例的影响
        /// </summary>
        private static int _unscaledTimeOfMilliseconds;

        /// <summary>
        /// 从程序开始后所运行的时间（由fixedDeltaTime累加），会受时间缩放比例的影响
        /// </summary>
        private static float _fixedTime;

        /// <summary>
        /// 从程序开始后所运行的时间（由fixedDeltaTime累加），不受时间缩放比例的影响
        /// </summary>
        private static float _fixedUnscaledTime;

        /// <summary>
        /// 从游戏开始后所运行的真实时间，不受时间缩放比例的影响
        /// 它与‘time’不同在于‘time’会从第一帧开始计算，而‘realtimeSinceStartup’是启动程序就开始计算
        /// </summary>
        private static float _realtimeSinceStartup;

        /// <summary>
        /// 表示从上一帧到当前帧的时间，也就是时间增量，以秒为单位，它会受到时间缩放影响
        /// 在“FixedUpdate”中‘deltaTime’就是‘fixedDeltaTime’，而“Update”和“LateUpdate”中的时间增量是不固定的，由帧率决定
        /// </summary>
        private static float _deltaTime;

        /// <summary>
        /// 表示从上一帧到当前帧的时间，也就是时间增量，以毫秒为单位，它会受到时间缩放影响
        /// </summary>
        private static int _deltaTimeOfMilliseconds;

        /// <summary>
        /// 表示从上一帧到当前帧的真实时间，不受时间缩放比例的影响
        /// </summary>
        private static float _unscaledDeltaTime;

        /// <summary>
        /// 表示从上一帧到当前帧的真实时间，以毫秒为单位，不受时间缩放比例的影响
        /// </summary>
        private static int _unscaledDeltaTimeOfMilliseconds;

        /// <summary>
        /// 这是一个固定的时间增量，以秒为单位，不受时间缩放比例的影响
        /// 在“Edit->ProjectSettings->Time”中‘Fixed Timestamp’设置的值就是‘fixedDeltaTime’
        /// </summary>
        private static float _fixedDeltaTime;

        /// <summary>
        /// 总帧数，不受时间缩放比例的影响
        /// </summary>
        private static int _frameCount;

        /// <summary>
        /// 计算当前帧的增量时间后的剩余时间（以毫秒为单位），它会受到时间缩放影响
        /// </summary>
        private static float _leftoverTimeOfMilliseconds;

        /// <summary>
        /// 计算当前帧的增量时间后的剩余时间（以毫秒为单位），不受时间缩放比例的影响
        /// </summary>
        private static float _leftoverUnscaledTimeOfMilliseconds;

        /// <summary>
        /// 在“Update”调度时刷新时间记录
        /// </summary>
        internal static void RefreshTimeOnUpdate()
        {
            _time = UnityTime.time;
            _unscaledTime = UnityTime.unscaledTime;
            _fixedTime = UnityTime.fixedTime;
            _fixedUnscaledTime = UnityTime.fixedUnscaledTime;
            _realtimeSinceStartup = UnityTime.realtimeSinceStartup;
            _deltaTime = UnityTime.deltaTime;
            _unscaledDeltaTime = UnityTime.unscaledDeltaTime;
            _fixedDeltaTime = UnityTime.fixedDeltaTime;
            _frameCount = UnityTime.frameCount;

            float ms_time = _deltaTime * Definition.Measure.SecondsToMilliseconds + _leftoverTimeOfMilliseconds;
            int ums = (int) ms_time;

            _leftoverTimeOfMilliseconds = ms_time - (float) ums;
            _timeOfMilliseconds = _timeOfMilliseconds + ums;
            _deltaTimeOfMilliseconds = ums;

            ms_time = _unscaledDeltaTime * Definition.Measure.SecondsToMilliseconds + _leftoverUnscaledTimeOfMilliseconds;
            ums = (int) ms_time;

            _leftoverUnscaledTimeOfMilliseconds = ms_time - (float) ums;
            _unscaledTimeOfMilliseconds = _unscaledTimeOfMilliseconds + ums;
            _unscaledDeltaTimeOfMilliseconds = ums;
        }

        public static float Time => _time;

        public static int TimeOfMilliseconds => _timeOfMilliseconds;

        public static float UnscaledTime => _unscaledTime;

        public static int UnscaledTimeOfMilliseconds => _unscaledTimeOfMilliseconds;

        public static float FixedTime => _fixedTime;

        public static float FixedUnscaledTime => _fixedUnscaledTime;

        public static float RealtimeSinceStartup => _realtimeSinceStartup;

        public static float DeltaTime => _deltaTime;

        public static int DeltaTimeOfMilliseconds => _deltaTimeOfMilliseconds;

        public static float UnscaledDeltaTime => _unscaledDeltaTime;

        public static int UnscaledDeltaTimeOfMilliseconds => _unscaledDeltaTimeOfMilliseconds;

        public static float FixedDeltaTime => _fixedDeltaTime;

        public static int FrameCount => _frameCount;
    }
}
