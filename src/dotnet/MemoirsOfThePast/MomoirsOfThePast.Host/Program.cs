using MemoirsOfThePast.Infrastructure.Agents;
using MemoirsOfThePast.Infrastructure.Core;
using MemoirsOfThePast.Infrastructure.Options;
using MemoirsOfThePast.Infrastructure.Tools;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using Scalar.AspNetCore;
using System.ClientModel;
using System.Text;
using static MemoirsOfThePast.Infrastructure.AgentFrameworkSample.AgentExecutor;
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

app.MapPost("agent/executor",async (IChatClient chatClient) =>
{
    var sloganWriter = new SloganWriterExecutor("SloganWriter", chatClient);
    var feedbackProvider = new FeedbackExecutor("FeedbackProvider", chatClient);

    // Build the workflow by adding executors and connecting them
    var workflow = new WorkflowBuilder(sloganWriter)
        .AddEdge(sloganWriter, feedbackProvider)
        .AddEdge(feedbackProvider, sloganWriter)
        .WithOutputFrom(feedbackProvider)
        .Build();

    // Execute the workflow
    await using StreamingRun run = await InProcessExecution.StreamAsync(workflow, input: "Create a slogan for a new electric SUV that is affordable and fun to drive.");
    await foreach (WorkflowEvent evt in run.WatchStreamAsync())
    {
        if (evt is SloganGeneratedEvent or FeedbackEvent)
        {
            // Custom events to allow us to monitor the progress of the workflow.
            Console.WriteLine($"{evt}");
        }

        if (evt is WorkflowOutputEvent outputEvent)
        {
            Console.WriteLine($"{outputEvent}");
        }
    }
});
await app.RunAsync();
