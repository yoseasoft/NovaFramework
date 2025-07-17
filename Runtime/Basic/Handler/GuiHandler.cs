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
using System.Linq;

using SystemType = System.Type;

namespace GameEngine
{
    /// <summary>
    /// 用户界面模块封装的句柄对象类
    /// 模块具体功能接口请参考<see cref="NovaEngine.GuiModule"/>类
    /// </summary>
    public sealed partial class GuiHandler : EcsmHandler
    {
        /// <summary>
        /// 视图对象类型映射注册管理容器
        /// </summary>
        private readonly IDictionary<string, SystemType> _viewClassTypes;

        /// <summary>
        /// 视图功能类型映射注册管理容器
        /// </summary>
        private readonly IDictionary<string, int> _viewFunctionTypes;

        /// <summary>
        /// 当前环境下所有实例化的视图对象
        /// </summary>
        private readonly IList<CView> _views;

        /// <summary>
        /// 句柄对象的单例访问获取接口
        /// </summary>
        public static GuiHandler Instance => HandlerManagement.GuiHandler;

        /// <summary>
        /// 句柄对象默认构造函数
        /// </summary>
        public GuiHandler()
        {
            // 初始化视图类注册容器
            _viewClassTypes = new Dictionary<string, SystemType>();
            _viewFunctionTypes = new Dictionary<string, int>();

            // 初始化视图实例容器
            _views = new List<CView>();
        }

        /// <summary>
        /// 句柄对象析构函数
        /// </summary>
        ~GuiHandler()
        {
            // 清理视图类注册容器
            _viewClassTypes.Clear();
            _viewFunctionTypes.Clear();
        }

        /// <summary>
        /// 句柄对象内置初始化接口函数
        /// </summary>
        /// <returns>若句柄对象初始化成功则返回true，否则返回false</returns>
        protected override bool OnInitialize()
        {
            if (false == base.OnInitialize()) return false;

            // 启动视图辅助工具类
            GuiHelper.Startup();

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
            GuiHelper.Shutdown();

            base.OnCleanup();
        }

        /// <summary>
        /// 句柄对象内置刷新接口
        /// </summary>
        protected override void OnUpdate()
        {
            base.OnUpdate();

            // 刷新视图辅助工具类
            GuiHelper.Update();
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
        /// <param name="funcType">功能类型</param>
        /// <returns>若视图类型注册成功则返回true，否则返回false</returns>
        private bool RegisterViewClass(string viewName, SystemType clsType, int funcType)
        {
            Debugger.Assert(false == string.IsNullOrEmpty(viewName) && null != clsType, "Invalid arguments");

            if (false == typeof(CView).IsAssignableFrom(clsType))
            {
                Debugger.Warn("The register type {0} must be inherited from 'CView'.", clsType.Name);
                return false;
            }

            if (_viewClassTypes.ContainsKey(viewName))
            {
                Debugger.Warn("The view name '{0}' was already registed, repeat add will be override old name.", viewName);
                _viewClassTypes.Remove(viewName);
            }

            _viewClassTypes.Add(viewName, clsType);
            if (funcType > 0)
            {
                _viewFunctionTypes.Add(viewName, funcType);
            }

            return true;
        }

        /// <summary>
        /// 注销当前句柄实例绑定的所有视图类型
        /// </summary>
        private void UnregisterAllViewClasses()
        {
            _viewClassTypes.Clear();
            _viewFunctionTypes.Clear();
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
            if (false == _viewClassTypes.TryGetValue(viewName, out viewType))
            {
                Debugger.Warn("Could not found any correct view class with target name '{0}', opened view failed.", viewName);
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
            if (false == _viewClassTypes.Values.Contains(viewType))
            {
                Debugger.Error("Could not found any correct view class with target type '{0}', opened view failed.", viewType.FullName);
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
            Debugger.Assert(null != viewType, "Invalid arguments.");
            if (false == _viewClassTypes.Values.Contains(viewType))
            {
                Debugger.Error("Could not found any correct view class with target type '{0}', opened view failed.", viewType.FullName);

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
                Debugger.Warn("The view instance '{0}' initialization for error, added it failed.", viewType.FullName);

                // return null;
                throw new NovaEngine.CFrameworkException();
            }

            // 添加实例到管理容器中
            _views.Add(view);

            await view.CreateWindow();
            if (!view.IsLoaded)
            {
                _views.Remove(view);
                RemoveEntity(view);

                // 回收视图实例
                ReleaseInstance(view);

                Debugger.Throw("Open target ui {%f} failed", viewType);
                return null;
            }

            // 启动视图对象
            Call(view.Startup);

            // 唤醒视图对象
            CallEntityAwakeProcess(view);

            await Cysharp.Threading.Tasks.UniTask.WaitUntil(() => view.IsReady, cancellationToken : view.CancellationTokenSource.Token);

            view.ShowWindow();

            ViewStatModule.CallStatAction(ViewStatModule.ON_VIEW_CREATE_CALL, view);

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
            if (false == _viewClassTypes.TryGetValue(viewName, out viewType))
            {
                Debugger.Warn("Could not found any correct view class with target name '{0}', found view failed.", viewName);
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
            return _views.OfType<T>().Any();
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
                if (view.GetType() == viewType)
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
            if (false == _viewClassTypes.TryGetValue(viewName, out viewType))
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
            if (false == _viewClassTypes.Values.Contains(viewType))
            {
                Debugger.Error("Could not found any correct view class with target type '{0}', found view failed.", viewType.FullName);
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
                if (view.GetType() == viewType)
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
            if (false == _viewClassTypes.Values.Contains(viewType))
            {
                Debugger.Error("Could not found any correct view class with target type '{0}', found view failed.", viewType.FullName);
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
                if (view.GetType() == viewType)
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
                Debugger.Error("Could not found any view reference '{0}' with manage container, removed it failed.", view.GetType().FullName);
                return;
            }

            // 视图尚未关闭，则先执行视图关闭操作
            if (false == view.IsClosed)
            {
                ViewStatModule.CallStatAction(ViewStatModule.ON_VIEW_CLOSE_CALL, view);

                // 销毁视图对象
                CallEntityDestroyProcess(view);

                // 关闭视图实例
                view.__Close();
                return;
            }

            _views.Remove(view);
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
                Debugger.Error("Could not found any view reference '{0}' with manage container, removed it failed.", view.GetType().FullName);
                return;
            }

            // 刷新状态时推到销毁队列中
            // if (view.IsOnUpdatingStatus())
            if (view.CurrentLifecycleProcessRunning)
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
            if (false == _viewClassTypes.TryGetValue(viewName, out viewType))
            {
                Debugger.Warn("Could not found any correct view class with target name '{0}', closed view failed.", viewName);
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
            foreach (CView view in _views.Reverse<CView>())
            {
                if (view.GetType() == viewType)
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
            foreach (KeyValuePair<string, SystemType> pair in _viewClassTypes)
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
                if (viewType.IsAssignableFrom(view.GetType()))
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
