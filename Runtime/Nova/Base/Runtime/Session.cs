/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyring (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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
    /// 会话管理工具类，对项目内部所有会话进行统一管理，该接口在系统中是线程安全的
    /// </summary>
    public sealed partial class Session
    {
        /// <summary>
        /// 初始会话值
        /// </summary>
        public const int SESSION_INIT_VALUE = 10000;

        private readonly object m_locked = new object();

        /// <summary>
        /// 初始会话值，用于进行会话值的重置
        /// </summary>
        private int m_initValue = 0;

        /// <summary>
        /// 最大会话值，用于进行会话值的边界检查
        /// </summary>
        private int m_maxValue = 0;

        /// <summary>
        /// 会话值，记录当前会话递增后的结果值
        /// </summary>
        private int m_currValue = 0;

        /// <summary>
        /// 会话对象构造函数
        /// </summary>
        private Session() { }

        /// <summary>
        /// 会话对象析构函数
        /// </summary>
        ~Session() { }

        /// <summary>
        /// 会话对象基于数值初始化函数接口
        /// </summary>
        /// <param name="init">初始会话值</param>
        /// <param name="max">最大会话值</param>
        private void Init(int init, int max)
        {
            m_initValue = init;
            m_maxValue  = max;
            m_currValue = init + 1;
        }

        /// <summary>
        /// 会话对象基于对象初始化函数接口
        /// </summary>
        /// <param name="session">会话对象</param>
        private void InitWithSession(Session session)
        {
            m_initValue = session.m_initValue;
            m_maxValue  = session.m_maxValue;
            m_currValue = session.m_currValue;
        }

        /// <summary>
        /// 通过给定的初始会话值创建一个会话对象实例
        /// </summary>
        /// <param name="init">初始会话值</param>
        /// <param name="max">最大会话值</param>
        /// <returns>若创建会话对象实例成功返回其引用，否则返回null</returns>
        public static Session Create(int init = SESSION_INIT_VALUE, int max = System.Int32.MaxValue)
        {
            if (init <= 0 || max <= 0)
            {
                Logger.Error("All session parameters must be greater than zero.");
                return null;
            }

            if (init >= max)
            {
                Logger.Error("The init value must be less than the max value.");
                return null;
            }

            Session ret = new Session();
            ret.Init(init, max);
            return ret;
        }

        /// <summary>
        /// 通过给定的初始会话对象创建一个会话对象实例
        /// </summary>
        /// <param name="session">会话对象</param>
        /// <returns>若创建会话对象实例成功返回其引用，否则返回null</returns>
        public static Session CreateWithSession(Session session)
        {
            Session ret = new Session();
            ret.InitWithSession(session);
            return ret;
        }

        /// <summary>
        /// 会话对象增量获取接口
        /// </summary>
        /// <returns>返回自增后的当前会话值标识</returns>
        public int Next()
        {
            return __CalcNextUnsafeSessionID(m_currValue, m_initValue, m_maxValue);
        }

        /// <summary>
        /// 重置当前会话对象
        /// </summary>
        public void Reset()
        {
            m_currValue = m_initValue;
        }
    }
}
