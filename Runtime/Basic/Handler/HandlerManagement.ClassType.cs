/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
    /// 句柄对象的封装管理类，对已定义的所有句柄类进行统一的调度派发操作
    /// </summary>
    internal static partial class HandlerManagement
    {
        /// <summary>
        /// 句柄对象类的类型枚举定义
        /// </summary>
        public enum EHandlerClassType : byte
        {
            /// <summary>
            /// 默认定义
            /// </summary>
            Default = 0,

            /// <summary>
            /// 定时器模块
            /// </summary>
            Timer = 1,

            /// <summary>
            /// 线程模块
            /// </summary>
            Thread = 2,

            /// <summary>
            /// 任务模块
            /// </summary>
            Task = 3,

            /// <summary>
            /// 网络模块
            /// </summary>
            Network = 11,

            /// <summary>
            /// 资源模块
            /// </summary>
            Resource = 12,

            /// <summary>
            /// 文件模块
            /// </summary>
            File = 13,

            /// <summary>
            /// 输入模块
            /// </summary>
            Input = 21,

            /// <summary>
            /// 场景模块
            /// </summary>
            Scene = 31,

            /// <summary>
            /// 角色模块
            /// </summary>
            Actor = 32,

            /// <summary>
            /// UI模块
            /// </summary>
            Gui = 33,

            /// <summary>
            /// 音频模块
            /// </summary>
            Sound = 41,

            /// <summary>
            /// 用户自定义
            /// </summary>
            User = 101,
        }

        /// <summary>
        /// 通过句柄的类型标识获取其对应实例的优先级
        /// </summary>
        /// <param name="type">句柄类型标识</param>
        /// <returns>返回类型对应句柄的优先级</returns>
        public static int GetHandlerPriorityWithClassType(EHandlerClassType type)
        {
            return (int) type;
        }

        /// <summary>
        /// 通过句柄的类型标识获取其对应实例的优先级
        /// </summary>
        /// <param name="type">句柄类型标识</param>
        /// <returns>返回类型对应句柄的优先级</returns>
        public static int GetHandlerPriorityWithClassType(int type)
        {
            return type;
        }
    }
}
