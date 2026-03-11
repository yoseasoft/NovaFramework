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
using UnityEngine.Scripting;

namespace GameEngine
{
    /// 数据同步管理对象类
    internal sealed partial class ReplicateController
    {
        /// <summary>
        /// 数据同步标签映射缓存管理容器
        /// </summary>
        private NovaEngine.MultivalueDictionary<int, ReplicateBaseStructureInfo> _beanArranges;

        /// <summary>
        /// 标签映射缓存管理模块的初始化函数
        /// </summary>
        [Preserve]
        [OnControllerSubmoduleInitCallback]
        private void InitializeForBeanArrange()
        {
            _beanArranges = new NovaEngine.MultivalueDictionary<int, ReplicateBaseStructureInfo>();
        }

        /// <summary>
        /// 标签映射缓存管理模块的清理函数
        /// </summary>
        [Preserve]
        [OnControllerSubmoduleCleanupCallback]
        private void CleanupForBeanArrange()
        {
            RemoveAllReplicateObjectInfos();
        }

        /// <summary>
        /// 标签映射缓存管理模块的重载函数
        /// </summary>
        [Preserve]
        [OnControllerSubmoduleReloadCallback]
        private void ReloadForBeanArrange()
        {
        }

        #region 对象的数据同步相关内容的结构信息类型及接口函数定义

        /// <summary>
        /// 添加指定索引键对应的数据同步对象信息
        /// </summary>
        /// <param name="key">索引键</param>
        /// <param name="classType">对象类型</param>
        /// <returns>若数据信息添加成功则返回true，否则返回false</returns>
        private bool AddReplicateClassInfo(string key, Type classType)
        {
            Loader.Symbolling.SymClass symClass = Loader.CodeLoader.GetSymClassByType(classType);

            return AddReplicateClassInfo(key, symClass);
        }

        /// <summary>
        /// 添加指定索引键对应的数据同步对象信息
        /// </summary>
        /// <param name="key">索引键</param>
        /// <param name="symClass">对象符号类型</param>
        /// <returns>若数据信息添加成功则返回true，否则返回false</returns>
        private bool AddReplicateClassInfo(string key, Loader.Symbolling.SymClass symClass)
        {
            int keyId = key.GetHashCode();
            if (_beanArranges.ContainsKey(keyId))
            {
                Debugger.Warn(LogGroupTag.Controller, "The replicate key '{%s}' was already exist, repeat added it failed.", key);
                return false;
            }

            ReplicateClassStructureInfo classInfo = new ReplicateClassStructureInfo(symClass);
            _beanArranges.Add(keyId, classInfo);

            return true;
        }

        /// <summary>
        /// 添加指定索引键对应数据同步字段信息
        /// </summary>
        /// <param name="key">索引键</param>
        /// <param name="classType">对象类型</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns>若数据信息添加成功则返回true，否则返回false</returns>
        private bool AddReplicateFieldInfo(string key, Type classType, string fieldName)
        {
            Loader.Symbolling.SymClass symClass = Loader.CodeLoader.GetSymClassByType(classType);
            Loader.Symbolling.SymField symField = symClass.GetFieldByName(fieldName);

            return AddReplicateFieldInfo(key, symClass, symField);
        }

        /// <summary>
        /// 添加指定索引键对应的数据同步字段信息
        /// </summary>
        /// <param name="key">索引键</param>
        /// <param name="symClass">对象符号类型</param>
        /// <param name="symField">字段符号类型</param>
        /// <returns>若数据信息添加成功则返回true，否则返回false</returns>
        private bool AddReplicateFieldInfo(string key, Loader.Symbolling.SymClass symClass, Loader.Symbolling.SymField symField)
        {
            int keyId = key.GetHashCode();
            if (_beanArranges.ContainsKey(keyId))
            {
                Debugger.Warn(LogGroupTag.Controller, "The replicate key '{%s}' was already exist, repeat added it failed.", key);
                return false;
            }

            ReplicateFieldStructureInfo fieldInfo = new ReplicateFieldStructureInfo(symClass, symField);
            _beanArranges.Add(keyId, fieldInfo);

            return true;
        }

        /// <summary>
        /// 移除指定对象类型在容器中对应的数据同步对象信息
        /// </summary>
        /// <param name="classType">对象类型</param>
        private void RemoveReplicateObjectInfo(Type classType)
        {
            foreach (KeyValuePair<int, NovaEngine.DoubleLinkedList<ReplicateBaseStructureInfo>> kvp in _beanArranges)
            {
                foreach (ReplicateBaseStructureInfo info in kvp.Value)
                {
                    if (info.ObjectType == classType)
                    {
                        _beanArranges.Remove(kvp.Key, info);
                    }
                }

                if (kvp.Value.Count == 0)
                {
                    _beanArranges.RemoveAll(kvp.Key);
                }
            }
        }

        /// <summary>
        /// 移除所有数据同步对象信息
        /// </summary>
        private void RemoveAllReplicateObjectInfos()
        {
            _beanArranges.Clear();
        }

        /// <summary>
        /// 数据同步目标类型枚举定义
        /// </summary>
        private enum ReplicateTargetType
        {
            Unknown = 0,
            Class = 1,
            Field = 11,
        }

        /// <summary>
        /// 数据同步基础结构信息封装对象类
        /// </summary>
        private abstract class ReplicateBaseStructureInfo
        {
            /// <summary>
            /// 同步目标类型
            /// </summary>
            private readonly ReplicateTargetType _repType;
            /// <summary>
            /// 对象编码
            /// </summary>
            private readonly int _objectId;
            /// <summary>
            /// 对象类型
            /// </summary>
            private readonly Type _objectType;
            /// <summary>
            /// 对象名称
            /// </summary>
            private readonly string _objectName;

            public ReplicateTargetType ReplicateTargetType => _repType;
            public int ObjectId => _objectId;
            public Type ObjectType => _objectType;
            public string ObjectName => _objectName;

            protected ReplicateBaseStructureInfo(ReplicateTargetType repType, Loader.Symbolling.SymClass symClass)
            {
                _repType = repType;
                _objectId = symClass.Uid;
                _objectType = symClass.ClassType;
                _objectName = symClass.ClassName;
            }
        }

        /// <summary>
        /// 数据同步类结构信息封装对象类
        /// </summary>
        private sealed class ReplicateClassStructureInfo : ReplicateBaseStructureInfo
        {
            /// <summary>
            /// 对象的符号实例
            /// </summary>
            private readonly Loader.Symbolling.SymClass _class;

            public ReplicateClassStructureInfo(Loader.Symbolling.SymClass symClass) : base(ReplicateTargetType.Class, symClass)
            {
                _class = symClass;
            }
        }

        /// <summary>
        /// 数据同步字段结构信息封装对象类
        /// </summary>
        private sealed class ReplicateFieldStructureInfo : ReplicateBaseStructureInfo
        {
            /// <summary>
            /// 父对象符号实例
            /// </summary>
            private readonly Loader.Symbolling.SymClass _class;

            /// <summary>
            /// 字段符号实例
            /// </summary>
            private readonly Loader.Symbolling.SymField _field;

            public ReplicateFieldStructureInfo(Loader.Symbolling.SymClass symClass, Loader.Symbolling.SymField symField)
                : base(ReplicateTargetType.Field, symClass)
            {
                _class = symClass;
                _field = symField;
            }
        }

        #endregion
    }
}
