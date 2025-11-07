namespace MemoirsOfThePast.Infrastructure.JwtAuthentication
{
    /// <summary>
    /// 用户上下文
    /// </summary>
    public class UserContext:IUserContext
    {
        /// <summary>
        /// 用户
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; }
    }
}
