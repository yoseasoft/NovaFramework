/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using System;
using System.Xml;

namespace GameEngine
{
    /// <summary>
    /// 基于XML文件的配置解析器回调句柄函数的属性定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class OnXmlConfigureResolvingCallbackAttribute : Attribute
    {
        /// <summary>
        /// 解析的目标节点类型
        /// </summary>
        private XmlNodeType _nodeType;
        /// <summary>
        /// 解析的目标节点名称
        /// </summary>
        private string _nodeName;

        public XmlNodeType NodeType => _nodeType;
        public string NodeName => _nodeName;

        public OnXmlConfigureResolvingCallbackAttribute(string nodeName) : this(XmlNodeType.Element, nodeName) { }

        public OnXmlConfigureResolvingCallbackAttribute(XmlNodeType nodeType, string nodeName) : base()
        {
            _nodeType = nodeType;
            _nodeName = nodeName;
        }
    }
}
