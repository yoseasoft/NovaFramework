/// <summary>
/// 基于 NovaFramework 的演示案例
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-29
/// 功能描述：
/// </summary>

using System.Collections.Generic;

namespace Game.Sample.TextFormat
{
    /// <summary>
    /// Logo场景输入逻辑类
    /// </summary>
    [GameEngine.KeycodeSystem]
    static class LogoSceneInputSystem
    {
        [GameEngine.OnKeycodeDispatchResponse((int) UnityEngine.KeyCode.A, GameEngine.InputOperationType.Released)]
        static void OnSceneInputed(int keycode, int operationType)
        {
            LogoScene logo = GameEngine.SceneHandler.Instance.GetCurrentScene() as LogoScene;
            Debugger.Assert(null != logo, "Invalid activated scene.");

            int[] arr_int = new int[2];
            arr_int[0] = 1;
            arr_int[1] = 2;

            SoldierBlockInfo[] arr_sbi = new SoldierBlockInfo[2];
            arr_sbi[0] = new SoldierBlockInfo { block_id = 1, block_type = 5, block_name = "pos1" };
            arr_sbi[1] = new SoldierBlockInfo { block_id = 2, block_type = 10, block_name = "pos2" };

            IList<int> list_int = new List<int>();
            list_int.Add(1);
            list_int.Add(2);

            Queue<int> queue_int = new Queue<int>();
            queue_int.Enqueue(1);
            queue_int.Enqueue(2);

            IDictionary<System.Type, PlayerCardInfo> dict_pci = new Dictionary<System.Type, PlayerCardInfo>();
            dict_pci.Add(typeof(System.NotImplementedException), new PlayerCardInfo() { card_id = 1, card_type = 12, card_name = "yuh" });
            dict_pci.Add(typeof(GameEngine.CScene), new PlayerCardInfo() { card_id = 2, card_type = 25, card_name = "goo" });

            Debugger.Warn(arr_int.GetType().FullName);
            Debugger.Warn(arr_sbi.GetType().FullName);
            Debugger.Warn(list_int.GetType().FullName);
            Debugger.Warn(queue_int.GetType().FullName);
            Debugger.Warn(dict_pci.GetType().FullName);

            NovaEngine.Formatter.ToString(arr_int as System.Collections.ICollection, (index, v) =>
            { 
                Debugger.Warn(v.GetType().FullName);
                return v.ToString();
            });
            NovaEngine.Formatter.ToString<SoldierBlockInfo>(arr_sbi, (index, v) =>
            {
                Debugger.Warn(v.GetType().FullName);
                return v.block_name;
            });
            //NovaEngine.Format.ToString(list_int, (v) => { return v.ToString(); });
            NovaEngine.Formatter.ToString(queue_int, (index, v) =>
            {
                Debugger.Warn(v.GetType().FullName);
                return v.ToString();
            });
            NovaEngine.Formatter.ToString(dict_pci, (k, v) =>
            {
                Debugger.Warn("~~~~~~~~~~~~~~~~~~~~~" + v.GetType().FullName);
                return v.card_name;
            });
        }
    }
}
