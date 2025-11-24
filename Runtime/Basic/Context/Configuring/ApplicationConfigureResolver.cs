/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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
using System.Runtime.CompilerServices;
using System.Xml;

using SystemType = System.Type;
using SystemAttribute = System.Attribute;

namespace GameEngine.Context.Configuring
{
    /// <summary>
    /// 程序配置数据的解析类，对外部配置数据的结构信息进行解析和构建
    /// </summary>
    internal static partial class ApplicationConfigureResolver
    {
        /// <summary>
        /// 配置数据对象解析的函数句柄定义
        /// </summary>
        /// <param name="node">目标对象类型</param>
        private delegate void OnConfigureObjectLoadingHandler(XmlNode node);

        /// <summary>
        /// 配置解析回调句柄管理容器
        /// </summary>
        private static IDictionary<XmlNodeType, IDictionary<string, OnConfigureObjectLoadingHandler>> _applicationConfigureResolveCallbacks = null;

        /// <summary>
        /// 配置解析类的初始化函数
        /// </summary>
        public static void Initialize()
        {
            // 配置管理对象初始化
            ApplicationConfigureInfo.Initialize();

            // 初始化解析容器
            _applicationConfigureResolveCallbacks = new Dictionary<XmlNodeType, IDictionary<string, OnConfigureObjectLoadingHandler>>();

            SystemType targetType = typeof(ApplicationConfigureResolver);
            MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            for (int n = 0; n < methods.Length; ++n)
            {
                MethodInfo method = methods[n];
                SystemAttribute attr = method.GetCustomAttribute(typeof(OnXmlConfigureResolvingCallbackAttribute));
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
            // 清理解析容器
            RemoveAllConfigureResolveCallbacks();
            _applicationConfigureResolveCallbacks = null;

            // 配置管理对象清理
            ApplicationConfigureInfo.Cleanup();
        }

        /// <summary>
        /// 加载通过指定节点实例解析的配置数据对象
        /// </summary>
        /// <param name="node">节点实例</param>
        public static void LoadConfigureContent(XmlNode node)
        {
            XmlNodeType nodeType = node.NodeType;
            string nodeName = node.Name;

            if (false == _applicationConfigureResolveCallbacks.TryGetValue(nodeType, out IDictionary<string, OnConfigureObjectLoadingHandler> callbacks))
            {
                Debugger.Error("Could not resolve target node type '{%i}', loaded content failed.", nodeType);
                return;
            }

            if (false == callbacks.TryGetValue(nodeName, out OnConfigureObjectLoadingHandler callback))
            {
                Debugger.Error("Could not found any node name '{%s}' from current resolve process, loaded it failed.", nodeName);
                return;
            }

            callback(node);
        }

        /// <summary>
        /// 获取下一个未处理的配置文件加载路径
        /// </summary>
        /// <returns>返回文件路径地址</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string MoveNextConfigureFile()
        {
            return ApplicationConfigureInfo.PopNextConfigureFileLoadPath();
        }

        /// <summary>
        /// 卸载当前所有解析登记的配置数据对象实例
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnloadAllConfigureContents()
        {
            ApplicationConfigureInfo.RemoveAllConfigureInfos();
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
            IDictionary<string, OnConfigureObjectLoadingHandler> callbacks;
            if (false == _applicationConfigureResolveCallbacks.TryGetValue(nodeType, out callbacks))
            {
                callbacks = new Dictionary<string, OnConfigureObjectLoadingHandler>();
                _applicationConfigureResolveCallbacks.Add(nodeType, callbacks);
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
            _applicationConfigureResolveCallbacks.Clear();
        }

        #endregion
    }
}
