/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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

using System.Text;
using System.Security.Cryptography;
using System.Customize.Extension;

using SystemGuid = System.Guid;
using SystemType = System.Type;
using SystemObject = System.Object;
using SystemArray = System.Array;
using SystemEnum = System.Enum;

namespace NovaEngine
{
    /// 实用函数集合工具类
    public static partial class Utility
    {
        /// <summary>
        /// 类型转换相关实用函数集合
        /// </summary>
        public static class Convertion
        {
            #region 通用转换接口函数

            /// <summary>
            /// 万能的基础数据类型转换接口函数
            /// 
            /// 示例：
            /// <code>
            /// 将 null 转换为目标类型，默认为该类型的默认值
            /// var result = ConvertTo<T>(null);
            /// 
            /// 从字典中获取一个值并转换
            /// var result = ConvertTo<T>(valueDict[key]);
            /// 
            /// 将字符串 "True" 转换为布尔类型
            /// var result = ConvertTo<bool>("True");
            /// 
            /// 将字符串 "123" 转换为整型
            /// var result = ConvertTo<int>("123");
            /// 
            /// 尝试将无效字符串 "test" 转换为整型，捕获异常
            /// var result = ConvertTo<int>("test");
            /// 
            /// 将整型 123 转换为字符串
            /// var result = ConvertTo<string>(123);
            /// 
            /// 将 null 转换为字符串，返回空字符串
            /// var result = ConvertTo<string>(null);
            /// </code>
            /// 
            /// </summary>
            /// <typeparam name="T">目标基础数据类型</typeparam>
            /// <param name="obj">待转换数据</param>
            /// <returns>转换后的数据，类型为T</returns>
            public static T ConvertTo<T>(SystemObject obj)
            {
                try
                {
                    // 如果传入对象是null或DBNull，并且目标类型是string，返回空字符串
                    if ((null == obj || System.DBNull.Value == obj) && (typeof(string) == typeof(T)))
                    {
                        return (T) System.Convert.ChangeType("", typeof(T));
                    }

                    // 如果传入对象已经是目标类型，直接返回
                    if (obj is T)
                    {
                        return (T) obj;
                    }

                    SystemType targetType = typeof(T);

                    // 处理Nullable<T>类型
                    if (targetType.IsGenericType)
                    {
                        SystemType genericTypeDefinition = targetType.GetGenericTypeDefinition();
                        if (typeof(System.Nullable<>) == genericTypeDefinition)
                        {
                            return (T) System.Convert.ChangeType(obj, System.Nullable.GetUnderlyingType(targetType));
                        }
                    }

                    // 进行类型转换
                    return (T) System.Convert.ChangeType(obj, typeof(T));
                }
                catch (System.InvalidCastException) // e)
                {
                    // 如果发生转换异常，尝试返回原对象的强制转换
                    return (T) obj;
                }
                catch (System.Exception e)
                {
                    // 其它异常直接抛出
                    throw e;
                }

                // 返回目标类型的默认值
                // return default(T);
            }

            #endregion

            #region 字符串转换为基础类型相关接口函数

            /// <summary>
            /// 字符串类型转换为布尔类型
            /// </summary>
            /// <param name="text">字符串内容</param>
            /// <param name="defaultValue">默认布尔值</param>
            /// <returns>返回转换结果，若转换失败则返回默认值</returns>
            public static bool StringToBool(string text, bool defaultValue = false)
            {
                if (false == bool.TryParse(text, out bool result))
                {
                    Logger.Warn("Convert string '{0}' to bool failed.", text);

                    result = defaultValue;
                }

                return result;
            }

            /// <summary>
            /// 字符串类型转换为整数类型
            /// </summary>
            /// <param name="text">字符串内容</param>
            /// <param name="defaultValue">默认整数值</param>
            /// <returns>返回转换结果，若转换失败则返回默认值</returns>
            public static int StringToInt(string text, int defaultValue = 0)
            {
                if (false == int.TryParse(text, out int result))
                {
                    Logger.Warn("Convert string '{0}' to int failed.", text);

                    result = defaultValue;
                }

                return result;
            }

            /// <summary>
            /// 字符串类型转换为长整数类型
            /// </summary>
            /// <param name="text">字符串内容</param>
            /// <param name="defaultValue">默认长整数值</param>
            /// <returns>返回转换结果，若转换失败则返回默认值</returns>
            public static long StringToLong(string text, long defaultValue = 0)
            {
                if (false == long.TryParse(text, out long result))
                {
                    Logger.Warn("Convert string '{0}' to int failed.", text);

                    result = defaultValue;
                }

                return result;
            }

            /// <summary>
            /// 字符串类型转换为浮点数类型
            /// </summary>
            /// <param name="text">字符串内容</param>
            /// <param name="defaultValue">默认浮点数值</param>
            /// <returns>返回转换结果，若转换失败则返回默认值</returns>
            public static float StringToFloat(string text, float defaultValue = 0f)
            {
                if (false == float.TryParse(text, out float result))
                {
                    Logger.Warn("Convert string '{0}' to float failed.", text);

                    result = defaultValue;
                }

                return result;
            }

            /// <summary>
            /// 字符串类型转换为双精度浮点数类型
            /// </summary>
            /// <param name="text">字符串内容</param>
            /// <param name="defaultValue">默认浮点数值</param>
            /// <returns>返回转换结果，若转换失败则返回默认值</returns>
            public static double StringToDouble(string text, double defaultValue = 0D)
            {
                if (false == double.TryParse(text, out double result))
                {
                    Logger.Warn("Convert string '{0}' to double failed.", text);

                    result = defaultValue;
                }

                return result;
            }

            /// <summary>
            /// 字符串类型转换为日期时间数据类型
            /// </summary>
            /// <param name="text">字符串内容</param>
            /// <param name="defaultValue">默认日期时间数据</param>
            /// <returns>返回转换结果，若转换失败则返回默认值</returns>
            public static System.DateTime StringToDateTime(string text, System.DateTime defaultValue = default(System.DateTime))
            {
                if (false == System.DateTime.TryParse(text, out System.DateTime result))
                {
                    Logger.Warn("Convert string '{0}' to DateTime failed.", text);

                    result = defaultValue;
                }

                return result;
            }

            /// <summary>
            /// 字符串类型转换为指定类型对应的数据
            /// </summary>
            /// <param name="text">字符串内容</param>
            /// <param name="targetType">目标数据类型</param>
            /// <returns>返回转换结果，若转换失败则返回null</returns>
            public static object StringToTargetType(string text, SystemType targetType)
            {
                if (targetType == typeof(bool))
                {
                    // 布尔类型
                    return StringToBool(text);
                }
                else if (targetType == typeof(int))
                {
                    // 整型
                    return StringToInt(text);
                }
                else if (targetType == typeof(long))
                {
                    // 长整型
                    return StringToLong(text);
                }
                else if (targetType == typeof(float))
                {
                    // 单精度浮点类型
                    return StringToFloat(text);
                }
                else if (targetType == typeof(double))
                {
                    // 双精度浮点类型
                    return StringToDouble(text);
                }
                else if (targetType == typeof(string))
                {
                    return text;
                }
                else if (targetType == typeof(System.DateTime))
                {
                    // 日期时间类型
                    return StringToDateTime(text);
                }

                Logger.Warn("Cannot convert string to target type '{%t}', getting value failed.", targetType);
                return null;
            }

            #endregion

            #region GUID类型转换相关接口函数

            /// <summary>
            /// GUID类型转换为长整数类型
            /// </summary>
            /// <param name="guid">GUID数据</param>
            /// <returns>返回转换结果，若转换失败则返回默认值</returns>
            public static long GuidToLong(SystemGuid guid)
            {
                byte[] buffer = guid.ToByteArray();
                return System.BitConverter.ToInt64(buffer, 0);
            }

            #endregion

            #region 枚举类型转换相关接口函数

            /// <summary>
            /// 检测指定类型枚举值是否为一个合法值
            /// </summary>
            /// <typeparam name="T">枚举类型</typeparam>
            /// <param name="value">枚举值</param>
            /// <returns>若给定值合法则返回true，否则返回false</returns>
            public static bool IsCorrectedEnumValue<T>(int value) where T : SystemEnum
            {
                foreach (object v in SystemEnum.GetValues(typeof(T)))
                {
                    if (System.Convert.ToInt32(v) == value)
                    {
                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// 获取指定类型枚举值所对应的索引序号
            /// </summary>
            /// <typeparam name="T">枚举类型</typeparam>
            /// <param name="value">枚举值</param>
            /// <returns>返回枚举值对应的索引序号，若该枚举值为无效值，则返回-1</returns>
            public static int GetEnumIndex<T>(int value) where T : SystemEnum
            {
                int n = 0;
                foreach (object v in SystemEnum.GetValues(typeof(T)))
                {
                    if (System.Convert.ToInt32(v) == value)
                    {
                        return n;
                    }

                    ++n;
                }

                return -1;
            }

            /// <summary>
            /// 通过指定枚举类型的索引序号，查找其对应的枚举值
            /// </summary>
            /// <typeparam name="T">枚举类型</typeparam>
            /// <param name="value">枚举索引序号</param>
            /// <returns>返回索引序号对应的枚举值</returns>
            /// <exception cref="ArgumentOutOfRangeException"></exception>
            public static T GetEnumFromIndex<T>(int value) where T : SystemEnum
            {
                SystemArray array = SystemEnum.GetValues(typeof(T));
                if (array.Length <= value)
                {
                    throw new System.ArgumentOutOfRangeException($"The enum {typeof(T).Name} index {value} out of the bounds.");
                }

                return (T) array.GetValue(value);
            }

            /// <summary>
            /// 通过指定枚举类型的字符串名称，查找其对应的枚举值
            /// </summary>
            /// <typeparam name="T">枚举类型</typeparam>
            /// <param name="value">字符串名称</param>
            /// <returns>返回字符串名称对应的枚举值，若名称为无效参数，则返回枚举的默认值</returns>
            public static T GetEnumFromName<T>(string value) where T : SystemEnum
            {
                if (false == SystemEnum.IsDefined(typeof(T), value))
                {
                    return default(T);
                }

                return (T) SystemEnum.Parse(typeof(T), value);
            }

            /// <summary>
            /// 通过指定枚举类型的数值，查找其对应的枚举值
            /// </summary>
            /// <typeparam name="T">枚举类型</typeparam>
            /// <param name="value">数值</param>
            /// <returns>返回数值对应的枚举值，若数值为无效参数，则返回枚举的默认值</returns>
            public static T GetEnumFromValue<T>(int value) where T : SystemEnum
            {
                object obj = System.Convert.ChangeType(value, SystemEnum.GetUnderlyingType(typeof(T)));
                if (false == SystemEnum.IsDefined(typeof(T), obj))
                {
                    return default(T);
                }

                return (T) SystemEnum.ToObject(typeof(T), obj);
            }

            #endregion

            #region 数据编码转换相关接口函数

            /// <summary>
            /// 以字节数组的形式获取‘UTF-8’编码的字符串
            /// </summary>
            /// <param name="value">转换的字符串</param>
            /// <returns>返回用于存放结果的字节数组</returns>
            public static byte[] GetBytes(string value)
            {
                return GetBytes(value, Encoding.UTF8);
            }

            /// <summary>
            /// 以字节数组的形式获取‘UTF-8’编码的字符串
            /// </summary>
            /// <param name="value">转换的字符串</param>
            /// <param name="buffer">用于存放结果的字节数组</param>
            /// <returns>返回实际填充的字节数</returns>
            public static int GetBytes(string value, byte[] buffer)
            {
                return GetBytes(value, Encoding.UTF8, buffer, 0);
            }

            /// <summary>
            /// 以字节数组的形式获取‘UTF-8’编码的字符串
            /// </summary>
            /// <param name="value">转换的字符串</param>
            /// <param name="buffer">用于存放结果的字节数组</param>
            /// <param name="startIndex">数组的起始位置</param>
            /// <returns>返回实际填充的字节数</returns>
            public static int GetBytes(string value, byte[] buffer, int startIndex)
            {
                return GetBytes(value, Encoding.UTF8, buffer, startIndex);
            }

            /// <summary>
            /// 以字节数组的形式获取指定编码的字符串
            /// </summary>
            /// <param name="value">转换的字符串</param>
            /// <param name="encoding">转换使用的编码</param>
            /// <returns>返回用于存放结果的字节数组</returns>
            public static byte[] GetBytes(string value, Encoding encoding)
            {
                if (null == value)
                {
                    throw new CFrameworkException("Value is invalid.");
                }

                if (null == encoding)
                {
                    throw new CFrameworkException("Encoding is invalid.");
                }

                return encoding.GetBytes(value);
            }

            /// <summary>
            /// 以字节数组的形式获取指定编码的字符串
            /// </summary>
            /// <param name="value">转换的字符串</param>
            /// <param name="encoding">转换使用的编码</param>
            /// <param name="buffer">用于存放结果的字节数组</param>
            /// <returns>返回实际填充的字节数</returns>
            public static int GetBytes(string value, Encoding encoding, byte[] buffer)
            {
                return GetBytes(value, encoding, buffer, 0);
            }

            /// <summary>
            /// 以字节数组的形式获取指定编码的字符串
            /// </summary>
            /// <param name="value">转换的字符串</param>
            /// <param name="encoding">转换使用的编码</param>
            /// <param name="buffer">用于存放结果的字节数组</param>
            /// <param name="startIndex">数组的起始位置</param>
            /// <returns>返回实际填充的字节数</returns>
            public static int GetBytes(string value, Encoding encoding, byte[] buffer, int startIndex)
            {
                if (null == value)
                {
                    throw new CFrameworkException("Value is invalid.");
                }

                if (null == encoding)
                {
                    throw new CFrameworkException("Encoding is invalid.");
                }

                return encoding.GetBytes(value, 0, value.Length, buffer, startIndex);
            }

            /// <summary>
            /// 返回由字节数组使用‘UTF-8’编码转换成的字符串
            /// </summary>
            /// <param name="value">字节数组</param>
            /// <returns>返回转换后的字符串</returns>
            public static string GetString(byte[] value)
            {
                return GetString(value, Encoding.UTF8);
            }

            /// <summary>
            /// 返回由字节数组使用指定编码转换成的字符串
            /// </summary>
            /// <param name="value">字节数组</param>
            /// <param name="encoding">转换使用的编码</param>
            /// <returns>返回转换后的字符串</returns>
            public static string GetString(byte[] value, Encoding encoding)
            {
                if (null == value)
                {
                    throw new CFrameworkException("Value is invalid.");
                }

                if (null == encoding)
                {
                    throw new CFrameworkException("Encoding is invalid.");
                }

                return encoding.GetString(value);
            }

            /// <summary>
            /// 返回由字节数组使用‘UTF-8’编码转换成的字符串
            /// </summary>
            /// <param name="value">字节数组</param>
            /// <param name="startIndex">字节数组的起始位置</param>
            /// <param name="length">数据长度</param>
            /// <returns>返回转换后的字符串</returns>
            public static string GetString(byte[] value, int startIndex, int length)
            {
                return GetString(value, startIndex, length, Encoding.UTF8);
            }

            /// <summary>
            /// 返回由字节数组使用指定编码转换成的字符串
            /// </summary>
            /// <param name="value">字节数组</param>
            /// <param name="startIndex">字节数组的起始位置</param>
            /// <param name="length">数据长度</param>
            /// <param name="encoding">转换使用的编码</param>
            /// <returns>返回转换后的字符串</returns>
            public static string GetString(byte[] value, int startIndex, int length, Encoding encoding)
            {
                if (null == value)
                {
                    throw new CFrameworkException("Value is invalid.");
                }

                if (null == encoding)
                {
                    throw new CFrameworkException("Encoding is invalid.");
                }

                return encoding.GetString(value, startIndex, length);
            }

            #endregion

            #region 屏幕像素转换相关接口函数

            /// <summary>
            /// 英寸到厘米的转换单位
            /// </summary>
            private const float InchesToCentimeters = 2.54f;
            /// <summary>
            /// 厘米到英寸的转换单位
            /// </summary>
            private const float CentimetersToInches = 0.393700778f;

            /// <summary>
            /// 将指定的像素值转换为厘米
            /// </summary>
            /// <param name="pixels">像素值</param>
            /// <returns>返回转换后的厘米值</returns>
            public static float GetCentimetersFromPixels(float pixels)
            {
                float screenDpi = NovaEngine.Device.Instance.ScreenDpi;
                if (screenDpi <= 0f)
                {
                    throw new CFrameworkException("You must set screen DPI at first.");
                }

                return InchesToCentimeters * pixels / screenDpi;
            }

            /// <summary>
            /// 将指定的厘米值转换为像素
            /// </summary>
            /// <param name="centimeters">厘米值</param>
            /// <returns>返回转换后的像素值</returns>
            public static float GetPixelsFromCentimeters(float centimeters)
            {
                float screenDpi = NovaEngine.Device.Instance.ScreenDpi;
                if (screenDpi <= 0f)
                {
                    throw new CFrameworkException("You must set screen DPI at first.");
                }

                return CentimetersToInches * centimeters * screenDpi;
            }

            /// <summary>
            /// 将指定的像素值转换为英寸
            /// </summary>
            /// <param name="pixels">像素值</param>
            /// <returns>返回转换后的英寸值</returns>
            public static float GetInchesFromPixels(float pixels)
            {
                float screenDpi = NovaEngine.Device.Instance.ScreenDpi;
                if (screenDpi <= 0f)
                {
                    throw new CFrameworkException("You must set screen DPI at first.");
                }

                return pixels / screenDpi;
            }

            /// <summary>
            /// 将指定的英寸值转换为像素
            /// </summary>
            /// <param name="inches">英寸值</param>
            /// <returns>返回转换后的像素值</returns>
            public static float GetPixelsFromInches(float inches)
            {
                float screenDpi = NovaEngine.Device.Instance.ScreenDpi;
                if (screenDpi <= 0f)
                {
                    throw new CFrameworkException("You must set screen DPI at first.");
                }

                return inches * screenDpi;
            }

            #endregion

            #region 数据MD5编码相关接口函数

            /// <summary>
            /// 获取指定字符串内容所计算的MD5哈希值的字符串格式数据
            /// </summary>
            /// <param name="content">字符串数据</param>
            /// <returns>返回给定字符串的哈希值结果</returns>
            public static string GetMD5HashString(string content)
            {
                return MD5.Create().ComputeHash(GetBytes(content)).ToHexString();
            }

            /// <summary>
            /// 获取指定文件内容计算的MD5哈希值的字符串格式数据
            /// </summary>
            /// <param name="filename">文件名称</param>
            /// <returns>返回给定文件的哈希值结果</returns>
            /// <exception cref="CFrameworkException"></exception>
            public static string GetMD5HashStringFromFile(string filename)
            {
                string content = Path.ReadAllText(filename);
                if (null == content)
                {
                    throw new CFrameworkException("Read file {0} failed.", filename);
                }

                return GetMD5HashString(content);
            }

            #endregion
        }
    }
}
