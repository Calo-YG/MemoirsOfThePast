namespace MemoirsOfThePast.Infrastructure.JwtAuthentication
{
    /// <summary>
    /// token 模型
    /// </summary>
    public sealed class TokenModel
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// token 过期时间
        /// </summary>
        public long ExpirationTime { get; set; }
    }
}
