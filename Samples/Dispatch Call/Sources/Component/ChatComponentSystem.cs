/// -------------------------------------------------------------------------------
/// NovaEngine Framework Samples
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

using System.Collections.Generic;

namespace GameEngine.Sample.DispatchCall
{
    /// <summary>
    /// 聊天组件逻辑类
    /// </summary>
    public static class ChatComponentSystem
    {
        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Awake)]
        static void Awake(this ChatComponent self)
        {
            self.messages = new List<string>();
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Start)]
        static void Start(this ChatComponent self)
        {
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Update)]
        static void Update(this ChatComponent self)
        {
            if (null != self.messages && self.messages.Count > 0)
            {
                if (self.last_chat_time > NovaEngine.Timestamp.RealtimeSinceStartup)
                {
                    return;
                }

                self.last_chat_time = NovaEngine.Timestamp.RealtimeSinceStartup + 2.0f;
                string text = self.messages[0];
                self.messages.RemoveAt(0);

                Debugger.Info("角色对象‘{%s}’说：{%s}", ((Soldier) self.Entity).GetComponent<IdentityComponent>().objectName, text);
            }
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Destroy)]
        static void Destroy(this ChatComponent self)
        {
            self.messages.Clear();
            self.messages = null;
        }

        [GameEngine.MessageListenerBindingOfTarget(typeof(ActorChatResp))]
        static void OnChatResp(this ChatComponent self, ActorChatResp message)
        {
            for (int n = 0; null != message.ChatList && n < message.ChatList.Count; ++n)
            {
                Debugger.Info("角色对象‘{%s}’收到聊天信息：{%s}", ((Soldier) self.Entity).GetComponent<IdentityComponent>().objectName, message.ChatList[n].Text);
                self.AddChat(message.ChatList[n].Text);
            }
        }

        public static void AddChat(this ChatComponent self, string text)
        {
            self.messages ??= new List<string>();
            self.messages.Add(text);
        }
    }
}
