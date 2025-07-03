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
        private static Symboling.SymClassMap s_symClassMaps = null;

        /// <summary>
        /// 对象类的Bean信息管理容器
        /// </summary>
        private static IDictionary<string, Symboling.Bean> s_beanClassMaps = null;

        /// <summary>
        /// 初始化针对所有标记对象类声明的全部绑定回调接口
        /// </summary>
        [OnCodeLoaderSubmoduleInitCallback]
        private static void InitAllSymClassLoadingCallbacks()
        {
            // 初始化标记数据容器
            s_symClassMaps = new Symboling.SymClassMap();
            // 初始化Bean数据容器
            s_beanClassMaps = new Dictionary<string, Symboling.Bean>();
        }

        /// <summary>
        /// 清理针对所有标记对象类声明的全部绑定回调接口
        /// </summary>
        [OnCodeLoaderSubmoduleCleanupCallback]
        private static void CleanupAllSymClassLoadingCallbacks()
        {
            // 清理标识数据容器
            UnloadAllSymClasses();

            s_symClassMaps = null;
            s_beanClassMaps = null;
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
                if (s_symClassMaps.ContainsKey(symbol.ClassName))
                {
                    s_symClassMaps.Remove(symbol.ClassName);
                }
                else
                {
                    // 在重载前已经卸载掉所有的类标记对象，所以此处必定查找不到目标对象
                    // Debugger.Warn("Could not found any class symbol with target name '{0}' and type '{1}', removed it failed.", symbol.TargetName, NovaEngine.Utility.Text.ToString(targetType));
                }
            }

            // 安全检查
            Debugger.Assert(false == s_symClassMaps.ContainsKey(symbol.ClassName), $"Load class symbol {symbol.ClassName} error.");

            Debugger.Log(LogGroupTag.CodeLoader, "Load class symbol '{%s}' succeed from target class type '{%f}'.", Symboling.Formatter.ToString(symbol), targetType);

            // 添加标记信息
            s_symClassMaps.Add(symbol);

            // 添加Bean信息
            IEnumerator<KeyValuePair<string, Symboling.Bean>> beans = symbol.GetBeanEnumerator();
            if (null != beans)
            {
                while (beans.MoveNext())
                {
                    Symboling.Bean bean = beans.Current.Value;

                    RegisterBeanObjectOfSymClass(bean);
                }
            }

            return symbol;
        }

        /// <summary>
        /// 卸载所有对象类型的标记信息
        /// </summary>
        private static void UnloadAllSymClasses()
        {
            s_symClassMaps.Clear();
            s_beanClassMaps.Clear();
        }

        /// <summary>
        /// 注册标记类对应的实体对象配置信息
        /// </summary>
        /// <param name="bean">实体实例</param>
        private static void RegisterBeanObjectOfSymClass(Symboling.Bean bean)
        {
            string beanName = bean.BeanName;
            if (s_beanClassMaps.ContainsKey(beanName))
            {
                Debugger.Warn("The bean object '{0}' was already exist within class map, repeat added it failed.", beanName);
                return;
            }

            Debugger.Info(LogGroupTag.CodeLoader, "Register new bean object '{0}' to target symbol class '{1}'.", beanName, bean.TargetClass.FullName);

            s_beanClassMaps.Add(beanName, bean);
        }

        /// <summary>
        /// 通过指定名称获取对象类的标记数据
        /// </summary>
        /// <param name="className">对象名称</param>
        /// <returns>返回对应的标记数据实例，若查找失败返回null</returns>
        public static Symboling.SymClass GetSymClassByName(string className)
        {
            if (s_symClassMaps.TryGetValue(className, out Symboling.SymClass symbol))
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
            if (s_symClassMaps.TryGetValue(targetType, out Symboling.SymClass symbol))
            {
                return symbol;
            }

            return null;
        }

        /// <summary>
        /// 通过指定的特性类型获取具备该特性的对象类的标记数据清单
        /// </summary>
        /// <param name="featureType">特性类型</param>
        /// <returns>返回对应的标记数据列表，若查找失败返回null</returns>
        public static IList<Symboling.SymClass> FindAllSymClassesByFeatureType(SystemType featureType)
        {
            IList<Symboling.SymClass> results = null;

            IEnumerator<Symboling.SymClass> e = s_symClassMaps.GetEnumerator();
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

            IEnumerator<Symboling.SymClass> e = s_symClassMaps.GetEnumerator();
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
            if (s_beanClassMaps.TryGetValue(beanName, out Symboling.Bean bean))
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
