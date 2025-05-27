/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyring (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemArray = System.Array;
using SystemConvert = System.Convert;
using SystemEncoding = System.Text.Encoding;
using SystemMemoryStream = System.IO.MemoryStream;
using SystemAes = System.Security.Cryptography.Aes;
using SystemCryptoStream = System.Security.Cryptography.CryptoStream;
using SystemCryptoStreamMode = System.Security.Cryptography.CryptoStreamMode;

namespace NovaEngine
{
    /// <summary>
    /// 实用函数集合工具类
    /// </summary>
    public static partial class Utility
    {
        /// <summary>
        /// 加密解密相关实用函数集合
        /// </summary>
        public static class Encryption
        {
            #region 异或加密方式提供的功能函数接口

            /// <summary>
            /// 快速异或加密允许的缓冲区长度
            /// </summary>
            private const int QUICK_XOR_ENCRYPT_BUFFER_SIZE = 128;

            /// <summary>
            /// 将bytes使用code做异或运算的快速版本
            /// </summary>
            /// <param name="bytes">原始二进制流</param>
            /// <param name="code">异或二进制流</param>
            /// <returns>异或后的二进制流</returns>
            public static byte[] GetQuickXorBytes(byte[] bytes, byte[] code)
            {
                return GetXorBytes(bytes, 0, QUICK_XOR_ENCRYPT_BUFFER_SIZE, code);
            }

            /// <summary>
            /// 将bytes使用code做异或运算的快速版本，此方法将复用并改写传入的bytes作为返回值，而不额外分配内存空间
            /// </summary>
            /// <param name="bytes">原始及异或后的二进制流</param>
            /// <param name="code">异或二进制流</param>
            public static void GetQuickXorBytesOnSelf(byte[] bytes, byte[] code)
            {
                GetXorBytesOnSelf(bytes, 0, QUICK_XOR_ENCRYPT_BUFFER_SIZE, code);
            }

            /// <summary>
            /// 将bytes使用code做异或运算
            /// </summary>
            /// <param name="bytes">原始二进制流</param>
            /// <param name="code">异或二进制流</param>
            /// <returns>异或后的二进制流</returns>
            public static byte[] GetXorBytes(byte[] bytes, byte[] code)
            {
                if (null == bytes)
                {
                    return null;
                }

                return GetXorBytes(bytes, 0, bytes.Length, code);
            }

            /// <summary>
            /// 将bytes使用code做异或运算，此方法将复用并改写传入的bytes作为返回值，而不额外分配内存空间
            /// </summary>
            /// <param name="bytes">原始及异或后的二进制流</param>
            /// <param name="code">异或二进制流</param>
            public static void GetXorBytesOnSelf(byte[] bytes, byte[] code)
            {
                if (null == bytes)
                {
                    return;
                }

                GetXorBytesOnSelf(bytes, 0, bytes.Length, code);
            }

            /// <summary>
            /// 将bytes使用code做异或运算
            /// </summary>
            /// <param name="bytes">原始二进制流</param>
            /// <param name="index">异或计算的开始位置</param>
            /// <param name="length">异或计算长度，若小于0，则计算整个二进制流</param>
            /// <param name="code">异或二进制流</param>
            /// <returns>异或后的二进制流</returns>
            public static byte[] GetXorBytes(byte[] bytes, int index, int length, byte[] code)
            {
                if (null == bytes)
                {
                    return null;
                }

                int bytesLength = bytes.Length;
                byte[] results = new byte[bytesLength];
                System.Array.Copy(bytes, 0, results, 0, bytesLength);
                GetXorBytesOnSelf(results, index, length, code);
                return results;
            }

            /// <summary>
            /// 将bytes使用code做异或运算，此方法将复用并改写传入的bytes作为返回值，而不额外分配内存空间
            /// </summary>
            /// <param name="bytes">原始及异或后的二进制流</param>
            /// <param name="index">异或计算的开始位置</param>
            /// <param name="length">异或计算长度</param>
            /// <param name="code">异或二进制流</param>
            public static void GetXorBytesOnSelf(byte[] bytes, int index, int length, byte[] code)
            {
                if (null == bytes)
                {
                    return;
                }

                if (null == code)
                {
                    throw new CFrameworkException("Code is invalid.");
                }

                int codeLength = code.Length;
                if (codeLength <= 0)
                {
                    throw new CFrameworkException("Code length is invalid.");
                }

                if (index < 0 || length < 0 || index + length > bytes.Length)
                {
                    throw new CFrameworkException("Start index or length is invalid.");
                }

                int codeIndex = index % codeLength;
                for (int n = index; n < length; n++)
                {
                    bytes[n] ^= code[codeIndex++];
                    codeIndex %= codeLength;
                }
            }

            #endregion

            #region AES加密方式提供的功能函数接口

            /// <summary>
            /// AES加密字符串密钥长度
            /// </summary>
            private const int AES_CRYPTO_KEY_LENGTH = 32;
            /// <summary>
            /// AES加密字符串初始化向量长度
            /// </summary>
            private const int AES_CRYPTO_VEC_LENGTH = 16;

            /// <summary>
            /// 获取指定字符串通过AES加密后的字符串数据
            /// </summary>
            /// <param name="data">原始字符串</param>
            /// <param name="key">密钥字符串</param>
            /// <param name="vector">初始化向量字符串</param>
            /// <returns>返回加密后的字符串信息，若加密失败则返回null</returns>
            public static string GetAesEncryptString(string data, string key, string vector)
            {
                if (string.IsNullOrEmpty(data))
                {
                    Logger.Warn("Invalid arguments to perform AES encryption.");
                    return null;
                }

                byte[] plainBytes = Convertion.GetBytes(data);
                byte[] encryptBytes = GetAesEncryptBytes(plainBytes, key, vector);
                if (null == encryptBytes)
                    return null;

                return SystemConvert.ToBase64String(encryptBytes);
            }

            /// <summary>
            /// 获取指定字节流通过AES加密后的字节流数据
            /// </summary>
            /// <param name="data">原始字节流</param>
            /// <param name="key">密钥字符串</param>
            /// <param name="vector">初始化向量字符串</param>
            /// <returns>返回加密后的字节流信息，若加密失败则返回null</returns>
            public static byte[] GetAesEncryptBytes(byte[] data, string key, string vector)
            {
                if (null == data || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(vector))
                {
                    Logger.Warn("Invalid arguments to perform AES encryption.");
                    return null;
                }

                if (AES_CRYPTO_KEY_LENGTH != key.Length)
                {
                    Logger.Warn("The key param must have a length of {0} chars.", AES_CRYPTO_KEY_LENGTH);
                    return null;
                }

                if (AES_CRYPTO_VEC_LENGTH != vector.Length)
                {
                    Logger.Warn("The vector param must have a length of {0} chars.", AES_CRYPTO_VEC_LENGTH);
                    return null;
                }

                byte[] plainBytes = data;

                byte[] keyBytes = new byte[AES_CRYPTO_KEY_LENGTH];
                SystemArray.Copy(Convertion.GetBytes(key.PadRight(keyBytes.Length)), keyBytes, keyBytes.Length);

                byte[] vectorBytes = new byte[AES_CRYPTO_VEC_LENGTH];
                SystemArray.Copy(Convertion.GetBytes(vector.PadRight(vectorBytes.Length)), vectorBytes, vectorBytes.Length);

                byte[] encryptData = null; // encrypted data
                using SystemAes aes = SystemAes.Create();
                try
                {
                    using SystemMemoryStream memoryStream = new SystemMemoryStream();
                    using SystemCryptoStream cryptoStream = new SystemCryptoStream(memoryStream, aes.CreateEncryptor(keyBytes, vectorBytes), SystemCryptoStreamMode.Write);
                    cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    encryptData = memoryStream.ToArray();
                }
                catch
                {
                    encryptData = null;
                }

                return encryptData;
            }

            /// <summary>
            /// 获取指定字符串通过AES解密后的字符串数据
            /// </summary>
            /// <param name="data">原始字符串</param>
            /// <param name="key">密钥字符串</param>
            /// <param name="vector">初始化向量字符串</param>
            /// <returns>返回解密后的字符串信息，若解密失败则返回null</returns>
            public static string GetAesDecryptString(string data, string key, string vector)
            {
                if (string.IsNullOrEmpty(data))
                {
                    Logger.Warn("Invalid arguments to perform AES decryption.");
                    return null;
                }

                byte[] encryptBytes = SystemConvert.FromBase64String(data);
                byte[] decryptBytes = GetAesDecryptBytes(encryptBytes, key, vector);
                if (null == decryptBytes)
                    return null;

                return Convertion.GetString(decryptBytes);
            }

            /// <summary>
            /// 获取指定字节流通过AES解密后的字节流数据
            /// </summary>
            /// <param name="data">原始字节流</param>
            /// <param name="key">密钥字符串</param>
            /// <param name="vector">初始化向量字符串</param>
            /// <returns>返回解密后的字节流信息，若解密失败则返回null</returns>
            public static byte[] GetAesDecryptBytes(byte[] data, string key, string vector)
            {
                if (null == data || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(vector))
                {
                    Logger.Warn("Invalid arguments to perform AES decryption.");
                    return null;
                }

                if (AES_CRYPTO_KEY_LENGTH != key.Length)
                {
                    Logger.Warn("The key param must have a length of {0} chars.", AES_CRYPTO_KEY_LENGTH);
                    return null;
                }

                if (AES_CRYPTO_VEC_LENGTH != vector.Length)
                {
                    Logger.Warn("The vector param must have a length of {0} chars.", AES_CRYPTO_VEC_LENGTH);
                    return null;
                }

                byte[] encryptBytes = data;

                byte[] keyBytes = new byte[AES_CRYPTO_KEY_LENGTH];
                SystemArray.Copy(Convertion.GetBytes(key.PadRight(keyBytes.Length)), keyBytes, keyBytes.Length);

                byte[] vectorBytes = new byte[AES_CRYPTO_VEC_LENGTH];
                SystemArray.Copy(Convertion.GetBytes(vector.PadRight(vectorBytes.Length)), vectorBytes, vectorBytes.Length);

                byte[] decryptData = null; // decrypted data
                using SystemAes aes = SystemAes.Create();
                try
                {
                    using SystemMemoryStream memoryStream = new SystemMemoryStream(encryptBytes);
                    using SystemCryptoStream cryptoStream = new SystemCryptoStream(memoryStream, aes.CreateDecryptor(keyBytes, vectorBytes), SystemCryptoStreamMode.Read);
                    using SystemMemoryStream tempStream = new SystemMemoryStream();

                    byte[] buffer = new byte[1024];
                    int readBytes = 0;
                    while ((readBytes = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        tempStream.Write(buffer, 0, readBytes);
                    }

                    decryptData = tempStream.ToArray();
                }
                catch
                {
                    decryptData = null;
                }

                return decryptData;
            }

            #endregion
        }
    }
}
