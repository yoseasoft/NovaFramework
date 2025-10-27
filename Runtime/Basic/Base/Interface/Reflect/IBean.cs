/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemType = System.Type;

namespace GameEngine
{
    /// <summary>
    /// 可实例化对象接口类，用于对一个可实例化对象的类作标记，在类型判断或反射检测时以该标记作为依据<br/>
    /// 您可以通过继承该接口类，标记一个类是否为可实例化对象类，并参与到可实例化对象类的通用逻辑流程中<br/>
    /// 该抽象类不具备任何功能接口，仅作为一个检测标记而存在
    /// </summary>
    public interface IBean
    {
        /// <summary>
        /// 获取实体对象的标识
        /// </summary>
        int BeanId { get; }

        /// <summary>
        /// 获取实体对象的名称
        /// </summary>
        string BeanName { get; }

        /// <summary>
        /// 获取实体对象的类型
        /// </summary>
        SystemType BeanType { get; }
    }
}
