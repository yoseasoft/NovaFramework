/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace GameEngine
{
    /// <summary>
    /// 反射注入接口的控制器类，对整个程序所有反射注入函数进行统一的整合和管理
    /// </summary>
    internal partial class InjectController
    {
        /// <summary>
        /// 多例模式的实体对象的缓存管理容器
        /// </summary>
        private IList<CBean> _multipleBeanInstanceCaches = null;
        /// <summary>
        /// 单例模式的实体对象的缓存管理容器
        /// </summary>
        private static IDictionary<string, CBean> _singletonBeanInstanceCaches = null;

        /// <summary>
        /// 实体注入缓存相关的初始化回调函数
        /// </summary>
        [OnControllerSubmoduleInitCallback]
        private void InitInjectBeanCaches()
        {
            // 多例实体对象缓存容器初始化
            _multipleBeanInstanceCaches = new List<CBean>();
            // 单例实体对象缓存容器初始化
            _singletonBeanInstanceCaches = new Dictionary<string, CBean>();
        }

        /// <summary>
        /// 实体注入缓存相关的清理回调函数
        /// </summary>
        [OnControllerSubmoduleCleanupCallback]
        private void CleanupInjectBeanCaches()
        {
            // 移除全部多例实体对象缓存实例
            RemoveAllMultipleBeanInstanceFromCache();
            // 移除全部单例实体对象缓存实例
            RemoveAllSingletonBeanInstancesFromCache();

            // 移除数据容器
            _multipleBeanInstanceCaches = null;
            _singletonBeanInstanceCaches = null;
        }

        #region 实体对象实例的缓存管理接口函数

        /// <summary>
        /// 新增指定的目标实体对象实例到当前的缓存队列中
        /// </summary>
        /// <param name="obj">对象实例</param>
        private void AddMultipleBeanInstanceToCache(CBean obj)
        {
            if (_multipleBeanInstanceCaches.Contains(obj))
            {
                Debugger.Warn("The target bean object '{0}' was already exist within multiple bean instance cache, repeat added it failed.", obj.BeanName);
                return;
            }

            _multipleBeanInstanceCaches.Add(obj);
        }

        /// <summary>
        /// 新增指定名称的单例实体对象到当前缓存队列中
        /// </summary>
        /// <param name="obj">对象实例</param>
        private void AddSingletonBeanInstanceToCache(CBean obj)
        {
            string beanName = obj.BeanName;
            if (_singletonBeanInstanceCaches.ContainsKey(beanName))
            {
                Debugger.Warn("The target bean name '{0}' was already exist within singleton bean instance cache, repeat added it will be override old value.", beanName);

                // 移除旧的实例
                RemoveCachedSingletonBeanInstanceByName(beanName);
            }

            _singletonBeanInstanceCaches.Add(beanName, obj);
        }

        /// <summary>
        /// 通过指定实体名称获取全部多例实体对象列表
        /// </summary>
        /// <param name="beanName">实体名称</param>
        /// <returns>返回多例实体对象列表</returns>
        private IList<CBean> FindAllMultipleBeanInstanceByName(string beanName)
        {
            IList<CBean> list = new List<CBean>();

            if (null == beanName)
            {
                return list;
            }

            for (int n = 0; n < _multipleBeanInstanceCaches.Count; ++n)
            {
                CBean bean = _multipleBeanInstanceCaches[n];
                if (null != bean.BeanName && bean.BeanName.Equals(beanName))
                {
                    list.Add(bean);
                }
            }

            return list;
        }

        /// <summary>
        /// 通过指定实体名称获取对应的单例实体对象
        /// </summary>
        /// <param name="beanName">实体名称</param>
        /// <returns>返回单例实体对象实例</returns>
        private CBean FindSingletonBeanInstanceByName(string beanName)
        {
            if (_singletonBeanInstanceCaches.TryGetValue(beanName, out CBean obj))
            {
                return obj;
            }

            return null;
        }

        /// <summary>
        /// 移除指定名称的多例实体对象缓存
        /// </summary>
        /// <param name="beanName">实体名称</param>
        private void RemoveCachedMultipleBeanInstanceByName(string beanName)
        {
            if (null == beanName)
            {
                return;
            }

            for (int n = _multipleBeanInstanceCaches.Count - 1; n >= 0; --n)
            {
                CBean bean = _multipleBeanInstanceCaches[n];
                if (null != bean.BeanName && bean.BeanName.Equals(beanName))
                {
                    _multipleBeanInstanceCaches.RemoveAt(n);
                }
            }
        }

        /// <summary>
        /// 从多例实体对象缓存中移除指定的实例
        /// </summary>
        /// <param name="obj">对象实例</param>
        private void RemoveCachedMultipleBeanInstanceByTarget(CBean obj)
        {
            if (false == _multipleBeanInstanceCaches.Contains(obj))
            {
                Debugger.Warn("Could not found any bean instance '{0}' within multiple cache, removed it failed.", obj.BeanName);
                return;
            }

            _multipleBeanInstanceCaches.Remove(obj);
        }

        /// <summary>
        /// 移除指定名称的单例实体对象缓存
        /// </summary>
        /// <param name="beanName">实体名称</param>
        private void RemoveCachedSingletonBeanInstanceByName(string beanName)
        {
            if (false == _singletonBeanInstanceCaches.TryGetValue(beanName, out CBean obj))
            {
                Debugger.Warn("Could not found any bean record with target name '{0}' from singleton bean instance cache, removed it failed.", beanName);
                return;
            }

            _singletonBeanInstanceCaches.Remove(beanName);
        }

        /// <summary>
        /// 从多例实体对象缓存中移除指定的实例
        /// </summary>
        /// <param name="obj">对象实例</param>
        private void ReleaseMultipleBeanInstanceFromCache(CBean obj)
        {
            if (false == _multipleBeanInstanceCaches.Contains(obj))
            {
                Debugger.Warn("Could not found any bean instance '{0}' within multiple cache, removed it failed.", obj.BeanName);
                return;
            }

            // 多例实体对象从缓存移除时，同步销毁对象实例
            ReleaseBeanInstance(obj);

            // 移除缓存记录
            RemoveCachedMultipleBeanInstanceByTarget(obj);
        }

        /// <summary>
        /// 释放指定名称的单例实体对象实例，并从缓存中移除
        /// </summary>
        /// <param name="beanName">实体名称</param>
        private void ReleaseSingletonBeanInstanceFromCache(string beanName)
        {
            if (false == _singletonBeanInstanceCaches.TryGetValue(beanName, out CBean obj))
            {
                Debugger.Warn("Could not found any bean record with target name '{0}' from singleton bean instance cache, released it failed.", beanName);
                return;
            }

            // 单例实体对象从缓存移除时，同步销毁对象实例
            ReleaseBeanInstance(obj);

            // 移除缓存记录
            RemoveCachedSingletonBeanInstanceByName(beanName);
        }

        /// <summary>
        /// 释放当前记录的所有多例实体对象实例，并从缓存中移除
        /// </summary>
        private void RemoveAllMultipleBeanInstanceFromCache()
        {
            while (_multipleBeanInstanceCaches.Count > 0)
            {
                ReleaseMultipleBeanInstanceFromCache(_multipleBeanInstanceCaches[0]);
            }
        }

        /// <summary>
        /// 释放当前记录的所有单例实体对象实例，并从缓存中移除
        /// </summary>
        private void RemoveAllSingletonBeanInstancesFromCache()
        {
            while (_singletonBeanInstanceCaches.Count > 0)
            {
                string beanName = NovaEngine.Utility.Collection.FirstKey<string, CBean>(_singletonBeanInstanceCaches);
                ReleaseSingletonBeanInstanceFromCache(beanName);
            }
        }

        #endregion
    }
}
