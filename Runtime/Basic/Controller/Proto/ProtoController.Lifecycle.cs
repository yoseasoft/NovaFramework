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
using SystemBindingFlags = System.Reflection.BindingFlags;
using SystemMethodInfo = System.Reflection.MethodInfo;

namespace GameEngine
{
    /// <summary>
    /// 原型对象管理类，用于对场景上下文中的所有原型对象提供通用的访问操作接口
    /// </summary>
    public sealed partial class ProtoController
    {
        /// <summary>
        /// 原型对象生命周期处理函数接口定义
        /// </summary>
        /// <param name="proto"></param>
        private delegate void OnProtoLifecycleProcessingHandler(IProto proto);

        /// <summary>
        /// 原型对象生命周期注册相关函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private class OnProtoLifecycleRegisterAttribute : SystemAttribute
        {
            /// <summary>
            /// 管理生命周期对象的行为类型
            /// </summary>
            private readonly AspectBehaviourType _behaviourType;

            public AspectBehaviourType BehaviourType => _behaviourType;

            public OnProtoLifecycleRegisterAttribute(AspectBehaviourType behaviourType) { _behaviourType = behaviourType; }
        }

        /// <summary>
        /// 原型对象生命周期注销相关函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private class OnProtoLifecycleUnregisterAttribute : SystemAttribute
        {
            /// <summary>
            /// 管理生命周期对象的行为类型
            /// </summary>
            private readonly AspectBehaviourType _behaviourType;

            public AspectBehaviourType BehaviourType => _behaviourType;

            public OnProtoLifecycleUnregisterAttribute(AspectBehaviourType behaviourType) { _behaviourType = behaviourType; }
        }

        /// <summary>
        /// 原型对象生命周期处理服务接口注册相关函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private class OnProtoLifecycleProcessRegisterOfTargetAttribute : SystemAttribute
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

            public OnProtoLifecycleProcessRegisterOfTargetAttribute(SystemType classType, AspectBehaviourType behaviourType)
            {
                _classType = classType;
                _behaviourType = behaviourType;
            }
        }

        /// <summary>
        /// 原型对象生命周期服务处理句柄列表容器
        /// </summary>
        private IDictionary<SystemType, IDictionary<AspectBehaviourType, OnProtoLifecycleProcessingHandler>> _protoLifecycleProcessingCallbacks = null;

        /// <summary>
        /// 原型对象生命周期注册函数列表容器
        /// </summary>
        private IDictionary<AspectBehaviourType, OnProtoLifecycleProcessingHandler> _protoLifecycleRegisterCallbacks = null;
        /// <summary>
        /// 原型对象生命周期注销函数列表容器
        /// </summary>
        private IDictionary<AspectBehaviourType, OnProtoLifecycleProcessingHandler> _protoLifecycleUnregisterCallbacks = null;

        /// <summary>
        /// 原型管理对象的生命周期管理初始化通知接口函数
        /// </summary>
        [OnControllerSubmoduleInitCallback]
        private void OnProtoLifecycleInitialize()
        {
            // 初始化原型对象生命周期句柄列表容器
            _protoLifecycleProcessingCallbacks = new Dictionary<SystemType, IDictionary<AspectBehaviourType, OnProtoLifecycleProcessingHandler>>();
            // 初始化原型对象生命周期注册函数列表容器
            _protoLifecycleRegisterCallbacks = new Dictionary<AspectBehaviourType, OnProtoLifecycleProcessingHandler>();
            // 初始化原型对象生命周期注销函数列表容器
            _protoLifecycleUnregisterCallbacks = new Dictionary<AspectBehaviourType, OnProtoLifecycleProcessingHandler>();

            SystemType classType = typeof(ProtoController);
            SystemMethodInfo[] methods = classType.GetMethods(SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Instance);
            for (int n = 0; n < methods.Length; ++n)
            {
                SystemMethodInfo method = methods[n];
                IEnumerable<SystemAttribute> e = method.GetCustomAttributes();
                foreach (SystemAttribute attr in e)
                {
                    SystemType attrType = attr.GetType();
                    if (typeof(OnProtoLifecycleRegisterAttribute) == attrType)
                    {
                        Debugger.Assert(false == method.IsStatic);

                        OnProtoLifecycleRegisterAttribute _attr = (OnProtoLifecycleRegisterAttribute) attr;

                        SystemDelegate callback = method.CreateDelegate(typeof(OnProtoLifecycleProcessingHandler), this);
                        // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                        NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(callback, typeof(IProto));

                        AddProtoLifecycleRegisterCallback(_attr.BehaviourType, callback as OnProtoLifecycleProcessingHandler);
                    }
                    else if (typeof(OnProtoLifecycleUnregisterAttribute) == attrType)
                    {
                        Debugger.Assert(false == method.IsStatic);

                        OnProtoLifecycleUnregisterAttribute _attr = (OnProtoLifecycleUnregisterAttribute) attr;

                        SystemDelegate callback = method.CreateDelegate(typeof(OnProtoLifecycleProcessingHandler), this);
                        // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                        NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(callback, typeof(IProto));

                        AddProtoLifecycleUnregisterCallback(_attr.BehaviourType, callback as OnProtoLifecycleProcessingHandler);
                    }
                    else if (typeof(OnProtoLifecycleProcessRegisterOfTargetAttribute) == attrType)
                    {
                        Debugger.Assert(false == method.IsStatic);

                        OnProtoLifecycleProcessRegisterOfTargetAttribute _attr = (OnProtoLifecycleProcessRegisterOfTargetAttribute) attr;

                        SystemDelegate callback = method.CreateDelegate(typeof(OnProtoLifecycleProcessingHandler), this);
                        // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                        NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(callback, _attr.ClassType);

                        AddProtoLifecycleProcessingCallHandler(_attr.ClassType, _attr.BehaviourType, callback as OnProtoLifecycleProcessingHandler);
                    }
                }
            }
        }

        /// <summary>
        /// 原型管理对象的生命周期管理清理通知接口函数
        /// </summary>
        [OnControllerSubmoduleCleanupCallback]
        private void OnProtoLifecycleCleanup()
        {
            // 清理原型对象生命周期句柄列表容器
            _protoLifecycleProcessingCallbacks.Clear();
            _protoLifecycleProcessingCallbacks = null;

            // 清理原型对象生命周期注册函数列表容器
            _protoLifecycleRegisterCallbacks.Clear();
            _protoLifecycleRegisterCallbacks = null;
            // 清理原型对象生命周期注销函数列表容器
            _protoLifecycleUnregisterCallbacks.Clear();
            _protoLifecycleUnregisterCallbacks = null;
        }

        /// <summary>
        /// 原型管理对象的生命周期处理接口函数
        /// </summary>
        // [OnControllerSubmoduleUpdateCallback]
        private void OnProtoLifecycleProcess()
        {
        }

        #region 原型对象生命周期管理接口函数

        /// <summary>
        /// 新增原型对象生命周期注册回调句柄
        /// </summary>
        /// <param name="behaviourType">行为类型</param>
        /// <param name="callback">回调函数句柄</param>
        private void AddProtoLifecycleRegisterCallback(AspectBehaviourType behaviourType, OnProtoLifecycleProcessingHandler callback)
        {
            if (_protoLifecycleRegisterCallbacks.ContainsKey(behaviourType))
            {
                Debugger.Warn("The proto lifecycle register callback for target behaviour type '{0}' was already exist, repeat added it will be override old value.", behaviourType.ToString());
                _protoLifecycleRegisterCallbacks.Remove(behaviourType);
            }

            _protoLifecycleRegisterCallbacks.Add(behaviourType, callback);
        }

        /// <summary>
        /// 新增原型对象生命周期注册回调句柄
        /// </summary>
        /// <param name="behaviourType">行为类型</param>
        /// <param name="callback">回调函数句柄</param>
        private void AddProtoLifecycleUnregisterCallback(AspectBehaviourType behaviourType, OnProtoLifecycleProcessingHandler callback)
        {
            if (_protoLifecycleUnregisterCallbacks.ContainsKey(behaviourType))
            {
                Debugger.Warn("The proto lifecycle unregister callback for target behaviour type '{0}' was already exist, repeat added it will be override old value.", behaviourType.ToString());
                _protoLifecycleUnregisterCallbacks.Remove(behaviourType);
            }

            _protoLifecycleUnregisterCallbacks.Add(behaviourType, callback);
        }

        /// <summary>
        /// 原型对象的生命周期通知注册接口函数
        /// </summary>
        /// <param name="behaviourType">行为类型</param>
        /// <param name="proto">原型对象实例</param>
        public void RegProtoLifecycleNotification(AspectBehaviourType behaviourType, IProto proto)
        {
            if (false == _protoLifecycleRegisterCallbacks.TryGetValue(behaviourType, out OnProtoLifecycleProcessingHandler callback))
            {
                Debugger.Error("Could not found any lifecycle notfication callback with target behaviour type '{0}', register proto lifecycle notification failed.", behaviourType.ToString());
                return;
            }

            callback(proto);
        }

        /// <summary>
        /// 原型对象的生命周期通知注销接口函数
        /// </summary>
        /// <param name="proto">原型对象实例</param>
        public void UnregProtoLifecycleNotification(IProto proto)
        {
            IEnumerator<OnProtoLifecycleProcessingHandler> e = _protoLifecycleUnregisterCallbacks.Values.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current(proto);
            }
        }

        /// <summary>
        /// 在指定容器中查找属于指定实体对象的全部组件实例
        /// </summary>
        /// <param name="entity">目标实体对象</param>
        /// <param name="container">对象容器</param>
        /// <returns>返回查找的组件列表，若不存在则返回null</returns>
        private IList<CComponent> FindAllComponentsBelongingToTargetEntityFromTheContainer(CEntity entity, IList<IProto> container)
        {
            IList<CComponent> result = null;

            IEnumerator<IProto> e = container.GetEnumerator();
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
        private void RemoveAllComponentsBelongingToTargetEntityFromTheContainer(CEntity entity, IList<IProto> container)
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
        private bool TryGetProtoLifecycleProcessingCallback(SystemType targetType, AspectBehaviourType behaviourType, out OnProtoLifecycleProcessingHandler callback)
        {
            callback = null;

            foreach (KeyValuePair<SystemType, IDictionary<AspectBehaviourType, OnProtoLifecycleProcessingHandler>> pair in _protoLifecycleProcessingCallbacks)
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
        private void AddProtoLifecycleProcessingCallHandler(SystemType targetType, AspectBehaviourType behaviourType, OnProtoLifecycleProcessingHandler callback)
        {
            IDictionary<AspectBehaviourType, OnProtoLifecycleProcessingHandler> dict;
            if (false == _protoLifecycleProcessingCallbacks.TryGetValue(targetType, out dict))
            {
                dict = new Dictionary<AspectBehaviourType, OnProtoLifecycleProcessingHandler>();

                _protoLifecycleProcessingCallbacks.Add(targetType, dict);
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
