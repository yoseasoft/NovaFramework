/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

namespace NovaEngine
{
    /// <summary>
    /// 针对MONO行为组件进行封装的控制器接口类<br/>
    /// 用户需要通过该类，实现自定义的控制器对象，用于对整个引擎进行调度
    /// </summary>
    public static class CFrameworkController
    {
        public static void OnApplicationFocus(bool focus)
        {
            // 程序焦点切换刷新通知
            Director.OnApplicationFocus(focus);
        }

        public static void OnApplicationPause(bool pause)
        {
            // 程序暂停切换刷新通知
            Director.OnApplicationPause(pause);
        }

        public static void OnApplicationLaunching()
        {
            // 程序启动通知
            Director.OnApplicationLaunching();
        }

        public static void OnApplicationQuit()
        {
            // 程序退出通知
            Director.OnApplicationQuit();
        }
    }
}
