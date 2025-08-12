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
        /// 原型对象开启通知管理容器
        /// </summary>
        private IList<IProto> _protoStartNotificationList = null;

        /// <summary>
        /// 原型管理对象的开启流程初始化通知接口函数
        /// </summary>
        [OnControllerSubmoduleInitCallback]
        private void OnProtoStartInitialize()
        {
            // 初始化开启对象通知容器
            _protoStartNotificationList = new List<IProto>();
        }

        /// <summary>
        /// 原型管理对象的开启流程清理通知接口函数
        /// </summary>
        [OnControllerSubmoduleCleanupCallback]
        private void OnProtoStartCleanup()
        {
            // 清理开启对象通知容器
            _protoStartNotificationList.Clear();
            _protoStartNotificationList = null;
        }

        /// <summary>
        /// 原型管理对象的开启流程处理接口函数
        /// </summary>
        [OnControllerSubmoduleUpdateCallback]
        private void OnProtoStartUpdate()
        {
            if (_protoStartNotificationList.Count > 0)
            {
                OnProtoStartProcess();
            }
        }

        #region 原型对象开启流程管理接口函数

        /// <summary>
        /// 原型对象的开启通知注册接口函数
        /// </summary>
        /// <param name="proto">原型对象实例</param>
        [OnProtoLifecycleRegister(AspectBehaviourType.Start)]
        private void RegProtoStartNotification(IProto proto)
        {
            if (null == proto)
            {
                Debugger.Error("The register start notification proto object must be non-null.");
                return;
            }

            if (_protoStartNotificationList.Contains(proto))
            {
                Debugger.Warn("The register start notification proto object '{0}' was already exist, repeat added it failed.", proto.GetType().FullName);
                return;
            }

            _protoStartNotificationList.Add(proto);
        }

        /// <summary>
        /// 原型对象的开启通知注销接口函数
        /// </summary>
        /// <param name="proto">原型对象实例</param>
        [OnProtoLifecycleUnregister(AspectBehaviourType.Start)]
        private void UnregProtoStartNotification(IProto proto)
        {
            if (null == proto)
            {
                Debugger.Error("The unregister start notification proto object must be non-null.");
                return;
            }

            if (_protoStartNotificationList.Contains(proto))
            {
                _protoStartNotificationList.Remove(proto);
            }
        }

        /// <summary>
        /// 原型对象的开启操作处理接口函数
        /// </summary>
        private void OnProtoStartProcess()
        {
            List<IProto> list = new List<IProto>();
            list.AddRange(_protoStartNotificationList);

            _protoStartNotificationList.Clear();

            for (int n = 0; n < list.Count; ++n)
            {
                IProto proto = list[n];

                OnProtoLifecycleProcessingHandler callback;
                if (false == TryGetProtoLifecycleProcessingCallback(proto.GetType(), AspectBehaviourType.Start, out callback))
                {
                    Debugger.Warn("Could not found any proto start processing callback with target type '{0}', calling start process failed.", proto.GetType().FullName);
                    continue;
                }

                callback(proto);
            }
        }

        [OnProtoLifecycleProcessRegisterOfTarget(typeof(CObject), AspectBehaviourType.Start)]
        private void OnObjectStartProcess(IProto proto)
        {
            CObject obj = proto as CObject;
            Debugger.Assert(null != obj, "Invalid arguments.");

            ObjectHandler.Instance.OnObjectStartProcessing(obj);
        }

        [OnProtoLifecycleProcessRegisterOfTarget(typeof(CScene), AspectBehaviourType.Start)]
        private void OnSceneStartProcess(IProto proto)
        {
            CScene scene = proto as CScene;
            Debugger.Assert(null != scene, "Invalid arguments.");

            SceneHandler.Instance.OnEntityStartProcessing(scene);
        }

        [OnProtoLifecycleProcessRegisterOfTarget(typeof(CActor), AspectBehaviourType.Start)]
        private void OnActorStartProcess(IProto proto)
        {
            CActor actor = proto as CActor;
            Debugger.Assert(null != actor, "Invalid arguments.");

            ActorHandler.Instance.OnEntityStartProcessing(actor);
        }

        [OnProtoLifecycleProcessRegisterOfTarget(typeof(CView), AspectBehaviourType.Start)]
        private void OnViewStartProcess(IProto proto)
        {
            CView view = proto as CView;
            Debugger.Assert(null != view, "Invalid arguments.");

            GuiHandler.Instance.OnEntityStartProcessing(view);
        }

        [OnProtoLifecycleProcessRegisterOfTarget(typeof(CComponent), AspectBehaviourType.Start)]
        private void OnComponentStartProcess(IProto proto)
        {
            CComponent component = proto as CComponent;
            Debugger.Assert(null != component && null != component.Entity, "Invalid arguments.");

            if (false == component.Entity.IsOnStartingStatus())
            {
                // 宿主实体对象尚未进入开始阶段或已经进入销毁阶段，组件实例跳过开始处理逻辑
                Debugger.Warn("The component parent entity '{0}' instance doesnot entry starting status, started component failed.", component.Entity.GetType().FullName);
                return;
            }

            // Entity对象在BeforeStart阶段添加的组件对象实例，组件的Start通知会多调用一次
            if (component.IsOnStartingStatus() || component.IsOnDestroyingStatus())
            {
                Debugger.Info(LogGroupTag.Controller, "The component '{0}' was already entry starting or destroying status, repeat started component failed.", component.GetType().FullName);
                return;
            }

            // 组件必须处于合法阶段
            // Debugger.Assert(false == component.IsOnStartingStatus() && false == component.IsOnDestroyingStatus(), "Invalid component lifecycle.");

            component.Entity.OnComponentStartProcessing(component);
        }

        #endregion
    }
}
