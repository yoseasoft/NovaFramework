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

namespace GameEngine.Sample.DispatchCall
{
    /// <summary>
    /// 主场景通知逻辑类
    /// </summary>
    static class MainSceneNotifySystem
    {
        [GameEngine.MessageListenerBindingOfTarget(typeof(EnterWorldResp))]
        static void OnEnterWorldNotify(this MainScene self, EnterWorldResp message)
        {
            MainDataComponent mainDataComponent = self.GetComponent<MainDataComponent>();
            Debugger.Assert(null == mainDataComponent.player, "接收到进入场景通知时，玩家对象实例必须为空！");

            Player player = GameEngine.ActorHandler.Instance.CreateActor<Player>();

            InitPlayerFromMessage(player, message.Player);

            mainDataComponent.player = player;

            Debugger.Info("玩家对象‘{%s}’进入场景成功，正式开始游戏！", player.GetComponent<IdentityComponent>().objectName);
        }

        [GameEngine.MessageListenerBindingOfTarget(typeof(LeaveWorldResp))]
        static void OnLeaveWorldNotify(this MainScene self, LeaveWorldResp message)
        {
            MainDataComponent mainDataComponent = self.GetComponent<MainDataComponent>();
            Debugger.Assert(null != mainDataComponent.player, "接收到离开场景通知时，玩家对象实例必须有效！");

            for (int n = 0; null != mainDataComponent.monsters && n < mainDataComponent.monsters.Count; ++n)
            {
                Monster monster = mainDataComponent.monsters[n];

                Debugger.Info("怪物对象‘{%s}’强制离开场景成功！", monster.GetComponent<IdentityComponent>().objectName);

                GameEngine.ActorHandler.Instance.DestroyActor(monster);
            }

            mainDataComponent.monsters?.Clear();
            mainDataComponent.monsters = null;

            Debugger.Info("玩家对象‘{%s}’离开场景成功，正式结束游戏！", mainDataComponent.player.GetComponent<IdentityComponent>().objectName);

            GameEngine.ActorHandler.Instance.DestroyActor(mainDataComponent.player);

            mainDataComponent.player = null;
        }

        [GameEngine.MessageListenerBindingOfTarget(typeof(LevelSpawnResp))]
        static void OnLevelSpawnNotify(this MainScene self, LevelSpawnResp message)
        {
            MainDataComponent mainDataComponent = self.GetComponent<MainDataComponent>();
            mainDataComponent.monsters ??= new List<Monster>();

            for (int n = 0; n < message.MonsterList.Count; ++n)
            {
                MonsterInfo monsterInfo = message.MonsterList[n];

                if (mainDataComponent.monsters.Count > 5)
                {
                    Debugger.Info("当前场景怪物数量超出限制范围，新增怪物对象‘{%s}’失败！", monsterInfo.Soldier.Basic.Name);
                    continue;
                }

                Monster monster = GameEngine.ActorHandler.Instance.CreateActor<Monster>();

                InitMonsterFromMessage(monster, monsterInfo);

                mainDataComponent.monsters.Add(monster);

                Debugger.Info("怪物对象‘{%s}’进入场景成功，请锁定并攻击它！", monster.GetComponent<IdentityComponent>().objectName);
            }
        }

        static void InitPlayerFromMessage(Player player, PlayerInfo message)
        {
            InitSoldierFromMessage(player, message.Soldier);
        }

        static void InitMonsterFromMessage(Monster monster, MonsterInfo message)
        {
            InitSoldierFromMessage(monster, message.Soldier);

            TransformComponent transformComponent = monster.GetComponent<TransformComponent>();
            SpawnComponent spawnComponent = monster.GetComponent<SpawnComponent>();
            spawnComponent.born_position = transformComponent.position;
        }

        static void InitSoldierFromMessage(Soldier soldier, SoldierInfo message)
        {
            IdentityComponent identityComponent = soldier.GetComponent<IdentityComponent>();
            identityComponent.objectID = message.Basic.Uid;
            identityComponent.objectType = 1;
            identityComponent.objectName = message.Basic.Name;

            AttributeComponent attributeComponent = soldier.GetComponent<AttributeComponent>();
            attributeComponent.level = message.AttrStat.Level;
            attributeComponent.exp = message.AttrStat.Exp;
            attributeComponent.health = message.AttrStat.Health;
            attributeComponent.energy = message.AttrStat.Energy;
            attributeComponent.attack = message.AttrStat.Attack;

            TransformComponent transformComponent = soldier.GetComponent<TransformComponent>();
            transformComponent.position = new UnityEngine.Vector3(message.Position.x, message.Position.y, message.Position.z);
            transformComponent.rotation = new UnityEngine.Vector3(message.Direction.x, message.Direction.y, message.Direction.z);

            SkillComponent skillComponent = soldier.GetComponent<SkillComponent>();
            skillComponent.skills = new List<SkillComponent.Skill>();
            for (int n = 0; null != message.SkillList && n < message.SkillList.Count; ++n)
            {
                SkillInfo skillInfo = message.SkillList[n];
                skillComponent.skills.Add(new SkillComponent.Skill() {
                    id = skillInfo.Id,
                    name = skillInfo.Name,
                    range = skillInfo.Range,
                    is_coolingdown = true,
                    cooling_time = skillInfo.CoolingTime,
                    last_used_time = 0f,
                });
            }
        }
    }
}
