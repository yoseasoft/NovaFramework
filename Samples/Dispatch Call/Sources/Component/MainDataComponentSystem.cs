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
    /// 主数据组件逻辑类
    /// </summary>
    [GameEngine.Aspect]
    public static class MainDataComponentSystem
    {
        [GameEngine.OnAspectAfterCallOfTarget(typeof(MainDataComponent), GameEngine.AspectBehaviourType.Awake)]
        static void Awake(this MainDataComponent self)
        {
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(MainDataComponent), GameEngine.AspectBehaviourType.Start)]
        static void Start(this MainDataComponent self)
        {
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(MainDataComponent), GameEngine.AspectBehaviourType.Update)]
        static void Update(this MainDataComponent self)
        {
            for (int n = 0; null != self.monsters && n < self.monsters.Count; ++n)
            {
                Monster monster = self.monsters[n];
                AttributeComponent attributeComponent = monster.GetComponent<AttributeComponent>();
                if (attributeComponent.health <= 0)
                {
                    IdentityComponent identityComponent = monster.GetComponent<IdentityComponent>();

                    // 发送死亡通知
                    GameEngine.NetworkHandler.Instance.OnSimulationReceiveMessageComposedOfProtoBuf(new ActorDieResp() { Uid = identityComponent.objectID });
                }
            }
        }

        [GameEngine.OnAspectBeforeCallOfTarget(typeof(MainDataComponent), GameEngine.AspectBehaviourType.Destroy)]
        static void Destroy(this MainDataComponent self)
        {
        }

        public static Soldier GetSoldierByUid(this MainDataComponent self, int uid)
        {
            if (null != self.player)
            {
                if (self.player.GetComponent<IdentityComponent>().objectID == uid)
                {
                    return self.player;
                }
            }

            for (int n = 0; null != self.monsters && n < self.monsters.Count; ++n)
            {
                Monster monster = self.monsters[n];

                if (monster.GetComponent<IdentityComponent>().objectID == uid)
                {
                    return monster;
                }
            }

            return null;
        }

        public static Monster GetRandomMonsterObject(this MainDataComponent self)
        {
            if (null != self.monsters && self.monsters.Count > 0)
            {
                int c = self.monsters.Count;

                int r = NovaEngine.Utility.Random.GetRandom(c);

                return self.monsters[r];
            }

            return null;

        }
    }
}
