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
    internal sealed partial class ProtoController
    {
        /// <summary>
        /// 原型对象查找操作函数接口定义
        /// </summary>
        /// <param name="targetType">目标对象类型</param>
        private delegate IList<IProto> OnProtoLookupProcessingHandler(SystemType targetType);

        /// <summary>
        /// 原型对象查找操作服务接口注册相关函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private class OnProtoLookupProcessRegisterOfTargetAttribute : SystemAttribute
        {
            /// <summary>
            /// 匹配查找操作服务的目标对象类型
            /// </summary>
            private readonly SystemType _classType;

            public SystemType ClassType => _classType;

            public OnProtoLookupProcessRegisterOfTargetAttribute(SystemType classType)
            {
                _classType = classType;
            }
        }

        /// <summary>
        /// 原型对象查找操作处理句柄列表容器
        /// </summary>
        private IDictionary<SystemType, SystemDelegate> _protoLookupProcessingCallbacks = null;

        /// <summary>
        /// 原型管理对象的查找操作初始化通知接口函数
        /// </summary>
        [OnControllerSubmoduleInitCallback]
        private void OnProtoLookupInitialize()
        {
            // 初始化原型对象查找操作句柄列表容器
            _protoLookupProcessingCallbacks = new Dictionary<SystemType, SystemDelegate>();

            SystemType classType = typeof(ProtoController);
            SystemMethodInfo[] methods = classType.GetMethods(SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Instance);
            for (int n = 0; n < methods.Length; ++n)
            {
                SystemMethodInfo method = methods[n];
                IEnumerable<SystemAttribute> e = method.GetCustomAttributes();
                foreach (SystemAttribute attr in e)
                {
                    SystemType attrType = attr.GetType();
                    if (typeof(OnProtoLookupProcessRegisterOfTargetAttribute) == attrType)
                    {
                        Debugger.Assert(false == method.IsStatic);

                        OnProtoLookupProcessRegisterOfTargetAttribute _attr = (OnProtoLookupProcessRegisterOfTargetAttribute) attr;

                        // SystemDelegate callback = NovaEngine.Utility.Reflection.CreateGenericFuncDelegate(this, method);
                        // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                        SystemDelegate callback = NovaEngine.Utility.Reflection.CreateGenericFuncDelegateAndCheckParameterAndReturnType(this, method, null, typeof(SystemType));

                        AddProtoLookupProcessingCallHandler(_attr.ClassType, callback);
                    }
                }
            }
        }

        /// <summary>
        /// 原型管理对象的查找操作清理通知接口函数
        /// </summary>
        [OnControllerSubmoduleCleanupCallback]
        private void OnProtoLookupCleanup()
        {
            // 清理原型对象查找操作句柄列表容器
            _protoLookupProcessingCallbacks.Clear();
            _protoLookupProcessingCallbacks = null;
        }

        /// <summary>
        /// 通过指定的类型，获取该类型的全部实例<br/>
        /// 返回的实例列表中，包括了该类型及其子类的全部实例
        /// </summary>
        /// <typeparam name="T">类型标识</typeparam>
        /// <returns>返回给定类型的全部实例</returns>
        public IList<T> FindAllProtos<T>() where T : IProto
        {
            SystemType classType = typeof(T);

            return NovaEngine.Utility.Collection.CastAndToList<IProto, T>(FindAllProtos(classType));
        }

        /// <summary>
        /// 通过指定的类型，获取该类型的全部实例<br/>
        /// 返回的实例列表中，包括了该类型及其子类的全部实例
        /// </summary>
        /// <param name="classType">类型标识</param>
        /// <returns>返回给定类型的全部实例</returns>
        public IList<IProto> FindAllProtos(SystemType classType)
        {
            SystemDelegate callback;
            if (false == TryGetProtoLookupProcessingCallback(classType, out callback))
            {
                Debugger.Warn("Could not found any proto lookup processing callback with target type '{0}', calling lookup process failed.", classType.FullName);
                return null;
            }

            return callback.DynamicInvoke(classType) as IList<IProto>;
        }

        #region 原型对象查找操作注册绑定接口函数

        /// <summary>
        /// 通过指定的类型从服务处理容器中查找对应的回调句柄实例
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="callback">回调句柄</param>
        /// <returns>若查找回调句柄成功返回true，否则返回false</returns>
        private bool TryGetProtoLookupProcessingCallback(SystemType targetType, out SystemDelegate callback)
        {
            callback = null;

            foreach (KeyValuePair<SystemType, SystemDelegate> pair in _protoLookupProcessingCallbacks)
            {
                if (pair.Key.IsAssignableFrom(targetType))
                {
                    callback = pair.Value;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 新增指定类型和函数名称对应的服务处理回调句柄
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="callback">回调句柄</param>
        private void AddProtoLookupProcessingCallHandler(SystemType targetType, SystemDelegate callback)
        {
            if (_protoLookupProcessingCallbacks.ContainsKey(targetType))
            {
                Debugger.Warn("The callback '{0}' was already exists for target type '{1}', repeated add it will be override old handler.",
                        NovaEngine.Utility.Text.ToString(callback), targetType.FullName);

                _protoLookupProcessingCallbacks.Remove(targetType);
            }

            _protoLookupProcessingCallbacks.Add(targetType, callback);
        }

        #endregion

        #region 原型对象查找操作访问相关接口函数

        /// <summary>
        /// 获取当前上下文中所有实体对象的实例引用<br/>
        /// 返回的实例列表中，包括了该类型及其子类的全部实例
        /// </summary>
        /// <returns>返回实体类型的全部实例</returns>
        private IList<CEntity> FindAllEntities()
        {
            SystemType entityType = typeof(CEntity);
            List<CEntity> entities = new List<CEntity>();

            foreach (KeyValuePair<SystemType, SystemDelegate> pair in _protoLookupProcessingCallbacks)
            {
                if (entityType.IsAssignableFrom(pair.Key))
                {
                    IList<IProto> list = pair.Value.DynamicInvoke(entityType) as IList<IProto>;
                    if (null != list && list.Count > 0)
                    {
                        entities.AddRange(NovaEngine.Utility.Collection.CastAndToList<IProto, CEntity>(list));
                    }
                }
            }

            return entities;
        }

        /// <summary>
        /// 通过指定的场景类型，获取该类型的全部实例<br/>
        /// 返回的实例列表中，包括了该类型及其子类的全部场景实例
        /// </summary>
        /// <param name="classType">场景类型</param>
        /// <returns>返回给定类型的全部实例</returns>
        [OnProtoLookupProcessRegisterOfTarget(typeof(CScene))]
        private IList<IProto> FindAllScenesByType(SystemType classType)
        {
            SceneHandler handler = SceneHandler.Instance;
            CScene currentScene = handler.GetCurrentScene();
            if (null != currentScene && classType.IsAssignableFrom(currentScene.GetType()))
            {
                IList<IProto> result = new List<IProto>(1);
                result.Add(currentScene);
                return result;
            }

            return null;
        }

        /// <summary>
        /// 通过指定的对象类型，获取该类型的全部实例<br/>
        /// 返回的实例列表中，包括了该类型及其子类的全部对象实例
        /// </summary>
        /// <param name="classType">对象类型</param>
        /// <returns>返回给定类型的全部实例</returns>
        [OnProtoLookupProcessRegisterOfTarget(typeof(CActor))]
        private IList<IProto> FindAllActorsByType(SystemType classType)
        {
            ActorHandler handler = ActorHandler.Instance;
            IList<CActor> list = handler.FindAllActorsByType(classType);
            return NovaEngine.Utility.Collection.CastAndToList<CActor, IProto>(list);
        }

        /// <summary>
        /// 通过指定的视图类型，获取该类型的全部实例<br/>
        /// 返回的实例列表中，包括了该类型及其子类的全部视图实例
        /// </summary>
        /// <param name="classType">视图类型</param>
        /// <returns>返回给定类型的全部实例</returns>
        [OnProtoLookupProcessRegisterOfTarget(typeof(CView))]
        private IList<IProto> FindAllViewsByType(SystemType classType)
        {
            GuiHandler handler = GuiHandler.Instance;
            IList<CView> list = handler.FindAllViewsByType(classType);
            return NovaEngine.Utility.Collection.CastAndToList<CView, IProto>(list);
        }

        /// <summary>
        /// 通过指定的组件类型，获取该类型的全部实例<br/>
        /// 返回的实例列表中，包括了该类型及其子类的全部组件实例
        /// </summary>
        /// <param name="classType">组件类型</param>
        /// <returns>返回给定类型的全部实例</returns>
        [OnProtoLookupProcessRegisterOfTarget(typeof(CComponent))]
        private IList<IProto> FindAllComponentsByType(SystemType classType)
        {
            IList<CEntity> entities = FindAllEntities();
            IList<IProto> components = new List<IProto>();

            IEnumerator<CEntity> e = entities.GetEnumerator();
            while (e.MoveNext())
            {
                CComponent c = e.Current.GetComponent(classType);
                if (null != c)
                {
                    components.Add(c);
                }
            }

            // 如果搜索结果为空，则直接返回null
            if (components.Count <= 0)
            {
                components = null;
            }

            return components;
        }

        #endregion
    }
}
