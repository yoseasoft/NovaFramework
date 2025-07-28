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

namespace GameEngine.Sample.InversionOfControl
{
    /// <summary>
    /// 玩家对象基类
    /// </summary>
    [GameEngine.DeclareActorClass("Player")]
    [GameEngine.EntityActivationComponent(typeof(MoveComponent))]
    [GameEngine.EntityActivationComponent(typeof(AttackComponent), 0, GameEngine.AspectBehaviourType.Start)]
    public class Player : Actor
    {
        protected override void OnInitialize()
        {
            Debugger.Info("Call Player.OnInitialize Method.");
        }

        protected override void OnStartup()
        {
            Debugger.Info("Call Player.OnStartup Method.");
        }

        protected override void OnAwake()
        {
            Debugger.Info("Call Player.OnAwake Method.");
        }

        protected override void OnStart()
        {
            Debugger.Info("Call Player.OnStart Method.");
        }

        protected override void OnDestroy()
        {
            Debugger.Info("Call Player.OnDestroy Method.");
        }

        protected override void OnShutdown()
        {
            Debugger.Info("Call Player.OnShutdown Method.");
        }

        protected override void OnCleanup()
        {
            Debugger.Info("Call Player.OnCleanup Method.");
        }
    }
}
