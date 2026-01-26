/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
using UnityEngine.Scripting;

namespace GameEngine.Loader
{
    /// 程序集中网络消息对象的分析处理类
    internal static partial class NetworkCodeLoader
    {
        /// <summary>
        /// 网络消息对象类的结构信息管理容器
        /// </summary>
        private readonly static IDictionary<int, Structuring.NetworkMessageCodeInfo> _networkMessageCodeInfos = new Dictionary<int, Structuring.NetworkMessageCodeInfo>();

        [Preserve]
        [OnCodeLoaderClassLoadOfTarget(typeof(MessageObjectAttribute))]
        private static bool LoadNetworkMessageClass(Symbolling.SymClass symClass, bool reload)
        {
            if (false == NovaEngine.Utility.Reflection.IsTypeOfInstantiableClass(symClass.ClassType))
            {
                Debugger.Warn(LogGroupTag.CodeLoader, "The target class type '{%s}' must be instantiable class, load it failed.", symClass.FullName);
                return false;
            }

            Structuring.NetworkMessageCodeInfo info = new Structuring.NetworkMessageCodeInfo();
            info.ClassType = symClass.ClassType;

            // IMessageObjectTypeResolver messageObjectTypeResolver = NetworkHandler.Instance.GetMessageObjectTypeResolver();
            // if (null == messageObjectTypeResolver)
            // {
            //     Debugger.Error(LogGroupTag.CodeLoader, "The message object type resolver must be non-null, resolve message object '{%s}' failed.", symClass.FullName);
            //     return false;
            // }

            /**
             * 2025-11-07：
             * 通过外部设置的解析器来对消息对象进行解析
             * 这样可以支持非ProtoBuf的消息对象类
             * 
             * IList<SystemAttribute> attrs = symClass.Attributes;
             * for (int n = 0; null != attrs && n < attrs.Count; ++n)
             * {
             *     SystemAttribute attr = attrs[n];
             *     SystemType attrType = attr.GetType();
             *     if (typeof(ProtoBuf.Extension.MessageAttribute) == attrType)
             *     {
             *         ProtoBuf.Extension.MessageAttribute _attr = (ProtoBuf.Extension.MessageAttribute) attr;
             *         info.Opcode = _attr.Opcode;
             *     }
             *     else if (typeof(ProtoBuf.Extension.MessageResponseTypeAttribute) == attrType)
             *     {
             *         ProtoBuf.Extension.MessageResponseTypeAttribute _attr = (ProtoBuf.Extension.MessageResponseTypeAttribute) attr;
             *         info.ResponseCode = _attr.Opcode;
             *     }
             * }
             */

            if (symClass.TryGetFeatureObject<MessageObjectAttribute>(out MessageObjectAttribute messageObjectAttribute))
            {
                info.Opcode = messageObjectAttribute.Opcode;
                info.ResponseCode = messageObjectAttribute.ResponseCode;
            }
            //IMessageProtocolLoader.MessageObjectTypeInfo messageObjectTypeInfo = NetworkHandler.Instance.ParseMessageObjectType(symClass.ClassType);
            //info.Opcode = messageObjectTypeInfo.opcode;
            //info.ResponseCode = messageObjectTypeInfo.responseCode;

            if (info.Opcode <= 0)
            {
                Debugger.Warn(LogGroupTag.CodeLoader, "The network message opcode '{%d}' was invalid, newly added class '{%s}' failed.", info.Opcode, symClass.FullName);
                return false;
            }

            if (_networkMessageCodeInfos.ContainsKey(info.Opcode))
            {
                if (reload)
                {
                    _networkMessageCodeInfos.Remove(info.Opcode);
                }
                else
                {
                    Debugger.Warn(LogGroupTag.CodeLoader, "The network message opcode '{%d}' was already existed, repeat added it failed.", info.Opcode);
                    return false;
                }
            }

            _networkMessageCodeInfos.Add(info.Opcode, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load 'IMessage' code info '{%s}' succeed from target class type '{%s}'.", CodeLoaderUtils.ToString(info), symClass.FullName);

            return true;
        }

        [Preserve]
        [OnCodeLoaderClassCleanupOfTarget(typeof(MessageObjectAttribute))]
        private static void CleanupAllNetworkMessageClasses()
        {
            _networkMessageCodeInfos.Clear();
        }

        [Preserve]
        [OnCodeLoaderClassLookupOfTarget(typeof(MessageObjectAttribute))]
        private static Structuring.NetworkMessageCodeInfo LookupNetworkMessageCodeInfo(Symbolling.SymClass symClass)
        {
            foreach (KeyValuePair<int, Structuring.NetworkMessageCodeInfo> pair in _networkMessageCodeInfos)
            {
                if (pair.Value.ClassType == symClass.ClassType)
                {
                    return pair.Value;
                }
            }

            return null;
        }
    }
}
