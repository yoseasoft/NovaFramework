/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
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
using SystemAction = System.Action;
using SystemSendOrPostCallback = System.Threading.SendOrPostCallback;

namespace NovaEngine
{
    /// <summary>
    /// 引擎框架模块中控台管理类，对当前程序的所有模块对象实例进行统一管理调度
    /// </summary>
    public static partial class ModuleController
    {
        /// <summary>
        /// 模块实例管理队列
        /// </summary>
        private static readonly LinkedList<ModuleObject> _modules = new LinkedList<ModuleObject>();
        private static readonly IDictionary<int, ModuleObject> _moduleCollections = new Dictionary<int, ModuleObject>();

        /// <summary>
        /// 任务调度管理队列
        /// </summary>
        private static readonly IList<TaskInvokeAction> _runningActions = new List<TaskInvokeAction>();
        private static readonly IList<TaskInvokeAction> _waitingActions = new List<TaskInvokeAction>();

        private static bool _isRunning = false;
        private static CommandDispatcher _commandDispatcher = null;
        private static EventPool<ModuleEventArgs> _eventHandler;

        /// <summary>
        /// 模块中控台启动管理，该操作将启动所有已经添加的模块实例
        /// </summary>
        public static void Startup()
        {
            if (_isRunning)
            {
                throw new CFrameworkException("Running status is invalid.");
            }

            // 在加载模块前先标记运行状态为启动模式
            _isRunning = true;

            // 指令转发管理器初始化操作
            _commandDispatcher = new CommandDispatcher();
            _commandDispatcher.Initialize();

            // 事件处理句柄初始化操作
            _eventHandler = new EventPool<ModuleEventArgs>(EventPool<ModuleEventArgs>.AllowHandlerType.Default);
            _eventHandler.Startup();

            // 启动当前添加管理的全部模块
            LoadModuleOnStartup();
        }

        /// <summary>
        /// 模块中控台关闭管理，该操作将关闭所有已经添加的模块实例
        /// </summary>
        public static void Shutdown()
        {
            if (false == _isRunning)
            {
                throw new CFrameworkException("Running status is invalid.");
            }

            // 关闭当前添加管理的全部模块
            UnloadModuleOnShutdown();

            // 事件处理句柄清理操作
            _eventHandler.Shutdown();
            _eventHandler = null;

            // 指令转发管理器清理操作
            _commandDispatcher.Cleanup();
            _commandDispatcher = null;

            _waitingActions.Clear();
            _runningActions.Clear();

            // 在卸载模块之后再标记运行状态为停止模式
            _isRunning = false;
        }

        /// <summary>
        /// 模块中控台垃圾回收管理，该操作将对所有已经添加的模块实例进行垃圾回收操作
        /// </summary>
        public static void Dump()
        {
            if (false == _isRunning)
            {
                return;
            }

            foreach (ModuleObject module in _modules)
            {
                module.Dump();
            }
        }

        /// <summary>
        /// 模块中控台当前添加的全部模块实例刷新调度接口
        /// </summary>
        public static void Update()
        {
            if (false == _isRunning)
            {
                return;
            }

            foreach (ModuleObject module in _modules)
            {
                module.Update();
            }

            if (_waitingActions.Count > 0)
            {
                lock (_waitingActions)
                {
                    float t = Timestamp.Time;
                    for (int n = _waitingActions.Count - 1; n >= 0; --n)
                    {
                        TaskInvokeAction task = _waitingActions[n];
                        if (task._timestamp <= t)
                        {
                            _runningActions.Add(task);
                            _waitingActions.Remove(task);
                        }
                    }
                }

                for (int n = 0; n < _runningActions.Count; ++n)
                {
                    _runningActions[n]._action();
                }

                _runningActions.Clear();
            }

            _eventHandler.Update();
        }

        /// <summary>
        /// 模块中控台当前添加的全部模块实例延迟刷新调度接口
        /// </summary>
        public static void LateUpdate()
        {
            if (false == _isRunning)
            {
                return;
            }

            foreach (ModuleObject module in _modules)
            {
                module.LateUpdate();
            }
        }

        #region 模块对象实例管理控制接口函数

        /// <summary>
        /// 通过指定模块类型获取对应的模块对象实例
        /// </summary>
        /// <typeparam name="T">模块类型</typeparam>
        /// <returns>返回类型获取对应的对象实例</returns>
        public static T GetModule<T>() where T : ModuleObject
        {
            return (T) GetModule(typeof(T));
        }

        /// <summary>
        /// 通过指定模块类型名称获取对应的模块对象实例
        /// </summary>
        /// <param name="clsType">模块类型</param>
        /// <returns>返回类型名称获取对应的对象实例</returns>
        public static ModuleObject GetModule(SystemType clsType)
        {
            int type = Config.GetModuleType(clsType);

            return GetModule(type);
        }

        /// <summary>
        /// 通过指定模块类型获取对应的模块对象实例
        /// </summary>
        /// <param name="moduleType">模块对象类型</param>
        /// <returns>返回类型获取对应的对象实例</returns>
        public static ModuleObject GetModule(ModuleObject.EEventType moduleType)
        {
            return GetModule((int) moduleType);
        }

        /// <summary>
        /// 通过指定模块类型获取对应的模块对象实例
        /// </summary>
        /// <param name="moduleType">模块对象类型</param>
        /// <returns>返回类型获取对应的对象实例</returns>
        public static ModuleObject GetModule(int moduleType)
        {
            if (_moduleCollections.ContainsKey(moduleType))
            {
                return _moduleCollections[moduleType];
            }

            return null;
        }

        /// <summary>
        /// 创建指定类型的模块实例
        /// </summary>
        /// <param name="moduleType">模块对象类型</param>
        /// <returns>返回当前创建的模块新实例</returns>
        private static ModuleObject CreateModuleObject(ModuleObject.EEventType moduleType)
        {
            return CreateModuleObject((int) moduleType);
        }

        /// <summary>
        /// 创建指定类型的模块实例
        /// </summary>
        /// <param name="moduleType">模块对象类型</param>
        /// <returns>返回当前创建的模块新实例</returns>
        private static ModuleObject CreateModuleObject(int moduleType)
        {
            Logger.Assert(_isRunning);

            if (_moduleCollections.ContainsKey(moduleType))
            {
                throw new CFrameworkException("Module type is already exists.");
            }

            SystemType typeName = Config.GetModuleReflectionType(moduleType);
            if (null == typeName)
            {
                throw new CFrameworkException("Module type is invalid.");
            }

            ModuleObject module = (ModuleObject) System.Activator.CreateInstance(typeName);
            module.Initialize();

            // 启动模块实例
            module.Startup();

            _moduleCollections.Add(moduleType, module);
            _modules.AddLast(module);

            return module;
        }

        /// <summary>
        /// 移除指定类型的模块实例
        /// </summary>
        /// <param name="moduleType">模块对象类型</param>
        private static void ReleaseModuleObject(ModuleObject.EEventType moduleType)
        {
            ReleaseModuleObject((int) moduleType);
        }

        /// <summary>
        /// 移除指定类型的模块实例
        /// </summary>
        /// <param name="moduleType">模块对象类型</param>
        private static void ReleaseModuleObject(int moduleType)
        {
            Logger.Assert(_isRunning);

            if (false == _moduleCollections.ContainsKey(moduleType))
            {
                throw new CFrameworkException("Module type is invalid.");
            }

            ModuleObject module = _moduleCollections[moduleType];
            module.Cleanup();

            // 关闭模块实例
            module.Shutdown();

            _moduleCollections.Remove(moduleType);
            _modules.Remove(module);
        }

        /// <summary>
        /// 按优先级对模块列表进行排序
        /// </summary>
        private static void SortingAllModules()
        {
            LinkedList<ModuleObject> result = new LinkedList<ModuleObject>();

            LinkedListNode<ModuleObject> lln;
            foreach (ModuleObject m in _modules)
            {
                lln = result.First;
                while (true)
                {
                    if (null == lln)
                    {
                        result.AddLast(m);
                        break;
                    }
                    // else if (nodevalue.GetPriority() >= lln.Value.GetPriority())
                    else if (m.GetPriority() <= lln.Value.GetPriority())
                    {
                        result.AddBefore(lln, m);
                        break;
                    }
                    else
                    {
                        lln = lln.Next;
                    }
                }
            }

            _modules.Clear();
            lln = result.First;
            while (null != lln)
            {
                _modules.AddLast(lln.Value);
                lln = lln.Next;
            }
        }

        /// <summary>
        /// 在启动模式下加载记录类型的全部模块实例
        /// </summary>
        private static void LoadModuleOnStartup()
        {
            IList<int> moduleTypes = Config.GetAllRegModuleTypes();
            foreach (int moduleType in moduleTypes)
            {
                if (_moduleCollections.ContainsKey(moduleType))
                {
                    ReleaseModuleObject(moduleType);
                }

                CreateModuleObject(moduleType);
            }

            // 重新排序
            SortingAllModules();
        }

        /// <summary>
        /// 在关闭模式下卸载当前管理的全部模块实例
        /// </summary>
        private static void UnloadModuleOnShutdown()
        {
            IList<int> moduleTypes = Config.GetAllRegModuleTypes();
            foreach (int moduleType in moduleTypes)
            {
                if (_moduleCollections.ContainsKey(moduleType))
                {
                    ReleaseModuleObject(moduleType);
                }
            }
        }

        #endregion

        /// <summary>
        /// 在当前主线程下以排队方式执行目标任务逻辑，该逻辑仅可一次性执行完成，不可循环等待
        /// </summary>
        /// <param name="action">目标任务项</param>
        public static void QueueOnMainThread(SystemAction action)
        {
            QueueOnMainThread(action, 0f);
        }

        /// <summary>
        /// 在当前主线程下以排队方式执行目标任务逻辑，逻辑执行过程可定制参数
        /// </summary>
        /// <param name="callback">任务回调接口</param>
        /// <param name="args">参数对象</param>
        public static void QueueOnMainThread(SystemSendOrPostCallback callback, object args)
        {
            QueueOnMainThread(() => { callback(args); });
        }

        /// <summary>
        /// 在当前主线程下延迟特定时长后以排队方式执行目标任务逻辑，该逻辑仅可一次性执行完成，不可循环等待
        /// </summary>
        /// <param name="action">目标任务项</param>
        /// <param name="delay">延迟时间</param>
        public static void QueueOnMainThread(SystemAction action, float delay)
        {
            float time = 0f;
            if (delay > 0f)
            {
                time = Timestamp.Time + delay;
            }

            lock (_waitingActions)
            {
                _waitingActions.Add(new TaskInvokeAction { _timestamp = time, _action = action, });
            }
        }

        #region 指令事件转发调度接口函数

        /// <summary>
        /// 模块实例发送事件接口，将特定事件标识及其对应的数据实体添加到事件队列中，将在下一帧主线程业务更新时统一处理
        /// </summary>
        /// <param name="e">事件对象</param>
        public static void SendEvent(object sender, ModuleEventArgs e)
        {
            SendEvent(sender, e, false);
        }

        /// <summary>
        /// 模块实例发送事件接口，将特定事件标识及其对应的数据实例提交，并通过执行状态标识决定是否在当前线程中被立即执行还是延后处理
        /// </summary>
        /// <param name="e">事件对象</param>
        /// <param name="now">立即执行标识</param>
        public static void SendEvent(object sender, ModuleEventArgs e, bool now)
        {
            if (now)
            {
                // 事件发送 - 立即处理模式
                _eventHandler.FireNow(sender, e);
            }
            else
            {
                // 事件发送
                _eventHandler.Fire(sender, e);
            }
        }

        /// <summary>
        /// 注册事件处理函数
        /// </summary>
        /// <param name="id">事件类型编号</param>
        /// <param name="handler">事件处理函数</param>
        public static void RegisterEventHandler(int id, System.EventHandler<ModuleEventArgs> handler)
        {
            _eventHandler.Subscribe(id, handler);
        }

        /// <summary>
        /// 取消注册事件处理函数
        /// </summary>
        /// <param name="id">事件类型编号</param>
        /// <param name="handler">事件处理函数</param>
        public static void UnregisterEventHandler(int id, System.EventHandler<ModuleEventArgs> handler)
        {
            _eventHandler.Unsubscribe(id, handler);
        }

        /// <summary>
        /// 增加当前模块中控台管理的指令代理实例
        /// </summary>
        /// <param name="cname">代理对象名称</param>
        /// <param name="agent">代理对象实例</param>
        public static void AddCommandAgent(string cname, ICommandAgent agent)
        {
            Logger.Assert(_isRunning);

            _commandDispatcher.AddAgent(cname, agent);
        }

        /// <summary>
        /// 移除当前模块中控台管理的指令代理实例
        /// </summary>
        /// <param name="cname">代理对象名称</param>
        public static void RemoveCommandAgent(string cname)
        {
            Logger.Assert(_isRunning);

            _commandDispatcher.RemoveAgent(cname);
        }

        /// <summary>
        /// 使用通用指令处理模式转发目标事件类型通知
        /// </summary>
        /// <param name="id">事件标识</param>
        /// <param name="type">事件类型</param>
        public static void CallCommand(int id, int type)
        {
            if (false == _isRunning)
            {
                return;
            }

            CommonEventArgs e = ReferencePool.Acquire<CommonEventArgs>();
            e.EventID = id;
            e.EventType = type;

            ModuleCommandArgs c = ReferencePool.Acquire<ModuleCommandArgs>();
            c.SetType(e.EventID);
            c.SetData(e);

            _commandDispatcher.Call(c);

            ReferencePool.Release(e);
        }

        /// <summary>
        /// 使用指令管理器转发目标指令参数实例
        /// </summary>
        /// <param name="args">指令参数实例</param>
        public static void CallCommand(ModuleCommandArgs args)
        {
            if (false == _isRunning)
            {
                return;
            }

            _commandDispatcher.Call(args);
        }

        #endregion
    }
}
