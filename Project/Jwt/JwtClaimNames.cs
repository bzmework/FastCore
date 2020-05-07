using System;
using System.ComponentModel;

namespace FastCore.Jwt
{
    /// <summary>
    /// 来自不同来源的已注册claim名称清单
    /// </summary>
    /// <remarks>
    /// http://tools.ietf.org/html/rfc7519#section-4
    /// http://openid.net/specs/openid-connect-core-1_0.html#IDToken
    /// </remarks>
    public enum JwtClaimNames
    {
        /// <summary>签发人，令牌由谁签发，例如某某公司或个人</summary>
        /// <remarks>http://tools.ietf.org/html/rfc7519#section-4</remarks>
        [Description("iss")]
        Issuer,

        /// <summary>主题，对令牌的简单描述</summary>
        /// <remarks>http://tools.ietf.org/html/rfc7519#section-4</remarks>
        [Description("sub")]
        Subject,

        /// <summary>受众，令牌颁发给谁使用</summary>
        /// <remarks>http://tools.ietf.org/html/rfc7519#section-4</remarks>
        [Description("aud")]
        Audience,

        /// <summary>过期时间，令牌的过期时间。按照规范，应该设置成秒数</summary>
        /// <remarks>http://tools.ietf.org/html/rfc7519#section-4</remarks>
        [Description("exp")]
        ExpirationTime,

        /// <summary>生效时间，令牌的生效时间</summary>
        /// <remarks>http://tools.ietf.org/html/rfc7519#section-4</remarks>
        [Description("nbf")]
        NotBefore,

        /// <summary>签发时间，令牌的签发时间</summary>
        /// <remarks>http://tools.ietf.org/html/rfc7519#section-4</remarks>
        [Description("iat")]
        IssuedAt,

        /// <summary>编号，令牌的编号</summary>
        /// <remarks>http://tools.ietf.org/html/rfc7519#section-4</remarks>
        [Description("jti")]
        JwtId,

        /// <summary>电子邮箱</summary>
        /// <remarks>https://openid.net/specs/openid-connect-core-1_0.html#StandardClaims</remarks>
        [Description("email")]
        Email,

        /// <summary>Web站点</summary>
        /// <remarks>https://openid.net/specs/openid-connect-core-1_0.html#StandardClaims</remarks>
        [Description("website")]
        Website,

        /// <summary>办公地址</summary>
        /// <remarks>https://openid.net/specs/openid-connect-core-1_0.html#StandardClaims</remarks>
        [Description("address")]
        Address,

        /// <summary>电话号码</summary>
        /// <remarks>https://openid.net/specs/openid-connect-core-1_0.html#StandardClaims</remarks>
        [Description("phone_number")]
        PhoneNumber,

        /// <summary>授权时间</summary>
        /// <remarks>http://openid.net/specs/openid-connect-core-1_0.html#IDToken</remarks>
        [Description("auth_time")]
        AuthTime,

        /// <summary>语言，例如：zh-CN, en-US</summary>
        /// <remarks>https://openid.net/specs/openid-connect-core-1_0.html#StandardClaims</remarks>
        [Description("locale")]
        Locale
    }
}
