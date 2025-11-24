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

namespace GameEngine.Loader.Configuring
{
    /// <summary>
    /// 基础配置信息的枚举类型定义
    /// </summary>
    public enum ConfigureInfoType : byte
    {
        Unknown = 0,
        Comment = 1,
        File = 2,
        Constant = 11,
        Bean = 12,
    }

    /// <summary>
    /// Bean配置数据的节点标签命名
    /// </summary>
    internal static class BeanConfigureNodeName
    {
        public const string Comment = "#comment";
        public const string File = "file";
        public const string Constant = "constant";
        public const string Bean = "bean";
        public const string Field = "field";
        public const string Property = "property";
        public const string Method = "method";
        public const string Component = "component";
    }

    /// <summary>
    /// Bean配置数据的语法标签定义
    /// </summary>
    internal static class BeanConfigureNodeAttributeName
    {
        public const string Name = "name";
        public const string Include = "include";
        public const string ClassType = "class_type";
        public const string ParentName = "parent_name";
        public const string Singleton = "singleton";
        public const string Inherited = "inherited";
        public const string ReferenceName = "reference_name";
        public const string ReferenceType = "reference_type";
        public const string ReferenceValue = "reference_value";
        //public const string ReferenceBean = "reference_bean";
        //public const string ReferenceField = "reference_field";
        //public const string ReferenceProperty = "reference_property";
        public const string Priority = "priority";
        public const string ActivationOn = "activation_on";
    }
}
