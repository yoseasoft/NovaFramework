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
        /// 角色统计模块信息展示窗口的对象类
        /// </summary>
        private sealed class RuntimeActorStatInformationWindow : RuntimeStatInformationWindow<Statistics.ActorStat>
        {
            protected override void OnDrawStatInfoTitle()
            {
                UnityGUILayout.Label("<b>Index</b>");
                UnityGUILayout.Label("<b>Actor Name</b>", UnityGUILayout.Width(120f));
                UnityGUILayout.Label("<b>Create Time</b>", UnityGUILayout.Width(160f));
                UnityGUILayout.Label("<b>Release Time</b>", UnityGUILayout.Width(160f));
                UnityGUILayout.Label("<b>Hash Code</b>", UnityGUILayout.Width(80f));
            }

            protected override void OnDrawStatInfoContent(Statistics.IStatInfo info)
            {
                Statistics.ActorStatInfo actorStatInfo = info as Statistics.ActorStatInfo;

                UnityGUILayout.Label(actorStatInfo.Uid.ToString());
                UnityGUILayout.Label(actorStatInfo.ActorName, UnityGUILayout.Width(120f));
                UnityGUILayout.Label(StatDateTimeToString(actorStatInfo.CreateTime), UnityGUILayout.Width(160f));
                UnityGUILayout.Label(StatDateTimeToString(actorStatInfo.ReleaseTime), UnityGUILayout.Width(160f));
                UnityGUILayout.Label(actorStatInfo.HashCode.ToString());
            }
        }
    }
}
