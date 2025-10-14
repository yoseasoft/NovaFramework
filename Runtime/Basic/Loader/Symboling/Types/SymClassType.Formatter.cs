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
    /// <summary>
    /// 针对标记符号类型对象的格式化辅助工具类，通过该类定义一些用于标记符号对象的格式化接口函数
    /// </summary>
    public static partial class CodeLoaderObject
    {
        public static string ToString(Symboling.SymClass targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("SymClass={");

            fsb.Append("Uid={%d},", targetObject.Uid);
            fsb.Append("ClassName={%s},", targetObject.ClassName);
            fsb.Append("FullName={%s},", targetObject.FullName);
            fsb.Append("ClassType={%t},", targetObject.ClassType);
            fsb.Append("BaseType={%t},", targetObject.BaseType);

            fsb.Append("Attributes={{{%s}}},", targetObject.Attributes, (v) => NovaEngine.Utility.Text.GetFullName(v));
            fsb.Append("FeatureTypes={{{%s}}},", targetObject.FeatureTypes, (v) => NovaEngine.Utility.Text.GetFullName(v));
            fsb.Append("InterfaceTypes={{{%s}}},", targetObject.InterfaceTypes, (v) => NovaEngine.Utility.Text.GetFullName(v));
            fsb.Append("AspectBehaviourTypes={{{%s}}},", targetObject.AspectBehaviourTypes);
            fsb.Append("Fields={{{%s}}},", targetObject.Fields, (k, v) => v.FieldName);
            fsb.Append("Properties={{{%s}}},", targetObject.Properties, (k, v) => v.PropertyName);
            fsb.Append("Methods={{{%s}}},", targetObject.Methods, (k, v) => v.MethodName);

            fsb.Append("Beans={{{%s}}},", targetObject.GetAllBeans(), ToString);

            fsb.Append("}");
            return fsb.ToString();
        }

        public static string ToString(Symboling.SymField targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("SymField={");
            fsb.Append("Uid={%d},", targetObject.Uid);
            //fsb.Append("FieldName={%s},", targetObject.FieldName);
            //fsb.Append("FieldType={%t},", targetObject.FieldType);
            fsb.Append("FieldInfo={%t},", targetObject.FieldInfo);
            fsb.Append("Attributes={{{%s}}},", targetObject.Attributes, (index, v) => $"[{index}]={NovaEngine.Utility.Text.GetFullName(v)}");
            fsb.Append("}");
            return fsb.ToString();
        }

        public static string ToString(Symboling.SymProperty targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("SymProperty={");
            fsb.Append("Uid={%d},", targetObject.Uid);
            //fsb.Append("PropertyName={%s},", targetObject.PropertyName);
            //fsb.Append("PropertyType={%t},", targetObject.PropertyType);
            fsb.Append("PropertyInfo={%t},", targetObject.PropertyInfo);
            fsb.Append("Attributes={{{%s}}},", targetObject.Attributes, (index, v) => $"[{index}]={NovaEngine.Utility.Text.GetFullName(v)}");
            fsb.Append("}");
            return fsb.ToString();
        }

        public static string ToString(Symboling.SymMethod targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("SymMethod={");
            fsb.Append("Uid={%d},", targetObject.Uid);
            //fsb.Append("MethodName={%s},", targetObject.MethodName);
            //fsb.Append("FullName={%s},", targetObject.FullName);
            //fsb.Append("ReturnType={%t},", targetObject.ReturnType);
            fsb.Append("MethodInfo={%t},", targetObject.MethodInfo);
            fsb.Append("Attributes={{{%s}}},", targetObject.Attributes, (index, v) => $"[{index}]={NovaEngine.Utility.Text.GetFullName(v)}");
            fsb.Append("}");
            return fsb.ToString();
        }

        public static string ToString(Symboling.Bean targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("Bean={");
            fsb.Append("ClassName={%s},", targetObject.TargetClass.ClassName);
            fsb.Append("BeanName={%s},", targetObject.BeanName);
            fsb.Append("Singleton={%b},", targetObject.Singleton);
            fsb.Append("Inherited={%b},", targetObject.Inherited);
            fsb.Append("Fields={{{%s}}},", targetObject.Fields, (k, v) => v.FieldName);
            fsb.Append("Properties={{{%s}}},", targetObject.Properties, (k, v) => v.PropertyName);
            fsb.Append("Components={{{%s}}},", targetObject.Components, (index, v) =>
            {
                if (null != v.ReferenceClassType) return $"ClassType={NovaEngine.Utility.Text.GetFullName(v.ReferenceClassType)}";
                else if (null != v.ReferenceBeanName) return $"BeanName={v.ReferenceBeanName}";
                else return NovaEngine.Definition.CString.Null;
            });
            fsb.Append("}");
            return fsb.ToString();
        }

        public static string ToString(Symboling.BeanField targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("BeanField={");
            fsb.Append("FieldName={%s},", targetObject.FieldName);
            if (null != targetObject.ReferenceClassType) fsb.Append("ReferenceClassType={%t},", targetObject.ReferenceClassType);
            if (null != targetObject.ReferenceBeanName) fsb.Append("ReferenceBeanName={%s},", targetObject.ReferenceBeanName);
            if (null != targetObject.ReferenceValue) fsb.Append("ReferenceValue={%t},", targetObject.ReferenceValue);
            fsb.Append("}");
            return fsb.ToString();
        }

        public static string ToString(Symboling.BeanProperty targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("BeanProperty={");
            fsb.Append("PropertyName={%s},", targetObject.PropertyName);
            if (null != targetObject.ReferenceClassType) fsb.Append("ReferenceClassType={%t},", targetObject.ReferenceClassType);
            if (null != targetObject.ReferenceBeanName) fsb.Append("ReferenceBeanName={%s},", targetObject.ReferenceBeanName);
            if (null != targetObject.ReferenceValue) fsb.Append("ReferenceValue={%t},", targetObject.ReferenceValue);
            fsb.Append("}");
            return fsb.ToString();
        }

        public static string ToString(Symboling.BeanComponent targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("BeanComponent={");
            if (null != targetObject.ReferenceClassType) fsb.Append("ReferenceClassType={%t},", targetObject.ReferenceClassType);
            if (null != targetObject.ReferenceBeanName) fsb.Append("ReferenceBeanName={%s},", targetObject.ReferenceBeanName);
            fsb.Append("Priority={%d},", targetObject.Priority);
            fsb.Append("ActivationBehaviourType={%i},", targetObject.ActivationBehaviourType);
            fsb.Append("}");
            return fsb.ToString();
        }
    }
}
