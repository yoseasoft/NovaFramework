/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemType = System.Type;
using SystemBindingFlags = System.Reflection.BindingFlags;
using SystemFieldInfo = System.Reflection.FieldInfo;

namespace NovaEngine
{
    /// <summary>
    /// 通用错误码定义公共接口类，用于对框架内部的错误码进行统一定义
    /// </summary>
    public static class ErrorCode
    {
        public const int UNKNOWN = 0;

        /// <summary>
        /// 系统错误
        /// </summary>
        public const int SYSTEM = 1001;

        /// <summary>
        /// 方法的参数是非法的
        /// </summary>
        public const int ARGUMENT = 1011;

        /// <summary>
        /// 一个空参数传递给方法，该方法不能接受该参数
        /// </summary>
        public const int ARGUMENT_NULL = 1012;

        /// <summary>
        /// 方法的参数值超出范围
        /// </summary>
        public const int ARGUMENT_OUT_OF_RANGE = 1013;

        /// <summary>
        /// 出现算术的上溢或者下溢
        /// </summary>
        public const int ARITHMETIC = 1021;

        /// <summary>
        /// 试图在数组中存储错误类型的对象
        /// </summary>
        public const int ARRAY_TYPE_MISMATCH = 1022;

        /// <summary>
        /// 除零错误
        /// </summary>
        public const int DIVIDE_BY_ZERO = 1023;

        /// <summary>
        /// 找不到引用的DLL
        /// </summary>
        public const int DLL_NOT_FOUND = 1031;

        /// <summary>
        /// 参数格式错误
        /// </summary>
        public const int FORMAT = 1041;

        /// <summary>
        /// 数组索引超出范围
        /// </summary>
        public const int INDEX_OUT_OF_RANGE = 1042;

        /// <summary>
        /// 使用无效的类
        /// </summary>
        public const int INVALID_CAST = 1043;

        /// <summary>
        /// 方法的调用方式错误
        /// </summary>
        public const int INVALID_OPERATION = 1044;

        /// <summary>
        /// 试图访问‘private’或‘protected’的字段
        /// </summary>
        public const int FIELD_ACCESS = 1051;

        /// <summary>
        /// 对象不是一个有效的字段
        /// </summary>
        public const int MISSING_FIELD = 1052;

        /// <summary>
        /// 试图访问‘private’或‘protected’的成员
        /// </summary>
        public const int MEMBER_ACCESS = 1053;

        /// <summary>
        /// 对象不是一个有效的成员
        /// </summary>
        public const int MISSING_MEMBER = 1054;

        /// <summary>
        /// 试图访问‘private’或‘protected’的方法
        /// </summary>
        public const int METHOD_ACCESS = 1055;

        /// <summary>
        /// 对象不是一个有效的方法
        /// </summary>
        public const int MISSING_METHOD = 1056;

        /// <summary>
        /// 尚未实现的方法
        /// </summary>
        public const int NOT_IMPLEMENTED = 1061;

        /// <summary>
        /// 不支持在类中调用此方法
        /// </summary>
        public const int NOT_SUPPORTED = 1062;

        /// <summary>
        /// 试图使用一个未分配的引用
        /// </summary>
        public const int NULL_REFERENCE = 1071;

        /// <summary>
        /// 内存空间不足
        /// </summary>
        public const int OUT_OF_MEMORY = 1081;

        /// <summary>
        /// 堆栈溢出
        /// </summary>
        public const int STACK_OVERFLOW = 1082;

        /// <summary>
        /// 平台不支持某个特定属性时发生该错误
        /// </summary>
        public const int PLATFORM_NOT_SUPPORTED = 1091;

        /// <summary>
        /// 驱动查找失败
        /// </summary>
        public const int DRIVE_NOT_FOUND = 2011;

        /// <summary>
        /// 目录查找失败
        /// </summary>
        public const int DIRECTORY_NOT_FOUND = 2012;

        /// <summary>
        /// 文件查找失败
        /// </summary>
        public const int FILE_NOT_FOUND = 2013;

        /// <summary>
        /// 文件路径超出有效长度
        /// </summary>
        public const int PATH_TOO_LONG = 2021;

        /// <summary>
        /// 文件加载时发生该错误
        /// </summary>
        public const int FILE_LOAD = 2022;

        /// <summary>
        /// 无效数据
        /// </summary>
        public const int INVALID_DATA = 2023;

        /// <summary>
        /// 数据流已到末端
        /// </summary>
        public const int END_OF_STREAM = 2024;

        /// <summary>
        /// 通过指定的异常类型，获取对应的错误码值
        /// </summary>
        /// <param name="exceptionType">异常类型</param>
        /// <returns>返回异常类型对应的错误码值，若类型解析失败，则返回0</returns>
        public static int GetErrorCodeByExceptionType(SystemType exceptionType)
        {
            return GetErrorCodeByExceptionName(CFrameworkException.GetExceptionDescriptorName(exceptionType));
        }

        /// <summary>
        /// 通过指定的异常名称，获取对应的错误码值
        /// </summary>
        /// <param name="exceptionName">异常名称</param>
        /// <returns>返回异常名称对应的错误码值，若名称映射失败，则返回0</returns>
        public static int GetErrorCodeByExceptionName(string exceptionName)
        {
            if (exceptionName.IsNullOrEmpty())
            {
                return UNKNOWN;
            }

            SystemType errorCodeType = typeof(ErrorCode);
            // 注意，“const”被隐式的认为是静态属性，因此在反射时需要设置“static”标识
            SystemFieldInfo field = errorCodeType.GetField(exceptionName, SystemBindingFlags.Public | SystemBindingFlags.Static);
            if (null == field)
            {
                Logger.Error("Could not found ErrorCode field name '{0}', get target property value failed.", exceptionName);
                return 0;
            }

            return (int) field.GetValue(null);
        }
    }
}
