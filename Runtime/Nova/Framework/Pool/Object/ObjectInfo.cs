/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

using System.Runtime.InteropServices;

using SystemDateTime = System.DateTime;

namespace NovaEngine.ObjectPool
{
    /// <summary>
    /// 应用于对象池管理的对象基础信息结构类，它定义了一个标准的池化对象的描述信息所需的一些属性及访问接口
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct ObjectInfo
    {
        /// <summary>
        /// 对象的名称
        /// </summary>
        private readonly string _name;
        /// <summary>
        /// 对象是否被加锁的状态标识
        /// </summary>
        private readonly bool _locked;
        /// <summary>
        /// 对象自定义的可释放检查标记
        /// </summary>
        private readonly bool _releasabled;
        /// <summary>
        /// 对象的优先级
        /// </summary>
        private readonly int _priority;
        /// <summary>
        /// 对象上次使用的时间
        /// </summary>
        private readonly SystemDateTime _lastUseTime;
        /// <summary>
        /// 对象的孵化计数
        /// </summary>
        private readonly int _spawnCount;

        /// <summary>
        /// 获取对象的名称
        /// </summary>
        public string Name => _name;
        /// <summary>
        /// 获取对象的加锁状态
        /// </summary>
        public bool Locked => _locked;
        /// <summary>
        /// 获取对象自定义的可释放检查标记
        /// </summary>
        public bool Releasabled => _releasabled;
        /// <summary>
        /// 获取对象的优先级
        /// </summary>
        public int Priority => _priority;
        /// <summary>
        /// 获取对象的上次使用时间
        /// </summary>
        public SystemDateTime LastUseTime => _lastUseTime;
        /// <summary>
        /// 获取对象的孵化计数
        /// </summary>
        public int SpawnCount => _spawnCount;

        /// <summary>
        /// 获取对象当前是否正在使用的状态
        /// </summary>
        public bool IsOnUsed
        {
            get { return _spawnCount > 0; }
        }

        /// <summary>
        /// 构建对象信息的新实例
        /// </summary>
        /// <param name="name">对象名称</param>
        /// <param name="locked">对象是否被加锁</param>
        /// <param name="releasabled">对象可释放检查标记</param>
        /// <param name="priority">对象优先级</param>
        /// <param name="lastUseTime">对象上次使用时间</param>
        /// <param name="spawnCount">对象孵化计数</param>
        public ObjectInfo(string name, bool locked, bool releasabled, int priority, SystemDateTime lastUseTime, int spawnCount)
        {
            _name = name;
            _locked = locked;
            _releasabled = releasabled;
            _priority = priority;
            _lastUseTime = lastUseTime;
            _spawnCount = spawnCount;
        }
    }
}
