/// -------------------------------------------------------------------------------
/// NovaEngine Framework Samples
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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

namespace Game.Sample.DispatchCall
{
    /// <summary>
    /// 事件通知类
    /// </summary>
    public static class EventNotify
    {
        public const int PlayerDisplayInfo = 1101;
        public const int PlayerSearchAllEnemies = 1102; // 检索所有敌方单位
        public const int PlayerLockOneTarget = 1103; // 锁定一个目标
        public const int PlayerUpgrade = 1104;

        public const int MonsterReturnSpawnPoint = 1201;
        public const int MonsterRestoreHealth = 1202;
    }

    /// <summary>
    /// 士兵移动通知
    /// </summary>
    public struct SoldierMovedNotify
    {
        public int uid;
        public UnityEngine.Vector3 rotation;
        public float distance;
    }

    /// <summary>
    /// 士兵使用技能通知
    /// </summary>
    public struct SoldierUseSkillNotify
    {
        public int uid;
        public int skill_id;
    }
}
