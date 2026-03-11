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

namespace GameEngine.Loader.Structuring
{
    /// <summary>
    /// 数据同步模块的编码结构信息对象类
    /// </summary>
    internal abstract class ReplicateCodeInfo : GeneralCodeInfo
    { }

    /// <summary>
    /// 实体对象同步模块的编码结构信息
    /// </summary>
    internal sealed class ReplicateBeanCodeInfo : ReplicateCodeInfo
    {
        /// <summary>
        /// 实体对象成员数据同步结构的列表容器
        /// </summary>
        private IList<ReplicateBeanMemberCodeInfo> _members;

        /// <summary>
        /// 符号对象类型的唯一标识
        /// </summary>
        public int SymbolId { get; internal set; }
        /// <summary>
        /// 实体对象的数据标签
        /// </summary>
        public string DataLabel { get; internal set; }

        /// <summary>
        /// 获取当前成员数据同步结构信息容器
        /// </summary>
        public IList<ReplicateBeanMemberCodeInfo> Members => _members;

        /// <summary>
        /// 新增指定成员的数据同步相关的结构信息
        /// </summary>
        /// <param name="codeInfo">成员的结构信息</param>
        public void AddMember(ReplicateBeanMemberCodeInfo codeInfo)
        {
            if (null == _members)
            {
                _members = new List<ReplicateBeanMemberCodeInfo>();
            }

            _members.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有成员的数据同步相关的结构信息
        /// </summary>
        public void RemoveAllMembers()
        {
            _members?.Clear();
            _members = null;
        }

        /// <summary>
        /// 获取当前成员数据同步的结构信息数量
        /// </summary>
        /// <returns>返回成员数据同步的结构信息数量</returns>
        public int GetMemberCount()
        {
            return _members?.Count ?? 0;
        }

        /// <summary>
        /// 获取当前成员数据同步的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public ReplicateBeanMemberCodeInfo GetMember(int index)
        {
            if (null == _members || index < 0 || index >= _members.Count)
            {
                Debugger.Warn(LogGroupTag.CodeLoader, "当前传入的索引值‘{%d}’超出了目标成员类型数据列表容器的可读范围，访问列表元素失败！", index);
                return null;
            }

            return _members[index];
        }
    }

    /// <summary>
    /// 实体对象同步模块成员相关的编码结构信息
    /// </summary>
    internal sealed class ReplicateBeanMemberCodeInfo
    {
        /// <summary>
        /// 符号对象成员的唯一标识
        /// </summary>
        public int SymbolId { get; internal set; }
        /// <summary>
        /// 需要数据同步的成员名称
        /// </summary>
        public string MemberName { get; internal set; }
        /// <summary>
        /// 成员是否为属性的状态标识
        /// </summary>
        public bool IsProperty { get; internal set; }
        /// <summary>
        /// 成员对应的数据标签
        /// </summary>
        public string DataLabel { get; internal set; }
    }

    /// <summary>
    /// 同步播报模块的编码结构信息对象类
    /// </summary>
    internal sealed class ReplicateCallCodeInfo : ReplicateCodeInfo
    {
        /// <summary>
        /// 同步调用模块的数据引用对象
        /// </summary>
        private MethodTypeList<ReplicateCallMethodTypeCodeInfo> _methodTypes;

        internal MethodTypeList<ReplicateCallMethodTypeCodeInfo> MethodTypes => _methodTypes;

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        public void AddMethodType(ReplicateCallMethodTypeCodeInfo codeInfo)
        {
            if (null == _methodTypes)
            {
                _methodTypes = new MethodTypeList<ReplicateCallMethodTypeCodeInfo>();
            }

            _methodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有函数的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllMethodTypes()
        {
            _methodTypes?.Clear();
            _methodTypes = null;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        public int GetMethodTypeCount()
        {
            return _methodTypes?.Count() ?? 0;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public ReplicateCallMethodTypeCodeInfo GetMethodType(int index)
        {
            return _methodTypes?.Get(index);
        }
    }

    /// <summary>
    /// 同步调用模块的函数结构信息
    /// </summary>
    internal class ReplicateCallMethodTypeCodeInfo : MethodTypeCodeInfo
    {
        /// <summary>
        /// 同步调用模块的数据标签
        /// </summary>
        public string Tags { get; internal set; }
        /// <summary>
        /// 同步调用模块的数据播报方式
        /// </summary>
        public ReplicateAnnounceType AnnounceType { get; internal set; }
    }

    /// <summary>
    /// 切面管理的播报同步函数结构信息
    /// </summary>
    internal sealed class ReplicateAnnouncingMethodTypeCodeInfo : ReplicateCallMethodTypeCodeInfo
    {
        /// <summary>
        /// 播报绑定的观察行为类型
        /// </summary>
        public AspectBehaviourType BehaviourType { get; internal set; }
    }
}
