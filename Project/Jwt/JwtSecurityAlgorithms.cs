using System;
using System.ComponentModel;

namespace FastCore.Jwt
{
    /// <summary>
    /// JWT采用的安全算法
    /// </summary>
    public enum JwtSecurityAlgorithms
    {
        /// <summary>HMAC SHA-256 散列算法</summary>
        [Description("HS256")]  
        HmacSha256,

        /// <summary>HMAC SHA-384 散列算法</summary>
        [Description("HS384")]
        HmacSha384,

        /// <summary>HMAC SHA-512 散列算法</summary>
        [Description("HS512")]
        HmacSha512
    }
}
