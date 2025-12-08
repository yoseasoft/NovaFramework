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

namespace GameEngine
{
    /// 提供注入操作接口的服务类
    public static partial class InjectBeanService
    {
        /// <summary>
        /// 处理创建目标对象的自动装配流程的接口函数
        /// </summary>
        /// <param name="obj">目标对象实例</param>
        /// <returns>若目标对象装配成功返回true，否则返回false</returns>
        public static bool AutowiredProcessingOnCreateTargetObject(CBean obj)
        {
            Loader.Symboling.SymClass symClass = obj.Symbol;
            if (null == symClass)
            {
                Debugger.Warn("Could not found any bean class info with target type '{%t}', processed it autowired failed.", obj.BeanType);
                return false;
            }

            Loader.Symboling.Bean bean = symClass.GetBean(obj.BeanName);
            // Debugger.Info("find bean name '{%s}' and field count '{%d}' with target object '{%t}' !!!", obj.BeanName, bean.GetFieldCount(), obj.BeanType);

            IEnumerator<KeyValuePair<string, Loader.Symboling.BeanField>> e_beanField = bean.GetFieldEnumerator();
            if (null != e_beanField)
            {
                while (e_beanField.MoveNext())
                {
                    Loader.Symboling.BeanField beanField = e_beanField.Current.Value;
                    Loader.Symboling.SymField symField = beanField.SymField;

                    // Debugger.Warn("create field '{%s}' with target object '{%t}' !!!", beanField.FieldName, obj.BeanType);

                    if (null == symField)
                    {
                        Debugger.Warn("Could not found any symbol field instance with name '{%s}' and class type '{%s}', injected it failed.",
                                beanField.FieldName, beanField.BeanObject.BeanName);
                        continue;
                    }

                    if (symField.FieldType.IsClass || symField.FieldType.IsInterface)
                    {
                        object value = __CreateAutowiredObjectForClassType(beanField);
                        if (null == value)
                        {
                            Debugger.Warn("Create autowired object error with target field name '{%s}', setting it failed.", symField.FieldName);
                            continue;
                        }

                        symField.FieldInfo.SetValue(obj, value);
                    }
                    else
                    {
                        // 结构体或基础类型的赋值
                        object value = __CreateAutowiredObjectFromConstantType(beanField);
                        if (null == value)
                        {
                            Debugger.Warn("Create autowired constant error with target field name '{%s}', setting it failed.", symField.FieldName);
                            continue;
                        }

                        symField.FieldInfo.SetValue(obj, value);
                    }
                }
            }

            return true;
        }

        private static object __CreateAutowiredObjectForClassType(Loader.Symboling.BeanField beanField)
        {
            if (false == string.IsNullOrEmpty(beanField.ReferenceBeanName))
            {
                Loader.Symboling.Bean referenceBean = Loader.CodeLoader.GetBeanClassByName(beanField.ReferenceBeanName);
                if (null == referenceBean)
                {
                    Debugger.Warn("Could not found any reference bean class with reference name '{%s}' from target field '{%s}', setting this field value failed.",
                            beanField.ReferenceBeanName, beanField.FieldName);
                    return null;
                }

                return CreateBeanInstance(referenceBean);
            }
            else if (null != beanField.ReferenceClassType)
            {
                // referenceBeanInfo = InjectController.Instance.FindGenericBeanByType(beanFieldInfo.ReferenceType);

                // 包含通用对象类型object和实体对象类型bean
                return CreateObjectInstance(beanField.ReferenceClassType);
            }

            return null;
        }

        private static object __CreateAutowiredObjectFromConstantType(Loader.Symboling.BeanField beanField)
        {
            return null;
        }

        /// <summary>
        /// 处理释放目标对象的自动装配流程的接口函数
        /// </summary>
        /// <param name="obj">目标对象实例</param>
        public static void AutowiredProcessingOnReleaseTargetObject(CBean obj)
        {
            Loader.Symboling.SymClass symClass = obj.Symbol;
            // InjectController.BeanClass beanClass = InjectController.Instance.FindGenericBeanClassByType(obj.BeanType);
            if (null == symClass)
            {
                Debugger.Warn("Could not found any bean class with target type '{%t}', processed it autowired failed.", obj.BeanType);
                return;
            }

            Loader.Symboling.Bean bean = symClass.GetBean(obj.BeanName);

            IEnumerator<KeyValuePair<string, Loader.Symboling.BeanField>> e_beanField = bean.GetFieldEnumerator();
            if (null != e_beanField)
            {
                while (e_beanField.MoveNext())
                {
                    Loader.Symboling.BeanField beanField = e_beanField.Current.Value;
                    Loader.Symboling.SymField symField = beanField.SymField;

                    if (null == symField)
                    {
                        Debugger.Warn("Could not found any symbol field instance with name '{%s}' and class type '{%s}', injected it failed.",
                                beanField.FieldName, beanField.BeanObject.BeanName);
                        continue;
                    }

                    if (symField.FieldType.IsClass || symField.FieldType.IsInterface)
                    {
                        object value = symField.FieldInfo.GetValue(obj);

                        // 包含通用对象类型object和实体对象类型bean
                        ReleaseObjectInstance(value);

                        symField.FieldInfo.SetValue(obj, null);
                    }
                    else
                    {
                        // 结构体或基础类型无需处理
                    }
                }
            }
        }
    }
}
