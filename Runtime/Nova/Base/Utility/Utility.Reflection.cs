/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
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

using System.Reflection;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Customize.Extension;

using SystemType = System.Type;
using SystemAction = System.Action;
using SystemDelegate = System.Delegate;
using SystemStringBuilder = System.Text.StringBuilder;

namespace NovaEngine
{
    /// 实用函数集合工具类
    public static partial class Utility
    {
        /// <summary>
        /// 反射相关实用函数集合
        /// </summary>
        public static class Reflection
        {
            /// <summary>
            /// 通过反射的方式调用指定函数<br/>
            /// 调用时，将根据函数参数进行相应的类型检查
            /// </summary>
            /// <param name="obj">对象实例</param>
            /// <param name="method">反射的函数实例</param>
            /// <param name="args">函数参数列表</param>
            /// <returns>返回函数的调用返回值，若函数调用失败返回null</returns>
            public static object CallMethod(object obj, MethodInfo method, params object[] args)
            {
                ParameterInfo[] parameterInfoArray = method.GetParameters();

                if (null == parameterInfoArray || parameterInfoArray.Length == 0)
                {
                    return method.Invoke(null, null);
                }

                if (null == args || args.Length != parameterInfoArray.Length)
                {
                    Logger.Error("Invalid arguments length.");
                    return null;
                }

                for (int n = 0; n < args.Length; ++n)
                {
                    if (null != args[n] && parameterInfoArray[n].ParameterType != args[n].GetType())
                    {
                        Logger.Error("Invalid arguments type {0} at param index {1}.", parameterInfoArray[n].ParameterType.ToString(), n);
                        return null;
                    }
                }

                return method.Invoke(obj, args);
            }

            /// <summary>
            /// 调用指定类的指定函数
            /// </summary>
            /// <param name="classType">类的类型</param>
            /// <param name="methodName">函数名称</param>
            /// <param name="args">函数参数列表</param>
            /// <returns>返回函数的调用返回值，若函数调用失败返回null</returns>
            public static object CallMethod(SystemType classType, string methodName, params object[] args)
            {
                // using Unity.VisualScripting.FullSerializer.Internal;
                // MethodInfo methodInfo = classType.GetDeclaredMethod(methodName);

                BindingFlags bindingFlags = BindingFlags.NonPublic |
                                            BindingFlags.Public |
                                            BindingFlags.Instance |
                                            BindingFlags.Static |
                                            BindingFlags.DeclaredOnly;
                MethodInfo methodInfo = classType.GetMethod(methodName, bindingFlags);

                return CallMethod(null, methodInfo, args);
            }

            /// <summary>
            /// 调用指定类的指定函数
            /// </summary>
            /// <param name="className">类的名称</param>
            /// <param name="methodName">函数名称</param>
            /// <param name="args">函数参数列表</param>
            /// <returns>返回函数的调用返回值，若函数调用失败返回null</returns>
            public static object CallMethod(string className, string methodName, params object[] args)
            {
                SystemType type = Assembly.GetType(className);
                if (null == type)
                {
                    Logger.Error("Could not found {%s} class type with current assemblies list, call that function {%s} failed.", className, methodName);
                    return null;
                }

                return CallMethod(type, methodName, args);
            }

            /// <summary>
            /// 调用程序集中指定类的指定函数
            /// </summary>
            /// <param name="assembly">程序集</param>
            /// <param name="className">类名称</param>
            /// <param name="methodName">函数名称</param>
            /// <param name="args">函数参数列表</param>
            /// <returns>返回函数的调用返回值，若函数调用失败返回null</returns>
            public static object CallAssemblyMethod(System.Reflection.Assembly assembly, string className, string methodName, params object[] args)
            {
                SystemType type = assembly.GetType(className);
                MethodInfo methodInfo = type.GetMethod(methodName);

                return CallMethod(null, methodInfo, args);
            }

            /// <summary>
            /// 将大小写混合的成员名称转换为大写加下划线形式的名称
            /// 例如：“NovaEngineClassType”格式的名称将转换为“NOVA_ENGINE_CLASS_TYPE”
            /// </summary>
            /// <param name="memberName">成员名称</param>
            /// <returns>返回转换后的成员名称</returns>
            public static string ConvertMixedNamesToCapitalizeWithUnderlineNames(string memberName)
            {
                if (memberName.IsNullOrEmpty())
                {
                    return memberName;
                }

                FormatStringBuilder fsb = FormatStringBuilder.Create();
                SystemStringBuilder sb = new SystemStringBuilder();
                int start = 0;
                int pos = 1;

                string sub_name;
                do
                {
                    // 每个大写字符判定为一个新的单词的开始
                    // 从这里截取出上一个完整的单词
                    // 单词之间用‘_’进行连接
                    if (System.Char.IsUpper(memberName[pos]))
                    {
                        sub_name = memberName.Substring(start, pos - start);
                        if (sb.Length > 0) sb.Append(Definition.CCharacter.Underline);
                        sb.Append(sub_name.ToUpper());

                        start = pos;
                    }

                    ++pos;
                } while (pos < memberName.Length);

                // 处理最后一个单词
                sub_name = memberName.Substring(start, pos - start);
                if (sb.Length > 0) sb.Append(Definition.CCharacter.Underline);
                sb.Append(sub_name.ToUpper());

                return sb.ToString();
            }

            #region 对象类型的属性检测函数接口

            /// <summary>
            /// 检测目标类型是否是一个声明为结构体的类
            /// </summary>
            /// <param name="targetType">目标类型</param>
            /// <returns>若给定类型是一个结构体则返回true，否则返回false</returns>
            public static bool IsTypeOfStruct(SystemType targetType)
            {
                if (null == targetType) return false;

                if (targetType.IsValueType && false == targetType.IsPrimitive && false == targetType.IsEnum)
                { return true; }

                return false;
            }

            /// <summary>
            /// 检测目标类型是否是一个编译器自动生成的中间类
            /// </summary>
            /// <param name="targetType">目标类型</param>
            /// <returns>若给定类型是一个中间类则返回true，否则返回false</returns>
            public static bool IsTypeOfCompilerGeneratedClass(SystemType targetType)
            {
                SystemType compilerGeneratedAttribute = typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute);
                return Collection.Any(targetType.GetCustomAttributes(compilerGeneratedAttribute, true));
            }

            /// <summary>
            /// 检测目标类型是否是一个内部类
            /// </summary>
            /// <param name="targetType">目标类型</param>
            /// <returns>若给定类型是一个内部类则返回true，否则返回false</returns>
            public static bool IsTypeOfInternalClass(SystemType targetType)
            {
                if (null == targetType) return false;

                if (targetType.IsClass &&
                    targetType.IsNotPublic &&
                    targetType.IsNested)
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// 检测目标类型是否是一个可以被实例化的类
            /// </summary>
            /// <param name="targetType">目标类型</param>
            /// <returns>若给定类型是一个可以实例化的类则返回true，否则返回false</returns>
            public static bool IsTypeOfInstantiableClass(SystemType targetType)
            {
                if (null == targetType) return false;

                if (targetType.IsClass &&
                    false == targetType.IsAbstract &&
                    // false == targetType.IsInterface && // 因为已经检查了是否为类类型，所以不用再次检查是否为接口类型
                    // targetType.IsPublic && // 允许实体对象被定义成保护类型
                    null != targetType.GetConstructor(SystemType.EmptyTypes) // 要求必须有一个空参构造函数
                   )
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// 检测目标类型是否是一个声明为静态的类
            /// </summary>
            /// <param name="targetType">目标类型</param>
            /// <returns>若给定类型是一个静态类则返回true，否则返回false</returns>
            public static bool IsTypeOfStaticClass(SystemType targetType)
            {
                if (null == targetType) return false;

                if (targetType.IsClass && targetType.IsAbstract && targetType.IsSealed)
                { return true; }

                return false;
            }

            /// <summary>
            /// 检测目标类型是否是一个<see cref="System.Action"/>类型或者其泛型
            /// </summary>
            /// <param name="targetType">目标类型</param>
            /// <returns>若给定类型是一个Action类型则返回true，否则返回false</returns>
            public static bool IsTypeOfAction(SystemType targetType)
            {
                if (typeof(SystemAction) == targetType)
                { return true; }

                if (false == targetType.IsGenericType)
                { return false; }

                if (typeof(System.Action<>) == targetType.GetGenericTypeDefinition())
                { return true; }

                return false;
            }

            /// <summary>
            /// 检测目标对象类型是否是一个扩展类型回调函数
            /// </summary>
            /// <param name="targetType">对象类型</param>
            /// <returns>若给定类型是一个扩展类型则返回true，否则返回false</returns>
            public static bool IsTypeOfExtension(SystemType targetType)
            {
                // 扩展类首先必须得是静态类
                if (IsTypeOfStaticClass(targetType))
                {
                    return targetType.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), inherit: false);
                }

                return false;
            }

            /// <summary>
            /// 检测目标委托句柄是否是一个扩展类型回调函数
            /// </summary>
            /// <param name="handler">委托句柄</param>
            /// <returns>若给定委托句柄是一个扩展类型则返回true，否则返回false</returns>
            public static bool IsTypeOfExtension(SystemDelegate handler)
            {
                if (null == handler) return false;

                return IsTypeOfExtension(handler.Method);
            }

            /// <summary>
            /// 检测目标函数是否是一个扩展函数
            /// </summary>
            /// <param name="method">函数对象</param>
            /// <returns>若给定函数是一个扩展类型则返回true，否则返回false</returns>
            public static bool IsTypeOfExtension(MethodBase method)
            {
                SystemType declaringType = method.DeclaringType;
                if (declaringType.IsSealed && !declaringType.IsGenericType && !declaringType.IsNested)
                {
                    return method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), inherit: false);
                }

                return false;
            }

            #endregion

            #region 创建函数的泛型委托

            /// <summary>
            /// 检查目标委托回调函数是否匹配指定的参数类型
            /// </summary>
            /// <param name="handler">委托回调函数</param>
            /// <param name="parameterTypes">参数类型</param>
            /// <returns>若目标函数的参数类型匹配则返回true，否则返回false</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsGenericDelegateParameterTypeMatched(SystemDelegate handler, params SystemType[] parameterTypes)
            {
                if (null == handler) return false;

                return IsGenericDelegateParameterTypeMatched(handler.Method, parameterTypes);
            }

            /// <summary>
            /// 检查目标函数是否匹配指定的参数类型
            /// </summary>
            /// <param name="methodInfo">函数信息</param>
            /// <param name="parameterTypes">参数类型</param>
            /// <returns>若目标函数的参数类型匹配则返回true，否则返回false</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsGenericDelegateParameterTypeMatched(MethodInfo methodInfo, params SystemType[] parameterTypes)
            {
                return IsGenericDelegateParameterAndReturnTypeMatched(methodInfo, null, parameterTypes);
            }

            /// <summary>
            /// 检查目标委托回调函数是否匹配指定的参数类型
            /// </summary>
            /// <param name="handler">委托回调函数</param>
            /// <param name="returnType">函数返回类型</param>
            /// <param name="parameterTypes">参数类型</param>
            /// <returns>若目标函数的参数类型匹配则返回true，否则返回false</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsGenericDelegateParameterAndReturnTypeMatched(SystemDelegate handler, SystemType returnType, params SystemType[] parameterTypes)
            {
                if (null == handler) return false;

                return IsGenericDelegateParameterAndReturnTypeMatched(handler.Method, returnType, parameterTypes);
            }

            /// <summary>
            /// 检查目标函数是否匹配指定的参数类型
            /// </summary>
            /// <param name="methodInfo">函数信息</param>
            /// <param name="returnType">函数返回类型</param>
            /// <param name="parameterTypes">参数类型</param>
            /// <returns>若目标函数的参数类型匹配则返回true，否则返回false</returns>
            public static bool IsGenericDelegateParameterAndReturnTypeMatched(MethodInfo methodInfo, SystemType returnType, params SystemType[] parameterTypes)
            {
                ParameterInfo[] paramInfos = methodInfo.GetParameters();

                if (null != returnType)
                {
                    // 函数返回对象的类型必须可以赋予给函数定义返回值的类型
                    if (null == methodInfo.ReturnType || false == methodInfo.ReturnType.IsAssignableFrom(returnType))
                    {
                        Debugger.Warn("The target method '{%t}' declared return type '{%t}' cannot matched calc return type '{%t}', created it for delegate failed.",
                                methodInfo, methodInfo.ReturnType, returnType);
                        return false;
                    }
                }

                // 空参检查
                if (null == paramInfos || paramInfos.Length <= 0)
                {
                    if (null == parameterTypes || parameterTypes.Length <= 0)
                    {
                        return true;
                    }

                    return false;
                }

                // 检查参数类型可以比函数的实际参数少，仅对函数前几位参数进行检查
                // 但检查参数不能比函数的实际参数多，直接返回不匹配的结果
                if (parameterTypes.Length > paramInfos.Length)
                {
                    return false;
                }

                for (int n = 0; n < parameterTypes.Length; ++n)
                {
                    SystemType paramInfoType = paramInfos[n].ParameterType;

                    // 函数传入参数的类型必须可以赋予给函数定义参数的类型
                    if (false == paramInfoType.IsAssignableFrom(parameterTypes[n]))
                    {
                        Debugger.Warn("The target method '{%t}' parameter info type '{%t}' cannot matched assign param type '{%t}', created it for delegate failed.",
                                methodInfo, paramInfoType, parameterTypes[n]);
                        return false;
                    }
                }

                return true;
            }

            /// <summary>
            /// 创建指定函数的泛型包装代理函数
            /// </summary>
            /// <param name="methodInfo">函数信息</param>
            /// <returns>返回包装后的代理函数</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static SystemDelegate CreateGenericActionDelegate(MethodInfo methodInfo)
            {
                return CreateGenericActionDelegate(null, methodInfo);
            }

            /// <summary>
            /// 创建指定函数的泛型包装代理函数
            /// </summary>
            /// <param name="self">函数调用的目标对象实例</param>
            /// <param name="methodInfo">函数信息</param>
            /// <returns>返回包装后的代理函数</returns>
            public static SystemDelegate CreateGenericActionDelegate(object self, MethodInfo methodInfo)
            {
                // 2025-09-28：
                // 允许普通的成员函数在初始构建时不传入自身实例，而在实际调用时传入
                // 2025-11-29：
                // 通过测试后发现，通过普通函数创建的委托代理句柄，在创建时不指明对象实例，
                // 而在`DynamicInvoke`调用时再传入自身对象实例，
                // 虽然在编辑器环境下可以正常执行，但打包后执行会弹出异常：
                // ArgumentException: Delegate to an instance method cannot have null 'this'.
                // 所以此处的检测需要重新打开
                if (null == self && false == methodInfo.IsStatic)
                {
                    Logger.Error("The target method '{%t}' wasn't static function, the 'self' object must be non-null.", methodInfo);
                    return null;
                }

                ParameterInfo[] paramInfos = methodInfo.GetParameters();
                if (null == paramInfos || paramInfos.Length <= 0)
                {
                    // 创建一个空参的Action对象
                    SystemType emptyActionType = CreateGenericActionType(0);

                    return SystemDelegate.CreateDelegate(emptyActionType, self, methodInfo);
                }

                SystemType[] genericTypes = new SystemType[paramInfos.Length];
                for (int n = 0; n < paramInfos.Length; ++n)
                {
                    // 提取方法的参数
                    ParameterInfo paramInfo = paramInfos[n];
                    // 获取方法参数的类型
                    SystemType paramInfoType = paramInfo.ParameterType;

                    genericTypes[n] = paramInfoType;
                }

                // 创建一个通用的Action泛型对象
                SystemType genericActionType = CreateGenericActionType(paramInfos.Length);
                if (null == genericActionType)
                {
                    Debugger.Warn("No supported generic action type with parameters length '{%d}' for target method '{%t}', created action delegate failed.", paramInfos.Length, methodInfo);
                    return null;
                }

                // 指定泛型的类型
                SystemType actionType = genericActionType.MakeGenericType(genericTypes);
                // 为Action指定具体的实现
                // 需要注意的是，如果是非静态方法，此处的第二个参数需要传方法所在的实例（就是this指针），只有为静态方法第二个参数才传递null
                return SystemDelegate.CreateDelegate(actionType, self, methodInfo);
            }

            /// <summary>
            /// 创建指定函数的泛型包装代理函数，同时检查函数的参数是否与指定的类型匹配
            /// </summary>
            /// <param name="methodInfo">函数信息</param>
            /// <param name="parameterTypes">参数类型</param>
            /// <returns>若创建委托成功并符合指定参数类型则返回新创建的函数，否则返回null</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static SystemDelegate CreateGenericActionDelegateAndCheckParameterType(MethodInfo methodInfo, params SystemType[] parameterTypes)
            {
                return CreateGenericActionDelegateAndCheckParameterType(null ,methodInfo, parameterTypes);
            }

            /// <summary>
            /// 创建指定函数的泛型包装代理函数，同时检查函数的参数是否与指定的类型匹配
            /// </summary>
            /// <param name="self">函数调用的目标对象实例</param>
            /// <param name="methodInfo">函数信息</param>
            /// <param name="parameterTypes">参数类型</param>
            /// <returns>若创建委托成功并符合指定参数类型则返回新创建的函数，否则返回null</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static SystemDelegate CreateGenericActionDelegateAndCheckParameterType(object self, MethodInfo methodInfo, params SystemType[] parameterTypes)
            {
                Debugger.Verification.CheckGenericDelegateParameterTypeMatched(methodInfo, parameterTypes);

                // if (false == IsGenericDelegateParameterTypeMatched(methodInfo, parameterTypes)) return null;
                return CreateGenericActionDelegate(self, methodInfo);
            }

            /// <summary>
            /// 创建指定函数的泛型包装代理函数
            /// </summary>
            /// <param name="methodInfo">函数信息</param>
            /// <param name="outType">返回值类型</param>
            /// <param name="paramTypes">参数类型</param>
            /// <returns>返回包装后的代理函数</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static SystemDelegate CreateGenericFuncDelegate(MethodInfo methodInfo)
            {
                return CreateGenericFuncDelegate(null, methodInfo);
            }

            /// <summary>
            /// 创建指定函数的泛型包装代理函数
            /// </summary>
            /// <param name="self">函数调用的目标对象实例</param>
            /// <param name="methodInfo">函数信息</param>
            /// <param name="outType">返回值类型</param>
            /// <param name="paramTypes">参数类型</param>
            /// <returns>返回包装后的代理函数</returns>
            public static SystemDelegate CreateGenericFuncDelegate(object self, MethodInfo methodInfo)
            {
                // 2025-09-28：
                // 允许普通的成员函数在初始构建时不传入自身实例，而在实际调用时传入
                // 2025-11-29：
                // 通过测试后发现，通过普通函数创建的委托代理句柄，在创建时不指明对象实例，
                // 而在`DynamicInvoke`调用时再传入自身对象实例，
                // 虽然在编辑器环境下可以正常执行，但打包后执行会弹出异常：
                // ArgumentException: Delegate to an instance method cannot have null 'this'.
                // 所以此处的检测需要重新打开
                if (null == self && false == methodInfo.IsStatic)
                {
                    Logger.Error("The target method '{%t}' wasn't static function, the 'self' object must be non-null.", methodInfo);
                    return null;
                }

                SystemType returnType = methodInfo.ReturnType;
                if (null == returnType)
                {
                    Logger.Error("The target method '{%t}' must be has return value, created delegate failed.", methodInfo);
                    return null;
                }

                ParameterInfo[] paramInfos = methodInfo.GetParameters();
                if (null == paramInfos || paramInfos.Length <= 0)
                {
                    // 创建一个空参的Func对象
                    SystemType emptyGenericFuncType = CreateGenericFuncType(0);
                    // 指定泛型的类型
                    SystemType emptyFuncType = emptyGenericFuncType.MakeGenericType(returnType);

                    return SystemDelegate.CreateDelegate(emptyFuncType, self, methodInfo);
                }

                SystemType[] genericTypes = new SystemType[paramInfos.Length + 1];
                for (int n = 0; n < paramInfos.Length; ++n)
                {
                    // 提取方法的参数
                    ParameterInfo paramInfo = paramInfos[n];
                    // 获取方法参数的类型
                    SystemType paramInfoType = paramInfo.ParameterType;

                    genericTypes[n] = paramInfoType;
                }

                genericTypes[paramInfos.Length] = returnType;

                // 创建一个通用的Action泛型对象
                SystemType genericFuncType = CreateGenericFuncType(paramInfos.Length);
                if (null == genericFuncType)
                {
                    Debugger.Warn("No supported generic func type with parameters length '{%d}' for target method '{%t}', created func delegate failed.", paramInfos.Length, methodInfo);
                    return null;
                }

                // 指定泛型的类型
                SystemType funcType = genericFuncType.MakeGenericType(genericTypes);
                // 为Func指定具体的实现
                // 需要注意的是，如果是非静态方法，此处的第二个参数需要传方法所在的实例（就是this指针），只有为静态方法第二个参数才传递null
                return SystemDelegate.CreateDelegate(funcType, self, methodInfo);
            }

            /// <summary>
            /// 创建指定函数的泛型包装代理函数，同时检查函数的参数是否与指定的类型匹配
            /// </summary>
            /// <param name="methodInfo">函数信息</param>
            /// <param name="parameterTypes">参数类型</param>
            /// <returns>若创建委托成功并符合指定参数类型则返回新创建的函数，否则返回null</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static SystemDelegate CreateGenericFuncDelegateAndCheckParameterAndReturnType(MethodInfo methodInfo, SystemType returnType, params SystemType[] parameterTypes)
            {
                return CreateGenericFuncDelegateAndCheckParameterAndReturnType(null, methodInfo, returnType, parameterTypes);
            }

            /// <summary>
            /// 创建指定函数的泛型包装代理函数，同时检查函数的参数是否与指定的类型匹配
            /// </summary>
            /// <param name="self">函数调用的目标对象实例</param>
            /// <param name="methodInfo">函数信息</param>
            /// <param name="parameterTypes">参数类型</param>
            /// <returns>若创建委托成功并符合指定参数类型则返回新创建的函数，否则返回null</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static SystemDelegate CreateGenericFuncDelegateAndCheckParameterAndReturnType(object self, MethodInfo methodInfo, SystemType returnType, params SystemType[] parameterTypes)
            {
                Debugger.Verification.CheckGenericDelegateParameterAndReturnTypeMatched(methodInfo, returnType, parameterTypes);

                // if (false == IsGenericDelegateParameterAndReturnTypeMatched(methodInfo, returnType, parameterTypes)) return null;
                return CreateGenericFuncDelegate(self, methodInfo);
            }

            /// <summary>
            /// 创建一个指定参数数量的Action泛型类型
            /// </summary>
            /// <param name="paramCount">参数数量</param>
            /// <returns>返回Action泛型类型</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static SystemType CreateGenericActionType(int paramCount)
            {
                return paramCount switch
                {
                    0 => typeof(System.Action),
                    1 => typeof(System.Action<>),
                    2 => typeof(System.Action<,>),
                    3 => typeof(System.Action<,,>),
                    4 => typeof(System.Action<,,,>),
                    5 => typeof(System.Action<,,,,>),
                    6 => typeof(System.Action<,,,,,>),
                    7 => typeof(System.Action<,,,,,,>),
                    8 => typeof(System.Action<,,,,,,,>),
                    9 => typeof(System.Action<,,,,,,,,>),
                    _ => null,
                };
            }

            /// <summary>
            /// 创建一个指定参数数量的Func泛型类型
            /// </summary>
            /// <param name="paramCount">参数数量</param>
            /// <returns>返回Func泛型类型</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static SystemType CreateGenericFuncType(int paramCount)
            {
                return paramCount switch
                {
                    0 => typeof(System.Func<>),
                    1 => typeof(System.Func<,>),
                    2 => typeof(System.Func<,,>),
                    3 => typeof(System.Func<,,,>),
                    4 => typeof(System.Func<,,,,>),
                    5 => typeof(System.Func<,,,,,>),
                    6 => typeof(System.Func<,,,,,,>),
                    7 => typeof(System.Func<,,,,,,,>),
                    8 => typeof(System.Func<,,,,,,,,>),
                    9 => typeof(System.Func<,,,,,,,,,>),
                    _ => null,
                };
            }

            /// <summary>
            /// 创建指定函数的泛型包装代理函数
            /// </summary>
            /// <typeparam name="T">参数类型</typeparam>
            /// <param name="methodInfo">函数信息</param>
            /// <param name="paramType">参数对象类型</param>
            /// <returns>返回包装后的代理函数</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static System.Action<T> CreateGenericAction<T>(MethodInfo methodInfo, SystemType paramType)
            {
                return CreateGenericAction<T>(null, methodInfo, paramType);
            }

            /// <summary>
            /// 创建指定函数的泛型包装代理函数
            /// </summary>
            /// <typeparam name="T">参数类型</typeparam>
            /// <param name="self">函数调用的目标对象实例</param>
            /// <param name="methodInfo">函数信息</param>
            /// <param name="paramType">参数对象类型</param>
            /// <returns>返回包装后的代理函数，若创建失败返回null</returns>
            public static System.Action<T> CreateGenericAction<T>(object self, MethodInfo methodInfo, SystemType paramType)
            {
                //if (null == self && false == methodInfo.IsStatic)
                //{
                //    Logger.Error("The target method '{%t}' wasn't static function, the 'self' object must be non-null.", methodInfo);
                //    return null;
                //}

                //ParameterInfo[] paramInfos = methodInfo.GetParameters();
                //if (null == paramInfos || paramInfos.Length != 1)
                //{
                //    Logger.Error("The target method '{%t}' parameter size must be only one, method arguments was invalid.", methodInfo);
                //    return null;
                //}

                //if (false == typeof(T).IsAssignableFrom(paramInfos[0].ParameterType))
                //{
                //    Logger.Error("The target method '{%t}' parameter type '{%t}' must be assignable from '{%t}', generated action failed.",
                //            methodInfo, paramInfos[0].ParameterType, typeof(T));
                //    return null;
                //}

                //ParameterExpression expressionParam = Expression.Parameter(typeof(T));
                //UnaryExpression expressionParamUnary = Expression.Convert(expressionParam, paramType);

                //MethodCallExpression expressionMethodCall = null;
                //if (null == self)
                //{
                //    // 如果是静态函数，此处绑定的表达式实例为null
                //    expressionMethodCall = Expression.Call(null, methodInfo, expressionParamUnary);
                //}
                //else
                //{
                //    // 如果是非静态函数，此处需要绑定表达式对应的实例（就是this指针）
                //    expressionMethodCall = Expression.Call(Expression.Constant(self), methodInfo, expressionParamUnary);
                //}

                //Expression<System.Action<T>> expressionActionCall = Expression.Lambda<System.Action<T>>(expressionMethodCall, expressionParam);
                // 编译为Action的具体实现
                //return expressionActionCall.Compile();

                return (System.Action<T>) CreateGenericAction(self, methodInfo, new SystemType[] { typeof(T) }, new SystemType[] { paramType });
            }

            /// <summary>
            /// 创建指定函数的泛型包装代理函数，同时检查函数的参数是否与指定的类型匹配
            /// </summary>
            /// <param name="methodInfo">函数信息</param>
            /// <param name="parameterType">参数类型</param>
            /// <returns>若创建委托成功并符合指定参数类型则返回新创建的函数，否则返回null</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static System.Action<T> CreateGenericActionAndCheckParameterType<T>(MethodInfo methodInfo, SystemType parameterType)
            {
                return CreateGenericActionAndCheckParameterType<T>(null, methodInfo, parameterType);
            }

            /// <summary>
            /// 创建指定函数的泛型包装代理函数，同时检查函数的参数是否与指定的类型匹配
            /// </summary>
            /// <param name="self">函数调用的目标对象实例</param>
            /// <param name="methodInfo">函数信息</param>
            /// <param name="parameterType">参数类型</param>
            /// <returns>若创建委托成功并符合指定参数类型则返回新创建的函数，否则返回null</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static System.Action<T> CreateGenericActionAndCheckParameterType<T>(object self, MethodInfo methodInfo, SystemType parameterType)
            {
                Debugger.Verification.CheckGenericDelegateParameterTypeMatched(methodInfo, parameterType);

                // if (false == IsGenericDelegateParameterTypeMatched(methodInfo, parameterTypes)) return null;
                return CreateGenericAction<T>(self, methodInfo, parameterType);
            }

            /// <summary>
            /// 创建指定函数的泛型包装代理函数
            /// </summary>
            /// <typeparam name="T1">参数1类型</typeparam>
            /// <typeparam name="T2">参数2类型</typeparam>
            /// <param name="methodInfo">函数信息</param>
            /// <param name="paramTypes">参数类型列表</param>
            /// <returns>返回包装后的代理函数，若创建失败返回null</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static System.Action<T1, T2> CreateGenericAction<T1, T2>(MethodInfo methodInfo, params SystemType[] paramTypes)
            {
                return CreateGenericAction<T1, T2>(null, methodInfo, paramTypes);
            }

            /// <summary>
            /// 创建指定函数的泛型包装代理函数
            /// </summary>
            /// <typeparam name="T1">参数1类型</typeparam>
            /// <typeparam name="T2">参数2类型</typeparam>
            /// <param name="self">函数调用的目标对象实例</param>
            /// <param name="methodInfo">函数信息</param>
            /// <param name="paramTypes">参数类型列表</param>
            /// <returns>返回包装后的代理函数，若创建失败返回null</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static System.Action<T1, T2> CreateGenericAction<T1, T2>(object self, MethodInfo methodInfo, params SystemType[] paramTypes)
            {
                return (System.Action<T1, T2>) CreateGenericAction(self, methodInfo, new SystemType[] { typeof(T1), typeof(T2) }, paramTypes);
            }

            /// <summary>
            /// 创建指定函数的泛型包装代理函数，同时检查函数的参数是否与指定的类型匹配
            /// </summary>
            /// <param name="methodInfo">函数信息</param>
            /// <param name="parameterTypes">参数类型</param>
            /// <returns>若创建委托成功并符合指定参数类型则返回新创建的函数，否则返回null</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static System.Action<T1, T2> CreateGenericActionAndCheckParameterType<T1, T2>(MethodInfo methodInfo, params SystemType[] parameterTypes)
            {
                return CreateGenericActionAndCheckParameterType<T1, T2>(null, methodInfo, parameterTypes);
            }

            /// <summary>
            /// 创建指定函数的泛型包装代理函数，同时检查函数的参数是否与指定的类型匹配
            /// </summary>
            /// <param name="self">函数调用的目标对象实例</param>
            /// <param name="methodInfo">函数信息</param>
            /// <param name="parameterTypes">参数类型</param>
            /// <returns>若创建委托成功并符合指定参数类型则返回新创建的函数，否则返回null</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static System.Action<T1, T2> CreateGenericActionAndCheckParameterType<T1, T2>(object self, MethodInfo methodInfo, params SystemType[] parameterTypes)
            {
                Debugger.Verification.CheckGenericDelegateParameterTypeMatched(methodInfo, parameterTypes);

                // if (false == IsGenericDelegateParameterTypeMatched(methodInfo, parameterTypes)) return null;
                return CreateGenericAction<T1, T2>(self, methodInfo, parameterTypes);
            }

            /// <summary>
            /// 创建指定函数的动态泛型包装代理函数
            /// </summary>
            /// <param name="self">函数调用的目标对象实例</param>
            /// <param name="methodInfo">函数信息</param>
            /// <param name="genericTypes">泛型类型列表</param>
            /// <param name="paramTypes">参数类型列表</param>
            /// <returns>返回包装后的代理函数，若创建失败返回null</returns>
            private static SystemDelegate CreateGenericAction(object self, MethodInfo methodInfo, SystemType[] genericTypes, SystemType[] paramTypes)
            {
                if (null == self && false == methodInfo.IsStatic)
                {
                    Logger.Error("The target method '{%t}' wasn't static function, the 'self' object must be non-null.", methodInfo);
                    return null;
                }

                int length = genericTypes.Length;

                if (length > 0)
                {
                    ParameterInfo[] paramInfos = methodInfo.GetParameters();
                    if (null == paramInfos || paramInfos.Length != length)
                    {
                        Logger.Error("The target method '{%t}' parameter size must be equal to '{%d}', method arguments was invalid.", methodInfo, length);
                        return null;
                    }

                    if (null == paramTypes || paramTypes.Length != length)
                    {
                        Logger.Error("The target method '{%t}' adapter parameter type size must be equal to '{%d}', method arguments was invalid.", methodInfo, length);
                        return null;
                    }

                    for (int n = 0; n < length; ++n)
                    {
                        // 函数参数校验
                        if (false == genericTypes[n].IsAssignableFrom(paramInfos[n].ParameterType))
                        {
                            Logger.Error("The target method '{%t}' parameter type '{%t}' must be assignable from '{%t}', generated action failed.",
                                    methodInfo, paramInfos[n].ParameterType, genericTypes[n]);
                            return null;
                        }

                        // 传递参数类型校验
                        if (false == genericTypes[n].IsAssignableFrom(paramTypes[n]))
                        {
                            Logger.Error("The target method '{%t}' adapter parameter type '{%t}' must be assignable from '{%t}', generated action failed.",
                                    methodInfo, paramTypes[n], genericTypes[n]);
                            return null;
                        }
                    }
                }

                ParameterExpression[] expressionParams = new ParameterExpression[length];
                UnaryExpression[] expressionUnarys = new UnaryExpression[length];
                for (int n = 0; n < length; ++n)
                {
                    expressionParams[n] = Expression.Parameter(genericTypes[n]);
                    expressionUnarys[n] = Expression.Convert(expressionParams[n], paramTypes[n]);
                }

                MethodCallExpression expressionMethodCall = null;
                if (null == self)
                {
                    // 如果是静态函数，此处绑定的表达式实例为null
                    expressionMethodCall = Expression.Call(null, methodInfo, expressionUnarys);
                }
                else
                {
                    // 如果是非静态函数，此处需要绑定表达式对应的实例（就是this指针）
                    expressionMethodCall = Expression.Call(Expression.Constant(self), methodInfo, expressionUnarys);
                }

                SystemType actionType = CreateGenericActionType(length);
                SystemType genericActionType = actionType.MakeGenericType(genericTypes);
                LambdaExpression expressionActionCall = Expression.Lambda(genericActionType, expressionMethodCall, expressionParams);

                // 编译为Action的具体实现
                return expressionActionCall.Compile();
            }

            #endregion
        }
    }
}
