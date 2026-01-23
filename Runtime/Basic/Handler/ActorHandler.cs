/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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

using UnityGameObject = UnityEngine.GameObject;

namespace GameEngine
{
    /// <summary>
    /// 角色模块封装的句柄对象类
    /// 模块具体功能接口请参考<see cref="NovaEngine.Module.ActorModule"/>类
    /// </summary>
    public sealed partial class ActorHandler : GenericEntityHandler<CActor>
    {
        /// <summary>
        /// 句柄对象的单例访问获取接口
        /// </summary>
        public static ActorHandler Instance => HandlerManagement.ActorHandler;

        /// <summary>
        /// 句柄对象内置初始化接口函数
        /// </summary>
        /// <returns>若句柄对象初始化成功则返回true，否则返回false</returns>
        protected override bool OnInitialize()
        {
            if (false == base.OnInitialize()) return false;

            return true;
        }

        /// <summary>
        /// 句柄对象内置清理接口函数
        /// </summary>
        protected override void OnCleanup()
        {
            // 移除全部对象实例
            RemoveAllActors();

            // 清理对象类型注册列表
            UnregisterAllActorClasses();

            base.OnCleanup();
        }

        /// <summary>
        /// 句柄对象内置重载接口函数
        /// </summary>
        protected override void OnReload()
        {
            base.OnReload();
        }

        /// <summary>
        /// 句柄对象内置执行接口
        /// </summary>
        protected override void OnExecute()
        {
            base.OnExecute();
        }

        /// <summary>
        /// 句柄对象内置延迟执行接口
        /// </summary>
        protected override void OnLateExecute()
        {
            base.OnLateExecute();
        }

        /// <summary>
        /// 句柄对象内置刷新接口
        /// </summary>
        protected override void OnUpdate()
        {
            base.OnUpdate();
        }

        /// <summary>
        /// 句柄对象内置延迟刷新接口
        /// </summary>
        protected override void OnLateUpdate()
        {
            base.OnLateUpdate();
        }

        /// <summary>
        /// 句柄对象的模块事件转发回调接口
        /// </summary>
        /// <param name="e">模块事件参数</param>
        public override void OnEventDispatch(NovaEngine.Module.ModuleEventArgs e)
        {
        }

        #region 角色对象类型注册绑定相关的接口函数

        /// <summary>
        /// 注册指定的角色名称及对应的角色类到当前的句柄管理容器中
        /// 注意，注册的角色类必须继承自<see cref="GameEngine.CActor"/>，否则无法正常注册
        /// </summary>
        /// <param name="actorName">对象名称</param>
        /// <param name="clsType">对象类型</param>
        /// <param name="priority">对象优先级</param>
        /// <returns>若对象类型注册成功则返回true，否则返回false</returns>
        private bool RegisterActorClass(string actorName, Type clsType, int priority)
        {
            Debugger.Assert(false == string.IsNullOrEmpty(actorName) && null != clsType, NovaEngine.ErrorText.InvalidArguments);

            if (false == clsType.Is<CActor>())
            {
                Debugger.Warn(LogGroupTag.Module, "The register type '{%t}' must be inherited from 'CActor'.", clsType);
                return false;
            }

            if (false == RegisterEntityClass(actorName, clsType, priority))
            {
                Debugger.Warn(LogGroupTag.Module, "The scene class '{%t}' was already registered, repeat added it failed.", clsType);
                return false;
            }

            // Debugger.Info(LogGroupTag.Module, "Register new actor class type '{%t}' with target name '{%s}'.", clsType, actorName);

            return true;
        }

        /// <summary>
        /// 注销当前句柄实例绑定的所有角色类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UnregisterAllActorClasses()
        {
            UnregisterAllEntityClasses();
        }

        #endregion

        #region 角色对象实例创建/销毁管理相关的操作函数合集

        /// <summary>
        /// 通过指定的角色名称从实例容器中获取对应的角色对象实例列表
        /// </summary>
        /// <param name="actorName">对象名称</param>
        /// <returns>返回角色对象实例列表，若检索失败则返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyList<CActor> GetActor(string actorName)
        {
            if (_entityClassTypes.TryGetValue(actorName, out Type actorType))
            {
                return GetActor(actorType);
            }

            return null;
        }

        /// <summary>
        /// 通过指定的角色类型从实例容器中获取对应的角色对象实例列表
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回角色对象实例列表，若检索失败则返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyList<T> GetActor<T>() where T : CActor
        {
            return NovaEngine.Utility.Collection.CastAndToReadOnlyList<CActor, T>(GetActor(typeof(T)));
        }

        /// <summary>
        /// 通过指定的角色类型从实例容器中获取对应的角色对象实例列表
        /// </summary>
        /// <param name="actorType">对象类型</param>
        /// <returns>返回角色对象实例列表，若检索失败则返回null</returns>
        public IReadOnlyList<CActor> GetActor(Type actorType)
        {
            List<CActor> actors = null;
            for (int n = 0; n < Entities.Count; ++n)
            {
                CActor actor = Entities[n];
                if (actor.BeanType == actorType)
                {
                    if (null == actors) actors = new List<CActor>();

                    actors.Add(actor);
                }
            }

            return actors;
        }

        /// <summary>
        /// 获取当前已创建的全部角色对象实例
        /// </summary>
        /// <returns>返回已创建的全部角色对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyList<CActor> GetAllActors()
        {
            return GetAllEntities();
        }

        /// <summary>
        /// 检测当前已创建的角色对象列表中是否存在指定标识的对象实例
        /// </summary>
        /// <param name="beanId">实体标识</param>
        /// <returns>若存在指定标识的角色对象实例则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasActorById(int beanId)
        {
            return HasEntityById(beanId);
        }

        /// <summary>
        /// 通过指定的对象标识查找对应的角色对象实例
        /// </summary>
        /// <param name="beanId">实体标识</param>
        /// <returns>返回对应的角色对象实例，若该实例不存在则返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CActor GetActorById(int beanId)
        {
            return GetEntityById(beanId);
        }

        /// <summary>
        /// 通过指定的角色名称动态创建一个对应的角色对象实例
        /// </summary>
        /// <param name="actorName">对象名称</param>
        /// <param name="userData">用户数据</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CActor CreateActor(string actorName, object userData = null)
        {
            if (false == _entityClassTypes.TryGetValue(actorName, out Type actorType))
            {
                Debugger.Warn(LogGroupTag.Module, "Could not found any correct actor class with target name '{%s}', created actor failed.", actorName);
                return null;
            }

            return CreateActor(actorType, userData);
        }

        /// <summary>
        /// 通过指定的角色类型动态创建一个对应的角色对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="userData">用户数据</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CreateActor<T>(object userData = null) where T : CActor
        {
            return CreateActor(typeof(T), userData) as T;
        }

        /// <summary>
        /// 通过指定的角色类型动态创建一个对应的角色对象实例
        /// </summary>
        /// <param name="actorType">对象类型</param>
        /// <param name="userData">用户数据</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CActor CreateActor(Type actorType, object userData = null)
        {
            return CreateActor(actorType, null, userData);
        }

        /// <summary>
        /// 通过指定的角色类型动态创建一个对应的角色对象实例
        /// </summary>
        /// <param name="actorType">对象类型</param>
        /// <param name="beanName">实体名称</param>
        /// <param name="userData">用户数据</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        internal CActor CreateActor(Type actorType, string beanName, object userData = null)
        {
            Debugger.Assert(actorType, NovaEngine.ErrorText.InvalidArguments);
            if (false == _entityClassTypes.Values.Contains(actorType))
            {
                Debugger.Error(LogGroupTag.Module, "Could not found any correct actor class with target type '{%t}', created actor failed.", actorType);
                return null;
            }

            // 对象实例化
            CActor obj = CreateInstance(actorType) as CActor;
            obj.BeanName = beanName;
            obj.UserData = userData;

            if (false == AddEntity(obj))
            {
                Debugger.Warn(LogGroupTag.Module, "The actor instance '{%t}' initialization for error, added it failed.", actorType);
                return null;
            }

            _Profiler.CallStat(Profiler.Statistics.StatCode.ActorCreate, obj);

            // 启动对象实例
            Call(obj, obj.Startup, AspectBehaviourType.Startup);

            // 唤醒对象实例
            CallEntityAwakeProcess(obj);

            return obj;
        }

        /// <summary>
        /// 从当前角色管理容器中移除指定的角色对象实例
        /// </summary>
        /// <param name="actor">对象实例</param>
        internal void RemoveActor(CActor actor)
        {
            if (false == Entities.Contains(actor))
            {
                Debugger.Warn(LogGroupTag.Module, "Could not found target actor instance '{%t}' from current container, removed it failed.", actor.BeanType);
                return;
            }

            // 销毁角色对象实例
            CallEntityDestroyProcess(actor);

            // 关闭角色对象实例
            Call(actor, actor.Shutdown, AspectBehaviourType.Shutdown);

            _Profiler.CallStat(Profiler.Statistics.StatCode.ActorRelease, actor);

            // 移除实例
            RemoveEntity(actor);

            // 回收对象实例
            ReleaseInstance(actor);
        }

        /// <summary>
        /// 从当前角色管理容器中移除指定名称对应的所有角色对象实例
        /// </summary>
        /// <param name="actorName">对象名称</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RemoveActor(string actorName)
        {
            if (_entityClassTypes.TryGetValue(actorName, out Type actorType))
            {
                RemoveActor(actorType);
            }
        }

        /// <summary>
        /// 从当前角色管理容器中移除指定类型对应的所有角色对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RemoveActor<T>() where T : CActor
        {
            RemoveActor(typeof(T));
        }

        /// <summary>
        /// 从当前角色管理容器中移除指定类型对应的所有角色对象实例
        /// </summary>
        /// <param name="actorType">对象类型</param>
        internal void RemoveActor(Type actorType)
        {
            IEnumerable<CActor> actors = NovaEngine.Utility.Collection.Reverse(Entities);
            foreach (CActor obj in actors)
            {
                if (obj.BeanType == actorType)
                {
                    RemoveActor(obj);
                }
            }
        }

        /// <summary>
        /// 从当前角色管理容器中移除所有注册的角色对象实例
        /// </summary>
        internal void RemoveAllActors()
        {
            while (Entities.Count > 0)
            {
                RemoveActor(Entities[Entities.Count - 1]);
            }
        }

        /// <summary>
        /// 从当前角色管理容器中销毁指定的角色对象实例
        /// </summary>
        /// <param name="actor">对象实例</param>
        public void DestroyActor(CActor actor)
        {
            if (false == Entities.Contains(actor))
            {
                Debugger.Warn("Could not found target actor instance '{%t}' from current container, removed it failed.", actor.BeanType);
                return;
            }

            // 刷新状态时推到销毁队列中
            // if ( /* false == obj.IsOnSchedulingProcessForTargetLifecycle(CBase.LifecycleKeypointType.Start) || */ obj.IsOnUpdatingStatus())
            if (actor.IsCurrentLifecycleTypeRunning)
            {
                actor.OnPrepareToDestroy();
                return;
            }

            // 在非逻辑刷新的状态下，直接移除对象即可
            RemoveActor(actor);
        }

        /// <summary>
        /// 从当前角色管理容器中销毁指定名称对应的所有角色对象实例
        /// </summary>
        /// <param name="actorName">对象名称</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DestroyActor(string actorName)
        {
            if (_entityClassTypes.TryGetValue(actorName, out Type actorType))
            {
                DestroyActor(actorType);
            }
        }

        /// <summary>
        /// 从当前角色管理容器中销毁指定类型对应的所有角色对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DestroyActor<T>() where T : CActor
        {
            Type actorType = typeof(T);

            DestroyActor(actorType);
        }

        /// <summary>
        /// 从当前角色管理容器中销毁指定类型对应的所有角色对象实例
        /// </summary>
        /// <param name="actorType">对象类型</param>
        public void DestroyActor(Type actorType)
        {
            IEnumerable<CActor> actors = NovaEngine.Utility.Collection.Reverse(Entities);
            foreach (CActor obj in actors)
            {
                if (obj.BeanType == actorType)
                {
                    DestroyActor(obj);
                }
            }
        }

        /// <summary>
        /// 从当前角色管理容器中销毁所有注册的角色对象实例
        /// </summary>
        public void DestroyAllActors()
        {
            while (Entities.Count > 0)
            {
                DestroyActor(Entities[Entities.Count - 1]);
            }
        }

        #endregion

        #region 角色对象实例检索查询相关的操作函数合集

        /// <summary>
        /// 同步实例化对象
        /// </summary>
        /// <param name="url">资源地址(名字或路径)</param>
        public UnityGameObject Instantiate(string url)
        {
            return ResourceModule.InstantiateObject(url);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 通过指定的角色类型获取对应角色的名称
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回对应角色的名称，若角色不存在则返回null</returns>
        internal string GetActorNameForType<T>() where T : CActor
        {
            return GetActorNameForType(typeof(T));
        }

        /// <summary>
        /// 通过指定的角色类型获取对应角色的名称
        /// </summary>
        /// <param name="actorType">对象类型</param>
        /// <returns>返回对应角色的名称，若角色不存在则返回null</returns>
        internal string GetActorNameForType(Type actorType)
        {
            foreach (KeyValuePair<string, Type> pair in _entityClassTypes)
            {
                if (pair.Value == actorType)
                {
                    return pair.Key;
                }
            }

            return null;
        }

        /// <summary>
        /// 通过指定的角色对象类型，搜索该类型的全部实例<br/>
        /// 返回的实例列表中，包括了该类型及其子类的全部对象实例
        /// </summary>
        /// <param name="actorType">对象类型</param>
        /// <returns>返回给定类型的全部实例</returns>
        internal IReadOnlyList<CActor> FindAllActorsByType(Type actorType)
        {
            List<CActor> result = null;
            for (int n = 0; n < Entities.Count; ++n)
            {
                CActor obj = Entities[n];
                if (obj.BeanType.Is(actorType))
                {
                    if (null == result) result = new List<CActor>();

                    result.Add(obj);
                }
            }

            return result;
        }

        #endregion
    }
}
