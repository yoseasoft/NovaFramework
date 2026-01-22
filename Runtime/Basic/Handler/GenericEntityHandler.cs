/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
using System.Customize.Extension;
using System.Runtime.CompilerServices;

namespace GameEngine
{
    /// <summary>
    /// 基于ECS模式定义的泛型句柄对象类，针对实体类型访问操作的接口进行封装
    /// 该句柄对象提供实体相关的操作访问接口
    /// 
    /// 2026-01-11：
    /// 将实体句柄改为泛型定义，这样所有继承自该句柄的实例，其内部的实体列表都可以直接使用`Entities`
    /// 而无需在每个子句柄中再次定义，增加列表维护的难度
    /// </summary>
    public abstract partial class GenericEntityHandler<T> : EntityHandler where T : CEntity
    {
        /// <summary>
        /// 实体对象类型映射注册管理容器
        /// </summary>
        protected IDictionary<string, Type> _entityClassTypes;
        /// <summary>
        /// 实体优先级映射注册管理容器
        /// </summary>
        protected IDictionary<string, int> _entityPriorities;

        /// <summary>
        /// 实体对象映射管理容器
        /// </summary>
        private IList<T> _entities;
        /// <summary>
        /// 实体对象执行列表容器
        /// </summary>
        private IList<T> _executeEntitiesList;
        /// <summary>
        /// 实体对象刷新列表容器
        /// </summary>
        private IList<T> _updateEntitiesList;

        /// <summary>
        /// 获取当前记录的全部实体对象实例
        /// </summary>
        protected IList<T> Entities => _entities;

        /// <summary>
        /// 句柄对象默认构造函数
        /// </summary>
        protected internal GenericEntityHandler() : base()
        { }

        /// <summary>
        /// 句柄对象析构函数
        /// </summary>
        ~GenericEntityHandler()
        { }

        /// <summary>
        /// 句柄对象内置初始化接口函数
        /// </summary>
        /// <returns>若句柄对象初始化成功则返回true，否则返回false</returns>
        protected override bool OnInitialize()
        {
            // 初始化实体类注册容器
            _entityClassTypes = new Dictionary<string, Type>();
            _entityPriorities = new Dictionary<string, int>();

            // 实体映射容器初始化
            _entities = new List<T>();
            // 实体执行列表初始化
            _executeEntitiesList = new List<T>();
            // 实体刷新列表初始化
            _updateEntitiesList = new List<T>();

            // 实体系统接口初始化
            OnEntitySystemInitialize();

            return true;
        }

        /// <summary>
        /// 句柄对象内置清理接口函数
        /// </summary>
        protected override void OnCleanup()
        {
            // 实体系统接口清理
            OnEntitySystemCleanup();

            // 移除所有实体对象实例
            RemoveAllEntities();

            _entities = null;
            _executeEntitiesList = null;
            _updateEntitiesList = null;

            // 清理实体类注册容器
            _entityClassTypes.Clear();
            _entityPriorities.Clear();

            _entityClassTypes = null;
            _entityPriorities = null;
        }

        /// <summary>
        /// 句柄对象内置重载接口函数
        /// </summary>
        protected override void OnReload()
        {
            // 实体系统接口重载
            OnEntitySystemReload();
        }

        /// <summary>
        /// 句柄对象内置执行接口
        /// </summary>
        protected override void OnExecute()
        {
            // 实体实例执行
            OnEntitiesExecute();
        }

        /// <summary>
        /// 句柄对象内置延迟执行接口
        /// </summary>
        protected override void OnLateExecute()
        {
            // 实体实例后置执行
            OnEntitiesLateExecute();
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

        #region 实体类动态注册绑定接口函数

        /// <summary>
        /// 注册指定的实体名称及对应的对象类到当前的句柄管理容器中
        /// 注意，注册的对象类必须继承自<see cref="GameEngine.CEntity"/>，否则无法正常注册
        /// </summary>
        /// <param name="entityName">实体名称</param>
        /// <param name="clsType">实体类型</param>
        /// <param name="priority">实体优先级</param>
        /// <returns>若实体类型注册成功则返回true，否则返回false</returns>
        protected bool RegisterEntityClass(string entityName, Type clsType, int priority)
        {
            Debugger.Assert(false == string.IsNullOrEmpty(entityName) && null != clsType, NovaEngine.ErrorText.InvalidArguments);

            if (false == typeof(CEntity).IsAssignableFrom(clsType))
            {
                Debugger.Warn(LogGroupTag.Module, "The register type '{%t}' must be inherited from 'CEntity'.", clsType);
                return false;
            }

            if (_entityClassTypes.ContainsKey(entityName))
            {
                Debugger.Warn(LogGroupTag.Module, "The entity name '{%s}' was already registered, repeat add will be override old name.", entityName);
                _entityClassTypes.Remove(entityName);
            }

            // Debugger.Info(LogGroupTag.Module, "The new entity class name {%s} and type {%t} register succeed.", entityName, clsType);
            _entityClassTypes.Add(entityName, clsType);
            if (priority > 0)
            {
                _entityPriorities.TryAdd(entityName, priority);
            }

            return true;
        }

        /// <summary>
        /// 注销当前句柄实例绑定的所有实体类型
        /// </summary>
        protected void UnregisterAllEntityClasses()
        {
            _entityClassTypes.Clear();
            _entityPriorities.Clear();
        }

        #endregion

        #region 实体对象元素实例访问相关操作函数合集

        /// <summary>
        /// 获取当前记录的全部实体对象实例<br/>
        /// 这里返回的列表为克隆出来的，对该列表的操作不影响原数据
        /// </summary>
        /// <returns>返回当前记录的全部实体对象实例列表</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected IReadOnlyList<T> GetAllEntities()
        {
            IReadOnlyList<T> entities = new List<T>(_entities);
            return entities;
        }

        /// <summary>
        /// 检测当前的实体对象列表中是否存在指定标识的对象实例
        /// </summary>
        /// <param name="beanId">实体标识</param>
        /// <returns>若存在指定标识的对象实例则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool HasEntityById(int beanId)
        {
            for (int n = 0; n < _entities.Count; ++n)
            {
                if (_entities[n].BeanId == beanId)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 通过指定的实体标识查找对应的实体对象实例
        /// </summary>
        /// <param name="beanId">实体标识</param>
        /// <returns>返回对应的实体对象实例，若该实例不存在则返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected T GetEntityById(int beanId)
        {
            for (int n = 0; n < _entities.Count; ++n)
            {
                if (_entities[n].BeanId == beanId)
                {
                    return _entities[n];
                }
            }

            return null;
        }

        /// <summary>
        /// 添加指定的实体对象实例到当前句柄容器中<br/>
        /// 每次进行添加操作时，会对实体对象实例进行初始化操作<br/>
        /// 请勿自行调用实例的初始化接口，会导致多次调用的问题
        /// </summary>
        /// <param name="entity">实体对象实例</param>
        /// <returns>若添加实体成功则返回true，否则返回false</returns>
        protected bool AddEntity(T entity)
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
                Debugger.Warn(LogGroupTag.Module, "The entity instance was expired, do add operation failed.");
                return false;
            }

            if (_entities.Contains(entity))
            {
                Debugger.Warn(LogGroupTag.Module, "The entity instance was already added, repeat add it failed.");
                return false;
            }

            // 初始化实体实例
            Call(entity, entity.Initialize, AspectBehaviourType.Initialize);

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
        protected void RemoveEntity(T entity)
        {
            // 2024-04-28:
            // 只有唤醒后且尚未销毁的实体对象，需要关心当前被移除的时候是否处于刷新循环中，
            // 因为只有处于这个生命周期范围内的实体对象才会注册到刷新列表中进行刷新操作，
            // 处于这个生命周期范围之外的实体对象，无需关心当前是否在刷新循环中
            if (IsOnWorkingStatus && entity.IsOnAwakingStatus())
            {
                entity.OnPrepareToDestroy();

                Debugger.Warn(LogGroupTag.Module, "The container instance was updating now, cannot remove any entity at once.");
                return;
            }

            if (false == _entities.Contains(entity))
            {
                Debugger.Warn(LogGroupTag.Module, "Could not found any entity instance in this container, remove it failed.");
                return;
            }

            // 以防万一，先删为敬
            _executeEntitiesList.Remove(entity);
            _updateEntitiesList.Remove(entity);
            _entities.Remove(entity);

            // 调用系统清理回调接口
            CallCleanupForSystem(entity);

            // 清理实体实例
            Call(entity, entity.Cleanup, AspectBehaviourType.Cleanup);
        }

        /// <summary>
        /// 移除当前句柄容器中所有处于过期状态的实体对象实例
        /// </summary>
        protected void RemoveAllExpiredEntities()
        {
            for (int n = _entities.Count - 1; n >= 0; --n)
            {
                T entity = _entities[n];
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
        protected void CallEntityAwakeProcess(T entity)
        {
            if (false == _entities.Contains(entity))
            {
                Debugger.Error(LogGroupTag.Module, "Could not found any entity instance '{%t}' from current instantiation list, wakeup it failed.", entity.BeanType);
                return;
            }

            if (false == entity.IsOnTargetLifecycle(AspectBehaviourType.Startup))
            {
                Debugger.Error(LogGroupTag.Module, "The entity instance '{%t}' was startup incompleted, wakeup it failed.", entity.BeanType);
                // 无效的生命周期，直接终结目标实体对象
                RemoveEntity(entity);
                return;
            }

            // 唤醒实例
            Call(entity, entity.Awake, AspectBehaviourType.Awake);

            BeanController.Instance.RegBeanLifecycleNotification(AspectBehaviourType.Start, entity);
        }

        /// <summary>
        /// 对指定的实体对象实例进行销毁处理<br/>
        /// 未启动完成的对象实例，无需进行销毁处理
        /// </summary>
        /// <param name="entity">实体对象实例</param>
        protected void CallEntityDestroyProcess(T entity)
        {
            if (false == entity.IsOnAwakingStatus())
            {
                Debugger.Warn(LogGroupTag.Module, "The entity instance '{%t}' was startup incompleted, wakeup it failed.", entity.BeanType);
                return;
            }

            BeanController.Instance.UnregBeanLifecycleNotification(entity);

            // 销毁实例
            Call(entity, entity.Destroy, AspectBehaviourType.Destroy);
        }

        /// <summary>
        /// 对指定实体对象实例开启处理的回调函数
        /// </summary>
        /// <param name="entity">实体对象实例</param>
        protected internal void OnEntityStartProcessing(T entity)
        {
            if (false == _entities.Contains(entity))
            {
                Debugger.Error(LogGroupTag.Module, "Could not found any added record of the entity instance '{%t}', calling start process failed.", entity.BeanType);
                return;
            }

            // 开始运行实例
            Call(entity, entity.Start, AspectBehaviourType.Start);

            // 激活执行接口的对象实例，放入到执行队列中
            // if (entity.BeanType.Is<IExecuteActivation>())
            if (entity.IsExecuteActivation())
            {
                _executeEntitiesList.Add(entity);
            }

            // 激活刷新接口的对象实例，放入到刷新队列中
            // if (entity.BeanType.Is<IUpdateActivation>())
            if (entity.IsUpdateActivation())
            {
                _updateEntitiesList.Add(entity);
            }
        }

        /// <summary>
        /// 指定实体对象实例的组件列表发生变更时触发的回调通知接口函数
        /// </summary>
        /// <param name="entity">实体对象实例</param>
        protected internal void OnEntityInternalComponentsChanged(T entity)
        {
            if (false == entity.IsOnStartingStatus() || entity.IsOnDestroyingStatus())
            {
                return;
            }

            if (entity.IsExecuteActivation())
            {
                if (false == _executeEntitiesList.Contains(entity))
                    _executeEntitiesList.Add(entity);
            }
            else
            {
                if (_executeEntitiesList.Contains(entity))
                    _executeEntitiesList.Remove(entity);
            }

            if (entity.IsUpdateActivation())
            {
                if (false == _updateEntitiesList.Contains(entity))
                    _updateEntitiesList.Add(entity);
            }
            else
            {
                if (_updateEntitiesList.Contains(entity))
                    _updateEntitiesList.Remove(entity);
            }
        }

        /// <summary>
        /// 执行当前句柄容器中记录的所有实体对象实例
        /// </summary>
        protected void OnEntitiesExecute()
        {
            for (int n = 0; n < _executeEntitiesList.Count; ++n)
            {
                T entity = _executeEntitiesList[n];
                // 过期对象跳过该操作
                if (entity.IsOnExpired)
                {
                    continue;
                }

                // 对象执行操作
                Call(entity, entity.Execute, AspectBehaviourType.Execute);
            }
        }

        /// <summary>
        /// 后置执行当前句柄容器中记录的所有实体对象实例
        /// </summary>
        protected void OnEntitiesLateExecute()
        {
            for (int n = 0; n < _executeEntitiesList.Count; ++n)
            {
                T entity = _executeEntitiesList[n];
                // 过期对象跳过该操作
                if (entity.IsOnExpired)
                {
                    continue;
                }

                // 对象后置执行操作
                Call(entity, entity.LateExecute, AspectBehaviourType.LateExecute);
            }
        }

        /// <summary>
        /// 刷新当前句柄容器中记录的所有实体对象实例
        /// </summary>
        protected void OnEntitiesUpdate()
        {
            for (int n = 0; n < _updateEntitiesList.Count; ++n)
            {
                T entity = _updateEntitiesList[n];
                // 过期对象跳过该操作
                if (entity.IsOnExpired)
                {
                    continue;
                }

                // 对象刷新操作
                Call(entity, entity.Update, AspectBehaviourType.Update);

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
                T entity = _updateEntitiesList[n];
                // 过期对象跳过该操作
                if (entity.IsOnExpired)
                {
                    continue;
                }

                // 对象后置刷新操作
                Call(entity, entity.LateUpdate, AspectBehaviourType.LateUpdate);

                // 调用系统后置刷新回调接口
                CallLateUpdateForSystem(entity);
            }
        }

        #endregion
    }
}
