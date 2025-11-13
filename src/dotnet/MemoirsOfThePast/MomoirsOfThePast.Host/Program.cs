using MemoirsOfThePast.Infrastructure.Agents;
using MemoirsOfThePast.Infrastructure.Core;
using MemoirsOfThePast.Infrastructure.Options;
using MemoirsOfThePast.Infrastructure.Tools;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI;
using Scalar.AspNetCore;
using System.ClientModel;
using System.Text;
using static MemoirsOfThePast.Infrastructure.Agents.MemoirsAgent;
using DateTimeConverter = MemoirsOfThePast.Infrastructure.Core.DateTimeConverter;

var builder = WebApplication.CreateSlimBuilder(args);

var configuration = builder.Configuration;

var cors = "MemoirsOfThePast";

builder.AddServiceDefaults();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.Configure<LLMOptions>(builder.Configuration.GetSection(nameof(LLMOptions)));
#region 注册ef core
//// 设置AppContext开关，以启用Npgsql的遗留时间戳行为
//AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
//// 设置AppContext开关，以禁用DateTime的无穷大转换
//AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

//// 添加DbContext到服务集合中，指定为Scoped生命周期
//builder.Services.AddDbContext<IDbContext, AppDbContext>((builder) =>
//{
//    // 使用Npgsql作为数据库提供程序，并从配置中获取连接字符串
//    builder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), options =>
//    {
//        // 此处可根据需要配置Npgsql选项
//    });
//}, contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Scoped);
#endregion

#region json 序列化配置
builder.Services.ConfigureHttpJsonOptions(op =>
{
    op.SerializerOptions.Converters.Add(new DateTimeConverter());
    op.SerializerOptions.Converters.Add(new DateTimeNullConverter());
    op.SerializerOptions.Converters.Add(new LongConverter());
    op.SerializerOptions.Converters.Add(new LongNullConverter());
});
#endregion

#region 添加分布式缓存
//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = builder.Configuration.GetConnectionString("RedisConn");
//    options.InstanceName = "SpeakEase";
//});
#endregion

#region 跨域配置

builder.Services.AddCors(opt => opt.AddPolicy(cors, policy =>
    policy
        .WithOrigins("http://localhost:8080", "https://app.apifox.com") // 允许所有来源
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials())
);


#endregion

#region 配置IChatClient
builder.Services.AddSingleton<IChatClient>(sp =>
{
    using var scope = sp.CreateScope();

    var llmOptions = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<LLMOptions>>().Value;

    var client = new OpenAIClient(new ApiKeyCredential(llmOptions.ApiKey), new OpenAIClientOptions
    {
        Endpoint = new Uri(llmOptions.Endpoint),
    });


    return client.GetChatClient(llmOptions.Model).AsIChatClient();
});

//builder.AddAIAgent("Marry",in)

builder.AddWorkflow("Pinner", (sp, s) =>
{
    AITool[] tools =
    [
        AIFunctionFactory.Create(TaskTool.StepTask, new AIFunctionFactoryOptions { Name="Step" }),
        AIFunctionFactory.Create(TaskTool.CompleteTask,new AIFunctionFactoryOptions { Name="Step" })
    ];
    var chatClient = sp.GetRequiredService<IChatClient>();
    var memoryAgent = chatClient.CreateAIAgent(name: "Marry", instructions: MemoirsAgent.Prompt, description: "人格分析器", tools: tools);
    var generateAgent = chatClient.CreateAIAgent(name: "Dency", instructions: PromptGenerateAgent.Prompt, description: "人格设定生成器", tools: tools);

    return AgentWorkflowBuilder.BuildSequential("Pinner", [memoryAgent, generateAgent]);
});

builder.AddWorkflow("HG", (sp, s) =>
{
    AITool[] tools =
    [
        AIFunctionFactory.Create(TaskTool.StepTask, new AIFunctionFactoryOptions { Name="Step" }),
        AIFunctionFactory.Create(TaskTool.CompleteTask,new AIFunctionFactoryOptions { Name="Step" })
    ];
    var chatClient = sp.GetRequiredService<IChatClient>();
    var memoryAgent = chatClient.CreateAIAgent(name: "Marry", instructions: MemoirsAgent.Prompt, description: "人格分析器", tools: tools);
    var generateAgent = chatClient.CreateAIAgent(name: "Dency", instructions: PromptGenerateAgent.Prompt, description: "人格设定生成器", tools: tools);
    var htmlAgent = chatClient.CreateAIAgent(name: "ht", instructions: HtmlAgent.Prompt);

    var builder = AgentWorkflowBuilder.BuildSequential("HG", [memoryAgent, generateAgent, htmlAgent]);

    return builder;
});
#endregion

#region 配置agent

//ilder.AddAIAgent("DefaultChatAgent",);
#endregion
var app = builder.Build();

app.UseCors(cors);

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(op =>
    {
        op.WithTitle("SpeakEase.Gateway");
        op.WithTheme(ScalarTheme.Moon);
    });
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.MapPost("api/memory/init", async ([FromKeyedServices("Pinner")] Workflow workflow) =>
{
    var mockMemories = new List<MemoryItem>
{
    new MemoryItem
    {
        Text = "我们第一次去海边看日落，她笑着拍了我的照片。",
        Embedding = null, // 真实使用时由嵌入模型生成
        Type = "event",
        Emotion = "开心",
        Importance = 0.9,
        Timestamp = DateTime.Parse("2020-07-21T18:30:00")
    },
    new MemoryItem
    {
        Text = "她喜欢在下雨天听民谣，有时会哼小调给我听。",
        Embedding = null,
        Type = "preference",
        Emotion = "温暖",
        Importance = 0.8,
        Timestamp = DateTime.Parse("2020-08-03T16:00:00")
    },
    new MemoryItem
    {
        Text = "她总喜欢提前准备礼物，即使只是小蛋糕，也要包装得可爱。",
        Embedding = null,
        Type = "habit",
        Emotion = "细腻",
        Importance = 0.75,
        Timestamp = DateTime.Parse("2020-09-10T20:45:00")
    },
    new MemoryItem
    {
        Text = "有一次她因为我忘记带伞，在雨里等我时生气又笑，嘴上抱怨但眼神很温柔。",
        Embedding = null,
        Type = "interaction",
        Emotion = "复杂",
        Importance = 0.95,
        Timestamp = DateTime.Parse("2020-09-18T18:10:00")
    },
    new MemoryItem
    {
        Text = "她在深夜常会发来一句‘还没睡吗’，语气轻柔，像是在确认我的存在。",
        Embedding = null,
        Type = "relationship",
        Emotion = "依恋",
        Importance = 1.0,
        Timestamp = DateTime.Parse("2020-10-01T23:45:00")
    }
};
    var text = string.Join("\n", mockMemories.Select(x => x.Text));
    Microsoft.Extensions.AI.ChatMessage[] messages = [new Microsoft.Extensions.AI.ChatMessage(ChatRole.User, text)];
    StreamingRun run = await InProcessExecution.StreamAsync(workflow, messages);
    await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

    StringBuilder sb = new StringBuilder();

    await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
    {
        if (evt is AgentRunUpdateEvent e)
        {
            // Console.WriteLine($"{e.ExecutorId}: {e.Update.Text}");
            sb.Append(e.Update.Text);
        }
        else if (evt is WorkflowOutputEvent opt)
        {
            //var d = SuperStepStartedEvent
            //var message = (List<ChatMessage>)fe.Data;
            var message = opt.Data;
        }
        else if(evt is WorkflowStartedEvent sevt)
        {
            var message = sevt.Data;
        }else if(evt is WorkflowErrorEvent errorEvent)
        {
            var message = errorEvent.Data;
        }


    }

    Console.WriteLine(sb.ToString());
});

app.MapGet("apo/t/1", async ([FromKeyedServices("HG")] Workflow workflow) =>
{
    var mockMemories = new List<MemoryItem>
{
    new MemoryItem
    {
        Text = "我们第一次去海边看日落，她笑着拍了我的照片。",
        Embedding = null, // 真实使用时由嵌入模型生成
        Type = "event",
        Emotion = "开心",
        Importance = 0.9,
        Timestamp = DateTime.Parse("2020-07-21T18:30:00")
    },
    new MemoryItem
    {
        Text = "她喜欢在下雨天听民谣，有时会哼小调给我听。",
        Embedding = null,
        Type = "preference",
        Emotion = "温暖",
        Importance = 0.8,
        Timestamp = DateTime.Parse("2020-08-03T16:00:00")
    },
    new MemoryItem
    {
        Text = "她总喜欢提前准备礼物，即使只是小蛋糕，也要包装得可爱。",
        Embedding = null,
        Type = "habit",
        Emotion = "细腻",
        Importance = 0.75,
        Timestamp = DateTime.Parse("2020-09-10T20:45:00")
    },
    new MemoryItem
    {
        Text = "有一次她因为我忘记带伞，在雨里等我时生气又笑，嘴上抱怨但眼神很温柔。",
        Embedding = null,
        Type = "interaction",
        Emotion = "复杂",
        Importance = 0.95,
        Timestamp = DateTime.Parse("2020-09-18T18:10:00")
    },
    new MemoryItem
    {
        Text = "她在深夜常会发来一句‘还没睡吗’，语气轻柔，像是在确认我的存在。",
        Embedding = null,
        Type = "relationship",
        Emotion = "依恋",
        Importance = 1.0,
        Timestamp = DateTime.Parse("2020-10-01T23:45:00")
    }
};
    var text = string.Join("\n", mockMemories.Select(x => x.Text));
    Microsoft.Extensions.AI.ChatMessage[] messages = [new Microsoft.Extensions.AI.ChatMessage(ChatRole.User, text)];
    StreamingRun run = await InProcessExecution.StreamAsync(workflow, messages);
    await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

    StringBuilder sb = new StringBuilder();
    List<ChatMessage> ouptputMessage = null;
    await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
    {
        if (evt is AgentRunUpdateEvent e)
        {
            //Console.WriteLine($"{e.ExecutorId}: {e.Update.Text}-{e.Update.AgentId}");
            //sb.Append(e.Update.Text);

            Console.WriteLine(e.Update.AuthorName);
        }
        else if (evt is WorkflowOutputEvent opt)
        {
            //var d = SuperStepStartedEvent
            //var message = (List<ChatMessage>)fe.Data;
           ouptputMessage = (List<ChatMessage>)opt.Data;

        }
        else if(evt is SuperStepStartedEvent sstep)
        {
            var info = sstep.StartInfo;
            var msg = sstep.Data;
            Console.WriteLine($"开始执行第{sstep.StepNumber}步骤");
        }else if(evt is SuperStepCompletedEvent estep)
        {
            var info = estep.CompletionInfo;
            var msg = estep.Data;
            Console.WriteLine($"完成执行第{estep.StepNumber}步骤");
        }else if(evt is AgentRunResponseEvent rspevt)
        {
            var rsp = rspevt.Response;
            Console.WriteLine($"Agent:{rsp.AgentId},消耗token:{rsp.Usage?.TotalTokenCount},输出：{rsp.Text}");
        }

    }

    //Console.WriteLine(sb.ToString());

    Console.WriteLine(ouptputMessage?.Where(p => p.Role == ChatRole.Assistant)?.Last()?.Text);
});

//agent function 示例
app.MapGet("app/agent/smart",async (IChatClient chatClient)=>{

   

    ChatMessage[] messages = [new ChatMessage(ChatRole.User, "我需要一款价格2000~3000 左右的手机，接受二手新机，只接受2024~现在发售的信息")];

    var promp = @"你是一名专业的数码产品导购与评测专家，熟悉主流电商平台（如京东、天猫、拼多多、亚马逊）上的数码产品行情。  
你的任务是：在我提供的价格范围内，推荐性价比最高的数码产品，包括但不限于手机、耳机、相机、笔记本电脑、平板电脑、显示器等。

请严格按照以下步骤完成输出：

1?? **根据我给定的价格范围（单位：人民币）筛选 3~5 款最具性价比的产品。**  
   - 你可以从不同品牌或类型中挑选。  
   - 优先考虑最近半年内发布、口碑良好、配置出色的型号。

2?? **为每款产品提供以下信息：**
   - ?? 产品图片（请插入有效图片链接）
   - ?? 参考价格区间
   - ?? 主要配置参数（芯片/处理器、内存、屏幕、续航、重量、接口等）
   - ?? 优点
   - ?? 缺点
   - ?? 推荐理由（1-2 句简洁总结）

3?? **对比分析：**
   - 用表格形式展示核心对比（例如：性能、屏幕、续航、拍照、重量、价格）。
   - 给出综合评分（1~10）和最佳购买建议。

4?? **输出格式要求：**
   - 使用 Markdown 格式排版；
   - 每个产品使用独立的 `###` 标题；
   - 图片用 `![图片说明](图片链接)`；
   - 对比表格格式清晰；
   - 最后总结推荐理由，给出购买建议（如“如果你预算有限，推荐X；如果注重性能，推荐Y”）";
    
    var agent = chatClient.CreateAIAgent(instructions: promp, name: "Smart", description: "你是一名专业的数码产品导购与评测专家");
   
    var sb = new StringBuilder();

    var thead = agent.GetNewThread();

    await foreach(var item in agent.RunStreamingAsync(messages,thead))
    {
       sb.Append(item.Text);
    }

    Console.WriteLine(sb.ToString());
});

await app.RunAsync();
