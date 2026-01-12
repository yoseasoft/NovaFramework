/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using System;
using System.Collections.Generic;

namespace GameEngine
{
    /// 基于ECS模式定义的泛型句柄对象类
    public abstract partial class GenericEntityHandler<T>
    {
        /// <summary>
        /// 系统对象注册列表容器
        /// </summary>
        private IList<ISystem> _systems;
        /// <summary>
        /// 系统对象初始化回调列表容器
        /// </summary>
        private IList<IInitializeSystem> _initializeSystems;
        /// <summary>
        /// 系统对象清理回调列表容器
        /// </summary>
        private IList<ICleanupSystem> _cleanupSystems;
        /// <summary>
        /// 系统对象刷新回调列表容器
        /// </summary>
        private IList<IUpdateSystem> _updateSystems;
        /// <summary>
        /// 系统对象后置刷新回调列表容器
        /// </summary>
        private IList<ILateUpdateSystem> _lateUpdateSystems;

        /// <summary>
        /// 实体系统管理接口初始化回调函数
        /// </summary>
        // [OnSubmoduleInitCallback]
        private void OnEntitySystemInitialize()
        {
            //
            // 2025-12-29：
            // 因为`EntityHandler`为抽象类，且该回调函数为私有访问，
            // 所以在其可实例化的子类中查找子模块回调接口时，无法查找到该函数。
            // 除非把该函数的访问权限改为子类可访问的，
            // 再三考虑，暂且还是手动调用此回调吧。
            //

            // 系统注册列表初始化
            _systems = new List<ISystem>();
            // 系统初始化列表初始化
            _initializeSystems = new List<IInitializeSystem>();
            // 系统清理列表初始化
            _cleanupSystems = new List<ICleanupSystem>();
            // 系统刷新列表初始化
            _updateSystems = new List<IUpdateSystem>();
            // 系统后置刷新列表初始化
            _lateUpdateSystems = new List<ILateUpdateSystem>();
        }

        /// <summary>
        /// 实体系统管理接口清理回调函数
        /// </summary>
        // [OnSubmoduleCleanupCallback]
        private void OnEntitySystemCleanup()
        {
            // 移除所有系统对象实例
            RemoveAllSystems();

            _systems = null;
            _initializeSystems = null;
            _cleanupSystems = null;
            _updateSystems = null;
            _lateUpdateSystems = null;
        }

        /// <summary>
        /// 实体系统管理接口重载回调函数
        /// </summary>
        // [OnSubmoduleReloadCallback]
        private void OnEntitySystemReload()
        {
        }

        #region 实体对象系统模块相关操作函数合集

        /// <summary>
        /// 添加指定的系统对象实例到当前句柄容器中
        /// </summary>
        /// <param name="system">系统对象实例</param>
        /// <returns>若添加系统成功则返回true，否则返回false</returns>
        public bool AddSystem(ISystem system)
        {
            if (_systems.Contains(system))
            {
                Debugger.Warn("The system instance was already added, repeat add it failed.");
                return false;
            }

            _systems.Add(system);

            Type type = system.GetType();

            // 注册初始化回调接口
            if (typeof(IInitializeSystem).IsAssignableFrom(type))
            {
                _initializeSystems.Add(system as IInitializeSystem);
            }

            // 注册清理回调接口
            if (typeof(ICleanupSystem).IsAssignableFrom(type))
            {
                _cleanupSystems.Add(system as ICleanupSystem);
            }

            // 注册刷新回调接口
            if (typeof(IUpdateSystem).IsAssignableFrom(type))
            {
                _updateSystems.Add(system as IUpdateSystem);
            }

            // 注册后置刷新回调接口
            if (typeof(ILateUpdateSystem).IsAssignableFrom(type))
            {
                _lateUpdateSystems.Add(system as ILateUpdateSystem);
            }

            return true;
        }

        /// <summary>
        /// 从当前句柄容器中移除指定的系统对象实例
        /// </summary>
        /// <param name="system">系统对象实例</param>
        public void RemoveSystem(ISystem system)
        {
            if (false == _systems.Contains(system))
            {
                Debugger.Warn("Could not found any system instance in this container, remove it failed.");
                return;
            }

            _systems.Remove(system);
            _initializeSystems.Remove(system as IInitializeSystem);
            _cleanupSystems.Remove(system as ICleanupSystem);
            _updateSystems.Remove(system as IUpdateSystem);
            _lateUpdateSystems.Remove(system as ILateUpdateSystem);
        }

        /// <summary>
        /// 移除当前句柄容器中记录的所有系统对象实例
        /// </summary>
        protected void RemoveAllSystems()
        {
            while (_systems.Count > 0)
            {
                // 从最后一个元素开始进行删除
                RemoveSystem(_systems[_systems.Count - 1]);
            }
        }

        /// <summary>
        /// 调用指定实体对象的初始化回调系统接口
        /// </summary>
        /// <param name="entity">实体对象实例</param>
        protected void CallInitializeForSystem(T entity)
        {
            for (int n = 0; n < _initializeSystems.Count; ++n)
            {
                _initializeSystems[n].Initialize(entity);
            }
        }

        /// <summary>
        /// 调用指定实体对象的清理回调系统接口
        /// </summary>
        /// <param name="entity">实体对象实例</param>
        protected void CallCleanupForSystem(T entity)
        {
            for (int n = 0; n < _cleanupSystems.Count; ++n)
            {
                _cleanupSystems[n].Cleanup(entity);
            }
        }

        /// <summary>
        /// 调用指定实体对象的刷新回调系统接口
        /// </summary>
        /// <param name="entity">实体对象实例</param>
        protected void CallUpdateForSystem(T entity)
        {
            for (int n = 0; n < _updateSystems.Count; ++n)
            {
                _updateSystems[n].Update(entity);
            }
        }

        /// <summary>
        /// 调用指定实体对象的后置刷新回调系统接口
        /// </summary>
        /// <param name="entity">实体对象实例</param>
        protected void CallLateUpdateForSystem(T entity)
        {
            for (int n = 0; n < _lateUpdateSystems.Count; ++n)
            {
                _lateUpdateSystems[n].LateUpdate(entity);
            }
        }

        #endregion
    }
}
