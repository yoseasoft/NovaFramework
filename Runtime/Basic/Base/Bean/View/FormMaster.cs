/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2025, Hurley, Independent Studio.
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

using SystemType = System.Type;

using UniTask = Cysharp.Threading.Tasks.UniTask;

namespace GameEngine
{
    /// <summary>
    /// GUI的窗口对象的主控制器类
    /// </summary>
    public static class FormMaster
    {
        /// <summary>
        /// 工具类启动状态标识
        /// </summary>
        private static bool _isOnStartup = false;

        private static IFormManager _formManager;

        /// <summary>
        /// 工具类启动函数
        /// </summary>
        internal static void Startup()
        {
            _isOnStartup = true;

            // 如果提前注册了表单管理器，则在主控启动时，同时启动该管理器实例
            // 正常情况下，应该不会出现这种情况，因为一般注册行为是发生在引擎完全启动结束之后
            // 但是为了避免出现这种情况，这里添加了该功能
            if (null != _formManager)
            {
                _formManager.Startup();
            }
        }

        /// <summary>
        /// 工具类关闭函数
        /// </summary>
        internal static void Shutdown()
        {
            // 此处添加该代码的原因与上面 Startup() 函数的添加原因一样
            if (null != _formManager)
            {
                _formManager.Shutdown();
            }

            _isOnStartup = false;
        }

        /// <summary>
        /// 工具类刷新函数
        /// </summary>
        internal static void Update()
        {
            Debugger.Assert(_isOnStartup, "Invalid status.");

            // 刷新表单管理器实例
            if (null != _formManager)
            {
                _formManager.Update();
            }
        }

        /// <summary>
        /// 创建一个指定类型的窗口对象实例
        /// </summary>
        /// <param name="formType">窗口类型标识</param>
        /// <param name="viewType">视图类型</param>
        /// <returns>返回窗口对象实例</returns>
        internal static Form CreateForm(SystemType viewType)
        {
            Debugger.Assert(_formManager, NovaEngine.ErrorText.NullObjectReference);

            return _formManager.CreateForm(viewType);
        }

        #region 表单管理器对象的添加/移除相关操作的接口函数

        /// <summary>
        /// 注册指定的窗口管理器到当前的主控制器中
        /// </summary>
        /// <typeparam name="T">管理器类型</typeparam>
        public static void RegisterFormManager<T>() where T : IFormManager, new()
        {
            RegisterFormManager(typeof(T));
        }

        /// <summary>
        /// 注册指定的窗口管理器到当前的主控制器中
        /// </summary>
        /// <param name="classType">管理器类型</param>
        public static void RegisterFormManager(SystemType classType)
        {
            if (false == typeof(IFormManager).IsAssignableFrom(classType))
            {
                Debugger.Error(LogGroupTag.Module, "The form manager class '{%t}' must be inherited from 'IFormManager' type, registed it failed.", classType);
                return;
            }

            IFormManager formManager = System.Activator.CreateInstance(classType) as IFormManager;
            RegisterFormManager(formManager);
        }

        /// <summary>
        /// 注册指定的窗口管理器到当前的主控制器中
        /// </summary>
        /// <param name="formManager">管理器对象</param>
        public static void RegisterFormManager(IFormManager formManager)
        {
            Debugger.Assert(formManager, NovaEngine.ErrorText.InvalidArguments);

            if (null != _formManager)
            {
                Debugger.Warn(LogGroupTag.Module, "当前的视图窗口主控制器中已存在一个有效的窗口管理器对象实例‘{%t}’，重复添加的新管理器对象将覆盖掉旧的管理器对象实例！", _formManager);
                if (_isOnStartup)
                {
                    _formManager.Shutdown();
                }
                _formManager = null;
            }

            _formManager = formManager;
            if (_isOnStartup)
            {
                // 如果主控已启动，则在注册的同时启动管理器实例
                _formManager.Startup();
            }
        }

        /// <summary>
        /// 从主控制器中移除当前实施的
        /// </summary>
        public static void UnregisterCurrentFormManager()
        {
            if (null == _formManager)
            {
                Debugger.Warn(LogGroupTag.Module, "当前的视图窗口主控制器中尚未注册有效的窗口管理器对象实例，无法执行注销操作！");
                return;
            }

            if (_isOnStartup)
            {
                _formManager.Shutdown();
            }
            _formManager = null;
        }

        #endregion
    }
}
