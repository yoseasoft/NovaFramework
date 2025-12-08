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

using System;
using System.Collections.Generic;

namespace NovaEngine.ObjectPool
{
    /// <summary>
    /// 对象池的抽象父类，声明一个简易的通用对象池接口类<br/>
    /// 该类可以认为是所有特例对象池的通用父类，它同时为所有池化对象提供了一些标准属性及管理方式<br/>
    /// 当您需要创建一个对象池类时，建议您以该类的子类的方式去实现它
    /// </summary>
    public interface IObjectPool<T> where T : ObjectBase
    {
        /// <summary>
        /// 获取对象池的名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取对象池的完整名称
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// 获取对象池中的对象类型
        /// </summary>
        Type ObjectType { get; }

        /// <summary>
        /// 获取对象池中对象的数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 获取对象池中可被释放的对象的数量
        /// </summary>
        int CanReleaseCount { get; }

        /// <summary>
        /// 获取是否允许对象可被多次分配
        /// </summary>
        bool AllowMultiSpawn { get; }

        /// <summary>
        /// 获取或设置对象池自动释放内部对象的间隔秒数
        /// </summary>
        float AutoReleaseInterval { get; set; }

        /// <summary>
        /// 获取或设置对象池的容量
        /// </summary>
        int Capacity { get; set; }

        /// <summary>
        /// 获取或设置对象池中对象的过期时间
        /// </summary>
        float ExpireTime { get; set; }

        /// <summary>
        /// 获取或设置对象池的优先级
        /// </summary>
        int Priority { get; set; }

        /// <summary>
        /// 注册一个用于孵化的对象实例
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <param name="spawned">对象是否已被孵化的标识</param>
        void Register(T obj, bool spawned);

        /// <summary>
        /// 检测当前对象池是否处于可孵化状态
        /// </summary>
        /// <returns>若当前对象池可孵化对象则返回true，否则返回false</returns>
        bool CanSpawn();

        /// <summary>
        /// 检测当前对象池是否可孵化指定名称的对象
        /// </summary>
        /// <param name="name">对象名称</param>
        /// <returns>若当前对象池可孵化对象则返回true，否则返回false</returns>
        bool CanSpawn(string name);

        /// <summary>
        /// 从当前对象池中孵化一个对象实例
        /// </summary>
        /// <returns>返回孵化的对象实例</returns>
        T Spawn();

        /// <summary>
        /// 从当前对象池中孵化一个指定名称的对象实例
        /// </summary>
        /// <param name="name">对象名称</param>
        /// <returns>返回孵化的对象实例</returns>
        T Spawn(string name);

        /// <summary>
        /// 回收一个指定的孵化对象实例
        /// </summary>
        /// <param name="obj">目标对象实例</param>
        void Unspawn(T obj);

        /// <summary>
        /// 回收一个指定的孵化对象实例
        /// </summary>
        /// <param name="target">目标对象实例</param>
        void Unspawn(object target);

        /// <summary>
        /// 设置一个指定的对象实例是否被锁定
        /// </summary>
        /// <param name="obj">目标对象实例</param>
        /// <param name="locked">锁定状态</param>
        void SetLocked(T obj, bool locked);

        /// <summary>
        /// 设置一个指定的对象实例是否被加锁
        /// </summary>
        /// <param name="target">目标对象实例</param>
        /// <param name="locked">锁定状态</param>
        void SetLocked(object target, bool locked);

        /// <summary>
        /// 设置一个指定的对象实例的优先级
        /// </summary>
        /// <param name="obj">目标对象实例</param>
        /// <param name="priority">优先级</param>
        void SetPriority(T obj, int priority);

        /// <summary>
        /// 设置一个指定的对象实例的优先级
        /// </summary>
        /// <param name="target">目标对象实例</param>
        /// <param name="priority">优先级</param>
        void SetPriority(object target, int priority);

        /// <summary>
        /// 释放指定的对象实例
        /// </summary>
        /// <param name="obj">目标对象实例</param>
        /// <returns>释放对象实例成功则返回true，否则返回false</returns>
        bool ReleaseObject(T obj);

        /// <summary>
        /// 释放指定的对象实例
        /// </summary>
        /// <param name="target">目标对象实例</param>
        /// <returns>释放对象实例成功则返回true，否则返回false</returns>
        bool ReleaseObject(object target);

        /// <summary>
        /// 释放对象池中的所有可释放对象实例
        /// </summary>
        void Release();

        /// <summary>
        /// 释放对象池中指定数量的所有可释放对象实例<br/>
        /// 若剩余对象的数量不足，则释放全部对象实例
        /// </summary>
        /// <param name="releaseCount">尝试释放的对象数量</param>
        void Release(int releaseCount);

        /// <summary>
        /// 根据指定筛选规则释放对象池中的所有可释放对象实例
        /// </summary>
        /// <param name="callback">释放对象筛选函数</param>
        void Release(ReleaseObjectFilterCallback<T> callback);

        /// <summary>
        /// 根据指定筛选规则释放对象池中指定数量的所有可释放对象实例
        /// </summary>
        /// <param name="releaseCount">尝试释放的对象数量</param>
        /// <param name="callback">释放对象筛选函数</param>
        void Release(int releaseCount, ReleaseObjectFilterCallback<T> callback);

        /// <summary>
        /// 释放对象池中的所有未使用的对象实例
        /// </summary>
        void ReleaseAllUnused();
    }
}
