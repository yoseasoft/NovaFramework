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

namespace GameEngine
{
    /// <summary>
    /// 原型对象管理类，用于对场景上下文中的所有原型对象提供通用的访问操作接口
    /// </summary>
    internal sealed partial class BeanController
    {
        /// <summary>
        /// 原型对象销毁通知管理容器
        /// </summary>
        private IList<IBean> _beanDestroyNotificationList = null;

        /// <summary>
        /// 原型管理对象的销毁流程初始化通知接口函数
        /// </summary>
        [OnControllerSubmoduleInitCallback]
        private void OnBeanDestroyInitialize()
        {
            // 初始化销毁对象通知容器
            _beanDestroyNotificationList = new List<IBean>();
        }

        /// <summary>
        /// 原型管理对象的销毁流程清理通知接口函数
        /// </summary>
        [OnControllerSubmoduleCleanupCallback]
        private void OnBeanDestroyCleanup()
        {
            // 清理销毁对象通知容器
            _beanDestroyNotificationList.Clear();
            _beanDestroyNotificationList = null;
        }

        /// <summary>
        /// 原型管理对象的销毁流程处理接口函数
        /// </summary>
        [OnControllerSubmoduleDumpCallback]
        private void OnBeanDestroyDump()
        {
            if (_beanDestroyNotificationList.Count > 0)
            {
                OnBeanDestroyProcess();
            }
        }

        #region 原型对象销毁流程管理接口函数

        /// <summary>
        /// 原型对象的销毁通知注册接口函数
        /// </summary>
        /// <param name="bean">原型对象实例</param>
        [OnBeanLifecycleRegister(AspectBehaviourType.Destroy)]
        private void RegBeanDestroyNotification(IBean bean)
        {
            if (null == bean)
            {
                Debugger.Error("The register destroy notification bean object must be non-null.");
                return;
            }

            if (_beanDestroyNotificationList.Contains(bean))
            {
                Debugger.Warn("The register destroy notification bean object '{%t}' was already exist, repeat added it failed.", bean.GetType());
                return;
            }

            // 撤销其它通知
            OnBeanDestroyLifecycleNotifyPostProcess(bean);

            _beanDestroyNotificationList.Add(bean);
        }

        /// <summary>
        /// 当原型对象触发了销毁通知时，其它所有通知均可撤销
        /// </summary>
        /// <param name="bean">原型对象实例</param>
        private void OnBeanDestroyLifecycleNotifyPostProcess(IBean bean)
        {
            foreach (KeyValuePair<AspectBehaviourType, OnBeanLifecycleProcessingHandler> pair in _beanLifecycleUnregisterCallbacks)
            {
                if (AspectBehaviourType.Destroy == pair.Key)
                {
                    continue;
                }

                pair.Value(bean);
            }
        }

        /// <summary>
        /// 原型对象的销毁通知注销接口函数
        /// </summary>
        /// <param name="bean">原型对象实例</param>
        [OnBeanLifecycleUnregister(AspectBehaviourType.Destroy)]
        private void UnregBeanDestroyNotification(IBean bean)
        {
            if (null == bean)
            {
                Debugger.Error("The unregister destroy notification bean object must be non-null.");
                return;
            }

            if (_beanDestroyNotificationList.Contains(bean))
            {
                _beanDestroyNotificationList.Remove(bean);
            }
        }

        /// <summary>
        /// 原型对象的销毁操作处理接口函数
        /// </summary>
        private void OnBeanDestroyProcess()
        {
            while (_beanDestroyNotificationList.Count > 0)
            {
                IBean bean = _beanDestroyNotificationList[0];

                // 先从队列中移除目标对象
                _beanDestroyNotificationList.Remove(bean);

                OnBeanLifecycleProcessingHandler callback;
                if (false == TryGetBeanLifecycleProcessingCallback(bean.GetType(), AspectBehaviourType.Destroy, out callback))
                {
                    Debugger.Error("Could not found any bean destroy processing callback with target type '{%t}', calling destroy process failed.", bean.GetType());
                    continue;
                }

                callback(bean);
            }
        }

        [OnBeanLifecycleProcessRegisterOfTarget(typeof(CObject), AspectBehaviourType.Destroy)]
        private void OnObjectDestroyProcess(IBean bean)
        {
            CObject obj = bean as CObject;
            Debugger.Assert(null != obj, "Invalid arguments.");

            ObjectHandler.Instance.RemoveObject(obj);
        }

        [OnBeanLifecycleProcessRegisterOfTarget(typeof(CScene), AspectBehaviourType.Destroy)]
        private void OnSceneDestroyProcess(IBean bean)
        {
            throw new System.NotImplementedException();
        }

        [OnBeanLifecycleProcessRegisterOfTarget(typeof(CActor), AspectBehaviourType.Destroy)]
        private void OnActorDestroyProcess(IBean bean)
        {
            CActor actor = bean as CActor;
            Debugger.Assert(null != actor, "Invalid arguments.");

            RemoveAllComponentsBelongingToTargetEntityFromTheContainer(actor, _beanDestroyNotificationList);

            ActorHandler.Instance.RemoveActor(actor);
        }

        [OnBeanLifecycleProcessRegisterOfTarget(typeof(CView), AspectBehaviourType.Destroy)]
        private void OnViewDestroyProcess(IBean bean)
        {
            CView view = bean as CView;
            Debugger.Assert(null != view, "Invalid arguments.");

            RemoveAllComponentsBelongingToTargetEntityFromTheContainer(view, _beanDestroyNotificationList);

            GuiHandler.Instance.RemoveUI(view);
        }

        [OnBeanLifecycleProcessRegisterOfTarget(typeof(CComponent), AspectBehaviourType.Destroy)]
        private void OnComponentDestroyProcess(IBean bean)
        {
            CComponent component = bean as CComponent;
            Debugger.Assert(null != component && null != component.Entity, "Invalid arguments.");

            // Entity对象在BeforeDestroy阶段移除的组件对象实例，组件的Destroy通知会多调用一次
            if (false == component.IsOnAwakingStatus() || component.IsOnDestroyingStatus())
            {
                Debugger.Info(LogGroupTag.Controller, "The component '{%t}' was already entry destroying status, repeat destroyed component failed.", component.BeanType);
                return;
            }

            component.Entity.RemoveComponent(component);
        }

        #endregion
    }
}
