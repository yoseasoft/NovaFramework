/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace GameEngine
{
    /// 数据同步管理对象类
    internal sealed partial class ReplicateController
    {
        /// <summary>
        /// 实体对象类型与标签的参照表
        /// </summary>
        private IDictionary<Type, string> _beanTypeAndLabelReferenceTables;
        /// <summary>
        /// 实体对象成员名称与标签的参照表
        /// </summary>
        private IDictionary<Type, IDictionary<string, string>> _beanMemberNameAndLabelReferenceTables;

        /// <summary>
        /// 实体成员映射缓存管理模块的初始化函数
        /// </summary>
        [Preserve]
        [OnControllerSubmoduleInitCallback]
        private void InitializeForBeanMember()
        {
            // 参照容器初始化
            _beanTypeAndLabelReferenceTables = new Dictionary<Type, string>();
            _beanMemberNameAndLabelReferenceTables = new Dictionary<Type, IDictionary<string, string>>();
        }

        /// <summary>
        /// 实体成员映射缓存管理模块的清理函数
        /// </summary>
        [Preserve]
        [OnControllerSubmoduleCleanupCallback]
        private void CleanupForBeanMember()
        {
            // 参照容器清理
            UnregisterAllReplicateInfos();

            _beanTypeAndLabelReferenceTables = null;
            _beanMemberNameAndLabelReferenceTables = null;
        }

        /// <summary>
        /// 注册实体对象类型对应的数据标签
        /// </summary>
        /// <param name="classType">对象类型</param>
        /// <param name="tag">数据标签</param>
        private void RegisterReplicateObject(Type classType, string tag)
        {
            if (_beanTypeAndLabelReferenceTables.ContainsKey(classType))
            {
                Debugger.Warn(LogGroupTag.Controller, "The bean type '{%t}' was already exist with target tag '{%s}', repeat registered it will be override the old value.",
                    classType, _beanTypeAndLabelReferenceTables[classType]);
                _beanTypeAndLabelReferenceTables.Remove(classType);
            }

            if (_beanMemberNameAndLabelReferenceTables.ContainsKey(classType))
            {
                Debugger.Warn(LogGroupTag.Controller, "The bean type '{%t}' was already added the member tables, repeat added it will be override the old value.", classType);
                _beanMemberNameAndLabelReferenceTables.Remove(classType);
            }

            _beanTypeAndLabelReferenceTables.Add(classType, tag);
            _beanMemberNameAndLabelReferenceTables.Add(classType, new Dictionary<string, string>());
        }

        /// <summary>
        /// 注册实体对象成员对应的数据标签
        /// </summary>
        /// <param name="classType">对象类型</param>
        /// <param name="memberName">成员名称</param>
        /// <param name="tag">数据标签</param>
        private void RegisterReplicateMember(Type classType, string memberName, string tag)
        {
            if (false == _beanMemberNameAndLabelReferenceTables.TryGetValue(classType, out IDictionary<string, string> referenceTables))
            {
                referenceTables = new Dictionary<string, string>();
                _beanMemberNameAndLabelReferenceTables.Add(classType, referenceTables);
            }

            if (referenceTables.ContainsKey(memberName))
            {
                Debugger.Warn(LogGroupTag.Controller, "The member name '{%s}' was already exist with bean type '{%t}', repeat added it will be override the old value.", memberName, classType);
                referenceTables.Remove(memberName);
            }

            referenceTables.Add(memberName, tag);
        }

        /// <summary>
        /// 注销指定实体对象类型登记的数据标签信息
        /// </summary>
        /// <param name="classType">对象类型</param>
        private void UnregisterReplicateObject(Type classType)
        {
            if (_beanTypeAndLabelReferenceTables.ContainsKey(classType))
            {
                _beanTypeAndLabelReferenceTables.Remove(classType);
            }

            if (_beanMemberNameAndLabelReferenceTables.ContainsKey(classType))
            {
                _beanMemberNameAndLabelReferenceTables.Remove(classType);
            }
        }

        /// <summary>
        /// 注销当前登记的所有实体对象数据标签信息
        /// </summary>
        private void UnregisterAllReplicateInfos()
        {
            _beanTypeAndLabelReferenceTables.Clear();
            _beanMemberNameAndLabelReferenceTables.Clear();
        }

        /// <summary>
        /// 通过指定的实体对象类型和成员名称，检索与其对应的数据标签
        /// </summary>
        /// <param name="beanType">实体对象类型</param>
        /// <param name="memberName">成员名称</param>
        /// <returns>返回检索到的数据标签</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string RetrievingCompleteReplicateTags(Type beanType, string memberName)
        {
            return RetrievingCompleteReplicateTags(beanType, null, memberName);
        }

        /// <summary>
        /// 通过指定的实体对象类型和成员名称，检索与其对应的数据标签
        /// </summary>
        /// <param name="beanType">实体对象类型</param>
        /// <param name="moduleType">模块对象类型</param>
        /// <param name="memberName">成员名称</param>
        /// <returns>返回检索到的数据标签</returns>
        public string RetrievingCompleteReplicateTags(Type beanType, Type moduleType, string memberName)
        {
            if (false == TryGetBeanLabel(beanType, moduleType, out string beanTag))
            {
                Debugger.Warn(LogGroupTag.Controller, "Could not found any bean replicate tag with target bean type '{%t}' and module type '{%t}'.", beanType, moduleType);
                return null;
            }

            // 如果指定的模块对象类型，则成员需要从模块对象中获取
            Type targetType = moduleType ?? beanType;
            if (false == TryGetMemberLabel(targetType, memberName, out string memberTag))
            {
                Debugger.Warn(LogGroupTag.Controller, "Could not found member tag with target bean type '{%t}' and member name '{%s}'.", targetType, memberName);
                return null;
            }

            return string.Concat(beanTag, NovaEngine.Definition.CString.Dot, memberTag);
        }

        /// <summary>
        /// 尝试通过指定实体对象及模块对象获取对应的数据标签
        /// </summary>
        /// <param name="beanType">实体对象类型</param>
        /// <param name="moduleType">模块对象类型</param>
        /// <param name="beanTag">数据标签</param>
        /// <returns>若获取数据标签成功则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetBeanLabel(Type beanType, Type moduleType, out string beanTag)
        {
            if (false == _beanTypeAndLabelReferenceTables.TryGetValue(beanType, out beanTag))
            {
                Debugger.Warn(LogGroupTag.Controller, "Could not found any bean replicate tag with target bean type '{%t}'.", beanType);
                return false;
            }

            if (null != moduleType && _beanTypeAndLabelReferenceTables.TryGetValue(moduleType, out string moduleTag))
            {
                beanTag = string.Concat(beanTag, NovaEngine.Definition.CString.Dot, moduleTag);
            }

            return true;
        }

        /// <summary>
        /// 尝试通过指定实体对象及指定成员获取对应的成员标签
        /// </summary>
        /// <param name="beanType">实体对象类型</param>
        /// <param name="memberName">成员名称</param>
        /// <param name="memberTag">成员标签</param>
        /// <returns>若获取成员标签成功则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetMemberLabel(Type beanType, string memberName, out string memberTag)
        {
            if (false == _beanMemberNameAndLabelReferenceTables.TryGetValue(beanType, out IDictionary<string, string> memberReferenceTables))
            {
                Debugger.Warn(LogGroupTag.Controller, "Could not found any member reference tables with target bean type '{%t}'.", beanType);
                memberTag = null;
                return false;
            }

            return memberReferenceTables.TryGetValue(memberName, out memberTag);
        }
    }
}
