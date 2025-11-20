/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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

using System.Runtime.CompilerServices;

using SystemType = System.Type;

namespace GameEngine
{
    /// <summary>
    /// 应用程序的上下文管理器对象类，对应用通用接口进行封装，对外提供访问函数
    /// </summary>
    public static partial class ApplicationContext
    {
        /// <summary>
        /// 通过指定的类型获取Bean对象的实例<br/>
        /// 此接口获取的均为单例对象实例，若属性或配置上标识该类型为非单例模式，则使用该函数获取实例将返回失败结果<br/>
        /// 单例对象无需外部手动处理释放操作，实例的初始化和销毁均由引擎内部统一管理
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回对应的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetBean<T>() where T : CBean
        {
            return GetBean(typeof(T)) as T;
        }

        /// <summary>
        /// 通过指定的类型获取Bean对象的实例<br/>
        /// 此接口获取的均为单例对象实例，若属性或配置上标识该类型为非单例模式，则使用该函数获取实例将返回失败结果<br/>
        /// 单例对象无需外部手动处理释放操作，实例的初始化和销毁均由引擎内部统一管理
        /// </summary>
        /// <param name="classType">对象类型</param>
        /// <returns>返回对应的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CBean GetBean(SystemType classType)
        {
            return InjectController.Instance.GetBean(classType);
        }

        /// <summary>
        /// 通过指定的名称获取Bean对象的实例<br/>
        /// 此接口获取的均为单例对象实例，若属性或配置上标识该名称为非单例模式，则使用该函数获取实例将返回失败结果<br/>
        /// 单例对象无需外部手动处理释放操作，实例的初始化和销毁均由引擎内部统一管理
        /// </summary>
        /// <param name="beanName">对象名称</param>
        /// <returns>返回对应的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CBean GetBean(string beanName)
        {
            return InjectController.Instance.GetBean(beanName);
        }

        /// <summary>
        /// 通过指定的类型创建一个新的Bean对象实例<br/>
        /// 此接口获取的均为多例对象实例，若属性或配置上标识该类型为单例模式，则使用该函数获取实例将返回失败结果<br/>
        /// 多例对象需要外部手动处理释放操作，否则将导致内存泄漏的情况发生
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回对应的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateBean<T>() where T : CBase
        {
            return CreateBean(typeof(T)) as T;
        }

        /// <summary>
        /// 通过指定的类型创建一个新的Bean对象实例<br/>
        /// 此接口获取的均为多例对象实例，若属性或配置上标识该类型为单例模式，则使用该函数获取实例将返回失败结果<br/>
        /// 多例对象需要外部手动处理释放操作，否则将导致内存泄漏的情况发生
        /// </summary>
        /// <param name="classType">对象类型</param>
        /// <returns>返回对应的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CBean CreateBean(SystemType classType)
        {
            return InjectController.Instance.CreateBean(classType);
        }

        /// <summary>
        /// 通过指定的名称创建一个新的Bean对象实例<br/>
        /// 此接口获取的均为多例对象实例，若属性或配置上标识该名称为单例模式，则使用该函数获取实例将返回失败结果<br/>
        /// 多例对象需要外部手动处理释放操作，否则将导致内存泄漏的情况发生
        /// </summary>
        /// <param name="beanName">对象名称</param>
        /// <returns>返回对应的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CBean CreateBean(string beanName)
        {
            return InjectController.Instance.CreateBean(beanName);
        }

        /// <summary>
        /// 释放指定的Bean对象实例<br/>
        /// 目标对象实例必须为多例模式，否则将导致单例对象被提前释放从而发生未知异常
        /// </summary>
        /// <param name="bean">对象实例</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReleaseBean(CBean bean)
        {
            InjectController.Instance.ReleaseBean(bean);
        }
    }
}
