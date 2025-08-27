/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
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
using SystemAttribute = System.Attribute;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader.Symboling
{
    /// <summary>
    /// 针对标记符号类型对象的格式化辅助工具类，通过该类定义一些用于标记符号对象的格式化接口函数
    /// </summary>
    internal static class Formatter
    {
        public static string ToString(SymClass targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("SymClass={");

            sb.AppendFormat("Uid={0},", targetObject.Uid);
            sb.AppendFormat("ClassName={0},", targetObject.ClassName);
            sb.AppendFormat("FullName={0},", targetObject.FullName);
            sb.AppendFormat("ClassType={0},", NovaEngine.Utility.Text.ToString(targetObject.ClassType));
            sb.AppendFormat("BaseType={0},", NovaEngine.Utility.Text.ToString(targetObject.BaseType));

            sb.AppendFormat("Attributes={{{0}}},", NovaEngine.Utility.Text.ToString<SystemAttribute>(targetObject.Attributes, (index, v) =>
            {
                return $"[{index}]={NovaEngine.Utility.Text.GetFullName(v)}";
            }));
            sb.AppendFormat("FeatureTypes={{{0}}},", NovaEngine.Utility.Text.ToString<SystemType>(targetObject.FeatureTypes, (index, v) =>
            {
                return $"[{index}]={NovaEngine.Utility.Text.GetFullName(v)}";
            }));
            sb.AppendFormat("InterfaceTypes={{{0}}},", NovaEngine.Utility.Text.ToString<SystemType>(targetObject.InterfaceTypes, (index, v) =>
            {
                return $"[{index}]={NovaEngine.Utility.Text.GetFullName(v)}";
            }));
            sb.AppendFormat("AspectBehaviourTypes={{{0}}},", NovaEngine.Utility.Text.ToString<AspectBehaviourType>(targetObject.AspectBehaviourTypes, (index, v) =>
            {
                return $"[{index}]={v.ToString()}";
            }));
            sb.AppendFormat("Fields={{{0}}},", NovaEngine.Utility.Text.ToString<string, SymField>(targetObject.Fields, (k, v) =>
            {
                return v.FieldName;
            }));
            sb.AppendFormat("Properties={{{0}}},", NovaEngine.Utility.Text.ToString<string, SymProperty>(targetObject.Properties, (k, v) =>
            {
                return v.PropertyName;
            }));
            sb.AppendFormat("Methods={{{0}}},", NovaEngine.Utility.Text.ToString<string, SymMethod>(targetObject.Methods, (k, v) =>
            {
                return v.MethodName;
            }));

            sb.AppendFormat("Beans={{{0}}},", NovaEngine.Utility.Text.ToString<Bean>(targetObject.GetAllBeans(), (index, v) =>
            {
                return ToString(v);
            }));

            sb.Append("}");
            return sb.ToString();
        }

        public static string ToString(SymField targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("SymField={");

            sb.AppendFormat("Uid={0},", targetObject.Uid);
            //sb.AppendFormat("FieldName={0},", targetObject.FieldName);
            //sb.AppendFormat("FieldType={0},", NovaEngine.Utility.Text.ToString(targetObject.FieldType));
            sb.AppendFormat("FieldInfo={0},", NovaEngine.Utility.Text.ToString(targetObject.FieldInfo));

            sb.AppendFormat("Attributes={{{0}}},", NovaEngine.Utility.Text.ToString<SystemAttribute>(targetObject.Attributes, (index, v) =>
            {
                return $"[{index}]={NovaEngine.Utility.Text.GetFullName(v)}";
            }));

            sb.Append("}");
            return sb.ToString();
        }

        public static string ToString(SymProperty targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("SymProperty={");

            sb.AppendFormat("Uid={0},", targetObject.Uid);
            //sb.AppendFormat("PropertyName={0},", targetObject.PropertyName);
            //sb.AppendFormat("PropertyType={0},", NovaEngine.Utility.Text.ToString(targetObject.PropertyType));
            sb.AppendFormat("PropertyInfo={0},", NovaEngine.Utility.Text.ToString(targetObject.PropertyInfo));

            sb.AppendFormat("Attributes={{{0}}},", NovaEngine.Utility.Text.ToString<SystemAttribute>(targetObject.Attributes, (index, v) =>
            {
                return $"[{index}]={NovaEngine.Utility.Text.GetFullName(v)}";
            }));

            sb.Append("}");
            return sb.ToString();
        }

        public static string ToString(SymMethod targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("SymMethod={");

            sb.AppendFormat("Uid={0},", targetObject.Uid);
            //sb.AppendFormat("MethodName={0},", targetObject.MethodName);
            //sb.AppendFormat("FullName={0},", targetObject.FullName);
            //sb.AppendFormat("ReturnType={0},", NovaEngine.Utility.Text.ToString(targetObject.ReturnType));
            sb.AppendFormat("MethodInfo={0},", NovaEngine.Utility.Text.ToString(targetObject.MethodInfo));

            sb.AppendFormat("Attributes={{{0}}},", NovaEngine.Utility.Text.ToString<SystemAttribute>(targetObject.Attributes, (index, v) =>
            {
                return $"[{index}]={NovaEngine.Utility.Text.GetFullName(v)}";
            }));

            sb.Append("}");
            return sb.ToString();
        }

        public static string ToString(Bean targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("Bean={");

            sb.AppendFormat("ClassName={0},", targetObject.TargetClass.ClassName);
            sb.AppendFormat("BeanName={0},", targetObject.BeanName);
            sb.AppendFormat("Singleton={0},", targetObject.Singleton);
            sb.AppendFormat("Inherited={0},", targetObject.Inherited);

            sb.AppendFormat("Fields={{{0}}},", NovaEngine.Utility.Text.ToString<string, BeanField>(targetObject.Fields, (k, v) =>
            {
                return v.FieldName;
            }));
            sb.AppendFormat("Properties={{{0}}},", NovaEngine.Utility.Text.ToString<string, BeanProperty>(targetObject.Properties, (k, v) =>
            {
                return v.PropertyName;
            }));
            sb.AppendFormat("Components={{{0}}},", NovaEngine.Utility.Text.ToString<BeanComponent>(targetObject.Components, (index, v) =>
            {
                if (null != v.ReferenceClassType) return $"ClassType={NovaEngine.Utility.Text.GetFullName(v.ReferenceClassType)}";
                else if (null != v.ReferenceBeanName) return $"BeanName={v.ReferenceBeanName}";
                else return NovaEngine.Definition.CString.Null;
            }));

            sb.Append("}");
            return sb.ToString();
        }

        public static string ToString(BeanField targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("BeanField={");

            sb.AppendFormat("FieldName={0},", targetObject.FieldName);
            if (null != targetObject.ReferenceClassType) sb.AppendFormat("ReferenceClassType={0},", NovaEngine.Utility.Text.ToString(targetObject.ReferenceClassType));
            if (null != targetObject.ReferenceBeanName) sb.AppendFormat("ReferenceBeanName={0},", targetObject.ReferenceBeanName);
            if (null != targetObject.ReferenceValue) sb.AppendFormat("ReferenceValue={0},", NovaEngine.Utility.Text.GetFullName(targetObject.ReferenceValue.GetType()));

            sb.Append("}");
            return sb.ToString();
        }

        public static string ToString(BeanProperty targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("BeanProperty={");

            sb.AppendFormat("PropertyName={0},", targetObject.PropertyName);
            if (null != targetObject.ReferenceClassType) sb.AppendFormat("ReferenceClassType={0},", NovaEngine.Utility.Text.ToString(targetObject.ReferenceClassType));
            if (null != targetObject.ReferenceBeanName) sb.AppendFormat("ReferenceBeanName={0},", targetObject.ReferenceBeanName);
            if (null != targetObject.ReferenceValue) sb.AppendFormat("ReferenceValue={0},", NovaEngine.Utility.Text.GetFullName(targetObject.ReferenceValue.GetType()));

            sb.Append("}");
            return sb.ToString();
        }

        public static string ToString(BeanComponent targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("BeanComponent={");

            if (null != targetObject.ReferenceClassType) sb.AppendFormat("ReferenceClassType={0},", NovaEngine.Utility.Text.ToString(targetObject.ReferenceClassType));
            if (null != targetObject.ReferenceBeanName) sb.AppendFormat("ReferenceBeanName={0},", targetObject.ReferenceBeanName);
            sb.AppendFormat("Priority={0},", targetObject.Priority);
            sb.AppendFormat("ActivationBehaviourType={0},", targetObject.ActivationBehaviourType);

            sb.Append("}");
            return sb.ToString();
        }
    }
}
