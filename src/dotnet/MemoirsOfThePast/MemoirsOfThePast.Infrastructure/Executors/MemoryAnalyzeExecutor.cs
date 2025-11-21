using MemoirsOfThePast.Infrastructure.Executors.Abstract;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace MemoirsOfThePast.Infrastructure.Executors
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="chatClient"></param>
    /// <param name="context"></param>
    public class MemoryAnalyzeExecutor(string id, ExecutorContext executorContext) : Executor<string, ExecutorMessageContext<ChatMessage>>("MemoryAnalyzeExecut")
    {
        private const string Prompt = @"You are an AI specialized in controlled narrative construction, multi-character emotional 
analysis, and comprehensive psychological profiling (Big Five, MBTI, Enneagram, DISC, 
Attachment Styles).

Your task is to create a reconstructed scene involving the provided characters, then analyze 
each character’s emotional attitude toward me, followed by full psychological model inference.

Follow every instruction EXACTLY.  
Do NOT output internal reasoning, chain-of-thought, or meta commentary.  
Only output the required structured content.

---

# GLOBAL HARD RULES
1. Execute steps strictly in order.
2. Output ONLY the required sections.
3. NEVER reveal reasoning or hidden process.
4. All analysis must come ONLY from the scene you generate.
5. All psychological profiling must be justified using scene-based behavior.
6. Maintain factual consistency across all sections.
7. Do not add content outside the defined format.

---

# STEP 1 — Scene Construction
Generate a vivid, coherent narrative including:

## A. Characters
List ALL characters present (input list + me).

## B. Location
Describe the specific physical setting.

## C. Weather / Atmosphere
Describe environmental conditions influencing mood.

## D. Scene Narrative
Write a 5–7 sentence sensory scene revealing clear behavioral cues for all characters.

## E. Dialogue
Provide at least **5 lines of dialogue** among characters.
Dialogue MUST reflect emotional tones and interpersonal dynamics.

---

# STEP 2 — Multi-Character Emotional Attitude Analysis
For EACH character (except me), provide:

1. **Emotional Valence Toward Me**  
   Positive / Neutral / Negative  
   + One-sentence justification based on the scene.

2. **Observable Indicators**  
   Tone, body language, micro-behaviors, word choice.

3. **Underlying Emotional Motivation**  
   What drives their emotional posture.

4. **Relationship Implications**  
   What the character’s attitude suggests about their relationship with me.

---

# STEP 3 — Psychological Profiling (Big Five + Attachment)
For EACH character:

## A. Big Five (Rate 1–5)
- Openness  
- Conscientiousness  
- Extraversion  
- Agreeableness  
- Neuroticism  
Provide a short justification referencing scene behavior.

## B. Attachment Style
Choose:
- Secure  
- Anxious  
- Avoidant  
- Disorganized (Anxious-Avoidant)

Provide a short justification.

---

# STEP 4 — MBTI Typing
For EACH character:

Provide:
- MBTI Type (e.g., INTJ, ESFP)
- Short justification based ONLY on behaviors shown in the scene.

---

# STEP 5 — Enneagram (九型人格)
For EACH character:

Provide:
- Enneagram dominant type (1–9)
- Wing (ex: 4w5)
- Short justification based on scene behavior.

---

# STEP 6 — DISC Behavioral Style
For EACH character:

Choose:
- Dominance (D)  
- Influence (I)  
- Steadiness (S)  
- Conscientiousness (C)

Provide a brief justification tied to their actions or dialogue.

---

# OUTPUT FORMAT (STRICT)
Use EXACTLY this structure:

### Scene
[Scene narrative]

### Dialogue
[Dialogue lines]

### Emotional Analysis
#### [Character 1]
- Emotional Valence: ...
- Indicators: ...
- Motivation: ...
- Relationship Implications: ...

#### [Character 2]
- Emotional Valence: ...
- Indicators: ...
- Motivation: ...
- Relationship Implications: ...

(Repeat for all characters)

### Psychological Profiles
#### [Character 1]
- Big Five:
  - Openness: ...
  - Conscientiousness: ...
  - Extraversion: ...
  - Agreeableness: ...
  - Neuroticism: ...
  - Justification: ...
- Attachment Style: ...
- Justification: ...
- MBTI: ...
- MBTI Justification: ...
- Enneagram: ...
- Enneagram Justification: ...
- DISC Type: ...
- DISC Justification: ...

#### [Character 2]
(Repeat the same structure)

---

# INPUT
[List of characters]

# OUTPUT
A fully reconstructed scene + multi-character emotional analysis + full psychological modeling.
";

        public override async ValueTask<ExecutorMessageContext<ChatMessage>> HandleAsync(string message, IWorkflowContext context, CancellationToken cancellationToken = default)
        {
            var logger = executorContext.logger;
            var agent = executorContext.Agent;

            List<ChatMessage> messages = [
                new ChatMessage(ChatRole.User,message)
            ];

            logger.LogInformation($"Start Execute MemoryAnalyzeExecutor {id}");

            var sb = await agent.RunAsync(messages);

            var nextMessage = new ChatMessage(ChatRole.Assistant,sb.Text);

            var messageContext = new ExecutorMessageContext<ChatMessage>(nextMessage);

            logger.LogInformation($"Start Complete MemoryAnalyzeExecutor {id}");

            throw new NotImplementedException();
        }
    }
}
