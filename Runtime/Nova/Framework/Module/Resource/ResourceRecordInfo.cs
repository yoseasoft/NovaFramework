/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using UnityObject = UnityEngine.Object;

namespace NovaEngine
{
    /// <summary>
    /// 资源对象运行时信息数据封装对象类，对资源及相关实例化对象进行统一封装管理
    /// 其数据结构包含名称，资源包，实例化对象等
    /// </summary>
    internal sealed class ResourceRecordInfo
    {
        /// <summary>
        /// 资源唯一标识
        /// </summary>
        private int _uid = 0;

        /// <summary>
        /// 资源名称
        /// </summary>
        private string _name = string.Empty;

        /// <summary>
        /// 资源实例化对象
        /// </summary>
        private UnityObject _object;

        /// <summary>
        /// 资源信息对象构造函数
        /// </summary>
        private ResourceRecordInfo()
        { }

        /// <summary>
        /// 资源信息对象析构函数
        /// </summary>
        ~ResourceRecordInfo()
        { }

        /// <summary>
        /// 资源信息对象初始化回调函数
        /// </summary>
        private void Initialize()
        { }

        /// <summary>
        /// 资源信息对象清理回调函数
        /// </summary>
        private void Cleanup()
        { }

        /// <summary>
        /// 资源信息对象静态创建函数，通过给定名称创建一个资源对象实例
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <returns>若资源信息对象创建成功则返回对应的引用实例，否则返回null</returns>
        public static ResourceRecordInfo Create(string name)
        {
            ResourceRecordInfo ret = new ResourceRecordInfo();

            ret.Initialize();

            return ret;
        }

        /// <summary>
        /// 资源信息对象销毁函数
        /// </summary>
        public void Destroy()
        {
            Cleanup();
        }

        #region 资源信息对象基础属性Getter/Setter函数

        /// <summary>
        /// 资源唯一标识属性访问Getter/Setter接口
        /// </summary>
        public int Uid
        {
            set { _uid = value; }
            get { return _uid; }
        }

        /// <summary>
        /// 资源名称属性访问Getter/Setter接口
        /// </summary>
        public string Name
        {
            set { _name = value; }
            get { return _name; }
        }

        /// <summary>
        /// 资源对象属性访问Getter/Setter接口
        /// </summary>
        public UnityObject Object
        {
            set { _object = value; }
            get { return _object; }
        }

        #endregion
    }
}
