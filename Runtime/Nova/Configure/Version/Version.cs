/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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

using System;
using System.Reflection;

namespace NovaEngine
{
    /// <summary>
    /// 软件版本管理工具类，对外提供版号及版本比对等相关接口
    /// 
    /// 版本号基本格式如下“1.1.1.20100101_beta”，其内容主要分为五个部分：
    /// （1）主版本号：当功能模块有较大的变动，比如增加模块或是整体架构发生变化。此版本号由项目决定是否修改。
    /// （2）次版本号：相对于主版本号而言，次版本号的升级对应的只是局部的变动，但该局部的变动造成程序和以前版本不能兼容，或者对该程序以前的协作关系产生了破坏，或者是功能上有大的改进或增强。此版本号由项目决定是否修改。
    /// （3）修订版本号：一般是Bug的修复或是一些小的变动或是一些功能的扩充，要经常发布修订版，修复一个严重Bug即可发布一个修订版。此版本号由项目决定是否修改。
    /// （4）日期版本号：用于记录修改项目的当前日期，每天对项目的修改都需要更改日期版本号。此版本号由开发人员决定是否修改。
    /// （5）希腊字母版本号：此版本号用于标注当前版本的软件处于哪个开发阶段，当软件进入到另一个阶段时需要修改此版本号。此版本号由项目决定是否修改。
    /// </summary>
    public static partial class Version
    {
        ///
        /// 引擎版本号
        /// 

        /// <summary>
        /// 主版本号，重大变动时更改该值
        /// </summary>
        public const int FRAMEWORK_MAJOR = VersionInfo.Major;

        /// <summary>
        /// 次版本号，功能升级或局部变动时更改该值
        /// </summary>
        public const int FRAMEWORK_MINOR = VersionInfo.Minor;

        /// <summary>
        /// 修订版本号，功能扩充或BUG修复时更改该值
        /// </summary>
        public const int FRAMEWORK_REVISION = VersionInfo.Revision;

        /// <summary>
        /// 编译版本号，每次重新编译版本时更改该值
        /// </summary>
        public const int FRAMEWORK_BUILD = VersionInfo.Build;

        /// <summary>
        /// 字母版本号，用于标识当前软件所属的开发阶段
        /// Base：此版本表示该软件仅仅是一个假链接，通常包括所有的功能和布局，但是其中的功能都没有做完整的实现，只是做为整体软件的一个基础架构。
        /// Alpha：软件的初级版本，表示该软件在此阶段以实现软件功能为主，通常只在软件开发者内部交流，一般而言，该版本软件的Bug较多，需要继续修改，是测试版本。测试人员提交Bug经开发人员修改确认之后，发布到测试网址让测试人员测试，此时可将软件版本标注为alpha版。
        /// Beta：该版本相对于Alpha 版已经有了很大的进步，消除了严重错误，但还需要经过多次测试来进一步消除，此版本主要的修改对象是软件的UI。修改的的Bug经测试人员测试确认后可发布到外网上，此时可将软件版本标注为beta版。
        /// RC：该版本已经相当成熟了，基本上不存在导致错误的Bug，与即将发行的正式版本相差无几。
        /// Release：该版本意味“最终版本”，在前面版本的一系列测试版之后，终归会有一个正式的版本，是最终交付用户使用的一个版本。该版本有时也称标准版。
        /// </summary>
        public const PublishType FRAMEWORK_LETTER = VersionInfo.Letter;

        /// <summary>
        /// 获取当前框架版本的格式化串信息
        /// </summary>
        /// <returns>返回当前框架版本信息</returns>
        public static string FrameworkVersionName()
        {
            string v = string.Format("{0}.{1}.{2}.{3}-{4}", FRAMEWORK_MAJOR, FRAMEWORK_MINOR, FRAMEWORK_REVISION, FRAMEWORK_BUILD, FRAMEWORK_LETTER.ToString());

            return v;
        }

        ///
        /// 应用版本号
        ///

        /// <summary>
        /// 主版本号，重大变动时更改该值
        /// </summary>
        public static readonly int APPLICATION_MAJOR = 1;

        /// <summary>
        /// 次版本号，功能升级或局部变动时更改该值
        /// </summary>
        public static readonly int APPLICATION_MINOR = 0;

        /// <summary>
        /// 修订版本号，功能扩充或BUG修复时更改该值
        /// </summary>
        public static readonly int APPLICATION_REVISION = 0;

        /// <summary>
        /// 打包版本号，在程序进行打包分割时更改该值
        /// </summary>
        public static readonly int APPLICATION_PACK = 0;

        /// <summary>
        /// 编译版本号，每次重新编译版本时更改该值
        /// </summary>
        public static readonly int APPLICATION_BUILD = 197001010;

        /// <summary>
        /// 字母版本号，用于标识当前软件所属的开发阶段
        /// Base：此版本表示该软件仅仅是一个假链接，通常包括所有的功能和布局，但是其中的功能都没有做完整的实现，只是做为整体软件的一个基础架构。
        /// Alpha：软件的初级版本，表示该软件在此阶段以实现软件功能为主，通常只在软件开发者内部交流，一般而言，该版本软件的Bug较多，需要继续修改，是测试版本。测试人员提交Bug经开发人员修改确认之后，发布到测试网址让测试人员测试，此时可将软件版本标注为alpha版。
        /// Beta：该版本相对于Alpha 版已经有了很大的进步，消除了严重错误，但还需要经过多次测试来进一步消除，此版本主要的修改对象是软件的UI。修改的的Bug经测试人员测试确认后可发布到外网上，此时可将软件版本标注为beta版。
        /// RC：该版本已经相当成熟了，基本上不存在导致错误的Bug，与即将发行的正式版本相差无几。
        /// Release：该版本意味“最终版本”，在前面版本的一系列测试版之后，终归会有一个正式的版本，是最终交付用户使用的一个版本。该版本有时也称标准版。
        /// Mini：该版本意味“精简版本”，基于最终交付用户使用的标准版的基础上，裁剪掉部分当前阶段暂时不会使用的资源后的一个精简版本，该版本需要通过更新方式补充成完整的标准版。
        /// </summary>
        public static readonly PublishType APPLICATION_LETTER = PublishType.Alpha;

        /// <summary>
        /// 获取当前框架版本的格式化串信息
        /// </summary>
        /// <returns>返回当前框架版本信息</returns>
        public static string ApplicationVersionName()
        {
            string v = string.Format("{0}.{1}.{2}_{3}.{4}_{5}", APPLICATION_MAJOR, APPLICATION_MINOR, APPLICATION_REVISION, APPLICATION_PACK, APPLICATION_BUILD, APPLICATION_LETTER.ToString());

            return v;
        }

        /// <summary>
        /// 设置环境成员属性的值，通过查找与指定字符串相匹配的成员属性设定其对应值
        /// </summary>
        /// <param name="fieldName">属性名称</param>
        /// <param name="fieldValue">属性值</param>
        internal static void SetProperty(string fieldName, object fieldValue)
        {
            Type type = typeof(Version);
            FieldInfo field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.Public);
            field.SetValue(null, fieldValue);
        }

        /// <summary>
        /// 设置环境成员属性的值，通过预定义字段类型设定其对应值
        /// </summary>
        /// <param name="fieldType">属性名称</param>
        /// <param name="fieldValue">属性值</param>
        internal static void SetProperty(FieldType fieldType, object fieldValue)
        {
            SetProperty(fieldType.ToString(), fieldValue);
        }
    }
}
