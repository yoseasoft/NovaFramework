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
        /// 配置类的参数存储容器
        /// </summary>
        private static IDictionary<string, string> _variables;

        /// <summary>
        /// 设置属性参数
        /// </summary>
        /// <param name="properties">属性映射集合</param>
        internal static void LoadProperties(IReadOnlyDictionary<string, string> properties)
        {
            if (null == properties)
            {
                Logger.Error("The input properties params must be non-null.");
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

            // SetValue(key, value);
        }

        /// <summary>
        /// 检测当前容器中是否存在指定键对应的属性值
        /// </summary>
        /// <param name="key">属性键</param>
        /// <returns>若存在对应属性值则返回true，否则返回false</returns>
        public static bool HasValue(string key)
        {
            if (_variables.ContainsKey(key))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 设置配置变量对应的键值对
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="value">变量值</param>
        public static void SetValue(string key, string value)
        {
            if (_variables.ContainsKey(key))
            {
                // Logger.Warn("The key '{%s}' was already exist in variables, repeat added it will be override old value.", key);

                _variables.Remove(key);
            }

            _variables.Add(key, value);
        }

        /// <summary>
        /// 获取指定键对应的配置变量值
        /// </summary>
        /// <param name="key">配置键</param>
        /// <returns>返回变量值</returns>
        public static string GetValue(string key)
        {
            if (_variables.TryGetValue(key, out string value))
            {
                return value;
            }

            return null;
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
                // 初始化参数容器
                _variables = new Dictionary<string, string>();

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
        private static IList<string> FindAssemblyNamesByTag(NovaFramework.LibraryTag tag)
        {
            return NovaFramework.DynamicLibrary.GetAllPlayableAssemblyNames((info) =>
            {
                if (false == info.IsContainsTag(tag))
                    return false;

                // 对教程模式进行过滤
                if (false == tutorialMode && info.IsContainsTag(NovaFramework.LibraryTag.Tutorial))
                    return false;

                return true;
            });
        }

        /// <summary>
        /// 获取配置库中可编译的程序集名称
        /// </summary>
        /// <returns>返回对应的程序集名称集合</returns>
        public static IList<string> GetAllCompilableAssemblyNames()
        {
            return FindAssemblyNamesByTag(NovaFramework.LibraryTag.Compile);
        }

        #endregion
    }
}
