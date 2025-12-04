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

namespace GameEngine
{
    /// <summary>
    /// 原型对象管理类，用于对场景上下文中的所有原型对象提供通用的访问操作接口
    /// </summary>
    internal sealed partial class BeanController
    {
        /// <summary>
        /// 原型对象生命周期处理函数接口定义
        /// </summary>
        /// <param name="bean">原型对象实例</param>
        private delegate void OnBeanLifecycleProcessingHandler(IBean bean);

        /// <summary>
        /// 原型对象生命周期注册相关函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private class OnBeanLifecycleRegisterAttribute : SystemAttribute
        {
            /// <summary>
            /// 管理生命周期对象的行为类型
            /// </summary>
            private readonly AspectBehaviourType _behaviourType;

            public AspectBehaviourType BehaviourType => _behaviourType;

            public OnBeanLifecycleRegisterAttribute(AspectBehaviourType behaviourType) { _behaviourType = behaviourType; }
        }

        /// <summary>
        /// 原型对象生命周期注销相关函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private class OnBeanLifecycleUnregisterAttribute : SystemAttribute
        {
            /// <summary>
            /// 管理生命周期对象的行为类型
            /// </summary>
            private readonly AspectBehaviourType _behaviourType;

            public AspectBehaviourType BehaviourType => _behaviourType;

            public OnBeanLifecycleUnregisterAttribute(AspectBehaviourType behaviourType) { _behaviourType = behaviourType; }
        }

        /// <summary>
        /// 原型对象生命周期处理服务接口注册相关函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private class OnBeanLifecycleProcessRegisterOfTargetAttribute : SystemAttribute
        {
            /// <summary>
            /// 匹配生命周期处理服务的目标对象类型
            /// </summary>
            private readonly SystemType _classType;
            /// <summary>
            /// 执行生命周期处理服务的行为类型
            /// </summary>
            private readonly AspectBehaviourType _behaviourType;

            public SystemType ClassType => _classType;
            public AspectBehaviourType BehaviourType => _behaviourType;

            public OnBeanLifecycleProcessRegisterOfTargetAttribute(SystemType classType, AspectBehaviourType behaviourType)
            {
                _classType = classType;
                _behaviourType = behaviourType;
            }
        }

        /// <summary>
        /// 原型对象生命周期服务处理句柄列表容器
        /// </summary>
        private IDictionary<SystemType, IDictionary<AspectBehaviourType, OnBeanLifecycleProcessingHandler>> _beanLifecycleProcessingCallbacks = null;

        /// <summary>
        /// 原型对象生命周期注册函数列表容器
        /// </summary>
        private IDictionary<AspectBehaviourType, OnBeanLifecycleProcessingHandler> _beanLifecycleRegisterCallbacks = null;
        /// <summary>
        /// 原型对象生命周期注销函数列表容器
        /// </summary>
        private IDictionary<AspectBehaviourType, OnBeanLifecycleProcessingHandler> _beanLifecycleUnregisterCallbacks = null;

        /// <summary>
        /// 原型管理对象的生命周期管理初始化通知接口函数
        /// </summary>
        [OnControllerSubmoduleInitCallback]
        private void OnBeanLifecycleInitialize()
        {
            // 初始化原型对象生命周期句柄列表容器
            _beanLifecycleProcessingCallbacks = new Dictionary<SystemType, IDictionary<AspectBehaviourType, OnBeanLifecycleProcessingHandler>>();
            // 初始化原型对象生命周期注册函数列表容器
            _beanLifecycleRegisterCallbacks = new Dictionary<AspectBehaviourType, OnBeanLifecycleProcessingHandler>();
            // 初始化原型对象生命周期注销函数列表容器
            _beanLifecycleUnregisterCallbacks = new Dictionary<AspectBehaviourType, OnBeanLifecycleProcessingHandler>();

            SystemType classType = typeof(BeanController);
            MethodInfo[] methods = classType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for (int n = 0; n < methods.Length; ++n)
            {
                MethodInfo method = methods[n];
                IEnumerable<SystemAttribute> e = method.GetCustomAttributes();
                foreach (SystemAttribute attr in e)
                {
                    SystemType attrType = attr.GetType();
                    if (typeof(OnBeanLifecycleRegisterAttribute) == attrType)
                    {
                        Debugger.Assert(false == method.IsStatic);

                        OnBeanLifecycleRegisterAttribute _attr = (OnBeanLifecycleRegisterAttribute) attr;

                        SystemDelegate callback = method.CreateDelegate(typeof(OnBeanLifecycleProcessingHandler), this);
                        // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                        NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(callback, typeof(IBean));

                        AddBeanLifecycleRegisterCallback(_attr.BehaviourType, callback as OnBeanLifecycleProcessingHandler);
                    }
                    else if (typeof(OnBeanLifecycleUnregisterAttribute) == attrType)
                    {
                        Debugger.Assert(false == method.IsStatic);

                        OnBeanLifecycleUnregisterAttribute _attr = (OnBeanLifecycleUnregisterAttribute) attr;

                        SystemDelegate callback = method.CreateDelegate(typeof(OnBeanLifecycleProcessingHandler), this);
                        // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                        NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(callback, typeof(IBean));

                        AddBeanLifecycleUnregisterCallback(_attr.BehaviourType, callback as OnBeanLifecycleProcessingHandler);
                    }
                    else if (typeof(OnBeanLifecycleProcessRegisterOfTargetAttribute) == attrType)
                    {
                        Debugger.Assert(false == method.IsStatic);

                        OnBeanLifecycleProcessRegisterOfTargetAttribute _attr = (OnBeanLifecycleProcessRegisterOfTargetAttribute) attr;

                        SystemDelegate callback = method.CreateDelegate(typeof(OnBeanLifecycleProcessingHandler), this);
                        // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                        NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(callback, _attr.ClassType);

                        AddBeanLifecycleProcessingCallHandler(_attr.ClassType, _attr.BehaviourType, callback as OnBeanLifecycleProcessingHandler);
                    }
                }
            }
        }

        /// <summary>
        /// 原型管理对象的生命周期管理清理通知接口函数
        /// </summary>
        [OnControllerSubmoduleCleanupCallback]
        private void OnBeanLifecycleCleanup()
        {
            // 清理原型对象生命周期句柄列表容器
            _beanLifecycleProcessingCallbacks.Clear();
            _beanLifecycleProcessingCallbacks = null;

            // 清理原型对象生命周期注册函数列表容器
            _beanLifecycleRegisterCallbacks.Clear();
            _beanLifecycleRegisterCallbacks = null;
            // 清理原型对象生命周期注销函数列表容器
            _beanLifecycleUnregisterCallbacks.Clear();
            _beanLifecycleUnregisterCallbacks = null;
        }

        /// <summary>
        /// 原型管理对象的生命周期处理接口函数
        /// </summary>
        // [OnControllerSubmoduleUpdateCallback]
        private void OnBeanLifecycleProcess()
        {
        }

        #region 原型对象生命周期管理接口函数

        /// <summary>
        /// 新增原型对象生命周期注册回调句柄
        /// </summary>
        /// <param name="behaviourType">行为类型</param>
        /// <param name="callback">回调函数句柄</param>
        private void AddBeanLifecycleRegisterCallback(AspectBehaviourType behaviourType, OnBeanLifecycleProcessingHandler callback)
        {
            if (_beanLifecycleRegisterCallbacks.ContainsKey(behaviourType))
            {
                Debugger.Warn("The bean lifecycle register callback for target behaviour type '{0}' was already exist, repeat added it will be override old value.", behaviourType.ToString());
                _beanLifecycleRegisterCallbacks.Remove(behaviourType);
            }

            _beanLifecycleRegisterCallbacks.Add(behaviourType, callback);
        }

        /// <summary>
        /// 新增原型对象生命周期注册回调句柄
        /// </summary>
        /// <param name="behaviourType">行为类型</param>
        /// <param name="callback">回调函数句柄</param>
        private void AddBeanLifecycleUnregisterCallback(AspectBehaviourType behaviourType, OnBeanLifecycleProcessingHandler callback)
        {
            if (_beanLifecycleUnregisterCallbacks.ContainsKey(behaviourType))
            {
                Debugger.Warn("The bean lifecycle unregister callback for target behaviour type '{0}' was already exist, repeat added it will be override old value.", behaviourType.ToString());
                _beanLifecycleUnregisterCallbacks.Remove(behaviourType);
            }

            _beanLifecycleUnregisterCallbacks.Add(behaviourType, callback);
        }

        /// <summary>
        /// 原型对象的生命周期通知注册接口函数
        /// </summary>
        /// <param name="behaviourType">行为类型</param>
        /// <param name="bean">原型对象实例</param>
        public void RegBeanLifecycleNotification(AspectBehaviourType behaviourType, IBean bean)
        {
            if (false == _beanLifecycleRegisterCallbacks.TryGetValue(behaviourType, out OnBeanLifecycleProcessingHandler callback))
            {
                Debugger.Error("Could not found any lifecycle notfication callback with target behaviour type '{0}', register bean lifecycle notification failed.", behaviourType.ToString());
                return;
            }

            callback(bean);
        }

        /// <summary>
        /// 原型对象的生命周期通知注销接口函数
        /// </summary>
        /// <param name="bean">原型对象实例</param>
        public void UnregBeanLifecycleNotification(IBean bean)
        {
            IEnumerator<OnBeanLifecycleProcessingHandler> e = _beanLifecycleUnregisterCallbacks.Values.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current(bean);
            }
        }

        /// <summary>
        /// 在指定容器中查找属于指定实体对象的全部组件实例
        /// </summary>
        /// <param name="entity">目标实体对象</param>
        /// <param name="container">对象容器</param>
        /// <returns>返回查找的组件列表，若不存在则返回null</returns>
        private IList<CComponent> FindAllComponentsBelongingToTargetEntityFromTheContainer(CEntity entity, IList<IBean> container)
        {
            IList<CComponent> result = null;

            IEnumerator<IBean> e = container.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current is CComponent)
                {
                    CComponent component = e.Current as CComponent;
                    if (entity == component.Entity)
                    {
                        if (null == result) result = new List<CComponent>();

                        result.Add(component);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 移除指定容器中属于指定实体对象的全部组件实例
        /// </summary>
        /// <param name="entity">目标实体对象</param>
        /// <param name="container">对象容器</param>
        private void RemoveAllComponentsBelongingToTargetEntityFromTheContainer(CEntity entity, IList<IBean> container)
        {
            IList<CComponent> components = FindAllComponentsBelongingToTargetEntityFromTheContainer(entity, container);
            if (null == components)
            {
                return;
            }

            for (int n = 0; n < components.Count; ++n)
            {
                container.Remove(components[n]);
            }
        }

        /// <summary>
        /// 通过指定的类型从服务处理容器中查找对应的回调句柄实例
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="behaviourType">行为类型</param>
        /// <param name="callback">回调句柄</param>
        /// <returns>若查找回调句柄成功返回true，否则返回false</returns>
        private bool TryGetBeanLifecycleProcessingCallback(SystemType targetType, AspectBehaviourType behaviourType, out OnBeanLifecycleProcessingHandler callback)
        {
            callback = null;

            foreach (KeyValuePair<SystemType, IDictionary<AspectBehaviourType, OnBeanLifecycleProcessingHandler>> pair in _beanLifecycleProcessingCallbacks)
            {
                if (pair.Key.IsAssignableFrom(targetType))
                {
                    if (pair.Value.ContainsKey(behaviourType))
                    {
                        callback = pair.Value[behaviourType];
                        return true;
                    }

                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 新增指定类型和函数名称对应的服务处理回调句柄
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="behaviourType">行为类型</param>
        /// <param name="callback">回调句柄</param>
        private void AddBeanLifecycleProcessingCallHandler(SystemType targetType, AspectBehaviourType behaviourType, OnBeanLifecycleProcessingHandler callback)
        {
            IDictionary<AspectBehaviourType, OnBeanLifecycleProcessingHandler> dict;
            if (false == _beanLifecycleProcessingCallbacks.TryGetValue(targetType, out dict))
            {
                dict = new Dictionary<AspectBehaviourType, OnBeanLifecycleProcessingHandler>();

                _beanLifecycleProcessingCallbacks.Add(targetType, dict);
            }

            if (dict.ContainsKey(behaviourType))
            {
                Debugger.Warn("The callback '{0}' was already exists for target type '{1} - {2}', repeated add it will be override old handler.",
                        NovaEngine.Utility.Text.ToString(callback), targetType.FullName, behaviourType.ToString());

                dict.Remove(behaviourType);
            }

            dict.Add(behaviourType, callback);
        }

        #endregion
    }
}
