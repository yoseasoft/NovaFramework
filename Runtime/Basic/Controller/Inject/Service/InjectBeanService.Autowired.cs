/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace GameEngine
{
    /// <summary>
    /// 提供注入操作接口的服务类，对整个程序内部的对象实例提供注入操作的服务逻辑处理
    /// </summary>
    public static partial class InjectBeanService
    {
        /// <summary>
        /// 处理创建目标对象的自动装配流程的接口函数
        /// </summary>
        /// <param name="obj">目标对象实例</param>
        /// <returns>若目标对象装配成功返回true，否则返回false</returns>
        public static bool AutowiredProcessingOnCreateTargetObject(CBean obj)
        {
            Loader.Symboling.SymClass symClass = Loader.CodeLoader.GetSymClassByType(obj.GetType());
            if (null == symClass)
            {
                Debugger.Warn("Could not found any bean class info with target type '{0}', processed it autowired failed.",
                        NovaEngine.Utility.Text.ToString(obj.GetType()));
                return false;
            }

            Loader.Symboling.Bean bean = symClass.GetBean(obj.BeanName);
            Debugger.Info("find bean name '{0}' and field count '{1}' with target object '{2}' !!!", obj.BeanName, bean.GetFieldCount(), NovaEngine.Utility.Text.ToString(obj.GetType()));

            IEnumerator<KeyValuePair<string, Loader.Symboling.BeanField>> e_beanField = bean.GetFieldEnumerator();
            if (null != e_beanField)
            {
                while (e_beanField.MoveNext())
                {
                    Loader.Symboling.BeanField beanField = e_beanField.Current.Value;
                    Loader.Symboling.SymField symField = beanField.SymField;

                    Debugger.Warn("create field '{0}' with target object '{1}' !!!", beanField.FieldName, obj.GetType().FullName);

                    if (null == symField)
                    {
                        Debugger.Warn("Could not found any symbol field instance with name '{0}' and class type '{1}', injected it failed.",
                                beanField.FieldName, beanField.BeanObject.BeanName);
                        continue;
                    }

                    if (symField.FieldType.IsClass || symField.FieldType.IsInterface)
                    {
                        object value = __CreateAutowiredObjectForClassType(beanField);
                        if (null == value)
                        {
                            Debugger.Warn("Create autowired object error with target field name '{0}', setting it failed.", symField.FieldName);
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
                            Debugger.Warn("Create autowired constant error with target field name '{0}', setting it failed.", symField.FieldName);
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
                    Debugger.Warn("Could not found any reference bean class with reference name '{0}' from target field '{1}', setting this field value failed.",
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
            Loader.Symboling.SymClass symClass = Loader.CodeLoader.GetSymClassByType(obj.GetType());
            // InjectController.BeanClass beanClass = InjectController.Instance.FindGenericBeanClassByType(obj.GetType());
            if (null == symClass)
            {
                Debugger.Warn("Could not found any bean class with target type '{0}', processed it autowired failed.", NovaEngine.Utility.Text.ToString(obj.GetType()));
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
                        Debugger.Warn("Could not found any symbol field instance with name '{0}' and class type '{1}', injected it failed.",
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
