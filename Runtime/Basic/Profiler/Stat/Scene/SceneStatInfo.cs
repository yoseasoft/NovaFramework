/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

using SystemDateTime = System.DateTime;

namespace GameEngine.Profiler.Statistics
{
    /// <summary>
    /// 场景模块统计项对象类，对场景模块访问记录进行单项统计的数据单元
    /// </summary>
    public sealed class SceneStatInfo : IStatInfo
    {
        /// <summary>
        /// 场景记录索引标识
        /// </summary>
        public int Uid { get; private set; }
        /// <summary>
        /// 场景名称
        /// </summary>
        public string SceneName { get; private set; }
        /// <summary>
        /// 场景的哈希码，用来确保场景唯一性
        /// </summary>
        public int HashCode { get; private set; }
        /// <summary>
        /// 场景的进入时间
        /// </summary>
        public SystemDateTime EnterTime { get; internal set; }
        /// <summary>
        /// 场景的退出时间
        /// </summary>
        public SystemDateTime ExitTime { get; internal set; }

        public SceneStatInfo(int uid, string sceneName, int hashCode)
        {
            this.Uid = uid;
            this.SceneName = sceneName;
            this.HashCode = hashCode;
            this.EnterTime = SystemDateTime.MinValue;
            this.ExitTime = SystemDateTime.MinValue;
        }
    }
}
