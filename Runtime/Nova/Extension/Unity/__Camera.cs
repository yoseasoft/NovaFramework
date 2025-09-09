/// -------------------------------------------------------------------------------
/// NovaEngine Framework
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

using UnityCamera = UnityEngine.Camera;
using UnityTexture2D = UnityEngine.Texture2D;
using UnityRenderTexture = UnityEngine.RenderTexture;
using UnitySprite = UnityEngine.Sprite;
using UnityVector2 = UnityEngine.Vector2;
using UnityRect = UnityEngine.Rect;
using UnityTextureFormat = UnityEngine.TextureFormat;

namespace NovaEngine
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
        public static UnityTexture2D ScreenshotAsTextureRGB(this UnityCamera self)
        {
            return ScreenshotAsTexture(self, UnityTextureFormat.RGB565);
        }
        public static UnityTexture2D ScreenshotAsTextureRGBA(this UnityCamera self)
        {
            return ScreenshotAsTexture(self, UnityTextureFormat.RGBA32);
        }
        public static UnityTexture2D ScreenshotAsTexture(this UnityCamera self, UnityTextureFormat textureFormat)
        {
            UnityRenderTexture oldRenderTexture = self.targetTexture;
            int width = self.pixelWidth;
            int height = self.pixelHeight;
            UnityRenderTexture renderTexture = new UnityRenderTexture(width, height, 24);
            self.targetTexture = renderTexture;
            self.Render();
            UnityTexture2D texture2D = new UnityTexture2D(width, height, textureFormat, false);
            UnityRenderTexture.active = renderTexture;
            texture2D.ReadPixels(new UnityRect(0f, 0f, width, height), 0, 0);
            texture2D.Apply();
            UnityRenderTexture.active = null;
            self.targetTexture = oldRenderTexture;
            return texture2D;
        }

        /// <summary>
        /// 通过相机截取屏幕并转换为Sprite
        /// </summary>
        /// <param name="self">目标相机</param>
        /// <returns>相机抓取的屏幕Texture2D</returns>
        public static UnitySprite ScreenshotAsSpriteRGBA(this UnityCamera self)
        {
            UnityTexture2D texture2D = ScreenshotAsTextureRGBA(self);
            return UnitySprite.Create(texture2D, new UnityRect(0f, 0f, texture2D.width, texture2D.height), UnityVector2.zero);
        }
        public static UnitySprite ScreenshotAsSpriteRGB(this UnityCamera self)
        {
            UnityTexture2D texture2D = ScreenshotAsTextureRGB(self);
            return UnitySprite.Create(texture2D, new UnityRect(0f, 0f, texture2D.width, texture2D.height), UnityVector2.zero);
        }
        public static UnitySprite ScreenshotAsSprite(this UnityCamera self, UnityTextureFormat textureFormat)
        {
            UnityTexture2D texture2D = ScreenshotAsTexture(self, textureFormat);
            return UnitySprite.Create(texture2D, new UnityRect(0f, 0f, texture2D.width, texture2D.height), UnityVector2.zero);
        }
    }
}
