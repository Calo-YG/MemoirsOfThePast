namespace MemoirsOfThePast.HoST.Service.Auth.Dto
{
    /// <summary>
    /// 刷新token
    /// </summary>
    public sealed class RefreshInput
    {
        /// <summary>
        /// 刷新token
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public string UserId { get; set; }
    }
}
