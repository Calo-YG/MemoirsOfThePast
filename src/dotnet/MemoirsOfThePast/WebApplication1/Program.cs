using MemoirsOfThePast.Infrastructure.Core;
using MemoirsOfThePast.Infrastructure.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DateTimeConverter = MemoirsOfThePast.Infrastructure.Core.DateTimeConverter;

var builder = WebApplication.CreateSlimBuilder(args);

var configuration = builder.Configuration;

var cors = "MemoirsOfThePast"; ;

builder.AddServiceDefaults();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

#region 注册ef core
// 设置AppContext开关，以启用Npgsql的遗留时间戳行为
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// 设置AppContext开关，以禁用DateTime的无穷大转换
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

// 添加DbContext到服务集合中，指定为Scoped生命周期
builder.Services.AddDbContext<IDbContext, AppDbContext>((builder) =>
{
    // 使用Npgsql作为数据库提供程序，并从配置中获取连接字符串
    builder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), options =>
    {
        // 此处可根据需要配置Npgsql选项
    });
}, contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Scoped);
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
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConn");
    options.InstanceName = "SpeakEase";
});
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
var app = builder.Build();

app.UseCors(cors);

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

await app.RunAsync();