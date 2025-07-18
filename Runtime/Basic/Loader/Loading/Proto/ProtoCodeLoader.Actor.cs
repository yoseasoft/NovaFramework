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
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader
{
    /// <summary>
    /// 角色类的结构信息
    /// </summary>
    public class ActorCodeInfo : EntityCodeInfo
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        private string _actorName;
        /// <summary>
        /// 角色功能类型
        /// </summary>
        private int _funcType;

        public string ActorName { get { return _actorName; } internal set { _actorName = value; } }
        public int FuncType { get { return _funcType; } internal set { _funcType = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("Actor = { ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());
            sb.AppendFormat("Name = {0}, ", _actorName ?? NovaEngine.Definition.CString.Unknown);
            sb.AppendFormat("FuncType = {0}, ", _funcType);
            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 程序集中原型对象的分析处理类，对业务层载入的所有原型对象类进行统一加载及分析处理
    /// </summary>
    internal static partial class ProtoCodeLoader
    {
        /// <summary>
        /// 对象类的结构信息管理容器
        /// </summary>
        private static IDictionary<string, ActorCodeInfo> _actorCodeInfos = new Dictionary<string, ActorCodeInfo>();

        [OnProtoClassLoadOfTarget(typeof(CActor))]
        private static bool LoadActorClass(Symboling.SymClass symClass, bool reload)
        {
            if (false == typeof(CActor).IsAssignableFrom(symClass.ClassType))
            {
                Debugger.Warn("The target class type '{0}' must be inherited from 'CActor' interface, load it failed.", symClass.FullName);
                return false;
            }

            ActorCodeInfo info = new ActorCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<SystemAttribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                SystemAttribute attr = attrs[n];
                SystemType attrType = attr.GetType();
                if (typeof(DeclareActorClassAttribute) == attrType)
                {
                    DeclareActorClassAttribute _attr = (DeclareActorClassAttribute) attr;
                    info.ActorName = _attr.ActorName;
                    info.FuncType = _attr.FuncType;
                }
                else
                {
                    LoadEntityClassByAttributeType(symClass, info, attr);
                }
            }

            IList<Symboling.SymMethod> symMethods = symClass.GetAllMethods();
            for (int n = 0; null != symMethods && n < symMethods.Count; ++n)
            {
                Symboling.SymMethod symMethod = symMethods[n];

                LoadActorMethod(symClass, info, symMethod);
            }

            if (string.IsNullOrEmpty(info.ActorName))
            {
                const string ACTOR_TAG = CActor.CLASS_SUFFIX_TAG;
                string actorName = symClass.ClassName;
                if (actorName.Length > ACTOR_TAG.Length)
                {
                    // 判断是否有后缀标签
                    if (actorName.Substring(actorName.Length - ACTOR_TAG.Length).Equals(ACTOR_TAG))
                    {
                        // 裁剪掉后缀标签
                        string prefixName = actorName.Substring(0, actorName.Length - ACTOR_TAG.Length);
                        if (prefixName.Length > 0)
                        {
                            info.ActorName = prefixName;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(info.ActorName))
            {
                Debugger.Warn(LogGroupTag.CodeLoader, "The actor '{0}' name must be non-null or empty space.", symClass.FullName);
                info.ActorName = symClass.ClassName;
            }

            if (_actorCodeInfos.ContainsKey(info.ActorName))
            {
                if (reload)
                {
                    _actorCodeInfos.Remove(info.ActorName);
                }
                else
                {
                    Debugger.Warn("The actor name '{0}' was already existed, repeat added it failed.", info.ActorName);
                    return false;
                }
            }

            _actorCodeInfos.Add(info.ActorName, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load 'CActor' code info '{0}' succeed from target class type '{1}'.", info.ToString(), symClass.FullName);

            return true;
        }

        private static void LoadActorMethod(Symboling.SymClass symClass, ActorCodeInfo codeInfo, Symboling.SymMethod symMethod)
        {
            // 静态函数直接忽略
            if (symMethod.IsStatic)
            {
                return;
            }

            IList<SystemAttribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                SystemAttribute attr = attrs[n];

                LoadEntityMethodByAttributeType(symClass, codeInfo, symMethod, attr);
            }
        }

        [OnProtoClassCleanupOfTarget(typeof(CActor))]
        private static void CleanupAllActorClasses()
        {
            _actorCodeInfos.Clear();
        }

        [OnProtoCodeInfoLookupOfTarget(typeof(CActor))]
        private static ActorCodeInfo LookupActorCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<string, ActorCodeInfo> pair in _actorCodeInfos)
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
