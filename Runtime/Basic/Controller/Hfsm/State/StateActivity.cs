/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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

using SystemTask = System.Threading.Tasks.Task;
using SystemCancellationToken = System.Threading.CancellationToken;

namespace GameEngine.HFSM
{
    /// <summary>
    /// 状态激活模式的枚举类型定义
    /// 该模式用于代表状态在状态机中的活动生命周期，可以通过该模式追踪实例当前的活动状态
    /// </summary>
    public enum StateActivityMode
    {
        Inactive,
        Activating,
        Active,
        Deactivating,
    }

    /// <summary>
    /// 状态的活动声明接口
    /// </summary>
    public interface IStateActivity
    {
        /// <summary>
        /// 获取当前状态的激活模式
        /// </summary>
        StateActivityMode ActivityMode { get; }

        /// <summary>
        /// 状态异步激活操作函数
        /// </summary>
        SystemTask ActivateAsync(SystemCancellationToken cancellationToken);

        /// <summary>
        /// 状态异步退出激活操作函数
        /// </summary>
        SystemTask DeactivateAsync(SystemCancellationToken cancellationToken);
    }

    /// <summary>
    /// 状态激活的通用基类，为我们提供管理激活和停用的通用模式
    /// </summary>
    public abstract class StateActivity : IStateActivity
    {
        /// <summary>
        /// 获取当前状态的激活模式
        /// 每个活动都会从非活动状态开始跟踪其当前模式
        /// </summary>
        public StateActivityMode ActivityMode { get; protected set; } = StateActivityMode.Inactive;

        /// <summary>
        /// 状态异步激活操作函数
        /// </summary>
        public virtual async SystemTask ActivateAsync(SystemCancellationToken cancellationToken)
        {
            // 若已激活或处于过渡状态，则跳过激活操作
            if (StateActivityMode.Inactive != ActivityMode) return;

            // 将该活动标记为正在激活
            ActivityMode = StateActivityMode.Activating;
            // 只需要等待已完成的任务即可
            await SystemTask.CompletedTask;
            // 然后将活动模式设为激活
            ActivityMode = StateActivityMode.Active;
        }

        /// <summary>
        /// 状态异步退出激活操作函数
        /// </summary>
        public virtual async SystemTask DeactivateAsync(SystemCancellationToken cancellationToken)
        {
            // 仅在其初始为激活状态时才可以停用
            if (StateActivityMode.Active != ActivityMode) return;

            // 将该活动标记为停用
            ActivityMode = StateActivityMode.Deactivating;
            // 等待任务完成
            await SystemTask.CompletedTask;
            // 最后将模式设为非激活状态
            ActivityMode = StateActivityMode.Inactive;
        }
    }
}
