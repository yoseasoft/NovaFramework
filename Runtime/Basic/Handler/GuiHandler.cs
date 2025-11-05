/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
    /// 视图对象窗口表单类型的枚举定义
    /// </summary>
    public enum ViewFormType : byte
    {
        /// <summary>
        /// 无效
        /// </summary>
        Unknown = 0,

        UGUI = 1,

        FairyGUI = 2,

        UIToolkit = 3,
    }

    /// <summary>
    /// 用户界面模块封装的句柄对象类
    /// 模块具体功能接口请参考<see cref="NovaEngine.GuiModule"/>类
    /// </summary>
    public sealed partial class GuiHandler : EntityHandler
    {
        /// <summary>
        /// 视图默认窗口表单类型
        /// </summary>
        private ViewFormType _defaultViewFormType;

        /// <summary>
        /// 视图窗口类型映射注册管理容器
        /// </summary>
        private IDictionary<SystemType, ViewFormType> _viewFormTypes;

        /// <summary>
        /// 当前环境下所有实例化的视图对象
        /// </summary>
        private IList<CView> _views;

        /// <summary>
        /// 句柄对象的单例访问获取接口
        /// </summary>
        public static GuiHandler Instance => HandlerManagement.GuiHandler;

        /// <summary>
        /// 视图默认窗口表单类型的获取接口
        /// </summary>
        internal ViewFormType DefaultViewFormType => _defaultViewFormType;

        /// <summary>
        /// 句柄对象默认构造函数
        /// </summary>
        public GuiHandler()
        {
        }

        /// <summary>
        /// 句柄对象析构函数
        /// </summary>
        ~GuiHandler()
        {
        }

        /// <summary>
        /// 句柄对象内置初始化接口函数
        /// </summary>
        /// <returns>若句柄对象初始化成功则返回true，否则返回false</returns>
        protected override bool OnInitialize()
        {
            if (false == base.OnInitialize()) return false;

            // 必须存在一个有效的视图窗口的表单类型
            _defaultViewFormType = NovaEngine.Utility.Convertion.GetEnumFromValue<ViewFormType>(NovaEngine.Configuration.formSystemType);
            Debugger.Assert(ViewFormType.Unknown != _defaultViewFormType, "The view form type must be non-null.");

            // 初始化视图窗口类型注册列表
            _viewFormTypes = new Dictionary<SystemType, ViewFormType>();

            // 初始化视图实例容器
            _views = new List<CView>();

            // 启动视图辅助工具类
            FormHelper.Startup();

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
            FormHelper.Shutdown();

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
            FormHelper.Update();
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

        #region 视图类动态注册绑定接口函数

        /// <summary>
        /// 注册指定的视图名称及对应的对象类到当前的句柄管理容器中
        /// 注意，注册的对象类必须继承自<see cref="GameEngine.CView"/>，否则无法正常注册
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <param name="clsType">视图类型</param>
        /// <param name="priority">视图优先级</param>
        /// <param name="formType">视图窗口类型</param>
        /// <returns>若视图类型注册成功则返回true，否则返回false</returns>
        private bool RegisterViewClass(string viewName, SystemType clsType, int priority, ViewFormType formType)
        {
            Debugger.Assert(false == string.IsNullOrEmpty(viewName) && null != clsType, NovaEngine.ErrorText.InvalidArguments);

            if (false == typeof(CView).IsAssignableFrom(clsType))
            {
                Debugger.Warn("The register type '{%t}' must be inherited from 'CView'.", clsType);
                return false;
            }

            if (false == RegisterEntityClass(viewName, clsType, priority))
            {
                Debugger.Warn("The view class '{%t}' was already registed, repeat added it failed.", clsType);
                return false;
            }

            if (_viewFormTypes.ContainsKey(clsType))
            {
                Debugger.Warn("The view class '{%t}' binding group '{%i}' was already registed, repeat add will be override old value.", clsType, formType);
                _viewFormTypes.Remove(clsType);
            }

            if (ViewFormType.Unknown != formType && _defaultViewFormType != formType)
            {
                // Debugger.Info("Add view form type {%i} with target class {%t}", formType, clsType);
                _viewFormTypes.Add(clsType, formType);
            }

            return true;
        }

        /// <summary>
        /// 注销当前句柄实例绑定的所有视图类型
        /// </summary>
        private void UnregisterAllViewClasses()
        {
            UnregisterAllEntityClasses();

            _viewFormTypes.Clear();
        }

        /// <summary>
        /// 通过视图类型获取其对应的窗口表单类型，
        /// 若未配置该视图类型的映射关系，则返回系统默认配置的窗口表单类型
        /// </summary>
        /// <param name="viewType">视图类型</param>
        /// <returns>返回该视图类型对应的窗口表单类型</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        internal ViewFormType GetFormTypeByViewType(SystemType viewType)
        {
            if (_viewFormTypes.TryGetValue(viewType, out ViewFormType formType))
            {
                return formType;
            }

            return _defaultViewFormType;
        }

        #endregion

        /// <summary>
        /// 通过指定的视图名称动态创建一个对应的视图对象实例
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        public async Cysharp.Threading.Tasks.UniTask<CView> OpenUI(string viewName)
        {
            SystemType viewType;
            if (false == _entityClassTypes.TryGetValue(viewName, out viewType))
            {
                Debugger.Warn("Could not found any correct view class with target name '{%s}', opened view failed.", viewName);
                return null;
            }

            // 视图对象实例化
            return await OpenUI(viewType);
        }

        /// <summary>
        /// 通过指定的视图类型动态创建一个对应的视图对象实例
        /// </summary>
        /// <typeparam name="T">视图类型</typeparam>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        public async Cysharp.Threading.Tasks.UniTask<T> OpenUI<T>() where T : CView
        {
            SystemType viewType = typeof(T);
            if (false == _entityClassTypes.Values.Contains(viewType))
            {
                Debugger.Error("Could not found any correct view class with target type '{%t}', opened view failed.", viewType);
                return null;
            }

            // 视图对象实例化
            return await OpenUI(viewType) as T;
        }

        /// <summary>
        /// 通过指定的视图类型动态创建一个对应的视图对象实例
        /// </summary>
        /// <param name="viewType">视图类型</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        public async Cysharp.Threading.Tasks.UniTask<CView> OpenUI(SystemType viewType)
        {
            Debugger.Assert(viewType, NovaEngine.ErrorText.InvalidArguments);
            if (false == _entityClassTypes.Values.Contains(viewType))
            {
                Debugger.Error("Could not found any correct view class with target type '{%t}', opened view failed.", viewType);

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
            if (false == AddEntity(view))
            {
                Debugger.Warn("The view instance '{%t}' initialization for error, added it failed.", viewType);

                // return null;
                throw new NovaEngine.CFrameworkException();
            }

            // 添加实例到管理容器中
            _AddView(view);

            await view.CreateWindow();
            if (!view.IsLoaded)
            {
                _RemoveView(view);
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

            await Cysharp.Threading.Tasks.UniTask.WaitUntil(() => view.IsReady, cancellationToken : view.CancellationTokenSource.Token);

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
            SystemType viewType;
            if (false == _entityClassTypes.TryGetValue(viewName, out viewType))
            {
                Debugger.Warn("Could not found any correct view class with target name '{%s}', found view failed.", viewName);
                return false;
            }

            return HasUI(viewType);
        }

        /// <summary>
        /// 判断指定类型的视图是否处于打开状态
        /// </summary>
        /// <typeparam name="T">视图类型</typeparam>
        /// <returns>若视图处于打开状态则返回true，否则返回false</returns>
        public bool HasUI<T>() where T : CView
        {
            return NovaEngine.Utility.Collection.ContainsType<CView, T>(_views);
        }

        /// <summary>
        /// 判断指定类型的视图是否处于打开状态
        /// </summary>
        /// <param name="viewType">视图类型</param>
        /// <returns>若视图处于打开状态则返回true，否则返回false</returns>
        public bool HasUI(SystemType viewType)
        {
            foreach (CView view in _views)
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
            SystemType viewType;
            if (false == _entityClassTypes.TryGetValue(viewName, out viewType))
            {
                Debugger.Warn("Could not found any correct view class with target name '{0}', found view failed.", viewName);
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
            SystemType viewType = typeof(T);
            if (false == _entityClassTypes.Values.Contains(viewType))
            {
                Debugger.Error("Could not found any correct view class with target type '{%t}', found view failed.", viewType);
                return null;
            }

            return FindUI(viewType) as T;
        }

        /// <summary>
        /// 通过指定的视图类型查找对应的视图对象实例
        /// </summary>
        /// <param name="viewType">视图类型</param>
        /// <returns>返回查找到的视图对象实例，若查找失败则返回null</returns>
        public CView FindUI(SystemType viewType)
        {
            foreach (CView view in _views)
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
        public async Cysharp.Threading.Tasks.UniTask<T> FindUIAsync<T>() where T : CView
        {
            SystemType viewType = typeof(T);
            if (false == _entityClassTypes.Values.Contains(viewType))
            {
                Debugger.Error("Could not found any correct view class with target type '{%t}', found view failed.", viewType);
                return null;
            }

            return await FindUIAsync(viewType) as T;
        }

        /// <summary>
        /// 通过指定的视图类型查找对应的视图对象实例
        /// </summary>
        /// <param name="viewType">视图类型</param>
        /// <returns>返回查找到的视图对象实例，若查找失败则返回null</returns>
        public async Cysharp.Threading.Tasks.UniTask<CView> FindUIAsync(SystemType viewType)
        {
            foreach (CView view in _views)
            {
                if (view.BeanType == viewType)
                {
                    if (view.IsReady)
                    {
                        return view;
                    }

                    await Cysharp.Threading.Tasks.UniTask.WaitUntil(() => view.IsReady, cancellationToken: view.CancellationTokenSource.Token);
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
            if (false == _views.Contains(view))
            {
                Debugger.Error("Could not found any view reference '{%t}' with manage container, removed it failed.", view.BeanType);
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
            _RemoveView(view);
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
            while (_views.Count > 0)
            {
                RemoveUI(_views[0]);
            }
        }

        /// <summary>
        /// 关闭指定的视图对象实例
        /// </summary>
        /// <param name="view">视图对象实例</param>
        public void CloseUI(CView view)
        {
            if (false == _views.Contains(view))
            {
                Debugger.Error("Could not found any view reference '{%t}' with manage container, removed it failed.", view.BeanType);
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
        public void CloseUI(string viewName)
        {
            SystemType viewType;
            if (false == _entityClassTypes.TryGetValue(viewName, out viewType))
            {
                Debugger.Warn("Could not found any correct view class with target name '{%s}', closed view failed.", viewName);
                return;
            }

            CloseUI(viewType);
        }

        /// <summary>
        /// 关闭指定的视图类型对应的视图对象实例
        /// 若存在相同类型的多个视图对象实例，则一同移除
        /// </summary>
        /// <typeparam name="T">视图类型</typeparam>
        public void CloseUI<T>() where T : CView
        {
            SystemType viewType = typeof(T);

            CloseUI(viewType);
        }

        /// <summary>
        /// 关闭指定的视图类型对应的视图对象实例
        /// 若存在相同类型的多个视图对象实例，则一同移除
        /// </summary>
        /// <param name="viewType">视图类型</param>
        public void CloseUI(SystemType viewType)
        {
            foreach (CView view in NovaEngine.Utility.Collection.Reverse<CView>(_views))
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
            for (int n = _views.Count - 1; n >= 0; --n)
            {
                _views[n].Close();
            }
        }

        /// <summary>
        /// 新增视图对象实例到管理容器中
        /// </summary>
        /// <param name="view">视图对象实例</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void _AddView(CView view)
        {
            _views.Add(view);

            ViewGroup viewGroup = FindGroupByViewType(view.BeanType);
            viewGroup.OnViewGroupBinding(view);
        }

        /// <summary>
        /// 从管理容器中移除指定的视图对象实例
        /// </summary>
        /// <param name="view">视图对象实例</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void _RemoveView(CView view)
        {
            ViewGroup viewGroup = FindGroupByViewType(view.BeanType);
            viewGroup?.OnViewGroupUnbinding(view);

            _views.Remove(view);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 通过指定的视图类型获取对应视图的名称
        /// </summary>
        /// <typeparam name="T">视图类型</typeparam>
        /// <returns>返回对应视图的名称，若视图不存在则返回null</returns>
        internal string GetViewNameForType<T>() where T : CView
        {
            return GetViewNameForType(typeof(T));
        }

        /// <summary>
        /// 通过指定的视图类型获取对应视图的名称
        /// </summary>
        /// <param name="viewType">视图类型</param>
        /// <returns>返回对应视图的名称，若视图不存在则返回null</returns>
        internal string GetViewNameForType(SystemType viewType)
        {
            foreach (KeyValuePair<string, SystemType> pair in _entityClassTypes)
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
        public IList<CView> GetAllViews()
        {
            return _views;
        }

        /// <summary>
        /// 通过指定的视图类型，搜索该类型的全部实例<br/>
        /// 返回的实例列表中，包括了该类型及其子类的全部视图实例
        /// </summary>
        /// <param name="viewType">视图类型</param>
        /// <returns>返回给定类型的全部实例</returns>
        internal IList<CView> FindAllViewsByType(SystemType viewType)
        {
            IList<CView> result = new List<CView>();
            IEnumerator<CView> e = _views.GetEnumerator();
            while (e.MoveNext())
            {
                CView view = e.Current;
                if (viewType.IsAssignableFrom(view.BeanType))
                {
                    result.Add(view);
                }
            }

            // 如果搜索结果为空，则直接返回null
            if (result.Count <= 0)
            {
                result = null;
            }

            return result;
        }
    }
}
