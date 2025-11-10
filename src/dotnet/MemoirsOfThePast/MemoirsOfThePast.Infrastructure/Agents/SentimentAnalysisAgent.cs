namespace MemoirsOfThePast.Infrastructure.Agents
{
    /// <summary>
    /// 
    /// </summary>
    public class SentimentAnalysisAgent
    {
        private const string Prompt = @"你是一个情绪分析专家，负责分析用户与智能体（Agent）之间的历史对话。

以下是最近的对话记录：
---
{conversation_history}
---

请你完成以下任务：

1. **整体情绪分析**  
   - 判断用户情绪（积极 / 中立 / 消极），并说明依据。  
   - 判断 Agent 的情绪（温柔 / 理性 / 冷淡 / 调侃等），并说明依据。  

2. **情感趋势**  
   - 总结用户情绪的变化趋势（例如：从冷淡 → 放松 → 轻微依赖）。  
   - 判断是否存在情感升温、冷却或冲突的迹象。  

3. **对话好感评估**  
   - 给出用户对 Agent 的当前好感度（0～100 分）。  
   - 简述提升或降低好感的关键对话片段。  

4. **输出格式（JSON）**
   ```json
   {
       ""user_emotion"": ""积极"",
       ""agent_emotion"": ""温柔"",
       ""trend"": ""逐渐升温"",
       ""affection_score"": 78,
       ""key_moments"": [
           ""用户开始使用亲昵称呼"",
           ""Agent 回忆往事后，用户语气变柔和""
       ]
   }
"; 
    }
}
