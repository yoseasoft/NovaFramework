/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace GameEngine
{
    /// <summary>
    /// FairyGUI加载器的扩展实现类<br/>
    /// 通过扩展FairyGUI默认GLoader，使其可以加载外部图集
    /// </summary>
    public class FairyGLoader : FairyGUI.GLoader
    {
        /// <summary>
        /// 等待显示的URL标识, 不能使用空字符串, 因为若代码真正赋空没办法知道, 但_waitLoadUrl还在, 就会误加载回去
        /// </summary>
        private const string WaitingLoadTag = "wait://";

        /// <summary>
        /// 等待显示时加载的url
        /// </summary>
        private string _waitingLoadUrl;

        protected override void LoadExternal()
        {
            // 等待加载中, 不正式加载
            if (url.Equals(WaitingLoadTag))
                return;

            // 更换url时, 首先清空等待记录
            _waitingLoadUrl = null;

            // 每次设置url都重新添加监听, 以免放构造函数中途监听被清除掉(例如富文本图片回到池里面会清掉委托)
            onAddedToStage.Add(OnAddedToStage);
            onRemovedFromStage.Add(OnRemovedFromStage);

            if (!onStage)
            {
                _waitingLoadUrl = url;
                url = WaitingLoadTag;
                return;
            }

            FairyGuiHelper.LoadExternalIcon(url, OnLoadSuccess, OnLoadFailed);
        }

        /// <summary>
        /// 加载成功回调函数
        /// </summary>
        private void OnLoadSuccess(FairyGUI.NTexture nTexture, string textureUrl)
        {
            // 因为通常是异步的,所以加载完成后需要判断自身是否已销毁,url是否还相同
            if (isDisposed || string.IsNullOrEmpty(url) || url.Equals(WaitingLoadTag))
                return;

            string curUrl = url.Split(FairyGuiHelper.SplitSymbol)[0];
            if (!curUrl.Equals(textureUrl))
                return;

            if (!onStage)
            {
                _waitingLoadUrl = url;
                url = WaitingLoadTag;
                return;
            }

            onExternalLoadSuccess(nTexture);
        }

        /// <summary>
        /// 加载失败回调函数
        /// </summary>
        private void OnLoadFailed(string textureUrl)
        {
            // 因为通常是异步的,所以加载完成后需要判断自身是否已销毁,url是否还相同
            if (isDisposed || string.IsNullOrEmpty(url) || url.Equals(WaitingLoadTag))
                return;

            string curUrl = url.Split(FairyGuiHelper.SplitSymbol)[0];
            if (!curUrl.Equals(textureUrl))
                return;

            onExternalLoadFailed();

            if (NovaEngine.Environment.IsDevelopmentState())
            {
                Debugger.Error($"加载图片失败, 资源路径:{textureUrl}");
            }
        }

        /// <summary>
        /// 重新回到舞台处理(即窗口重新显示时调用)
        /// 贴图重新赋值
        /// </summary>
        private void OnAddedToStage(FairyGUI.EventContext _)
        {
            if (string.IsNullOrEmpty(_waitingLoadUrl))
                return;

            // 重新回到舞台时, url已被逻辑代码清空
            if (string.IsNullOrEmpty(url))
            {
                _waitingLoadUrl = null;
                return;
            }

            url = _waitingLoadUrl;
            _waitingLoadUrl = null;
        }

        /// <summary>
        /// 移出舞台处理(即窗口隐藏时调用)
        /// 清空外部加载贴图, 以免占用内存
        /// </summary>
        private void OnRemovedFromStage(FairyGUI.EventContext _)
        {
            if (string.IsNullOrEmpty(url) || url.StartsWith(FairyGUI.UIPackage.URL_PREFIX))
                return;

            _waitingLoadUrl = url;
            url = WaitingLoadTag;
        }
    }
}
