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

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Cysharp.Threading.Tasks;

namespace GameEngine
{
    /// <summary>
    /// 场景模块封装的句柄对象类
    /// 模块具体功能接口请参考<see cref="NovaEngine.Module.SceneModule"/>类
    /// </summary>
    public sealed partial class SceneHandler : GenericEntityHandler<CScene>
    {
        /// <summary>
        /// 句柄对象锁实例
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// 当前运行的场景对象类型
        /// </summary>
        private Type _currentSceneType;
        /// <summary>
        /// 当前待命的场景对象类型
        /// </summary>
        private Type _waitingSceneType;
        /// <summary>
        /// 当前待命的场景上下文数据
        /// </summary>
        private object _waitingSceneContext;

        /// <summary>
        /// 句柄对象的单例访问获取接口
        /// </summary>
        public static SceneHandler Instance => HandlerManagement.SceneHandler;

        /// <summary>
        /// 句柄对象默认构造函数
        /// </summary>
        public SceneHandler()
        {
        }

        /// <summary>
        /// 句柄对象析构函数
        /// </summary>
        ~SceneHandler()
        {
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
            CScene scene = GetCurrentScene();
            if (null != scene)
            {
                CallEntityDestroyProcess(scene);
                Call(scene, scene.Shutdown, AspectBehaviourType.Shutdown);
                RemoveEntity(scene);

                // 回收场景实例
                ReleaseInstance(scene);

                _currentSceneType = null;
            }

            _waitingSceneType = null;
            _waitingSceneContext = null;

            // 清理场景类型注册列表
            UnregisterAllSceneClasses();

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
            if (null != _waitingSceneType)
            {
                lock (_lock)
                {
                    ChangeScene(_waitingSceneType, _waitingSceneContext);

                    // 注意，不能在一个场景实例的启动期间跳转到另一个场景，这将导致用户数据绑定异常

                    // 每次替换场景后，都将待命的场景重置掉
                    _waitingSceneType = null;

                    // 使用完成后将上下文数据清除
                    _waitingSceneContext = null;
                }
            }

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

        /// <summary>
        /// 获取当前运行的场景实例
        /// </summary>
        /// <returns>返回当前运行的场景实例，若没有则返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CScene GetCurrentScene()
        {
            if (Entities.Count > 0)
            {
                if (Entities.Count > 1)
                {
                    Debugger.Error("There can only be one valid scene instance in the containers, don't multiple insert.");
                }

                return Entities[0];
            }

            return null;
        }

        /// <summary>
        /// 替换管理器中当前的场景实例
        /// 注意，替换操作并非立即执行，而是在下一次更新前进行替换
        /// 若在此之前多次进行替换操作，将只保留最后一次作为最终的场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="userData">用户数据</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReplaceScene(string sceneName, object userData = null)
        {
            if (_entityClassTypes.TryGetValue(sceneName, out Type sceneType))
            {
                ReplaceScene(sceneType, userData);
            }
        }

        /// <summary>
        /// 替换管理器中当前的场景实例
        /// 注意，替换操作并非立即执行，而是在下一次更新前进行替换
        /// 若在此之前多次进行替换操作，将只保留最后一次作为最终的场景
        /// </summary>
        /// <typeparam name="T">场景类型</typeparam>
        /// <param name="userData">用户数据</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReplaceScene<T>(object userData = null) where T : CScene
        {
            Type sceneType = typeof(T);
            ReplaceScene(sceneType, userData);
        }

        /// <summary>
        /// 替换管理器中当前的场景实例
        /// 注意，替换操作并非立即执行，而是在下一次更新前进行替换
        /// 若在此之前多次进行替换操作，将只保留最后一次作为最终的场景
        /// </summary>
        /// <param name="sceneType">场景类型</param>
        /// <param name="userData">用户数据</param>
        public void ReplaceScene(Type sceneType, object userData = null)
        {
            Debugger.Assert(sceneType, NovaEngine.ErrorText.InvalidArguments);
            if (sceneType == _currentSceneType)
            {
                Debugger.Warn("The replace scene '{%t}' must be not equals to current scene, replaced it failed.", sceneType);
                return;
            }

            if (false == _entityClassTypes.Values.Contains(sceneType))
            {
                Debugger.Error("Could not found any correct scene class with target type '{%t}', replaced scene failed.", sceneType);
                return;
            }

            if (sceneType == _waitingSceneType)
            {
                Debugger.Warn("The target scene '{%t}' was already in a waiting state, repeat setted it failed.", sceneType);
                return;
            }

            _waitingSceneType = sceneType;
            _waitingSceneContext = userData;
        }

        /// <summary>
        /// 通过指定的场景类型动态创建一个对应的场景对象实例
        /// </summary>
        /// <param name="sceneType">场景类型</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private CScene CreateScene(Type sceneType)
        {
            if (false == _entityClassTypes.Values.Contains(sceneType))
            {
                Debugger.Error("Unknown scene type '{%t}', create the scene instance failed.", sceneType);
                return null;
            }

            return CreateInstance(sceneType) as CScene;
        }

        /// <summary>
        /// 将当前场景切换到指定名称的场景实例
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="userData">用户数据</param>
        /// <returns>返回改变的目标场景实例，若切换场景失败返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CScene ChangeScene(string sceneName, object userData = null)
        {
            if (_entityClassTypes.TryGetValue(sceneName, out Type sceneType))
            {
                return ChangeScene(sceneType, userData);
            }

            return null;
        }

        /// <summary>
        /// 将当前场景切换到指定类型的场景实例
        /// </summary>
        /// <typeparam name="T">场景类型</typeparam>
        /// <param name="userData">用户数据</param>
        /// <returns>返回改变的目标场景实例，若切换场景失败返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ChangeScene<T>(object userData = null) where T : CScene
        {
            Type sceneType = typeof(T);
            return ChangeScene(sceneType, userData) as T;
        }

        /// <summary>
        /// 将当前场景切换到指定类型的场景实例
        /// </summary>
        /// <param name="sceneType">场景类型</param>
        /// <param name="userData">用户数据</param>
        /// <returns>返回改变的目标场景实例，若切换场景失败返回null</returns>
        public CScene ChangeScene(Type sceneType, object userData = null)
        {
            CScene scene = GetCurrentScene();
            if (null != scene)
            {
                // 相同的场景无需切换
                if (scene.BeanType == sceneType)
                {
                    return null;
                }

                _Profiler.CallStat(Profiler.Statistics.StatCode.SceneExit, scene);

                // 销毁场景对象实例
                CallEntityDestroyProcess(scene);

                // 关闭场景对象实例
                Call(scene, scene.Shutdown, AspectBehaviourType.Shutdown);

                // 移除实例
                RemoveEntity(scene);

                // 回收场景实例
                ReleaseInstance(scene);

                _currentSceneType = null;
            }

            scene = CreateScene(sceneType);
            scene.UserData = userData;

            if (null == scene || false == AddEntity(scene))
            {
                Debugger.Error("Create or register the scene instance '{%t}' failed.", sceneType);
                return null;
            }

            // 设置当前场景类型
            _currentSceneType = sceneType;

            // 设置当前场景后再启动场景
            Call(scene, scene.Startup, AspectBehaviourType.Startup);

            // 唤醒场景对象实例
            CallEntityAwakeProcess(scene);

            _Profiler.CallStat(Profiler.Statistics.StatCode.SceneEnter, scene);

            return scene;
        }

        /// <summary>
        /// 加载指定名称及路径的场景资源对象实例
        /// </summary>
        /// <param name="assetName">场景资源名称</param>
        /// <param name="url">场景资源路径</param>
        /// <param name="completed">结束回调</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GooAsset.Scene LoadSceneAsset(string assetName, string url, Action<GooAsset.Scene> completed = null)
        {
            return SceneModule.LoadScene(assetName, url, completed);
        }

        /// <summary>
        /// 异步加载场景资源
        /// </summary>
        /// <param name="assetName">场景资源名称</param>
        /// <param name="url">场景资源路径</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask<GooAsset.Scene> LoadSceneAssetAsync(string assetName, string url)
        {
            // string assetName = System.IO.Path.GetFileNameWithoutExtension(assetUrl);
            return await SceneModule.LoadScene(assetName, url).Task;
        }

        /// <summary>
        /// 卸载指定名称的场景资源对象实例
        /// </summary>
        /// <param name="assetName">场景资源名称</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnloadSceneAsset(string assetName)
        {
            SceneModule.UnloadScene(assetName);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 通过指定的场景类型获取对应场景的名称
        /// </summary>
        /// <typeparam name="T">场景类型</typeparam>
        /// <returns>返回对应场景的名称，若场景不存在则返回null</returns>
        internal string GetSceneNameForType<T>() where T : CScene
        {
            return GetSceneNameForType(typeof(T));
        }

        /// <summary>
        /// 通过指定的场景类型获取对应场景的名称
        /// </summary>
        /// <param name="sceneType">场景类型</param>
        /// <returns>返回对应场景的名称，若场景不存在则返回null</returns>
        internal string GetSceneNameForType(Type sceneType)
        {
            foreach (KeyValuePair<string, Type> pair in _entityClassTypes)
            {
                if (pair.Value == sceneType)
                {
                    return pair.Key;
                }
            }

            return null;
        }

        #region 场景对象类型组件快速访问的便捷接口函数

        /// <summary>
        /// 通过组件名称在当前场景对象实例中获取对应的组件对象实例
        /// </summary>
        /// <param name="name">组件名称</param>
        /// <returns>若查找组件实例成功则返回对应实例的引用，否则返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CComponent GetCurrentSceneComponent(string name)
        {
            CScene scene = GetCurrentScene();
            return scene?.GetComponent(name);
        }

        /// <summary>
        /// 通过组件类型在当前场景对象实例中获取对应的组件对象实例
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>若查找组件实例成功则返回对应实例的引用，否则返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetCurrentSceneComponent<T>() where T : CComponent
        {
            CScene scene = GetCurrentScene();
            return scene?.GetComponent<T>();
        }

        /// <summary>
        /// 通过组件类型在当前场景对象实例中获取对应的组件对象实例
        /// </summary>
        /// <param name="componentType">组件类型</param>
        /// <returns>若查找组件实例成功则返回对应实例的引用，否则返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CComponent GetCurrentSceneComponent(Type componentType)
        {
            CScene scene = GetCurrentScene();
            return scene?.GetComponent(componentType);
        }

        #endregion

        #region 场景对象类型注册绑定相关的接口函数

        /// <summary>
        /// 注册指定的场景名称及对应的对象类到当前的句柄管理容器中
        /// 注意，注册的对象类必须继承自<see cref="GameEngine.CScene"/>，否则无法正常注册
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="clsType">场景类型</param>
        /// <param name="priority">场景优先级</param>
        /// <returns>若场景类型注册成功则返回true，否则返回false</returns>
        private bool RegisterSceneClass(string sceneName, Type clsType, int priority)
        {
            Debugger.Assert(false == string.IsNullOrEmpty(sceneName) && null != clsType, NovaEngine.ErrorText.InvalidArguments);

            if (false == typeof(CScene).IsAssignableFrom(clsType))
            {
                Debugger.Warn(LogGroupTag.Module, "The register type '{%t}' must be inherited from 'CScene'.", clsType);
                return false;
            }

            if (false == RegisterEntityClass(sceneName, clsType, priority))
            {
                Debugger.Warn(LogGroupTag.Module, "The scene class '{%t}' was already registered, repeat added it failed.", clsType);
                return false;
            }

            // Debugger.Info(LogGroupTag.Module, "Register new scene class type '{%t}' with target name '{%s}'.", clsType, sceneName);

            return true;
        }

        /// <summary>
        /// 注销当前句柄实例绑定的所有场景类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UnregisterAllSceneClasses()
        {
            UnregisterAllEntityClasses();
        }

        #endregion
    }
}
