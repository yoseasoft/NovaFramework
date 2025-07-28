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

namespace Game.Sample.ObjectLifecycle
{
    /// <summary>
    /// 玩家对象逻辑类
    /// </summary>
    public static class PlayerSystem
    {
        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Initialize)]
        static void BeforeInitialize(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}前置初始化完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Initialize)]
        static void AfterInitialize(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}后置初始化完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Startup)]
        static void BeforeStartup(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}前置开启完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Startup)]
        static void AfterStartup(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}后置开启完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Awake)]
        static void BeforeAwake(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}前置唤醒完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Awake)]
        static void AfterAwake(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}后置唤醒完成！", self);

            self.AddComponent<LeapAttackComponent>();
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Start)]
        static void BeforeStart(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}前置启动完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Start)]
        static void AfterStart(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}后置启动完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Update)]
        static void BeforeUpdate(this Player self)
        {
            if (GameSample.OnceTimeUpdateCallPassed(self)) GameEngine.Debugger.Info("目标玩家实例{%t}前置刷新完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Update)]
        static void AfterUpdate(this Player self)
        {
            if (GameSample.OnceTimeUpdateCallPassed(self)) GameEngine.Debugger.Info("目标玩家实例{%t}后置刷新完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.LateUpdate)]
        static void BeforeLateUpdate(this Player self)
        {
            if (GameSample.OnceTimeUpdateCallPassed(self)) GameEngine.Debugger.Info("目标玩家实例{%t}前置延迟刷新完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.LateUpdate)]
        static void AfterLateUpdate(this Player self)
        {
            if (GameSample.OnceTimeUpdateCallPassed(self)) GameEngine.Debugger.Info("目标玩家实例{%t}后置延迟刷新完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Destroy)]
        static void BeforeDestroy(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}前置销毁完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Destroy)]
        static void AfterDestroy(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}后置销毁完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Shutdown)]
        static void BeforeShutdown(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}前置关闭完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Shutdown)]
        static void AfterShutdown(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}后置关闭完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Cleanup)]
        static void BeforeCleanup(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}前置清理完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Cleanup)]
        static void AfterCleanup(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}后置清理完成！", self);
        }
    }
}
