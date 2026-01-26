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

namespace GameEngine
{
    /// 提供切面访问接口的服务类
    public static partial class AspectCallService
    {
        [OnServiceProcessRegisterOfTarget(typeof(CScene), AspectBehaviourType.Startup)]
        private static void CallServiceProcessOfSceneStartup(CScene scene, bool reload)
        {
            Type targetType = scene.BeanType;
            Loader.Structuring.GeneralCodeInfo codeInfo = Loader.CodeLoader.LookupGeneralCodeInfo(targetType, typeof(CScene));
            if (null == codeInfo)
            {
                Debugger.Warn("Could not found any aspect call scene service process with target type '{%t}', called it failed.", targetType);
                return;
            }

            Loader.Structuring.SceneCodeInfo sceneCodeInfo = codeInfo as Loader.Structuring.SceneCodeInfo;
            if (null == sceneCodeInfo)
            {
                Debugger.Warn("The aspect call scene service process getting error code info '{%t}' with target type '{%t}', called it failed.", codeInfo, targetType);
                return;
            }

            if (reload)
            {
                // 重载时无需执行该流程
                return;
            }

            for (int n = 0; n < sceneCodeInfo.GetAutoDisplayViewNamesCount(); ++n)
            {
                string viewName = sceneCodeInfo.GetAutoDisplayViewName(n);
                Debugger.Log("---------------------------------------- open UI '{%s}' with target scene '{%t}' !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", viewName, targetType);
            }
        }

        [OnServiceProcessRegisterOfTarget(typeof(CScene), AspectBehaviourType.Shutdown)]
        private static void CallServiceProcessOfSceneShutdown(CScene scene, bool reload)
        {
            if (reload)
            {
                Debugger.Error("The scene shutdown service unsupported reload processing.");
                return;
            }

            // Debugger.Log("-------------------------------------- close all ui with target scene '{0}' ???", scene.GetType().FullName);
        }
    }
}
