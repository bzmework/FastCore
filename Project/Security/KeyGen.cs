using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace FastCore.Security
{
    /// <summary>
    /// 安全密钥生成器(Security Key Generator)
    /// </summary>
    public static class KeyGen
    {
        /// <summary>
        /// 生成随机密钥
        /// </summary>
        /// <param name="length">Key长度, 16,24,32,64...</param>
        /// <returns>返回Base64编码的密钥，必须保存好不要丢失</returns>
        public static string GenerateRandomKey(int length = 32)
        {
            char[] chrs = new char[]
            {
               'a','b','d','c','e','f','g','h','i','j','k','l','m','n','p','r','q','s','t','u','v','w','z','y','x',
               '0','1','2','3','4','5','6','7','8','9',
               'A','B','C','D','E','F','G','H','I','J','K','L','M','N','Q','P','R','T','S','V','U','W','X','Y','Z'
            };

            StringBuilder num = new StringBuilder();
            Random rnd = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < length; i++)
            {
                num.Append(chrs[rnd.Next(0, chrs.Length)].ToString());
            }

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(num.ToString()));
        }

        /// <summary>
        /// 生成随机的Aes密钥
        /// </summary>
        /// <returns>返回Base64编码的密钥，必须保存好不要丢失</returns>
        public static string GenerateAesKey()
        {
            using (var key = new RijndaelManaged())
            {
                key.KeySize = 256; //密钥长度,以位(Bit)为单位，可通过LegalKeySizes获得支持的值
                key.BlockSize = 128; //块大小,以位(Bit)为单位，可通过LegalBlockSizes获得支持的值
                key.GenerateKey(); // 生成用于该算法的随机密钥 
                return Convert.ToBase64String(key.Key);
            }
        }

        /// <summary>
        /// 生成随机的Des密钥
        /// </summary>
        /// <returns>返回Base64编码的密钥，必须保存好不要丢失</returns>
        public static string GenerateDesKey()
        {
            using (var key = TripleDES.Create())
            {
                key.KeySize = 192; //密钥长度,以位(Bit)为单位，可通过LegalKeySizes获得支持的值
                key.BlockSize = 64; //块大小,以位(Bit)为单位，可通过LegalBlockSizes获得支持的值
                key.GenerateKey(); // 生成用于该算法的随机密钥 
                return Convert.ToBase64String(key.Key);
            }
        }

        /// <summary>
        /// 生成随机的Tea密钥
        /// </summary>
        /// <returns>返回Base64编码的密钥，必须保存好不要丢失</returns>
        public static string GenerateTeaKey()
        {
            DateTimeOffset now = DateTime.Now;
            //long time = now.ToUnixTimeMilliseconds();  // for above .NET Ver 3.6
            long time = (long)((now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
            long random = (long)(new Random().NextDouble() * 65536);
            long keyValue = time * random;
            string teaKey = string.Format("{0:D16}", keyValue);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(teaKey));
        }

        /// <summary>
        /// 生成有效的密钥
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="minLength">密钥最小长度(字节数)，默认16字节</param>
        /// <param name="maxLength">密钥最大长度(字节数)，默认32字节</param>
        /// <returns></returns>
        public static byte[] GenerateValidKey(string key, int minLength = 16, int maxLength = 32)
        {
            minLength = minLength < 16 ? 16 : minLength;
            maxLength = maxLength < minLength ? minLength : maxLength;

            byte[] keys;
            if(ObjectCheck.IsBase64(key))
            {
                keys = Convert.FromBase64String(key);
            }
            else
            {
                keys = Encoding.UTF8.GetBytes(key);
            }

            int len = minLength;
            if (keys.Length > maxLength) // 如果密钥超过maxLength长度则截取
            {
                byte[] temp = new byte[maxLength];
                Array.Copy(keys, 0, temp, 0, maxLength);
                keys = temp;
            }
            else if ((keys.Length % len) != 0) // 如果密钥不足minLength长度就补足
            {
                int groups = (keys.Length / len) + ((keys.Length % len) != 0 ? 1 : 0);
                byte[] temp = new byte[groups * len];
                BitConverter.ToInt32(temp, 0); // 补足
                Array.Copy(keys, 0, temp, 0, keys.Length);
                keys = temp;
            }

            return keys;
        }

    }
}
