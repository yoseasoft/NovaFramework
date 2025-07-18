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

using System.Collections.Generic;

namespace Game.Sample.TextFormat
{
    /// <summary>
    /// 操作任务接口类
    /// </summary>
    static partial class OpsTask
    {
        [GameEngine.OnInputDispatchCall(TaskCode_DataCollection, GameEngine.InputOperationType.Released)]
        static void TestDataCollection(int keycode, int operationType)
        {
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
            dict_pci.Add(typeof(GameEngine.CScene), new PlayerCardInfo() { card_id = 1, card_type = 15, card_name = "yuh" });
            dict_pci.Add(typeof(GameEngine.CActor), new PlayerCardInfo() { card_id = 2, card_type = 25, card_name = "goo" });
            dict_pci.Add(typeof(GameEngine.CView), new PlayerCardInfo() { card_id = 3, card_type = 35, card_name = "bee" });

            Debugger.Warn(arr_int.GetType().FullName);
            Debugger.Warn(arr_sbi.GetType().FullName);
            Debugger.Warn(list_int.GetType().FullName);
            Debugger.Warn(queue_int.GetType().FullName);
            Debugger.Warn(dict_pci.GetType().FullName);

            Debugger.Warn("int[] data = {%s}", NovaEngine.Utility.Text.ToString(arr_int, (index, v) => { return v.ToString(); }));
            Debugger.Warn("SoldierBlockInfo[] data = {%s}", NovaEngine.Utility.Text.ToString<SoldierBlockInfo>(arr_sbi, (index, v) => { return v.block_name; }));
            Debugger.Warn("IList<int> data = {%s}", NovaEngine.Utility.Text.ToString(list_int, (index, v) => { return v.ToString(); }));
            Debugger.Warn("Queue<int> data = {%s}", NovaEngine.Utility.Text.ToString(queue_int, (index, v) => { return v.ToString(); }));
            Debugger.Warn("Dictionary<System.Type, PlayerCardInfo> data = {%s}", NovaEngine.Utility.Text.ToString(dict_pci, (k, v) => { return v.card_name; }));
        }
    }
}
