/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace GameEngine
{
    /// <summary>
    /// 程序的配置管理器封装对象类，提供业务层对配置数据的读写访问接口
    /// </summary>
    public static class GameConfig
    {
        /// <summary>
        /// 测试案例演示流程的调度转发功能启动的状态标识<br/>
        /// 该模块只能从导入模块中进入，不可从引擎层直接进入<br/>
        /// 注意这个标识需手动设置，确定当前项目是否需要接入测试案例演示流程，从而决定是否需要开启该表示
        /// </summary>
        // public static readonly bool TUTORIAL_MODULE_DISPATCHING_FORWARD_ENABLED = false;

        /// <summary>
        /// 业务管理模块的对外关口名称
        /// </summary>
        public const string GAME_MODULE_EXTERNAL_GATEWAY_NAME = @"Game.GameWorld";
        /// <summary>
        /// 测试案例模块的对外关口名称
        /// </summary>
        public const string TUTORIAL_MODULE_EXTERNAL_GATEWAY_NAME = @"GameSample.GameWorld";
    }
}
