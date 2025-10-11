/// -------------------------------------------------------------------------------
/// NovaEngine Framework
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

namespace NovaEngine
{
    /// <summary>
    /// 引擎工作台总控类，用于管理子任务工作流的调度分配
    /// 您可以通过 <see cref="IExecutable"/> 接口来定义子任务，并通过管理器或其它方式来调度执行
    /// </summary>
    internal static class Workbench
    {
        /// <summary>
        /// 工作台运行状态
        /// </summary>
        private static bool _isRunning;

        /// <summary>
        /// 工作台运行帧率
        /// </summary>
        private static int _frameRate;
        /// <summary>
        /// 工作台运行帧间隔时间（以秒为单位）
        /// </summary>
        private static float _frameInterval;

        /// <summary>
        /// 工作区对象实例管理容器
        /// </summary>
        private static IList<Subwork> _subworks;
        /// <summary>
        /// 工作区对象实例数量
        /// </summary>
        private static int _subworkCount;

        /// <summary>
        /// 工作区对象实例执行状态
        /// </summary>
        private static bool _isSubworkExecuting;
        /// <summary>
        /// 当前正在执行的工作区对象实例名称
        /// </summary>
        private static string _currentExecutingSubworkName;
        /// <summary>
        /// 当前正在执行的工作区对象实例标识
        /// </summary>
        private static int _currentExecutingSubworkId;

        /// <summary>
        /// 当前正在执行的工作区对象实例帧数
        /// </summary>
        private static int _currentSubworkFrameCount;
        /// <summary>
        /// 当前正在执行工作区对象实例的帧间隔时间
        /// </summary>
        private static float _currentSubworkDeltaTime;

        public static bool IsSubworkExecuting => _isSubworkExecuting;
        public static string CurrentExecutingSubworkName => _currentExecutingSubworkName;
        public static int CurrentExecutingSubworkId => _currentExecutingSubworkId;
        public static int CurrentSubworkFrameCount => _currentSubworkFrameCount;
        public static float CurrentSubworkDeltaTime => _currentSubworkDeltaTime;

        /// <summary>
        /// 工作台启动接口函数
        /// </summary>
        public static void Startup()
        {
            if (Environment.frameRate <= 0)
            {
                // 如果当前引擎没有配置逻辑帧率，则不启动工作台进行逻辑调度
                return;
            }

            _frameRate = Environment.frameRate;
            _frameInterval = 1f / (float) _frameRate;

            _subworks = new List<Subwork>();
            _subworkCount = 0;

            // 添加默认工作区对象实例
            AddSubwork(null);

            _isRunning = true;
            _isSubworkExecuting = false;
            _currentExecutingSubworkName = null;
            _currentExecutingSubworkId = 0;
        }

        /// <summary>
        /// 工作台关闭接口函数
        /// </summary>
        public static void Shutdown()
        {
            _isRunning = false;
            _isSubworkExecuting = false;
            _currentExecutingSubworkName = null;
            _currentExecutingSubworkId = 0;

            _frameRate = 0;
            _frameInterval = 0f;

            _subworks.Clear();
            _subworks = null;
            _subworkCount = 0;
        }

        /// <summary>
        /// 工作台分时检测接口函数
        /// </summary>
        public static bool OnWorkTimingAlarmClock()
        {
            if (!_isRunning)
            {
                return false;
            }

            float dt = Timestamp.DeltaTime;
            Subwork subwork = null;
            for (int n = 0; n < _subworkCount; ++n)
            {
                if (_subworks[n].Tick(dt))
                {
                    subwork ??= _subworks[n];
                }
            }

            if (null != subwork)
            {
                _currentExecutingSubworkName = subwork.Name;
                _currentExecutingSubworkId = subwork.Uid;

                _currentSubworkFrameCount = subwork.FrameCount;
                _currentSubworkDeltaTime = subwork.DeltaTime;

                return true;
            }

            return false;
        }

        /// <summary>
        /// 工作台任务完成接口函数
        /// </summary>
        public static void OnWorkTimingCompleted()
        {
            if (!_isRunning)
            {
                return;
            }

            Logger.Assert(!_currentExecutingSubworkName.IsNullOrEmpty(), "Invalid arguments.");

            Subwork subwork = GetSubwork(_currentExecutingSubworkName);
            if (null == subwork)
            {
                Logger.Warn("Could not found any subwork instance by name '{%s}', on completed it failed.", _currentExecutingSubworkName);
                return;
            }

            subwork.Completed();

            _isSubworkExecuting = false;
            _currentExecutingSubworkName = null;
            _currentExecutingSubworkId = 0;
            _currentSubworkFrameCount = 0;
            _currentSubworkDeltaTime = 0f;
        }

        /// <summary>
        /// 工作台任务开始接口函数
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void OnWorkTimingStart()
        {
            _isSubworkExecuting = true;
        }

        /// <summary>
        /// 工作台任务停止接口函数
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void OnWorkTimingStop()
        {
            _isSubworkExecuting = false;
        }

        /// <summary>
        /// 添加指定名称的子工作流到当前的调度队列中
        /// </summary>
        /// <param name="name">名称</param>
        public static void AddSubwork(string name)
        {
            bool isMain = name.IsNullOrEmpty();

            for (int n = 0; n < _subworkCount; ++n)
            {
                Subwork tmp = _subworks[n];
                if ((isMain && tmp.IsMain) || (!isMain && tmp.Name == name))
                {
                    Logger.Error("The subwork '{%s}' was already exist, repeat added it failed.", name);
                    return;
                }
            }

            Subwork subwork = new Subwork(name);
            _subworks.Add(subwork);
            _subworkCount = _subworks.Count;
        }

        /// <summary>
        /// 从当前的调度队列中移除指定名称的子工作流
        /// </summary>
        /// <param name="name">名称</param>
        public static void RemoveSubwork(string name)
        {
            string s = name ?? Definition.CString.Default;

            for (int n = 0; n < _subworkCount; ++n)
            {
                if (_subworks[n].Name == s)
                {
                    _subworks.RemoveAt(n);
                    _subworkCount = _subworks.Count;
                    return;
                }
            }

            Logger.Error("Could not found any subwork with target name '{%s}', removed it failed.", s);
        }

        /// <summary>
        /// 通过名称获取对应的子工作流对象
        /// </summary>
        /// <param name="name">工作流名称</param>
        /// <returns>返回子工作流对象实例</returns>
        private static Subwork GetSubwork(string name)
        {
            for (int n = 0; n < _subworkCount; ++n)
            {
                if (_subworks[n].Name == name)
                {
                    return _subworks[n];
                }
            }

            return null;
        }

        /// <summary>
        /// 通过唯一标识获取对应的子工作流对象
        /// </summary>
        /// <param name="uid">工作流标识</param>
        /// <returns>返回子工作流对象实例</returns>
        private static Subwork GetSubwork(int uid)
        {
            for (int n = 0; n < _subworkCount; ++n)
            {
                if (_subworks[n].Uid == uid)
                {
                    return _subworks[n];
                }
            }

            return null;
        }

        /// <summary>
        /// 子工作任务对象类
        /// </summary>
        private sealed class Subwork
        {
            /// <summary>
            /// 工作区名称
            /// </summary>
            private readonly string _name;
            /// <summary>
            /// 工作区唯一标识
            /// </summary>
            private readonly int _uid;
            /// <summary>
            /// 工作区主实例状态标识
            /// </summary>
            private readonly bool _isMain;
            /// <summary>
            /// 工作区帧计数器
            /// </summary>
            private int _frameCount;
            /// <summary>
            /// 工作区帧间隔时间
            /// </summary>
            private float _deltaTime;

            /// <summary>
            /// 工作区调度就绪状态标识
            /// </summary>
            private bool _isReady;

            public string Name => _name;
            public int Uid => _uid;
            public bool IsMain => _isMain;
            public int FrameCount => _frameCount;
            public float DeltaTime => _deltaTime;
            public bool IsReady => _isReady;

            public Subwork(string name)
            {
                if (name.IsNullOrEmpty())
                {
                    _name = Definition.CString.Default;
                    _uid = 0;
                    _isMain = true;
                }
                else
                {
                    _name = name;
                    _uid = Session.NextSession(Utility.Text.GetFullName(GetType()));
                    _isMain = false;
                }

                _frameCount = 0;
                _deltaTime = 0f;
                _isReady = false;
            }

            public bool Tick(float dt)
            {
                // 当前帧的时间间隔
                _deltaTime += dt;

                if (_deltaTime >= _frameInterval)
                {
                    // 帧累加
                    _frameCount++;

                    _isReady = true;
                }

                return _isReady;
            }

            public void Completed()
            {
                _deltaTime = 0f;

                _isReady = false;
            }
        }
    }
}
