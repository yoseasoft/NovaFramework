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

using UnityScreen = UnityEngine.Screen;
using UnitySleepTimeout = UnityEngine.SleepTimeout;

namespace NovaEngine
{
    /// <summary>
    /// 运行的设备环境管理类，用于访问及设置当前运行设备的配置参数
    /// </summary>
    public partial class Device : Singleton<Device>
    {
        /// <summary>
        /// 定义默认的屏幕dpi值
        /// </summary>
        private const float DefaultScreenDpi = 96; // // default windows dpi

        /// <summary>
        /// 当前设备屏幕的dpi（每英寸点数），对于不支持读取dpi的设备，默认为0f
        /// </summary>
        private float _screenDpi = 0f;

        /// <summary>
        /// 获取当前设备屏幕的dpi
        /// </summary>
        public float ScreenDpi
        {
            get { return _screenDpi; }
        }

        /// <summary>
        /// 设备管理对象实例的初始化回调接口
        /// </summary>
        protected override void Initialize()
        {
        }

        /// <summary>
        /// 设备管理对象实例的清理回调接口
        /// </summary>
        protected override void Cleanup()
        {
        }

        /// <summary>
        /// 设备管理对象实例启动函数
        /// </summary>
        /// <returns>当对象实例启动成功时返回true，否则返回false</returns>
        public bool Startup()
        {
            _screenDpi = UnityScreen.dpi;
            if (_screenDpi <= 0f)
            {
                _screenDpi = DefaultScreenDpi;
            }

            // 屏幕休眠设置
            UnityScreen.sleepTimeout = Configuration.OnScreenNeverSleep ? UnitySleepTimeout.NeverSleep : UnitySleepTimeout.SystemSetting;

            return true;
        }

        /// <summary>
        /// 设备管理对象实例关闭函数
        /// </summary>
        public void Shutdown()
        {
        }
    }
}
