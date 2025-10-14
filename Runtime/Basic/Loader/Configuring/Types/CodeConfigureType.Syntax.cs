/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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

namespace GameEngine.Loader.Configuring
{
    /// <summary>
    /// 配置数据源的节点标签命名
    /// </summary>
    internal static class ConfigureNodeName
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
    /// 配置数据的语法标签定义
    /// </summary>
    internal static class ConfigureNodeAttributeName
    {
        public const string K_NAME = "name";
        public const string K_CLASS_TYPE = "class_type";
        public const string K_PARENT_NAME = "parent_name";
        public const string K_SINGLETON = "singleton";
        public const string K_INHERITED = "inherited";
        public const string K_REFERENCE_NAME = "reference_name";
        public const string K_REFERENCE_TYPE = "reference_type";
        public const string K_REFERENCE_VALUE = "reference_value";
        //public const string K_REFERENCE_BEAN = "reference_bean";
        //public const string K_REFERENCE_FIELD = "reference_field";
        //public const string K_REFERENCE_PROPERTY = "reference_property";
        public const string K_PRIORITY = "priority";
        public const string K_ACTIVATION_ON = "activation_on";
        public const string K_INCLUDE = "include";
    }
}
