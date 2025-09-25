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

namespace GameEngine.Loader.Structuring
{
    /// <summary>
    /// 针对编码信息结构类型对象的格式化辅助工具类，通过该类定义一些用于编码信息结构对象的格式化接口函数
    /// </summary>
    internal static class Formatter
    {
        private static string ToString(MethodTypeCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("Fullname={%s},", targetObject.Fullname);
            fsb.Append("TargetType={%t},", targetObject.TargetType);
            fsb.Append("MethodInfo={%t},", targetObject.Method);
            return fsb.ToString();
        }

        #region 输入回调模块相关的编码信息结构类型对象“ToString”封装

        public static string ToString(InputCallCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("InputCall={");
            fsb.Append("ClassType={%t},", targetObject.ClassType);
            fsb.Append("MethodTypes={{{%s}}},", targetObject.MethodTypes?.Values(), ToString);
            return fsb.ToString();
        }

        private static string ToString(InputCallMethodTypeCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append(ToString((MethodTypeCodeInfo) targetObject));
            fsb.Append("InputCode={%d},", targetObject.InputCode);
            fsb.Append("OperationType={%v},", targetObject.OperationType);
            fsb.Append("InputDataType={%t},", targetObject.InputDataType);
            return fsb.ToString();
        }

        private static string ToString(InputResponsingMethodTypeCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append(ToString((InputCallMethodTypeCodeInfo) targetObject));
            fsb.Append("BehaviourType={%v},", targetObject.BehaviourType);
            return fsb.ToString();
        }

        #endregion

        #region 事件回调模块相关的编码信息结构类型对象“ToString”封装

        public static string ToString(EventCallCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("EventCall={");
            fsb.Append("ClassType={%t},", targetObject.ClassType);
            fsb.Append("MethodTypes={{{%s}}},", targetObject.MethodTypes?.Values(), ToString);
            fsb.Append("}");
            return fsb.ToString();
        }

        private static string ToString(EventCallMethodTypeCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append(ToString((MethodTypeCodeInfo) targetObject));
            fsb.Append("EventID={%d},", targetObject.EventID);
            fsb.Append("EventDataType={%t},", targetObject.EventDataType);
            return fsb.ToString();
        }

        private static string ToString(EventSubscribingMethodTypeCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append(ToString((EventCallMethodTypeCodeInfo) targetObject));
            fsb.Append("BehaviourType={%v},", targetObject.BehaviourType);
            return fsb.ToString();
        }

        #endregion

        #region 消息回调模块相关的编码信息结构类型对象“ToString”封装

        public static string ToString(NetworkMessageCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("NetworkMessage={");
            fsb.Append("ClassType={%t},", targetObject.ClassType);
            fsb.Append("Opcode={%d},", targetObject.Opcode);
            fsb.Append("ResponseCode={%d},", targetObject.ResponseCode);
            fsb.Append("}");
            return fsb.ToString();
        }

        public static string ToString(MessageCallCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("MessageCall={");
            fsb.Append("ClassType={%t},", targetObject.ClassType);
            fsb.Append("MethodTypes={{{%s}}},", targetObject.MethodTypes?.Values(), ToString);
            fsb.Append("}");
            return fsb.ToString();
        }

        private static string ToString(MessageCallMethodTypeCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append(ToString((MethodTypeCodeInfo) targetObject));
            fsb.Append("Opcode={%d},", targetObject.Opcode);
            fsb.Append("MessageType={%t},", targetObject.MessageType);
            return fsb.ToString();
        }

        private static string ToString(MessageBindingMethodTypeCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append(ToString((MessageCallMethodTypeCodeInfo) targetObject));
            fsb.Append("BehaviourType={%v},", targetObject.BehaviourType);
            return fsb.ToString();
        }

        #endregion

        #region API回调模块相关的编码信息结构类型对象“ToString”封装

        public static string ToString(ApiCallCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("ApiCall={");
            fsb.Append("ClassType={%t},", targetObject.ClassType);
            fsb.Append("MethodTypes={{{%s}}},", targetObject.MethodTypes?.Values(), ToString);
            fsb.Append("}");
            return fsb.ToString();
        }

        private static string ToString(ApiCallMethodTypeCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append(ToString((MethodTypeCodeInfo) targetObject));
            fsb.Append("FunctionName={%s},", targetObject.FunctionName);
            return fsb.ToString();
        }

        #endregion

        #region 切面回调模块相关的编码信息结构类型对象“ToString”封装

        public static string ToString(AspectCallCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("AspectCall={");
            fsb.Append("ClassType={%t},", targetObject.ClassType);
            fsb.Append("MethodTypes={{{%s}}},", targetObject.MethodTypes?.Values(), ToString);
            fsb.Append("}");
            return fsb.ToString();
        }

        private static string ToString(AspectCallMethodTypeCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append(ToString((MethodTypeCodeInfo) targetObject));
            fsb.Append("MethodName={%s},", targetObject.MethodName);
            fsb.Append("AccessType={%v},", targetObject.AccessType);
            return fsb.ToString();
        }

        #endregion

        #region 注入回调模块相关的编码信息结构类型对象“ToString”封装

        public static string ToString(InjectCallCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("InjectCall={");
            fsb.Append("ClassType={%t},", targetObject.ClassType);
            fsb.Append("BehaviourType={%v},", targetObject.BehaviourType);
            fsb.Append("}");
            return fsb.ToString();
        }

        #endregion

        #region 缓存回调模块相关的编码信息结构类型对象“ToString”封装

        public static string ToString(PoolCallCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("PoolCall={");
            fsb.Append("ClassType={%t},", targetObject.ClassType);
            fsb.Append("}");
            return fsb.ToString();
        }

        #endregion

        #region 原型对象模块相关的编码信息结构类型对象“ToString”封装

        private static string ToString(BaseBeanCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("ClassType={%t},", targetObject.ClassType);
            fsb.Append("InputResponsingMethodTypes={{{%s}}},", targetObject.InputResponsingMethodTypes?.Values(), ToString);
            fsb.Append("EventSubscribingMethodTypes={{{%s}}},", targetObject.EventSubscribingMethodTypes?.Values(), ToString);
            fsb.Append("MessageBindingMethodTypes={{{%s}}},", targetObject.MessageBindingMethodTypes?.Values(), ToString);
            return fsb.ToString();
        }

        private static string ToString(RefCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append(ToString((BaseBeanCodeInfo) targetObject));
            return fsb.ToString();
        }

        public static string ToString(ObjectCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("Object={");
            fsb.Append(ToString((RefCodeInfo) targetObject));
            fsb.Append("Name={%s},", targetObject.ObjectName);
            fsb.Append("Priority={%d},", targetObject.Priority);
            fsb.Append("}");
            return fsb.ToString();
        }

        public static string ToString(ComponentCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("Component={");
            fsb.Append(ToString((BaseBeanCodeInfo) targetObject));
            fsb.Append("Name={%s},", targetObject.ComponentName);
            fsb.Append("}");
            return fsb.ToString();
        }

        private static string ToString(EntityCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append(ToString((RefCodeInfo) targetObject));
            return fsb.ToString();
        }

        public static string ToString(ActorCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("Actor={");
            fsb.Append(ToString((EntityCodeInfo) targetObject));
            fsb.Append("Name={%s},", targetObject.ActorName);
            fsb.Append("Priority={%d},", targetObject.Priority);
            fsb.Append("}");
            return fsb.ToString();
        }

        public static string ToString(SceneCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("Scene={");
            fsb.Append(ToString((EntityCodeInfo) targetObject));
            fsb.Append("Name={%s},", targetObject.SceneName);
            fsb.Append("Priority={%d},", targetObject.Priority);
            fsb.Append("AutoDisplayViews={{{%s}}},", targetObject.AutoDisplayViewNames);
            fsb.Append("}");
            return fsb.ToString();
        }

        public static string ToString(ViewCodeInfo targetObject)
        {
            NovaEngine.FormatStringBuilder fsb = NovaEngine.FormatStringBuilder.Create();
            fsb.Append("View={");
            fsb.Append(ToString((EntityCodeInfo) targetObject));
            fsb.Append("Name={%s},", targetObject.ViewName);
            fsb.Append("Priority={%d},", targetObject.Priority);
            fsb.Append("GroupViews={{{%s}}},", targetObject.GroupOfSymbioticViewNames);
            fsb.Append("}");
            return fsb.ToString();
        }

        #endregion

        #region 扩展定义模块相关的编码信息结构类型对象“ToString”封装

        public static string ToString(ExtendCallCodeInfo targetObject)
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
