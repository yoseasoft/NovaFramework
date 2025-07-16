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

namespace Game.Sample.PerformanceAnalysis
{
    /// <summary>
    /// 属性组件逻辑类
    /// </summary>
    public static class GameAttributeComponentSystem
    {
        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Initialize)]
        static void Initialize(this GameAttributeComponent self)
        {
            GameAttributeComponent.attribute_lifecycle_count++;
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Startup)]
        static void Startup(this GameAttributeComponent self)
        {
            GameAttributeComponent.attribute_lifecycle_count++;
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Awake)]
        static void Awake(this GameAttributeComponent self)
        {
            GameAttributeComponent.attribute_lifecycle_count++;
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Start)]
        static void Start(this GameAttributeComponent self)
        {
            GameAttributeComponent.attribute_lifecycle_count++;
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Destroy)]
        static void Destroy(this GameAttributeComponent self)
        {
            GameAttributeComponent.attribute_lifecycle_count++;
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Shutdown)]
        static void Shutdown(this GameAttributeComponent self)
        {
            GameAttributeComponent.attribute_lifecycle_count++;
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Cleanup)]
        static void Cleanup(this GameAttributeComponent self)
        {
            GameAttributeComponent.attribute_lifecycle_count++;
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Update)]
        static void Update(this GameAttributeComponent self)
        {
            self.level++;
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.LateUpdate)]
        static void LateUpdate(this GameAttributeComponent self)
        {
            self.level++;
        }

        public static void ResetData(this GameAttributeComponent self)
        {
            self.level = 0;

            GameAttributeComponent.attribute_lifecycle_count = 0;
        }
    }
}
