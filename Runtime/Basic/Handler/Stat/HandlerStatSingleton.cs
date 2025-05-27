/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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
using System.Reflection;

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemDelegate = System.Delegate;
using SystemBindingFlags = System.Reflection.BindingFlags;
using SystemMethodInfo = System.Reflection.MethodInfo;

namespace GameEngine
{
    /// <summary>
    /// 句柄统计模块对象的单例构建类，该类以单例的形式对目标统计对象进行实例化管理
    /// </summary>
    public abstract class HandlerStatSingleton<T> where T : class, IStatModule, new()
    {
        private static T _instance = null;

        /// <summary>
        /// 统计模块的模块类型标识
        /// </summary>
        private int m_moduleType;

        /// <summary>
        /// 统计模块对象启动状态标识
        /// </summary>
        private bool m_enabled = false;

        private IDictionary<int, SystemMethodInfo> m_regStatMethodTypes;

        /// <summary>
        /// 获取统计模块的模块类型标识
        /// </summary>
        public int ModuleType => m_moduleType;

        /// <summary>
        /// 获取统计模块对象的启动状态
        /// </summary>
        public bool Enabled => m_enabled;

        /// <summary>
        /// 单例对象的默认构造函数<br/>
        /// 此处将函数的作用域声明为‘protected’，需要在自定义子类时实现该默认构造函数，且打开其访问作用域
        /// </summary>
        protected HandlerStatSingleton()
        {
        }

        /// <summary>
        /// 单例对象的默认析构函数
        /// </summary>
        ~HandlerStatSingleton()
        {
        }

        /// <summary>
        /// 获取单例类当前的有效实例
        /// </summary>
        public static T Instance
        {
            get
            {
                // 禁止被动创建实例，必须通过主动调用“Create”进行实例创建
                // if (null == HandlerStatSingleton<T>._instance) { HandlerStatSingleton<T>.Create(); }

                return HandlerStatSingleton<T>._instance;
            }
        }

        /// <summary>
        /// 单例类的实例创建接口
        /// </summary>
        /// <param name="handler">句柄对象实例</param>
        internal static T Create(IHandler handler)
        {
            // 仅在调试模式下才创建统计模块实例
            if (NovaEngine.Environment.debugMode)
            {
                if (HandlerStatSingleton<T>._instance == null)
                {
                    HandlerStatSingleton<T>._instance = System.Activator.CreateInstance<T>();
                    (HandlerStatSingleton<T>._instance as HandlerStatSingleton<T>).Initialize(handler);
                }
            }

            return HandlerStatSingleton<T>._instance;
        }

        /// <summary>
        /// 单例类的实例销毁接口
        /// </summary>
        internal static void Destroy()
        {
            if (HandlerStatSingleton<T>._instance != null)
            {
                (HandlerStatSingleton<T>._instance as HandlerStatSingleton<T>).Cleanup();
                HandlerStatSingleton<T>._instance = (T) ((object) null);
            }
        }

        /// <summary>
        /// 单例类初始化回调接口
        /// </summary>
        /// <param name="handler">句柄对象实例</param>
        private void Initialize(IHandler handler)
        {
            Debugger.Assert(null != handler, "Invalid arguments.");

            m_moduleType = handler.HandlerType;
            m_regStatMethodTypes = new Dictionary<int, SystemMethodInfo>();

            // 仅在调试模块下开启统计功能
            if (NovaEngine.Environment.debugMode)
            {
                m_enabled = true;
            }

            // 统计接口初始化绑定
            InitAllStatMethods();

            // 注册统计模块实例
            HandlerManagement.RegisterStatModule(m_moduleType, _instance);

            OnInitialize();
        }

        /// <summary>
        /// 单例类清理回调接口
        /// </summary>
        private void Cleanup()
        {
            OnCleanup();

            // 注销统计模块实例
            HandlerManagement.UnregisterStatModule(m_moduleType);

            // 注销所有统计回调函数
            UnregisterAllStatMethods();
            m_regStatMethodTypes = null;
        }

        /// <summary>
        /// 初始化引擎框架模块实例的回调接口
        /// </summary>
        protected abstract void OnInitialize();

        /// <summary>
        /// 清理引擎框架模块实例的回调接口
        /// </summary>
        protected abstract void OnCleanup();

        /// <summary>
        /// 对模块内部所有的统计函数进行初始化绑定操作
        /// </summary>
        private void InitAllStatMethods()
        {
            SystemType classType = GetType();
            SystemMethodInfo[] methods = classType.GetMethods(SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Instance);
            for (int n = 0; n < methods.Length; ++n)
            {
                SystemMethodInfo method = methods[n];
                SystemAttribute attr = method.GetCustomAttribute(typeof(IStatModule.OnStatModuleRegisterCallbackAttribute));
                if (null != attr)
                {
                    IStatModule.OnStatModuleRegisterCallbackAttribute _attr = (IStatModule.OnStatModuleRegisterCallbackAttribute) attr;
                    RegisterStatMethod(_attr.FuncType, method);
                }
            }
        }

        /// <summary>
        /// 注册指定类型的统计函数
        /// </summary>
        /// <param name="funcType">统计类型</param>
        /// <param name="methodName">函数名称</param>
        protected void RegisterStatMethod(int funcType, string methodName)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                throw new NovaEngine.CFrameworkException("The method name is invalid.");
            }

            SystemMethodInfo method = this.GetType().GetMethod(methodName, SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Instance);
            if (null == method)
            {
                Debugger.Error("Could not found any method info with name '{0}' in current class '{1}', register that method info failed.", methodName, this.GetType().FullName);
                return;
            }

            RegisterStatMethod(funcType, method);
        }

        /// <summary>
        /// 注册指定类型的统计函数
        /// </summary>
        /// <param name="funcType">统计类型</param>
        /// <param name="method">函数信息</param>
        protected void RegisterStatMethod(int funcType, SystemMethodInfo method)
        {
            Debugger.Assert(null != m_regStatMethodTypes, "The register method container must be non-null.");

            if (m_regStatMethodTypes.ContainsKey(funcType))
            {
                Debugger.Warn("The stat method type '{0}' was already register, repeat do it will be override old value.", funcType);
                m_regStatMethodTypes.Remove(funcType);
            }

            m_regStatMethodTypes.Add(funcType, method);
        }

        /// <summary>
        /// 注销当前已注册的所有统计函数
        /// </summary>
        private void UnregisterAllStatMethods()
        {
            // 清空容器
            m_regStatMethodTypes.Clear();
        }

        /// <summary>
        /// 检测指定的统计模块对象实例是否处于激活状态
        /// </summary>
        /// <returns>返回统计模块实例的激活状态</returns>
        internal static bool IsActivated()
        {
            if (null == _instance) return false;

            return (HandlerStatSingleton<T>._instance as HandlerStatSingleton<T>).Enabled;
        }

        /// <summary>
        /// 清理统计模块对象实例中的所有临时记录数据
        /// </summary>
        internal static void ClearAll()
        {
            if (null == _instance) return;

            _instance.Dump();
        }

        /// <summary>
        /// 调用指定类型的统计函数
        /// </summary>
        /// <param name="funcType">统计类型</param>
        /// <param name="args">参数列表</param>
        public static void CallStatAction(int funcType, params object[] args)
        {
            if (false == IsActivated()) return;

            SystemMethodInfo method = null;
            if (false == (HandlerStatSingleton<T>._instance as HandlerStatSingleton<T>).m_regStatMethodTypes.TryGetValue(funcType, out method))
            {
                Debugger.Warn("Could not found any register stat method with type '{0}', invoke it failed.", funcType);
                return;
            }

            method.Invoke(HandlerStatSingleton<T>._instance as HandlerStatSingleton<T>, args);
        }
    }
}
