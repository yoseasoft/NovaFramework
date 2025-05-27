/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using SystemType = System.Type;

namespace GameEngine
{
    /// <summary>
    /// 反射注入接口的控制器类，对整个程序所有反射注入函数进行统一的整合和管理
    /// </summary>
    public partial class InjectController
    {
        /// <summary>
        /// 通过指定的类型获取Bean对象的实例<br/>
        /// 此接口获取的均为单例对象实例，若属性或配置上标识该类型为非单例模式，则使用该函数获取实例将返回失败结果<br/>
        /// 单例对象无需外部手动处理释放操作，实例的初始化和销毁均由引擎内部统一管理
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回对应的对象实例</returns>
        public T GetBean<T>() where T : CBean
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
        public CBean GetBean(SystemType classType)
        {
            Loader.Symboling.SymClass symbol = Loader.CodeLoader.GetSymClassByType(classType);
            if (null != symbol)
            {
                // 获取默认Bean实例
                return GetBean(symbol.DefaultBeanName);
            }

            return null;
        }

        /// <summary>
        /// 通过指定的名称获取Bean对象的实例<br/>
        /// 此接口获取的均为单例对象实例，若属性或配置上标识该名称为非单例模式，则使用该函数获取实例将返回失败结果<br/>
        /// 单例对象无需外部手动处理释放操作，实例的初始化和销毁均由引擎内部统一管理
        /// </summary>
        /// <param name="beanName">对象名称</param>
        /// <returns>返回对应的对象实例</returns>
        public CBean GetBean(string beanName)
        {
            CBean obj = FindSingletonBeanInstanceByName(beanName);
            if (null != obj)
            {
                return obj;
            }

            Loader.Symboling.Bean bean = Loader.CodeLoader.GetBeanClassByName(beanName);
            if (null == bean)
            {
                Debugger.Warn("Could not found any bean class with target name '{0}' from loader, created bean object instance failed.", beanName);
                return null;
            }

            if (false == bean.Singleton)
            {
                Debugger.Warn("The target bean '{0}' was multiple mode, please used 'CreateBean' method to build bean instance.", beanName);
                return null;
            }

            obj = CreateBeanInstance(bean);
            if (null == obj)
            {
                Debugger.Warn("Failed to create singleton bean instance with target name '{0}', please checked the bean configure was correct.", beanName);
                return null;
            }

            // 添加实例到缓存中
            AddSingletonBeanInstanceToCache(obj);

            return obj;
        }

        /// <summary>
        /// 通过指定的类型创建一个新的Bean对象实例<br/>
        /// 此接口获取的均为多例对象实例，若属性或配置上标识该类型为单例模式，则使用该函数获取实例将返回失败结果<br/>
        /// 多例对象需要外部手动处理释放操作，否则将导致内存泄漏的情况发生
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回对应的对象实例</returns>
        public T CreateBean<T>() where T : CBase
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
        public CBean CreateBean(SystemType classType)
        {
            Loader.Symboling.SymClass symbol = Loader.CodeLoader.GetSymClassByType(classType);
            if (null != symbol)
            {
                // 获取默认Bean实例
                return CreateBean(symbol.DefaultBeanName);
            }

            return null;
        }

        /// <summary>
        /// 通过指定的名称创建一个新的Bean对象实例<br/>
        /// 此接口获取的均为多例对象实例，若属性或配置上标识该名称为单例模式，则使用该函数获取实例将返回失败结果<br/>
        /// 多例对象需要外部手动处理释放操作，否则将导致内存泄漏的情况发生
        /// </summary>
        /// <param name="beanName">对象名称</param>
        /// <returns>返回对应的对象实例</returns>
        public CBean CreateBean(string beanName)
        {
            Loader.Symboling.Bean bean = Loader.CodeLoader.GetBeanClassByName(beanName);
            if (null == bean)
            {
                Debugger.Warn("Could not found any bean class with target name '{0}' from loader, created bean object instance failed.", beanName);
                return null;
            }

            if (bean.Singleton)
            {
                Debugger.Warn("The target bean '{0}' was singleton mode, please used 'GetBean' method to build bean instance.", beanName);
                return null;
            }

            CBean obj = CreateBeanInstance(bean);
            if (null == obj)
            {
                Debugger.Warn("Failed to create multiple bean instance with target name '{0}', please checked the bean configure was correct.", beanName);
                return null;
            }

            AddMultipleBeanInstanceToCache(obj);

            return obj;
        }

        /// <summary>
        /// 释放指定的Bean对象实例<br/>
        /// 目标对象实例必须为多例模式，否则将导致单例对象被提前释放从而发生未知异常
        /// </summary>
        /// <param name="bean">对象实例</param>
        public void ReleaseBean(CBean bean)
        {
            string beanName = bean.BeanName;
            if (string.IsNullOrEmpty(beanName))
            {
                Debugger.Warn("The target bean name must be non-null, released it failed.");
                return;
            }

            Loader.Symboling.Bean beanClass = Loader.CodeLoader.GetBeanClassByName(beanName);
            if (null == beanClass)
            {
                Debugger.Warn("Could not found any bean class with target name '{0}' from loader, created bean object instance failed.", beanName);
                return;
            }

            if (beanClass.Singleton)
            {
                // 从缓存中移除单例对象实例
                RemoveCachedSingletonBeanInstanceByName(beanName);
            }
            else
            {
                // 从缓存中移除多例对象实例
                RemoveCachedMultipleBeanInstanceByTarget(bean);
            }

            ReleaseBeanInstance(bean);
        }

        #region 实体对象的实例创建/销毁管理接口函数

        /// <summary>
        /// 通过指定的对象名称创建一个对象实例
        /// </summary>
        /// <param name="beanName">实体名称</param>
        /// <returns>返回新创建的对象实例</returns>
        private CBean CreateBeanInstance(string beanName)
        {
            Loader.Symboling.Bean bean = Loader.CodeLoader.GetBeanClassByName(beanName);
            if (null == bean)
            {
                Debugger.Warn("Could not found any bean class struct with target name '{0}', create bean instance failed.", beanName);
                return null;
            }

            return InjectBeanService.CreateBeanInstance(bean);
        }

        /// <summary>
        /// 通过指定的对象类型创建一个对象实例
        /// </summary>
        /// <param name="classType">对象类型</param>
        /// <returns>返回新创建的对象实例</returns>
        private CBean CreateBeanInstance(SystemType classType)
        {
            Loader.Symboling.SymClass symClass = Loader.CodeLoader.GetSymClassByType(classType);
            if (null == symClass)
            {
                Debugger.Warn("Could not found any bean class struct with target type '{0}', create bean instance failed.", NovaEngine.Utility.Text.ToString(classType));
                return null;
            }

            return InjectBeanService.CreateBeanInstance(symClass, null);
        }

        /// <summary>
        /// 通过指定的实体信息，创建一个新的对象实例
        /// </summary>
        /// <param name="bean">实体信息</param>
        /// <returns>返回新创建的实体对象实例，若创建失败返回null</returns>
        private CBean CreateBeanInstance(Loader.Symboling.Bean bean)
        {
            return InjectBeanService.CreateBeanInstance(bean);
        }

        /// <summary>
        /// 释放指定的对象实例
        /// </summary>
        /// <param name="bean">对象实例</param>
        private void ReleaseBeanInstance(CBean bean)
        {
            InjectBeanService.ReleaseBeanInstance(bean);
        }

        #endregion
    }
}
