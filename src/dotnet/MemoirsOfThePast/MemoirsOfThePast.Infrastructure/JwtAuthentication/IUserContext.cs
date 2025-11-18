namespace MemoirsOfThePast.Infrastructure.JwtAuthentication
{
    public interface IUserContext
    {
        public string UserName { get;}

        public string UserId { get;}
    }
}
