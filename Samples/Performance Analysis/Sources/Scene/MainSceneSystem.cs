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

using SystemStringBuilder = System.Text.StringBuilder;

namespace Game.Sample.PerformanceAnalysis
{
    /// <summary>
    /// 主场景逻辑类
    /// </summary>
    static class MainSceneSystem
    {
        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Start)]
        static void OnStart(this MainScene self)
        {
            PrintUsage();
        }

        [GameEngine.InputResponseBindingOfTarget((int) UnityEngine.KeyCode.Alpha1, GameEngine.InputOperationType.Released)]
        static void OnSceneDataCreatingInputed(this MainScene self, int keycode, int operationType)
        {
            MainDataComponent mainDataComponent = self.GetComponent<MainDataComponent>();
            if (mainDataComponent.game_players != null && mainDataComponent.game_players.Count > 0)
            {
                Debugger.Info("当前用户数据已存在，不能多次重复创建！");
                return;
            }

            int c = 100;

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            for (int n = 0; n < c; ++n)
            {
                GamePlayer player = GameEngine.ActorHandler.Instance.CreateActor<GamePlayer>();
                if (null == mainDataComponent.game_players) mainDataComponent.game_players = new List<GamePlayer>();
                mainDataComponent.game_players.Add(player);
            }
            stopwatch.Stop();
            Debugger.Info("成功创建游戏用户数据{%d}次，耗时{%d}毫秒！", c, stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            for (int n = 0; n < c; ++n)
            {
                NativePlayer player = new NativePlayer();

                player.attributeComponent = new NativeAttributeComponent();
                player.transformComponent = new NativeTransformComponent();

                player.OnInitialize();
                player.OnStartup();
                player.OnAwake();
                player.OnStart();

                if (null == mainDataComponent.native_players) mainDataComponent.native_players = new List<NativePlayer>();
                mainDataComponent.native_players.Add(player);
            }
            stopwatch.Stop();
            Debugger.Info("成功创建本地用户数据{%d}次，耗时{%d}毫秒！", c, stopwatch.ElapsedMilliseconds);

            PrintUsage();
        }

        [GameEngine.InputResponseBindingOfTarget((int) UnityEngine.KeyCode.Alpha2, GameEngine.InputOperationType.Released)]
        static void OnSceneDataRunningInputed(this MainScene self, int keycode, int operationType)
        {
            MainDataComponent mainDataComponent = self.GetComponent<MainDataComponent>();
            if (mainDataComponent.game_players == null || mainDataComponent.game_players.Count <= 0)
            {
                Debugger.Info("当前没有任何可用的用户数据，执行调度任务失败！");
                return;
            }

            int c = 1000;

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            for (int n = 0; n < mainDataComponent.game_players.Count; ++n)
            {
                GamePlayer player = mainDataComponent.game_players[n];
                for (int u = 0; u < c; ++u)
                {
                    //player.Call(player.Update);
                    //player.Call(player.LateUpdate);
                    player.Call("Update");
                    player.Call("LateUpdate");
                }
            }
            stopwatch.Stop();
            Debugger.Info("成功调度游戏用户数据{%d}次，耗时{%d}毫秒！", mainDataComponent.game_players.Count * c, stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            for (int n = 0; n < mainDataComponent.native_players.Count; ++n)
            {
                NativePlayer player = mainDataComponent.native_players[n];
                for (int u = 0; u < c; ++u)
                {
                    player.OnUpdate();
                    player.OnLateUpdate();
                }
            }
            stopwatch.Stop();
            Debugger.Info("成功调度本地用户数据{%d}次，耗时{%d}毫秒！", mainDataComponent.native_players.Count * c, stopwatch.ElapsedMilliseconds);

            PrintUsage();
        }

        [GameEngine.InputResponseBindingOfTarget((int) UnityEngine.KeyCode.Alpha3, GameEngine.InputOperationType.Released)]
        static void OnSceneDataPrintingInputed(this MainScene self, int keycode, int operationType)
        {
            MainDataComponent mainDataComponent = self.GetComponent<MainDataComponent>();
            if (mainDataComponent.game_players == null || mainDataComponent.game_players.Count <= 0)
            {
                Debugger.Info("当前没有任何可用的用户数据，执行打印任务失败！");
                return;
            }

            int r = NovaEngine.Utility.Random.GetRandom(mainDataComponent.game_players.Count);
            Debugger.Info("正在打印输出第{%d}个实例：", r);

            GamePlayer gamePlayer = mainDataComponent.game_players[r];
            PrintGamePlayer(gamePlayer);

            NativePlayer nativePlayer = mainDataComponent.native_players[r];
            PrintNativePlayer(nativePlayer);

            PrintUsage();
        }

        [GameEngine.InputResponseBindingOfTarget((int) UnityEngine.KeyCode.Alpha4, GameEngine.InputOperationType.Released)]
        static void OnSceneDataResettingInputed(this MainScene self, int keycode, int operationType)
        {
            MainDataComponent mainDataComponent = self.GetComponent<MainDataComponent>();
            if (mainDataComponent.game_players == null || mainDataComponent.game_players.Count <= 0)
            {
                Debugger.Info("当前没有任何可用的用户数据，执行重置任务失败！");
                return;
            }

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            for (int n = 0; n < mainDataComponent.game_players.Count; ++n)
            {
                GamePlayer player = mainDataComponent.game_players[n];
                player.ResetPlayerData();
            }
            stopwatch.Stop();
            Debugger.Info("成功重置游戏用户数据{%d}次，耗时{%d}毫秒！", mainDataComponent.game_players.Count, stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            for (int n = 0; n < mainDataComponent.native_players.Count; ++n)
            {
                NativePlayer player = mainDataComponent.native_players[n];
                player.Reset();
            }
            stopwatch.Stop();
            Debugger.Info("成功重置本地用户数据{%d}次，耗时{%d}毫秒！", mainDataComponent.native_players.Count, stopwatch.ElapsedMilliseconds);

            PrintUsage();
        }

        [GameEngine.InputResponseBindingOfTarget((int) UnityEngine.KeyCode.Alpha5, GameEngine.InputOperationType.Released)]
        static void OnSceneDataRemovingInputed(this MainScene self, int keycode, int operationType)
        {
            MainDataComponent mainDataComponent = self.GetComponent<MainDataComponent>();
            if (mainDataComponent.game_players == null || mainDataComponent.game_players.Count <= 0)
            {
                Debugger.Info("当前没有任何可用的用户数据，执行移除任务失败！");
                return;
            }

            int c = 0;
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            c = mainDataComponent.game_players.Count;
            for (int n = 0; n < c; ++n)
            {
                GameEngine.ActorHandler.Instance.DestroyActor(mainDataComponent.game_players[n]);
            }
            mainDataComponent.game_players.Clear();
            mainDataComponent.game_players = null;
            stopwatch.Stop();
            Debugger.Info("成功移除游戏用户数据{%d}次，耗时{%d}毫秒！", c, stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            c = mainDataComponent.native_players.Count;
            for (int n = 0; n < c; ++n)
            {
                NativePlayer player = mainDataComponent.native_players[n];
                player.OnDestroy();
                player.OnShutdown();
                player.OnCleanup();

                player.attributeComponent = null;
                player.transformComponent = null;
            }
            mainDataComponent.native_players.Clear();
            mainDataComponent.native_players = null;
            stopwatch.Stop();
            Debugger.Info("成功移除本地用户数据{%d}次，耗时{%d}毫秒！", c, stopwatch.ElapsedMilliseconds);

            PrintUsage();
        }

        static void PrintGamePlayer(GamePlayer player)
        {
            SystemStringBuilder sb = new SystemStringBuilder();

            GameAttributeComponent gameAttributeComponent = player.GetComponent<GameAttributeComponent>();
            GameTransformComponent gameTransformComponent = player.GetComponent<GameTransformComponent>();
            sb.AppendFormat("角色逻辑次数={0},周期次数={1},", player.actor_count, GameActor.actor_lifecycle_count);
            sb.AppendFormat("玩家逻辑次数={0},周期次数={1},", player.player_count, GamePlayer.player_lifecycle_count);
            sb.AppendFormat("属性等级={0},周期次数={1},", gameAttributeComponent.level, GameAttributeComponent.attribute_lifecycle_count);
            sb.AppendFormat("变换位置={{{0},{1},{2}}},周期次数={3},",
                gameTransformComponent.position.x, gameTransformComponent.position.y, gameTransformComponent.position.z,
                GameTransformComponent.transform_lifecycle_count);

            Debugger.Info("游戏对象实例：{%s}", sb.ToString());
        }

        static void PrintNativePlayer(NativePlayer player)
        {
            SystemStringBuilder sb = new SystemStringBuilder();

            NativeAttributeComponent nativeAttributeComponent = player.attributeComponent;
            NativeTransformComponent nativeTransformComponent = player.transformComponent;
            sb.AppendFormat("角色逻辑次数={0},周期次数={1},", player.actor_count, NativeActor.actor_lifecycle_count);
            sb.AppendFormat("玩家逻辑次数={0},周期次数={1},", player.player_count, NativePlayer.player_lifecycle_count);
            sb.AppendFormat("属性等级={0},周期次数={1},", nativeAttributeComponent.level, NativeAttributeComponent.attribute_lifecycle_count);
            sb.AppendFormat("变换位置={{{0},{1},{2}}},周期次数={3},",
                nativeTransformComponent.position.x, nativeTransformComponent.position.y, nativeTransformComponent.position.z,
                NativeTransformComponent.transform_lifecycle_count);

            Debugger.Info("本地对象实例：{%s}", sb.ToString());
        }

        static void PrintUsage()
        {
            Debugger.Info(@"使用说明：①创建演示数据；②调度演示数据；③打印演示数据；④重置演示数据；⑤清除演示数据；");
        }
    }
}
