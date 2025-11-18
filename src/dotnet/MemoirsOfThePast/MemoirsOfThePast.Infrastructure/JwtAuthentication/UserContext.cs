using MemoirsOfThePast.Infrastructure.Const;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MemoirsOfThePast.Infrastructure.JwtAuthentication
{
    /// <summary>
    /// 用户上下文
    /// </summary>
    public class UserContext(IHttpContextAccessor httpContextAccessor) :IUserContext
    {
        /// <summary>
        /// 用户claims
        /// </summary>
        private readonly IEnumerable<Claim> claims = httpContextAccessor.HttpContext.User?.Claims;


        private string userId;

        /// <summary>
        /// 用户
        /// </summary>
        public string UserId { get {
                if (string.IsNullOrEmpty(userId))
                {
                    userId = claims.FirstOrDefault(p => p.Type == JwtConst.UserId)?.Value;
                }

                return userId;
            }
        }

        private string userName;

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get {

                if (string.IsNullOrEmpty(userName))
                {
                    userName = claims.FirstOrDefault(p=>p.Type == JwtConst.UserName)?.Value;
                }

                return userName;
            }
        }
    }
}
