/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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

using System.Collections.Generic;

using SystemType = System.Type;

namespace NovaEngine
{
    /// <summary>
    /// 单例类对象的抽象父类，声明一个简易的单例对象类<br/>
    /// 该单例类通过单例工厂实现单例对象，而非内部自行管理实例
    /// </summary>
    public abstract class SingletonInstance<T> : ISingleton, IInitializable where T : class, ISingleton, new()
    {
        /// <summary>
        /// 获取单例类当前的有效实例
        /// </summary>
        public static T Instance
        {
            get { return SingletonFactory.GetInstance<T>(); }
        }

        /// <summary>
        /// 单例类的实例销毁接口
        /// </summary>
        public static void Destroy()
        {
            SingletonFactory.ReleaseInstance<T>();
        }

        /// <summary>
        /// 单例类的初始化回调接口函数
        /// </summary>
        public virtual void Initialize()
        { }

        /// <summary>
        /// 单例类的清理回调接口函数
        /// </summary>
        public virtual void Cleanup()
        { }
    }

    /// <summary>
    /// 单例工厂类，用于生成对象的单例，这里要求对象必须实现<see cref="NovaEngine.ISingleton"/>接口标识
    /// </summary>
    public static class SingletonFactory
    {
        /// <summary>
        /// 单例对象的实例管理容器
        /// </summary>
        private static IDictionary<SystemType, ISingleton> s_instances = new Dictionary<SystemType, ISingleton>();

        /// <summary>
        /// 获取指定类型的对象实例<br/>
        /// 此处如果在缓存中无法获取到对应的对象实例，将直接创建一个新的实例放入缓存中，同时返回给使用方
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回类型对应的对象实例</returns>
        public static T GetInstance<T>() where T : class, ISingleton, new()
        {
            SystemType classType = typeof(T);

            if (Environment.IsDevelopmentState())
            {
                Logger.Assert(Utility.Reflection.IsTypeOfInstantiableClass(classType), "Invalid arguments.");
            }

            if (s_instances.TryGetValue(classType, out ISingleton instance))
            {
                return instance as T;
            }

            instance = CreateInstance<T>();
            s_instances.Add(classType, instance);
            return instance as T;
        }

        /// <summary>
        /// 通过指定的类型创建一个新的对象实例<br/>
        /// 此处将检查类型是否继承了<see cref="NovaEngine.IInitializable"/>接口，如果继承自该接口，将自动调用初始化回调函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回给定类型新创建的对象实例</returns>
        private static T CreateInstance<T>() where T : class, ISingleton, new()
        {
            T instance = System.Activator.CreateInstance<T>();

            if (typeof(IInitializable).IsAssignableFrom(instance.GetType()))
            {
                (instance as IInitializable).Initialize();
            }

            return instance;
        }

        /// <summary>
        /// 通过指定的类型释放缓存中对应的对象实例<br/>
        /// 此处将检查类型是否继承了<see cref="NovaEngine.IInitializable"/>接口，如果继承自该接口，将自动调用清理回调函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        internal static void ReleaseInstance<T>() where T : class
        {
            SystemType classType = typeof(T);

            ReleaseInstance(classType);
        }

        /// <summary>
        /// 通过指定的类型释放缓存中对应的对象实例<br/>
        /// 此处将检查类型是否继承了<see cref="NovaEngine.IInitializable"/>接口，如果继承自该接口，将自动调用清理回调函数
        /// </summary>
        /// <param name="classType">对象类型</param>
        private static void ReleaseInstance(SystemType classType)
        {
            if (false == s_instances.ContainsKey(classType))
            {
                Logger.Warn("Could not found any object instance with target class type '{0}', released instance failed.", classType.FullName);
                return;
            }

            ISingleton instance = s_instances[classType];
            s_instances.Remove(classType);

            if (typeof(IInitializable).IsAssignableFrom(instance.GetType()))
            {
                (instance as IInitializable).Cleanup();
            }
        }

        /// <summary>
        /// 释放当前缓存的所有对象实例
        /// </summary>
        internal static void ReleaseAllInstances()
        {
            IList<SystemType> keys = Utility.Collection.ToList<SystemType>(s_instances.Keys);
            for (int n = 0; n < keys.Count; ++n)
            {
                ReleaseInstance(keys[n]);
            }
        }
    }
}
