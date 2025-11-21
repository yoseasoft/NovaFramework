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

using System.Collections.Generic;
using System.Reflection;
using System.Customize.Extension;

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
using SystemXmlNodeList = System.Xml.XmlNodeList;
using SystemXmlNodeType = System.Xml.XmlNodeType;

namespace GameEngine.Loader
{
    /// <summary>
    /// 程序集的分析处理类，对业务层载入的所有对象类进行统一加载及分析处理
    /// </summary>
    public static partial class CodeLoader
    {
        /// <summary>
        /// 配置文件数据加载的函数句柄定义
        /// </summary>
        /// <param name="url">资源路径</param>
        /// <param name="ms">数据流</param>
        /// <returns>加载成功返回true，若加载失败返回false</returns>
        public delegate bool OnConfigureFileStreamLoadHandler(string url, SystemMemoryStream ms);

        /// <summary>
        /// 配置文件数据加载的函数句柄定义
        /// </summary>
        /// <param name="url">资源路径</param>
        /// <returns>返回加载成功的配置数据内容，若加载失败返回null</returns>
        public delegate string OnConfigureFileTextLoadHandler(string url);

        /// <summary>
        /// 配置数据对象加载的函数句柄定义
        /// </summary>
        /// <param name="node">目标对象类型</param>
        /// <returns>返回加载成功的配置数据对象实例，若加载失败返回null</returns>
        private delegate Configuring.BaseConfigureInfo OnConfigureObjectLoadinghHandler(SystemXmlNode node);

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
        private static IDictionary<SystemXmlNodeType, IDictionary<string, OnConfigureObjectLoadinghHandler>> _codeConfigureResolveCallbacks = null;

        /// <summary>
        /// 配置基础对象类管理容器
        /// </summary>
        private static IDictionary<string, Configuring.BaseConfigureInfo> _nodeConfigureInfos = null;

        /// <summary>
        /// 配置文件清单管理容器
        /// </summary>
        private static IList<ConfigureFileInfo> _nodeConfigureFilePaths = null;

        /// <summary>
        /// 初始化针对所有配置解析类声明的全部绑定回调接口
        /// </summary>
        [OnCodeLoaderSubmoduleInitCallback]
        private static void InitAllCodeConfigureLoadingCallbacks()
        {
            // 初始化解析容器
            _codeConfigureResolveCallbacks = new Dictionary<SystemXmlNodeType, IDictionary<string, OnConfigureObjectLoadinghHandler>>();
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

                    OnConfigureObjectLoadinghHandler callback = method.CreateDelegate(typeof(OnConfigureObjectLoadinghHandler)) as OnConfigureObjectLoadinghHandler;
                    Debugger.Assert(null != callback, "Invalid configure resolve callback.");

                    AddConfigureResolveCallback(_attr.NodeType, _attr.NodeName, callback);
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
            _nodeConfigureFilePaths = null;

            // 清理解析容器
            RemoveAllConfigureResolveCallbacks();
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
        /// 重载通用类库的配置数据
        /// </summary>
        /// <param name="callback">回调句柄</param>
        private static void ReloadGeneralConfigure(OnConfigureFileStreamLoadHandler callback)
        {
            // 卸载旧的配置数据
            UnloadAllConfigureContents();

            string path = null;
            if (null == callback)
            {
                Debugger.Error(LogGroupTag.CodeLoader, "The configure file load handler must be non-null, reload general configure failed!");
                return;
            }

            do
            {
                SystemMemoryStream ms = new SystemMemoryStream();
                Debugger.Info(LogGroupTag.CodeLoader, "指定的实体配置文件‘{%s}’开始进行进入加载队列中……", path);
                if (false == callback(path, ms))
                {
                    Debugger.Error(LogGroupTag.CodeLoader, "重载Bean配置数据失败：指定路径‘{%s}’下的配置文件加载回调接口执行异常！", path);
                    return;
                }

                // 加载配置
                LoadGeneralConfigure(ms);

                ms.Dispose();

                // 标识该路径加载完成
                OnConfigureFileLoadingCompleted(path);

                // 获取下一个文件路径
                path = GetNextUnprocessedConfigureFileLoadPath();
            } while (null != path);
        }

        /// <summary>
        /// 重载通用类库的配置数据
        /// </summary>
        /// <param name="callback">回调句柄</param>
        private static void ReloadGeneralConfigure(OnConfigureFileTextLoadHandler callback)
        {
            ReloadGeneralConfigure((url, ms) =>
            {
                string text = callback(url);
                if (text.IsNullOrEmpty())
                    return false;

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
                ms.Write(buffer, 0, buffer.Length);
                ms.Seek(0, SystemSeekOrigin.Begin);

                return true;
            });
        }

        /// <summary>
        /// 加载通过指定节点实例解析的配置数据对象
        /// </summary>
        /// <param name="node">节点实例</param>
        private static void LoadConfigureContent(SystemXmlNode node)
        {
            SystemXmlNodeType nodeType = node.NodeType;
            string nodeName = node.Name;

            if (false == _codeConfigureResolveCallbacks.TryGetValue(nodeType, out IDictionary<string, OnConfigureObjectLoadinghHandler> callbacks))
            {
                Debugger.Error("Could not resolve target node type '{0}', loaded content failed.", nodeType);
                return;
            }

            if (false == callbacks.TryGetValue(nodeName, out OnConfigureObjectLoadinghHandler callback))
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

            switch (info.Type)
            {
                case Configuring.ConfigureInfoType.File:
                    AddConfigureFilePath((info as Configuring.FileConfigureInfo)?.Include);
                    break;
                case Configuring.ConfigureInfoType.Bean:
                    if (_nodeConfigureInfos.ContainsKey(info.Name))
                    {
                        Debugger.Warn("The resolve configure info '{0}' was already exist, repeat added it will be override old value.", info.Name);
                        _nodeConfigureInfos.Remove(info.Name);
                    }

                    _nodeConfigureInfos.Add(info.Name, info);
                    break;
            }
        }

        /// <summary>
        /// 卸载当前所有解析登记的配置数据对象实例
        /// </summary>
        private static void UnloadAllConfigureContents()
        {
            _nodeConfigureInfos?.Clear();
            _nodeConfigureFilePaths?.Clear();
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
        private static void AddConfigureResolveCallback(SystemXmlNodeType nodeType, string nodeName, OnConfigureObjectLoadinghHandler callback)
        {
            IDictionary<string, OnConfigureObjectLoadinghHandler> callbacks;
            if (false == _codeConfigureResolveCallbacks.TryGetValue(nodeType, out callbacks))
            {
                callbacks = new Dictionary<string, OnConfigureObjectLoadinghHandler>();
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
        private static void RemoveAllConfigureResolveCallbacks()
        {
            _codeConfigureResolveCallbacks.Clear();
        }

        #endregion

        #region 内联配置文件的访问记录登记/管理接口函数

        /// <summary>
        /// 配置文件信息管理对象类
        /// </summary>
        private class ConfigureFileInfo
        {
            /// <summary>
            /// 文件路径
            /// </summary>
            public string path;
            /// <summary>
            /// 文件解析完成状态标识
            /// </summary>
            public bool loaded;
        }

        /// <summary>
        /// 添加新的配置文件路径到当前的加载上下文中
        /// </summary>
        /// <param name="path">文件路径</param>
        private static void AddConfigureFilePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (null == _nodeConfigureFilePaths)
                _nodeConfigureFilePaths = new List<ConfigureFileInfo>();

            ConfigureFileInfo info = null;
            for (int n = 0; n < _nodeConfigureFilePaths.Count; ++n)
            {
                info = _nodeConfigureFilePaths[n];
                if (info.path.Equals(path))
                {
                    Debugger.Warn("The configure file path '{%s}' was already exists, repeat added it will be overried old value.");
                    return;
                }
            }

            info = new ConfigureFileInfo() { path = path, loaded = false };
            _nodeConfigureFilePaths.Add(info);
        }

        /// <summary>
        /// 指定文件路径下的配置文件加载完成回调通知接口函数
        /// </summary>
        /// <param name="path">文件路径</param>
        private static void OnConfigureFileLoadingCompleted(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            for (int n = 0; null != _nodeConfigureFilePaths && n < _nodeConfigureFilePaths.Count; ++n)
            {
                ConfigureFileInfo info = _nodeConfigureFilePaths[n];
                if (info.path.Equals(path))
                {
                    info.loaded = true;
                    return;
                }
            }

            Debugger.Warn("Could not found any valid configure info matched target path '{%s}', notify loading message failed.", path);
        }

        /// <summary>
        /// 获取下一个未处理的配置文件加载路径
        /// </summary>
        /// <returns>返回文件路径地址</returns>
        private static string GetNextUnprocessedConfigureFileLoadPath()
        {
            for (int n = 0; null != _nodeConfigureFilePaths && n <_nodeConfigureFilePaths.Count; ++n)
            {
                ConfigureFileInfo info = _nodeConfigureFilePaths[n];
                if (false == info.loaded)
                {
                    return info.path;
                }
            }

            return null;
        }

        #endregion
    }
}
