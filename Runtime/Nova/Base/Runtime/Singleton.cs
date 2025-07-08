/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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

namespace NovaEngine
{
    /// <summary>
    /// 单例类对象的抽象父类，声明一个简易的单例对象类<br/>
    /// 当您需要创建一个单例对象类时，建议通过继承该抽象类来实现<br/>
    /// 它提供了对象初始化及清理的默认接口函数，并通过静态函数进行实例的创建及销毁
    /// </summary>
    public abstract class Singleton<T> : ISingleton where T : class, new()
    {
        private static T _instance;

        /// <summary>
        /// 单例对象的默认构造函数<br/>
        /// 此处将函数的作用域声明为‘protected’，需要在自定义子类时实现该默认构造函数，且打开其访问作用域
        /// </summary>
        protected Singleton()
        {
        }

        /// <summary>
        /// 单例对象的默认析构函数
        /// </summary>
        ~Singleton()
        {
        }

        /// <summary>
        /// 获取单例类当前的有效实例
        /// </summary>
        public static T Instance
        {
            get
            {
                if (null == Singleton<T>._instance)
                {
                    Singleton<T>.Create();
                }

                return Singleton<T>._instance;
            }
        }

        /// <summary>
        /// 单例类的实例创建接口
        /// </summary>
        public static T Create()
        {
            if (Singleton<T>._instance == null)
            {
                Singleton<T>._instance = System.Activator.CreateInstance<T>();
                (Singleton<T>._instance as Singleton<T>).Initialize();
            }

            return Singleton<T>._instance;
        }

        /// <summary>
        /// 单例类的实例销毁接口
        /// </summary>
        public static void Destroy()
        {
            if (Singleton<T>._instance != null)
            {
                (Singleton<T>._instance as Singleton<T>).Cleanup();
                Singleton<T>._instance = (T) ((object) null);
            }
        }

        /// <summary>
        /// 判断指定类型的单例是否为空
        /// </summary>
        /// <returns>若该类型的单例为空则返回true，否则返回false</returns>
        public static bool IsNull()
        {
            return (null == Singleton<T>._instance);
        }

        /// <summary>
        /// 单例类初始化回调接口
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// 单例类清理回调接口
        /// </summary>
        protected abstract void Cleanup();
    }
}
