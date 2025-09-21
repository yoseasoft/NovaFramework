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
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader.Structuring
{
    /// <summary>
    /// 针对编码信息结构类型对象的格式化辅助工具类，通过该类定义一些用于编码信息结构对象的格式化接口函数
    /// </summary>
    internal static class Formatter
    {
        private static string ToString(MethodTypeCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.AppendFormat("Fullname={0},", targetObject.Fullname);
            sb.AppendFormat("TargetType={0},", NovaEngine.Utility.Text.ToString(targetObject.TargetType));
            sb.AppendFormat("MethodInfo={0},", NovaEngine.Utility.Text.ToString(targetObject.Method));
            return sb.ToString();
        }

        #region 输入回调模块相关的编码信息结构类型对象“ToString”封装

        public static string ToString(InputCallCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("InputCall={");
            sb.AppendFormat("ClassType={0},", NovaEngine.Utility.Text.ToString(targetObject.ClassType));
            sb.AppendFormat("MethodTypes={{{0}}},", NovaEngine.Utility.Text.ToString<InputCallMethodTypeCodeInfo>(targetObject.MethodTypes?.Values(), (n, obj) =>
            {
                return $"{n}={{{ToString(obj)}}}";
            }));
            sb.Append("}");
            return sb.ToString();
        }

        private static string ToString(InputCallMethodTypeCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append(ToString((MethodTypeCodeInfo) targetObject));
            sb.AppendFormat("InputCode={0},", targetObject.InputCode);
            sb.AppendFormat("OperationType={0},", targetObject.OperationType);
            sb.AppendFormat("InputDataType={0},", NovaEngine.Utility.Text.ToString(targetObject.InputDataType));
            return sb.ToString();
        }

        private static string ToString(InputResponsingMethodTypeCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append(ToString((InputCallMethodTypeCodeInfo) targetObject));
            sb.AppendFormat("BehaviourType={0},", targetObject.BehaviourType.ToString());
            return sb.ToString();
        }

        #endregion

        #region 事件回调模块相关的编码信息结构类型对象“ToString”封装

        public static string ToString(EventCallCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("EventCall={");
            sb.AppendFormat("ClassType={0},", NovaEngine.Utility.Text.ToString(targetObject.ClassType));
            sb.AppendFormat("MethodTypes={{{0}}},", NovaEngine.Utility.Text.ToString<EventCallMethodTypeCodeInfo>(targetObject.MethodTypes?.Values(), (n, obj) =>
            {
                return $"{n}={{{ToString(obj)}}}";
            }));
            sb.Append("}");
            return sb.ToString();
        }

        private static string ToString(EventCallMethodTypeCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append(ToString((MethodTypeCodeInfo) targetObject));
            sb.AppendFormat("EventID={0},", targetObject.EventID);
            sb.AppendFormat("EventDataType={0},", NovaEngine.Utility.Text.ToString(targetObject.EventDataType));
            return sb.ToString();
        }

        private static string ToString(EventSubscribingMethodTypeCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append(ToString((EventCallMethodTypeCodeInfo) targetObject));
            sb.AppendFormat("BehaviourType={0},", targetObject.BehaviourType.ToString());
            return sb.ToString();
        }

        #endregion

        #region 消息回调模块相关的编码信息结构类型对象“ToString”封装

        public static string ToString(NetworkMessageCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("NetworkMessage={");
            sb.AppendFormat("ClassType={0},", NovaEngine.Utility.Text.ToString(targetObject.ClassType));
            sb.AppendFormat("Opcode={0},", targetObject.Opcode);
            sb.AppendFormat("ResponseCode={0},", targetObject.ResponseCode);
            sb.Append("}");
            return sb.ToString();
        }

        public static string ToString(MessageCallCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("MessageCall={");
            sb.AppendFormat("ClassType={0},", NovaEngine.Utility.Text.ToString(targetObject.ClassType));
            sb.AppendFormat("MethodTypes={{{0}}},", NovaEngine.Utility.Text.ToString<MessageCallMethodTypeCodeInfo>(targetObject.MethodTypes?.Values(), (n, obj) =>
            {
                return $"{n}={{{ToString(obj)}}}";
            }));
            sb.Append("}");
            return sb.ToString();
        }

        private static string ToString(MessageCallMethodTypeCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append(ToString((MethodTypeCodeInfo) targetObject));
            sb.AppendFormat("Opcode={0},", targetObject.Opcode);
            sb.AppendFormat("MessageType={0},", NovaEngine.Utility.Text.ToString(targetObject.MessageType));
            return sb.ToString();
        }

        private static string ToString(MessageBindingMethodTypeCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append(ToString((MessageCallMethodTypeCodeInfo) targetObject));
            sb.AppendFormat("BehaviourType={0},", targetObject.BehaviourType.ToString());
            return sb.ToString();
        }

        #endregion

        #region API回调模块相关的编码信息结构类型对象“ToString”封装

        public static string ToString(ApiCallCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("ApiCall={");
            sb.AppendFormat("ClassType={0},", NovaEngine.Utility.Text.ToString(targetObject.ClassType));
            sb.AppendFormat("MethodTypes={{{0}}},", NovaEngine.Utility.Text.ToString<ApiCallMethodTypeCodeInfo>(targetObject.MethodTypes?.Values(), (n, obj) =>
            {
                return $"{n}={{{ToString(obj)}}}";
            }));
            sb.Append("}");
            return sb.ToString();
        }

        private static string ToString(ApiCallMethodTypeCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append(ToString((MethodTypeCodeInfo) targetObject));
            sb.AppendFormat("FunctionName={0},", targetObject.FunctionName);
            return sb.ToString();
        }

        #endregion

        #region 切面回调模块相关的编码信息结构类型对象“ToString”封装

        public static string ToString(AspectCallCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("AspectCall={");

            sb.AppendFormat("ClassType={0},", NovaEngine.Utility.Text.ToString(targetObject.ClassType));
            sb.AppendFormat("MethodTypes={{{0}}},", NovaEngine.Utility.Text.ToString<AspectCallMethodTypeCodeInfo>(targetObject.MethodTypes?.Values(), (n, info) =>
            {
                return $"{n}={{{ToString(info)}}}";
            }));

            sb.Append("}");
            return sb.ToString();
        }

        private static string ToString(AspectCallMethodTypeCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append(ToString((MethodTypeCodeInfo) targetObject));
            sb.AppendFormat("MethodName={0},", targetObject.MethodName ?? NovaEngine.Definition.CString.Unknown);
            sb.AppendFormat("AccessType={0},", targetObject.AccessType.ToString());
            return sb.ToString();
        }

        #endregion

        #region 注入回调模块相关的编码信息结构类型对象“ToString”封装

        public static string ToString(InjectCallCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("InjectCall={");
            sb.AppendFormat("ClassType={0},", NovaEngine.Utility.Text.ToString(targetObject.ClassType));
            sb.AppendFormat("BehaviourType={0},", targetObject.BehaviourType);
            sb.Append("}");
            return sb.ToString();
        }

        #endregion

        #region 缓存回调模块相关的编码信息结构类型对象“ToString”封装

        public static string ToString(PoolCallCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("PoolCall={");
            sb.AppendFormat("ClassType={0},", NovaEngine.Utility.Text.ToString(targetObject.ClassType));
            sb.Append("}");
            return sb.ToString();
        }

        #endregion

        #region 原型对象模块相关的编码信息结构类型对象“ToString”封装

        private static string ToString(BaseBeanCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.AppendFormat("ClassType={0},", NovaEngine.Utility.Text.ToString(targetObject.ClassType));
            sb.AppendFormat("InputResponsingMethodTypes={{{0}}},", NovaEngine.Utility.Text.ToString<InputResponsingMethodTypeCodeInfo>(targetObject.InputResponsingMethodTypes?.Values(), (n, obj) =>
            {
                return $"{n}={{{ToString(obj)}}}";
            }));
            sb.AppendFormat("EventSubscribingMethodTypes={{{0}}},", NovaEngine.Utility.Text.ToString<EventSubscribingMethodTypeCodeInfo>(targetObject.EventSubscribingMethodTypes?.Values(), (n, obj) =>
            {
                return $"{n}={{{ToString(obj)}}}";
            }));
            sb.AppendFormat("MessageBindingMethodTypes={{{0}}},", NovaEngine.Utility.Text.ToString<MessageBindingMethodTypeCodeInfo>(targetObject.MessageBindingMethodTypes?.Values(), (n, obj) =>
            {
                return $"{n}={{{ToString(obj)}}}";
            }));
            return sb.ToString();
        }

        private static string ToString(RefCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append(ToString((BaseBeanCodeInfo) targetObject));
            return sb.ToString();
        }

        public static string ToString(ObjectCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("Object={");
            sb.Append(ToString((RefCodeInfo) targetObject));
            sb.AppendFormat("Name={0},", targetObject.ObjectName ?? NovaEngine.Definition.CString.Unknown);
            sb.AppendFormat("Priority={0},", targetObject.Priority);
            sb.Append("}");
            return sb.ToString();
        }

        public static string ToString(ComponentCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("Component={");
            sb.Append(ToString((BaseBeanCodeInfo) targetObject));
            sb.AppendFormat("Name={0},", targetObject.ComponentName ?? NovaEngine.Definition.CString.Unknown);
            sb.Append("}");
            return sb.ToString();
        }

        private static string ToString(EntityCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append(ToString((RefCodeInfo) targetObject));
            return sb.ToString();
        }

        public static string ToString(ActorCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("Actor={");
            sb.Append(ToString((EntityCodeInfo) targetObject));
            sb.AppendFormat("Name={0},", targetObject.ActorName ?? NovaEngine.Definition.CString.Unknown);
            sb.AppendFormat("Priority={0},", targetObject.Priority);
            sb.Append("}");
            return sb.ToString();
        }

        public static string ToString(SceneCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("Scene={");
            sb.Append(ToString((EntityCodeInfo) targetObject));
            sb.AppendFormat("Name={0},", targetObject.SceneName ?? NovaEngine.Definition.CString.Unknown);
            sb.AppendFormat("Priority={0},", targetObject.Priority);
            sb.AppendFormat("AutoDisplayViews={{{0}}},", NovaEngine.Utility.Text.ToString(targetObject.AutoDisplayViewNames));
            sb.Append("}");
            return sb.ToString();
        }

        public static string ToString(ViewCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("View={");
            sb.Append(ToString((EntityCodeInfo) targetObject));
            sb.AppendFormat("Name={0},", targetObject.ViewName ?? NovaEngine.Definition.CString.Unknown);
            sb.AppendFormat("Priority={0},", targetObject.Priority);
            sb.AppendFormat("GroupViews={{{0}}},", NovaEngine.Utility.Text.ToString(targetObject.GroupOfSymbioticViewNames));
            sb.Append("}");
            return sb.ToString();
        }

        #endregion

        #region 扩展定义模块相关的编码信息结构类型对象“ToString”封装

        public static string ToString(ExtendCallCodeInfo targetObject)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("ExtendCall={");
            sb.AppendFormat("ClassType={0},", NovaEngine.Utility.Text.ToString(targetObject.ClassType));
            sb.AppendFormat("InputCallMethodTypes={{{0}}},", NovaEngine.Utility.Text.ToString<InputResponsingMethodTypeCodeInfo>(targetObject.InputCallMethodTypes?.Values(), (n, obj) =>
            {
                return $"{n}={{{ToString(obj)}}}";
            }));
            sb.AppendFormat("EventCallMethodTypes={{{0}}},", NovaEngine.Utility.Text.ToString<EventSubscribingMethodTypeCodeInfo>(targetObject.EventCallMethodTypes?.Values(), (n, obj) =>
            {
                return $"{n}={{{ToString(obj)}}}";
            }));
            sb.AppendFormat("MessageCallMethodTypes={{{0}}},", NovaEngine.Utility.Text.ToString<MessageBindingMethodTypeCodeInfo>(targetObject.MessageCallMethodTypes?.Values(), (n, obj) =>
            {
                return $"{n}={{{ToString(obj)}}}";
            }));
            sb.Append("}");
            return sb.ToString();
        }

        #endregion
    }
}
