/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
/// Copyright (C) 2026, Hurley, Independent Studio.
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
using System.Runtime.CompilerServices;

using UnityEngine.SceneManagement;
using NovaFramework.AssetLoader;

namespace NovaEngine.Module
{
    /// <summary>
    /// 场景管理器对象类，处理场景相关的加载/卸载，及同屏场景间切换等访问接口
    /// </summary>
    internal sealed partial class SceneModule : ModuleObject
    {
        /// <summary>
        /// 初始主场景实例的名称
        /// </summary>
        private string _mainSceneName = string.Empty;

        /// <summary>
        /// 当前加载的场景实例的管理容器
        /// </summary>
        private IDictionary<string, SceneOperationRecord> _sceneOperationRecords;

        /// <summary>
        /// 场景模块事件类型
        /// </summary>
        public override sealed int EventType => (int) ModuleEventType.Scene;

        /// <summary>
        /// 管理器对象初始化接口函数
        /// </summary>
        protected override sealed void OnInitialize()
        {
            CLogger.Assert(SceneManager.sceneCount > 0, "程序启动初始环境中必须存在至少一个有效的场景实例，当前环境获取场景数据失败！");
            if (SceneManager.sceneCount > 1)
            {
                CLogger.Warn("当前程序启动的初始环境中存在多个场景实例，引擎将选取首个实例作为主场景，其它场景实例可能在后期运行过程中被随机移除掉！");
            }

            _sceneOperationRecords = new Dictionary<string, SceneOperationRecord>();

            // 获取main场景，这是项目启动的基础场景组件
            // 若当前已启动了多个场景，则默认选择第一个场景作为主场景
            // 需要注意的是，我们选择的主场景可能不是挂载了程序调度脚本所在的场景
            // 所以可能在后续的运行过程中意外关闭了调度脚本所在的场景，从而导致整个程序崩溃
            Scene scene = SceneManager.GetSceneAt(0); // UnitySceneManager.GetActiveScene()
            CLogger.Assert(scene.IsValid(), "程序运行时自动加载的主场景当前处于无效状态，调度该场景对象实例失败！");
            _mainSceneName = scene.name;

            SceneOperationRecord rec = SceneOperationRecord.Create(_mainSceneName);
            rec.Enabled = true;
            rec.Unmovabled = true;
            rec.StateType = SceneOperationRecord.SceneStateType.Complete;
            rec.SceneObject = scene;
            _sceneOperationRecords.Add(_mainSceneName, rec);

            CLogger.Info("设置程序的主场景对象实例‘{%s}’成功！", _mainSceneName);
        }

        /// <summary>
        /// 管理器对象清理接口函数
        /// </summary>
        protected override sealed void OnCleanup()
        {
            // 对象清理时清除掉全部场景
            this.RemoveAllSceneOperationRecords();
        }

        /// <summary>
        /// 管理器对象初始启动接口
        /// </summary>
        protected override sealed void OnStartup()
        {
        }

        /// <summary>
        /// 管理器对象结束关闭接口
        /// </summary>
        protected override sealed void OnShutdown()
        {
        }

        /// <summary>
        /// 管理器对象垃圾回收调度接口
        /// </summary>
        protected override sealed void OnDump()
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SendEvent(ProtocolType protocol, object data)
        {
            SendEvent((int) protocol, data);
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
        protected override sealed void OnUpdate()
        {
            foreach (KeyValuePair<string, SceneOperationRecord> pair in _sceneOperationRecords)
            {
                SceneOperationRecord rec = pair.Value;
                if (SceneOperationRecord.SceneStateType.Loading == rec.StateType)
                {
                    ISceneHandler sceneHandler = rec.SceneHandler;
                    this.SendEvent(ProtocolType.Progressed, "{\"sceneName\":\"" + pair.Key + "\",\"progress\":" + sceneHandler.LoadingProgress + "}");
                }
            }
        }

        protected override sealed void OnLateUpdate()
        {
        }

        /// <summary>
        /// 加载指定地址的场景对象实例
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="url">资源地址</param>
        /// <returns>返回场景资源句柄实例</returns>
        public ISceneHandler LoadScene(string sceneName, string url)
        {
            SceneOperationRecord rec = null;
            if (_sceneOperationRecords.ContainsKey(sceneName))
            {
                rec = _sceneOperationRecords[sceneName];
                // 加载中或加载完成两种情况下均直接返回当前资源数据对象
                if (SceneOperationRecord.SceneStateType.Loading == rec.StateType || SceneOperationRecord.SceneStateType.Complete == rec.StateType)
                {
                    return rec.SceneHandler;
                }
            }

            if (null != rec)
            {
                // 状态移除，暂时先直接移除
                _sceneOperationRecords.Remove(sceneName);
                rec.Destroy();
                rec = null;
            }

            ISceneHandler sceneHandler = GetModule<ResourceModule>().LoadSceneAsync(url);
            rec = SceneOperationRecord.Create(sceneName);
            rec.StateType = SceneOperationRecord.SceneStateType.Loading;
            rec.SceneHandler = sceneHandler;
            _sceneOperationRecords.Add(sceneName, rec);

            sceneHandler.Completed += (handler) =>
            {
                // Scene sceneObject = SceneManager.GetSceneByName(sceneName);
                Scene sceneObject = handler.SceneObject;
                CLogger.Assert(sceneObject.IsValid(), "检测到当前世界容器中不存在指定名称为‘{%s}’的场景实例，异步加载场景成功后的回调查询操作失败！", sceneName);

                if (false == sceneObject.IsValid())
                {
                    rec.Enabled = false;
                    rec.StateType = SceneOperationRecord.SceneStateType.Fault;
                    rec.SceneHandler = null;

                    SendEvent(ProtocolType.Exception, sceneName);
                }
                else
                {
                    rec.Enabled = true;
                    rec.StateType = SceneOperationRecord.SceneStateType.Complete;
                    rec.SceneObject = sceneObject;

                    // 重置激活场景
                    ReactivationScene();

                    SendEvent(ProtocolType.Loaded, sceneName);
                }
            };

            return sceneHandler;
        }

        /// <summary>
        /// 卸载指定名称的场景对象实例
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        public void UnloadScene(string sceneName)
        {
            if (false == _sceneOperationRecords.TryGetValue(sceneName, out SceneOperationRecord rec))
            {
                CLogger.Warn("检测到当前世界容器中不存在指定名称为‘{%s}’的场景实例，卸载场景对象操作失败！", sceneName);
                return;
            }

            if (rec.Unmovabled)
            {
                CLogger.Warn("检测到指定名称为‘{%s}’的场景实例被标识为不可移除状态，卸载场景对象操作失败！", sceneName);
                return;
            }

            _sceneOperationRecords.Remove(sceneName);
            rec.Destroy();

            // 重置激活场景
            ReactivationScene();

            this.SendEvent(ProtocolType.Unloaded, sceneName);
        }

        /// <summary>
        /// 通过场景名称获取对应的场景运行时资源对象实例
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="scene">场景对象实例</param>
        /// <returns>若查找资源对象实例成功则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetScene(string sceneName, out Scene scene)
        {
            if (_sceneOperationRecords.TryGetValue (sceneName, out SceneOperationRecord rec))
            {
                scene = rec.SceneObject;
                return true;
            }

            scene = default;
            return false;
        }

        /// <summary>
        /// 获取当前激活的主控场景对象实例的名称
        /// </summary>
        /// <returns>返回当前激活的场景名称</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetActiveSceneName()
        {
            Scene scene = SceneManager.GetActiveScene();

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

            if (false == _sceneOperationRecords.TryGetValue(sceneName, out SceneOperationRecord rec))
            {
                CLogger.Warn("检测到当前场景容器中不存在名称为‘{%s}’的场景实例，激活该场景对象失败！", sceneName);
                return;
            }

            Scene scene = rec.SceneObject;
            SceneManager.SetActiveScene(scene);
        }

        /// <summary>
        /// 移除当前模块中激活的全部场景实例
        /// PS.仅有main场景生命周期不受此接口影响
        /// </summary>
        private void RemoveAllSceneOperationRecords()
        {
            // 记录除主场景以外的其它场景名称
            IList<string> keys = new List<string>(_sceneOperationRecords.Count);
            foreach (KeyValuePair<string, SceneOperationRecord> pair in _sceneOperationRecords)
            {
                SceneOperationRecord rec = pair.Value;
                if (false == rec.Unmovabled)
                {
                    keys.Add(rec.Name);
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
            foreach (KeyValuePair<string, SceneOperationRecord> pair in _sceneOperationRecords)
            {
                SceneOperationRecord rec = pair.Value;
                if (rec.Enabled && false == rec.Unmovabled && SceneOperationRecord.SceneStateType.Complete == rec.StateType)
                {
                    SetActiveScene(rec.Name);
                    return;
                }
            }

            CLogger.Info("从当前激活场景列表中未找到任何有效的动态场景实例，只能使用主场景对象作为当前激活场景！");
            SetActiveScene(_mainSceneName);
        }
    }
}
