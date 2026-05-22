/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NovaEngine
{
    /// <summary>
    /// 基础配置参数定义类，对当前引擎运行所需的初始化参数进行设置及管理
    /// </summary>
    public static partial class Configuration
    {
        /// <summary>
        /// 配置类的所有字段映射集合
        /// </summary>
        private static IDictionary<string, FieldInfo> _configureFields;

        /// <summary>
        /// 设置属性参数
        /// </summary>
        /// <param name="properties">属性映射集合</param>
        internal static void LoadProperties(IReadOnlyDictionary<string, string> properties)
        {
            if (null == properties)
            {
                CLogger.Error("The input properties params must be non-null.");
                return;
            }

            foreach (KeyValuePair<string, string> pair in properties)
            {
                SetProperty(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// 设置属性参数
        /// </summary>
        /// <param name="key">属性键</param>
        /// <param name="value">属性值</param>
        internal static void SetProperty(string key, string value)
        {
            // 初始化配置管理所需容器
            InitConfigureFieldCollection();

            // 基础属性类型
            if (_configureFields.TryGetValue(key, out FieldInfo field))
            {
                field.SetValue(null, Utility.Convertion.StringToTargetType(value, field.FieldType));

                return;
            }
        }

        /// <summary>
        /// 初始化配置类中的所有字段成员集合
        /// </summary>
        private static void InitConfigureFieldCollection()
        {
            if (null == _configureFields)
            {
                // 初始化字段容器
                _configureFields = new Dictionary<string, FieldInfo>();

                Type classType = typeof(Configuration);
                FieldInfo[] fields = classType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                for (int n = 0; n < fields.Length; ++n)
                {
                    FieldInfo field = fields[n];

                    if (field.IsStatic && field.IsInitOnly)
                    {
                        // 只读模式的静态字段成员
                        _configureFields.Add(field.Name, field);
                    }
                }
            }
        }

        #region 获取配置库相关数据对应的接口函数

        /// <summary>
        /// 获取配置库中指定标签对应的所有可用程序集名称
        /// </summary>
        /// <param name="tag">功能标签</param>
        /// <returns>返回给定标签对应的程序集名称集合</returns>
        private static IReadOnlyList<string> FindAssemblyNamesByTag(NovaFramework.ExtensionModuleTag tag)
        {
            return NovaFramework.ExtensionModuleService.Instance.GetAllPlayableAssemblyNames((info) =>
            {
                if (false == info.IsContainsTag(tag))
                    return false;

                // 对教程模式进行过滤
                if (false == TutorialMode && info.IsContainsTag(NovaFramework.ExtensionModuleTag.Tutorial))
                    return false;

                return true;
            });
        }

        /// <summary>
        /// 获取配置库中所有编译模组的程序集名称
        /// </summary>
        /// <returns>返回对应的程序集名称集合</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadOnlyList<string> GetAllCompileAssemblyNames()
        {
            return FindAssemblyNamesByTag(NovaFramework.ExtensionModuleTag.Compile);
        }

        /// <summary>
        /// 获取配置库中所有重载模组的程序集名称
        /// </summary>
        /// <returns>返回对应的程序集名称集合</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadOnlyList<string> GetAllHotfixAssemblyNames()
        {
            return FindAssemblyNamesByTag(NovaFramework.ExtensionModuleTag.Hotfix);
        }

        /// <summary>
        /// 获取配置库中所有热插模组的程序集名称
        /// </summary>
        /// <returns>返回对应的程序集名称集合</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadOnlyList<string> GetAllHotloadAssemblyNames()
        {
            return FindAssemblyNamesByTag(NovaFramework.ExtensionModuleTag.Hotload);
        }

        #endregion
    }
}
