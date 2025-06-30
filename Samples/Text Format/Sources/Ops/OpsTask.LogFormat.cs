/// <summary>
/// 基于 NovaFramework 的演示案例
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-30
/// 功能描述：
/// </summary>

namespace Game.Sample.TextFormat
{
    /// <summary>
    /// 操作任务接口类
    /// </summary>
    static partial class OpsTask
    {
        [GameEngine.OnKeycodeDispatchResponse(TaskCode_LogFormat, GameEngine.InputOperationType.Released)]
        static void TestLogFormat(int keycode, int operationType)
        {
            int num = 10;
            float num2 = 20f;
            string str = "hello，中国";
            Debugger.Log("测试整型值：{%d}，浮点数：{%d}，字符串：{%s}！", num, num2, str);
        }
    }
}
