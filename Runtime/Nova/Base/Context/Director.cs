/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

namespace NovaEngine
{
    /// <summary>
    /// 应用接口管理导演类，用于在引擎内部对事件及函数进行统一的调度转发
    /// </summary>
    internal static class Director
    {
        #region 应用程序管理相关原生UNITY通知接口

        //
        // 原生函数调用顺序及bool变化如下：
        // 强制暂停时：OnApplicationPause -> OnApplicationFocus
        // 重新启动时：OnApplicationFocus -> OnApplicationPause
        //
        // 正常进入游戏：
        // OnApplicationFocus.focus = true
        //
        // 正常启动游戏：
        // OnApplicationLaunching
        //
        // 正常退出游戏：
        // OnApplicationQuit
        //
        // Home键退出游戏：
        // OnApplicationPause.pause = true
        // OnApplicationFocus.focus = false
        //
        // Home键返回游戏：
        // OnApplicationFocus.focus = true
        // OnApplicationPause.pause = false
        //
        // 当前应用双击Home键，Kill进程：
        // OnApplicationQuit（PS：iOS有回调，android没有回调）
        //
        // 跳出当前应用，Kill进程：
        // OnApplicationQuit（PS：iOS和android均没有回调）
        //

        /// <summary>
        /// 当应用程序聚集/失去焦点的时候执行
        /// PS：游戏失去焦点也就是进入后台时focus状态为false，切换回前台时focus状态为true
        /// </summary>
        /// <param name="focus">聚焦状态标识</param>
        public static void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                Application.Instance.OnApplicationWillEnterForeground();
            }
            else
            {
                Application.Instance.OnApplicationDidEnterBackground();
            }
        }

        /// <summary>
        /// 当应用程序暂停/恢复使用的时候执行
        /// PS：游戏进入后台时执行该方法pause状态为true，切换回前台时pause状态为false
        /// </summary>
        /// <param name="pause">暂停状态标识</param>
        public static void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                Application.Instance.OnApplicationWillResignActive();
            }
            else
            {
                Application.Instance.OnApplicationDidBecomeActive();
            }
        }

        /// <summary>
        /// 当应用程序启动的时候执行
        /// </summary>
        public static void OnApplicationLaunching()
        {
            Application.Instance.OnApplicationDidFinishLaunching();
        }

        /// <summary>
        /// 当应用程序退出的时候执行
        /// </summary>
        public static void OnApplicationQuit()
        {
            Application.Instance.OnApplicationWillTerminate();
        }

        #endregion
    }
}
