/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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

namespace GameEngine.Loader
{
    /// 代码加载器的辅助工具类
    public static partial class CodeLoaderUtils
    {
        private static string ToString(Configuring.BaseConfigureInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("Type={%i},", targetObject.Type);
            fsb.Append("Name={%s},", targetObject.Name);
            return fsb.ToString();
        }

        internal static string ToString(Configuring.BeanConfigureInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("Bean={");
            fsb.Append(ToString((Configuring.BaseConfigureInfo) targetObject));
            fsb.Append("ClassType={%t},", targetObject.ClassType);
            fsb.Append("ParentName={%s},", targetObject.ParentName);
            fsb.Append("Singleton={%b},", targetObject.Singleton);
            fsb.Append("Inherited={%b},", targetObject.Inherited);
            fsb.Append("Fields={{{%s}}},", targetObject.Fields, (k, v) => ToString(v));
            fsb.Append("Components={{{%s}}},", targetObject.Components, ToString);
            fsb.Append("}");
            return fsb.ToString();
        }

        private static string ToString(Configuring.BeanFieldConfigureInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("FieldName={%s},", targetObject.FieldName);
            fsb.Append("ReferenceName={%s},", targetObject.ReferenceName);
            fsb.Append("ReferenceType={%t},", targetObject.ReferenceType);
            fsb.Append("ReferenceValue={%s},", targetObject.ReferenceValue);
            return fsb.ToString();
        }

        private static string ToString(Configuring.BeanPropertyConfigureInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("PropertyName={%s},", targetObject.PropertyName);
            fsb.Append("ReferenceName={%s},", targetObject.ReferenceName);
            fsb.Append("ReferenceType={%t},", targetObject.ReferenceType);
            fsb.Append("ReferenceValue={%s},", targetObject.ReferenceValue);
            return fsb.ToString();
        }

        private static string ToString(Configuring.BeanComponentConfigureInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("ReferenceName={%s},", targetObject.ReferenceName);
            fsb.Append("ReferenceType={%t},", targetObject.ReferenceType);
            fsb.Append("Priority={%d},", targetObject.Priority);
            fsb.Append("ActivationBehaviourType={%i},", targetObject.ActivationBehaviourType);
            return fsb.ToString();
        }
    }
}
