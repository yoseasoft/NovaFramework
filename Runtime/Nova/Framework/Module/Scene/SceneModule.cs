/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
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

using UnityScene = UnityEngine.SceneManagement.Scene;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace NovaEngine
{
    /// <summary>
    /// 场景管理器，处理场景相关的加载/卸载，及同屏场景间切换等访问接口
    /// </summary>
    public sealed partial class SceneModule : ModuleObject
    {
        /// <summary>
        /// 初始主场景实例的名称
        /// </summary>
        private string _mainSceneName = string.Empty;

        /// <summary>
        /// 当前加载的场景实例的管理容器
        /// </summary>
        private IDictionary<string, SceneRecordInfo> _sceneRecordInfos = null;

        /// <summary>
        /// 场景模块事件类型
        /// </summary>
        public override int EventType => (int) ModuleEventType.Scene;

        /// <summary>
        /// 管理器对象初始化接口函数
        /// </summary>
        protected override void OnInitialize()
        {
            Logger.Assert(UnitySceneManager.sceneCount > 0, "程序启动初始环境中必须存在至少一个有效的场景实例，当前环境获取场景数据失败！");
            if (UnitySceneManager.sceneCount > 1)
            {
                Logger.Warn("当前程序启动的初始环境中存在多个场景实例，系统将随机选取一个作为主场景，其它场景实例可能在后期运行过程中被随机移除掉！");
            }

            _sceneRecordInfos = new Dictionary<string, SceneRecordInfo>();

            // 获取main场景，这是项目启动的基础场景组件
            // 若当前已启动了多个场景，则默认选择第一个场景作为主场景
            // 需要注意的是，我们选择的主场景可能不是挂载了程序调度脚本所在的场景
            // 所以可能在后续的运行过程中意外关闭了调度脚本所在的场景，从而导致整个程序崩溃
            UnityScene scene = UnitySceneManager.GetSceneAt(0); // UnitySceneManager.GetActiveScene()
            Logger.Assert(scene.IsValid(), "程序运行时自动加载的主场景当前处于无效状态，调度该场景对象实例失败！");
            _mainSceneName = scene.name;

            SceneRecordInfo info = SceneRecordInfo.Create(_mainSceneName);
            info.Enabled = true;
            info.Unmovabled = true;
            info.StateType = SceneRecordInfo.SceneStateType.Complete;
            info.Scene = scene;
            _sceneRecordInfos.Add(_mainSceneName, info);

            Logger.Info("设置程序的主场景对象实例‘{%s}’成功！", _mainSceneName);
        }

        /// <summary>
        /// 管理器对象清理接口函数
        /// </summary>
        protected override void OnCleanup()
        {
            // 对象清理时清除掉全部场景
            this.RemoveAllSceneRecordInfos();
        }

        /// <summary>
        /// 管理器对象初始启动接口
        /// </summary>
        protected override void OnStartup()
        {
        }

        /// <summary>
        /// 管理器对象结束关闭接口
        /// </summary>
        protected override void OnShutdown()
        {
        }

        /// <summary>
        /// 管理器对象垃圾回收调度接口
        /// </summary>
        protected override void OnDump()
        {
        }

        private void SendEvent(int eventID, object data)
        {
            SceneEventArgs e = this.AcquireEvent<SceneEventArgs>();
            e.Protocol = eventID;
            e.Data = data;
            this.SendEvent(e);
        }

        /// <summary>
        /// 场景管理器内部事务更新接口
        /// </summary>
        protected override void OnUpdate()
        {
            foreach (KeyValuePair<string, SceneRecordInfo> pair in _sceneRecordInfos)
            {
                SceneRecordInfo info = pair.Value;
                if (SceneRecordInfo.SceneStateType.Loading == info.StateType)
                {
                    GooAsset.Scene assetScene = info.AssetScene;
                    this.SendEvent((int) ProtocolType.Progressed, "{\"sceneName\":\"" + pair.Key + "\",\"progress\":" + assetScene.Progress + "}");
                }
            }
        }

        protected override void OnLateUpdate()
        {
        }

        /// <summary>
        /// 加载指定地址的场景对象实例
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="sceneAddress">资源地址</param>
        public GooAsset.Scene LoadScene(string sceneName, string sceneAddress, System.Action<GooAsset.Scene> completed = null)
        {
            SceneRecordInfo info = null;
            if (_sceneRecordInfos.ContainsKey(sceneName))
            {
                info = _sceneRecordInfos[sceneName];
                // 加载中或加载完成两种情况下均直接返回当前资源数据对象
                if (SceneRecordInfo.SceneStateType.Loading == info.StateType || SceneRecordInfo.SceneStateType.Complete == info.StateType)
                {
                    return info.AssetScene;
                }
            }

            if (null != info)
            {
                // 状态移除，暂时先直接移除
                _sceneRecordInfos.Remove(sceneName);
                info.Destroy();
                info = null;
            }

            GooAsset.Scene assetScene = ResourceModule.LoadSceneAsync(sceneAddress, true, completed);
            info = SceneRecordInfo.Create(sceneName);
            info.StateType = SceneRecordInfo.SceneStateType.Loading;
            info.AssetScene = assetScene;
            _sceneRecordInfos.Add(sceneName, info);

            assetScene.completed += scene =>
            {
                UnityScene unityScene = UnitySceneManager.GetSceneByName(sceneName);
                Logger.Assert(unityScene.IsValid(), $"检测到当前世界容器中不存在指定名称为‘{sceneName}’的场景实例，异步加载场景成功后的回调查询操作失败！");

                //if (false == unityScene.IsValid())
                //{
                //    rec.Enabled = false;
                //    rec.StateType = SceneRecObject.EStateType.Fault;
                //    rec.AssetScene = null;
                //}

                info.Enabled = true;
                info.StateType = SceneRecordInfo.SceneStateType.Complete;
                info.Scene = unityScene;

                // 重置激活场景
                ReactivationScene();

                SendEvent((int) ProtocolType.Loaded, sceneName);
            };

            return assetScene;
        }

        /// <summary>
        /// 卸载指定名称的场景对象实例
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        public void UnloadScene(string sceneName)
        {
            SceneRecordInfo info = null;
            if (false == _sceneRecordInfos.TryGetValue(sceneName, out info))
            {
                Logger.Warn("检测到当前世界容器中不存在指定名称为‘{%s}’的场景实例，卸载场景对象操作失败！", sceneName);
                return;
            }

            if (info.Unmovabled)
            {
                Logger.Warn("检测到指定名称为‘{%s}’的场景实例被标识为不可移除状态，卸载场景对象操作失败！", sceneName);
                return;
            }

            if (SceneRecordInfo.SceneStateType.Complete == info.StateType)
            {
                // 2025-06-20:
                // 为什么新的资源库此处不能使用该代码，是因为在资源管理的场景操作句柄中重复使用了该代码
                // 因为异步卸载资源可能导致场景中材质的丢失，所以也把该操作和资源异步清理统一处理了
                // UnitySceneManager.UnloadSceneAsync(sceneName);
            }

            _sceneRecordInfos.Remove(sceneName);
            info.Destroy();

            // 重置激活场景
            ReactivationScene();

            this.SendEvent((int) ProtocolType.Unloaded, sceneName);
        }

        /// <summary>
        /// 通过场景名称获取对应的场景运行时信息数据对象实例
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <returns>返回给定名称对应的场景运行时信息数据对象实例</returns>
        public SceneRecordInfo GetSceneRecordInfo(string sceneName)
        {
            SceneRecordInfo info = null;
            if (_sceneRecordInfos.TryGetValue (sceneName, out info))
            {
                return info;
            }

            return null;
        }

        /// <summary>
        /// 获取当前激活的主控场景对象实例的名称
        /// </summary>
        /// <returns>返回当前激活的场景名称</returns>
        public string GetActiveSceneName()
        {
            UnityScene scene = UnitySceneManager.GetActiveScene();

            return scene.name;
        }

        /// <summary>
        /// 将指定名称的目标场景对象激活为主控场景
        /// </summary>
        /// <param name="sceneName">目标场景名称</param>
        public void SetActiveScene(string sceneName)
        {
            if (this.GetActiveSceneName() == sceneName)
            {
                // 当前激活场景与目标场景一致，无需切换工作
                return;
            }

            SceneRecordInfo info = null;
            if (false == _sceneRecordInfos.TryGetValue(sceneName, out info))
            {
                Logger.Warn("检测到当前场景容器中不存在名称为‘{0}’的场景实例，激活该场景对象失败！", sceneName);
                return;
            }

            UnityScene scene = info.Scene;
            UnitySceneManager.SetActiveScene(scene);
        }

        /// <summary>
        /// 移除当前模块中激活的全部场景实例
        /// PS.仅有main场景生命周期不受此接口影响
        /// </summary>
        private void RemoveAllSceneRecordInfos()
        {
            // 记录除主场景以外的其它场景名称
            IList<string> keys = new List<string>(_sceneRecordInfos.Count);
            foreach (KeyValuePair<string, SceneRecordInfo> pair in _sceneRecordInfos)
            {
                SceneRecordInfo info = pair.Value;
                if (false == info.Unmovabled)
                {
                    keys.Add(info.Name);
                }
            }

            for (int n = 0; n < keys.Count; ++n)
            {
                UnloadScene(keys[n]);
            }
        }

        /// <summary>
        /// 重新激活当前的有效场景实例，若当前不存在任何加载的场景实例，则默认将主场景处理为激活状态
        /// </summary>
        public void ReactivationScene()
        {
            foreach (KeyValuePair<string, SceneRecordInfo> pair in _sceneRecordInfos)
            {
                SceneRecordInfo info = pair.Value;
                if (info.Enabled && false == info.Unmovabled && SceneRecordInfo.SceneStateType.Complete == info.StateType)
                {
                    SetActiveScene(info.Name);
                    return;
                }
            }

            Logger.Info("从当前激活场景列表中未找到任何有效的动态场景实例，只能使用主场景对象作为当前激活场景！");
            SetActiveScene(_mainSceneName);
        }
    }
}
