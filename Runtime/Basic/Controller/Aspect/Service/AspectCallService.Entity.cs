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

namespace GameEngine
{
    /// 提供切面访问接口的服务类
    public static partial class AspectCallService
    {
        [OnServiceProcessRegisterOfTarget(typeof(CEntity), AspectBehaviourType.Initialize)]
        private static void CallServiceProcessOfEntityInitialize(CEntity entity, bool reload)
        {
            Type targetType = entity.BeanType;
            Loader.Structuring.GeneralCodeInfo codeInfo = Loader.CodeLoader.LookupGeneralCodeInfo(targetType, typeof(CEntity));
            if (null == codeInfo)
            {
                Debugger.Warn("Could not found any aspect call entity service process with target type '{%t}', called it failed.", targetType);
                return;
            }

            Loader.Structuring.EntityCodeInfo entityCodeInfo = codeInfo as Loader.Structuring.EntityCodeInfo;
            if (null == entityCodeInfo)
            {
                Debugger.Warn("The aspect call entity service process getting error code info '{0}' with target type '{1}', called it failed.", codeInfo.GetType().FullName, targetType.FullName);
                return;
            }

            if (reload)
            {
                // 重载时无需执行该流程
                return;
            }

            string beanName = entity.GetBeanNameOrDefault();
            Loader.Symboling.Bean bean = Loader.CodeLoader.GetBeanClassByName(beanName);
            if (null == bean)
            {
                Debugger.Warn("Could not found any bean instance with target object '{0}' and name '{1}', please checked the class type resolved process.",
                        NovaEngine.Utility.Text.ToString(targetType), beanName);
                return;
            }

            IList<Loader.Symboling.BeanComponent> beanComponents = bean.Components;
            for (int n = 0; null != beanComponents && n < beanComponents.Count; ++n)
            {
                Loader.Symboling.BeanComponent beanComponent = beanComponents[n];
                if (null != beanComponent.ReferenceClassType)
                {
                    entity.AddComponent(beanComponent.ReferenceClassType);
                }
                else if (null != beanComponent.ReferenceBeanName)
                {
                    entity.AddComponent(beanComponent.ReferenceBeanName);
                }
                else
                {
                    Debugger.Warn("The entity activation component name or type must be non-null with target bean '{0}', added activation component failed.", beanName);
                }
            }
        }
    }
}
