/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using UnityEngine.Scripting;

namespace GameEngine.Profiler.Statistics
{
    /// <summary>
    /// 资产统计模块，对资产模块对象提供数据统计所需的接口函数
    /// </summary>
    internal sealed class AssetStat : BaseStat<AssetStat, AssetStatInfo>
    {
        [IStat.OnStatFunctionRegister(StatCode.AssetLoad)]
        [Preserve]
        private void OnAssetLoad(CEntity entity, string name, string url)
        {
            IStat stat = Statistician.GetStatWithBeanType(entity.BeanType);
            if (null == stat)
            {
                Debugger.Warn(LogGroupTag.Profiler, "Could not found any 'IStat' type with target class '{%t}', the asset loaded failed.", entity.BeanType);
                return;
            }

            EntityStatInfo statInfo = stat.GetStateInfoByUid(entity.BeanId) as EntityStatInfo;
            if (null == statInfo)
            {
                Debugger.Warn(LogGroupTag.Profiler, "Could not found any 'EntityStatInfo' object with target bean id '{%d}', notify asset loaded failed.", entity.BeanId);
                return;
            }

            statInfo.OnAssetLoaded(entity, name, url);
        }

        [IStat.OnStatFunctionRegister(StatCode.AssetUnload)]
        [Preserve]
        private void OnAssetUnload(CEntity entity, string name)
        {
            IStat stat = Statistician.GetStatWithBeanType(entity.BeanType);
            if (null == stat)
            {
                Debugger.Warn(LogGroupTag.Profiler, "Could not found any 'IStat' type with target class '{%t}', the asset unloaded failed.", entity.BeanType);
                return;
            }

            EntityStatInfo statInfo = stat.GetStateInfoByUid(entity.BeanId) as EntityStatInfo;
            if (null == statInfo)
            {
                Debugger.Warn(LogGroupTag.Profiler, "Could not found any 'EntityStatInfo' object with target bean id '{%d}', notify asset unloaded failed.", entity.BeanId);
                return;
            }

            statInfo.OnAssetUnloaded(entity, name);
        }
    }
}
