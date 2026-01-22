/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using Cysharp.Threading.Tasks;

namespace GameEngine
{
    /// <summary>
    /// 用户界面模块封装的句柄对象类
    /// 模块具体功能接口请参考<see cref="NovaEngine.Module.GuiModule"/>类
    /// </summary>
    public sealed partial class GuiHandler : GenericEntityHandler<CView>
    {
        /// <summary>
        /// 句柄对象的单例访问获取接口
        /// </summary>
        public static GuiHandler Instance => HandlerManagement.GuiHandler;

        /// <summary>
        /// 句柄对象内置初始化接口函数
        /// </summary>
        /// <returns>若句柄对象初始化成功则返回true，否则返回false</returns>
        protected override bool OnInitialize()
        {
            if (false == base.OnInitialize()) return false;

            // 启动视图辅助工具类
            FormMaster.Startup();

            return true;
        }

        /// <summary>
        /// 句柄对象内置清理接口函数
        /// </summary>
        protected override void OnCleanup()
        {
            // 移除所有视图实例
            RemoveAllUI();

            // 清理视图类型注册列表
            UnregisterAllViewClasses();

            // 关闭视图辅助工具类
            FormMaster.Shutdown();

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

            // 刷新视图辅助工具类
            FormMaster.Update();
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

        #region 视图对象类型注册绑定相关的接口函数

        /// <summary>
        /// 注册指定的视图名称及对应的对象类到当前的句柄管理容器中
        /// 注意，注册的对象类必须继承自<see cref="GameEngine.CView"/>，否则无法正常注册
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <param name="clsType">视图类型</param>
        /// <param name="priority">视图优先级</param>
        /// <returns>若视图类型注册成功则返回true，否则返回false</returns>
        private bool RegisterViewClass(string viewName, Type clsType, int priority)
        {
            Debugger.Assert(false == string.IsNullOrEmpty(viewName) && null != clsType, NovaEngine.ErrorText.InvalidArguments);

            if (false == typeof(CView).IsAssignableFrom(clsType))
            {
                Debugger.Warn(LogGroupTag.Module, "The register type '{%t}' must be inherited from 'CView'.", clsType);
                return false;
            }

            if (false == RegisterEntityClass(viewName, clsType, priority))
            {
                Debugger.Warn(LogGroupTag.Module, "The view class '{%t}' was already registered, repeat added it failed.", clsType);
                return false;
            }

            // Debugger.Info(LogGroupTag.Module, "Register new view class type '{%t}' with target name '{%s}'.", clsType, viewName);

            return true;
        }

        /// <summary>
        /// 注销当前句柄实例绑定的所有视图类型
        /// </summary>
        private void UnregisterAllViewClasses()
        {
            UnregisterAllEntityClasses();
        }

        #endregion

        /// <summary>
        /// 通过指定的视图名称动态创建一个对应的视图对象实例
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <param name="userData">用户数据</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask<CView> OpenUI(string viewName, object userData = null)
        {
            if (false == _entityClassTypes.TryGetValue(viewName, out Type viewType))
            {
                Debugger.Warn(LogGroupTag.Module, "Could not found any correct view class with target name '{%s}', opened view failed.", viewName);
                return null;
            }

            // 视图对象实例化
            return await OpenUI(viewType, userData);
        }

        /// <summary>
        /// 通过指定的视图类型动态创建一个对应的视图对象实例
        /// </summary>
        /// <typeparam name="T">视图类型</typeparam>
        /// <param name="userData">用户数据</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask<T> OpenUI<T>(object userData = null) where T : CView
        {
            Type viewType = typeof(T);
            if (false == _entityClassTypes.Values.Contains(viewType))
            {
                Debugger.Error(LogGroupTag.Module, "Could not found any correct view class with target type '{%t}', opened view failed.", viewType);
                return null;
            }

            // 视图对象实例化
            return await OpenUI(viewType, userData) as T;
        }

        /// <summary>
        /// 通过指定的视图类型动态创建一个对应的视图对象实例
        /// </summary>
        /// <param name="viewType">视图类型</param>
        /// <param name="userData">用户数据</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        public async UniTask<CView> OpenUI(Type viewType, object userData = null)
        {
            Debugger.Assert(viewType, NovaEngine.ErrorText.InvalidArguments);
            if (false == _entityClassTypes.Values.Contains(viewType))
            {
                Debugger.Error(LogGroupTag.Module, "Could not found any correct view class with target type '{%t}', opened view failed.", viewType);

                // return null;
                throw new NovaEngine.CFrameworkException();
            }

            // 不允许重复创建
            CView view = await FindUIAsync(viewType);
            if (null != view)
            {
                return view;
            }

            // 视图对象实例化
            view = CreateInstance(viewType) as CView;
            view.UserData = userData;

            if (false == AddEntity(view))
            {
                Debugger.Warn(LogGroupTag.Module, "The view instance '{%t}' initialization for error, added it failed.", viewType);

                // return null;
                throw new NovaEngine.CFrameworkException();
            }

            // 添加实例到管理容器中
            OnGroupBindingForTargetView(view);

            await view.CreateWindow();
            if (!view.IsLoaded)
            {
                OnGroupUnbindingForTargetView(view);
                RemoveEntity(view);

                // 回收视图实例
                ReleaseInstance(view);

                Debugger.Throw("Open target ui {%t} failed", viewType);
                return null;
            }

            // 启动视图对象
            Call(view, view.Startup, AspectBehaviourType.Startup);

            // 唤醒视图对象
            CallEntityAwakeProcess(view);

            await UniTask.WaitUntil(() => view.IsReady, cancellationToken : view.CancellationTokenSource.Token);

            view.ShowWindow();

            _Profiler.CallStat(Profiler.Statistics.StatCode.ViewCreate, view);

            return view;
        }

        /// <summary>
        /// 判断指定名称的视图是否处于打开状态
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <returns>若视图处于打开状态则返回true，否则返回false</returns>
        public bool HasUI(string viewName)
        {
            if (false == _entityClassTypes.TryGetValue(viewName, out Type viewType))
            {
                Debugger.Warn(LogGroupTag.Module, "Could not found any correct view class with target name '{%s}', found view failed.", viewName);
                return false;
            }

            return HasUI(viewType);
        }

        /// <summary>
        /// 判断指定类型的视图是否处于打开状态
        /// </summary>
        /// <typeparam name="T">视图类型</typeparam>
        /// <returns>若视图处于打开状态则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasUI<T>() where T : CView
        {
            return NovaEngine.Utility.Collection.ContainsType<CView, T>(Entities);
        }

        /// <summary>
        /// 判断指定类型的视图是否处于打开状态
        /// </summary>
        /// <param name="viewType">视图类型</param>
        /// <returns>若视图处于打开状态则返回true，否则返回false</returns>
        public bool HasUI(Type viewType)
        {
            foreach (CView view in Entities)
            {
                if (view.BeanType == viewType)
                {
                    return view.IsReady;
                }
            }

            return false;
        }

        /// <summary>
        /// 通过指定的视图名称获取对应的视图对象实例
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <returns>返回查找到的视图对象实例，若查找失败则返回null</returns>
        public CView FindUI(string viewName)
        {
            if (false == _entityClassTypes.TryGetValue(viewName, out Type viewType))
            {
                Debugger.Warn(LogGroupTag.Module, "Could not found any correct view class with target name '{%s}', found view failed.", viewName);
                return null;
            }

            return FindUI(viewType);
        }

        /// <summary>
        /// 通过指定的视图类型获取对应的视图对象实例
        /// </summary>
        /// <typeparam name="T">视图类型</typeparam>
        /// <returns>返回查找到的视图对象实例，若查找失败则返回null</returns>
        public T FindUI<T>() where T : CView
        {
            Type viewType = typeof(T);
            if (false == _entityClassTypes.Values.Contains(viewType))
            {
                Debugger.Error(LogGroupTag.Module, "Could not found any correct view class with target type '{%t}', found view failed.", viewType);
                return null;
            }

            return FindUI(viewType) as T;
        }

        /// <summary>
        /// 通过指定的视图类型查找对应的视图对象实例
        /// </summary>
        /// <param name="viewType">视图类型</param>
        /// <returns>返回查找到的视图对象实例，若查找失败则返回null</returns>
        public CView FindUI(Type viewType)
        {
            foreach (CView view in Entities)
            {
                if (view.BeanType == viewType)
                {
                    if (view.IsReady)
                    {
                        return view;
                    }

                    // 视图尚未准备好
                    break;
                }
            }

            return null;
        }

        /// <summary>
        /// 通过指定的视图类型获取对应的视图对象实例
        /// </summary>
        /// <typeparam name="T">视图类型</typeparam>
        /// <returns>返回查找到的视图对象实例，若查找失败则返回null</returns>
        public async UniTask<T> FindUIAsync<T>() where T : CView
        {
            Type viewType = typeof(T);
            if (false == _entityClassTypes.Values.Contains(viewType))
            {
                Debugger.Error(LogGroupTag.Module, "Could not found any correct view class with target type '{%t}', found view failed.", viewType);
                return null;
            }

            return await FindUIAsync(viewType) as T;
        }

        /// <summary>
        /// 通过指定的视图类型查找对应的视图对象实例
        /// </summary>
        /// <param name="viewType">视图类型</param>
        /// <returns>返回查找到的视图对象实例，若查找失败则返回null</returns>
        public async UniTask<CView> FindUIAsync(Type viewType)
        {
            foreach (CView view in Entities)
            {
                if (view.BeanType == viewType)
                {
                    if (view.IsReady)
                    {
                        return view;
                    }

                    await UniTask.WaitUntil(() => view.IsReady, cancellationToken: view.CancellationTokenSource.Token);
                    return view;
                }
            }

            return null;
        }

        /// <summary>
        /// 移除指定的视图对象实例
        /// </summary>
        /// <param name="view">视图对象实例</param>
        internal void RemoveUI(CView view)
        {
            if (false == Entities.Contains(view))
            {
                Debugger.Error(LogGroupTag.Module, "Could not found any view reference '{%t}' with manage container, removed it failed.", view.BeanType);
                return;
            }

            // 视图尚未关闭，则先执行视图关闭操作
            if (false == view.IsClosed)
            {
                _Profiler.CallStat(Profiler.Statistics.StatCode.ViewClose, view);

                // 销毁视图对象
                CallEntityDestroyProcess(view);

                // 关闭视图实例
                view.__Close();
                return;
            }

            // 从管理容器中移除实例
            OnGroupUnbindingForTargetView(view);
            // 移除视图实例
            RemoveEntity(view);

            // 回收视图实例
            ReleaseInstance(view);
        }

        /// <summary>
        /// 移除当前环境下所有的视图对象实例
        /// </summary>
        internal void RemoveAllUI()
        {
            while (Entities.Count > 0)
            {
                RemoveUI(Entities[0]);
            }
        }

        /// <summary>
        /// 关闭指定的视图对象实例
        /// </summary>
        /// <param name="view">视图对象实例</param>
        public void CloseUI(CView view)
        {
            if (false == Entities.Contains(view))
            {
                Debugger.Error(LogGroupTag.Module, "Could not found any view reference '{%t}' with manage container, removed it failed.", view.BeanType);
                return;
            }

            // 刷新状态时推到销毁队列中
            // if (view.IsOnUpdatingStatus())
            if (view.IsCurrentLifecycleTypeRunning)
            {
                view.OnPrepareToDestroy();
                return;
            }

            // 在非逻辑刷新的状态下，直接移除对象即可
            RemoveUI(view);
        }

        /// <summary>
        /// 关闭指定的视图名称对应的视图对象实例
        /// 若存在相同名称的多个视图对象实例，则一同移除
        /// </summary>
        /// <param name="viewName">视图名称</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CloseUI(string viewName)
        {
            if (false == _entityClassTypes.TryGetValue(viewName, out Type viewType))
            {
                Debugger.Warn(LogGroupTag.Module, "Could not found any correct view class with target name '{%s}', closed view failed.", viewName);
                return;
            }

            CloseUI(viewType);
        }

        /// <summary>
        /// 关闭指定的视图类型对应的视图对象实例
        /// 若存在相同类型的多个视图对象实例，则一同移除
        /// </summary>
        /// <typeparam name="T">视图类型</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CloseUI<T>() where T : CView
        {
            CloseUI(typeof(T));
        }

        /// <summary>
        /// 关闭指定的视图类型对应的视图对象实例
        /// 若存在相同类型的多个视图对象实例，则一同移除
        /// </summary>
        /// <param name="viewType">视图类型</param>
        public void CloseUI(Type viewType)
        {
            foreach (CView view in NovaEngine.Utility.Collection.Reverse(Entities))
            {
                if (view.BeanType == viewType)
                {
                    CloseUI(view);
                }
            }
        }

        /// <summary>
        /// 关闭当前环境下所有的视图对象实例
        /// </summary>
        public void CloseAllUI()
        {
            for (int n = Entities.Count - 1; n >= 0; --n)
            {
                Entities[n].Close();
            }
        }

        /// <summary>
        /// 新增视图对象实例到管理容器中
        /// </summary>
        /// <param name="view">视图对象实例</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnGroupBindingForTargetView(CView view)
        {
            ViewGroup viewGroup = FindGroupByViewType(view.BeanType);
            viewGroup.OnViewGroupBinding(view);
        }

        /// <summary>
        /// 从管理容器中移除指定的视图对象实例
        /// </summary>
        /// <param name="view">视图对象实例</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnGroupUnbindingForTargetView(CView view)
        {
            ViewGroup viewGroup = FindGroupByViewType(view.BeanType);
            viewGroup?.OnViewGroupUnbinding(view);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 通过指定的视图类型获取对应视图的名称
        /// </summary>
        /// <typeparam name="T">视图类型</typeparam>
        /// <returns>返回对应视图的名称，若视图不存在则返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal string GetViewNameForType<T>() where T : CView
        {
            return GetViewNameForType(typeof(T));
        }

        /// <summary>
        /// 通过指定的视图类型获取对应视图的名称
        /// </summary>
        /// <param name="viewType">视图类型</param>
        /// <returns>返回对应视图的名称，若视图不存在则返回null</returns>
        internal string GetViewNameForType(Type viewType)
        {
            foreach (KeyValuePair<string, Type> pair in _entityClassTypes)
            {
                if (pair.Value == viewType)
                {
                    return pair.Key;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取当前已创建的全部视图对象实例
        /// </summary>
        /// <returns>返回已创建的全部视图对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyList<CView> GetAllViews()
        {
            return GetAllEntities();
        }

        /// <summary>
        /// 检测当前已创建的视图对象列表中是否存在指定标识的对象实例
        /// </summary>
        /// <param name="beanId">实体标识</param>
        /// <returns>若存在指定标识的视图对象实例则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasViewById(int beanId)
        {
            return HasEntityById(beanId);
        }

        /// <summary>
        /// 通过指定的对象标识查找对应的视图对象实例
        /// </summary>
        /// <param name="beanId">实体标识</param>
        /// <returns>返回对应的视图对象实例，若该实例不存在则返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CView GetViewById(int beanId)
        {
            return GetEntityById(beanId);
        }

        /// <summary>
        /// 通过指定的视图类型，搜索该类型的全部实例<br/>
        /// 返回的实例列表中，包括了该类型及其子类的全部视图实例
        /// </summary>
        /// <param name="viewType">视图类型</param>
        /// <returns>返回给定类型的全部实例</returns>
        internal IReadOnlyList<CView> FindAllViewsByType(Type viewType)
        {
            List<CView> result = null;
            for (int n = 0; n < Entities.Count; ++n)
            {
                CView view = Entities[n];
                if (view.BeanType.Is(viewType))
                {
                    if (null == result) result = new List<CView>();

                    result.Add(view);
                }
            }

            return result;
        }

        #region 表单管理器对象的添加/移除相关操作的接口函数

        /// <summary>
        /// 注册指定的窗口管理器到当前的主控制器中
        /// </summary>
        /// <typeparam name="T">管理器类型</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RegisterFormManager<T>() where T : IFormManager, new()
        {
            FormMaster.RegisterFormManager<T>();
        }

        /// <summary>
        /// 注册指定的窗口管理器到当前的主控制器中
        /// </summary>
        /// <param name="classType">管理器类型</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RegisterFormManager(Type classType)
        {
            FormMaster.RegisterFormManager(classType);
        }

        /// <summary>
        /// 注册指定的窗口管理器到当前的主控制器中
        /// </summary>
        /// <param name="formManager">管理器对象</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RegisterFormManager(IFormManager formManager)
        {
            FormMaster.RegisterFormManager(formManager);
        }

        /// <summary>
        /// 从主控制器中移除当前实施的
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnregisterCurrentFormManager()
        {
            FormMaster.UnregisterCurrentFormManager();
        }

        #endregion
    }
}
