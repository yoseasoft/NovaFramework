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
    internal sealed partial class ProtoController
    {
        /// <summary>
        /// 原型对象销毁通知管理容器
        /// </summary>
        private IList<IProto> _protoDestroyNotificationList = null;

        /// <summary>
        /// 原型管理对象的销毁流程初始化通知接口函数
        /// </summary>
        [OnControllerSubmoduleInitCallback]
        private void OnProtoDestroyInitialize()
        {
            // 初始化销毁对象通知容器
            _protoDestroyNotificationList = new List<IProto>();
        }

        /// <summary>
        /// 原型管理对象的销毁流程清理通知接口函数
        /// </summary>
        [OnControllerSubmoduleCleanupCallback]
        private void OnProtoDestroyCleanup()
        {
            // 清理销毁对象通知容器
            _protoDestroyNotificationList.Clear();
            _protoDestroyNotificationList = null;
        }

        /// <summary>
        /// 原型管理对象的销毁流程处理接口函数
        /// </summary>
        [OnControllerSubmoduleDumpCallback]
        private void OnProtoDestroyDump()
        {
            if (_protoDestroyNotificationList.Count > 0)
            {
                OnProtoDestroyProcess();
            }
        }

        #region 原型对象销毁流程管理接口函数

        /// <summary>
        /// 原型对象的销毁通知注册接口函数
        /// </summary>
        /// <param name="proto">原型对象实例</param>
        [OnProtoLifecycleRegister(AspectBehaviourType.Destroy)]
        private void RegProtoDestroyNotification(IProto proto)
        {
            if (null == proto)
            {
                Debugger.Error("The register destroy notification proto object must be non-null.");
                return;
            }

            if (_protoDestroyNotificationList.Contains(proto))
            {
                Debugger.Warn("The register destroy notification proto object '{0}' was already exist, repeat added it failed.", proto.GetType().FullName);
                return;
            }

            // 撤销其它通知
            OnProtoDestroyLifecycleNotifyPostProcess(proto);

            _protoDestroyNotificationList.Add(proto);
        }

        /// <summary>
        /// 当原型对象触发了销毁通知时，其它所有通知均可撤销
        /// </summary>
        /// <param name="proto">原型对象实例</param>
        private void OnProtoDestroyLifecycleNotifyPostProcess(IProto proto)
        {
            foreach (KeyValuePair<AspectBehaviourType, OnProtoLifecycleProcessingHandler> pair in _protoLifecycleUnregisterCallbacks)
            {
                if (AspectBehaviourType.Destroy == pair.Key)
                {
                    continue;
                }

                pair.Value(proto);
            }
        }

        /// <summary>
        /// 原型对象的销毁通知注销接口函数
        /// </summary>
        /// <param name="proto">原型对象实例</param>
        [OnProtoLifecycleUnregister(AspectBehaviourType.Destroy)]
        private void UnregProtoDestroyNotification(IProto proto)
        {
            if (null == proto)
            {
                Debugger.Error("The unregister destroy notification proto object must be non-null.");
                return;
            }

            if (_protoDestroyNotificationList.Contains(proto))
            {
                _protoDestroyNotificationList.Remove(proto);
            }
        }

        /// <summary>
        /// 原型对象的销毁操作处理接口函数
        /// </summary>
        private void OnProtoDestroyProcess()
        {
            while (_protoDestroyNotificationList.Count > 0)
            {
                IProto proto = _protoDestroyNotificationList[0];

                // 先从队列中移除目标对象
                _protoDestroyNotificationList.Remove(proto);

                OnProtoLifecycleProcessingHandler callback;
                if (false == TryGetProtoLifecycleProcessingCallback(proto.GetType(), AspectBehaviourType.Destroy, out callback))
                {
                    Debugger.Error("Could not found any proto destroy processing callback with target type '{0}', calling destroy process failed.", proto.GetType().FullName);
                    continue;
                }

                callback(proto);
            }
        }

        [OnProtoLifecycleProcessRegisterOfTarget(typeof(CObject), AspectBehaviourType.Destroy)]
        private void OnObjectDestroyProcess(IProto proto)
        {
            CObject obj = proto as CObject;
            Debugger.Assert(null != obj, "Invalid arguments.");

            ObjectHandler.Instance.RemoveObject(obj);
        }

        [OnProtoLifecycleProcessRegisterOfTarget(typeof(CScene), AspectBehaviourType.Destroy)]
        private void OnSceneDestroyProcess(IProto proto)
        {
            throw new System.NotImplementedException();
        }

        [OnProtoLifecycleProcessRegisterOfTarget(typeof(CActor), AspectBehaviourType.Destroy)]
        private void OnActorDestroyProcess(IProto proto)
        {
            CActor actor = proto as CActor;
            Debugger.Assert(null != actor, "Invalid arguments.");

            RemoveAllComponentsBelongingToTargetEntityFromTheContainer(actor, _protoDestroyNotificationList);

            ActorHandler.Instance.RemoveActor(actor);
        }

        [OnProtoLifecycleProcessRegisterOfTarget(typeof(CView), AspectBehaviourType.Destroy)]
        private void OnViewDestroyProcess(IProto proto)
        {
            CView view = proto as CView;
            Debugger.Assert(null != view, "Invalid arguments.");

            RemoveAllComponentsBelongingToTargetEntityFromTheContainer(view, _protoDestroyNotificationList);

            GuiHandler.Instance.RemoveUI(view);
        }

        [OnProtoLifecycleProcessRegisterOfTarget(typeof(CComponent), AspectBehaviourType.Destroy)]
        private void OnComponentDestroyProcess(IProto proto)
        {
            CComponent component = proto as CComponent;
            Debugger.Assert(null != component && null != component.Entity, "Invalid arguments.");

            // Entity对象在BeforeDestroy阶段移除的组件对象实例，组件的Destroy通知会多调用一次
            if (false == component.IsOnAwakingStatus() || component.IsOnDestroyingStatus())
            {
                Debugger.Info(LogGroupTag.Controller, "The component '{0}' was already entry destroying status, repeat destroyed component failed.", component.GetType().FullName);
                return;
            }

            component.Entity.RemoveComponent(component);
        }

        #endregion
    }
}
