using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FastCore.HashAlg
{
    /// <summary>
    /// Hash算法
    /// </summary>
    public static class Hash
    {
        /// <summary>
        /// Md5 哈希算法
        /// </summary>
        /// <param name="data">数据。要进行哈希的数据</param>
        /// <returns>哈希代码</returns>
        public static byte[] Md5(byte[] data)
        {
            using (var md = MD5.Create())
            {
                return md.ComputeHash(data);
            }
        }

        /// <summary>
        /// Md5 哈希算法
        /// </summary>
        /// <param name="data">数据。要进行哈希的数据</param>
        /// <returns>哈希代码</returns>
        public static string Md5(string data)
        {
            var value = Encoding.UTF8.GetBytes(data);
            using (var md = MD5.Create())
            {
                return Convert.ToBase64String(md.ComputeHash(value));
            }
        }

        /// <summary>
        /// Sha1 哈希算法
        /// </summary>
        /// <param name="data">数据。要进行哈希的数据</param>
        /// <returns>哈希代码</returns>
        public static byte[] Sha1(byte[] data)
        {
            using (var sha = SHA1.Create())
            {
                return sha.ComputeHash(data);
            }
        }

        /// <summary>
        /// Sha1 哈希算法
        /// </summary>
        /// <param name="data">数据。要进行哈希的数据</param>
        /// <returns>哈希代码</returns>
        public static string Sha1(string data)
        {
            var value = Encoding.UTF8.GetBytes(data);
            using (var sha = SHA1.Create())
            {
                return Convert.ToBase64String(sha.ComputeHash(value));
            }
        }

        /// <summary>
        /// Sha256 哈希算法
        /// </summary>
        /// <param name="data">数据。要进行哈希的数据</param>
        /// <returns>哈希代码</returns>
        public static byte[] Sha256(byte[] data)
        {
            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(data);
            }
        }

        /// <summary>
        /// Sha256 哈希算法
        /// </summary>
        /// <param name="data">数据。要进行哈希的数据</param>
        /// <returns>哈希代码</returns>
        public static string Sha256(string data)
        {
            var value = Encoding.UTF8.GetBytes(data);
            using (var sha = SHA256.Create())
            {
                return Convert.ToBase64String(sha.ComputeHash(value));
            }
        }

        /// <summary>
        /// Sha384 哈希算法
        /// </summary>
        /// <param name="data">数据。要进行哈希的数据</param>
        /// <returns>哈希代码</returns>
        public static byte[] Sha384(byte[] data)
        {
            using (var sha = SHA384.Create())
            {
                return sha.ComputeHash(data);
            }
        }

        /// <summary>
        /// Sha384 哈希算法
        /// </summary>
        /// <param name="data">数据。要进行哈希的数据</param>
        /// <returns>哈希代码</returns>
        public static string Sha384(string data)
        {
            var value = Encoding.UTF8.GetBytes(data);
            using (var sha = SHA384.Create())
            {
                return Convert.ToBase64String(sha.ComputeHash(value));
            }
        }

        /// <summary>
        /// Sha512 哈希算法
        /// </summary>
        /// <param name="data">数据。要进行哈希的数据</param>
        /// <returns>哈希代码</returns>
        public static byte[] Sha512(byte[] data)
        {
            using (var sha = SHA512.Create())
            {
                return sha.ComputeHash(data);
            }
        }

        /// <summary>
        /// Sha512 哈希算法
        /// </summary>
        /// <param name="data">数据。要进行哈希的数据</param>
        /// <returns>哈希代码</returns>
        public static string Sha512(string data)
        {
            var value = Encoding.UTF8.GetBytes(data);
            using (var sha = SHA512.Create())
            {
                return Convert.ToBase64String(sha.ComputeHash(value));
            }
        }

        /// <summary>
        /// Hmac Sha1 哈希算法
        /// </summary>
        /// <param name="key">密钥。该密钥可以是任意长度。但是建议的大小为64个字节。如果键的长度超过64个字节，将对其进行哈希运算（使用 SHA-256）以派生出一个64字节的密钥。如果少于64个字节，就填充到64个字节。</param>
        /// <param name="data">数据。要进行哈希的数据</param>
        /// <returns>哈希代码</returns>
        public static byte[] HSha1(byte[] key, byte[] data)
        {
            using (var sha = new HMACSHA1(key))
            {
                return sha.ComputeHash(data);
            }
        }

        /// <summary>
        /// Hmac Sha1 哈希算法
        /// </summary>
        /// <param name="key">密钥。该密钥可以是任意长度。但是建议的大小为64个字节。如果键的长度超过64个字节，将对其进行哈希运算（使用 SHA-256）以派生出一个64字节的密钥。如果少于64个字节，就填充到64个字节。</param>
        /// <param name="data">数据。要进行哈希的数据</param>
        /// <returns>哈希代码</returns>
        public static string HSha1(string key, string data)
        {
            var keys = Encoding.UTF8.GetBytes(key);
            var values = Encoding.UTF8.GetBytes(data);
            using (var sha = new HMACSHA1(keys))
            {
                return Convert.ToBase64String(sha.ComputeHash(values));
            }
        }

        /// <summary>
        /// Hmac Sha256 哈希算法
        /// </summary>
        /// <param name="key">密钥。该密钥可以是任意长度。但是建议的大小为64个字节。如果键的长度超过64个字节，将对其进行哈希运算（使用 SHA-256）以派生出一个64字节的密钥。如果少于64个字节，就填充到64个字节。</param>
        /// <param name="data">数据。要进行哈希的数据</param>
        /// <returns>哈希代码</returns>
        public static byte[] HSha256(byte[] key, byte[] data)
        {
            using (var sha = new HMACSHA256(key))
            {
                return sha.ComputeHash(data);
            }
        }

        /// <summary>
        /// Hmac Sha256 哈希算法
        /// </summary>
        /// <param name="key">密钥。该密钥可以是任意长度。但是建议的大小为64个字节。如果键的长度超过64个字节，将对其进行哈希运算（使用 SHA-256）以派生出一个64字节的密钥。如果少于64个字节，就填充到64个字节。</param>
        /// <param name="data">数据。要进行哈希的数据</param>
        /// <returns>哈希代码</returns>
        public static string HSha256(string key, string data)
        {
            var keys = Encoding.UTF8.GetBytes(key);
            var values = Encoding.UTF8.GetBytes(data);
            using (var sha = new HMACSHA256(keys))
            {
                return Convert.ToBase64String(sha.ComputeHash(values));
            }
        }

        /// <summary>
        /// Hmac Sha384 哈希算法
        /// </summary>
        /// <param name="key">密钥。该密钥可以是任意长度。但是建议的大小为128个字节。如果键的长度超过128个字节，将对其进行哈希运算（使用 SHA-384）以派生出一个128字节的密钥。如果少于128个字节，就填充到128个字节。</param>
        /// <param name="data">数据。要进行哈希的数据</param>
        /// <returns>哈希代码</returns>
        public static byte[] HSha384(byte[] key, byte[] data)
        {
            using (var sha = new HMACSHA384(key))
            {
                return sha.ComputeHash(data);
            }
        }

        /// <summary>
        /// Hmac Sha384 哈希算法
        /// </summary>
        /// <param name="key">密钥。该密钥可以是任意长度。但是建议的大小为128个字节。如果键的长度超过128个字节，将对其进行哈希运算（使用 SHA-384）以派生出一个128字节的密钥。如果少于128个字节，就填充到128个字节。</param>
        /// <param name="data">数据。要进行哈希的数据</param>
        /// <returns>哈希代码</returns>
        public static string HSha384(string key, string data)
        {
            var keys = Encoding.UTF8.GetBytes(key);
            var values = Encoding.UTF8.GetBytes(data);
            using (var sha = new HMACSHA384(keys))
            {
                return Convert.ToBase64String(sha.ComputeHash(values));
            }
        }

        /// <summary>
        /// Hmac Sha512 哈希算法
        /// </summary>
        /// <param name="key">密钥。该密钥可以是任意长度。但是建议的大小为128个字节。如果键的长度超过128个字节，将对其进行哈希运算（使用 SHA-384）以派生出一个128字节的密钥。如果少于128个字节，就填充到128个字节。</param>
        /// <param name="data">数据。要进行哈希的数据</param>
        /// <returns>哈希代码</returns>
        public static byte[] HSha512(byte[] key, byte[] data)
        {
            using (var sha = new HMACSHA512(key))
            {
                return sha.ComputeHash(data);
            }
        }

        /// <summary>
        /// Hmac Sha512 哈希算法
        /// </summary>
        /// <param name="key">密钥。该密钥可以是任意长度。但是建议的大小为128个字节。如果键的长度超过128个字节，将对其进行哈希运算（使用 SHA-384）以派生出一个128字节的密钥。如果少于128个字节，就填充到128个字节。</param>
        /// <param name="data">数据。要进行哈希的数据</param>
        /// <returns>哈希代码</returns>
        public static string HSha512(string key, string data)
        {
            var keys = Encoding.UTF8.GetBytes(key);
            var values = Encoding.UTF8.GetBytes(data);
            using (var sha = new HMACSHA512(keys))
            {
                return Convert.ToBase64String(sha.ComputeHash(values));
            }
        }

        /// <summary>
        /// Hmac Md5 哈希算法
        /// </summary>
        /// <param name="key">密钥。该密钥可以是任意长度。但是建议的大小为64个字节。如果键的长度超过64个字节，将对其进行哈希运算（使用 SHA-1）以派生出一个64字节的密钥。如果少于64个字节，就填充到64个字节。</param>
        /// <param name="data">数据。要进行哈希的数据</param>
        /// <returns>哈希代码</returns>
        public static byte[] HMd5(byte[] key, byte[] data)
        {
            using (var md5 = new HMACMD5(key))
            {
                return md5.ComputeHash(data);
            }
        }

        /// <summary>
        /// Hmac Md5 哈希算法
        /// </summary>
        /// <param name="key">密钥。该密钥可以是任意长度。但是建议的大小为64个字节。如果键的长度超过64个字节，将对其进行哈希运算（使用 SHA-1）以派生出一个64字节的密钥。如果少于64个字节，就填充到64个字节。</param>
        /// <param name="data">数据。要进行哈希的数据</param>
        /// <returns>哈希代码</returns>
        public static string HMd5(string key, string data)
        {
            var keys = Encoding.UTF8.GetBytes(key);
            var values = Encoding.UTF8.GetBytes(data);
            using (var sha = new HMACMD5(keys))
            {
                return Convert.ToBase64String(sha.ComputeHash(values));
            }
        }

        /// <summary>
        /// MurmurHash3 哈希算法
        /// </summary>
        /// <param name="data">数据。要进行哈希的数据</param>
        /// <returns>哈希代码</returns>
        public static long Mur3(string data)
        {
            var hash = new MurmurHash3();
            return hash.ComputeHash(data);
        }

    }
}
