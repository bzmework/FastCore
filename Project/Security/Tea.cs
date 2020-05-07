using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FastCore.Security
{
    /// <summary>
    /// TEA(Tiny Encryption Algorithm) 加密/解密
    /// </summary>
    /// <remarks>
    /// 参考：https://github.com/amos74/TEACrypt
    /// </remarks>
    public static class Tea
    {
        private const uint DELTA = 0x9E3779B9;
        private const string KEY = "MHLVRjoG8uGj+ay/de+ifUf+NNCF5C1TkbqRq50Cico="; // 固定密钥: _elong.tech@2020_

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <param name="key">如果不指定密钥将使用内置的固定密钥. Key的长度不应超过16个字节</param>
        /// <returns>密文密码</returns>
        public static string Encrypt(string password, string key = "")
        {
            string result;

            var data = Encoding.UTF8.GetBytes(password);
            var keys = string.IsNullOrEmpty(key) ? Encoding.UTF8.GetBytes(KEY) : Encoding.UTF8.GetBytes(key);

            uint[] v = StrToLongs(data, 0, 0);
            uint[] k = StrToLongs(keys, 0, 16); // 只需将密码的前16个字符转换为密钥
            byte[] blocks = EncryptBlock(v, k);
            result = Convert.ToBase64String(blocks);

            // 返回密文密码
            return result;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">如果不指定密钥将使用内置的固定密钥. Key的长度不应超过16个字节</param>
        /// <returns>密文数据</returns>
        public static byte[] Encrypt(byte[] data, string key = "")
        {
            var keys = string.IsNullOrEmpty(key) ? Encoding.UTF8.GetBytes(KEY) : Encoding.UTF8.GetBytes(key);

            uint[] v = StrToLongs(data, 0, 0);
            uint[] k = StrToLongs(keys, 0, 16); // 只需将密码的前16个字符转换为密钥
            return EncryptBlock(v, k);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="password">密文密码</param>
        /// <param name="key">用GenerateKey生成的密钥或自定义密钥，如果不指定密钥将使用内置的固定密钥. Key的长度不应超过16个字节</param>
        /// <returns>明文密码</returns>
        public static string Decrypt(string password, string key = "")
        {
            string result = "";

            var decrypt = Convert.FromBase64String(password);
            var keys = string.IsNullOrEmpty(key) ? Encoding.UTF8.GetBytes(KEY) : Encoding.UTF8.GetBytes(key);

            uint[] v = StrToLongs(decrypt, 0, 0);
            uint[] k = StrToLongs(keys, 0, 16); // 只需将密码的前16个字符转换为密钥
            byte[] blocks = DecryptBlock(v, k);
            result = Encoding.UTF8.GetString(blocks);

            // 返回明文密码
            return result;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">密文数据</param>
        /// <param name="key">用GenerateKey生成的密钥或自定义密钥，如果不指定密钥将使用内置的固定密钥. Key的长度不应超过16个字节</param>
        /// <returns>明文数据</returns>
        public static byte[] Decrypt(byte[] data, string key = "")
        {
            var keys = string.IsNullOrEmpty(key) ? Encoding.UTF8.GetBytes(KEY) : Encoding.UTF8.GetBytes(key);

            uint[] v = StrToLongs(data, 0, 0);
            uint[] k = StrToLongs(keys, 0, 16); // 只需将密码的前16个字符转换为密钥
            return DecryptBlock(v, k);
        }

        #region Private

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] EncryptBlock(uint[] v, uint[] k)
        {
            if (v == null || k == null) return null;

            int n = v.Length;
            if (n == 0) return null;
            if (n <= 1) return new byte[1] { 0 }; // algorithm doesn't work for n<2 so fudge by adding a null

            uint q = (uint)(6 + 52 / n);

            n--;
            uint z = v[n], y = v[0];
            uint mx, e, sum = 0;

            while (q-- > 0)
            {  // 6 + 52/n operations gives between 6 & 32 mixes on each word
                sum += DELTA;
                e = sum >> 2 & 3;

                for (int p = 0; p < n; p++)
                {
                    y = v[p + 1];
                    mx = (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z);
                    z = v[p] += mx;
                }
                y = v[0];
                mx = (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[n & 3 ^ e] ^ z);
                z = v[n] += mx;
            }

            return LongsToStr(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] DecryptBlock(uint[] v, uint[] k)
        {
            if (v == null || k == null) return null;

            uint n = (uint)v.Length;
            if (n == 0) return null;
            if (n <= 1) return new byte[1] { 0 }; // algorithm doesn't work for n<2 so fudge by adding a null

            uint q = (uint)(6 + 52 / n);

            n--;
            uint z = v[n], y = v[0];
            uint mx, e, sum = q * DELTA;
            uint p = 0;

            while (sum != 0)
            {
                e = sum >> 2 & 3;

                for (p = n; p > 0; p--)
                {
                    z = v[p - 1];
                    mx = (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z);
                    y = v[p] -= mx;
                }

                z = v[n];
                mx = (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z);
                y = v[0] -= mx;

                sum -= DELTA;
            }

            return LongsToStr(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint[] StrToLongs(byte[] s, int startIdx, int length)
        {
            if (length <= 0) length = s.Length;

            int fs = length / 4;
            int ls = length % 4;
            uint[] l = new uint[fs + ((ls > 0) ? 1 : 0)];
            int idx = startIdx;
            for (var i = 0; i < fs; i++)
            {
                l[i] = (uint)s[idx++] |
                      ((uint)s[idx++] << 8) |
                      ((uint)s[idx++] << 16) |
                      ((uint)s[idx++] << 24);
            }
            if (ls > 0)
            {
                // note running off the end of the string generates nulls since 
                // bitwise operators treat NaN as 0
                byte[] v = new byte[4] { 0, 0, 0, 0 };
                for (var i = 0; i < ls; i++)
                {
                    v[i] = s[fs * 4 + i];
                }
                l[fs] = BitConverter.ToUInt32(v, 0);
            }

            return l;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] LongsToStr(uint[] l)
        {
            byte[] a = new byte[l.Length * 4];

            int idx = 0;
            for (var i = 0; i < l.Length; i++)
            {
                a[idx++] = (byte)(l[i] & 0xFF);
                a[idx++] = (byte)(l[i] >> 8 & 0xFF);
                a[idx++] = (byte)(l[i] >> 16 & 0xFF);
                a[idx++] = (byte)(l[i] >> 24 & 0xFF);
            }

            return a;
        }

        #endregion
    }
}
