using System.ComponentModel.DataAnnotations;
namespace MemoirsOfThePast.HoST.Service.User.Dto
{
    /// <summary>
    /// 注册实体
    /// </summary>
    public sealed class RegisterInput
    {
        /// <summary>
        /// 账号
        /// </summary>
        [Required(AllowEmptyStrings = false ,ErrorMessage = "请输入用户名")]
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(AllowEmptyStrings = false,ErrorMessage = "请输入密码")]
        public string Password { get; set; }
    }
}
