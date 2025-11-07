namespace MemoirsOfThePast.Infrastructure.Options
{
    /// <summary>
    /// 大语言模型接口
    /// </summary>
    public class LLMOptions
    {
        /// <summary>
        /// api key
        /// </summary>
        public string ApiKey {get;set;}
        
        /// <summary>
        /// api 终结点
        /// </summary>
        public string Endpoint {get;set;}

        /// <summary>
        /// 模型名称
        /// </summary>
        public string Model {get;set;}
    }
}
