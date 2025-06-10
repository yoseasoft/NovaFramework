/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
    /// 基础管理句柄的模块管理部分，对外提供全部模块组件对象的统一访问接口
    /// </summary>
    public partial class Facade
    {
        /// <summary>
        /// 引擎运行期间的时间戳记录封装类，用于提供运行期间所有时间统计相关数据的访问接口
        /// </summary>
        public static class Timestamp
        {
            /// <summary>
            /// 从程序开始后所运行的时间，会受时间缩放比例的影响
            /// </summary>
            private static float s_time;

            /// <summary>
            /// 从程序开始后所运行的时间（以毫秒为单位），会受时间缩放比例的影响
            /// </summary>
            private static int s_timeOfMilliseconds;

            /// <summary>
            /// 从程序开始后所运行的时间，不受时间缩放比例的影响
            /// </summary>
            private static float s_unscaledTime;

            /// <summary>
            /// 从程序开始后所运行的时间（以毫秒为单位），不受时间缩放比例的影响
            /// </summary>
            private static int s_unscaledTimeOfMilliseconds;

            /// <summary>
            /// 从程序开始后所运行的时间（由fixedDeltaTime累加），会受时间缩放比例的影响
            /// </summary>
            private static float s_fixedTime;

            /// <summary>
            /// 从程序开始后所运行的时间（由fixedDeltaTime累加），不受时间缩放比例的影响
            /// </summary>
            private static float s_fixedUnscaledTime;

            /// <summary>
            /// 从游戏开始后所运行的真实时间，不受时间缩放比例的影响
            /// 它与‘time’不同在于‘time’会从第一帧开始计算，而‘realtimeSinceStartup’是启动程序就开始计算
            /// </summary>
            private static float s_realtimeSinceStartup;

            /// <summary>
            /// 表示从上一帧到当前帧的时间，也就是时间增量，以秒为单位，它会受到时间缩放影响
            /// 在“FixedUpdate”中‘deltaTime’就是‘fixedDeltaTime’，而“Update”和“LateUpdate”中的时间增量是不固定的，由帧率决定
            /// </summary>
            private static float s_deltaTime;

            /// <summary>
            /// 表示从上一帧到当前帧的时间，也就是时间增量，以毫秒为单位，它会受到时间缩放影响
            /// </summary>
            private static int s_deltaTimeOfMilliseconds;

            /// <summary>
            /// 表示从上一帧到当前帧的真实时间，不受时间缩放比例的影响
            /// </summary>
            private static float s_unscaledDeltaTime;

            /// <summary>
            /// 表示从上一帧到当前帧的真实时间，以毫秒为单位，不受时间缩放比例的影响
            /// </summary>
            private static int s_unscaledDeltaTimeOfMilliseconds;

            /// <summary>
            /// 这是一个固定的时间增量，以秒为单位，不受时间缩放比例的影响
            /// 在“Edit->ProjectSettings->Time”中‘Fixed Timestamp’设置的值就是‘fixedDeltaTime’
            /// </summary>
            private static float s_fixedDeltaTime;

            /// <summary>
            /// 总帧数，不受时间缩放比例的影响
            /// </summary>
            private static int s_frameCount;

            /// <summary>
            /// 计算当前帧的增量时间后的剩余时间（以毫秒为单位），它会受到时间缩放影响
            /// </summary>
            private static float s_leftoverTimeOfMilliseconds;

            /// <summary>
            /// 计算当前帧的增量时间后的剩余时间（以毫秒为单位），不受时间缩放比例的影响
            /// </summary>
            private static float s_leftoverUnscaledTimeOfMilliseconds;

            /// <summary>
            /// 在“Update”调度时刷新时间记录
            /// </summary>
            internal static void RefreshTimeOnUpdate()
            {
                s_time = UnityTime.time;
                s_unscaledTime = UnityTime.unscaledTime;
                s_fixedTime = UnityTime.fixedTime;
                s_fixedUnscaledTime = UnityTime.fixedUnscaledTime;
                s_realtimeSinceStartup = UnityTime.realtimeSinceStartup;
                s_deltaTime = UnityTime.deltaTime;
                s_unscaledDeltaTime = UnityTime.unscaledDeltaTime;
                s_fixedDeltaTime = UnityTime.fixedDeltaTime;
                s_frameCount = UnityTime.frameCount;

                float ms_time = s_deltaTime * Definition.Measure.SecondsToMilliseconds + s_leftoverTimeOfMilliseconds;
                int ums = (int) ms_time;

                s_leftoverTimeOfMilliseconds = ms_time - (float) ums;
                s_timeOfMilliseconds = s_timeOfMilliseconds + ums;
                s_deltaTimeOfMilliseconds = ums;

                ms_time = s_unscaledDeltaTime * Definition.Measure.SecondsToMilliseconds + s_leftoverUnscaledTimeOfMilliseconds;
                ums = (int) ms_time;

                s_leftoverUnscaledTimeOfMilliseconds = ms_time - (float) ums;
                s_unscaledTimeOfMilliseconds = s_unscaledTimeOfMilliseconds + ums;
                s_unscaledDeltaTimeOfMilliseconds = ums;
            }

            public static float Time => s_time;

            public static int TimeOfMilliseconds => s_timeOfMilliseconds;

            public static float UnscaledTime => s_unscaledTime;

            public static int UnscaledTimeOfMilliseconds => s_unscaledTimeOfMilliseconds;

            public static float FixedTime => s_fixedTime;

            public static float FixedUnscaledTime => s_fixedUnscaledTime;

            public static float RealtimeSinceStartup => s_realtimeSinceStartup;

            public static float DeltaTime => s_deltaTime;

            public static int DeltaTimeOfMilliseconds => s_deltaTimeOfMilliseconds;

            public static float UnscaledDeltaTime => s_unscaledDeltaTime;

            public static int UnscaledDeltaTimeOfMilliseconds => s_unscaledDeltaTimeOfMilliseconds;

            public static float FixedDeltaTime => s_fixedDeltaTime;

            public static int FrameCount => s_frameCount;
        }
    }
}
