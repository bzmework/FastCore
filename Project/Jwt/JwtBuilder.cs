using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FastCore.Json;
using FastCore.Security;
using System.Reflection;
using FastCore.HashAlg;
using FastCore.UniqueID;

namespace FastCore.Jwt
{
    /// <summary>
    /// JWT令牌构造器
    /// </summary>
    /// <remarks>构造令牌和解码令牌</remarks>
    public class JwtBuilder
    {
        private const string KEY = "MHLVRjoG8uGj+ay/de+ifUf+NNCF5C1TkbqRq50Cico="; // 固定密钥: _elong.tech@2020_
        private bool _useAes = false; // 使用Aes进行二次加密
        private JwtSecurityAlgorithms _algorithm = JwtSecurityAlgorithms.HmacSha256; // 签名算法
        private string _secret = KEY; // 签名密钥
        private IDictionary<string, object> _header = new Dictionary<string, object>(); // 头
        private IDictionary<string, object> _payload = new Dictionary<string, object>(); // 负载

        /// <summary>
        /// 生成一个密钥
        /// </summary>
        /// <param name="password">明文密码，为空则随机生成密钥</param>
        /// <returns>返回Base64编码的密钥，必须保存好不要丢失</returns>
        public static string GenerateKey(string password = "")
        {
            string key;
            if (string.IsNullOrEmpty(password))
            {
                key = KeyGen.GenerateAesKey(); // 由系统随机生成Key
            }
            else
            {
                key = Aes.Encrypt(password); // 根据密码生成Key
            }
            return key;
        }

        /// <summary>
        /// 设置JWT采用的算法
        /// </summary>
        /// <param name="algorithm">算法，参见JwtSecurityAlgorithms</param>
        /// <returns>返回当前构造器实例</returns>
        public JwtBuilder WithAlgorithm(JwtSecurityAlgorithms algorithm)
        {
            _algorithm = algorithm;
            return this;
        }

        /// <summary>
        /// 设置JWT签名密钥
        /// </summary>
        /// <param name="secret">密钥</param>
        /// <returns>返回当前构造器实例</returns>
        public JwtBuilder WithSecret(string secret)
        {
            _secret = string.IsNullOrEmpty(secret) ? KEY : secret;
            return this;
        }

        /// <summary>
        /// 使用Aes对JWT令牌进行加密
        /// </summary>
        /// <param name="sign">签名</param>
        /// <returns>返回当前构造器实例</returns>
        public JwtBuilder WithAes(bool sign)
        {
            _useAes = sign;
            return this;
        }

        /// <summary>
        /// 增加Claim
        /// </summary>
        /// <param name="name">Claim名称</param>
        /// <param name="value">Claim值</param>
        /// <exception cref="ArgumentNullException" />
        /// <returns>返回当前构造器实例</returns>
        public JwtBuilder AddClaim(JwtClaimNames name, object value)
        {
            AddClaim(name.ToStr(), value);
            return this;
        }

        /// <summary>
        /// 增加Claim
        /// </summary>
        /// <param name="name">Claim名称，名称可以是自定义名称，也可以是已注册名称，参见JwtClaimNames</param>
        /// <param name="value">Claim值</param>
        /// <exception cref="ArgumentNullException" />
        /// <returns>返回当前构造器实例</returns>
        public JwtBuilder AddClaim(string name, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (name.Equals(JwtClaimNames.ExpirationTime.ToStr())) // 设置过期时间
            {
                int seconds = ObjectConvert.C2Int(value);
                if(seconds > 0)
                {
                    value = DateTime.Now.AddSeconds(seconds).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                }
                else
                {
                    value = DateTime.Now.AddHours(2).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                }
                _payload.Add(name, value);
            }
            else if (value is DateTime dt) // 格式化成标准时间
            {
                _payload.Add(name, dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            }
            else
            {
                _payload.Add(name, value);
            }

            return this;
        }

        /// <summary>
        /// 生成JWT令牌
        /// </summary>
        /// <returns>返回生成的JWT令牌</returns>
        public string Build()
        {
            // JWT令牌
            var jwt = new List<string>(3);

            // 创建头部
            _header["typ"] = "JWT";// 类型
            _header["alg"] = _algorithm.ToStr();// 算法
            var headerJson = Json.Json.Serialize(_header); // 序列化成Json
            var headerBytes = Encoding.UTF8.GetBytes(headerJson); // 编码成UTF8
            var headerStr = JwtBase64Url.Encode(headerBytes); // 编码成Base64Url
            jwt.Add(headerStr); 

            // 创建负载
            if (!_payload.ContainsKey(JwtClaimNames.Issuer.ToStr())) // 签发人
                 _payload[JwtClaimNames.Issuer.ToStr()] = "eLong.Tech";
            if (!_payload.ContainsKey(JwtClaimNames.IssuedAt.ToStr())) // 颁发时间
                 _payload[JwtClaimNames.IssuedAt.ToStr()] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            if (!_payload.ContainsKey(JwtClaimNames.NotBefore.ToStr())) // 生效时间
                 _payload[JwtClaimNames.NotBefore.ToStr()] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            if (!_payload.ContainsKey(JwtClaimNames.ExpirationTime.ToStr())) // 过期时间(默认2小时)
                 _payload[JwtClaimNames.ExpirationTime.ToStr()] = DateTime.Now.AddHours(2).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            _payload[JwtClaimNames.JwtId.ToStr()] = new GuidCombGenerator().NextId(); // 为Jwt指定一个UID，由于JWT令牌很长，此UID用来在Redis等缓存中标识其唯一性减少空间占用非常有用。
            var payloadJson = Json.Json.Serialize(_payload); // 序列化成Json
            var payloadBytes = Encoding.UTF8.GetBytes(payloadJson); // 编码成UTF8
            var payloadStr = JwtBase64Url.Encode(payloadBytes); // 编码成Base64Url
            jwt.Add(payloadStr);

            // 根据密钥和算法生成签名
            var key = Encoding.UTF8.GetBytes(_secret);
            var signatureData = string.Join(".", jwt.ToArray()); // 生成签名数据
            var signatureBytes = Encoding.UTF8.GetBytes(signatureData); // 编码成UTF8
            byte[] signatureSha; 
            switch (_algorithm) // 进行签名
            {
                case JwtSecurityAlgorithms.HmacSha256: signatureSha = Hash.HSha256(key, signatureBytes); break;
                case JwtSecurityAlgorithms.HmacSha384: signatureSha = Hash.HSha384(key, signatureBytes); break;
                case JwtSecurityAlgorithms.HmacSha512: signatureSha = Hash.HSha512(key, signatureBytes); break;
                default: throw new InvalidOperationException($"不支持的签名算法[{_algorithm}]");
            }
            var signatureStr = JwtBase64Url.Encode(signatureSha); // 编码成Base64Url
            jwt.Add(signatureStr);

            // 生成令牌
            var jwtStr = string.Join(".", jwt.ToArray());

            // 返回令牌
            if (_useAes)
            {
                var jwtResult = JwtBase64Url.Encode(Aes.Encrypt(Encoding.UTF8.GetBytes(jwtStr))); // 对JWT令牌进行二次加密，防止恶意解开头和负载部分
                return jwtResult; 
            }
            else
            {
                return jwtStr;
            }
        }

        /// <summary>
        /// 解码JWT令牌
        /// </summary>
        /// <param name="token">JWT令牌</param>
        /// <param name="verifySignature">验证签名</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <returns>返回负载字典</returns>
        public IDictionary<string, object> Decode(string token, bool verifySignature = true)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException(nameof(token));

            // 令牌解密
            string jwtStr;
            if (_useAes)
            {
                var jwtBytes = JwtBase64Url.Decode(token); // 解码
                jwtStr = Encoding.UTF8.GetString(Aes.Decrypt(jwtBytes)); // 解密
            }
            else
            {
                jwtStr = token;
            }

            var parts = jwtStr.Split('.');
            if (parts.Length != 3)
                throw new ArgumentOutOfRangeException(token, "令牌必须由3个由点分隔的部分组成");

            // 验证签名
            if (verifySignature)
            {
                var header = parts[0]; // 头
                var payload = parts[1]; // 负载

                // 根据密钥和算法生成签名
                var key = Encoding.UTF8.GetBytes(_secret);
                var signatureData = $"{header}.{payload}"; // 生成签名数据
                var signatureBytes = Encoding.UTF8.GetBytes(signatureData); // 编码成UTF8
                byte[] signatureSha;
                switch (_algorithm) // 进行签名
                {
                    case JwtSecurityAlgorithms.HmacSha256: signatureSha = Hash.HSha256(key, signatureBytes); break;
                    case JwtSecurityAlgorithms.HmacSha384: signatureSha = Hash.HSha384(key, signatureBytes); break;
                    case JwtSecurityAlgorithms.HmacSha512: signatureSha = Hash.HSha512(key, signatureBytes); break;
                    default: throw new InvalidOperationException($"不支持的签名算法[{_algorithm}]");
                }
                var signatureStr = JwtBase64Url.Encode(signatureSha); // 编码成Base64Url
                
                if(!signatureStr.Equals(parts[2], StringComparison.InvariantCultureIgnoreCase)) // 比对签名
                {
                    throw new Exception("令牌属于伪造");
                }
            }
            
            // 解码负载
            var payloadDecode = JwtBase64Url.Decode(parts[1]); 
            var json = Encoding.UTF8.GetString(payloadDecode);
            var result = Json.Json.Deserialize<IDictionary<string, object>>(json);

            return result;
        }

        /// <summary>
        /// 解码JWT令牌
        /// </summary>
        /// <param name="token">JWT令牌</param>
        /// <param name="verifySignature">验证签名</param>
        /// <returns>返回指定的对象</returns>
        public T Decode<T>(string token, bool verifySignature = true) where T: class
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException(nameof(token));

            // 令牌解密
            string jwtStr;
            if (_useAes)
            {
                var jwtBytes = JwtBase64Url.Decode(token); // 解码
                jwtStr = Encoding.UTF8.GetString(Aes.Decrypt(jwtBytes)); // 解密
            }
            else
            {
                jwtStr = token;
            }

            var parts = jwtStr.Split('.');
            if (parts.Length != 3)
                throw new ArgumentOutOfRangeException(token, "令牌必须由3个由点分隔的部分组成");

            // 验证签名
            if (verifySignature)
            {
                var header = parts[0]; // 头
                var payload = parts[1]; // 负载

                // 根据密钥和算法生成签名
                var key = Encoding.UTF8.GetBytes(_secret);
                var signatureData = $"{header}.{payload}"; // 生成签名数据
                var signatureBytes = Encoding.UTF8.GetBytes(signatureData); // 编码成UTF8
                byte[] signatureSha;
                switch (_algorithm) // 进行签名
                {
                    case JwtSecurityAlgorithms.HmacSha256: signatureSha = Hash.HSha256(key, signatureBytes); break;
                    case JwtSecurityAlgorithms.HmacSha384: signatureSha = Hash.HSha384(key, signatureBytes); break;
                    case JwtSecurityAlgorithms.HmacSha512: signatureSha = Hash.HSha512(key, signatureBytes); break;
                    default: throw new InvalidOperationException($"不支持的签名算法[{_algorithm}]");
                }
                var signatureStr = JwtBase64Url.Encode(signatureSha); // 编码成Base64Url

                if (!signatureStr.Equals(parts[2], StringComparison.InvariantCultureIgnoreCase)) // 比对签名
                {
                    throw new Exception("令牌属于伪造");
                }
            }

            // 解码负载
            var payloadDecode = JwtBase64Url.Decode(parts[1]);
            var json = Encoding.UTF8.GetString(payloadDecode);
            var result = Json.Json.Deserialize<T>(json);

            return result;
        }

        /// <summary>
        /// 解析JWT令牌
        /// </summary>
        /// <param name="token">JWT令牌</param>
        /// <param name="claim">Claim名称</param>
        /// <returns>返回指定的Claim值，解析失败返回null</returns>
        public object Parse(string token, JwtClaimNames claim)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException(nameof(token));

            // 令牌解密
            string jwtStr;
            if (_useAes)
            {
                var jwtBytes = JwtBase64Url.Decode(token); // 解码
                jwtStr = Encoding.UTF8.GetString(Aes.Decrypt(jwtBytes)); // 解密
            }
            else
            {
                jwtStr = token;
            }

            var parts = jwtStr.Split('.');
            if (parts.Length != 3)
                throw new ArgumentOutOfRangeException(token, "令牌必须由3个由点分隔的部分组成");

            // 解码负载
            var payloadDecode = JwtBase64Url.Decode(parts[1]);
            var json = Encoding.UTF8.GetString(payloadDecode);
            var claims = Json.Json.Deserialize<IDictionary<string, object>>(json);
            if (claims != null && claims.TryGetValue(claim.ToStr(), out var result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// 校验JWT令牌是否有效
        /// </summary>
        /// <param name="token">JWT令牌</param>
        /// <returns>有效返回空字符串，无效返回错误</returns>
        public string Verify(string token)
        {
            string errMsg = "";
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    throw new ArgumentException(nameof(token));

                // 令牌解密
                string jwtStr;
                if (_useAes)
                {
                    var jwtBytes = JwtBase64Url.Decode(token); // 解码
                    jwtStr = Encoding.UTF8.GetString(Aes.Decrypt(jwtBytes)); // 解密
                }
                else
                {
                    jwtStr = token;
                }

                var parts = jwtStr.Split('.');
                if (parts.Length != 3)
                    throw new ArgumentOutOfRangeException(token, "令牌必须由3个由点分隔的部分组成");

                // 验证签名
                var header = parts[0]; // 头
                var payload = parts[1]; // 负载

                // 根据密钥和算法生成签名
                var key = Encoding.UTF8.GetBytes(_secret);
                var signatureData = $"{header}.{payload}"; // 生成签名数据
                var signatureBytes = Encoding.UTF8.GetBytes(signatureData); // 编码成UTF8
                byte[] signatureSha;
                switch (_algorithm) // 进行签名
                {
                    case JwtSecurityAlgorithms.HmacSha256: signatureSha = Hash.HSha256(key, signatureBytes); break;
                    case JwtSecurityAlgorithms.HmacSha384: signatureSha = Hash.HSha384(key, signatureBytes); break;
                    case JwtSecurityAlgorithms.HmacSha512: signatureSha = Hash.HSha512(key, signatureBytes); break;
                    default: throw new InvalidOperationException($"不支持的签名算法[{_algorithm}]");
                }
                var signatureStr = JwtBase64Url.Encode(signatureSha); // 编码成Base64Url

                if (!signatureStr.Equals(parts[2], StringComparison.InvariantCultureIgnoreCase)) // 比对签名
                {
                    throw new Exception("令牌属于伪造");
                }

                return errMsg;
            }
            catch(Exception e)
            {
                errMsg = e.Message;
            }

            return errMsg;
        }

    }
}
