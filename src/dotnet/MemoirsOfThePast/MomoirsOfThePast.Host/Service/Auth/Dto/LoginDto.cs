namespace MemoirsOfThePast.HoST.Service.Auth.Dto
{
    public sealed class LoginDto
    {
        /// <summary>
        /// token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 刷新token
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
