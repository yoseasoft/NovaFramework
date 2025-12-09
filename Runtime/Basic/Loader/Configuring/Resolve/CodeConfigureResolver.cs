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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Runtime.CompilerServices;

namespace GameEngine.Loader.Configuring
{
    /// <summary>
    /// 对象配置数据的解析类，对外部配置数据的结构信息进行解析和构建
    /// </summary>
    internal static partial class CodeConfigureResolver
    {
        /// <summary>
        /// 配置数据对象解析的函数句柄定义
        /// </summary>
        /// <param name="node">目标对象类型</param>
        private delegate void OnConfigureObjectLoadingHandler(XmlNode node);

        /// <summary>
        /// 配置解析回调句柄管理容器
        /// </summary>
        private static IDictionary<XmlNodeType, IDictionary<string, OnConfigureObjectLoadingHandler>> _codeConfigureResolveCallbacks = null;

        /// <summary>
        /// 配置基础对象类管理容器
        /// </summary>
        private static IDictionary<string, BaseConfigureInfo> _nodeConfigureInfos = null;

        /// <summary>
        /// 配置文件清单管理容器
        /// </summary>
        private static IList<string> _configureFilePaths = null;

        /// <summary>
        /// 配置解析类的初始化函数
        /// </summary>
        public static void Initialize()
        {
            // 初始化解析容器
            _codeConfigureResolveCallbacks = new Dictionary<XmlNodeType, IDictionary<string, OnConfigureObjectLoadingHandler>>();
            // 初始化实例容器
            _nodeConfigureInfos = new Dictionary<string, BaseConfigureInfo>();

            Type targetType = typeof(CodeConfigureResolver);
            MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            for (int n = 0; n < methods.Length; ++n)
            {
                MethodInfo method = methods[n];
                Attribute attr = method.GetCustomAttribute(typeof(OnXmlConfigureResolvingCallbackAttribute));
                if (null != attr)
                {
                    OnXmlConfigureResolvingCallbackAttribute _attr = (OnXmlConfigureResolvingCallbackAttribute) attr;

                    OnConfigureObjectLoadingHandler callback = method.CreateDelegate(typeof(OnConfigureObjectLoadingHandler)) as OnConfigureObjectLoadingHandler;
                    Debugger.Assert(null != callback, "Invalid configure resolve callback.");

                    AddConfigureResolveCallback(_attr.NodeType, _attr.NodeName, callback);
                }
            }
        }

        /// <summary>
        /// 配置解析类的清理函数
        /// </summary>
        public static void Cleanup()
        {
            // 清理实例容器
            UnloadAllConfigureContents();
            _nodeConfigureInfos = null;
            _configureFilePaths = null;

            // 清理解析容器
            RemoveAllConfigureResolveCallbacks();
            _codeConfigureResolveCallbacks = null;
        }

        /// <summary>
        /// 加载通过指定节点实例解析的配置数据对象
        /// </summary>
        /// <param name="node">节点实例</param>
        public static void LoadConfigureContent(XmlNode node)
        {
            XmlNodeType nodeType = node.NodeType;
            string nodeName = node.Name;

            if (false == _codeConfigureResolveCallbacks.TryGetValue(nodeType, out IDictionary<string, OnConfigureObjectLoadingHandler> callbacks))
            {
                Debugger.Error(LogGroupTag.CodeLoader, "Could not resolve target node type '{%i}', loaded content failed.", nodeType);
                return;
            }

            if (false == callbacks.TryGetValue(nodeName, out OnConfigureObjectLoadingHandler callback))
            {
                Debugger.Error(LogGroupTag.CodeLoader, "Could not found any node name '{%s}' from current resolve process, loaded it failed.", nodeName);
                return;
            }

            callback(node);
        }

        /// <summary>
        /// 卸载当前所有解析登记的配置数据对象实例
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnloadAllConfigureContents()
        {
            _nodeConfigureInfos?.Clear();
            _configureFilePaths?.Clear();
        }

        /// <summary>
        /// 新增指定的配置数据结构信息对象实例
        /// </summary>
        /// <param name="info">配置数据对象实例</param>
        private static void AddNodeConfigureInfo(BaseConfigureInfo info)
        {
            if (_nodeConfigureInfos.ContainsKey(info.Name))
            {
                Debugger.Error(LogGroupTag.CodeLoader, "The resolve configure info '{%s}' was already exist, repeat added it will be override old value.", info.Name);
                _nodeConfigureInfos.Remove(info.Name);
            }

            _nodeConfigureInfos.Add(info.Name, info);
        }

        /// <summary>
        /// 通过指定的配置名称，获取对应的配置数据结构信息
        /// </summary>
        /// <param name="targetName">配置名称</param>
        /// <returns>返回配置数据实例，若查找失败返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BaseConfigureInfo GetNodeConfigureInfoByName(string targetName)
        {
            if (_nodeConfigureInfos.TryGetValue(targetName, out BaseConfigureInfo info))
            {
                return info;
            }

            return null;
        }

        /// <summary>
        /// 获取当前注册的所有配置数据结构信息对象实例
        /// </summary>
        /// <returns>返回当前所有配置数据实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<BaseConfigureInfo> GetAllNodeConfigureInfos()
        {
            return NovaEngine.Utility.Collection.ToListForValues(_nodeConfigureInfos);
        }

        #region 配置解析回调句柄注册绑定接口函数

        /// <summary>
        /// 新增指定类型及名称对应的配置解析回调句柄
        /// </summary>
        /// <param name="nodeType">节点类型</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="callback">解析回调句柄</param>
        private static void AddConfigureResolveCallback(XmlNodeType nodeType, string nodeName, OnConfigureObjectLoadingHandler callback)
        {
            if (false == _codeConfigureResolveCallbacks.TryGetValue(nodeType, out IDictionary<string, OnConfigureObjectLoadingHandler> callbacks))
            {
                callbacks = new Dictionary<string, OnConfigureObjectLoadingHandler>();
                _codeConfigureResolveCallbacks.Add(nodeType, callbacks);
            }

            if (callbacks.ContainsKey(nodeName))
            {
                Debugger.Warn(LogGroupTag.CodeLoader, "The configure node name '{%s}' was already exist, repeat added it will be override old value.", nodeName);
                callbacks.Remove(nodeName);
            }

            callbacks.Add(nodeName, callback);
        }

        /// <summary>
        /// 移除所有注册的配置解析回调句柄
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RemoveAllConfigureResolveCallbacks()
        {
            _codeConfigureResolveCallbacks.Clear();
        }

        #endregion

        #region 内联配置文件的访问记录登记/管理接口函数

        /// <summary>
        /// 添加新的配置文件路径到当前的加载上下文中
        /// </summary>
        /// <param name="path">文件路径</param>
        private static void AddConfigureFilePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (null == _configureFilePaths)
                _configureFilePaths = new List<string>();

            if (_configureFilePaths.Contains(path))
            {
                Debugger.Warn(LogGroupTag.CodeLoader, "The configure file path '{%s}' was already exists, repeat added it will be override old value.", path);
                return;
            }

            _configureFilePaths.Add(path);
        }

        /// <summary>
        /// 获取下一个未处理的配置文件加载路径
        /// </summary>
        /// <returns>返回文件路径地址</returns>
        public static string PopNextConfigureFileLoadPath()
        {
            if (null != _configureFilePaths && _configureFilePaths.Count > 0)
            {
                string path = _configureFilePaths[0];
                _configureFilePaths.RemoveAt(0);
                return path;
            }

            return null;
        }

        #endregion
    }
}
