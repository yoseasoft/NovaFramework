/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

            IList<string> list = new List<string>()
            {
                "mscorlib",
                "netstandard",
                "log4net",
                "dnlib",
                "LZ4",
                "nunit.framework",
                "unityplastic",
                "Anonymously Hosted DynamicMethods Assembly",
                "PlayerBuildProgramLibrary.Data"
            };
            System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            System.Text.StringBuilder sb1 = new System.Text.StringBuilder();
            System.Text.StringBuilder sb2 = new System.Text.StringBuilder();
            int engineLibraryCount = 0, userLibraryCount = 0;
            for (int n = 0; n < assemblies.Length; n++)
            {
                System.Reflection.Assembly assembly = assemblies[n];
                string name = assembly.GetName().Name;
                if (name.StartsWith("System") ||
                    name.StartsWith("Unity") ||
                    name.StartsWith("Mono") ||
                    name.StartsWith("Assembly-") ||
                    name.StartsWith("Bee") ||
                    name.StartsWith("JetBrains") ||
                    name.StartsWith("Sirenix") ||
                    name.StartsWith("HybridCLR") ||
                    list.Contains(name))
                {
                    sb1.AppendFormat("[{0}]={1},", engineLibraryCount, name);
                    engineLibraryCount++;
                }
                else
                {
                    sb2.AppendFormat("[{0}]={1},", userLibraryCount, name);
                    userLibraryCount++;
                }
            }
            Debugger.Log("引擎库有{%d}个，用户库有{%d}个。", engineLibraryCount, userLibraryCount);
            Debugger.Log("引擎库有：{%s}", sb1.ToString());
            Debugger.Log("用户库有：{%s}", sb2.ToString());
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
