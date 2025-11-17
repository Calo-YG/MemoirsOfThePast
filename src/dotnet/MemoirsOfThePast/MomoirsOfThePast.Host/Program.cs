using MemoirsOfThePast.Infrastructure.Core;
using MemoirsOfThePast.Infrastructure.Options;
using MemoirsOfThePast.Infrastructure.SqlBot;
using MemoirsOfThePast.Infrastructure.SqlBot.SqlBotEvent;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI;
using Scalar.AspNetCore;
using System.ClientModel;
using static MemoirsOfThePast.Infrastructure.AgentFrameworkSample.AgentExecutor;
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
#endregion

#region 配置agent

builder.Services.AddTransient<ISqlAgent, SqlAgent>();
//ilder.AddAIAgent("DefaultChatAgent",);
builder.Services.AddAIAgent(SqlMessageAnalyzeExecutor.AgentName, (sp, s) =>
{
    var chatClient = sp.GetRequiredService<IChatClient>();
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

    return SqlAgent.CreateMessageAnalyzeAIAgent(chatClient,loggerFactory);
});
builder.Services.AddAIAgent(SqlErrorAmendExecutor.AgentName, (sp, s) =>
{
    var chatClient = sp.GetRequiredService<IChatClient>();
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

    return SqlAgent.CreateAmendAIAgent(chatClient, loggerFactory);
});
builder.Services.AddAIAgent(SqlPerformanceExecutor.AgentName, (sp, s) =>
{
    var chatClient = sp.GetRequiredService<IChatClient>();
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

    return SqlAgent.CreatePerformanceAIAgent(chatClient, loggerFactory);
});
builder.Services.AddAIAgent(SqlGenerateExecutor.AgentName, (sp, s) =>
{
    var chatClient = sp.GetRequiredService<IChatClient>();
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

    return SqlAgent.CreateGenereateAIAgent(chatClient, loggerFactory);
});
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

app.MapPost("agent/sqlbot/executor", async (ISqlAgent sqlAgent,ILoggerFactory factory) =>
{
    var workflow = sqlAgent.CreateWorkflow();

    var logger = factory.CreateLogger("sqlbot");

    await using StreamingRun run = await InProcessExecution.StreamAsync(workflow, input: @"WITH InvoiceTotal AS (
  SELECT
    OrderInvoiceId,
    SUM ( RealTotalPrice ) AS 开票总金额 
  FROM
    (
    SELECT DISTINCT
      oii.OrderInvoiceId,
      oii.OrderInvoiceNo,
      ISNULL( oid.RealInvoicePrice, 0 ) AS RealTotalPrice 
    FROM
      Client_OrderInvoice oii
      LEFT JOIN Client_OrderInvoiceDetial oid ON oid.OrderInvoiceId = oii.OrderInvoiceId
      LEFT JOIN Client_OrderInvoiceLink oil ON oil.OrderInvoiceId = oii.OrderInvoiceId
      LEFT JOIN Client_Order oi ON oi.OrderId = oil.OrderId 
    WHERE
      oii.CreateDate >= '2025-09-01 00:00:00' 
      AND oii.CreateDate <= '2025-09-30 23:59:59' 
      AND oi.PaymentCategoryId != 7 --and  oii.OrderInvoiceId ='0253a3ab-71f9-4d35-82c7-2a26ea21e4bc'
      
    ) AS t 
  GROUP BY
    t.OrderInvoiceId 
  ),
  Detail AS (
  SELECT
    oii.OrderInvoiceId,
    oii.OrderInvoiceNo AS 开票单号,
    oii.CreateDate AS 申请时间,
    oi.OrderNo AS 订单号,
    pr.GoodsNo AS 款号,
    oe.CostPrice AS 金额,
    oe.GoldWeight AS 金重,
    oe.GoldPrice AS 金价,
    pr.GoldTypeName AS 成色,
    pr.BuyCategoryName AS 采买品类,
    pr.CategoryName AS 大类,
    pr.CategoryItemName AS 小类,
  CASE
      
      WHEN ISNULL( oe.ReturnMark, 0 ) = 1 THEN
      '是' ELSE '否' 
    END AS 是否退货,
    ROW_NUMBER ( ) OVER ( PARTITION BY oii.OrderInvoiceId ORDER BY oii.OrderInvoiceId ) AS rn 
  FROM
    Client_OrderInvoice oii
    LEFT JOIN Client_OrderInvoiceLink oil ON oil.OrderInvoiceId = oii.OrderInvoiceId
    LEFT JOIN Client_Order oi ON oi.OrderId = oil.OrderId
    LEFT JOIN Client_OrderEntry oe ON oe.OrderId = oi.OrderId
    LEFT JOIN Product_ProductMain pr ON oe.GoodsId = pr.ProductId 
  WHERE
    oii.CreateDate >= '2025-09-01 00:00:00' 
    AND oii.CreateDate < '2025-09-30 23:59:59' 
    AND oi.PaymentCategoryId != 7 
  ) SELECT
  d.开票单号,
  d.申请时间,
  d.订单号,
  d.款号,
  d.金额,
  d.金重,
  d.金价,
  d.成色,
  d.采买品类,
  d.大类,
  d.小类,
  d.是否退货,
CASE
    
    WHEN d.rn = 1 THEN
    it.开票总金额 ELSE NULL 
  END AS 开票总金额 
FROM
  Detail d
  LEFT JOIN InvoiceTotal it ON d.OrderInvoiceId = it.OrderInvoiceId  帮我分析一下这段sql 的是否存在性能问题");

    await foreach (WorkflowEvent evt in run.WatchStreamAsync())
    {
        if (evt is SqlMessageAnalyseEvent @event)
        {
            var cev = @event;
        }

        if(evt is AgentRunUpdateEvent upt)
        {
            logger.LogInformation($"{upt.Update.AuthorName}--{upt.Update.Text}");
        }
    }
});
await app.RunAsync();
