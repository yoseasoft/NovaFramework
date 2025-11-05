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

using Cysharp.Threading.Tasks;

using SystemType = System.Type;

namespace GameEngine
{
    /// <summary>
    /// 场景模块封装的句柄对象类
    /// 模块具体功能接口请参考<see cref="NovaEngine.SceneModule"/>类
    /// </summary>
    public sealed partial class SceneHandler : EntityHandler
    {
        /// <summary>
        /// 句柄对象锁实例
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// 当前运行的场景对象类型
        /// </summary>
        private SystemType _currentSceneType;
        /// <summary>
        /// 当前待命的场景对象类型
        /// </summary>
        private SystemType _waitingSceneType;

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
                    ChangeScene(_waitingSceneType);
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
        public override void OnEventDispatch(NovaEngine.ModuleEventArgs e)
        {
        }

        /// <summary>
        /// 获取当前运行的场景实例
        /// </summary>
        /// <returns>返回当前运行的场景实例，若没有则返回null</returns>
        public CScene GetCurrentScene()
        {
            if (Entities.Count > 0)
            {
                if (Entities.Count > 1)
                {
                    Debugger.Error("There can only be one valid scene instance in the containers, don't multiple insert.");
                }

                return Entities[0] as CScene;
            }

            return null;
        }

        /// <summary>
        /// 替换管理器中当前的场景实例
        /// 注意，替换操作并非立即执行，而是在下一次更新前进行替换
        /// 若在此之前多次进行替换操作，将只保留最后一次作为最终的场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        public void ReplaceScene(string sceneName)
        {
            SystemType sceneType;
            if (_entityClassTypes.TryGetValue(sceneName, out sceneType))
            {
                ReplaceScene(sceneType);
            }
        }

        /// <summary>
        /// 替换管理器中当前的场景实例
        /// 注意，替换操作并非立即执行，而是在下一次更新前进行替换
        /// 若在此之前多次进行替换操作，将只保留最后一次作为最终的场景
        /// </summary>
        /// <typeparam name="T">场景类型</typeparam>
        public void ReplaceScene<T>() where T : CScene
        {
            SystemType sceneType = typeof(T);
            ReplaceScene(sceneType);
        }

        /// <summary>
        /// 替换管理器中当前的场景实例
        /// 注意，替换操作并非立即执行，而是在下一次更新前进行替换
        /// 若在此之前多次进行替换操作，将只保留最后一次作为最终的场景
        /// </summary>
        /// <param name="sceneType">场景类型</param>
        public void ReplaceScene(SystemType sceneType)
        {
            Debugger.Assert(null != sceneType, "Invalid arguments.");
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
        }

        /// <summary>
        /// 通过指定的场景类型动态创建一个对应的场景对象实例
        /// </summary>
        /// <param name="sceneType">场景类型</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        private CScene CreateScene(SystemType sceneType)
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
        /// <returns>返回改变的目标场景实例，若切换场景失败返回null</returns>
        public CScene ChangeScene(string sceneName)
        {
            SystemType sceneType;
            if (_entityClassTypes.TryGetValue(sceneName, out sceneType))
            {
                return ChangeScene(sceneType);
            }

            return null;
        }

        /// <summary>
        /// 将当前场景切换到指定类型的场景实例
        /// </summary>
        /// <typeparam name="T">场景类型</typeparam>
        /// <returns>返回改变的目标场景实例，若切换场景失败返回null</returns>
        public T ChangeScene<T>() where T : CScene
        {
            SystemType sceneType = typeof(T);
            return ChangeScene(sceneType) as T;
        }

        /// <summary>
        /// 将当前场景切换到指定类型的场景实例
        /// </summary>
        /// <param name="sceneType">场景类型</param>
        /// <returns>返回改变的目标场景实例，若切换场景失败返回null</returns>
        public CScene ChangeScene(SystemType sceneType)
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

                CallEntityDestroyProcess(scene);
                Call(scene, scene.Shutdown, AspectBehaviourType.Shutdown);
                RemoveEntity(scene);

                // 回收场景实例
                ReleaseInstance(scene);

                _currentSceneType = null;
            }

            scene = CreateScene(sceneType);
            if (null == scene || false == AddEntity(scene))
            {
                Debugger.Error("Create or register the scene instance '{%t}' failed.", sceneType);
                return null;
            }

            _currentSceneType = sceneType;
            // 每次替换场景后，都将待命的场景重置掉
            _waitingSceneType = null;

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
        public GooAsset.Scene LoadSceneAsset(string assetName, string url, System.Action<GooAsset.Scene> completed = null)
        {
            return SceneModule.LoadScene(assetName, url, completed);
        }

        /// <summary>
        /// 异步加载场景资源
        /// </summary>
        /// <param name="assetName">场景资源名称</param>
        /// <param name="url">场景资源路径</param>
        public async UniTask<GooAsset.Scene> LoadSceneAssetAsync(string assetName, string url)
        {
            // string assetName = System.IO.Path.GetFileNameWithoutExtension(assetUrl);
            return await SceneModule.LoadScene(assetName, url).Task;
        }

        /// <summary>
        /// 卸载指定名称的场景资源对象实例
        /// </summary>
        /// <param name="assetName">场景资源名称</param>
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
        internal string GetSceneNameForType(SystemType sceneType)
        {
            foreach (KeyValuePair<string, SystemType> pair in _entityClassTypes)
            {
                if (pair.Value == sceneType)
                {
                    return pair.Key;
                }
            }

            return null;
        }

        #region 场景对象类注册接口函数

        /// <summary>
        /// 注册指定的场景名称及对应的对象类到当前的句柄管理容器中
        /// 注意，注册的对象类必须继承自<see cref="GameEngine.CScene"/>，否则无法正常注册
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="clsType">场景类型</param>
        /// <param name="priority">场景优先级</param>
        /// <returns>若场景类型注册成功则返回true，否则返回false</returns>
        private bool RegisterSceneClass(string sceneName, SystemType clsType, int priority)
        {
            Debugger.Assert(false == string.IsNullOrEmpty(sceneName) && null != clsType, "Invalid arguments");

            if (false == typeof(CScene).IsAssignableFrom(clsType))
            {
                Debugger.Warn("The register type '{%t}' must be inherited from 'CScene'.", clsType);
                return false;
            }

            if (false == RegisterEntityClass(sceneName, clsType, priority))
            {
                Debugger.Warn("The scene class '{%t}' was already registed, repeat added it failed.", clsType);
                return false;
            }

            // Debugger.Info("Register new scene class type '{%t}' with target name '{%s}'.", clsType, sceneName);

            return true;
        }

        /// <summary>
        /// 注销当前句柄实例绑定的所有场景类型
        /// </summary>
        private void UnregisterAllSceneClasses()
        {
            UnregisterAllEntityClasses();
        }

        #endregion
    }
}
