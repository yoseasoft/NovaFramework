/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
    /// 引擎框架的配置参数定义文件，包括环境参数，版本参数等内容
    /// 
    /// 版本参数在每次引擎升级维护时，需同步修改相应的版本号
    /// 具体字段含义请参考<see cref="NovaEngine.Version"/>类的成员定义
    /// </summary>
    public static class GlobalMacros
    {
        /// <summary>
        /// 编译配置选项，调试模式
        /// </summary>
        public const string BUILD_CONFIGURATION_DEBUG = "DEBUG";

        /// <summary>
        /// 编译配置选项，发布模式
        /// </summary>
        public const string BUILD_CONFIGURATION_RELEASE = "RELEASE";

        /// <summary>
        /// 主版本号，重大变动时更改该值
        /// </summary>
        public const int VERSION_FRAMEWORK_MAJOR = 2;

        /// <summary>
        /// 次版本号，功能升级或局部变动时更改该值
        /// </summary>
        public const int VERSION_FRAMEWORK_MINOR = 1;

        /// <summary>
        /// 修订版本号，功能扩充或BUG修复时更改该值
        /// </summary>
        public const int VERSION_FRAMEWORK_REVISION = 1;

        /// <summary>
        /// 编译版本号，每次重新编译版本时更改该值
        /// </summary>
        public const int VERSION_FRAMEWORK_BUILD = 202501040;

        /// <summary>
        /// 字母版本号，用于标识当前软件所属的开发阶段
        /// </summary>
        public const Version.EPublishType VERSION_FRAMEWORK_LETTER = Version.EPublishType.Alpha;
    }
}
