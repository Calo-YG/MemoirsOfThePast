namespace MemoirsOfThePast.Infrastructure.JwtAuthentication
{
    /// <summary>
    /// jwt 验证结果
    /// </summary>
    public sealed class JwtAuthenticateResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }
    }
}
