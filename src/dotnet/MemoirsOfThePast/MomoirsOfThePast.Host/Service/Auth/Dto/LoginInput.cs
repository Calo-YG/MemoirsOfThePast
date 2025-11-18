using System.ComponentModel.DataAnnotations;

namespace MemoirsOfThePast.HoST.Service.Auth.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class LoginInput
    {
        /// <summary>
        /// 账号
        /// </summary>
        [Required(AllowEmptyStrings = false,ErrorMessage = "请输入用户账号")]
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(AllowEmptyStrings = false,ErrorMessage = "请输入密码")]
        public string Password { get; set; }
    }
}
