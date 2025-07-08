/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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

using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;

namespace NovaEngine
{
    /// <summary>
    /// 非混淆类型标识的属性类，用于声明给定的类，接口或函数不可参与混淆工作
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Class | SystemAttributeTargets.Interface | SystemAttributeTargets.Struct | SystemAttributeTargets.Method | SystemAttributeTargets.Enum)]
    public class NoConfusedAttribute : SystemAttribute
    {
        public NoConfusedAttribute() : base()
        {
        }
    }

    /// <summary>
    /// 非支持类型标识的属性类，用于声明当前版本中的指定函数不被支持
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method)]
    public class NoSupportedAttribute : SystemAttribute
    {
        public NoSupportedAttribute() : base()
        {
        }
    }

    /// <summary>
    /// 废弃类型标识的属性类，用于声明一个对象类，接口或方法在当前版本中被废弃，不推荐使用
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Class | SystemAttributeTargets.Interface | SystemAttributeTargets.Struct | SystemAttributeTargets.Method)]
    public class DeprecatedAttribute : SystemAttribute
    {
        public DeprecatedAttribute() : base()
        {
        }
    }

    /// <summary>
    /// 枚举描述类型标识的属性类，用于声明枚举属性的字符串描述文本
    /// </summary>
    public sealed class EnumDescriptionAttribute : SystemAttribute
    {
        /// <summary>
        /// 枚举描述内容
        /// </summary>
        private readonly string _description;

        public EnumDescriptionAttribute(string description) : base()
        {
            _description = description;
        }

        /// <summary>
        /// 描述属性值的访问操作接口
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// 获取枚举描述属性类型对应的描述内容
        /// </summary>
        /// <param name="value">枚举类型实例</param>
        /// <returns>返回描述内容，若未定义该属性则返回枚举名</returns>
        public static string GetDescription(System.Enum value)
        {
            if (null == value)
            {
                throw new CFrameworkException("Enum value is invalid.");
            }

            string description = value.ToString();
            System.Reflection.FieldInfo fieldInfo = value.GetType().GetField(description);
            EnumDescriptionAttribute[] attributes = (EnumDescriptionAttribute[]) fieldInfo.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
            if (null != attributes && attributes.Length > 0)
            {
                description = attributes[0].Description;
            }

            return description;
        }
    }
}
