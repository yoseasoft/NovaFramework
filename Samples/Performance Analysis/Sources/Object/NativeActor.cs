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
    /// 通过系统原生object实现的角色对象类
    /// </summary>
    public abstract class NativeActor : object
    {
        public int actor_count;

        public NativeAttributeComponent attributeComponent;

        public static int actor_lifecycle_count;

        public virtual void OnInitialize()
        {
            attributeComponent?.OnInitialize();
            actor_lifecycle_count++;
        }

        public virtual void OnStartup()
        {
            attributeComponent?.OnStartup();
            actor_lifecycle_count++;
        }

        public virtual void OnAwake()
        {
            attributeComponent?.OnAwake();
            actor_lifecycle_count++;
        }

        public virtual void OnStart()
        {
            attributeComponent?.OnStart();
            actor_lifecycle_count++;
        }

        public virtual void OnDestroy()
        {
            actor_lifecycle_count++;
            attributeComponent?.OnDestroy();
        }

        public virtual void OnShutdown()
        {
            actor_lifecycle_count++;
            attributeComponent?.OnShutdown();
        }

        public virtual void OnCleanup()
        {
            actor_lifecycle_count++;
            attributeComponent?.OnCleanup();
        }

        public virtual void OnUpdate()
        {
            attributeComponent?.OnUpdate();
            actor_count++;
        }

        public virtual void OnLateUpdate()
        {
            attributeComponent?.OnLateUpdate();
            actor_count++;
        }

        public virtual void Reset()
        {
            actor_count = 0;

            attributeComponent?.Reset();

            actor_lifecycle_count = 0;
        }
    }
}
