/// -------------------------------------------------------------------------------
/// GameEngine Framework
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

using SystemDateTime = System.DateTime;

namespace GameEngine
{
    /// <summary>
    /// 视图模块统计项对象类，对视图模块访问记录进行单项统计的数据单元
    /// </summary>
    public sealed class ViewStatInfo : IStatInfo
    {
        /// <summary>
        /// 视图记录索引标识
        /// </summary>
        private readonly int _uid;
        /// <summary>
        /// 视图名称
        /// </summary>
        private readonly string _viewName;
        /// <summary>
        /// 视图的哈希码，用来确保视图唯一性
        /// </summary>
        private readonly int _hashCode;
        /// <summary>
        /// 视图的创建时间
        /// </summary>
        private SystemDateTime _createTime;
        /// <summary>
        /// 视图的关闭时间
        /// </summary>
        private SystemDateTime _closeTime;

        public ViewStatInfo(int uid, string viewName, int hashCode)
        {
            _uid = uid;
            _viewName = viewName;
            _hashCode = hashCode;
            _createTime = SystemDateTime.MinValue;
            _closeTime = SystemDateTime.MinValue;

        }

        public int Uid
        {
            get { return _uid; }
        }

        public string ViewName
        {
            get { return _viewName; }
        }

        public int HashCode
        {
            get { return _hashCode; }
        }

        public SystemDateTime CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; }
        }

        public SystemDateTime CloseTime
        {
            get { return _closeTime; }
            set { _closeTime = value; }
        }
    }
}
