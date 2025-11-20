/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

using SystemAction = System.Action;

namespace NovaEngine
{
    /// <summary>
    /// 引擎框架模块对象的抽象定义类
    /// 我们使用该抽象模块对象类替换原本的管理器基类，重新设计管理器的调度接口及事件转发接口
    /// </summary>
    internal abstract partial class ModuleObject
    {
        /// <summary>
        /// 模块类型常量定义
        /// </summary>
        public static readonly System.Type CLASS_TYPE = typeof(ModuleObject);

        /// <summary>
        /// 模块类的新实例构建接口
        /// </summary>
        public ModuleObject()
        {
        }

        /// <summary>
        /// 获取引擎框架模块的事件类型
        /// </summary>
        public abstract int EventType
        {
            get;
        }

        /// <summary>
        /// 初始化引擎框架模块实例的回调接口
        /// </summary>
        protected abstract void OnInitialize();

        /// <summary>
        /// 清理引擎框架模块实例的回调接口
        /// </summary>
        protected abstract void OnCleanup();

        /// <summary>
        /// 启动引擎框架模块实例
        /// </summary>
        protected abstract void OnStartup();

        /// <summary>
        /// 关闭引擎框架模块实例
        /// </summary>
        protected abstract void OnShutdown();

        /// <summary>
        /// 引擎框架模块实例的垃圾回收调度接口
        /// </summary>
        protected abstract void OnDump();

        /// <summary>
        /// 引擎框架模块的轮询调度接口
        /// </summary>
        protected abstract void OnUpdate();

        /// <summary>
        /// 引擎框架模块的延迟轮询调度接口
        /// </summary>
        protected abstract void OnLateUpdate();

        /// <summary>
        /// 引擎框架模块实例初始化接口
        /// </summary>
        internal void Initialize()
        {
            OnInitialize();
        }

        /// <summary>
        /// 引擎框架模块实例清理接口
        /// </summary>
        internal void Cleanup()
        {
            OnCleanup();
        }

        /// <summary>
        /// 引擎框架模块实例启动接口
        /// </summary>
        internal void Startup()
        {
            OnEventSubscribe(this.EventType);

            OnStartup();
        }

        /// <summary>
        /// 引擎框架模块实例关闭接口
        /// </summary>
        internal void Shutdown()
        {
            OnShutdown();

            OnEventUnsubscribe(this.EventType);
        }

        /// <summary>
        /// 引擎框架模块实例刷新接口
        /// </summary>
        internal void Update()
        {
            OnUpdate();
        }

        /// <summary>
        /// 引擎框架模块实例延迟刷新接口
        /// </summary>
        internal void LateUpdate()
        {
            OnLateUpdate();
        }

        /// <summary>
        /// 引擎框架模块实例垃圾回收接口
        /// </summary>
        internal void Dump()
        {
            OnDump();
        }

        protected T AcquireEvent<T>() where T : class, IReference, new()
        {
            return ReferencePool.Acquire<T>();
        }

        /// <summary>
        /// 模块实例发送事件接口，将特定事件标识及其对应的数据实体添加到事件队列中，将在下一帧主线程业务更新时统一处理
        /// </summary>
        /// <param name="e">事件对象</param>
        protected void SendEvent(ModuleEventArgs e)
        {
            this.SendEvent(e, false);
        }

        /// <summary>
        /// 模块实例发送事件接口，将特定事件标识及其对应的数据实例提交，并通过执行状态标识决定是否在当前线程中被立即执行还是延后处理
        /// </summary>
        /// <param name="e">事件对象</param>
        /// <param name="now">立即执行标识</param>
        protected void SendEvent(ModuleEventArgs e, bool now)
        {
            ModuleController.SendEvent(this, e, now);
        }

        /// <summary>
        /// 事件订阅接口
        /// </summary>
        /// <param name="eventID">事件标识</param>
        protected void OnEventSubscribe(int eventID)
        {
            ModuleController.RegisterEventHandler(eventID, OnEventDispatch);
        }

        /// <summary>
        /// 事件取消订阅接口
        /// </summary>
        /// <param name="eventID">事件标识</param>
        protected void OnEventUnsubscribe(int eventID)
        {
            ModuleController.UnregisterEventHandler(eventID, OnEventDispatch);
        }

        /// <summary>
        /// 事件处理回调通知接口
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件对象</param>
        protected void OnEventDispatch(object sender, ModuleEventArgs e)
        {
            ModuleCommandArgs moduleCommand = ReferencePool.Acquire<ModuleCommandArgs>();
            moduleCommand.SetType(e.ID);
            moduleCommand.SetData(e);
            ModuleController.CallCommand(moduleCommand);
        }

        /// <summary>
        /// 获取当前模块对象的处理优先级
        /// </summary>
        /// <returns>返回模块对象的处理优先级</returns>
        public int GetPriority()
        {
            return EventType;
        }

        /// <summary>
        /// 通过指定类型获取对应的模块对象实例
        /// </summary>
        /// <typeparam name="T">模块类型</typeparam>
        /// <returns>返回对应类型的模块对象实例</returns>
        public static T GetModule<T>() where T : ModuleObject
        {
            return ModuleController.GetModule<T>();
        }

        /// <summary>
        /// 在当前主线程下以排队方式执行目标任务逻辑，该逻辑仅可一次性执行完成，不可循环等待
        /// </summary>
        /// <param name="action">目标任务项</param>
        public static void QueueOnMainThread(SystemAction action)
        {
            ModuleController.QueueOnMainThread(action);
        }

        /// <summary>
        /// 在当前主线程下延迟特定时长后以排队方式执行目标任务逻辑，该逻辑仅可一次性执行完成，不可循环等待
        /// </summary>
        /// <param name="action">目标任务项</param>
        /// <param name="delay">延迟时间</param>
        public static void QueueOnMainThread(SystemAction action, float delay)
        {
            ModuleController.QueueOnMainThread(action, delay);
        }

        /// <summary>
        /// 检查指定对象类型是否为基础模块类型的子类型
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>若对象类型继承自模块类则返回true，否则返回false</returns>
        public static bool IsInheritanceType<T>()
        {
            return IsInheritanceType(typeof(T));
        }

        /// <summary>
        /// 检查指定类型名称是否为基础模块类型的子类型名称
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <returns>若名称对应类型继承自模块类则返回true，否则返回false</returns>
        public static bool IsInheritanceType(System.Type typeName)
        {
            if (typeName.IsSubclassOf(ModuleObject.CLASS_TYPE))
            {
                return true;
            }

            return false;
        }
    }
}
