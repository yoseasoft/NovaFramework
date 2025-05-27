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

namespace GameEngine
{
    /// <summary>
    /// Bean对象抽象类，对需要进行Bean对象定义的场景提供一个通用的基类
    /// </summary>
    public abstract partial class CBean : IProto, NovaEngine.IInitializable
    {
        /// <summary>
        /// 实体对象的名称
        /// </summary>
        private string m_beanName;

        /// <summary>
        /// 获取或设置实体对象的名称
        /// </summary>
        public string BeanName { get { return m_beanName; } internal set { m_beanName = value; } }

        /// <summary>
        /// 对象初始化函数接口
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// 对象清理函数接口
        /// </summary>
        public abstract void Cleanup();

        /// <summary>
        /// 获取当前对象实例的Bean名称，若尚未赋值，则返回此对象类型的默认Bean名称
        /// </summary>
        /// <returns>返回对象实例的Bean名称</returns>
        public string GetBeanNameOrDefault()
        {
            if (null == m_beanName)
            {
                Loader.Symboling.SymClass symClass = Loader.CodeLoader.GetSymClassByType(GetType());
                Debugger.Assert(null != symClass, "Could not found any symbol class with type '{0}'.", NovaEngine.Utility.Text.ToString(GetType()));

                return symClass.DefaultBeanName;
            }

            return m_beanName;
        }
    }
}
