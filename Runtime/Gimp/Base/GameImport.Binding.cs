/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
    /// 程序的世界加载器入口封装对象类，提供业务层相关模块数据的动态加载
    /// </summary>
    public static partial class GameImport
    {
        /// <summary>
        /// 世界加载器的启动运行函数
        /// </summary>
        public static void Run()
        {
            Startup();
        }

        /// <summary>
        /// 世界加载器的停止运行函数
        /// </summary>
        public static void Stop()
        {
            Shutdown();
        }

        /// <summary>
        /// 世界加载器的重载函数
        /// </summary>
        public static void Reload()
        {
            Restart();
        }

        /// <summary>
        /// 调用游戏业务层的指定函数
        /// </summary>
        /// <param name="methodName">函数名称</param>
        private static void CallGameFunc(string methodName)
        {
            System.Type type = NovaEngine.Utility.Assembly.GetType(GameMacros.GAME_WORLD_MODULE_ENTRANCE_NAME);
            if (type == null)
            {
                Debugger.Error("Could not found '{%s}' class type with current assemblies list, call that function '{%s}' failed.",
                        GameMacros.GAME_WORLD_MODULE_ENTRANCE_NAME, methodName);
                return;
            }

            NovaEngine.Utility.Reflection.CallMethod(type, methodName);
        }
    }
}
