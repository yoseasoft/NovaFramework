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

using UnityGameObject = UnityEngine.GameObject;
using UnitySprite = UnityEngine.Sprite;
using UnityTexture = UnityEngine.Texture;
using UnityTexture2D = UnityEngine.Texture2D;
using UnityMathf = UnityEngine.Mathf;
using UnityRect = UnityEngine.Rect;
using UnityVector2 = UnityEngine.Vector2;
using UnityColor = UnityEngine.Color;
using UnityColor32 = UnityEngine.Color32;
using UnityTextureFormat = UnityEngine.TextureFormat;

namespace NovaEngine
{
    /// <summary>
    /// 基于Unity库纹理类的扩展接口支持类
    /// </summary>
    public static class __Texture
    {
        public static UnityTexture2D Clone(this UnityTexture2D self)
        {
            UnityTexture2D tex = new UnityTexture2D(self.width, self.height);
            UnityColor[] colors = self.GetPixels(0, 0, self.width, self.height);
            tex.SetPixels(colors);
            tex.Apply();
            return tex;
        }

        /// <summary>
        /// 双线性插值法缩放图片，等比缩放
        /// </summary>
        /// <param name="self">对象实例</param>
        /// <param name="scaleFactor">缩放比例</param>
        /// <returns>返回缩放后的纹理对象</returns>
        public static UnityTexture2D ScaleTextureBilinear(this UnityTexture2D self, float scaleFactor)
        {
            UnityTexture2D newTexture = new UnityTexture2D(UnityMathf.CeilToInt(self.width * scaleFactor), UnityMathf.CeilToInt(self.height * scaleFactor));
            float scale = 1.0f / scaleFactor;
            int maxX = self.width - 1;
            int maxY = self.height - 1;
            for (int y = 0; y < newTexture.height; ++y)
            {
                for (int x = 0; x < newTexture.width; ++x)
                {
                    float targetX = x * scale;
                    float targetY = y * scale;
                    int x1 = UnityMathf.Min(maxX, UnityMathf.FloorToInt(targetX));
                    int y1 = UnityMathf.Min(maxY, UnityMathf.FloorToInt(targetY));
                    int x2 = UnityMathf.Min(maxX, x1 + 1);
                    int y2 = UnityMathf.Min(maxY, y1 + 1);

                    float u = targetX - x1;
                    float v = targetY - y1;
                    float w1 = (1 - u) * (1 - v);
                    float w2 = u * (1 - v);
                    float w3 = (1 - u) * v;
                    float w4 = u * v;
                    UnityColor color1 = self.GetPixel(x1, y1);
                    UnityColor color2 = self.GetPixel(x2, y1);
                    UnityColor color3 = self.GetPixel(x1, y2);
                    UnityColor color4 = self.GetPixel(x2, y2);
                    UnityColor color = new UnityColor(
                        UnityMathf.Clamp01(color1.r * w1 + color2.r * w2 + color3.r * w3 + color4.r * w4),
                        UnityMathf.Clamp01(color1.g * w1 + color2.g * w2 + color3.g * w3 + color4.g * w4),
                        UnityMathf.Clamp01(color1.b * w1 + color2.b * w2 + color3.b * w3 + color4.b * w4),
                        UnityMathf.Clamp01(color1.a * w1 + color2.a * w2 + color3.a * w3 + color4.a * w4)
                    );
                    newTexture.SetPixel(x, y, color);

                }
            }
            newTexture.Apply();
            return newTexture;
        }

        /// <summary>
        /// 双线性插值法缩放图片为指定尺寸
        /// </summary>
        /// <param name="self">对象实例</param>
        /// <param name="size">缩放尺寸</param>
        /// <returns>返回缩放后的纹理对象</returns>
        public static UnityTexture2D SizeTextureBilinear(this UnityTexture2D self, UnityVector2 size)
        {
            UnityTexture2D newTexture = new UnityTexture2D(UnityMathf.CeilToInt(size.x), UnityMathf.CeilToInt(size.y));
            float scaleX = self.width / size.x;
            float scaleY = self.height / size.y;
            int maxX = self.width - 1;
            int maxY = self.height - 1;
            for (int y = 0; y < newTexture.height; y++)
            {
                for (int x = 0; x < newTexture.width; x++)
                {
                    float targetX = x * scaleX;
                    float targetY = y * scaleY;
                    int x1 = UnityMathf.Min(maxX, UnityMathf.FloorToInt(targetX));
                    int y1 = UnityMathf.Min(maxY, UnityMathf.FloorToInt(targetY));
                    int x2 = UnityMathf.Min(maxX, x1 + 1);
                    int y2 = UnityMathf.Min(maxY, y1 + 1);

                    float u = targetX - x1;
                    float v = targetY - y1;
                    float w1 = (1 - u) * (1 - v);
                    float w2 = u * (1 - v);
                    float w3 = (1 - u) * v;
                    float w4 = u * v;
                    UnityColor color1 = self.GetPixel(x1, y1);
                    UnityColor color2 = self.GetPixel(x2, y1);
                    UnityColor color3 = self.GetPixel(x1, y2);
                    UnityColor color4 = self.GetPixel(x2, y2);
                    UnityColor color = new UnityColor(
                        UnityMathf.Clamp01(color1.r * w1 + color2.r * w2 + color3.r * w3 + color4.r * w4),
                        UnityMathf.Clamp01(color1.g * w1 + color2.g * w2 + color3.g * w3 + color4.g * w4),
                        UnityMathf.Clamp01(color1.b * w1 + color2.b * w2 + color3.b * w3 + color4.b * w4),
                        UnityMathf.Clamp01(color1.a * w1 + color2.a * w2 + color3.a * w3 + color4.a * w4)
                    );
                    newTexture.SetPixel(x, y, color);

                }
            }
            newTexture.Apply();
            return newTexture;
        }

        /// <summary>
        /// 纹理对象按指定欧拉角进行旋转
        /// </summary>
        /// <param name="self">对象实例</param>
        /// <param name="eulerAngles">旋转角度</param>
        /// <returns>返回旋转后的纹理对象</returns>
        public static UnityTexture2D RotateTexture(this UnityTexture2D self, float eulerAngles)
        {
            int x;
            int y;
            int i;
            int j;
            float phi = eulerAngles / (180 / UnityMathf.PI);
            float sn = UnityMathf.Sin(phi);
            float cs = UnityMathf.Cos(phi);
            UnityColor32[] arr = self.GetPixels32();
            UnityColor32[] arr2 = new UnityColor32[arr.Length];
            int W = self.width;
            int H = self.height;
            int xc = W / 2;
            int yc = H / 2;

            for (j = 0; j < H; j++)
            {
                for (i = 0; i < W; i++)
                {
                    arr2[j * W + i] = new UnityColor32(0, 0, 0, 0);

                    x = (int) (cs * (i - xc) + sn * (j - yc) + xc);
                    y = (int) (-sn * (i - xc) + cs * (j - yc) + yc);

                    if ((x > -1) && (x < W) && (y > -1) && (y < H))
                    {
                        arr2[j * W + i] = arr[y * W + x];
                    }
                }
            }

            UnityTexture2D newImg = new UnityTexture2D(W, H);
            newImg.SetPixels32(arr2);
            newImg.Apply();

            return newImg;
        }

        /// <summary>
        /// 将 <see cref="UnityEngine.Texture2D"/> 转换为 <see cref="UnityEngine.Sprite"/>
        /// </summary>
        /// <param name="self">对象实例</param>
        /// <returns>返回转换后的精灵对象</returns>
        public static UnitySprite ToSprite(this UnityTexture2D self)
        {
            UnitySprite sprite = UnitySprite.Create(self, new UnityRect(0, 0, self.width, self.height), UnityVector2.zero);
            return sprite;
        }

        /// <summary>
        /// 将 <see cref="UnityEngine.Texture"/> 转换为 <see cref="UnityEngine.Texture2D"/>
        /// </summary>
        /// <param name="self">对象实例</param>
        /// <returns>返回转换后的纹理对象</returns>
        public static UnityTexture2D ToTexture2D(this UnityTexture self)
        {
            return UnityTexture2D.CreateExternalTexture(
                self.width,
                self.height,
                UnityTextureFormat.RGB24,
                false, false,
                self.GetNativeTexturePtr());
        }
    }
}
