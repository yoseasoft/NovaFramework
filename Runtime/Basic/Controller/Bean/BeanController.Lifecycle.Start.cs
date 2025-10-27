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
        /// 原型对象开启通知管理容器
        /// </summary>
        private IList<IBean> _beanStartNotificationList = null;

        /// <summary>
        /// 原型管理对象的开启流程初始化通知接口函数
        /// </summary>
        [OnControllerSubmoduleInitCallback]
        private void OnBeanStartInitialize()
        {
            // 初始化开启对象通知容器
            _beanStartNotificationList = new List<IBean>();
        }

        /// <summary>
        /// 原型管理对象的开启流程清理通知接口函数
        /// </summary>
        [OnControllerSubmoduleCleanupCallback]
        private void OnBeanStartCleanup()
        {
            // 清理开启对象通知容器
            _beanStartNotificationList.Clear();
            _beanStartNotificationList = null;
        }

        /// <summary>
        /// 原型管理对象的开启流程处理接口函数
        /// </summary>
        [OnControllerSubmoduleUpdateCallback]
        private void OnBeanStartUpdate()
        {
            if (_beanStartNotificationList.Count > 0)
            {
                OnBeanStartProcess();
            }
        }

        #region 原型对象开启流程管理接口函数

        /// <summary>
        /// 原型对象的开启通知注册接口函数
        /// </summary>
        /// <param name="bean">原型对象实例</param>
        [OnBeanLifecycleRegister(AspectBehaviourType.Start)]
        private void RegBeanStartNotification(IBean bean)
        {
            if (null == bean)
            {
                Debugger.Error("The register start notification bean object must be non-null.");
                return;
            }

            if (_beanStartNotificationList.Contains(bean))
            {
                Debugger.Warn("The register start notification bean object '{%t}' was already exist, repeat added it failed.", bean.GetType());
                return;
            }

            _beanStartNotificationList.Add(bean);
        }

        /// <summary>
        /// 原型对象的开启通知注销接口函数
        /// </summary>
        /// <param name="bean">原型对象实例</param>
        [OnBeanLifecycleUnregister(AspectBehaviourType.Start)]
        private void UnregBeanStartNotification(IBean bean)
        {
            if (null == bean)
            {
                Debugger.Error("The unregister start notification bean object must be non-null.");
                return;
            }

            if (_beanStartNotificationList.Contains(bean))
            {
                _beanStartNotificationList.Remove(bean);
            }
        }

        /// <summary>
        /// 原型对象的开启操作处理接口函数
        /// </summary>
        private void OnBeanStartProcess()
        {
            List<IBean> list = new List<IBean>();
            list.AddRange(_beanStartNotificationList);

            _beanStartNotificationList.Clear();

            for (int n = 0; n < list.Count; ++n)
            {
                IBean bean = list[n];

                OnBeanLifecycleProcessingHandler callback;
                if (false == TryGetBeanLifecycleProcessingCallback(bean.GetType(), AspectBehaviourType.Start, out callback))
                {
                    Debugger.Warn("Could not found any bean start processing callback with target type '{%t}', calling start process failed.", bean.GetType());
                    continue;
                }

                callback(bean);
            }
        }

        [OnBeanLifecycleProcessRegisterOfTarget(typeof(CObject), AspectBehaviourType.Start)]
        private void OnObjectStartProcess(IBean bean)
        {
            CObject obj = bean as CObject;
            Debugger.Assert(null != obj, "Invalid arguments.");

            ObjectHandler.Instance.OnObjectStartProcessing(obj);
        }

        [OnBeanLifecycleProcessRegisterOfTarget(typeof(CScene), AspectBehaviourType.Start)]
        private void OnSceneStartProcess(IBean bean)
        {
            CScene scene = bean as CScene;
            Debugger.Assert(null != scene, "Invalid arguments.");

            SceneHandler.Instance.OnEntityStartProcessing(scene);
        }

        [OnBeanLifecycleProcessRegisterOfTarget(typeof(CActor), AspectBehaviourType.Start)]
        private void OnActorStartProcess(IBean bean)
        {
            CActor actor = bean as CActor;
            Debugger.Assert(null != actor, "Invalid arguments.");

            ActorHandler.Instance.OnEntityStartProcessing(actor);
        }

        [OnBeanLifecycleProcessRegisterOfTarget(typeof(CView), AspectBehaviourType.Start)]
        private void OnViewStartProcess(IBean bean)
        {
            CView view = bean as CView;
            Debugger.Assert(null != view, "Invalid arguments.");

            GuiHandler.Instance.OnEntityStartProcessing(view);
        }

        [OnBeanLifecycleProcessRegisterOfTarget(typeof(CComponent), AspectBehaviourType.Start)]
        private void OnComponentStartProcess(IBean bean)
        {
            CComponent component = bean as CComponent;
            Debugger.Assert(null != component && null != component.Entity, "Invalid arguments.");

            if (false == component.Entity.IsOnStartingStatus())
            {
                // 宿主实体对象尚未进入开始阶段或已经进入销毁阶段，组件实例跳过开始处理逻辑
                Debugger.Warn("The component parent entity '{%t}' instance doesnot entry starting status, started component failed.", component.Entity.BeanType);
                return;
            }

            // Entity对象在BeforeStart阶段添加的组件对象实例，组件的Start通知会多调用一次
            if (component.IsOnStartingStatus() || component.IsOnDestroyingStatus())
            {
                Debugger.Info(LogGroupTag.Controller, "The component '{%t}' was already entry starting or destroying status, repeat started component failed.", component.BeanType);
                return;
            }

            // 组件必须处于合法阶段
            // Debugger.Assert(false == component.IsOnStartingStatus() && false == component.IsOnDestroyingStatus(), "Invalid component lifecycle.");

            component.Entity.OnComponentStartProcessing(component);
        }

        #endregion
    }
}
