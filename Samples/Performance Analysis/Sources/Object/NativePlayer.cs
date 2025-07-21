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

namespace GameEngine.Sample.PerformanceAnalysis
{
    /// <summary>
    /// 通过系统原生object实现的玩家对象类
    /// </summary>
    public class NativePlayer : NativeActor
    {
        public int player_count;

        public NativeTransformComponent transformComponent;

        public static int player_lifecycle_count;

        public override void OnInitialize()
        {
            base.OnInitialize();
            transformComponent?.OnInitialize();
            player_lifecycle_count++;
        }

        public override void OnStartup()
        {
            base.OnStartup();
            transformComponent?.OnStartup();
            player_lifecycle_count++;
        }

        public override void OnAwake()
        {
            base.OnAwake();
            transformComponent?.OnAwake();
            player_lifecycle_count++;
        }

        public override void OnStart()
        {
            base.OnStart();
            transformComponent?.OnStart();
            player_lifecycle_count++;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            player_lifecycle_count++;
            transformComponent?.OnDestroy();
        }

        public override void OnShutdown()
        {
            base.OnShutdown();
            player_lifecycle_count++;
            transformComponent?.OnShutdown();
        }

        public override void OnCleanup()
        {
            base.OnCleanup();
            player_lifecycle_count++;
            transformComponent?.OnCleanup();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            transformComponent?.OnUpdate();
            player_count++;
        }

        public override void OnLateUpdate()
        {
            base.OnLateUpdate();
            transformComponent?.OnLateUpdate();
            player_count++;
        }

        public override void Reset()
        {
            base.Reset();

            player_count = 0;

            transformComponent?.Reset();

            player_lifecycle_count = 0;
        }
    }
}
