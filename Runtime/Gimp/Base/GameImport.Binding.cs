/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
            OnReload();
        }

        /// <summary>
        /// 世界加载器的重新导入函数
        /// </summary>
        /// <param name="type">导入类型</param>
        public static void Reimport(int type)
        {
            OnReimport(type);
        }

        /// <summary>
        /// 调用游戏业务层的指定函数
        /// </summary>
        /// <param name="methodName">函数名称</param>
        /// <param name="args">函数参数列表</param>
        private static void CallGameFunc(string methodName, params object[] args)
        {
            string targetName = GameConfig.GAME_WORLD_MODULE_EXTERNAL_GATEWAY_NAME;
            if (NovaEngine.Environment.tutorialMode)
            {
                // 教程开启
                targetName = GameConfig.GAME_SAMPLE_MODULE_EXTERNAL_GATEWAY_NAME;
            }

            System.Type type = NovaEngine.Utility.Assembly.GetType(targetName);
            if (type == null)
            {
                Debugger.Error("Could not found '{%s}' class type with current assemblies list, call that function '{%s}' failed.", targetName, methodName);
                return;
            }

            // Debugger.Info("Call remote service {%s} with target function name {%s}.", targetName, methodName);

            NovaEngine.Utility.Reflection.CallMethod(type, methodName, args);
        }
    }
}
