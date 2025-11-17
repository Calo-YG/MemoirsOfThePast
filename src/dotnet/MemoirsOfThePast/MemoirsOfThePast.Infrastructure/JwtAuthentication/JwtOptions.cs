using Microsoft.AspNetCore.Authentication;

namespace MemoirsOfThePast.Infrastructure.JwtAuthentication;

public class JwtOptions:AuthenticationSchemeOptions
{
    /// <summary>
    /// 颁发者
    /// </summary>
    public string Issuer { get; set; } = "speak.ease";

    /// <summary>
    /// 接收者
    /// </summary>
    public string Audience { get; set; } = "user";

    /// <summary>
    /// 对称安全密钥
    /// </summary>
    public string SymmetricSecurityKey { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 过期时间 分
    /// </summary>
    public int ExpirationTime { get; set; } = 20;

    /// <summary>
    /// 刷新过期时间 分
    /// </summary>
    public int RefreshExpirationTime { get; set; } = 20160;
}
