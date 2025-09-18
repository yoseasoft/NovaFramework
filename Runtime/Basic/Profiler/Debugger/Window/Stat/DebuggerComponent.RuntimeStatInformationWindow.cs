/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemDateTime = System.DateTime;
using UnityGUILayout = UnityEngine.GUILayout;

namespace GameEngine.Profiler.Debugging
{
    /// <summary>
    /// 游戏调试器组件对象类，用于定义调试器对象的基础属性及访问操作函数
    /// </summary>
    public sealed partial class DebuggerComponent
    {
        /// <summary>
        /// 通用模式的统计模块信息展示窗口的对象类
        /// </summary>
        public abstract class RuntimeStatInformationWindow<T> : BaseScrollableDebuggerWindow where T : Statistics.IStat
        {
            protected override void OnDrawScrollableWindow()
            {
                T stat = Statistics.Statistician.GetStat<T>();

                int statType = 0;
                string statName = string.Empty;
                IList<Statistics.StatInfo> infos = null;
                if (null != stat)
                {
                    statType = stat.StatType;
                    statName = System.Enum.GetName(typeof(Statistics.StatType), statType);

                    infos = stat.GetAllStatInfos();
                }

                UnityGUILayout.Label(NovaEngine.Utility.Text.Format("<b>{0} Stat Information</b>", statName));
                UnityGUILayout.BeginVertical("box");
                {
                    if (null == infos || infos.Count <= 0)
                    {
                        UnityGUILayout.Label(NovaEngine.Utility.Text.Format("<b>Not found any {0} stat infos in module container.</b>", statName.ToLower()));
                    }
                    else
                    {
                        UnityGUILayout.Label(NovaEngine.Utility.Text.Format("<b>{0} {1} infos obtained at {2}.</b>",
                                infos.Count.ToString(), statName.ToLower(), SystemDateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")));

                        UnityGUILayout.BeginHorizontal();
                        {
                            OnDrawStatInfoTitle();
                        }
                        UnityGUILayout.EndHorizontal();

                        int count = 0;
                        for (int n = 0; n < infos.Count; ++n)
                        {
                            UnityGUILayout.BeginHorizontal();
                            {
                                OnDrawStatInfoContent(infos[n]);
                            }
                            UnityGUILayout.EndHorizontal();

                            count++;
                            if (count >= Configuration.ModuleStatInfoMaxShowCount)
                            {
                                break;
                            }
                        }
                    }
                }
                UnityGUILayout.EndVertical();
            }

            protected abstract void OnDrawStatInfoTitle();

            protected abstract void OnDrawStatInfoContent(Statistics.StatInfo info);

            /// <summary>
            /// 统计模块中对日期时间类型的显示格式
            /// </summary>
            /// <param name="dateTime">日期时间对象</param>
            /// <returns>返回日期时间的显示格式，若为非法格式则返回默认显示值</returns>
            protected static string StatDateTimeToString(SystemDateTime dateTime)
            {
                if (null == dateTime || SystemDateTime.MinValue.Equals(dateTime))
                {
                    return NovaEngine.Definition.CString.Minus;
                }

                return dateTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }
}
