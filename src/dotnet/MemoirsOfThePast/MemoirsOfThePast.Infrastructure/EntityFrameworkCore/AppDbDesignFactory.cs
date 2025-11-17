using MemoirsOfThePast.Infrastructure.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace SpeakEase.Gateway.Infrastructure.EntityFrameworkCore
{
    internal class AppDbDesignFactory: IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "MemoirsOfThePast.Host");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                // 从 appsettings.json 文件加载配置
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                // 也可以添加其他配置源，例如环境变量
                .AddEnvironmentVariables()
                // 或者命令行参数
                //.AddCommandLine(args)
                .Build();

            var connection = "User ID=postgres;Password=wyg154511;Host=117.72.66.170;Port=5432;Database=memoryofthepast;Pooling=true;";

            var options = new OptionsWrapper<DbContextOptions<AppDbContext>>(new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connection)
                .Options);

            return new AppDbContext(options.Value);
        }
    }
}
