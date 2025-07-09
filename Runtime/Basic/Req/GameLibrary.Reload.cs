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

using SystemType = System.Type;

namespace GameEngine
{
    /// <summary>
    /// 游戏运行库的静态管理类，对业务层载入的所有运行对象类进行统一加载管理<br/>
    /// 该管理类主要通过反射实现运行对象类的初始化及清理流程中的一些模版配置管理
    /// </summary>
    public static partial class GameLibrary
    {
        /// <summary>
        /// 对当前引擎上下文中的所有对象实例进行切面服务的重载处理
        /// </summary>
        private static void ReloadAspectService()
        {
            // 场景重载
            CScene scene = SceneHandler.Instance.GetCurrentScene();
            ReloadAspectServiceOnTargetEntity(scene);

            // 角色重载
            IList<CActor> actors = ActorHandler.Instance.GetAllActors();
            for (int n = 0; n < actors.Count; ++n)
            {
                ReloadAspectServiceOnTargetEntity(actors[n]);
            }

            // 视图重载
            IList<CView> views = GuiHandler.Instance.GetAllViews();
            for (int n = 0; n < views.Count; ++n)
            {
                ReloadAspectServiceOnTargetEntity(views[n]);
            }
        }

        /// <summary>
        /// 针对指定的实体对象实例进行切面服务的重载处理
        /// </summary>
        /// <param name="entity">实体对象实例</param>
        private static void ReloadAspectServiceOnTargetEntity(CEntity entity)
        {
            // 检查原型对象是否被销毁
            if (entity.IsOnDestroyingStatus())
            {
                // 原型对象当前处于销毁状态，无需对其进行重载操作
                return;
            }

            ReloadAspectServiceOfTargetBaseProto(entity);

            // 重载组件实例
            IList<CComponent> components = entity.GetAllComponents();
            for (int n = 0; null != components && n < components.Count; ++n)
            {
                CComponent component = components[n];
                ReloadAspectServiceOfTargetBaseProto(component);
            }
        }

        /// <summary>
        /// 针对指定的原型对象进行切面服务的重载处理
        /// </summary>
        /// <param name="obj">原型对象实例</param>
        private static void ReloadAspectServiceOfTargetBaseProto(CBase obj)
        {
            // 检查原型对象是否被销毁
            if (obj.IsOnDestroyingStatus())
            {
                // 原型对象当前处于销毁状态，无需对其进行重载操作
                return;
            }

            obj.UnsubscribeAllAutomaticallyEvents();
            obj.RemoveAllAutomaticallyMessageListeners();

            System.Array lifecycleTypes = System.Enum.GetValues(typeof(CBase.LifecycleKeypointType));
            for (int n = 0; n < lifecycleTypes.Length; ++n)
            {
                CBase.LifecycleKeypointType lifecycleType = (CBase.LifecycleKeypointType) lifecycleTypes.GetValue(n);
                if (CBase.LifecycleKeypointType.Unknown == lifecycleType)
                {
                    // 跳过未定义类型
                    continue;
                }

                if (lifecycleType > obj.CurrentLifecycleRunningStep)
                {
                    // 当前原型对象的生命周期阶段遍历完成
                    break;
                }

                AspectCallService.CallServiceProcess(obj, lifecycleType.ToString(), true);
            }
        }
    }
}
