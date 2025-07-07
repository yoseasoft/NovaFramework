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

using System.Collections.Generic;

namespace Game.Sample.DispatchCall
{
    /// <summary>
    /// 消息构建类
    /// </summary>
    static class MessageBuilder
    {
        /// <summary>
        /// 构建玩家
        /// </summary>
        public static PlayerInfo CreatePlayerInfo()
        {
            return new PlayerInfo()
            {
                Soldier = new SoldierInfo()
                {
                    Basic = new BaseInfo()
                    {
                        Uid = 1,
                        Name = @"骨灰玩家高希霸",
                    },
                    AttrStat = new AttrStatInfo()
                    {
                        Level = 10,
                        Exp = 50,
                        Health = 1000,
                        Energy = 100,
                        Attack = 20,
                    },
                    SkillList = new List<SkillInfo>()
                        {
                            new SkillInfo() { Id = 101, Name = @"一剑荡魔", Range = 2f, CoolingTime = 5f, },
                            new SkillInfo() { Id = 102, Name = @"万剑归宗", Range = 4f, CoolingTime = 10f, },
                        },
                    Position = new Vector3Info() { x = 0f, y = 0f, z = 0f, },
                    Direction = new Vector3Info() { x = 0f, y = 0f, z = 0f, },
                },
            };
        }

        /// <summary>
        /// 构建史莱姆
        /// </summary>
        public static MonsterInfo CreateSlimeMonsterInfo()
        {
            int uid = UniqueGenerator.NextId();

            float ox = (NovaEngine.Utility.Random.GetRandom(1000) + 1) * 0.1f;
            float oz = (NovaEngine.Utility.Random.GetRandom(1000) + 1) * 0.1f;

            return new MonsterInfo()
            {
                Soldier = new SoldierInfo()
                {
                    Basic = new BaseInfo()
                    {
                        Uid = uid,
                        Name = $"史莱姆_{uid}",
                    },
                    AttrStat = new AttrStatInfo()
                    {
                        Level = 5,
                        Exp = 0,
                        Health = 100,
                        Energy = 100,
                        Attack = 10,
                    },
                    SkillList = new List<SkillInfo>()
                        {
                            new SkillInfo() { Id = 1101, Name = @"黏液喷吐", Range = 2f, CoolingTime = 5f, },
                            new SkillInfo() { Id = 1102, Name = @"滚动突击", Range = 2f, CoolingTime = 10f, },
                        },
                    Position = new Vector3Info() { x = ox, y = 0f, z = oz, },
                    Direction = new Vector3Info() { x = 0f, y = 0f, z = 0f, },
                },
            };
        }

        /// <summary>
        /// 构建哥布林
        /// </summary>
        public static MonsterInfo CreateGoblinMonsterInfo()
        {
            int uid = UniqueGenerator.NextId();

            float ox = (NovaEngine.Utility.Random.GetRandom(1000) + 1) * 0.1f;
            float oz = (NovaEngine.Utility.Random.GetRandom(1000) + 1) * 0.1f;

            return new MonsterInfo()
            {
                Soldier = new SoldierInfo()
                {
                    Basic = new BaseInfo()
                    {
                        Uid = uid,
                        Name = $"哥布林_{uid}",
                    },
                    AttrStat = new AttrStatInfo()
                    {
                        Level = 5,
                        Exp = 0,
                        Health = 100,
                        Energy = 100,
                        Attack = 20,
                    },
                    SkillList = new List<SkillInfo>()
                        {
                            new SkillInfo() { Id = 1201, Name = @"疾速火箭", Range = 4f, CoolingTime = 5f, },
                            new SkillInfo() { Id = 1202, Name = @"爆裂散射", Range = 4f, CoolingTime = 10f, },
                        },
                    Position = new Vector3Info() { x = ox, y = 0f, z = oz, },
                    Direction = new Vector3Info() { x = 0f, y = 0f, z = 0f, },
                },
            };
        }
    }
}
