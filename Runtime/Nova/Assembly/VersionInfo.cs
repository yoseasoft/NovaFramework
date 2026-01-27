/// -------------------------------------------------------------------------------
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
///
/// NovaFramework organization and its derivative projects' copyrights, trademarks, patents, and related rights
/// are protected by the laws of the People's Republic of China and relevant international regulations.
///
/// Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
///
/// This project is dual-licensed under the MIT License and Apache License 2.0,
/// please refer to the LICENSE file in the root directory of the source code for the full license text.
///
/// It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
/// or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
/// Any legal disputes and liabilities arising from secondary development based on this project
/// shall be borne solely by the developer; the project organization and contributors assume no responsibility.
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
    /// 引擎框架的版本信息配置文件
    /// 
    /// 版本信息在每次引擎升级维护时，需同步修改相应的版本号
    /// 具体字段含义请参考<see cref="NovaEngine.Version"/>类的成员定义
    /// </summary>
    internal static class VersionInfo
    {
        /// <summary>
        /// 主版本号，重大变动时更改该值
        /// </summary>
        public const int Major = 1;

        /// <summary>
        /// 次版本号，功能升级或局部变动时更改该值
        /// </summary>
        public const int Minor = 0;

        /// <summary>
        /// 修订版本号，功能扩充或BUG修复时更改该值
        /// </summary>
        public const int Revision = 11;

        /// <summary>
        /// 编译版本号，每次重新编译版本时更改该值
        /// </summary>
        public const int Build = 202601141;

        /// <summary>
        /// 字母版本号，用于标识当前软件所属的开发阶段
        /// </summary>
        public const Version.PublishType Letter = Version.PublishType.Alpha;
    }
}
