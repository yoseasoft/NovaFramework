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

using System.Collections.Generic;
using System.Reflection;

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemBindingFlags = System.Reflection.BindingFlags;

using SystemMemoryStream = System.IO.MemoryStream;
using SystemSeekOrigin = System.IO.SeekOrigin;
using SystemXmlDocument = System.Xml.XmlDocument;
using SystemXmlElement = System.Xml.XmlElement;
using SystemXmlNode = System.Xml.XmlNode;
using SystemXmlNodeType = System.Xml.XmlNodeType;
using SystemXmlNodeList = System.Xml.XmlNodeList;

namespace GameEngine.Loader
{
    /// <summary>
    /// 程序集的分析处理类，对业务层载入的所有对象类进行统一加载及分析处理
    /// </summary>
    public static partial class CodeLoader
    {
        /// <summary>
        /// 配置数据对象加载的函数句柄定义
        /// </summary>
        /// <param name="node">目标对象类型</param>
        /// <returns>返回加载成功的配置数据对象实例，若加载失败返回null</returns>
        public delegate Configuring.BaseConfigureInfo OnConfigureObjectLoadhHandler(SystemXmlNode node);

        /// <summary>
        /// 配置解析器回调句柄函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        internal class OnConfigureResolvingCallbackAttribute : SystemAttribute
        {
            /// <summary>
            /// 解析的目标节点类型
            /// </summary>
            private SystemXmlNodeType _nodeType;
            /// <summary>
            /// 解析的目标节点名称
            /// </summary>
            private string _nodeName;

            public SystemXmlNodeType NodeType => _nodeType;
            public string NodeName => _nodeName;

            public OnConfigureResolvingCallbackAttribute(string nodeName) : this(SystemXmlNodeType.Element, nodeName) { }

            public OnConfigureResolvingCallbackAttribute(SystemXmlNodeType nodeType, string nodeName) : base()
            {
                _nodeType = nodeType;
                _nodeName = nodeName;
            }
        }

        /// <summary>
        /// 配置解析回调句柄管理容器
        /// </summary>
        private static IDictionary<SystemXmlNodeType, IDictionary<string, OnConfigureObjectLoadhHandler>> _codeConfigureResolveCallbacks = null;

        /// <summary>
        /// 配置基础对象类管理容器
        /// </summary>
        private static IDictionary<string, Configuring.BaseConfigureInfo> _nodeConfigureInfos = null;

        /// <summary>
        /// 初始化针对所有配置解析类声明的全部绑定回调接口
        /// </summary>
        [OnCodeLoaderSubmoduleInitCallback]
        private static void InitAllCodeConfigureLoadingCallbacks()
        {
            // 初始化解析容器
            _codeConfigureResolveCallbacks = new Dictionary<SystemXmlNodeType, IDictionary<string, OnConfigureObjectLoadhHandler>>();
            // 初始化实例容器
            _nodeConfigureInfos = new Dictionary<string, Configuring.BaseConfigureInfo>();

            SystemType targetType = typeof(Configuring.CodeConfigureResolver);
            SystemMethodInfo[] methods = targetType.GetMethods(SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Static);
            for (int n = 0; n < methods.Length; ++n)
            {
                SystemMethodInfo method = methods[n];
                SystemAttribute attr = method.GetCustomAttribute(typeof(OnConfigureResolvingCallbackAttribute));
                if (null != attr)
                {
                    OnConfigureResolvingCallbackAttribute _attr = (OnConfigureResolvingCallbackAttribute) attr;

                    OnConfigureObjectLoadhHandler callback = method.CreateDelegate(typeof(OnConfigureObjectLoadhHandler)) as OnConfigureObjectLoadhHandler;
                    Debugger.Assert(null != callback, "Invalid configure resolve callback.");

                    AddCodeConfigureResolveCallback(_attr.NodeType, _attr.NodeName, callback);
                }
            }
        }

        /// <summary>
        /// 清理针对所有配置解析类声明的全部绑定回调接口
        /// </summary>
        [OnCodeLoaderSubmoduleCleanupCallback]
        private static void CleanupAllCodeConfigureLoadingCallbacks()
        {
            // 清理实例容器
            UnloadAllConfigureContents();
            _nodeConfigureInfos = null;
            // 清理解析容器
            RemoveAllCodeConfigureResolveCallbacks();
            _codeConfigureResolveCallbacks = null;
        }

        /// <summary>
        /// 加载通用类库的配置数据
        /// </summary>
        /// <param name="buffer">数据流</param>
        /// <param name="offset">数据偏移</param>
        /// <param name="length">数据长度</param>
        private static void LoadGeneralConfigure(byte[] buffer, int offset, int length)
        {
            SystemMemoryStream memoryStream = new SystemMemoryStream();
            memoryStream.Write(buffer, offset, length);
            memoryStream.Seek(0, SystemSeekOrigin.Begin);

            LoadGeneralConfigure(memoryStream);

            memoryStream.Dispose();
        }

        /// <summary>
        /// 加载通用类库的配置数据
        /// </summary>
        /// <param name="memoryStream">数据流</param>
        private static void LoadGeneralConfigure(SystemMemoryStream memoryStream)
        {
            SystemXmlDocument document = new SystemXmlDocument();
            document.Load(memoryStream);

            SystemXmlElement element = document.DocumentElement;
            SystemXmlNodeList nodeList = element.ChildNodes;
            for (int n = 0; null != nodeList && n < nodeList.Count; ++n)
            {
                SystemXmlNode node = nodeList[n];

                LoadConfigureContent(node);
            }
        }

        /// <summary>
        /// 加载通过指定节点实例解析的配置数据对象
        /// </summary>
        /// <param name="node">节点实例</param>
        private static void LoadConfigureContent(SystemXmlNode node)
        {
            SystemXmlNodeType nodeType = node.NodeType;
            string nodeName = node.Name;

            if (false == _codeConfigureResolveCallbacks.TryGetValue(nodeType, out IDictionary<string, OnConfigureObjectLoadhHandler> callbacks))
            {
                Debugger.Error("Could not resolve target node type '{0}', loaded content failed.", nodeType);
                return;
            }

            if (false == callbacks.TryGetValue(nodeName, out OnConfigureObjectLoadhHandler callback))
            {
                Debugger.Error("Could not found any node name '{0}' from current resolve process, loaded it failed.", nodeName);
                return;
            }

            Configuring.BaseConfigureInfo info = callback(node);
            if (null == info)
            {
                // 注释节点不会生成信息对象实例
                if (SystemXmlNodeType.Comment != nodeType)
                {
                    Debugger.Warn("Cannot resolve configure object with target node type '{0}' and name '{1}', loaded it failed.", nodeType, nodeName);
                }
                return;
            }

            if (_nodeConfigureInfos.ContainsKey(info.Name))
            {
                Debugger.Warn("The resolve configure info '{0}' was already exist, repeat added it will be override old value.", info.Name);
                _nodeConfigureInfos.Remove(info.Name);
            }

            _nodeConfigureInfos.Add(info.Name, info);
        }

        /// <summary>
        /// 卸载当前所有解析登记的配置数据对象实例
        /// </summary>
        private static void UnloadAllConfigureContents()
        {
            _nodeConfigureInfos.Clear();
        }

        /// <summary>
        /// 通过指定的配置名称，获取对应的配置数据结构信息
        /// </summary>
        /// <param name="targetName">配置名称</param>
        /// <returns>返回配置数据实例，若查找失败返回null</returns>
        internal static Configuring.BaseConfigureInfo GetConfigureContentByName(string targetName)
        {
            if (_nodeConfigureInfos.TryGetValue(targetName, out Configuring.BaseConfigureInfo info))
            {
                return info;
            }

            return null;
        }

        #region 配置解析回调句柄注册绑定接口函数

        /// <summary>
        /// 新增指定类型及名称对应的配置解析回调句柄
        /// </summary>
        /// <param name="nodeType">节点类型</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="callback">解析回调句柄</param>
        private static void AddCodeConfigureResolveCallback(SystemXmlNodeType nodeType, string nodeName, OnConfigureObjectLoadhHandler callback)
        {
            IDictionary<string, OnConfigureObjectLoadhHandler> callbacks;
            if (false == _codeConfigureResolveCallbacks.TryGetValue(nodeType, out callbacks))
            {
                callbacks = new Dictionary<string, OnConfigureObjectLoadhHandler>();
                _codeConfigureResolveCallbacks.Add(nodeType, callbacks);
            }

            if (callbacks.ContainsKey(nodeName))
            {
                Debugger.Warn("The configure node name '{0}' was already exist, repeat added it will be override old value.", nodeName);
                callbacks.Remove(nodeName);
            }

            callbacks.Add(nodeName, callback);
        }

        /// <summary>
        /// 移除所有注册的配置解析回调句柄
        /// </summary>
        private static void RemoveAllCodeConfigureResolveCallbacks()
        {
            _codeConfigureResolveCallbacks.Clear();
        }

        #endregion
    }
}
