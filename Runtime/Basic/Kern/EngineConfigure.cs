/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hurley, Independent Studio.
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
using System.Reflection;

namespace GameEngine
{
    /// <summary>
    /// 游戏层配置封装管理类
    /// </summary>
    internal static class EngineConfigure
    {
        /// <summary>
        /// 初始化平台环境及相关接口
        /// </summary>
        private static void InitPlatform()
        {
            // 引擎帧率设置
            UnityEngine.Application.targetFrameRate = NovaEngine.Environment.animationRate;

            NovaEngine.Application.Instance.AddProtocolTransformationHandler(EngineDispatcher.OnApplicationResponseCallback);

            // 日志开启
            NovaEngine.Logger.Startup();
        }

        /// <summary>
        /// 游戏配置相关初始化函数
        /// </summary>
        public static void InitGameConfig(IReadOnlyDictionary<string, string> variables)
        {
            // 加载配置
            LoadEnvironmentAndConfigureSettings(variables);

            // 重置宏参数
            RefreshMacroConfigurationParameters();

            // 初始化平台
            InitPlatform();

            // 调试器参数初始化
            Debugger.Startup();
        }

        /// <summary>
        /// 游戏配置相关清理函数
        /// </summary>
        public static void CleanupGameConfig()
        {
            // 调试器参数清理
            Debugger.Shutdown();

            // 日志关闭
            NovaEngine.Logger.Shutdown();

            NovaEngine.Application.Instance.RemoveProtocolTransformationHandler(EngineDispatcher.OnApplicationResponseCallback);
        }

        #region 配置参数加载相关接口函数

        /// <summary>
        /// 加载配置参数数据
        /// </summary>
        /// <param name="variables">环境参数集合</param>
        private static void LoadEnvironmentAndConfigureSettings(IReadOnlyDictionary<string, string> variables)
        {
            // 加载环境参数
            NovaEngine.Environment.Load(variables);

            Debugger.Log(LogGroupTag.Basic, "Environment={{{%s}}}, Configuration={{{%s}}}",
                NovaEngine.Environment.ToCString(), NovaEngine.Configuration.ToCString());
        }

        #endregion

        #region 内部的宏定义配置数据调整刷新接口函数

        /// <summary>
        /// 针对环境参数对配置数据进行复位设置的接口函数<br/>
        /// 调用该方法前，需要进行环境参数的加载，然后该方法将根据环境参数对部分属性进行调整<br/>
        /// 目的是确保在项目正式发布时，避免因人为遗漏导致部分开发参数应用到正式环境中
        /// </summary>
        private static void RefreshMacroConfigurationParameters()
        {
            // 调试模式直接返回
            if (NovaEngine.Environment.debugMode)
            {
                return;
            }

            // 正式环境中的参数调整
            // 将仅在调试模式中开启的标识关闭
            Type classType = typeof(GameMacros);
            FieldInfo[] fieldInfos = classType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            for (int n = 0; n < fieldInfos.Length; ++n)
            {
                FieldInfo fieldInfo = fieldInfos[n];
                IEnumerable<Attribute> attrs = fieldInfo.GetCustomAttributes();
                foreach (Attribute attr in attrs)
                {
                    Type attrType = attr.GetType();
                    if (typeof(EnableOnReleaseModeAttribute) == attrType)
                    {
                        // 非调试模式下，该属性标识的字段直接设置为true
                        fieldInfo.SetValue(null, true);
                    }
                    else if (typeof(DisableOnReleaseModeAttribute) == attrType)
                    {
                        // 非调试模式下，该属性标识的字段直接设置为false
                        fieldInfo.SetValue(null, false);
                    }
                    else if (typeof(AssignableOnReleaseModeAttribute) == attrType)
                    {
                        AssignableOnReleaseModeAttribute _attr = (AssignableOnReleaseModeAttribute) attr;
                        if (false == fieldInfo.FieldType.IsAssignableFrom(_attr.Value.GetType()))
                        {
                            Debugger.Error("Invalid configure value type '{0}' will be assigned to target field value type '{1}', reconfigured it failed.",
                                    NovaEngine.Utility.Text.ToString(_attr.Value.GetType()), NovaEngine.Utility.Text.ToString(fieldInfo.FieldType));
                        }
                        else
                        {
                            fieldInfo.SetValue(null, _attr.Value);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
