/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace GameEngine.Loader
{
    /// 数据同步管理对象的分析处理类
    internal static partial class ReplicateCodeLoader
    {
        /// <summary>
        /// 数据同步管理类的结构信息管理容器
        /// </summary>
        private static readonly IDictionary<Type, Structuring.ReplicateBeanCodeInfo> _replicateBeanCodeInfos = new Dictionary<Type, Structuring.ReplicateBeanCodeInfo>();

        [Preserve]
        [OnCodeLoaderClassLoadOfTarget(typeof(ReplicateConfigureAttribute))]
        private static bool LoadReplicateBeanClass(Symbolling.SymClass symClass, bool reload)
        {
            Structuring.ReplicateBeanCodeInfo info = new Structuring.ReplicateBeanCodeInfo();
            info.ClassType = symClass.ClassType;

            if (false == symClass.IsInstantiate)
            {
                Debugger.Warn(LogGroupTag.CodeLoader, "The replicate supported class '{%t}' must be was instantiable, newly added it failed.", info.ClassType);
                return false;
            }

            IList<Attribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                Attribute attr = attrs[n];
                if (attr is CReplicateObjectAttribute replicateClassAttribute)
                {
                    // 若未配置名称，则默认使用类名小写作为标签
                    info.DataLabel = replicateClassAttribute.Tag ?? symClass.ClassName.ToLower();
                }
            }

            // 允许类作为附属的存在，没有单独的标签，只有数据成员
            // if (string.IsNullOrEmpty(info.DataLabel))
            // {
            //     // Debugger.Warn(LogGroupTag.CodeLoader, "The bean replicate tag '{%s}' must be non-null or empty space.", symClass.FullName);
            //     info.DataLabel = symClass.ClassName.ToLower();
            // }

            IList<Symbolling.SymField> fields = symClass.GetAllFields();
            for (int n = 0; null != fields && n < fields.Count; ++n)
            {
                Symbolling.SymField field = fields[n];

                CReplicateFieldAttribute replicateFieldAttribute = field.GetAttribute<CReplicateFieldAttribute>(true);
                if (null != replicateFieldAttribute)
                {
                    info.AddMember(new Structuring.ReplicateBeanMemberCodeInfo()
                    {
                        SymbolId = field.Uid,
                        MemberName = field.FieldName,
                        IsProperty = false,
                        // 若未配置名称，则默认使用字段名小写作为标签
                        DataLabel = replicateFieldAttribute.Tag ?? field.FieldName.ToLower(),
                    });
                }
            }

            IList<Symbolling.SymProperty> properties = symClass.GetAllProperties();
            for (int n = 0; null != properties && n < properties.Count; ++n)
            {
                Symbolling.SymProperty property = properties[n];

                CReplicatePropertyAttribute replicatePropertyAttribute = property.GetAttribute<CReplicatePropertyAttribute>(true);
                if (null != replicatePropertyAttribute)
                {
                    info.AddMember(new Structuring.ReplicateBeanMemberCodeInfo()
                    {
                        SymbolId = property.Uid,
                        MemberName = property.PropertyName,
                        IsProperty = true,
                        // 若未配置名称，则默认使用属性名小写作为标签
                        DataLabel = replicatePropertyAttribute.Tag ?? property.PropertyName.ToLower(),
                    });
                }
            }

            if (_replicateBeanCodeInfos.ContainsKey(symClass.ClassType))
            {
                if (reload)
                {
                    // 重载模式下，先移除旧的记录
                    _replicateBeanCodeInfos.Remove(symClass.ClassType);
                }
                else
                {
                    Debugger.Warn(LogGroupTag.CodeLoader, "The replicate bean type '{%s}' was already existed, repeat added it failed.", symClass.FullName);
                    return false;
                }
            }

            _replicateBeanCodeInfos.Add(symClass.ClassType, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load replicate bean code info '{%s}' succeed from target class type '{%s}'.", CodeLoaderUtils.ToString(info), symClass.FullName);

            return true;
        }

        [Preserve]
        [OnCodeLoaderClassCleanupOfTarget(typeof(ReplicateConfigureAttribute))]
        private static void CleanupAllPoolCallClasses()
        {
            _replicateBeanCodeInfos.Clear();
        }

        [Preserve]
        [OnCodeLoaderClassLookupOfTarget(typeof(ReplicateConfigureAttribute))]
        private static Structuring.ReplicateBeanCodeInfo LookupPoolCallCodeInfo(Symbolling.SymClass symClass)
        {
            if (_replicateBeanCodeInfos.TryGetValue(symClass.ClassType, out Structuring.ReplicateBeanCodeInfo codeInfo))
            {
                return codeInfo;
            }

            return null;
        }
    }
}
