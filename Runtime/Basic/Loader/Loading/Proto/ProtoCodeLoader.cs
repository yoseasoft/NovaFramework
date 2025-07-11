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
using System.Reflection;

using SystemType = System.Type;
using SystemDelegate = System.Delegate;
using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemBindingFlags = System.Reflection.BindingFlags;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader
{
    /// <summary>
    /// 原型类的结构信息
    /// </summary>
    public abstract class ProtoCodeInfo : GeneralCodeInfo
    {
        /// <summary>
        /// 原型类的类型标识
        /// </summary>
        protected SystemType _classType;

        public SystemType ClassType { get { return _classType; } set { _classType = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("Class = {0}, ", _classType.FullName);
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
        /// 加载原型类相关回调函数的管理容器
        /// </summary>
        private static IDictionary<SystemType, SystemDelegate> _protoClassLoadCallbacks = new Dictionary<SystemType, SystemDelegate>();
        /// <summary>
        /// 清理原型类相关回调函数的管理容器
        /// </summary>
        private static IDictionary<SystemType, SystemDelegate> _protoClassCleanupCallbacks = new Dictionary<SystemType, SystemDelegate>();
        /// <summary>
        /// 查找原型类结构信息相关回调函数的管理容器
        /// </summary>
        private static IDictionary<SystemType, SystemDelegate> _protoCodeInfoLookupCallbacks = new Dictionary<SystemType, SystemDelegate>();

        /// <summary>
        /// 加载原型类相关函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private class OnProtoClassLoadOfTargetAttribute : OnCodeLoaderClassLoadOfTargetAttribute
        {
            public OnProtoClassLoadOfTargetAttribute(SystemType classType) : base(classType) { }
        }

        /// <summary>
        /// 清理原型类相关函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private class OnProtoClassCleanupOfTargetAttribute : OnCodeLoaderClassCleanupOfTargetAttribute
        {
            public OnProtoClassCleanupOfTargetAttribute(SystemType classType) : base(classType) { }
        }

        /// <summary>
        /// 查找原型类对应结构信息相关函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private class OnProtoCodeInfoLookupOfTargetAttribute : OnCodeLoaderClassLookupOfTargetAttribute
        {
            public OnProtoCodeInfoLookupOfTargetAttribute(SystemType classType) : base(classType) { }
        }

        /// <summary>
        /// 初始化针对所有原型类声明的全部绑定回调接口
        /// </summary>
        [CodeLoader.OnGeneralCodeLoaderInit]
        private static void InitAllProtoClassLoadingCallbacks()
        {
            SystemType classType = typeof(ProtoCodeLoader);
            SystemMethodInfo[] methods = classType.GetMethods(SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Static);
            for (int n = 0; n < methods.Length; ++n)
            {
                SystemMethodInfo method = methods[n];
                IEnumerable<SystemAttribute> e = method.GetCustomAttributes();
                foreach (SystemAttribute attr in e)
                {
                    SystemType attrType = attr.GetType();
                    if (typeof(OnProtoClassLoadOfTargetAttribute) == attrType)
                    {
                        OnProtoClassLoadOfTargetAttribute _attr = (OnProtoClassLoadOfTargetAttribute) attr;

                        Debugger.Assert(!_protoClassLoadCallbacks.ContainsKey(_attr.ClassType), "Invalid proto class load type");
                        _protoClassLoadCallbacks.Add(_attr.ClassType, method.CreateDelegate(typeof(CodeLoader.OnGeneralCodeLoaderLoadHandler)));
                    }
                    else if (typeof(OnProtoClassCleanupOfTargetAttribute) == attrType)
                    {
                        OnProtoClassCleanupOfTargetAttribute _attr = (OnProtoClassCleanupOfTargetAttribute) attr;

                        Debugger.Assert(!_protoClassCleanupCallbacks.ContainsKey(_attr.ClassType), "Invalid proto class cleanup type");
                        _protoClassCleanupCallbacks.Add(_attr.ClassType, method.CreateDelegate(typeof(CodeLoader.OnCleanupAllGeneralCodeLoaderHandler)));
                    }
                    else if (typeof(OnProtoCodeInfoLookupOfTargetAttribute) == attrType)
                    {
                        OnProtoCodeInfoLookupOfTargetAttribute _attr = (OnProtoCodeInfoLookupOfTargetAttribute) attr;

                        Debugger.Assert(!_protoCodeInfoLookupCallbacks.ContainsKey(_attr.ClassType), "Invalid proto class lookup type");
                        _protoCodeInfoLookupCallbacks.Add(_attr.ClassType, method.CreateDelegate(typeof(CodeLoader.OnGeneralCodeLoaderLookupHandler)));
                    }
                }
            }
        }

        /// <summary>
        /// 清理针对所有原型类声明的全部绑定回调接口
        /// </summary>
        [CodeLoader.OnGeneralCodeLoaderCleanup]
        private static void CleanupAllProtoClassLoadingCallbacks()
        {
            IEnumerator<KeyValuePair<SystemType, SystemDelegate>> e = _protoClassCleanupCallbacks.GetEnumerator();
            while (e.MoveNext())
            {
                CodeLoader.OnCleanupAllGeneralCodeLoaderHandler handler = e.Current.Value as CodeLoader.OnCleanupAllGeneralCodeLoaderHandler;
                Debugger.Assert(null != handler, "Invalid proto class cleanup handler.");

                handler.Invoke();
            }

            _protoClassLoadCallbacks.Clear();
            _protoClassCleanupCallbacks.Clear();
            _protoCodeInfoLookupCallbacks.Clear();
        }

        /// <summary>
        /// 检测原型类指定的类型是否匹配当前加载器
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="filterType">过滤对象类型</param>
        /// <returns>若给定类型满足匹配规则则返回true，否则返回false</returns>
        [CodeLoader.OnGeneralCodeLoaderMatch]
        private static bool IsProtoClassMatched(Symboling.SymClass symClass, SystemType filterType)
        {
            // 存在过滤类型，则直接对比过滤类型即可
            if (null != filterType)
            {
                // 直接将目标类型替换为过滤类型进行检测即可
                return typeof(IProto).IsAssignableFrom(filterType);
            }

            // 原型类必须为可实例化的类
            if (false == symClass.IsInstantiate)
            {
                return false;
            }

            if (typeof(IProto).IsAssignableFrom(symClass.ClassType))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 加载原型类指定的类型
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="reload">重载状态标识</param>
        /// <returns>若存在给定类型属性原型类则返回对应处理结果，否则返回false</returns>
        [CodeLoader.OnGeneralCodeLoaderLoad]
        private static bool LoadProtoClass(Symboling.SymClass symClass, bool reload)
        {
            SystemDelegate callback = null;

            if (TryGetProtoClassCallbackForTargetContainer(symClass.ClassType, out callback, _protoClassLoadCallbacks))
            {
                CodeLoader.OnGeneralCodeLoaderLoadHandler handler = callback as CodeLoader.OnGeneralCodeLoaderLoadHandler;
                Debugger.Assert(null != handler, "Invalid proto class load handler.");
                return handler.Invoke(symClass, reload);
            }

            return false;
        }

        /// <summary>
        /// 查找原型类指定的类型对应的结构信息
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <returns>返回类型对应的结构信息</returns>
        [CodeLoader.OnGeneralCodeLoaderLookup]
        private static GeneralCodeInfo LookupProtoCodeInfo(Symboling.SymClass symClass)
        {
            SystemDelegate callback = null;

            if (TryGetProtoClassCallbackForTargetContainer(symClass.ClassType, out callback, _protoCodeInfoLookupCallbacks))
            {
                CodeLoader.OnGeneralCodeLoaderLookupHandler handler = callback as CodeLoader.OnGeneralCodeLoaderLookupHandler;
                Debugger.Assert(null != handler, "Invalid proto class lookup handler.");
                return handler.Invoke(symClass);
            }

            return null;
        }

        /// <summary>
        /// 通过指定的类型从原型类的回调管理容器中查找对应的回调句柄实例
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="callback">回调句柄</param>
        /// <param name="container">句柄列表容器</param>
        /// <returns>返回通过给定类型查找的回调句柄实例，若不存在则返回null</returns>
        private static bool TryGetProtoClassCallbackForTargetContainer(SystemType targetType, out SystemDelegate callback, IDictionary<SystemType, SystemDelegate> container)
        {
            callback = null;

            IEnumerator<KeyValuePair<SystemType, SystemDelegate>> e = container.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Key.IsAssignableFrom(targetType) && e.Current.Key != targetType)
                {
                    callback = e.Current.Value;
                    return true;
                }
            }

            return false;
        }
    }
}
