using MemoirsOfThePast.Infrastructure.JwtAuthentication;
using Microsoft.EntityFrameworkCore;

namespace MemoirsOfThePast.Infrastructure.EntityFrameworkCore
{
    public class AppDbContext: DbContext, IDbContext
    {
        private readonly IUserContext userContext;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">数据库上下文选项</param>
        /// <param name="userContext">用户上下文，用于获取当前用户信息</param>
        public AppDbContext(DbContextOptions<AppDbContext> options,IUserContext userContext) : base(options)
        {
            this.userContext = userContext;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">数据库上下文选项</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        // 重写SaveChangesAsync方法，在保存更改前执行额外操作
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            BeforeSaveChanges();
            return base.SaveChangesAsync(cancellationToken);
        }

        // 重写SaveChangesAsync方法，在保存更改前执行额外操作
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            BeforeSaveChanges();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        // 重写SaveChanges方法，在保存更改前执行额外操作
        public override int SaveChanges()
        {
            BeforeSaveChanges();
            return base.SaveChanges();
        }

        // 重写SaveChanges方法，在保存更改前执行额外操作
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            BeforeSaveChanges();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        // 在保存更改前执行的操作，主要用于设置创建和修改信息
        private void BeforeSaveChanges()
        {
            var changeTracker = ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified);

            foreach (var item in changeTracker)
            {

            }
        }

        /// <summary>
        /// 迁移
        /// </summary>
        public void Migrate()
        {
            Database.Migrate();
        }

        /// <summary>
        /// 执行查询但不跟踪实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>不跟踪的实体查询</returns>
        public IQueryable<T> QueryNoTracking<T>() where T : class
        {
            return Set<T>().AsNoTracking();
        }
    }
}
