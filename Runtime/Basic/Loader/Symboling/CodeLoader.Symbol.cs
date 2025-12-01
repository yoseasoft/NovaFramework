/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using System.Collections.Generic;

using SystemType = System.Type;

namespace GameEngine.Loader
{
    /// <summary>
    /// 程序集的分析处理类，对业务层载入的所有对象类进行统一加载及分析处理
    /// </summary>
    public static partial class CodeLoader
    {
        /// <summary>
        /// 对象类的标记信息管理容器
        /// </summary>
        private static Symboling.SymClassMap _symClassMaps = null;

        /// <summary>
        /// 对象类的Bean信息管理容器
        /// </summary>
        private static IDictionary<string, Symboling.Bean> _beanClassMaps = null;

        /// <summary>
        /// 初始化针对所有标记对象类声明的全部绑定回调接口
        /// </summary>
        [OnClassSubmoduleInitializeCallback]
        private static void InitAllSymClassLoadingCallbacks()
        {
            // 初始化标记数据容器
            _symClassMaps = new Symboling.SymClassMap();
            // 初始化Bean数据容器
            _beanClassMaps = new Dictionary<string, Symboling.Bean>();

            // 符号解析器初始化
            Symboling.SymClassResolver.OnInitialize();
        }

        /// <summary>
        /// 清理针对所有标记对象类声明的全部绑定回调接口
        /// </summary>
        [OnClassSubmoduleCleanupCallback]
        private static void CleanupAllSymClassLoadingCallbacks()
        {
            // 符号解析器清理
            Symboling.SymClassResolver.OnCleanup();

            // 清理标识数据容器
            UnloadAllSymClasses();

            _symClassMaps = null;
            _beanClassMaps = null;
        }

        /// <summary>
        /// 加载通用类库指定对象类型的标记信息
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="reload">重载状态标识</param>
        /// <returns>若存在给定类型属性通用类库则返回对应处理结果，否则返回false</returns>
        private static Symboling.SymClass LoadSymClass(SystemType targetType, bool reload)
        {
            Symboling.SymClass symbol = Symboling.SymClassResolver.ResolveSymClass(targetType, reload);
            if (null == symbol)
            {
                // 解析失败，这里直接返回
                return null;
            }

            if (reload)
            {
                if (_symClassMaps.ContainsKey(symbol.ClassName))
                {
                    // 先注销Bean信息
                    RemoveBeanObjectsOfTargetSymClassFromCache(symbol);
                    // 再移除标记对象
                    _symClassMaps.Remove(symbol.ClassName);
                }
                else
                {
                    // 在重载前已经卸载掉所有的类标记对象，所以此处必定查找不到目标对象
                    // Debugger.Warn("Could not found any class symbol with target name '{%s}' and type '{%t}', removed it failed.", symbol.TargetName, targetType);
                }
            }

            // 安全检查
            Debugger.Assert(false == _symClassMaps.ContainsKey(symbol.ClassName), $"Load class symbol {symbol.ClassName} error.");

            Debugger.Log(LogGroupTag.CodeLoader, "Load class symbol '{%s}' succeed from target class type '{%t}'.", CodeLoaderUtils.ToString(symbol), targetType);

            // 添加标记信息
            _symClassMaps.Add(symbol);

            // 添加Bean信息到缓存中
            AddBeanObjectFromSymClassToCache(symbol);

            return symbol;
        }

        /// <summary>
        /// 卸载所有对象类型的标记信息
        /// </summary>
        private static void UnloadAllSymClasses()
        {
            _symClassMaps.Clear();
            _beanClassMaps.Clear();
        }

        /// <summary>
        /// 重新绑定当前全部标记信息的Bean实例
        /// </summary>
        private static void RebindingBeanObjectsOfAllSymClasses()
        {
            // 先清理掉Bean对象的缓存
            _beanClassMaps.Clear();

            IList<Symboling.SymClass> symbols = _symClassMaps.Values;
            for (int n = 0; null != symbols && n < symbols.Count; ++n)
            {
                Symboling.SymClass symbol = symbols[n];

                Symboling.SymClassResolver.RebuildBeanObjectsWithConfigureFile(symbol);

                // 将新的Bean对象添加到缓存中
                AddBeanObjectFromSymClassToCache(symbol);
            }
        }

        /// <summary>
        /// 将符号对象中所有的Bean实例添加到缓存中
        /// </summary>
        /// <param name="symbol">符号对象实例</param>
        private static void AddBeanObjectFromSymClassToCache(Symboling.SymClass symbol)
        {
            // 添加Bean信息
            IEnumerator<KeyValuePair<string, Symboling.Bean>> beans = symbol.GetBeanEnumerator();
            if (null != beans)
            {
                while (beans.MoveNext())
                {
                    Symboling.Bean bean = beans.Current.Value;

                    AddBeanObjectToCache(bean);
                }
            }
        }

        /// <summary>
        /// 将标记类对应的实体对象配置信息添加到缓存中
        /// </summary>
        /// <param name="bean">实体实例</param>
        private static void AddBeanObjectToCache(Symboling.Bean bean)
        {
            string beanName = bean.BeanName;
            if (_beanClassMaps.ContainsKey(beanName))
            {
                Debugger.Warn("The bean object '{%s}' was already exist within class map, repeat added it failed.", beanName);
                return;
            }

            Debugger.Info(LogGroupTag.CodeLoader, "Register new bean object '{%s}' to target symbol class '{%t}'.", beanName, bean.TargetClass);

            _beanClassMaps.Add(beanName, bean);
        }

        /// <summary>
        /// 从缓存中移除与指定标识信息关联的所有Bean对象实例
        /// </summary>
        /// <param name="symbol">符号对象实例</param>
        private static void RemoveBeanObjectsOfTargetSymClassFromCache(Symboling.SymClass symbol)
        {
            ICollection<string> keys = _beanClassMaps.Keys;
            foreach (string k in keys)
            {
                if (false == _beanClassMaps.TryGetValue(k, out Symboling.Bean bean))
                {
                    Debugger.Warn("无法查找到与指定名字‘{%s}’对应的Bean对象实例！", k);
                    continue;
                }

                if (bean.TargetClass == symbol)
                {
                    // 移除指定标记对应的所有Bean实例
                    _beanClassMaps.Remove(k);
                }
            }
        }

        #region 符号对象外部解析器注册绑定相关的接口函数

        /// <summary>
        /// 注册外部实现的可实例化对象类型的符号解析器
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void RegisterSymbolResolverOfInstantiationClass<T>() where T : Symboling.ISymbolResolverOfInstantiationClass
        {
            Symboling.SymClassResolver.AddInstantiationClassResolver<T>();
        }

        /// <summary>
        /// 注册外部实现的可实例化对象类型的符号解析器
        /// </summary>
        /// <param name="classType">对象类型</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void RegisterSymbolResolverOfInstantiationClass(SystemType classType)
        {
            Symboling.SymClassResolver.AddInstantiationClassResolver(classType);
        }

        /// <summary>
        /// 注册外部实现的可实例化对象类型的符号解析器
        /// </summary>
        /// <param name="resolver">解析对象</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void RegisterSymbolResolverOfInstantiationClass(Symboling.ISymbolResolverOfInstantiationClass resolver)
        {
            Symboling.SymClassResolver.AddInstantiationClassResolver(resolver);
        }

        /// <summary>
        /// 移除外部实现的可实例化对象类型的符号解析器
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        public static void UnregisterSymbolResolverOfInstantiationClass<T>() where T : Symboling.ISymbolResolverOfInstantiationClass
        {
            Symboling.SymClassResolver.RemoveInstantiationClassResolver<T>();
        }

        /// <summary>
        /// 移除外部实现的可实例化对象类型的符号解析器
        /// </summary>
        /// <param name="classType">对象类型</param>
        public static void UnregisterSymbolResolverOfInstantiationClass(SystemType classType)
        {
            Symboling.SymClassResolver.RemoveInstantiationClassResolver(classType);
        }

        /// <summary>
        /// 移除外部实现的可实例化对象类型的符号解析器
        /// </summary>
        /// <param name="resolver">解析对象</param>
        public static void UnregisterSymbolResolverOfInstantiationClass(Symboling.ISymbolResolverOfInstantiationClass resolver)
        {
            Symboling.SymClassResolver.RemoveInstantiationClassResolver(resolver);
        }

        #endregion

        /// <summary>
        /// 通过指定名称获取对象类的标记数据
        /// </summary>
        /// <param name="className">对象名称</param>
        /// <returns>返回对应的标记数据实例，若查找失败返回null</returns>
        public static Symboling.SymClass GetSymClassByName(string className)
        {
            if (null == _symClassMaps)
            {
                return null;
            }

            if (_symClassMaps.TryGetValue(className, out Symboling.SymClass symbol))
            {
                return symbol;
            }

            return null;
        }

        /// <summary>
        /// 通过指定类型获取对象类的标记数据
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <returns>返回对应的标记数据实例，若查找失败返回null</returns>
        public static Symboling.SymClass GetSymClassByType(SystemType targetType)
        {
            if (null == _symClassMaps)
            {
                return null;
            }

            if (_symClassMaps.TryGetValue(targetType, out Symboling.SymClass symbol))
            {
                return symbol;
            }

            return null;
        }

        /// <summary>
        /// 通过指定的基础类型获取继承该类型的对象类的标记数据清单
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static IList<Symboling.SymClass> FindAllSymClassesInheritedFrom(SystemType baseType)
        {
            IList<Symboling.SymClass> results = null;

            IEnumerator<Symboling.SymClass> e = _symClassMaps.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.IsInheritedFrom(baseType))
                {
                    if (null == results) results = new List<Symboling.SymClass>();

                    results.Add(e.Current);
                }
            }

            return results;
        }

        /// <summary>
        /// 通过指定的特性类型获取具备该特性的对象类的标记数据清单
        /// </summary>
        /// <param name="featureType">特性类型</param>
        /// <returns>返回对应的标记数据列表，若查找失败返回null</returns>
        public static IList<Symboling.SymClass> FindAllSymClassesByFeatureType(SystemType featureType)
        {
            IList<Symboling.SymClass> results = null;

            IEnumerator<Symboling.SymClass> e = _symClassMaps.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.HasFeatureType(featureType))
                {
                    if (null == results) results = new List<Symboling.SymClass>();

                    results.Add(e.Current);
                }
            }

            return results;
        }

        /// <summary>
        /// 通过指定的接口类型获取实现该接口的对象类的标记数据清单
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <returns>返回对应的标记数据列表，若查找失败返回null</returns>
        public static IList<Symboling.SymClass> FindAllSymClassesByInterfaceType(SystemType interfaceType)
        {
            IList<Symboling.SymClass> results = null;

            IEnumerator<Symboling.SymClass> e = _symClassMaps.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.HasInterfaceType(interfaceType))
                {
                    if (null == results) results = new List<Symboling.SymClass>();

                    results.Add(e.Current);
                }
            }

            return results;
        }

        /// <summary>
        /// 通过指定名称获取对象Bean类型信息
        /// </summary>
        /// <param name="beanName">对象名称</param>
        /// <returns>返回对应的Bean信息数据实例，若查找失败返回null</returns>
        public static Symboling.Bean GetBeanClassByName(string beanName)
        {
            if (_beanClassMaps.TryGetValue(beanName, out Symboling.Bean bean))
            {
                return bean;
            }

            return null;
        }

        /// <summary>
        /// 通过指定类型获取对象Bean类型信息
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <returns>返回对应的Bean信息数据实例，若查找失败返回null</returns>
        public static Symboling.Bean GetBeanClassByType(SystemType targetType)
        {
            Symboling.SymClass symClass = GetSymClassByType(targetType);
            if (null != symClass)
            {
                return GetBeanClassByName(symClass.DefaultBeanName);
            }

            return null;
        }
    }
}
