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

namespace Game.Sample.TextFormat
{
    /// <summary>
    /// 主场景数据组件逻辑类
    /// </summary>
    static class MainDataComponentSystem
    {
        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Awake)]
        static void Awake(this MainDataComponent self)
        {
            self.player = DataBuilder.CreatePlayer();

            Debugger.Info("成功创建玩家对象实例！");
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Start)]
        static void Start(this MainDataComponent self)
        {
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Destroy)]
        static void Destroy(this MainDataComponent self)
        {
            GameEngine.GameApi.DestroyActor(self.player);
            self.player = null;

            Debugger.Info("成功销毁玩家对象实例！");
        }
    }
}
