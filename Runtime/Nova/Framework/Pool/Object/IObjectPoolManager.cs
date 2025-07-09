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

using System.Collections.Generic;

using SystemType = System.Type;

namespace NovaEngine.ObjectPool
{
    /// <summary>
    /// 对象池的管理器抽象父类，声明一个用于接管所有对象池实例的管理接口类<br/>
    /// 该类定义了针对所有对象池访问操作的所有接口函数，包括查找，获取，新增，移除等控制流程<br/>
    /// 当您需要创建一个对象池管理器类时，建议您以该类的子类的方式去实现它
    /// </summary>
    public interface IObjectPoolManager
    {
        /// <summary>
        /// 获取对象池的数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 检查是否存在指定类型的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>若存在给定类型的对象池则返回true，否则返回false</returns>
        bool HasObjectPool<T>() where T : ObjectBase;

        /// <summary>
        /// 检查是否存在指定类型的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>若存在给定类型的对象池则返回true，否则返回false</returns>
        bool HasObjectPool(SystemType objectType);

        /// <summary>
        /// 检查是否存在指定类型和名称的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <returns>若存在给定类型和名称的对象池则返回true，否则返回false</returns>
        bool HasObjectPool<T>(string name) where T : ObjectBase;

        /// <summary>
        /// 检查是否存在指定类型和名称的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <returns>若存在给定类型和名称的对象池则返回true，否则返回false</returns>
        bool HasObjectPool(SystemType objectType, string name);

        /// <summary>
        /// 检查是否存在匹配指定条件的对象池
        /// </summary>
        /// <param name="condition">要检查的条件</param>
        /// <returns>若存在匹配给定条件的对象池则返回true，否则返回false</returns>
        bool HasObjectPool(System.Predicate<ObjectPoolBase> condition);

        /// <summary>
        /// 获取指定类型对应的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回与类型对应的对象池实例</returns>
        IObjectPool<T> GetObjectPool<T>() where T : ObjectBase;

        /// <summary>
        /// 获取指定类型对应的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>返回与类型对应的对象池实例</returns>
        ObjectPoolBase GetObjectPool(SystemType objectType);

        /// <summary>
        /// 获取指定类型和名称对应的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <returns>返回与类型和名称对应的对象池实例</returns>
        IObjectPool<T> GetObjectPool<T>(string name) where T : ObjectBase;

        /// <summary>
        /// 获取指定类型和名称对应的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <returns>返回与类型和名称对应的对象池实例</returns>
        ObjectPoolBase GetObjectPool(SystemType objectType, string name);

        /// <summary>
        /// 获取匹配指定条件的对象池实例
        /// </summary>
        /// <param name="condition">要检查的条件</param>
        /// <returns>返回匹配给定条件的对象池实例</returns>
        ObjectPoolBase GetObjectPool(System.Predicate<ObjectPoolBase> condition);

        /// <summary>
        /// 批量获取匹配指定条件的对象池实例的集合
        /// </summary>
        /// <param name="condition">要检查的条件</param>
        /// <returns>返回匹配给定条件的对象池实例的集合</returns>
        ObjectPoolBase[] GetObjectPools(System.Predicate<ObjectPoolBase> condition);

        /// <summary>
        /// 获取匹配指定条件的对象池实例，并填充到指定容器中
        /// </summary>
        /// <param name="condition">要检查的条件</param>
        /// <param name="results">填充对象池的容器</param>
        void GetObjectPools(System.Predicate<ObjectPoolBase> condition, IList<ObjectPoolBase> results);

        /// <summary>
        /// 获取当前管理容器中所有的对象池实例
        /// </summary>
        /// <returns>返回所有对象池实例</returns>
        ObjectPoolBase[] GetAllObjectPools();

        /// <summary>
        /// 获取当前管理容器中所有的对象池实例，并填充到指定容器中
        /// </summary>
        /// <param name="results">填充对象池的容器</param>
        void GetAllObjectPools(List<ObjectPoolBase> results);

        /// <summary>
        /// 获取当前管理容器中所有的对象池实例，并根据标识是否按排序返回
        /// </summary>
        /// <param name="sort">是否按优先级排序</param>
        /// <returns>返回所有对象池实例</returns>
        ObjectPoolBase[] GetAllObjectPools(bool sort);

        /// <summary>
        /// 获取当前管理容器中所有的对象池实例，并根据标识是否按排序填充到指定容器中
        /// </summary>
        /// <param name="sort">是否按优先级排序</param>
        /// <param name="results">填充对象池的容器</param>
        void GetAllObjectPools(bool sort, List<ObjectPoolBase> results);

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        IObjectPool<T> CreateSingleSpawnObjectPool<T>() where T : ObjectBase;

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        ObjectPoolBase CreateSingleSpawnObjectPool(SystemType objectType);

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name) where T : ObjectBase;

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        ObjectPoolBase CreateSingleSpawnObjectPool(SystemType objectType, string name);

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        IObjectPool<T> CreateSingleSpawnObjectPool<T>(int capacity) where T : ObjectBase;

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        ObjectPoolBase CreateSingleSpawnObjectPool(SystemType objectType, int capacity);

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        IObjectPool<T> CreateSingleSpawnObjectPool<T>(float expireTime) where T : ObjectBase;

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        ObjectPoolBase CreateSingleSpawnObjectPool(SystemType objectType, float expireTime);

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, int capacity) where T : ObjectBase;

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        ObjectPoolBase CreateSingleSpawnObjectPool(SystemType objectType, string name, int capacity);

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, float expireTime) where T : ObjectBase;

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        ObjectPoolBase CreateSingleSpawnObjectPool(SystemType objectType, string name, float expireTime);

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        IObjectPool<T> CreateSingleSpawnObjectPool<T>(int capacity, float expireTime) where T : ObjectBase;

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        ObjectPoolBase CreateSingleSpawnObjectPool(SystemType objectType, int capacity, float expireTime);

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        IObjectPool<T> CreateSingleSpawnObjectPool<T>(int capacity, int priority) where T : ObjectBase;

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        ObjectPoolBase CreateSingleSpawnObjectPool(SystemType objectType, int capacity, int priority);

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        IObjectPool<T> CreateSingleSpawnObjectPool<T>(float expireTime, int priority) where T : ObjectBase;

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        ObjectPoolBase CreateSingleSpawnObjectPool(SystemType objectType, float expireTime, int priority);

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, int capacity, float expireTime) where T : ObjectBase;

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        ObjectPoolBase CreateSingleSpawnObjectPool(SystemType objectType, string name, int capacity, float expireTime);

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, int capacity, int priority) where T : ObjectBase;

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        ObjectPoolBase CreateSingleSpawnObjectPool(SystemType objectType, string name, int capacity, int priority);

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, float expireTime, int priority) where T : ObjectBase;

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        ObjectPoolBase CreateSingleSpawnObjectPool(SystemType objectType, string name, float expireTime, int priority);

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        IObjectPool<T> CreateSingleSpawnObjectPool<T>(int capacity, float expireTime, int priority) where T : ObjectBase;

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        ObjectPoolBase CreateSingleSpawnObjectPool(SystemType objectType, int capacity, float expireTime, int priority);

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, int capacity, float expireTime, int priority) where T : ObjectBase;

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        ObjectPoolBase CreateSingleSpawnObjectPool(SystemType objectType, string name, int capacity, float expireTime, int priority);

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="autoReleaseInterval">对象池自动释放的间隔时间</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, float autoReleaseInterval, int capacity, float expireTime, int priority) where T : ObjectBase;

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="autoReleaseInterval">对象池自动释放的间隔时间</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        ObjectPoolBase CreateSingleSpawnObjectPool(SystemType objectType, string name, float autoReleaseInterval, int capacity, float expireTime, int priority);

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        IObjectPool<T> CreateMultiSpawnObjectPool<T>() where T : ObjectBase;

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        ObjectPoolBase CreateMultiSpawnObjectPool(SystemType objectType);

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name) where T : ObjectBase;

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        ObjectPoolBase CreateMultiSpawnObjectPool(SystemType objectType, string name);

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        IObjectPool<T> CreateMultiSpawnObjectPool<T>(int capacity) where T : ObjectBase;

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        ObjectPoolBase CreateMultiSpawnObjectPool(SystemType objectType, int capacity);

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        IObjectPool<T> CreateMultiSpawnObjectPool<T>(float expireTime) where T : ObjectBase;

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        ObjectPoolBase CreateMultiSpawnObjectPool(SystemType objectType, float expireTime);

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, int capacity) where T : ObjectBase;

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        ObjectPoolBase CreateMultiSpawnObjectPool(SystemType objectType, string name, int capacity);

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, float expireTime) where T : ObjectBase;

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        ObjectPoolBase CreateMultiSpawnObjectPool(SystemType objectType, string name, float expireTime);

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        IObjectPool<T> CreateMultiSpawnObjectPool<T>(int capacity, float expireTime) where T : ObjectBase;

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        ObjectPoolBase CreateMultiSpawnObjectPool(SystemType objectType, int capacity, float expireTime);

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        IObjectPool<T> CreateMultiSpawnObjectPool<T>(int capacity, int priority) where T : ObjectBase;

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        ObjectPoolBase CreateMultiSpawnObjectPool(SystemType objectType, int capacity, int priority);

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        IObjectPool<T> CreateMultiSpawnObjectPool<T>(float expireTime, int priority) where T : ObjectBase;

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        ObjectPoolBase CreateMultiSpawnObjectPool(SystemType objectType, float expireTime, int priority);

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, int capacity, float expireTime) where T : ObjectBase;

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        ObjectPoolBase CreateMultiSpawnObjectPool(SystemType objectType, string name, int capacity, float expireTime);

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, int capacity, int priority) where T : ObjectBase;

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        ObjectPoolBase CreateMultiSpawnObjectPool(SystemType objectType, string name, int capacity, int priority);

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, float expireTime, int priority) where T : ObjectBase;

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        ObjectPoolBase CreateMultiSpawnObjectPool(SystemType objectType, string name, float expireTime, int priority);

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        IObjectPool<T> CreateMultiSpawnObjectPool<T>(int capacity, float expireTime, int priority) where T : ObjectBase;

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        ObjectPoolBase CreateMultiSpawnObjectPool(SystemType objectType, int capacity, float expireTime, int priority);

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, int capacity, float expireTime, int priority) where T : ObjectBase;

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        ObjectPoolBase CreateMultiSpawnObjectPool(SystemType objectType, string name, int capacity, float expireTime, int priority);

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="autoReleaseInterval">对象池自动释放的间隔时间</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, float autoReleaseInterval, int capacity, float expireTime, int priority) where T : ObjectBase;

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="autoReleaseInterval">对象池自动释放的间隔时间</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        ObjectPoolBase CreateMultiSpawnObjectPool(SystemType objectType, string name, float autoReleaseInterval, int capacity, float expireTime, int priority);

        /// <summary>
        /// 销毁指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>若对象池销毁成功返回true，否则返回false</returns>
        bool DestroyObjectPool<T>() where T : ObjectBase;

        /// <summary>
        /// 销毁指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>若对象池销毁成功返回true，否则返回false</returns>
        bool DestroyObjectPool(SystemType objectType);

        /// <summary>
        /// 销毁指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <returns>若对象池销毁成功返回true，否则返回false</returns>
        bool DestroyObjectPool<T>(string name) where T : ObjectBase;

        /// <summary>
        /// 销毁指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <returns>若对象池销毁成功返回true，否则返回false</returns>
        bool DestroyObjectPool(SystemType objectType, string name);

        /// <summary>
        /// 从当前管理容器中销毁指定的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="objectPool">目标对象池实例</param>
        /// <returns>若对象池销毁成功返回true，否则返回false</returns>
        bool DestroyObjectPool<T>(IObjectPool<T> objectPool) where T : ObjectBase;

        /// <summary>
        /// 从当前管理容器中销毁指定的对象池实例
        /// </summary>
        /// <param name="objectPool">目标对象池实例</param>
        /// <returns>若对象池销毁成功返回true，否则返回false</returns>
        bool DestroyObjectPool(ObjectPoolBase objectPool);

        /// <summary>
        /// 释放对象池中的所有可释放对象实例
        /// </summary>
        void Release();

        /// <summary>
        /// 释放对象池中的所有未使用对象实例
        /// </summary>
        void ReleaseAllUnused();
    }
}
