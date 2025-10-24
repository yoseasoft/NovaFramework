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
    /// 针对编码信息结构类型对象的格式化辅助工具类，通过该类定义一些用于编码信息结构对象的格式化接口函数
    /// </summary>
    public static partial class CodeLoaderObject
    {
        private static string ToString(Structuring.MethodTypeCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("Fullname={%s},", targetObject.Fullname);
            fsb.Append("TargetType={%t},", targetObject.TargetType);
            fsb.Append("MethodInfo={%t},", targetObject.Method);
            return fsb.ToString();
        }

        #region 输入回调模块相关的编码信息结构类型对象“ToString”封装

        internal static string ToString(Structuring.InputCallCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("InputCall={");
            fsb.Append("ClassType={%t},", targetObject.ClassType);
            fsb.Append("MethodTypes={{{%s}}},", targetObject.MethodTypes?.Values(), ToString);
            return fsb.ToString();
        }

        private static string ToString(Structuring.InputCallMethodTypeCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append(ToString((Structuring.MethodTypeCodeInfo) targetObject));
            fsb.Append("InputCode={%d},", targetObject.InputCode);
            fsb.Append("OperationType={%i},", targetObject.OperationType);
            fsb.Append("InputDataType={%t},", targetObject.InputDataType);
            return fsb.ToString();
        }

        private static string ToString(Structuring.InputResponsingMethodTypeCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append(ToString((Structuring.InputCallMethodTypeCodeInfo) targetObject));
            fsb.Append("BehaviourType={%i},", targetObject.BehaviourType);
            return fsb.ToString();
        }

        #endregion

        #region 事件回调模块相关的编码信息结构类型对象“ToString”封装

        internal static string ToString(Structuring.EventCallCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("EventCall={");
            fsb.Append("ClassType={%t},", targetObject.ClassType);
            fsb.Append("MethodTypes={{{%s}}},", targetObject.MethodTypes?.Values(), ToString);
            fsb.Append("}");
            return fsb.ToString();
        }

        private static string ToString(Structuring.EventCallMethodTypeCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append(ToString((Structuring.MethodTypeCodeInfo) targetObject));
            fsb.Append("EventID={%d},", targetObject.EventID);
            fsb.Append("EventDataType={%t},", targetObject.EventDataType);
            return fsb.ToString();
        }

        private static string ToString(Structuring.EventSubscribingMethodTypeCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append(ToString((Structuring.EventCallMethodTypeCodeInfo) targetObject));
            fsb.Append("BehaviourType={%i},", targetObject.BehaviourType);
            return fsb.ToString();
        }

        #endregion

        #region 消息回调模块相关的编码信息结构类型对象“ToString”封装

        internal static string ToString(Structuring.NetworkMessageCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("NetworkMessage={");
            fsb.Append("ClassType={%t},", targetObject.ClassType);
            fsb.Append("Opcode={%d},", targetObject.Opcode);
            fsb.Append("ResponseCode={%d},", targetObject.ResponseCode);
            fsb.Append("}");
            return fsb.ToString();
        }

        internal static string ToString(Structuring.MessageCallCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("MessageCall={");
            fsb.Append("ClassType={%t},", targetObject.ClassType);
            fsb.Append("MethodTypes={{{%s}}},", targetObject.MethodTypes?.Values(), ToString);
            fsb.Append("}");
            return fsb.ToString();
        }

        private static string ToString(Structuring.MessageCallMethodTypeCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append(ToString((Structuring.MethodTypeCodeInfo) targetObject));
            fsb.Append("Opcode={%d},", targetObject.Opcode);
            fsb.Append("MessageType={%t},", targetObject.MessageType);
            return fsb.ToString();
        }

        private static string ToString(Structuring.MessageBindingMethodTypeCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append(ToString((Structuring.MessageCallMethodTypeCodeInfo) targetObject));
            fsb.Append("BehaviourType={%i},", targetObject.BehaviourType);
            return fsb.ToString();
        }

        #endregion

        #region API回调模块相关的编码信息结构类型对象“ToString”封装

        internal static string ToString(Structuring.ApiCallCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("ApiCall={");
            fsb.Append("ClassType={%t},", targetObject.ClassType);
            fsb.Append("MethodTypes={{{%s}}},", targetObject.MethodTypes?.Values(), ToString);
            fsb.Append("}");
            return fsb.ToString();
        }

        private static string ToString(Structuring.ApiCallMethodTypeCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append(ToString((Structuring.MethodTypeCodeInfo) targetObject));
            fsb.Append("FunctionName={%s},", targetObject.FunctionName);
            return fsb.ToString();
        }

        #endregion

        #region 切面回调模块相关的编码信息结构类型对象“ToString”封装

        internal static string ToString(Structuring.AspectCallCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("AspectCall={");
            fsb.Append("ClassType={%t},", targetObject.ClassType);
            fsb.Append("MethodTypes={{{%s}}},", targetObject.MethodTypes?.Values(), ToString);
            fsb.Append("}");
            return fsb.ToString();
        }

        private static string ToString(Structuring.AspectCallMethodTypeCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append(ToString((Structuring.MethodTypeCodeInfo) targetObject));
            fsb.Append("MethodName={%s},", targetObject.MethodName);
            fsb.Append("AccessType={%i},", targetObject.AccessType);
            return fsb.ToString();
        }

        #endregion

        #region 注入回调模块相关的编码信息结构类型对象“ToString”封装

        internal static string ToString(Structuring.InjectCallCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("InjectCall={");
            fsb.Append("ClassType={%t},", targetObject.ClassType);
            fsb.Append("BehaviourType={%i},", targetObject.BehaviourType);
            fsb.Append("}");
            return fsb.ToString();
        }

        #endregion

        #region 缓存回调模块相关的编码信息结构类型对象“ToString”封装

        internal static string ToString(Structuring.PoolCallCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("PoolCall={");
            fsb.Append("ClassType={%t},", targetObject.ClassType);
            fsb.Append("}");
            return fsb.ToString();
        }

        #endregion

        #region 原型对象模块相关的编码信息结构类型对象“ToString”封装

        private static string ToString(Structuring.BaseBeanCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("ClassType={%t},", targetObject.ClassType);
            fsb.Append("InputResponsingMethodTypes={{{%s}}},", targetObject.InputResponsingMethodTypes?.Values(), ToString);
            fsb.Append("EventSubscribingMethodTypes={{{%s}}},", targetObject.EventSubscribingMethodTypes?.Values(), ToString);
            fsb.Append("MessageBindingMethodTypes={{{%s}}},", targetObject.MessageBindingMethodTypes?.Values(), ToString);
            return fsb.ToString();
        }

        private static string ToString(Structuring.RefCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append(ToString((Structuring.BaseBeanCodeInfo) targetObject));
            return fsb.ToString();
        }

        internal static string ToString(Structuring.ObjectCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("Object={");
            fsb.Append(ToString((Structuring.RefCodeInfo) targetObject));
            fsb.Append("Name={%s},", targetObject.ObjectName);
            fsb.Append("Priority={%d},", targetObject.Priority);
            fsb.Append("}");
            return fsb.ToString();
        }

        internal static string ToString(Structuring.ComponentCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("Component={");
            fsb.Append(ToString((Structuring.BaseBeanCodeInfo) targetObject));
            fsb.Append("Name={%s},", targetObject.ComponentName);
            fsb.Append("}");
            return fsb.ToString();
        }

        private static string ToString(Structuring.EntityCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append(ToString((Structuring.RefCodeInfo) targetObject));
            fsb.Append("EntityName={%s},", targetObject.EntityName);
            fsb.Append("Priority={%d},", targetObject.Priority);
            return fsb.ToString();
        }

        internal static string ToString(Structuring.ActorCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("Actor={");
            fsb.Append(ToString((Structuring.EntityCodeInfo) targetObject));
            fsb.Append("}");
            return fsb.ToString();
        }

        internal static string ToString(Structuring.SceneCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("Scene={");
            fsb.Append(ToString((Structuring.EntityCodeInfo) targetObject));
            fsb.Append("AutoDisplayViews={{{%s}}},", targetObject.AutoDisplayViewNames);
            fsb.Append("}");
            return fsb.ToString();
        }

        internal static string ToString(Structuring.ViewCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("View={");
            fsb.Append(ToString((Structuring.EntityCodeInfo) targetObject));
            fsb.Append("GroupName={%s},", targetObject.GroupName);
            fsb.Append("FormType={%i},", targetObject.FormType);
            fsb.Append("NoticeMethodTypes={{{%s}}},", targetObject.NoticeMethodTypes?.Values(), ToString);
            fsb.Append("GroupViews={{{%s}}},", targetObject.GroupOfSymbioticViewNames);
            fsb.Append("}");
            return fsb.ToString();
        }

        #endregion

        #region 原型对象模块通知接口相关的编码信息结构类型对象“ToString”封装

        internal static string ToString(Structuring.CViewNoticeCallCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("CViewNoticeCall={");
            fsb.Append("ClassType={%t},", targetObject.ClassType);
            fsb.Append("MethodTypes={{{%s}}},", targetObject.MethodTypes?.Values(), ToString);
            fsb.Append("}");
            return fsb.ToString();
        }

        private static string ToString(Structuring.CViewNoticeMethodTypeCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append(ToString((Structuring.MethodTypeCodeInfo) targetObject));
            fsb.Append("NoticeType={%i},", targetObject.NoticeType);
            fsb.Append("BehaviourType={%i},", targetObject.BehaviourType);
            return fsb.ToString();
        }

        #endregion

        #region 扩展定义模块相关的编码信息结构类型对象“ToString”封装

        internal static string ToString(Structuring.ExtendCallCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("ExtendCall={");
            fsb.Append("ClassType={%t},", targetObject.ClassType);
            fsb.Append("InputCallMethodTypes={{{%s}}},", targetObject.InputCallMethodTypes?.Values(), ToString);
            fsb.Append("EventCallMethodTypes={{{%s}}},", targetObject.EventCallMethodTypes?.Values(), ToString);
            fsb.Append("MessageCallMethodTypes={{{%s}}},", targetObject.MessageCallMethodTypes?.Values(), ToString);
            fsb.Append("}");
            return fsb.ToString();
        }

        #endregion
    }
}
