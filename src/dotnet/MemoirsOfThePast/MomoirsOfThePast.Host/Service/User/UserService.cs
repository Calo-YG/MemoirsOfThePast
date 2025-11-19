using MemoirsOfThePast.HoST.Service.User.Dto;
using MemoirsOfThePast.Infrastructure.Const;
using MemoirsOfThePast.Infrastructure.Domain;
using MemoirsOfThePast.Infrastructure.EntityFrameworkCore;
using MemoirsOfThePast.Infrastructure.Exceptions;
using MemoirsOfThePast.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace MemoirsOfThePast.HoST.Service.User
{
    public class UserService(IDbContext dbContext):IUserService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task RegisterAsync(RegisterInput input)
        {
            if(string.IsNullOrEmpty(input.Account) || string.IsNullOrEmpty(input.Password))
            {
                throw new BusinessException("请输入账号密码");
            }

            var any = await dbContext.Users.AsNoTracking().AnyAsync(p=>p.Account == input.Account);

            if (any)
            {
                throw new BusinessException("用户已存在");
            }

            var entity = new UserEntity
            {
                Id = Guid.NewGuid().ToString("N"),
                CreateDate = DateTime.Now,
                Account = input.Account,
                Name = input.Account,
                Avatar = "https://avatars.githubusercontent.com/u/74019004?s=400&u=bf9fc0cb7908138aed27fdd71cce648f29b624f5&v=4",
                Solt = Guid.NewGuid().ToString("N"),
            };

            var password = AesUtil.Encrypt(input.Password, entity.Solt, EncryptConst.AESIV);
            entity.SetPassword(password);

            await dbContext.Users.AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }
    }
}
