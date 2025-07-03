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

namespace Game.Sample.TextFormat
{
    /// <summary>
    /// 操作任务接口类
    /// </summary>
    static partial class OpsTask
    {
        [GameEngine.OnKeycodeDispatchResponse(TaskCode_ObjectInfo, GameEngine.InputOperationType.Released)]
        static void TestObjectInfo(int keycode, int operationType)
        {
            Player player = CreatePlayer();
        }

        static Player CreatePlayer()
        {
            Player player = new Player();

            player.ObjectID = 101;
            player.ObjectType = 102;
            player.objectName = "hurley";

            player.Tag = 201;
            player.blockInfo = new SoldierBlockInfo() { block_id = 211, block_type = 212, block_name = "空气墙" };
            player.buffs = new Dictionary<int, SoldierBuffInfo>() {
                { 1001, new SoldierBuffInfo() { buff_id = 11, buff_type = 12, buff_name = "灼烧" } },
                { 1002, new SoldierBuffInfo() { buff_id = 21, buff_type = 22, buff_name = "眩晕" } },
                { 1003, new SoldierBuffInfo() { buff_id = 31, buff_type = 32, buff_name = "流血" } },
            };

            player.Level = 301;
            player.Exp = 302;
            player.cardInfo = new PlayerCardInfo()
            {
                card_id = 311,
                card_type = 312,
                card_name = "身份证",
                card_ref_list = new List<PlayerCardRefInfo>() {
                    new PlayerCardRefInfo() { ref_count = 1, ref_name = "中国" },
                    new PlayerCardRefInfo() { ref_count = 2, ref_name = "法国" },
                    new PlayerCardRefInfo() { ref_count = 3, ref_name = "美国" },
                }
            };
            player.skillInfo = new PlayerSkillInfo() { skill_id = 321, skill_type = 322, skill_name = "剑气纵横" };

            AttributeComponent attributeComponent = player.AddComponent<AttributeComponent>();
            attributeComponent.health = 100;
            attributeComponent.mana = 100;
            attributeComponent.speed = 5;
            attributeComponent.attack = 20;
            attributeComponent.defense = 3;

            MoveComponent moveComponent = player.AddComponent<MoveComponent>();
            moveComponent.interval = 2f;
            moveComponent.escapedTime = 1.2f;

            AttackComponent attackComponent = player.AddComponent<AttackComponent>();
            attackComponent.coolingTime = 3f;

            return player;
        }
    }
}
