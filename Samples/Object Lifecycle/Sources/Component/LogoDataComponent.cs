/// <summary>
/// 基于 NovaFramework 的演示案例
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-25
/// 功能描述：
/// </summary>

namespace Game.Sample.ObjectLifecycle
{
    /// <summary>
    /// Logo数据组件类
    /// </summary>
    [GameEngine.DeclareComponentClass("LogoDataComponent")]
    public class LogoDataComponent : GameEngine.CComponent
    {
        public Player player = null;

        public LogoDataComponent()
        {
            Debugger.Info("Call LogoDataComponent Constructor Method...");
        }

        ~LogoDataComponent()
        {
            Debugger.Info("Call LogoDataComponent Destructor Method...");
        }
    }
}
