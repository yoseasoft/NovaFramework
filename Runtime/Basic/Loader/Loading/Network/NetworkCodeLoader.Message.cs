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

using SystemType = System.Type;
using SystemAttribute = System.Attribute;

namespace GameEngine.Loader
{
    /// <summary>
    /// 程序集中网络消息对象的分析处理类，对业务层载入的所有网络消息类进行统一加载及分析处理
    /// </summary>
    internal static partial class NetworkCodeLoader
    {
        /// <summary>
        /// 网络消息对象类的结构信息管理容器
        /// </summary>
        private static IDictionary<int, Structuring.NetworkMessageCodeInfo> _networkMessageCodeInfos = new Dictionary<int, Structuring.NetworkMessageCodeInfo>();

        [OnCodeLoaderClassLoadOfTarget(typeof(ProtoBuf.Extension.MessageAttribute))]
        private static bool LoadNetworkMessageClass(Symboling.SymClass symClass, bool reload)
        {
            if (false == typeof(ProtoBuf.Extension.IMessage).IsAssignableFrom(symClass.ClassType))
            {
                Debugger.Warn("The target class type '{0}' must be inherited from 'IMessage' interface, load it failed.", symClass.FullName);
                return false;
            }

            Structuring.NetworkMessageCodeInfo info = new Structuring.NetworkMessageCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<SystemAttribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                SystemAttribute attr = attrs[n];
                SystemType attrType = attr.GetType();
                if (typeof(ProtoBuf.Extension.MessageAttribute) == attrType)
                {
                    ProtoBuf.Extension.MessageAttribute _attr = (ProtoBuf.Extension.MessageAttribute) attr;
                    info.Opcode = _attr.Opcode;
                }
                else if (typeof(ProtoBuf.Extension.MessageResponseTypeAttribute) == attrType)
                {
                    ProtoBuf.Extension.MessageResponseTypeAttribute _attr = (ProtoBuf.Extension.MessageResponseTypeAttribute) attr;
                    info.ResponseCode = _attr.Opcode;
                }
            }

            if (info.Opcode <= 0)
            {
                Debugger.Warn("The network message opcode '{0}' was invalid, newly added class '{1}' failed.", info.Opcode, symClass.FullName);
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
                    Debugger.Warn("The network message opcode '{0}' was already existed, repeat added it failed.", info.Opcode);
                    return false;
                }
            }

            _networkMessageCodeInfos.Add(info.Opcode, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load 'IMessage' code info '{0}' succeed from target class type '{1}'.", CodeLoaderObject.ToString(info), symClass.FullName);

            return true;
        }

        [OnCodeLoaderClassCleanupOfTarget(typeof(ProtoBuf.Extension.MessageAttribute))]
        private static void CleanupAllNetworkMessageClasses()
        {
            _networkMessageCodeInfos.Clear();
        }

        [OnCodeLoaderClassLookupOfTarget(typeof(ProtoBuf.Extension.MessageAttribute))]
        private static Structuring.NetworkMessageCodeInfo LookupNetworkMessageCodeInfo(Symboling.SymClass symClass)
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
