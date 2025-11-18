using System.ComponentModel.DataAnnotations;

namespace MemoirsOfThePast.HoST.Service.Fragment.Dto
{
    public class FragmentListInput
    {
        [Required(AllowEmptyStrings = false,ErrorMessage = "请输入回忆id")]
        public string MemoryId { get; set; }

        public string Name { get; set; }
    }
}
