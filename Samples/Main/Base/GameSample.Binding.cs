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

namespace Game.Sample
{
    /// <summary>
    /// 演示案例总控
    /// </summary>
    public static partial class GameSample
    {
        public static void Run()
        {
            SampleFiltingProcessor.AddSampleFilter(GameSampleRunningType);

            // RegAssemblyNames(typeof(GameSample).Namespace);
            RegAssemblyNames("Nova.Sample");

            LoadAllAssemblies();

            // 启动应用通知回调接口
            GameEngine.EngineDispatcher.OnApplicationStartup(OnApplicationResponseCallback);

            CallSampleGate(GameEngine.GameMacros.GAME_REMOTE_PROCESS_CALL_RUN_SERVICE_NAME);
        }

        public static void Stop()
        {
            CallSampleGate(GameEngine.GameMacros.GAME_REMOTE_PROCESS_CALL_STOP_SERVICE_NAME);

            // 关闭应用通知回调接口
            GameEngine.EngineDispatcher.OnApplicationShutdown(OnApplicationResponseCallback);

            SampleFiltingProcessor.RemoveSampleFilter();
        }

        public static void Reload()
        {
            LoadAllAssemblies(true);
        }

        /// <summary>
        /// 调用游戏案例下的指定函数
        /// </summary>
        /// <param name="methodName">函数名称</param>
        private static void CallSampleGate(string methodName)
        {
            string targetName = SampleFiltingProcessor.GetFilterModuleName() + ".SampleGate";

            System.Type type = NovaEngine.Utility.Assembly.GetType(targetName);
            if (type == null)
            {
                Debugger.Error("Could not found '{%s}' class type with current assemblies list, call that function '{%s}' failed.", targetName, methodName);
                return;
            }

            Debugger.Info("Call remote service {%s} with target function name {%s}.", targetName, methodName);

            NovaEngine.Utility.Reflection.CallMethod(type, methodName);
        }

        /// <summary>
        /// 应用层相应通知回调函数
        /// </summary>
        /// <param name="protocolType">通知协议类型</param>
        private static void OnApplicationResponseCallback(NovaEngine.Application.ProtocolType protocolType)
        {
            switch (protocolType)
            {
                case NovaEngine.Application.ProtocolType.Startup:
                    Startup();
                    break;
                case NovaEngine.Application.ProtocolType.Shutdown:
                    Shutdown();
                    break;
                case NovaEngine.Application.ProtocolType.FixedUpdate:
                    FixedUpdate();
                    break;
                case NovaEngine.Application.ProtocolType.Update:
                    Update();
                    break;
                case NovaEngine.Application.ProtocolType.LateUpdate:
                    LateUpdate();
                    break;
                case NovaEngine.Application.ProtocolType.FixedExecute:
                    FixedExecute();
                    break;
                case NovaEngine.Application.ProtocolType.Execute:
                    Execute();
                    break;
                case NovaEngine.Application.ProtocolType.LateExecute:
                    LateExecute();
                    break;
                default:
                    break;
            }
        }
    }
}
