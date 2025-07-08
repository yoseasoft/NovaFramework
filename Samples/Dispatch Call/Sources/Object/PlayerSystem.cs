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

using SystemStringBuilder = System.Text.StringBuilder;

namespace Game.Sample.DispatchCall
{
    /// <summary>
    /// 玩家对象逻辑类
    /// </summary>
    [GameEngine.Aspect]
    public static class PlayerSystem
    {
        [GameEngine.OnAspectAfterCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.Awake)]
        static void Awake(this Player self)
        {
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.Start)]
        static void Start(this Player self)
        {
        }

        [GameEngine.OnAspectBeforeCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.Destroy)]
        static void Destroy(this Player self)
        {
        }

        [GameEngine.EventSubscribeBindingOfTarget(EventNotify.PlayerDisplayInfo)]
        private static void OnPlayerDisplayInfo(this Player self, int eventID, params object[] args)
        {
            Debugger.Info("玩家对象成功接收事件[{%d}]，事件参数：{%s}，信息输出：{%s}！", eventID, NovaEngine.Formatter.ToString(args), self.ToPlayerString());
        }

        [GameEngine.EventSubscribeBindingOfTarget(EventNotify.PlayerLockOneTarget)]
        private static void OnPlayerLockOneTarget(this Player self, int eventID, params object[] args)
        {
            MainDataComponent mainDataComponent = GameEngine.SceneHandler.Instance.GetCurrentScene().GetComponent<MainDataComponent>();
            Monster monster = null;
            int uid = 0;
            if (null != args && args.Length > 0)
            {
                uid = (int) args[0];
            }

            if (uid > 0)
            {
                monster = mainDataComponent.GetSoldierByUid(uid) as Monster;
            }
            else
            {
                monster = mainDataComponent.GetRandomMonsterObject();
            }

            if (null == monster)
            {
                Debugger.Info("玩家对象成功接收事件[{%d}]，事件参数：{%d}，未找到任何目标怪物对象！", eventID, uid);
            }
            else
            {
                AttackComponent attackComponent = self.GetComponent<AttackComponent>();
                int monsterId = monster.GetComponent<IdentityComponent>().objectID;
                if (attackComponent.targetId == monsterId)
                {
                    Debugger.Info("玩家对象成功接收事件[{%d}]，事件参数：{%d}，当前锁定重复的目标：{%s}！", eventID, uid, monster.GetComponent<IdentityComponent>().objectName);
                }
                else
                {
                    attackComponent.targetId = monsterId;

                    Debugger.Info("玩家对象成功接收事件[{%d}]，事件参数：{%d}，找到目标：{%s}！", eventID, uid, monster.GetComponent<IdentityComponent>().objectName);
                }
            }
        }

        [GameEngine.EventSubscribeBindingOfTarget(EventNotify.PlayerChaseTarget)]
        private static void OnPlayerChaseTarget(this Player self)
        {
            self.GetComponent<MoveComponent>().OnMovingStart();

            Debugger.Info("玩家对象‘{%s}’开始移动！", self.GetComponent<IdentityComponent>().objectName);
        }

        public static string ToPlayerString(this Player self)
        {
            SystemStringBuilder sb = new SystemStringBuilder();

            sb.AppendFormat("[玩家对象]:{0},", self.ToSoldierString());

            AttackComponent attackComponent = self.GetComponent<AttackComponent>();
            sb.AppendFormat("目标={0},", attackComponent.targetId);

            return sb.ToString();
        }
    }
}
