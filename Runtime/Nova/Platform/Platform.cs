/// -------------------------------------------------------------------------------
/// NovaEngine Framework
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

namespace NovaEngine
{
    /// <summary>
    /// 系统级的平台管理类，用于定义平台类型及平台参数配置
    /// </summary>
    public partial class Platform : Singleton<Platform>
    {
        /// <summary>
        /// 引擎对象实例当前运行的平台类型
        /// </summary>
        private PlatformType _platformType;

        /**
         * 当引擎打包为DLL后，该属性标识将不会发生任何作用
         * 因此在此处注释掉，若需要在项目启动时对平台进行初始化操作
         * 需要手动调用"Platform.Instance"，通过内部的初始化回调函数进行对应的操作
         * 
         * [UnityEngine.RuntimeInitializeOnLoadMethod]
         * static void _initialize()
         * {
         *     // Platform.PlatformType platformType = Platform.Instance.TargetPlatformType();
         *     // UnityEngine.Debug.Assert(Platform.PlatformType.Unknown != platformType, "Could not found any available platform on target application.");
         * }
         */

        /// <summary>
        /// 平台对象实例的初始化回调接口
        /// </summary>
        protected override void Initialize()
        {
            // 平台类型初始化
            _platformType = CurrentPlatformType;
        }

        /// <summary>
        /// 平台对象实例的清理回调接口
        /// </summary>
        protected override void Cleanup()
        {
            // 重置平台类型
            _platformType = PlatformType.Unknown;
        }

        /// <summary>
        /// 获取引擎对象实例当前运行的平台类型
        /// </summary>
        /// <returns>返回引擎对象实例当前运行的平台类型</returns>
        public PlatformType TargetPlatformType()
        {
            return _platformType;
        }
    }
}
