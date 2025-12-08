/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
using System.Reflection;
using System.Text;

namespace NovaEngine
{
    /// <summary>
    /// 基础配置参数定义类，对当前引擎运行所需的初始化参数进行设置及管理
    /// </summary>
    public static partial class Configuration
    {
        /// <summary>
        /// 游戏业务层访问入口名称
        /// </summary>
        public readonly static string gameEntryName = null;

        /// <summary>
        /// 屏幕禁止休眠模式启用开关
        /// </summary>
        public readonly static bool screenNeverSleep = false;

        /// <summary>
        /// 网络消息包头长度配置
        /// </summary>
        public readonly static int networkMessageHeaderSize = 0;

        /// <summary>
        /// 自动开启的日志通道类型标识
        /// </summary>
        public readonly static int logChannel = 0;

        /// <summary>
        /// 日志文本在编辑模式下使用自定义颜色的开启状态标识
        /// </summary>
        public readonly static bool logUsingCustomColor = false;

        /// <summary>
        /// 日志文本在编辑模式下使用系统颜色的开启状态标识
        /// </summary>
        public readonly static bool logUsingSystemColor = false;

        /// <summary>
        /// 教程模式，打开该选项后，将跳转至框架示例环境，执行教程代码<br/>
        /// 在正式版本中，该标识将默认关闭，仅在开发环境中有效
        /// </summary>
        public readonly static bool tutorialMode = false;

        /// <summary>
        /// 教程案例类型，通过该类型引导跳转到具体的示例代码环境<br/>
        /// 在正式版本中，该类型将默认置空，仅在开发环境中有效
        /// </summary>
        public readonly static string tutorialSampleType = null;

        ///////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 静态类的<see cref="object.ToString"/>函数
        /// </summary>
        /// <returns>返回字符串信息</returns>
        public static string ToCString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("PROPERTIES={");
            FieldInfo[] fields = typeof(Configuration).GetFields();
            for (int n = 0; n < fields.Length; ++n)
            {
                FieldInfo field = fields[n];
                sb.AppendFormat("{0}={1},", field.Name, field.GetValue(null));
            }

            sb.Append("},VARIABLES={");
            foreach (KeyValuePair<string, string> pair in _variables)
            {
                sb.AppendFormat("{0}={1},", pair.Key, pair.Value);
            }

            sb.Append("}");

            return sb.ToString();
        }
    }
}
