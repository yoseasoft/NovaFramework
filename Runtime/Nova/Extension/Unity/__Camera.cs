/// -------------------------------------------------------------------------------
/// NovaEngine Framework
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

namespace UnityEngine.Customize.Extension
{
    /// <summary>
    /// 基于Unity库相机类的扩展接口支持类
    /// </summary>
    public static class __Camera
    {
        /// <summary>
        /// 通过相机截取屏幕并转换为Texture2D
        /// </summary>
        /// <param name="self">目标相机</param>
        /// <returns>相机抓取的屏幕Texture2D</returns>
        public static Texture2D ScreenshotAsTextureRGB(this Camera self)
        {
            return ScreenshotAsTexture(self, TextureFormat.RGB565);
        }
        public static Texture2D ScreenshotAsTextureRGBA(this Camera self)
        {
            return ScreenshotAsTexture(self, TextureFormat.RGBA32);
        }
        public static Texture2D ScreenshotAsTexture(this Camera self, TextureFormat textureFormat)
        {
            RenderTexture oldRenderTexture = self.targetTexture;
            int width = self.pixelWidth;
            int height = self.pixelHeight;
            RenderTexture renderTexture = new RenderTexture(width, height, 24);
            self.targetTexture = renderTexture;
            self.Render();
            Texture2D texture2D = new Texture2D(width, height, textureFormat, false);
            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = null;
            self.targetTexture = oldRenderTexture;
            return texture2D;
        }

        /// <summary>
        /// 通过相机截取屏幕并转换为Sprite
        /// </summary>
        /// <param name="self">目标相机</param>
        /// <returns>相机抓取的屏幕Texture2D</returns>
        public static Sprite ScreenshotAsSpriteRGBA(this Camera self)
        {
            Texture2D texture2D = ScreenshotAsTextureRGBA(self);
            return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
        }
        public static Sprite ScreenshotAsSpriteRGB(this Camera self)
        {
            Texture2D texture2D = ScreenshotAsTextureRGB(self);
            return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
        }
        public static Sprite ScreenshotAsSprite(this Camera self, TextureFormat textureFormat)
        {
            Texture2D texture2D = ScreenshotAsTexture(self, textureFormat);
            return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
        }
    }
}
