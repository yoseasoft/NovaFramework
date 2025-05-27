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

using System.Collections.Generic;

using SystemType = System.Type;

namespace NovaEngine.ObjectPool
{
    /// <summary>
    /// 对象池的基类，针对对象池的一些常规接口进行通用性的实现
    /// </summary>
    public abstract class ObjectPoolBase
    {
        /// <summary>
        /// 对象池的名称
        /// </summary>
        private readonly string m_name;

        /// <summary>
        /// 获取对象池的名称
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// 获取对象池的完整名称
        /// </summary>
        public string FullName
        {
            get { return new TypeNamePair(ObjectType, m_name).ToString(); }
        }

        /// <summary>
        /// 获取对象池中的对象类型
        /// </summary>
        public abstract SystemType ObjectType
        {
            get;
        }

        /// <summary>
        /// 获取对象池中对象的数量
        /// </summary>
        public abstract int Count
        {
            get;
        }

        /// <summary>
        /// 获取对象池中可被释放的对象的数量
        /// </summary>
        public abstract int CanReleaseCount
        {
            get;
        }

        /// <summary>
        /// 获取是否允许对象可被多次分配
        /// </summary>
        public abstract bool AllowMultiSpawn
        {
            get;
        }

        /// <summary>
        /// 获取或设置对象池自动释放内部对象的间隔秒数
        /// </summary>
        public abstract float AutoReleaseInterval
        {
            get; set;
        }

        /// <summary>
        /// 获取或设置对象池的容量
        /// </summary>
        public abstract int Capacity
        {
            get; set;
        }

        /// <summary>
        /// 获取或设置对象池中对象的过期时间
        /// </summary>
        public abstract float ExpireTime
        {
            get; set;
        }

        /// <summary>
        /// 获取或设置对象池的优先级
        /// </summary>
        public abstract int Priority
        {
            get; set;
        }

        /// <summary>
        /// 对象池基类的默认构造函数
        /// </summary>
        public ObjectPoolBase() : this(null)
        { }

        /// <summary>
        /// 对象池基类的自定义构造函数
        /// </summary>
        /// <param name="name">对象池名称</param>
        public ObjectPoolBase(string name)
        {
            m_name = name ?? string.Empty;
        }

        /// <summary>
        /// 刷新对象池
        /// </summary>
        /// <param name="elapseSeconds"></param>
        /// <param name="realElapseSeconds"></param>
        internal abstract void Update(float elapseSeconds, float realElapseSeconds);

        /// <summary>
        /// 关闭对象池
        /// </summary>
        internal abstract void Shutdown();

        /// <summary>
        /// 释放对象池中的所有可释放对象实例
        /// </summary>
        public abstract void Release();

        /// <summary>
        /// 释放对象池中指定数量的所有可释放对象实例<br/>
        /// 若剩余对象的数量不足，则释放全部对象实例
        /// </summary>
        /// <param name="releaseCount">尝试释放的对象数量</param>
        public abstract void Release(int releaseCount);

        /// <summary>
        /// 释放对象池中的所有未使用的对象实例
        /// </summary>
        public abstract void ReleaseAllUnused();

        /// <summary>
        /// 获取所有实例的对象信息集合
        /// </summary>
        /// <returns>返回所有实例的对象信息集合</returns>
        public abstract ObjectInfo[] GetAllObjectInfos();
    }
}
