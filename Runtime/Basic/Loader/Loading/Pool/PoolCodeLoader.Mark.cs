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
    /// 对象池类的结构信息
    /// </summary>
    public class PoolMarkCodeInfo : GeneralCodeInfo
    {
        /// <summary>
        /// 对象池类的类型标识
        /// </summary>
        protected SystemType _classType;

        public SystemType ClassType { get { return _classType; } internal set { _classType = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("PoolMark = { ");
            sb.AppendFormat("Class = {0}, ", _classType.FullName);
            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 对象池容器管理对象的分析处理类，对业务层载入的所有对象池支持类进行统一加载及分析处理
    /// </summary>
    internal static partial class PoolCodeLoader
    {
        /// <summary>
        /// 对象池管理类的结构信息管理容器
        /// </summary>
        private static IDictionary<SystemType, PoolMarkCodeInfo> _poolMarkCodeInfos = new Dictionary<SystemType, PoolMarkCodeInfo>();

        [OnPoolClassLoadOfTarget(typeof(PoolSupportedAttribute))]
        private static bool LoadPoolMarkClass(Symboling.SymClass symClass, bool reload)
        {
            PoolMarkCodeInfo info = new PoolMarkCodeInfo();
            info.ClassType = symClass.ClassType;

            if (false == symClass.IsInstantiate)
            {
                Debugger.Warn("The pool supported class '{0}' must be was instantiable, newly added it failed.", info.ClassType.FullName);
                return false;
            }

            if (_poolMarkCodeInfos.ContainsKey(symClass.ClassType))
            {
                if (reload)
                {
                    // 重载模式下，先移除旧的记录
                    _poolMarkCodeInfos.Remove(symClass.ClassType);
                }
                else
                {
                    Debugger.Warn("The pool mark type '{0}' was already existed, repeat added it failed.", symClass.FullName);
                    return false;
                }
            }

            _poolMarkCodeInfos.Add(symClass.ClassType, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load pool mark code info '{0}' succeed from target class type '{1}'.", info.ToString(), symClass.FullName);

            return true;
        }

        [OnPoolClassCleanupOfTarget(typeof(PoolSupportedAttribute))]
        private static void CleanupAllPoolMarkClasses()
        {
            _poolMarkCodeInfos.Clear();
        }

        [OnPoolCodeInfoLookupOfTarget(typeof(PoolSupportedAttribute))]
        private static PoolMarkCodeInfo LookupPoolMarkCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<SystemType, PoolMarkCodeInfo> pair in _poolMarkCodeInfos)
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
