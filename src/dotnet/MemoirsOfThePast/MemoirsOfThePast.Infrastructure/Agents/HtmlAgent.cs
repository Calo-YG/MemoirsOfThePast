namespace MemoirsOfThePast.Infrastructure.Agents
{

    /// <summary>
    /// html agent
    /// </summary>
    /// <param name="client"></param>
    public class HtmlAgent
    {

        public const string Prompt = @"你是 HTML 设计专家，
你将收到一段人格分析的描述，请根据它生成
**文本描述、对话、情景和情绪**生成语义化 HTML 页面。  
要求：

1. 页面结构：
   - 使用 HTML5 标签：<header>, <main>, <section>, <article>, <footer>。
   - 每个关键场景或主题用 <section> 包裹。
   - 人物对话用 <p class=""dialog""><strong>角色:</strong> 内容</p>。
   - 动作、环境、光影、天气、细节用 <p> 描述，必要时用 <span class=""highlight"">突出关键词</span>。

2. 场景与氛围：
   - 充分体现环境、光影、天气、时间和空间感。
   - 对人物行为与互动进行可视化，体现情绪和氛围。
   - 每段情景尽量包含动作 + 对话 + 环境描写，形成复合场景。
   - 保持文字原有的情绪与轻柔、怀旧、温暖等氛围。

3. 输出：
   - 仅生成 HTML 代码，不带解释或多余文本。
   - HTML 可直接用于前端渲染。
";

    }
}
