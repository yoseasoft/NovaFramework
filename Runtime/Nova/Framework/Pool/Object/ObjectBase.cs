/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

using SystemDateTime = System.DateTime;

namespace NovaEngine.ObjectPool
{
    /// <summary>
    /// 应用于对象池管理的对象抽象父类，声明一个提供给对象池管理的标准对象类<br/>
    /// 该类可以认为是特例对象池管理的特例对象类，它定义了一个标准的池化对象所需的一些属性及访问接口<br/>
    /// 当您需要创建一个需要通过对象池进行管理的基础对象类时，建议您以该类的子类的方式去实现它
    /// </summary>
    public abstract class ObjectBase : IReference
    {
        /// <summary>
        /// 对象的名称
        /// </summary>
        private string m_name;
        /// <summary>
        /// 对象的引用实例
        /// </summary>
        private object m_target;
        /// <summary>
        /// 对象加锁的状态标识
        /// </summary>
        private bool m_locked;
        /// <summary>
        /// 对象的优先级
        /// </summary>
        private int m_priority;
        /// <summary>
        /// 对象最后使用的时间
        /// </summary>
        private SystemDateTime m_lastUseTime;

        /// <summary>
        /// 获取对象的名称
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// 获取对象的引用实例
        /// </summary>
        public object Target
        {
            get { return m_target; }
        }

        /// <summary>
        /// 获取或设置对象的加锁状态
        /// </summary>
        public bool Locked
        {
            get { return m_locked; }
            set { m_locked = value; }
        }

        /// <summary>
        /// 获取或设置对象的优先级
        /// </summary>
        public int Priority
        {
            get { return m_priority; }
            set { m_priority = value; }
        }

        /// <summary>
        /// 获取或设置对象最后使用的时间
        /// </summary>
        public SystemDateTime LastUseTime
        {
            get { return m_lastUseTime; }
            set { m_lastUseTime = value; }
        }

        /// <summary>
        /// 获取自定义的对象可释放检查标记
        /// </summary>
        public virtual bool Releasabled
        {
            get { return true; }
        }

        public ObjectBase()
        {
            m_name = null;
            m_target = null;
            m_locked = false;
            m_priority = 0;
            m_lastUseTime = default(SystemDateTime);
        }

        /// <summary>
        /// 特例对象的默认初始化回调函数
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// 特例对象的初始化回调函数
        /// </summary>
        /// <param name="target">对象的引用实例</param>
        protected void Initialize(object target)
        {
            Initialize(null, target, false, 0);
        }

        /// <summary>
        /// 特例对象的初始化回调函数
        /// </summary>
        /// <param name="name">对象名称</param>
        /// <param name="target">对象引用实例</param>
        protected void Initialize(string name, object target)
        {
            Initialize(name, target, false, 0);
        }

        /// <summary>
        /// 特例对象的初始化回调函数
        /// </summary>
        /// <param name="name">对象名称</param>
        /// <param name="target">对象引用实例</param>
        /// <param name="locked">锁定状态</param>
        protected void Initialize(string name, object target, bool locked)
        {
            Initialize(name, target, locked, 0);
        }

        /// <summary>
        /// 特例对象的初始化回调函数
        /// </summary>
        /// <param name="name">对象名称</param>
        /// <param name="target">对象引用实例</param>
        /// <param name="priority">优先级</param>
        protected void Initialize(string name, object target, int priority)
        {
            Initialize(name, target, false, priority);
        }

        /// <summary>
        /// 特例对象的初始化回调函数
        /// </summary>
        /// <param name="name">对象名称</param>
        /// <param name="target">对象引用实例</param>
        /// <param name="locked">锁定状态</param>
        /// <param name="priority">优先级</param>
        protected void Initialize(string name, object target, bool locked, int priority)
        {
            if (null == target)
            {
                throw new CFrameworkException("Target '{0}' is invalid.", name);
            }

            m_name = name ?? string.Empty;
            m_target = target;
            m_locked = locked;
            m_priority = priority;
            m_lastUseTime = SystemDateTime.UtcNow;
        }

        /// <summary>
        /// 特例对象的默认清理回调函数
        /// </summary>
        public virtual void Cleanup()
        {
            m_name = null;
            m_target = null;
            m_locked = false;
            m_priority = 0;
            m_lastUseTime = default(SystemDateTime);
        }

        /// <summary>
        /// 获取对象时发生的事件
        /// </summary>
        protected internal virtual void OnSpawn()
        {
        }

        /// <summary>
        /// 回收对象时发生的事件
        /// </summary>
        protected internal virtual void OnUnspawn()
        {
        }

        /// <summary>
        /// 释放当前的特例对象
        /// </summary>
        /// <param name="shutdown">是否是关闭对象池时触发的状态标识</param>
        protected internal abstract void Release(bool shutdown);
    }
}
