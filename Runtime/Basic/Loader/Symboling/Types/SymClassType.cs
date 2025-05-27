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

using SystemType = System.Type;

namespace GameEngine.Loader.Symboling
{
    /// <summary>
    /// 通用对象类的标记数据的集合类型<br/>
    /// 该类型是针对标记对象封装的映射容器，提供两种键的快速访问，可以提高访问效率
    /// </summary>
    public sealed class SymClassMap : NovaEngine.CompositeKeyMap<SystemType, string, SymClass>
    {
        /// <summary>
        /// 新增指定的标记对象实例到当前映射容器中
        /// </summary>
        /// <param name="symbol">标记对象实例</param>
        /// <returns>若新增实例成功返回true，否则返回false</returns>
        public bool Add(SymClass symbol)
        {
            return Add(symbol.ClassType, symbol.ClassName, symbol);
        }

        /// <summary>
        /// 从当前映射容器中移除指定的标记对象实例
        /// </summary>
        /// <param name="symbol">标记对象实例</param>
        public void Remove(SymClass symbol)
        {
            Remove(symbol.ClassType);
        }

        /// <summary>
        /// 检测当前映射容器中是否存在指定的标记对象实例
        /// </summary>
        /// <param name="symbol">标记对象实例</param>
        /// <returns>若存在指定标记对象实例则返回true，否则返回false</returns>
        public bool Contains(SymClass symbol)
        {
            return ContainsKey(symbol.ClassType);
        }
    }
}
