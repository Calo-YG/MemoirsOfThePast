using MemoirsOfThePast.Infrastructure.Domain;
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
            #region memory
            modelBuilder.Entity<MemoryEntity>(op =>
            {
                op.ToTable("memory");
                op.HasKey(p=>p.Id).HasName("id");
                op.Property(p => p.Name).IsRequired().HasColumnName("name");
                op.Property(p=>p.Description).IsRequired().HasColumnName("description");
                op.Property(p => p.Background).IsRequired().HasColumnName("background");
                op.Property(p => p.Prompt).HasColumnName("prompt");
                op.Property(p => p.CreateDate).IsRequired().HasColumnName("create_date");
            });
            #endregion
            #region fragment
            modelBuilder.Entity<FragmentEntity>(op =>
            {
                op.ToTable("fragment");
                op.HasKey(p => p.Id).HasName("id");
                op.Property(p => p.MemoryId).IsRequired().HasColumnName("memory_id");
                op.Property(p => p.Description).IsRequired().HasColumnName("description");
                op.Property(op => op.OccurDate).IsRequired().HasColumnName("occur_date");
                op.Property(op => op.Location).IsRequired().HasColumnName("location");
                op.Property(p => p.Scene).HasColumnName("secne");
                op.Property(p => p.CreateDate).IsRequired().HasColumnName("create_date");
            });
            #endregion
            #region users
            modelBuilder.Entity<UserEntity>(op =>
            {
                op.ToTable("users");
                op.HasKey(p => p.Id).HasName("id");
                op.Property(p => p.Name).IsRequired().HasColumnName("name");
                op.Property(p => p.Avatar).IsRequired().HasColumnName("avatar");
                op.Property(p => p.Account).IsRequired().HasColumnName("account");
                op.Property(p => p.Password).IsRequired().HasColumnName("password");
                op.Property(p => p.CreateDate).IsRequired().HasColumnName("create_date");
            });
            #endregion
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
