/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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

using SystemStream = System.IO.Stream;
using SystemBinaryWriter = System.IO.BinaryWriter;
using SystemBinaryReader = System.IO.BinaryReader;
using SystemEncoding = System.Text.Encoding;

namespace NovaEngine
{
    /// <summary>
    /// 对象序列化基类
    /// </summary>
    /// <typeparam name="T">序列化的数据类型</typeparam>
    public abstract class Serializer<T>
    {
        /// <summary>
        /// 序列化回调函数
        /// </summary>
        /// <param name="binaryWriter">目标流</param>
        /// <param name="data">要序列化的数据</param>
        /// <returns>若序列化数据成功则返回true，否则返回false</returns>
        public delegate bool SerializeCallback(SystemBinaryWriter binaryWriter, T data);

        /// <summary>
        /// 反序列化回调函数
        /// </summary>
        /// <param name="binaryReader">指定流</param>
        /// <returns>反序列化的数据</returns>
        public delegate T DeserializeCallback(SystemBinaryReader binaryReader);

        /// <summary>
        /// 尝试从指定流获取指定键的值回调函数
        /// </summary>
        /// <param name="binaryReader">指定流</param>
        /// <param name="key">指定键</param>
        /// <param name="value">指定键的值</param>
        /// <returns>若从指定流获取指定键的值成功则返回true，否则返回false</returns>
        public delegate bool TryGetValueCallback(SystemBinaryReader binaryReader, string key, out object value);

        private readonly Dictionary<byte, SerializeCallback> m_serializeCallbacks;
        private readonly Dictionary<byte, DeserializeCallback> m_deserializeCallbacks;
        private readonly Dictionary<byte, TryGetValueCallback> m_tryGetValueCallbacks;
        private byte m_latestSerializeCallbackVersion;

        /// <summary>
        /// 初始化游戏框架序列化器基类的新实例
        /// </summary>
        public Serializer()
        {
            m_serializeCallbacks = new Dictionary<byte, SerializeCallback>();
            m_deserializeCallbacks = new Dictionary<byte, DeserializeCallback>();
            m_tryGetValueCallbacks = new Dictionary<byte, TryGetValueCallback>();
            m_latestSerializeCallbackVersion = 0;
        }

        /// <summary>
        /// 注册序列化回调函数
        /// </summary>
        /// <param name="version">序列化回调函数的版本</param>
        /// <param name="callback">序列化回调函数</param>
        public void RegisterSerializeCallback(byte version, SerializeCallback callback)
        {
            if (null == callback)
            {
                throw new CFrameworkException("Serialize callback is invalid.");
            }

            m_serializeCallbacks[version] = callback;
            if (version > m_latestSerializeCallbackVersion)
            {
                m_latestSerializeCallbackVersion = version;
            }
        }

        /// <summary>
        /// 注册反序列化回调函数
        /// </summary>
        /// <param name="version">反序列化回调函数的版本</param>
        /// <param name="callback">反序列化回调函数</param>
        public void RegisterDeserializeCallback(byte version, DeserializeCallback callback)
        {
            if (null == callback)
            {
                throw new CFrameworkException("Deserialize callback is invalid.");
            }

            m_deserializeCallbacks[version] = callback;
        }

        /// <summary>
        /// 注册尝试从指定流获取指定键的值回调函数
        /// </summary>
        /// <param name="version">尝试从指定流获取指定键的值回调函数的版本</param>
        /// <param name="callback">尝试从指定流获取指定键的值回调函数</param>
        public void RegisterTryGetValueCallback(byte version, TryGetValueCallback callback)
        {
            if (null == callback)
            {
                throw new CFrameworkException("Try get value callback is invalid.");
            }

            m_tryGetValueCallbacks[version] = callback;
        }

        /// <summary>
        /// 序列化数据到目标流中
        /// </summary>
        /// <param name="stream">目标流</param>
        /// <param name="data">要序列化的数据</param>
        /// <returns>是否序列化数据成功</returns>
        public bool Serialize(SystemStream stream, T data)
        {
            if (m_serializeCallbacks.Count <= 0)
            {
                throw new CFrameworkException("No serialize callback registered.");
            }

            return Serialize(stream, data, m_latestSerializeCallbackVersion);
        }

        /// <summary>
        /// 序列化数据到目标流中
        /// </summary>
        /// <param name="stream">目标流</param>
        /// <param name="data">要序列化的数据</param>
        /// <param name="version">序列化回调函数的版本</param>
        /// <returns>是否序列化数据成功</returns>
        public bool Serialize(SystemStream stream, T data, byte version)
        {
            using (SystemBinaryWriter binaryWriter = new SystemBinaryWriter(stream, SystemEncoding.UTF8))
            {
                byte[] header = GetHeader();
                binaryWriter.Write(header[0]);
                binaryWriter.Write(header[1]);
                binaryWriter.Write(header[2]);
                binaryWriter.Write(version);
                SerializeCallback callback = null;
                if (!m_serializeCallbacks.TryGetValue(version, out callback))
                {
                    throw new CFrameworkException("Serialize callback '{%d}' is not exist.", version);
                }

                return callback(binaryWriter, data);
            }
        }

        /// <summary>
        /// 从指定流反序列化数据
        /// </summary>
        /// <param name="stream">指定流</param>
        /// <returns>反序列化的数据</returns>
        public T Deserialize(SystemStream stream)
        {
            using (SystemBinaryReader binaryReader = new SystemBinaryReader(stream, SystemEncoding.UTF8))
            {
                byte[] header = GetHeader();
                byte header0 = binaryReader.ReadByte();
                byte header1 = binaryReader.ReadByte();
                byte header2 = binaryReader.ReadByte();
                if (header0 != header[0] || header1 != header[1] || header2 != header[2])
                {
                    throw new CFrameworkException("Header is invalid, need '{0}{1}{2}', current '{3}{4}{5}'.",
                        ((char) header[0]).ToString(), ((char) header[1]).ToString(), ((char) header[2]).ToString(),
                        ((char) header0).ToString(), ((char) header1).ToString(), ((char) header2).ToString());
                }

                byte version = binaryReader.ReadByte();
                DeserializeCallback callback = null;
                if (!m_deserializeCallbacks.TryGetValue(version, out callback))
                {
                    throw new CFrameworkException("Deserialize callback '{%d}' is not exist.", version);
                }

                return callback(binaryReader);
            }
        }

        /// <summary>
        /// 尝试从指定流获取指定键的值
        /// </summary>
        /// <param name="stream">指定流</param>
        /// <param name="key">指定键</param>
        /// <param name="value">指定键的值</param>
        /// <returns>是否从指定流获取指定键的值成功</returns>
        public bool TryGetValue(SystemStream stream, string key, out object value)
        {
            value = null;
            using (SystemBinaryReader binaryReader = new SystemBinaryReader(stream, SystemEncoding.UTF8))
            {
                byte[] header = GetHeader();
                byte header0 = binaryReader.ReadByte();
                byte header1 = binaryReader.ReadByte();
                byte header2 = binaryReader.ReadByte();
                if (header0 != header[0] || header1 != header[1] || header2 != header[2])
                {
                    return false;
                }

                byte version = binaryReader.ReadByte();
                TryGetValueCallback callback = null;
                if (!m_tryGetValueCallbacks.TryGetValue(version, out callback))
                {
                    return false;
                }

                return callback(binaryReader, key, out value);
            }
        }

        /// <summary>
        /// 获取数据头标识
        /// </summary>
        /// <returns>数据头标识</returns>
        protected abstract byte[] GetHeader();
    }
}
