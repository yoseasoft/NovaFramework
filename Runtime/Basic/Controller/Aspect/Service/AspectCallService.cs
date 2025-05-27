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
using System.Reflection;

using SystemType = System.Type;
using SystemAction = System.Action;
using SystemDelegate = System.Delegate;
using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;
using SystemBindingFlags = System.Reflection.BindingFlags;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemParameterInfo = System.Reflection.ParameterInfo;

using SystemAction_object_bool = System.Action<object, bool>;

namespace GameEngine
{
    /// <summary>
    /// 提供切面访问接口的服务类，对整个程序内部的对象实例提供切面访问的服务逻辑处理
    /// </summary>
    public static partial class AspectCallService
    {
        /// <summary>
        /// 切面访问的后服务处理标准调用函数的句柄定义
        /// </summary>
        /// <param name="obj">调用对象实例</param>
        // public delegate void OnAspectServiceProcessingCallHandler(object obj);

        /// <summary>
        /// 切面处理服务接口注册相关函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private class OnServiceProcessRegisterOfTargetAttribute : SystemAttribute
        {
            /// <summary>
            /// 匹配切面服务的目标对象类型
            /// </summary>
            private readonly SystemType m_classType;
            /// <summary>
            /// 执行切面服务的函数名称
            /// </summary>
            private readonly string m_methodName;

            public SystemType ClassType => m_classType;
            public string MethodName => m_methodName;

            public OnServiceProcessRegisterOfTargetAttribute(SystemType classType, string methodName)
            {
                m_classType = classType;
                m_methodName = methodName;
            }

            public OnServiceProcessRegisterOfTargetAttribute(SystemType classType, AspectBehaviourType behaviourType)
            {
                if (false == NovaEngine.Utility.Convertion.IsCorrectedEnumValue<AspectBehaviourType>((int) behaviourType))
                {
                    Debugger.Error("Invalid aspect behaviour type ({0}).", behaviourType.ToString());
                    return;
                }

                m_classType = classType;
                m_methodName = behaviourType.ToString();
            }
        }

        /// <summary>
        /// 切面服务处理回调的管理容器
        /// </summary>
        // private static IDictionary<SystemType, IDictionary<string, OnAspectServiceProcessingCallHandler>> s_serviceProcessCallInfos = new Dictionary<SystemType, IDictionary<string, OnAspectServiceProcessingCallHandler>>();
        private static IDictionary<SystemType, IDictionary<string, SystemAction_object_bool>> s_serviceProcessCallInfos = null;
        /// <summary>
        /// 切面服务处理的启用状态标识的管理容器
        /// </summary>
        private static IDictionary<SystemType, IDictionary<string, bool>> s_serviceProcessCallStatus = null;

        /// <summary>
        /// 初始化切面服务处理类声明的全部回调接口
        /// </summary>
        internal static void InitAllServiceProcessingCallbacks()
        {
            // 切面服务管理容器初始化
            s_serviceProcessCallInfos = new Dictionary<SystemType, IDictionary<string, SystemAction_object_bool>>();
            s_serviceProcessCallStatus = new Dictionary<SystemType, IDictionary<string, bool>>();

            SystemType classType = typeof(AspectCallService);
            SystemMethodInfo[] methods = classType.GetMethods(SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Static);
            for (int n = 0; n < methods.Length; ++n)
            {
                SystemMethodInfo method = methods[n];
                IEnumerable<SystemAttribute> e = method.GetCustomAttributes();
                foreach (SystemAttribute attr in e)
                {
                    SystemType attrType = attr.GetType();
                    if (typeof(OnServiceProcessRegisterOfTargetAttribute) == attrType)
                    {
                        Debugger.Assert(method.IsStatic);

                        OnServiceProcessRegisterOfTargetAttribute _attr = (OnServiceProcessRegisterOfTargetAttribute) attr;

                        // SystemDelegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(method);
                        // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                        // SystemDelegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegateAndCheckParameterType(method, _attr.ClassType, typeof(bool));
                        SystemAction_object_bool callback = NovaEngine.Utility.Reflection.CreateGenericActionAndCheckParameterType<object, bool>(method, _attr.ClassType, typeof(bool));

                        AddServiceProcessingCallHandler(_attr.ClassType, _attr.MethodName, callback);
                    }
                }
            }
        }

        /// <summary>
        /// 清理切面服务处理类声明的全部回调接口
        /// </summary>
        internal static void CleanupAllServiceProcessingCallbacks()
        {
            // 切面服务管理容器清理
            s_serviceProcessCallInfos.Clear();
            s_serviceProcessCallStatus.Clear();

            s_serviceProcessCallInfos = null;
            s_serviceProcessCallStatus = null;
        }

        /// <summary>
        /// 调用指定对象和函数名称对应的服务处理程序
        /// </summary>
        /// <param name="target">对象实例</param>
        /// <param name="methodName">函数名称</param>
        /// <returns>若存在指定的服务处理程序并完成调度则返回true，否则返回false</returns>
        internal static bool CallServiceProcess(object target, string methodName, bool reload = false)
        {
            SystemType targetType = target.GetType();

            bool ret_status = false, has_status = false;
            if (TryGetServiceProcessingCallStatus(targetType, methodName, out ret_status))
            {
                if (false == ret_status && false == reload)
                {
                    // 当启用状态为false，且此次调用非reload模式时，直接返回
                    // 因为不存在任何能匹配指定类型和函数的服务
                    return false;
                }

                has_status = true;
            }

            ret_status = false;
            IList<IDictionary<string, SystemAction_object_bool>> list = null;
            if (TryGetServiceProcessingCallHandler(targetType, out list))
            {
                for (int n = 0; n < list.Count; ++n)
                {
                    IDictionary<string, SystemAction_object_bool> targetServiceInfos = list[n];
                    if (targetServiceInfos.ContainsKey(methodName))
                    {
                        ret_status = true;

                        SystemAction_object_bool handler = targetServiceInfos[methodName];
                        handler.Invoke(target, reload);
                    }
                }
            }

            // 可能会因为reload模式，导致即使已经存在指定类型和函数的状态标识，也进入了调度流程
            // 这种情况下，我们不考虑在reload过程中修改了相关服务的情况，依然沿用之前的状态标识
            if (false == has_status && false == reload)
            {
                AddServiceProcessingCallStatus(targetType, methodName, ret_status);
            }

            return ret_status;
        }

        /// <summary>
        /// 新增指定类型和函数名称对应的服务处理回调句柄
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="methodName">函数名称</param>
        /// <param name="handler">回调句柄</param>
        private static void AddServiceProcessingCallHandler(SystemType targetType, string methodName, SystemAction_object_bool handler)
        {
            IDictionary<string, SystemAction_object_bool> targetServiceInfos = null;
            if (false == s_serviceProcessCallInfos.TryGetValue(targetType, out targetServiceInfos))
            {
                targetServiceInfos = new Dictionary<string, SystemAction_object_bool>();

                s_serviceProcessCallInfos.Add(targetType, targetServiceInfos);
            }

            if (targetServiceInfos.ContainsKey(methodName))
            {
                Debugger.Warn("The handler '{0}' was already exists for target method '{1}.{2}', repeated add it will be override old handler.",
                        NovaEngine.Utility.Text.ToString(handler), NovaEngine.Utility.Text.ToString(targetType), methodName);

                targetServiceInfos.Remove(methodName);
            }

            targetServiceInfos.Add(methodName, handler);
        }

        /// <summary>
        /// 新增指定类型和函数名称对应的服务处理启用状态标识
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="methodName">函数名称</param>
        /// <param name="status">启用状态标识</param>
        private static void AddServiceProcessingCallStatus(SystemType targetType, string methodName, bool status)
        {
            IDictionary<string, bool> targetServiceStatus = null;
            if (false == s_serviceProcessCallStatus.TryGetValue(targetType, out targetServiceStatus))
            {
                targetServiceStatus = new Dictionary<string, bool>();

                s_serviceProcessCallStatus.Add(targetType, targetServiceStatus);
            }

            if (targetServiceStatus.ContainsKey(methodName))
            {
                Debugger.Warn("The service processing status '{0}' was already exists for target method '{1}.{2}', repeated add it will be override old handler.",
                        status, NovaEngine.Utility.Text.ToString(targetType), methodName);

                targetServiceStatus.Remove(methodName);
            }

            targetServiceStatus.Add(methodName, status);
        }

        /// <summary>
        /// 通过指定的类型从切面服务管理容器中查找对应的回调句柄实例
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="handlers">句柄列表</param>
        /// <returns>若查找句柄列表成功返回true，否则返回false</returns>
        private static bool TryGetServiceProcessingCallHandler(SystemType targetType, out IList<IDictionary<string, SystemAction_object_bool>> handlers)
        {
            handlers = null;

            IList<IDictionary<string, SystemAction_object_bool>> list = null;
            IEnumerator<KeyValuePair<SystemType, IDictionary<string, SystemAction_object_bool>>> e = s_serviceProcessCallInfos.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Key.IsSubclassOf(typeof(SystemAttribute)))
                {
                    // 属性类的绑定回调
                    IEnumerable<SystemAttribute> attrTypes = targetType.GetCustomAttributes();
                    foreach (SystemAttribute attrType in attrTypes)
                    {
                        if (e.Current.Key == attrType.GetType())
                        {
                            if (null == list) list = new List<IDictionary<string, SystemAction_object_bool>>();
                            list.Add(e.Current.Value);
                            break;
                        }
                    }
                }
                else
                {
                    // 对象类的绑定回调
                    if (e.Current.Key == targetType || e.Current.Key.IsAssignableFrom(targetType))
                    {
                        if (null == list) list = new List<IDictionary<string, SystemAction_object_bool>>();
                        list.Add(e.Current.Value);
                    }
                }
            }

            if (null != list && list.Count > 0)
            {
                handlers = list;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 通过指定的类型从切面服务状态容器中查找其当前的启用状态
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="methodName">函数名称</param>
        /// <param name="status">启用状态标识</param>
        /// <returns>若查找状态标识成功返回true，否则返回false</returns>
        private static bool TryGetServiceProcessingCallStatus(SystemType targetType, string methodName, out bool status)
        {
            status = false;

            if (false == s_serviceProcessCallStatus.TryGetValue(targetType, out IDictionary<string, bool> targetServiceStatus))
            {
                return false;
            }

            return targetServiceStatus.TryGetValue(methodName, out status);
        }
    }
}
