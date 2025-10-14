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

using UnityGameObject = UnityEngine.GameObject;
using UnitySprite = UnityEngine.Sprite;
using UnityTexture2D = UnityEngine.Texture2D;

namespace NovaEngine
{
    /// <summary>
    /// 基于Unity库精灵类的扩展接口支持类
    /// </summary>
    public static class __Sprite
    {
        /// <summary>
        /// 将 <see cref="UnityEngine.Sprite"/> 转换为 <see cref="UnityEngine.Texture2D"/>
        /// </summary>
        /// <param name="self">对象实例</param>
        /// <returns>返回转换后的纹理对象</returns>
        public static UnityTexture2D ConvertToSprite(this UnitySprite self)
        {
            var tex = new UnityTexture2D((int) self.rect.width, (int) self.rect.height);
            var pixels = self.texture.GetPixels(
                (int) self.textureRect.x,
                (int) self.textureRect.y,
                (int) self.textureRect.width,
                (int) self.textureRect.height);
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }
    }
}
