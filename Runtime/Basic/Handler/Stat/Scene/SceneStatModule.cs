/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using SystemDateTime = System.DateTime;

namespace GameEngine
{
    /// <summary>
    /// 场景统计模块，对场景模块对象提供数据统计所需的接口函数
    /// </summary>
    public sealed class SceneStatModule : HandlerStatSingleton<SceneStatModule>, IStatModule
    {
        public const int ON_SCENE_ENTER_CALL = 1;
        public const int ON_SCENE_EXIT_CALL = 2;

        /// <summary>
        /// 场景访问统计信息容器列表
        /// </summary>
        private IList<SceneStatInfo> _sceneStatInfos = null;

        /// <summary>
        /// 初始化统计模块实例的回调接口
        /// </summary>
        protected override void OnInitialize()
        {
            _sceneStatInfos = new List<SceneStatInfo>();
        }

        /// <summary>
        /// 清理统计模块实例的回调接口
        /// </summary>
        protected override void OnCleanup()
        {
            _sceneStatInfos.Clear();
            _sceneStatInfos = null;
        }

        /// <summary>
        /// 卸载统计模块实例中的垃圾数据
        /// </summary>
        public void Dump()
        {
            _sceneStatInfos.Clear();
        }

        /// <summary>
        /// 获取当前所有场景访问的统计信息
        /// </summary>
        /// <returns>返回所有的操作访问统计信息</returns>
        public IList<IStatInfo> GetAllStatInfos()
        {
            List<IStatInfo> results = new List<IStatInfo>();
            results.AddRange(_sceneStatInfos);

            return results;
        }

        [IStatModule.OnStatModuleRegisterCallback(ON_SCENE_ENTER_CALL)]
        private void OnSceneEnter(CScene scene)
        {
            SceneStatInfo info = null;

            int uid = _sceneStatInfos.Count + 1;

            info = new SceneStatInfo(uid, scene.Name, scene.GetHashCode());
            info.EnterTime = SystemDateTime.UtcNow;
            _sceneStatInfos.Add(info);
        }

        [IStatModule.OnStatModuleRegisterCallback(ON_SCENE_EXIT_CALL)]
        private void OnSceneExit(CScene scene)
        {
            SceneStatInfo info = null;
            if (false == TryGetSceneStatInfoByHashCode(scene.GetHashCode(), out info))
            {
                Debugger.Warn("Could not found any scene stat info with name '{0}', exited it failed.", scene.Name);
                return;
            }

            info.ExitTime = SystemDateTime.UtcNow;
        }

        private bool TryGetSceneStatInfoByHashCode(int hashCode, out SceneStatInfo info)
        {
            for (int n = _sceneStatInfos.Count - 1; n >= 0; --n)
            {
                SceneStatInfo found = _sceneStatInfos[n];
                if (found.HashCode == hashCode)
                {
                    info = found;
                    return true;
                }
            }

            info = null;
            return false;
        }
    }
}
