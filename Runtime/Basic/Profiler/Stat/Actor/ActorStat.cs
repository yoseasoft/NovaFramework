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

namespace GameEngine.Profiler.Statistics
{
    /// <summary>
    /// 角色统计模块，对角色模块对象提供数据统计所需的接口函数
    /// </summary>
    internal sealed class ActorStat : StatSingleton<ActorStat>, IStat
    {
        /// <summary>
        /// 角色访问统计信息容器列表
        /// </summary>
        private IList<ActorStatInfo> _actorStatInfos = null;

        /// <summary>
        /// 初始化统计模块实例的回调接口
        /// </summary>
        protected override void OnInitialize()
        {
            _actorStatInfos = new List<ActorStatInfo>();
        }

        /// <summary>
        /// 清理统计模块实例的回调接口
        /// </summary>
        protected override void OnCleanup()
        {
            _actorStatInfos.Clear();
            _actorStatInfos = null;
        }

        /// <summary>
        /// 卸载统计模块实例中的垃圾数据
        /// </summary>
        public void Dump()
        {
            _actorStatInfos.Clear();
        }

        /// <summary>
        /// 获取当前所有角色访问的统计信息
        /// </summary>
        /// <returns>返回所有的操作访问统计信息</returns>
        public IList<IStatInfo> GetAllStatInfos()
        {
            List<IStatInfo> results = new List<IStatInfo>();
            results.AddRange(_actorStatInfos);

            return results;
        }

        [IStat.OnStatFunctionRegister(StatCode.ActorCreate)]
        private void OnActorCreate(CActor obj)
        {
            ActorStatInfo info = null;

            int uid = _actorStatInfos.Count + 1;

            info = new ActorStatInfo(uid, obj.Name, obj.GetHashCode());
            info.CreateTime = SystemDateTime.UtcNow;
            _actorStatInfos.Add(info);
        }

        [IStat.OnStatFunctionRegister(StatCode.ActorRelease)]
        private void OnActorRelease(CActor obj)
        {
            ActorStatInfo info = null;
            if (false == TryGetActorStatInfoByHashCode(obj.GetHashCode(), out info))
            {
                Debugger.Warn("Could not found any actor stat info with name '{0}', exited it failed.", obj.Name);
                return;
            }

            info.ReleaseTime = SystemDateTime.UtcNow;
        }

        private bool TryGetActorStatInfoByHashCode(int hashCode, out ActorStatInfo info)
        {
            for (int n = _actorStatInfos.Count - 1; n >= 0; --n)
            {
                ActorStatInfo found = _actorStatInfos[n];
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
