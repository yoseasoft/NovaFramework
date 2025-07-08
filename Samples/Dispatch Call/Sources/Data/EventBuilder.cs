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
    /// 事件构建类
    /// </summary>
    [GameEngine.KeycodeSystem]
    static class EventBuilder
    {
        [GameEngine.OnKeycodeDispatchResponse((int) UnityEngine.KeyCode.Alpha1, GameEngine.InputOperationType.Released)]
        static void OnPlayerDisplayInfoEventSend(int keycode, int operationType)
        {
            GameEngine.EventController.Instance.Send(EventNotify.PlayerDisplayInfo, "疯狂星期四", 8080);
        }

        [GameEngine.OnKeycodeDispatchResponse((int) UnityEngine.KeyCode.Alpha2, GameEngine.InputOperationType.Released)]
        static void OnPlayerSearchAllEnemiesEventSend(int keycode, int operationType)
        {
            GameEngine.EventController.Instance.Send(EventNotify.PlayerSearchAllEnemies);
        }

        [GameEngine.OnKeycodeDispatchResponse((int) UnityEngine.KeyCode.Alpha3, GameEngine.InputOperationType.Released)]
        static void OnPlayerLockOneTargetEventSend(int keycode, int operationType)
        {
            int r = NovaEngine.Utility.Random.GetRandom(2);
            int uid = 0;
            if (r > 0)
            {
                Monster monster = GameEngine.SceneHandler.Instance.GetCurrentScene().GetComponent<MainDataComponent>().GetRandomMonsterObject();
                if (null != monster)
                {
                    uid = monster.GetComponent<IdentityComponent>().objectID;
                }
            }
            GameEngine.EventController.Instance.Send(EventNotify.PlayerLockOneTarget, uid);
        }

        [GameEngine.OnKeycodeDispatchResponse((int) UnityEngine.KeyCode.Alpha4, GameEngine.InputOperationType.Released)]
        static void OnPlayerUpgradeEventSend(int keycode, int operationType)
        {
            int exp = NovaEngine.Utility.Random.GetRandom(1000);

            GameEngine.EventController.Instance.Send(EventNotify.PlayerUpgrade, exp);
        }

        [GameEngine.OnKeycodeDispatchResponse((int) UnityEngine.KeyCode.Alpha5, GameEngine.InputOperationType.Released)]
        static void OnPlayerChaseTargetEventSend(int keycode, int operationType)
        {
            GameEngine.EventController.Instance.Send(EventNotify.PlayerChaseTarget);
        }
    }
}
