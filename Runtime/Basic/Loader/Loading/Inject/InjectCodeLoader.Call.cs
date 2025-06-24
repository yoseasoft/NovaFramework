/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
    /// 注入调用类的结构信息
    /// </summary>
    public class InjectCallCodeInfo : InjectCodeInfo
    {
        /// <summary>
        /// 调用对象的行为类型
        /// </summary>
        private AspectBehaviourType m_behaviourType;

        public AspectBehaviourType BehaviourType { get { return m_behaviourType; } internal set { m_behaviourType = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("InjectCall = { ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());
            sb.AppendFormat("BehaviourType = {0}, ", m_behaviourType);
            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 程序集中注入控制对象的分析处理类，对业务层载入的所有注入控制类进行统一加载及分析处理
    /// </summary>
    internal static partial class InjectCodeLoader
    {
        /// <summary>
        /// 注入实体类的结构信息管理容器
        /// </summary>
        private static IDictionary<SystemType, InjectCallCodeInfo> s_injectCallCodeInfos = new Dictionary<SystemType, InjectCallCodeInfo>();

        [OnInjectClassLoadOfTarget(typeof(InjectAttribute))]
        private static bool LoadInjectCallClass(Symboling.SymClass symClass, bool reload)
        {
            if (false == symClass.IsInstantiate)
            {
                Debugger.Error("The inject call class '{0}' must be instantiable class, loaded it failed.", symClass.FullName);
                return false;
            }

            InjectCallCodeInfo info = new InjectCallCodeInfo();
            info.ClassType = symClass.ClassType;
            info.BehaviourType = AspectBehaviourType.Initialize;

            if (s_injectCallCodeInfos.ContainsKey(symClass.ClassType))
            {
                if (reload)
                {
                    s_injectCallCodeInfos.Remove(symClass.ClassType);
                }
                else
                {
                    Debugger.Warn("The inject call '{0}' was already existed, repeat added it failed.", symClass.FullName);
                    return false;
                }
            }

            s_injectCallCodeInfos.Add(symClass.ClassType, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load inject call code info '{0}' succeed from target class type '{1}'.", info.ToString(), symClass.FullName);

            return true;
        }

        [OnInjectClassCleanupOfTarget(typeof(InjectAttribute))]
        private static void CleanupAllInjectCallClasses()
        {
            s_injectCallCodeInfos.Clear();
        }

        [OnInjectCodeInfoLookupOfTarget(typeof(InjectAttribute))]
        private static InjectCallCodeInfo LookupInjectCallCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<SystemType, InjectCallCodeInfo> pair in s_injectCallCodeInfos)
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
