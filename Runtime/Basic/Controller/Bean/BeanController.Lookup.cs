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

using System;
using System.Collections.Generic;
using System.Customize.Extension;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace GameEngine
{
    /// 原型对象管理类
    internal sealed partial class BeanController
    {
        /// <summary>
        /// 原型对象查找操作函数接口定义
        /// </summary>
        /// <param name="targetType">目标对象类型</param>
        private delegate IList<IBean> OnBeanLookupProcessingHandler(Type targetType);

        /// <summary>
        /// 原型对象查找操作服务接口注册相关函数的属性定义
        /// </summary>
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private class OnBeanLookupProcessRegisterOfTargetAttribute : Attribute
        {
            /// <summary>
            /// 匹配查找操作服务的目标对象类型
            /// </summary>
            private readonly Type _classType;

            public Type ClassType => _classType;

            public OnBeanLookupProcessRegisterOfTargetAttribute(Type classType)
            {
                _classType = classType;
            }
        }

        /// <summary>
        /// 原型对象查找操作处理句柄列表容器
        /// </summary>
        private IDictionary<Type, Delegate> _beanLookupProcessingCallbacks;

        /// <summary>
        /// 原型管理对象的查找操作初始化通知接口函数
        /// </summary>
        [Preserve]
        [OnControllerSubmoduleInitCallback]
        private void OnBeanLookupInitialize()
        {
            // 初始化原型对象查找操作句柄列表容器
            _beanLookupProcessingCallbacks = new Dictionary<Type, Delegate>();

            Type classType = typeof(BeanController);
            MethodInfo[] methods = classType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for (int n = 0; n < methods.Length; ++n)
            {
                MethodInfo method = methods[n];
                IEnumerable<Attribute> e = method.GetCustomAttributes();
                foreach (Attribute attr in e)
                {
                    Type attrType = attr.GetType();
                    if (typeof(OnBeanLookupProcessRegisterOfTargetAttribute) == attrType)
                    {
                        Debugger.Assert(false == method.IsStatic);

                        OnBeanLookupProcessRegisterOfTargetAttribute _attr = (OnBeanLookupProcessRegisterOfTargetAttribute) attr;

                        // Delegate callback = NovaEngine.Utility.Reflection.CreateGenericFuncDelegate(this, method);
                        // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                        Delegate callback = NovaEngine.Utility.Reflection.CreateGenericFuncDelegateAndCheckParameterAndReturnType(this, method, null, typeof(Type));

                        AddBeanLookupProcessingCallHandler(_attr.ClassType, callback);
                    }
                }
            }
        }

        /// <summary>
        /// 原型管理对象的查找操作清理通知接口函数
        /// </summary>
        [Preserve]
        [OnControllerSubmoduleCleanupCallback]
        private void OnBeanLookupCleanup()
        {
            // 清理原型对象查找操作句柄列表容器
            _beanLookupProcessingCallbacks.Clear();
            _beanLookupProcessingCallbacks = null;
        }

        /// <summary>
        /// 通过指定的类型，获取该类型的全部实例<br/>
        /// 返回的实例列表中，包括了该类型及其子类的全部实例
        /// </summary>
        /// <typeparam name="T">类型标识</typeparam>
        /// <returns>返回给定类型的全部实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyList<T> FindAllBeans<T>() where T : IBean
        {
            return NovaEngine.Utility.Collection.CastAndToReadOnlyList<IBean, T>(FindAllBeans(typeof(T)));
        }

        /// <summary>
        /// 通过指定的类型，获取该类型的全部实例<br/>
        /// 返回的实例列表中，包括了该类型及其子类的全部实例
        /// </summary>
        /// <param name="classType">类型标识</param>
        /// <returns>返回给定类型的全部实例</returns>
        public IReadOnlyList<IBean> FindAllBeans(Type classType)
        {
            if (false == TryGetBeanLookupProcessingCallback(classType, out Delegate callback))
            {
                Debugger.Warn(LogGroupTag.Controller, "Could not found any bean lookup processing callback with target type '{%t}', calling lookup process failed.", classType);
                return null;
            }

            return callback.DynamicInvoke(classType) as IReadOnlyList<IBean>;
        }

        #region 原型对象查找操作注册绑定接口函数

        /// <summary>
        /// 通过指定的类型从服务处理容器中查找对应的回调句柄实例
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="callback">回调句柄</param>
        /// <returns>若查找回调句柄成功返回true，否则返回false</returns>
        private bool TryGetBeanLookupProcessingCallback(Type targetType, out Delegate callback)
        {
            callback = null;

            foreach (KeyValuePair<Type, Delegate> pair in _beanLookupProcessingCallbacks)
            {
                if (targetType.Is(pair.Key))
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
        private void AddBeanLookupProcessingCallHandler(Type targetType, Delegate callback)
        {
            if (_beanLookupProcessingCallbacks.ContainsKey(targetType))
            {
                Debugger.Warn(LogGroupTag.Controller, "The callback '{%t}' was already exists for target type '{%t}', repeated add it will be override old handler.",
                        callback, targetType);

                _beanLookupProcessingCallbacks.Remove(targetType);
            }

            _beanLookupProcessingCallbacks.Add(targetType, callback);
        }

        #endregion

        #region 原型对象查找操作访问相关接口函数

        /// <summary>
        /// 获取当前上下文中所有实体对象的实例引用<br/>
        /// 返回的实例列表中，包括了该类型及其子类的全部实例
        /// </summary>
        /// <returns>返回实体类型的全部实例</returns>
        private IReadOnlyList<CEntity> FindAllEntities()
        {
            Type entityType = typeof(CEntity);
            List<CEntity> entities = new List<CEntity>();

            foreach (KeyValuePair<Type, Delegate> pair in _beanLookupProcessingCallbacks)
            {
                if (pair.Key.Is(entityType))
                {
                    IReadOnlyList<IBean> list = pair.Value.DynamicInvoke(entityType) as IReadOnlyList<IBean>;
                    if (null != list && list.Count > 0)
                    {
                        entities.AddRange(NovaEngine.Utility.Collection.CastAndToReadOnlyList<IBean, CEntity>(list));
                    }
                }
            }

            return entities;
        }

        /// <summary>
        /// 通过指定的对象类型，获取该类型的全部实例<br/>
        /// 返回的实例列表中，包括了该类型及其子类的全部对象实例
        /// </summary>
        /// <param name="classType">对象类型</param>
        /// <returns>返回给定类型的全部实例</returns>
        [OnBeanLookupProcessRegisterOfTarget(typeof(CObject))]
        private IReadOnlyList<IBean> FindAllObjectsByType(Type classType)
        {
            ObjectHandler handler = ObjectHandler.Instance;
            IReadOnlyList<CObject> list = handler.FindAllObjectsByType(classType);
            return NovaEngine.Utility.Collection.CastAndToReadOnlyList<CObject, IBean>(list);
        }

        /// <summary>
        /// 通过指定的场景类型，获取该类型的全部实例<br/>
        /// 返回的实例列表中，包括了该类型及其子类的全部场景实例
        /// </summary>
        /// <param name="classType">场景类型</param>
        /// <returns>返回给定类型的全部实例</returns>
        [OnBeanLookupProcessRegisterOfTarget(typeof(CScene))]
        private IList<IBean> FindAllScenesByType(Type classType)
        {
            SceneHandler handler = SceneHandler.Instance;
            CScene currentScene = handler.GetCurrentScene();
            if (null != currentScene && classType.IsAssignableFrom(currentScene.BeanType))
            {
                IList<IBean> result = new List<IBean>(1)
                {
                    currentScene,
                };
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
        [OnBeanLookupProcessRegisterOfTarget(typeof(CActor))]
        private IReadOnlyList<IBean> FindAllActorsByType(Type classType)
        {
            ActorHandler handler = ActorHandler.Instance;
            IReadOnlyList<CActor> list = handler.FindAllActorsByType(classType);
            return NovaEngine.Utility.Collection.CastAndToReadOnlyList<CActor, IBean>(list);
        }

        /// <summary>
        /// 通过指定的视图类型，获取该类型的全部实例<br/>
        /// 返回的实例列表中，包括了该类型及其子类的全部视图实例
        /// </summary>
        /// <param name="classType">视图类型</param>
        /// <returns>返回给定类型的全部实例</returns>
        [OnBeanLookupProcessRegisterOfTarget(typeof(CView))]
        private IReadOnlyList<IBean> FindAllViewsByType(Type classType)
        {
            GuiHandler handler = GuiHandler.Instance;
            IReadOnlyList<CView> list = handler.FindAllViewsByType(classType);
            return NovaEngine.Utility.Collection.CastAndToReadOnlyList<CView, IBean>(list);
        }

        /// <summary>
        /// 通过指定的组件类型，获取该类型的全部实例<br/>
        /// 返回的实例列表中，包括了该类型及其子类的全部组件实例
        /// </summary>
        /// <param name="classType">组件类型</param>
        /// <returns>返回给定类型的全部实例</returns>
        [OnBeanLookupProcessRegisterOfTarget(typeof(CComponent))]
        private IReadOnlyList<IBean> FindAllComponentsByType(Type classType)
        {
            IReadOnlyList<CEntity> entities = FindAllEntities();
            List<IBean> components = null;

            IEnumerator<CEntity> e = entities.GetEnumerator();
            while (e.MoveNext())
            {
                CComponent c = e.Current.GetComponent(classType);
                if (null != c)
                {
                    if (null == components) components = new List<IBean>();

                    components.Add(c);
                }
            }

            return components;
        }

        #endregion
    }
}
