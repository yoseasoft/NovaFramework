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

using System.Collections.Generic;
using System.Reflection;

namespace GameEngine.HFSM
{
    /// <summary>
    /// 状态机构建器对象类，提供对状态机数据的自动装配
    /// </summary>
    internal static class StateMachineBuilder
    {
        /// <summary>
        /// 使用指定的根节点构建一个状态机对象实例
        /// </summary>
        /// <param name="root">根节点实例</param>
        /// <returns>返回一个新构建的状态机对象实例</returns>
        public static StateMachine Build(State root)
        {
            if (null == root) return null;

            StateMachine machine = new StateMachine(root);
            Wire(root, machine, new HashSet<State>());
            return machine;
        }

        static void Wire(State state, StateMachine machine, HashSet<State> visited)
        {
            if (null == state) return;
            if (!visited.Add(state)) return; // State is already wired

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
            FieldInfo machineField = typeof(State).GetField(State.NameOfStateMachineField, flags);
            if (null != machineField) machineField.SetValue(state, machine);

            foreach (FieldInfo field in state.GetType().GetFields(flags))
            {
                if (false == typeof(State).IsAssignableFrom(field.FieldType)) continue; // Only consider fields that are State
                if (State.NameOfParentStateField == field.Name) continue; // Skip back-edge to parent

                State child = (State) field.GetValue(state);
                if (null == child) continue;
                if (false == ReferenceEquals(child.Parent, state)) continue; // Ensure it's actually our direct child

                Wire(child, machine, visited);
            }
        }
    }
}
