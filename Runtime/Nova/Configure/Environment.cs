/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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
using SystemStringBuilder = System.Text.StringBuilder;
using SystemBindingFlags = System.Reflection.BindingFlags;
using SystemFieldInfo = System.Reflection.FieldInfo;

namespace NovaEngine
{
    /// <summary>
    /// 基础环境属性定义类，对当前引擎运行所需的环境成员属性进行设置及管理
    /// </summary>
    public static partial class Environment
    {
        /// <summary>
        /// 编辑器模式，用于项目资源调试<br/>
        /// 在编辑器模式下，资源访问路径可以允许优先从Resource目录读取，若读取失败则从打包目录下读取，并且引擎默认选用编辑器配置参数<br/>
        /// 非编辑器模式直接从打包目录读取，不考虑Resource目录，并且引擎默认选用本地配置文件设置参数<br/>
        /// </summary>
        public static readonly bool editorMode = false;

        /// <summary>
        /// 调试模式，用于项目内部测试<br/>
        /// 在调试模式下，默认打开全部日志输入级别，同时开放全部第三方调试插件<br/>
        /// 非调试模式下所有第三方调试插件将全部关闭<br/>
        /// </summary>
        public static readonly bool debugMode = false;

        /// <summary>
        /// 调试级别，用于项目内部测试<br/>
        /// 若关闭调试模式，该属性将没有任何效果<br/>
        /// 仅在调试模式下，程序将根据级别参数进行对应的调试输出<br/>
        /// <br/>
        /// 引擎调试级别参数宏，具体参数定义如下：<br/>
        /// - 0：提供调试，普通，警告，错误和致命级别的日志追踪输出<br/>
        /// - 1：提供警告，错误和致命级别的日志追踪输出，调试和普通级别的日志标准输出<br/>
        /// - 2：提供警告，错误和致命级别的日志追踪输出，普通级别的日志标准输出<br/>
        /// - 3：提供错误和致命级别的日志追踪输出，警告级别的日志标准输出<br/>
        /// - 4：仅提供错误和致命级别的日志标准输出<br/>
        /// </summary>
        public static readonly int debugLevel = 0;

        /// <summary>
        /// 加密模式，项目发布阶段打开该选项，打包加密资源以避免数据泄露<br/>
        /// 在加密模式下，对本地资源及网络协议进行加密/解密处理（Resource目录资源除外）<br/>
        /// 在非加密模式下，所有资源及协议均直接访问，不考虑加密情况<br/>
        /// </summary>
        public static readonly bool cryptMode = false;

        /// <summary>
        /// 更新模式，项目发布阶段打开该选项，以便后期的更新维护，若本地代码开启了链接模式，则将同步更新代码<br/>
        /// 在更新模式下，对本地资源进行版本检测及更新<br/>
        /// 在非更新模式下，则直接装载本地资源，跳过更新检测环节<br/>
        /// </summary>
        public static readonly bool updateMode = false;

        /// <summary>
        /// 动态链接模式，项目发布阶段打开该选项，代码将以DLL形式进行打包，需要注意的是，更新模式下必须开启该模式<br/>
        /// 在链接模式下，对本地代码将分别把不同模块以DLL形式进行编译<br/>
        /// 在非链接模式下，所有代码被统一打包进本地代码中<br/>
        /// </summary>
        public static readonly bool dylinkMode = false;

        /// <summary>
        /// 程序名称，此处可设置为标识，通过本地化文件显示程序实际别名
        /// </summary>
        public static readonly string applicationName = "unknown";

        /// <summary>
        /// 程序编码，对应程序名称在应用平台上的唯一标识
        /// </summary>
        public static readonly int applicationCode = 0;

        /// <summary>
        /// 全局环境参数映射表
        /// </summary>
        private static readonly IDictionary<string, string> _variables = new Dictionary<string, string>();

        /// <summary>
        /// 设置环境成员属性的值，通过查找与指定字符串相匹配的成员属性设定其对应值
        /// </summary>
        /// <param name="fieldName">属性名称</param>
        /// <param name="fieldValue">属性值</param>
        public static void SetProperty(string fieldName, object fieldValue)
        {
            SystemType type = typeof(Environment);
            SystemFieldInfo field = type.GetField(fieldName, SystemBindingFlags.Static | SystemBindingFlags.Public | SystemBindingFlags.NonPublic);
            if (null == field)
            {
                Logger.Error("Could not found Environment field name '{%s}', set target property value failed.", fieldName);
                return;
            }

            field.SetValue(null, fieldValue);
        }

        /// <summary>
        /// 获取环境成员属性的值，通过查找与指定字符串相匹配的成员属性获取其对应值
        /// </summary>
        /// <param name="fieldName">属性名称</param>
        /// <returns>获取属性名称的对应值</returns>
        public static object GetProperty(string fieldName)
        {
            SystemType type = typeof(Environment);
            SystemFieldInfo field = type.GetField(fieldName, SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Static);
            if (null == field)
            {
                Logger.Error("Could not found Environment field name '{%s}', get target property value failed.", fieldName);
                return null;
            }

            return field.GetValue(null);
        }

        /// <summary>
        /// 检查当前环境成员属性是否有字段与目标串相匹配的名称
        /// </summary>
        /// <param name="fieldName">属性名称</param>
        /// <returns>若存在目标字段名称则返回true，否则返回false</returns>
        public static bool IsPropertyExists(string fieldName)
        {
            SystemType type = typeof(Environment);
            SystemFieldInfo field = type.GetField(fieldName, SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Static);
            return (null != field);
        }

        /// <summary>
        /// 清掉全局环境参数
        /// </summary>
        public static void CleanupAllVariables()
        {
            _variables.Clear();
        }

        /// <summary>
        /// 设置全局环境参数，通过指定键值对映射
        /// </summary>
        /// <param name="key">环境参数键</param>
        /// <param name="value">环境参数值</param>
        public static void SetVariable(string key, string value)
        {
            if (_variables.ContainsKey(key))
            {
                Logger.Info("The environment variable key {%s} was already exists, repeat setting it will be override old value.", key);

                _variables[key] = value;
            }
            else
            {
                _variables.Add(key, value);
            }

            // 每次设置变量时，同步赋值到配置类中
            Configuration.SetProperty(key, value);
        }

        /// <summary>
        /// 通过指定键名获取对应的环境参数值
        /// </summary>
        /// <param name="key">环境参数键</param>
        /// <returns>若存在指定环境参数则返回对应值，否则返回null</returns>
        public static string GetVariable(string key)
        {
            if (_variables.ContainsKey(key))
            {
                return _variables[key];
            }

            return null;
        }

        /// <summary>
        /// 设置环境参数，通过指定键值对映射
        /// 若参数键存在对应的属性，则进行属性设置，具体函数可参考<see cref="NovaEngine.Environment.SetProperty(string,object)"/>
        /// 否则添加到全局参数表中，具体函数可参考<see cref="NovaEngine.Environment.SetVariable(string,string)"/>
        /// </summary>
        /// <param name="key">环境参数键</param>
        /// <param name="value">环境参数值</param>
        public static void SetValue(string key, object value)
        {
            if (IsPropertyExists(key))
            {
                SetProperty(key, value);
            }
            else
            {
                SetVariable(key, value.ToString());
            }
        }

        /// <summary>
        /// 通过指定键名获取对应的环境参数值
        /// 该方法将优先搜索对象属性，若存在同名属性则返回属性值
        /// 若同名属性查找失败，则搜索全局参数表，具体函数可参考<see cref="NovaEngine.Environment.GetVariable(string)"/>
        /// </summary>
        /// <param name="key">环境参数键</param>
        /// <returns>若存在指定环境参数则返回对应值，否则返回null</returns>
        public static string GetValue(string key)
        {
            if (IsPropertyExists(key))
            {
                // 此处应有非空检查
                return GetProperty(key).ToString();
            }
            else
            {
                return GetVariable(key);
            }
        }

        /// <summary>
        /// 以文件的形式加载配置参数
        /// </summary>
        /// <param name="filename">文件名称</param>
        /// <returns>从指定文件中加载配置参数成功返回true，否则返回false</returns>
        public static bool LoadFromFile(string filename)
        {
            string text = Utility.Path.LoadTextAsset(filename);
            if (null == text)
            {
                return false;
            }

            return LoadFromText(text);
        }

        /// <summary>
        /// 以文本数据的形式加载配置参数
        /// </summary>
        /// <param name="textAsset">文本字符串</param>
        /// <returns>从指定文本数据中加载配置参数成功返回true，否则返回false</returns>
        public static bool LoadFromText(string textAsset)
        {
            IO.IniFile file = IO.IniFile.Create();
            if (false == file.LoadFromText(textAsset))
            {
                file.Close();
                return false;
            }

            IReadOnlyDictionary<string, string> conf = file.GetSectionValue();

            file.Close();

            return Load(conf);
        }

        /// <summary>
        /// 从字典数据中加载配置参数
        /// </summary>
        /// <param name="conf">字典数据实例</param>
        /// <returns>从指定字典数据中加载配置参数成功返回true，否则返回false</returns>
        public static bool Load(IReadOnlyDictionary<string, string> conf)
        {
            SystemType type = typeof(Environment);
            foreach (KeyValuePair<string, string> pair in conf)
            {
                SystemFieldInfo field = type.GetField(pair.Key, SystemBindingFlags.Static | SystemBindingFlags.Public | SystemBindingFlags.NonPublic);
                if (null == field)
                {
                    // 非预定义属性直接放入全局配置参数表中
                    SetVariable(pair.Key, pair.Value);
                }
                else
                {
                    object value = Utility.Convertion.StringToTargetType(pair.Value, field.FieldType);

                    // 非预定义属性类型，暂时放在全局环境中备用
                    if (null == value)
                    {
                        Logger.Warn("The Environment property '{%s}' field type '{%f}' parse failed.", field.Name, field.FieldType);
                        SetVariable(pair.Key, pair.Value);
                    }
                    else
                    {
                        SetProperty(pair.Key, value);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 重置当前设定的全部环境参数
        /// </summary>
        public static void Unload()
        {
            SetProperty(nameof(editorMode), false);
            SetProperty(nameof(debugMode), false);
            SetProperty(nameof(debugLevel), 0);
            SetProperty(nameof(cryptMode), false);
            SetProperty(nameof(updateMode), false);
            SetProperty(nameof(dylinkMode), false);
            SetProperty(nameof(applicationName), "unknown");
            SetProperty(nameof(applicationCode), 0);

            CleanupAllVariables();
        }

        public static string PrintString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("Environment = { PROPERTIES = { ");
            SystemFieldInfo[] fields = typeof(Environment).GetFields(SystemBindingFlags.Static | SystemBindingFlags.Public | SystemBindingFlags.NonPublic);
            for (int n = 0; n < fields.Length; ++n)
            {
                SystemFieldInfo field = fields[n];
                sb.AppendFormat("{0} = {1}, ", field.Name, field.GetValue(null));
            }

            sb.Append("}, VARIABLES = { ");
            foreach (KeyValuePair<string, string> pair in _variables)
            {
                sb.AppendFormat("{0} = {1}, ", pair.Key, pair.Value);
            }

            sb.Append("}, ");

            // 打印环境配置同时，后面加入版本信息
            sb.AppendFormat("FRAMEWORK_VERSION = {0}, ", Version.FrameworkVersionName());
            sb.AppendFormat("APPLICATION_VERSION = {0} ", Version.ApplicationVersionName());

            sb.Append("}");
            return sb.ToString();
        }
    }
}
