using MemoirsOfThePast.Infrastructure.Agents;
using MemoirsOfThePast.Infrastructure.Core;
using MemoirsOfThePast.Infrastructure.EntityFrameworkCore;
using MemoirsOfThePast.Infrastructure.Options;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Workflows;
using Microsoft.EntityFrameworkCore;
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

var cors = "MemoirsOfThePast"; ;

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


builder.AddWorkflow("Pinner", (sp, s) =>
{
    var chatClient = sp.GetRequiredService<IChatClient>();
    var memoryAgent = chatClient.CreateAIAgent(name: "Marry", instructions: MemoirsAgent.Prompt, description: "人格分析器");
    var generateAgent = chatClient.CreateAIAgent(name: "Dency", instructions: PromptGenerateAgent.Prompt, description: "人格设定生成器");

    return AgentWorkflowBuilder.BuildSequential("Pinner", [memoryAgent, generateAgent]);
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

app.MapPost("api/memory/init",async ([FromKeyedServices("Pinner")]Workflow workflow ) =>
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
    ChatMessage[] messages = [new ChatMessage(ChatRole.User, text)];
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


    }

    Console.WriteLine(sb.ToString());
});




await app.RunAsync();