
using log4net.Core;
using WebSocketSharp;


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
    /// 事件处理类
    /// </summary>
    static class EventProcessor
    {
        [GameEngine.OnEventDispatchCall(typeof(Player), EventNotify.PlayerUpgrade)]
        private static void OnPlayerUpgrade(Player self, int eventID, params object[] args)
        {
            AttributeComponent attributeComponent = self.GetComponent<AttributeComponent>();

            int level = attributeComponent.level;
            int exp = 0;

            if (null != args && args.Length > 0)
            {
                exp = (int) args[0];
            }

            attributeComponent.exp += exp;
            while (attributeComponent.exp >= 100)
            {
                attributeComponent.exp -= 100;
                attributeComponent.level += 1;
            }

            Debugger.Info("玩家对象‘{%s}’获取经验‘{%d}’，升级（{%d} -> {%d}）成功！", self.GetComponent<IdentityComponent>().objectName, exp, level, attributeComponent.level);
        }

        [GameEngine.OnEventDispatchCall(typeof(Monster), EventNotify.MonsterReturnSpawnPoint)]
        private static void OnMonsterReturnSpawnPoint(Monster self)
        {
            TransformComponent transformComponent = self.GetComponent<TransformComponent>();
            SpawnComponent spawnComponent = self.GetComponent<SpawnComponent>();

            transformComponent.position = spawnComponent.born_position;

            Debugger.Info("怪物对象‘{%s}’返回出生点完成！", self.GetComponent<IdentityComponent>().objectName);
        }
    }
}
