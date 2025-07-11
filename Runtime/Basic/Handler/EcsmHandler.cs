/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

namespace GameEngine
{
    /// <summary>
    /// 基于ECS模式定义的句柄对象类，针对实体类型访问操作的接口进行封装
    /// 该句柄对象提供实体相关的操作访问接口
    /// </summary>
    public abstract partial class EcsmHandler : BaseHandler
    {
        /// <summary>
        /// 实体对象映射管理容器
        /// </summary>
        private IList<CEntity> _entities = null;
        /// <summary>
        /// 实体对象唤醒列表容器
        /// </summary>
        private IList<CEntity> _awakeEntitiesList = null;
        /// <summary>
        /// 实体对象刷新列表容器
        /// </summary>
        private IList<CEntity> _updateEntitiesList = null;

        /// <summary>
        /// 系统对象注册列表容器
        /// </summary>
        private IList<ISystem> _systems = null;
        /// <summary>
        /// 系统对象初始化回调列表容器
        /// </summary>
        private IList<IInitializeSystem> _initializeSystems = null;
        /// <summary>
        /// 系统对象清理回调列表容器
        /// </summary>
        private IList<ICleanupSystem> _cleanupSystems = null;
        /// <summary>
        /// 系统对象刷新回调列表容器
        /// </summary>
        private IList<IUpdateSystem> _updateSystems = null;
        /// <summary>
        /// 系统对象后置刷新回调列表容器
        /// </summary>
        private IList<ILateUpdateSystem> _lateUpdateSystems = null;

        /// <summary>
        /// 获取当前记录的全部实体对象实例
        /// </summary>
        protected IList<CEntity> Entities => _entities;

        /// <summary>
        /// 句柄对象默认构造函数
        /// </summary>
        public EcsmHandler()
        {
        }

        /// <summary>
        /// 句柄对象析构函数
        /// </summary>
        ~EcsmHandler()
        {
        }

        /// <summary>
        /// 句柄对象内置初始化接口函数
        /// </summary>
        /// <returns>若句柄对象初始化成功则返回true，否则返回false</returns>
        protected override bool OnInitialize()
        {
            // 实体映射容器初始化
            _entities = new List<CEntity>();
            // 实体唤醒列表初始化
            _awakeEntitiesList = new List<CEntity>();
            // 实体刷新列表初始化
            _updateEntitiesList = new List<CEntity>();

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

            return true;
        }

        /// <summary>
        /// 句柄对象内置清理接口函数
        /// </summary>
        protected override void OnCleanup()
        {
            // 移除所有实体对象实例
            RemoveAllEntities();

            _entities = null;
            _awakeEntitiesList = null;
            _updateEntitiesList = null;

            // 移除所有系统对象实例
            RemoveAllSystems();

            _systems = null;
            _initializeSystems = null;
            _cleanupSystems = null;
            _updateSystems = null;
            _lateUpdateSystems = null;
        }

        /// <summary>
        /// 句柄对象内置刷新接口
        /// </summary>
        protected override void OnUpdate()
        {
            // 实体实例刷新
            OnEntitiesUpdate();
        }

        /// <summary>
        /// 句柄对象内置延迟刷新接口
        /// </summary>
        protected override void OnLateUpdate()
        {
            // 实体实例后置刷新
            OnEntitiesLateUpdate();

            // 移除过期实体对象实例
            // RemoveAllExpiredEntities();
        }

        /// <summary>
        /// 句柄对象的模块事件转发回调接口
        /// </summary>
        /// <param name="e">模块事件参数</param>
        // public abstract void OnEventDispatch(NovaEngine.ModuleEventArgs e);

        #region 实体对象相关操作函数合集

        /// <summary>
        /// 获取当前记录的全部实体对象实例<br/>
        /// 这里返回的列表为克隆出来的，对该列表的操作不影响原数据
        /// </summary>
        /// <returns>返回当前记录的全部实体对象实例列表</returns>
        protected IList<CEntity> GetAllEntities()
        {
            IList<CEntity> entities = new List<CEntity>(_entities);
            return entities;
        }

        /// <summary>
        /// 添加指定的实体对象实例到当前句柄容器中<br/>
        /// 每次进行添加操作时，会对实体对象实例进行初始化操作<br/>
        /// 请勿自行调用实例的初始化接口，会导致多次调用的问题
        /// </summary>
        /// <param name="entity">实体对象实例</param>
        /// <returns>若添加实体成功则返回true，否则返回false</returns>
        protected bool AddEntity(CEntity entity)
        {
            /**
             * 暂时允许在刷新过程中添加实体对象，因为大部分实体对象是在start之后才添加到刷新队列中，
             * 而start的调用时间点是延迟在下一帧的开始位置进行调用的，
             * 因此，这里直接调用应该不会有什么问题吧？
             * 如果其实现对象对start操作进行特殊处理，例如：CScene，在awake之后直接start，
             * 那就需要自行判断是否会导致其它问题。
             * 这里CScene之所以没有问题，因为激活的场景实例永远只有一个，不存在队列变化的情况
             * if (IsOnUpdatingStatus)
             * {
             *     Debugger.Error("The container instance was updating now, cannot add any entity at once.");
             *     return false;
             * }
             */

            if (entity.IsOnExpired)
            {
                Debugger.Warn("The entity instance was expired, do add operation failed.");
                return false;
            }

            if (_entities.Contains(entity))
            {
                Debugger.Warn("The entity instance was already added, repeat add it failed.");
                return false;
            }

            // 初始化实体实例
            Call(entity.Initialize);

            // 调用系统初始化回调接口
            CallInitializeForSystem(entity);

            _entities.Add(entity);

            return true;
        }

        /// <summary>
        /// 从当前句柄容器中移除指定的实体对象实例<br/>
        /// 我们在进行移除时，会同时对该实体对象实例进行清理操作<br/>
        /// 请勿自行调用实例的清理接口，会导致多次调用的问题
        /// </summary>
        /// <param name="entity">实体对象实例</param>
        protected void RemoveEntity(CEntity entity)
        {
            // 2024-04-28:
            // 只有唤醒后且尚未销毁的实体对象，需要关心当前被移除的时候是否处于刷新循环中，
            // 因为只有处于这个生命周期范围内的实体对象才会注册到刷新列表中进行刷新操作，
            // 处于这个生命周期范围之外的实体对象，无需关心当前是否在刷新循环中
            if (IsOnUpdatingStatus && entity.IsOnAwakingStatus())
            {
                entity.OnPrepareToDestroy();

                Debugger.Warn("The container instance was updating now, cannot remove any entity at once.");
                return;
            }

            if (false == _entities.Contains(entity))
            {
                Debugger.Warn("Could not found any entity instance in this container, remove it failed.");
                return;
            }

            // 以防万一，先删为敬
            _awakeEntitiesList.Remove(entity);
            _updateEntitiesList.Remove(entity);
            _entities.Remove(entity);

            // 调用系统清理回调接口
            CallCleanupForSystem(entity);

            // 清理实体实例
            Call(entity.Cleanup);
        }

        /// <summary>
        /// 移除当前句柄容器中所有处于过期状态的实体对象实例
        /// </summary>
        protected void RemoveAllExpiredEntities()
        {
            for (int n = _entities.Count - 1; n >= 0; --n)
            {
                CEntity entity = _entities[n];
                if (entity.IsOnExpired)
                {
                    RemoveEntity(entity);
                }
            }
        }

        /// <summary>
        /// 移除当前句柄容器中记录的所有实体对象实例
        /// </summary>
        protected void RemoveAllEntities()
        {
            while (_entities.Count > 0)
            {
                // 从最后一个元素开始进行删除
                RemoveEntity(_entities[_entities.Count - 1]);
            }
        }

        /// <summary>
        /// 对指定的实体对象实例进行唤醒处理<br/>
        /// 此处将实例放置到唤醒队列中，等待后续的唤醒操作
        /// </summary>
        /// <param name="entity">实体对象实例</param>
        protected void CallEntityAwakeProcess(CEntity entity)
        {
            if (false == _entities.Contains(entity))
            {
                Debugger.Error("Could not found any entity instance '{0}' from current instantiation list, wakeup it failed.", entity.GetType().FullName);
                return;
            }

            if (false == entity.IsOnTargetLifecycle(CBase.LifecycleKeypointType.Startup))
            {
                Debugger.Error("The entity instance '{0}' was startup incompleted, wakeup it failed.", entity.GetType().FullName);
                // 无效的生命周期，直接终结目标实体对象
                RemoveEntity(entity);
                return;
            }

            // 唤醒实例
            Call(entity.Awake);

            ProtoController.Instance.RegProtoLifecycleNotification(AspectBehaviourType.Start, entity);
        }

        /// <summary>
        /// 对指定的实体对象实例进行销毁处理<br/>
        /// 未启动完成的对象实例，无需进行销毁处理
        /// </summary>
        /// <param name="entity">实体对象实例</param>
        protected void CallEntityDestroyProcess(CEntity entity)
        {
            if (false == entity.IsOnAwakingStatus())
            {
                Debugger.Warn("The entity instance '{0}' was startup incompleted, wakeup it failed.", entity.GetType().FullName);
                return;
            }

            ProtoController.Instance.UnregProtoLifecycleNotification(entity);

            // 销毁实例
            Call(entity.Destroy);
        }

        /// <summary>
        /// 对指定实体对象实例开启处理的回调函数
        /// </summary>
        /// <param name="entity">实体对象实例</param>
        protected internal void OnEntityStartProcessing(CEntity entity)
        {
            if (false == _entities.Contains(entity))
            {
                Debugger.Error("Could not found any added record of the entity instance '{0}', calling start process failed.", entity.GetType().FullName);
                return;
            }

            // 开始运行实例
            Call(entity.Start);

            // 激活刷新接口的对象实例，放入到刷新队列中
            if (typeof(IUpdateActivation).IsAssignableFrom(entity.GetType()))
            {
                _updateEntitiesList.Add(entity);
            }
        }

        /// <summary>
        /// 刷新当前句柄容器中记录的所有实体对象实例
        /// </summary>
        protected void OnEntitiesUpdate()
        {
            for (int n = 0; n < _updateEntitiesList.Count; ++n)
            {
                CEntity entity = _updateEntitiesList[n];
                // 过期对象跳过该操作
                if (entity.IsOnExpired)
                {
                    continue;
                }

                // 对象刷新操作
                Call(entity.Update);

                // 调用系统刷新回调接口
                CallUpdateForSystem(entity);
            }
        }

        /// <summary>
        /// 后置刷新当前句柄容器中记录的所有实体对象实例
        /// </summary>
        protected void OnEntitiesLateUpdate()
        {
            for (int n = 0; n < _updateEntitiesList.Count; ++n)
            {
                CEntity entity = _updateEntitiesList[n];
                // 过期对象跳过该操作
                if (entity.IsOnExpired)
                {
                    continue;
                }

                // 对象后置刷新操作
                Call(entity.LateUpdate);

                // 调用系统后置刷新回调接口
                CallLateUpdateForSystem(entity);
            }
        }

        #endregion

        #region 系统对象相关操作函数合集

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

            // 注册初始化回调接口
            if (typeof(IInitializeSystem).IsAssignableFrom(system.GetType()))
            {
                _initializeSystems.Add(system as IInitializeSystem);
            }

            // 注册清理回调接口
            if (typeof(ICleanupSystem).IsAssignableFrom(system.GetType()))
            {
                _cleanupSystems.Add(system as ICleanupSystem);
            }

            // 注册刷新回调接口
            if (typeof(IUpdateSystem).IsAssignableFrom(system.GetType()))
            {
                _updateSystems.Add(system as IUpdateSystem);
            }

            // 注册后置刷新回调接口
            if (typeof(ILateUpdateSystem).IsAssignableFrom(system.GetType()))
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
        protected void CallInitializeForSystem(CEntity entity)
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
        protected void CallCleanupForSystem(CEntity entity)
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
        protected void CallUpdateForSystem(CEntity entity)
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
        protected void CallLateUpdateForSystem(CEntity entity)
        {
            for (int n = 0; n < _lateUpdateSystems.Count; ++n)
            {
                _lateUpdateSystems[n].LateUpdate(entity);
            }
        }

        #endregion
    }
}
