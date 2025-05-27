/// <summary>
/// 2023-11-20 GameEngine Framework Code By Hurley
/// </summary>

namespace GameEngine
{
    /// <summary>
    /// 程序的热更处理器封装对象类，提供业务层相关模块代码的更新及动态加载
    /// </summary>
    public class Updation : NovaEngine.IManager, NovaEngine.IInitializable, NovaEngine.IUpdatable
    {
        public int Priority => 0;

        public virtual void Initialize()
        {
            Debugger.Log("更新程序初始化成功！");
        }

        public virtual void Cleanup()
        {
            Debugger.Log("更新程序清理成功！");
        }

        public virtual void Update()
        {
            Debugger.Log("检测当前程序是否为最新版本！");
        }

        public virtual void LateUpdate()
        {
            GameImport.OnVersionUpdateCompleted();
        }
    }
}
