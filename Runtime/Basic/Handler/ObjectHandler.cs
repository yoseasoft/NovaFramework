/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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

using System.Collections.Generic;

using SystemType = System.Type;

namespace GameEngine
{
    /// <summary>
    /// 对象模块封装的句柄对象类
    /// 模块具体功能接口请参考<see cref="NovaEngine.Module.ObjectModule"/>类
    /// </summary>
    public sealed partial class ObjectHandler : BaseHandler
    {
        /// <summary>
        /// 基础对象类型映射注册管理容器
        /// </summary>
        private readonly IDictionary<string, SystemType> _objectClassTypes;
        /// <summary>
        /// 对象优先级映射注册管理容器
        /// </summary>
        private readonly IDictionary<string, int> _objectPriorities;

        /// <summary>
        /// 通过当前模块实例化的对象实例管理容器
        /// </summary>
        private readonly IList<CObject> _objects;
        /// <summary>
        /// 基础对象执行列表容器
        /// </summary>
        private IList<CObject> _objectExecuteList;
        /// <summary>
        /// 基础对象刷新列表容器
        /// </summary>
        private IList<CObject> _objectUpdateList;

        /// <summary>
        /// 句柄对象的单例访问获取接口
        /// </summary>
        public static ObjectHandler Instance => HandlerManagement.ObjectHandler;

        /// <summary>
        /// 句柄对象默认构造函数
        /// </summary>
        public ObjectHandler()
        {
            // 初始化对象类注册容器
            _objectClassTypes = new Dictionary<string, SystemType>();
            _objectPriorities = new Dictionary<string, int>();
            _objects = new List<CObject>();
            // 对象执行列表初始化
            _objectExecuteList = new List<CObject>();
            // 对象刷新列表初始化
            _objectUpdateList = new List<CObject>();
        }

        /// <summary>
        /// 句柄对象析构函数
        /// </summary>
        ~ObjectHandler()
        {
            // 清理对象类注册容器
            _objectClassTypes.Clear();
            _objectPriorities.Clear();
            _objects.Clear();

            // 清理对象执行列表容器
            _objectExecuteList.Clear();
            _objectExecuteList = null;

            // 清理对象刷新列表容器
            _objectUpdateList.Clear();
            _objectUpdateList = null;
        }

        /// <summary>
        /// 句柄对象内置初始化接口函数
        /// </summary>
        /// <returns>若句柄对象初始化成功则返回true，否则返回false</returns>
        protected override bool OnInitialize()
        {
            return true;
        }

        /// <summary>
        /// 句柄对象内置清理接口函数
        /// </summary>
        protected override void OnCleanup()
        {
            RemoveAllObjects();
        }

        /// <summary>
        /// 句柄对象内置重载接口函数
        /// </summary>
        protected override void OnReload()
        {
        }

        /// <summary>
        /// 句柄对象内置执行接口
        /// </summary>
        protected override void OnExecute()
        {
        }

        /// <summary>
        /// 句柄对象内置延迟执行接口
        /// </summary>
        protected override void OnLateExecute()
        {
        }

        /// <summary>
        /// 句柄对象内置刷新接口
        /// </summary>
        protected override void OnUpdate()
        {
        }

        /// <summary>
        /// 句柄对象内置延迟刷新接口
        /// </summary>
        protected override void OnLateUpdate()
        {
        }

        /// <summary>
        /// 句柄对象的模块事件转发回调接口
        /// </summary>
        /// <param name="e">模块事件参数</param>
        public override void OnEventDispatch(NovaEngine.Module.ModuleEventArgs e)
        {
        }

        #region 对象类注册/注销接口函数

        /// <summary>
        /// 注册指定的对象名称及对应的对象类到当前的句柄管理容器中
        /// 注意，注册的对象类必须继承自<see cref="GameEngine.CObject"/>，否则无法正常注册
        /// </summary>
        /// <param name="objectName">对象名称</param>
        /// <param name="clsType">对象类型</param>
        /// <param name="priority">优先级</param>
        /// <returns>若对象类型注册成功则返回true，否则返回false</returns>
        private bool RegisterObjectClass(string objectName, SystemType clsType, int priority)
        {
            Debugger.Assert(false == string.IsNullOrEmpty(objectName) && null != clsType, NovaEngine.ErrorText.InvalidArguments);

            if (false == typeof(CObject).IsAssignableFrom(clsType))
            {
                Debugger.Warn("The register type {0} must be inherited from 'CObject'.", clsType.Name);
                return false;
            }

            if (_objectClassTypes.ContainsKey(objectName))
            {
                Debugger.Warn("The object name {0} was already registed, repeat add will be override old name.", objectName);
                _objectClassTypes.Remove(objectName);
            }

            _objectClassTypes.Add(objectName, clsType);
            if (priority > 0)
            {
                _objectPriorities.Add(objectName, priority);
            }

            return true;
        }

        /// <summary>
        /// 注销当前句柄实例绑定的所有对象类型
        /// </summary>
        private void UnregisterAllObjectClasses()
        {
            _objectClassTypes.Clear();
            _objectPriorities.Clear();
        }

        #endregion

        #region 基础对象实例访问操作函数合集

        /// <summary>
        /// 通过指定的对象名称从实例容器中获取对应的基础对象实例列表
        /// </summary>
        /// <param name="objectName">对象名称</param>
        /// <returns>返回基础对象实例列表，若检索失败则返回null</returns>
        public IList<CObject> GetObject(string objectName)
        {
            SystemType objectType = null;
            if (_objectClassTypes.TryGetValue(objectName, out objectType))
            {
                return GetObject(objectType);
            }

            return null;
        }

        /// <summary>
        /// 通过指定的对象类型从实例容器中获取对应的基础对象实例列表
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回基础对象实例列表，若检索失败则返回null</returns>
        public IList<T> GetObject<T>() where T : CObject
        {
            SystemType objectType = typeof(T);

            return NovaEngine.Utility.Collection.CastAndToList<CObject, T>(GetObject(objectType));
        }

        /// <summary>
        /// 通过指定的对象类型从实例容器中获取对应的基础对象实例列表
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>返回基础对象实例列表，若检索失败则返回null</returns>
        public IList<CObject> GetObject(SystemType objectType)
        {
            List<CObject> objects = new List<CObject>();
            for (int n = 0; n < _objects.Count; ++n)
            {
                CObject actor = _objects[n];
                if (actor.BeanType == objectType)
                {
                    objects.Add(actor);
                }
            }

            if (objects.Count <= 0)
            {
                objects = null;
            }

            return objects;
        }

        /// <summary>
        /// 获取当前已创建的全部基础对象实例
        /// </summary>
        /// <returns>返回已创建的全部基础对象实例</returns>
        public IList<CObject> GetAllObjects()
        {
            return _objects;
        }

        /// <summary>
        /// 通过指定的对象名称动态创建一个对应的基础对象实例
        /// </summary>
        /// <param name="objectName">对象名称</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        public CObject CreateObject(string objectName)
        {
            SystemType objectType = null;
            if (false == _objectClassTypes.TryGetValue(objectName, out objectType))
            {
                Debugger.Warn("Could not found any correct object class with target name '{0}', created object failed.", objectName);
                return null;
            }

            return CreateObject(objectType);
        }

        /// <summary>
        /// 通过指定的对象类型动态创建一个对应的基础对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        public T CreateObject<T>() where T : CObject
        {
            SystemType objectType = typeof(T);

            return CreateObject(objectType) as T;
        }

        /// <summary>
        /// 通过指定的对象类型动态创建一个对应的基础对象实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        public CObject CreateObject(SystemType objectType)
        {
            Debugger.Assert(objectType, NovaEngine.ErrorText.InvalidArguments);
            if (false == _objectClassTypes.Values.Contains(objectType))
            {
                Debugger.Error("Could not found any correct object class with target type '{0}', created object failed.", objectType.FullName);
                return null;
            }

            // 对象实例化
            CObject obj = CreateInstance(objectType) as CObject;

            // 初始化实体实例
            Call(obj, obj.Initialize, AspectBehaviourType.Initialize);

            // 添加实例到管理容器中
            _objects.Add(obj);

            // 启动对象实例
            Call(obj, obj.Startup, AspectBehaviourType.Startup);

            // 唤醒实例
            Call(obj, obj.Awake, AspectBehaviourType.Awake);

            BeanController.Instance.RegBeanLifecycleNotification(AspectBehaviourType.Start, obj);

            _Profiler.CallStat(Profiler.Statistics.StatCode.ObjectCreate, obj);

            return obj;
        }

        /// <summary>
        /// 从当前对象管理容器中移除指定的基础对象实例
        /// </summary>
        /// <param name="obj">对象实例</param>
        internal void RemoveObject(CObject obj)
        {
            if (false == _objects.Contains(obj))
            {
                Debugger.Warn("Could not found target object instance '{%t}' from current container, removed it failed.", obj.BeanType);
                return;
            }

            _Profiler.CallStat(Profiler.Statistics.StatCode.ObjectRelease, obj);

            BeanController.Instance.UnregBeanLifecycleNotification(obj);

            // 销毁实例
            Call(obj, obj.Destroy, AspectBehaviourType.Destroy);

            // 关闭基础对象实例
            Call(obj, obj.Shutdown, AspectBehaviourType.Shutdown);

            // 从管理容器中移除实例
            _objectExecuteList.Remove(obj);
            _objectUpdateList.Remove(obj);
            _objects.Remove(obj);

            // 移除实例
            Call(obj, obj.Cleanup, AspectBehaviourType.Cleanup);

            // 回收对象实例
            ReleaseInstance(obj);
        }

        /// <summary>
        /// 从当前对象管理容器中移除指定名称对应的所有基础对象实例
        /// </summary>
        /// <param name="objectName">对象名称</param>
        internal void RemoveObject(string objectName)
        {
            SystemType objectType = null;
            if (_objectClassTypes.TryGetValue(objectName, out objectType))
            {
                RemoveObject(objectType);
            }
        }

        /// <summary>
        /// 从当前对象管理容器中移除指定类型对应的所有基础对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        internal void RemoveObject<T>() where T : CObject
        {
            SystemType objectType = typeof(T);

            RemoveObject(objectType);
        }

        /// <summary>
        /// 从当前对象管理容器中移除指定类型对应的所有基础对象实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        internal void RemoveObject(SystemType objectType)
        {
            IEnumerable<CObject> objects = NovaEngine.Utility.Collection.Reverse<CObject>(_objects);
            foreach (CObject obj in objects)
            {
                if (obj.BeanType == objectType)
                {
                    RemoveObject(obj);
                }
            }
        }

        /// <summary>
        /// 从当前对象管理容器中移除所有注册的基础对象实例
        /// </summary>
        internal void RemoveAllObjects()
        {
            while (_objects.Count > 0)
            {
                RemoveObject(_objects[_objects.Count - 1]);
            }
        }

        /// <summary>
        /// 从当前对象管理容器中销毁指定的基础对象实例
        /// </summary>
        /// <param name="obj">对象实例</param>
        public void DestroyObject(CObject obj)
        {
            if (false == _objects.Contains(obj))
            {
                Debugger.Warn("Could not found target object instance '{%t}' from current container, removed it failed.", obj.BeanType);
                return;
            }

            // 刷新状态时推到销毁队列中
            // if ( /* false == obj.IsOnSchedulingProcessForTargetLifecycle(CBase.LifecycleKeypointType.Start) || */ obj.IsOnUpdatingStatus())
            if (obj.IsCurrentLifecycleTypeRunning)
            {
                obj.OnPrepareToDestroy();
                return;
            }

            // 在非逻辑刷新的状态下，直接移除对象即可
            RemoveObject(obj);
        }

        /// <summary>
        /// 从当前对象管理容器中销毁指定名称对应的所有基础对象实例
        /// </summary>
        /// <param name="objectName">对象名称</param>
        public void DestroyObject(string objectName)
        {
            SystemType objectType = null;
            if (_objectClassTypes.TryGetValue(objectName, out objectType))
            {
                DestroyObject(objectType);
            }
        }

        /// <summary>
        /// 从当前对象管理容器中销毁指定类型对应的所有基础对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        public void DestroyObject<T>() where T : CObject
        {
            SystemType objectType = typeof(T);

            DestroyObject(objectType);
        }

        /// <summary>
        /// 从当前对象管理容器中销毁指定类型对应的所有基础对象实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        public void DestroyObject(SystemType objectType)
        {
            IEnumerable<CObject> objects = NovaEngine.Utility.Collection.Reverse<CObject>(_objects);
            foreach (CObject obj in objects)
            {
                if (obj.BeanType == objectType)
                {
                    DestroyObject(obj);
                }
            }
        }

        /// <summary>
        /// 从当前对象管理容器中销毁所有注册的基础对象实例
        /// </summary>
        public void DestroyAllObjects()
        {
            while (_objects.Count > 0)
            {
                DestroyObject(_objects[_objects.Count - 1]);
            }
        }

        /// <summary>
        /// 对指定基础对象实例开启处理的回调函数
        /// </summary>
        /// <param name="obj">基础对象实例</param>
        internal void OnObjectStartProcessing(CObject obj)
        {
            if (false == _objects.Contains(obj))
            {
                Debugger.Error("Could not found any added record of the object instance '{%t}', calling start process failed.", obj.BeanType);
                return;
            }

            // 开始运行实例
            Call(obj, obj.Start, AspectBehaviourType.Start);

            // 激活执行接口的对象实例，放入到执行队列中
            //if (typeof(IExecuteActivation).IsAssignableFrom(obj.BeanType))
            if (obj.IsExecuteActivation())
            {
                _objectExecuteList.Add(obj);
            }

            // 激活刷新接口的对象实例，放入到刷新队列中
            //if (typeof(IUpdateActivation).IsAssignableFrom(obj.BeanType))
            if (obj.IsUpdateActivation())
            {
                _objectUpdateList.Add(obj);
            }
        }

        #endregion

        #region 基础对象扩展操作函数合集

        /// <summary>
        /// 通过指定的对象类型获取对应对象的名称
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回对应对象的名称，若对象不存在则返回null</returns>
        internal string GetObjectNameForType<T>() where T : CObject
        {
            return GetObjectNameForType(typeof(T));
        }

        /// <summary>
        /// 通过指定的对象类型获取对应对象的名称
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>返回对应对象的名称，若对象不存在则返回null</returns>
        internal string GetObjectNameForType(SystemType objectType)
        {
            foreach (KeyValuePair<string, SystemType> pair in _objectClassTypes)
            {
                if (pair.Value == objectType)
                {
                    return pair.Key;
                }
            }

            return null;
        }

        /// <summary>
        /// 通过指定的基础对象类型，搜索该类型的全部实例<br/>
        /// 返回的实例列表中，包括了该类型及其子类的全部对象实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>返回给定类型的全部实例</returns>
        internal IList<CObject> FindAllObjectsByType(SystemType objectType)
        {
            IList<CObject> result = new List<CObject>();
            IEnumerator<CObject> e = _objects.GetEnumerator();
            while (e.MoveNext())
            {
                CObject obj = e.Current;
                if (objectType.IsAssignableFrom(obj.BeanType))
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
