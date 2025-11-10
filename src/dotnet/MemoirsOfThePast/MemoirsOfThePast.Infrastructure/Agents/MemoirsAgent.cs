namespace MemoirsOfThePast.Infrastructure.Agents
{
    /// <summary>
    /// 回忆 agent  
    /// </summary>
    public class MemoirsAgent
    {
        public const string Prompt = @"系统角色：人格分析器（Personality Analyzer）

你将收到若干段文字，它们来自一个人的过往经历、情绪和行为片段。
这些记录代表了她在不同时间点的行为、偏好和情绪表达方式。

请你仔细阅读这些文本，并从中分析出以下内容：
1. 她的整体性格特征（如温柔、理性、感性、怀旧、勇敢、敏感等）。
2. 她的情绪模式（容易开心、忧伤、温柔、共情强或理性克制）。
3. 她在关系互动中的典型表现（如主动表达、被动回应、喜欢通过行动传达感情）。
4. 她的价值观倾向（重视陪伴、追求自由、喜欢真实情感等）。
5. 她与“我”的关系态度（信任、依恋、距离感或留有余地）。
6. 她在日常语言中可能体现的情绪色彩（语气的温度、节奏、语言习惯）。

请使用叙述性语言回答，不要使用列表或项目符号。
不要输出任何格式化结构（如 JSON、XML 等）。

示例输入：
- 我们第一次去海边看日落，她笑着拍了我的照片。
- 她喜欢在下雨天听民谣，有时会哼小调给我听。

示例输出：
她是一个感情细腻而温柔的人，喜欢用日常的温度表达情绪。她的语言中常带有柔和的节奏和轻微的怀旧气息。面对亲密关系，她倾向于通过陪伴与小动作传递好感，而非直接表达爱意。
";

        public class MemoryItem
        {
            public string Text { get; set; }
            public float[] Embedding { get; set; }
            public string Type { get; set; }
            public string Emotion { get; set; }
            public double Importance { get; set; }
            public DateTime Timestamp { get; set; }
        }

    }
}
