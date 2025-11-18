using System.ComponentModel.DataAnnotations;

namespace MemoirsOfThePast.HoST.Service.Fragment.Dto
{
    public sealed class CreateFramentInput
    {
        /// <summary>
        /// 回忆id
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "请输入回忆id")]
        public string MemoryId { get; set; }

        /// <summary>
        /// 回忆藐视
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "请输入描述")]
        public string Description { get; set; }

        /// <summary>
        /// 发生时间
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "请选择时间")]
        public DateTime OccurDate { get; set; }

        /// <summary>
        /// 地点
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "请输入地点")]
        public string Location { get; set; }

        /// <summary>
        /// 场景
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "请输入场景")]
        public string Scene { get; set; }
    }
}
