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

namespace GameEngine.Loader
{
    /// <summary>
    /// 程序集的分析处理类，对业务层载入的所有对象类进行统一加载及分析处理
    /// </summary>
    public static partial class CodeLoader
    {
        /// <summary>
        /// 通过指定的实体名称获取对应的实体配置数据信息
        /// </summary>
        /// <param name="beanName">实体名称</param>
        /// <returns>返回配置数据实例，若查找失败返回null</returns>
        internal static Configuring.BeanConfigureInfo GetBeanConfigureByName(string beanName)
        {
            if (string.IsNullOrEmpty(beanName))
            {
                return null;
            }

            Configuring.BaseConfigureInfo baseConfigureInfo = GetConfigureInfoByName(beanName);
            if (null != baseConfigureInfo)
            {
                return baseConfigureInfo as Configuring.BeanConfigureInfo;
            }

            return null;
        }

        /// <summary>
        /// 通过指定的对象类型获取对应的配置数据信息集合
        /// </summary>
        /// <param name="targetType">目标对象类型</param>
        /// <returns>返回配置数据实例集合，若查找失败返回null</returns>
        internal static IList<Configuring.BeanConfigureInfo> GetBeanConfigureByType(SystemType targetType)
        {
            if (null == targetType)
            {
                return null;
            }

            IList<Configuring.BeanConfigureInfo> result = null;
            IList<Configuring.BaseConfigureInfo> list = GetAllConfigureInfos();
            for (int n = 0; null != list && n < list.Count; ++n)
            {
                Configuring.BaseConfigureInfo baseConfigureInfo = list[n];
                if (Configuring.ConfigureInfoType.Bean == baseConfigureInfo.Type)
                {
                    Configuring.BeanConfigureInfo info = (Configuring.BeanConfigureInfo) baseConfigureInfo;
                    if (info.ClassType == targetType)
                    {
                        if (null == result)
                        {
                            result = new List<Configuring.BeanConfigureInfo>();
                        }

                        result.Add(info);
                    }
                }
            }

            return result;
        }
    }
}
