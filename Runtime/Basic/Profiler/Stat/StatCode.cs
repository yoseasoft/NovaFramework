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

namespace GameEngine.Profiler.Statistics
{
    /// <summary>
    /// 统计模块编号的常量定义，仅用于外部方便使用，在调度过程中没有实际意义
    /// </summary>
    internal static class StatCode
    {
        /// <summary>
        /// 统计模块编号的编码间隔
        /// 以 0xff （255） 为一个状态标识的间隔，也就是说每个状态下最多允许记录255个统计信息
        /// </summary>
        private const int StatCodingInterval = 0x0100;

        private const int TimerCode = (int) StatType.Timer * StatCodingInterval;
        public const int TimerStartup = TimerCode + 0x01;
        public const int TimerFinished = TimerCode + 0x02;
        public const int TimerDispatched = TimerCode + 0x03;

        private const int NetworkCode = (int) StatType.Network * StatCodingInterval;
        public const int NetworkConnected = NetworkCode + 0x01;
        public const int NetworkDisconnected = NetworkCode + 0x02;
        public const int NetworkSend = NetworkCode + 0x03;
        public const int NetworkRecv = NetworkCode + 0x04;

        private const int ObjectCode = (int) StatType.Object * StatCodingInterval;
        public const int ObjectCreate = ObjectCode + 0x01;
        public const int ObjectRelease = ObjectCode + 0x02;

        private const int SceneCode = (int) StatType.Scene * StatCodingInterval;
        public const int SceneEnter = SceneCode + 0x01;
        public const int SceneExit = SceneCode + 0x02;

        private const int ActorCode = (int) StatType.Actor * StatCodingInterval;
        public const int ActorCreate = ActorCode + 0x01;
        public const int ActorRelease = ActorCode + 0x02;

        private const int ViewCode = (int) StatType.View * StatCodingInterval;
        public const int ViewCreate = ViewCode + 0x01;
        public const int ViewClose = ViewCode + 0x02;

        /// <summary>
        /// 通过统计编号获取统计模块类型
        /// </summary>
        /// <param name="code">统计编码</param>
        /// <returns>返回统计模块类型</returns>
        public static int GetStatTypeByCode(int code)
        {
            return (code & 0x00) / StatCodingInterval;
        }

        /// <summary>
        /// 通过统计模块类型和统计编号获取统计编码
        /// </summary>
        /// <param name="type">统计模块类型</param>
        /// <param name="code">统计编号</param>
        /// <returns>返回统计编码</returns>
        public static int GetStatCodeByType(StatType type, int code)
        {
            if ((code & 0x00) == 0)
            {
                code += (int) type * StatCodingInterval;
            }

            return code;
        }
    }
}
