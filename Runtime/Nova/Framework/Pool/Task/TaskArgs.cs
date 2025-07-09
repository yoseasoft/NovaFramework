/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

namespace NovaEngine
{
    /// <summary>
    /// 任务参数基类定义
    /// </summary>
    internal abstract class TaskArgs : IReference
    {
        /// <summary>
        /// 任务默认优先级设定
        /// </summary>
        public const int DEFAULT_PRIORITY = 0;

        private int _serialID;
        private int _priority;
        private bool _isDone;

        /// <summary>
        /// 任务参数对象的新实例构建接口
        /// </summary>
        public TaskArgs()
        {
            _serialID = 0;
            _priority = DEFAULT_PRIORITY;
            _isDone = false;
        }

        /// <summary>
        /// 获取任务的序列编号
        /// </summary>
        public int SerialID
        {
            get { return _serialID; }
        }

        /// <summary>
        /// 获取任务的优先级
        /// </summary>
        public int Priority
        {
            get { return _priority; }
        }

        /// <summary>
        /// 获取或设置任务的结束标识
        /// </summary>
        public bool Done
        {
            get { return _isDone; }
            set { _isDone = value; }
        }

        /// <summary>
        /// 获取任务描述
        /// </summary>
        public virtual string Description
        {
            get { return null; }
        }

        /// <summary>
        /// 任务参数对象初始化接口
        /// </summary>
        public void Initialize()
        {
            Initialize(0, DEFAULT_PRIORITY);
        }

        /// <summary>
        /// 任务参数对象初始化接口
        /// </summary>
        /// <param name="serialID">任务的序列编号</param>
        /// <param name="priority">任务的优先级</param>
        public void Initialize(int serialID, int priority)
        {
            _serialID = serialID;
            _priority = priority;
            _isDone = false;
        }

        /// <summary>
        /// 任务参数对象清理接口
        /// </summary>
        public void Cleanup()
        {
            _serialID = 0;
            _priority = DEFAULT_PRIORITY;
            _isDone = false;
        }
    }
}
