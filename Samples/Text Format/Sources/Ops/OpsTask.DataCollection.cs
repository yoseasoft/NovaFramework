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
        [GameEngine.OnKeycodeDispatchResponse(TaskCode_DataCollection, GameEngine.InputOperationType.Released)]
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
