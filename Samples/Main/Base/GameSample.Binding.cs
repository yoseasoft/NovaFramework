/// <summary>
/// 基于 NovaFramework 的测试用例
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-22
/// 功能描述：
/// </summary>

namespace Game.Sample
{
    /// <summary>
    /// 测试案例总控
    /// </summary>
    public static partial class GameSample
    {
        public static void Run()
        {
            SampleFiltingProcessor.AddSampleFilter(GameSampleRunningType);

            RegAssemblyNames(typeof(GameSample).Namespace);

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
                GameEngine.Debugger.Error("Could not found '{%s}' class type with current assemblies list, call that function '{%s}' failed.", targetName, methodName);
                return;
            }

            GameEngine.Debugger.Info("Call remote service {%s} with target function name {%s}.", targetName, methodName);

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
