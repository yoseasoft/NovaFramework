/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

using UnityGameObject = UnityEngine.GameObject;

namespace GameEngine
{
    /// <summary>
    /// 角色模块封装的句柄对象类
    /// 模块具体功能接口请参考<see cref="NovaEngine.ActorModule"/>类
    /// </summary>
    public sealed partial class ActorHandler : EntityHandler
    {
        /// <summary>
        /// 角色对象类型映射注册管理容器
        /// </summary>
        private readonly IDictionary<string, SystemType> _actorClassTypes;
        /// <summary>
        /// 角色优先级映射注册管理容器
        /// </summary>
        private readonly IDictionary<string, int> _actorPriorities;

        /// <summary>
        /// 通过当前模块实例化的对象实例管理容器
        /// </summary>
        private readonly IList<CActor> _actors;

        /// <summary>
        /// 句柄对象的单例访问获取接口
        /// </summary>
        public static ActorHandler Instance => HandlerManagement.ActorHandler;

        /// <summary>
        /// 句柄对象默认构造函数
        /// </summary>
        public ActorHandler()
        {
            // 初始化对象类注册容器
            _actorClassTypes = new Dictionary<string, SystemType>();
            _actorPriorities = new Dictionary<string, int>();
            _actors = new List<CActor>();
        }

        /// <summary>
        /// 句柄对象析构函数
        /// </summary>
        ~ActorHandler()
        {
            // 清理对象类注册容器
            _actorClassTypes.Clear();
            _actorPriorities.Clear();
            _actors.Clear();
        }

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
        public override void OnEventDispatch(NovaEngine.ModuleEventArgs e)
        {
        }

        #region 角色类注册/注销接口函数

        /// <summary>
        /// 注册指定的角色名称及对应的角色类到当前的句柄管理容器中
        /// 注意，注册的角色类必须继承自<see cref="GameEngine.CActor"/>，否则无法正常注册
        /// </summary>
        /// <param name="actorName">对象名称</param>
        /// <param name="clsType">对象类型</param>
        /// <param name="priority">对象优先级</param>
        /// <returns>若对象类型注册成功则返回true，否则返回false</returns>
        private bool RegisterActorClass(string actorName, SystemType clsType, int priority)
        {
            Debugger.Assert(false == string.IsNullOrEmpty(actorName) && null != clsType, "Invalid arguments");

            if (false == typeof(CActor).IsAssignableFrom(clsType))
            {
                Debugger.Warn("The register type {0} must be inherited from 'CActor'.", clsType.Name);
                return false;
            }

            if (_actorClassTypes.ContainsKey(actorName))
            {
                Debugger.Warn("The actor name {0} was already registed, repeat add will be override old name.", actorName);
                _actorClassTypes.Remove(actorName);
            }

            _actorClassTypes.Add(actorName, clsType);
            if (priority > 0)
            {
                _actorPriorities.Add(actorName, priority);
            }

            return true;
        }

        /// <summary>
        /// 注销当前句柄实例绑定的所有角色类型
        /// </summary>
        private void UnregisterAllActorClasses()
        {
            _actorClassTypes.Clear();
            _actorPriorities.Clear();
        }

        #endregion

        #region 角色对象实例访问操作函数合集

        /// <summary>
        /// 通过指定的角色名称从实例容器中获取对应的角色对象实例列表
        /// </summary>
        /// <param name="actorName">对象名称</param>
        /// <returns>返回角色对象实例列表，若检索失败则返回null</returns>
        public IList<CActor> GetActor(string actorName)
        {
            SystemType actorType = null;
            if (_actorClassTypes.TryGetValue(actorName, out actorType))
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
        public IList<T> GetActor<T>() where T : CActor
        {
            SystemType actorType = typeof(T);

            return NovaEngine.Utility.Collection.CastAndToList<CActor, T>(GetActor(actorType));
        }

        /// <summary>
        /// 通过指定的角色类型从实例容器中获取对应的角色对象实例列表
        /// </summary>
        /// <param name="actorType">对象类型</param>
        /// <returns>返回角色对象实例列表，若检索失败则返回null</returns>
        public IList<CActor> GetActor(SystemType actorType)
        {
            List<CActor> actors = new List<CActor>();
            for (int n = 0; n < _actors.Count; ++n)
            {
                CActor actor = _actors[n];
                if (actor.GetType() == actorType)
                {
                    actors.Add(actor);
                }
            }

            if (actors.Count <= 0)
            {
                actors = null;
            }

            return actors;
        }

        /// <summary>
        /// 获取当前已创建的全部角色对象实例
        /// </summary>
        /// <returns>返回已创建的全部角色对象实例</returns>
        public IList<CActor> GetAllActors()
        {
            return _actors;
        }

        /// <summary>
        /// 通过指定的角色名称动态创建一个对应的角色对象实例
        /// </summary>
        /// <param name="actorName">对象名称</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        public CActor CreateActor(string actorName)
        {
            SystemType actorType = null;
            if (false == _actorClassTypes.TryGetValue(actorName, out actorType))
            {
                Debugger.Warn("Could not found any correct actor class with target name '{0}', created actor failed.", actorName);
                return null;
            }

            return CreateActor(actorType);
        }

        /// <summary>
        /// 通过指定的角色类型动态创建一个对应的角色对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        public T CreateActor<T>() where T : CActor
        {
            SystemType actorType = typeof(T);

            return CreateActor(actorType) as T;
        }

        /// <summary>
        /// 通过指定的角色类型动态创建一个对应的角色对象实例
        /// </summary>
        /// <param name="actorType">对象类型</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        public CActor CreateActor(SystemType actorType)
        {
            Debugger.Assert(null != actorType, "Invalid arguments.");
            if (false == _actorClassTypes.Values.Contains(actorType))
            {
                Debugger.Error("Could not found any correct actor class with target type '{0}', created actor failed.", actorType.FullName);
                return null;
            }

            // 对象实例化
            CActor obj = CreateInstance(actorType) as CActor;
            if (false == AddEntity(obj))
            {
                Debugger.Warn("The actor instance '{0}' initialization for error, added it failed.", actorType.FullName);
                return null;
            }

            // 添加实例到管理容器中
            _actors.Add(obj);

            // 启动对象实例
            Call(obj.Startup);

            // 唤醒对象实例
            CallEntityAwakeProcess(obj);

            _Profiler.CallStat(Profiler.Statistics.StatCode.ActorCreate, obj);

            return obj;
        }

        /// <summary>
        /// 从当前角色管理容器中移除指定的角色对象实例
        /// </summary>
        /// <param name="actor">对象实例</param>
        internal void RemoveActor(CActor actor)
        {
            if (false == _actors.Contains(actor))
            {
                Debugger.Warn("Could not found target actor instance '{0}' from current container, removed it failed.", actor.GetType().FullName);
                return;
            }

            _Profiler.CallStat(Profiler.Statistics.StatCode.ActorRelease, actor);

            // 销毁角色对象实例
            CallEntityDestroyProcess(actor);

            // 关闭角色对象实例
            Call(actor.Shutdown);

            // 从管理容器中移除实例
            _actors.Remove(actor);

            // 移除实例
            RemoveEntity(actor);

            // 回收对象实例
            ReleaseInstance(actor);
        }

        /// <summary>
        /// 从当前角色管理容器中移除指定名称对应的所有角色对象实例
        /// </summary>
        /// <param name="actorName">对象名称</param>
        internal void RemoveActor(string actorName)
        {
            SystemType actorType = null;
            if (_actorClassTypes.TryGetValue(actorName, out actorType))
            {
                RemoveActor(actorType);
            }
        }

        /// <summary>
        /// 从当前角色管理容器中移除指定类型对应的所有角色对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        internal void RemoveActor<T>() where T : CActor
        {
            SystemType actorType = typeof(T);

            RemoveActor(actorType);
        }

        /// <summary>
        /// 从当前角色管理容器中移除指定类型对应的所有角色对象实例
        /// </summary>
        /// <param name="actorType">对象类型</param>
        internal void RemoveActor(SystemType actorType)
        {
            IEnumerable<CActor> actors = NovaEngine.Utility.Collection.Reverse<CActor>(_actors);
            foreach (CActor obj in actors)
            {
                if (obj.GetType() == actorType)
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
            while (_actors.Count > 0)
            {
                RemoveActor(_actors[_actors.Count - 1]);
            }
        }

        /// <summary>
        /// 从当前角色管理容器中销毁指定的角色对象实例
        /// </summary>
        /// <param name="actor">对象实例</param>
        public void DestroyActor(CActor actor)
        {
            if (false == _actors.Contains(actor))
            {
                Debugger.Warn("Could not found target actor instance '{0}' from current container, removed it failed.", actor.GetType().FullName);
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
        public void DestroyActor(string actorName)
        {
            SystemType actorType = null;
            if (_actorClassTypes.TryGetValue(actorName, out actorType))
            {
                DestroyActor(actorType);
            }
        }

        /// <summary>
        /// 从当前角色管理容器中销毁指定类型对应的所有角色对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        public void DestroyActor<T>() where T : CActor
        {
            SystemType actorType = typeof(T);

            DestroyActor(actorType);
        }

        /// <summary>
        /// 从当前角色管理容器中销毁指定类型对应的所有角色对象实例
        /// </summary>
        /// <param name="actorType">对象类型</param>
        public void DestroyActor(SystemType actorType)
        {
            IEnumerable<CActor> actors = NovaEngine.Utility.Collection.Reverse<CActor>(_actors);
            foreach (CActor obj in actors)
            {
                if (obj.GetType() == actorType)
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
            while (_actors.Count > 0)
            {
                DestroyActor(_actors[_actors.Count - 1]);
            }
        }

        #endregion

        #region 角色对象扩展操作函数合集

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
        internal string GetActorNameForType(SystemType actorType)
        {
            foreach (KeyValuePair<string, SystemType> pair in _actorClassTypes)
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
        internal IList<CActor> FindAllActorsByType(SystemType actorType)
        {
            IList<CActor> result = new List<CActor>();
            IEnumerator<CActor> e = _actors.GetEnumerator();
            while (e.MoveNext())
            {
                CActor obj = e.Current;
                if (actorType.IsAssignableFrom(obj.GetType()))
                {
                    result.Add(obj);
                }
            }

            // 如果搜索结果为空，则直接返回null
            if (result.Count <= 0)
            {
                result = null;
            }

            return result;
        }

        #endregion
    }
}
