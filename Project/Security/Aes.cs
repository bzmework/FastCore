using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FastCore.Security
{
    /// <summary>
    /// Aes军工级加密/解密
    /// </summary>
    public static class Aes
    {
        private const string KEY = "MHLVRjoG8uGj+ay/de+ifUf+NNCF5C1TkbqRq50Cico="; // 固定密钥: _elong.tech@2020_

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <param name="key">用GenerateKey生成的密钥或自定义密钥，如果不指定密钥将使用内置的固定密钥. Key的长度不应超过32个字节</param>
        /// <returns>密文密码</returns>
        public static string Encrypt(string password, string key = "")
        {
            string result = "";

            var encrypt = Encoding.UTF8.GetBytes(password);
            using (var rm = new RijndaelManaged())
            {
                key = string.IsNullOrEmpty(key) ? KEY : key;
                rm.KeySize = 256;
                rm.Key = KeyGen.GenerateValidKey(key, 16, 32);
                rm.Mode = CipherMode.ECB;
                rm.Padding = PaddingMode.PKCS7;

                using (var encryptor = rm.CreateEncryptor())
                {
                    var blocks = encryptor.TransformFinalBlock(encrypt, 0, encrypt.Length);
                    result = Convert.ToBase64String(blocks, 0, blocks.Length);
                }
            }

            // 返回密文密码
            return result;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">用GenerateKey生成的密钥或自定义密钥，如果不指定密钥将使用内置的固定密钥. Key的长度不应超过32个字节</param>
        /// <returns>密文数据</returns>
        public static byte[] Encrypt(byte[] data, string key = "")
        {
            using (var rm = new RijndaelManaged())
            {
                key = string.IsNullOrEmpty(key) ? KEY : key;
                rm.KeySize = 256;
                rm.BlockSize = 128;
                rm.Key = KeyGen.GenerateValidKey(key, 16, 32);
                rm.Mode = CipherMode.ECB;
                rm.Padding = PaddingMode.PKCS7;

                using (var encryptor = rm.CreateEncryptor())
                {
                    return encryptor.TransformFinalBlock(data, 0, data.Length);
                }
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="password">密文密码</param>
        /// <param name="key">用GenerateKey生成的密钥或自定义密钥，如果不指定密钥将使用内置的固定密钥. Key的长度不应超过32个字节</param>
        /// <returns>明文密码</returns>
        public static string Decrypt(string password, string key = "")
        {
            string result = "";

            var decrypt = Convert.FromBase64String(password);
            using (var rm = new RijndaelManaged())
            {
                key = string.IsNullOrEmpty(key) ? KEY : key;
                rm.KeySize = 256;
                rm.BlockSize = 128;
                rm.Key = KeyGen.GenerateValidKey(key, 16, 32);
                rm.Mode = CipherMode.ECB;
                rm.Padding = PaddingMode.PKCS7;

                using (var decryptor = rm.CreateDecryptor())
                {
                    var blocks = decryptor.TransformFinalBlock(decrypt, 0, decrypt.Length);
                    result = Encoding.UTF8.GetString(blocks, 0, blocks.Length);
                }
            }

            // 返回明文密码
            return result;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">用GenerateKey生成的密钥或自定义密钥，如果不指定密钥将使用内置的固定密钥. Key的长度不应超过32个字节</param>
        /// <returns>明文数据</returns>
        public static byte[] Decrypt(byte[] data, string key = "")
        {
            using (var rm = new RijndaelManaged())
            {
                key = string.IsNullOrEmpty(key) ? KEY : key;
                rm.KeySize = 256;
                rm.BlockSize = 128;
                rm.Key = KeyGen.GenerateValidKey(key, 16, 32);
                rm.Mode = CipherMode.ECB;
                rm.Padding = PaddingMode.PKCS7;

                using (var decryptor = rm.CreateDecryptor())
                {
                    return decryptor.TransformFinalBlock(data, 0, data.Length);
                }
            }
        }
    }
}
