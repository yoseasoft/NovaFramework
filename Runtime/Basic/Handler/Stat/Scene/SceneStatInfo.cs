/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

using SystemDateTime = System.DateTime;

namespace GameEngine
{
    /// <summary>
    /// 场景模块统计项对象类，对场景模块访问记录进行单项统计的数据单元
    /// </summary>
    public sealed class SceneStatInfo : IStatInfo
    {
        /// <summary>
        /// 场景记录索引标识
        /// </summary>
        private readonly int m_uid;
        /// <summary>
        /// 场景名称
        /// </summary>
        private readonly string m_sceneName;
        /// <summary>
        /// 场景的哈希码，用来确保场景唯一性
        /// </summary>
        private readonly int m_hashCode;
        /// <summary>
        /// 场景的进入时间
        /// </summary>
        private SystemDateTime m_enterTime;
        /// <summary>
        /// 场景的退出时间
        /// </summary>
        private SystemDateTime m_exitTime;

        public SceneStatInfo(int uid, string sceneName, int hashCode)
        {
            m_uid = uid;
            m_sceneName = sceneName;
            m_hashCode = hashCode;
            m_enterTime = SystemDateTime.MinValue;
            m_exitTime = SystemDateTime.MinValue;

        }

        public int Uid
        {
            get { return m_uid; }
        }

        public string SceneName
        {
            get { return m_sceneName; }
        }

        public int HashCode
        {
            get { return m_hashCode; }
        }

        public SystemDateTime EnterTime
        {
            get { return m_enterTime; }
            set { m_enterTime = value; }
        }

        public SystemDateTime ExitTime
        {
            get { return m_exitTime; }
            set { m_exitTime = value; }
        }
    }
}
