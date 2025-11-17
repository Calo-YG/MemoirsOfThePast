namespace MemoirsOfThePast.Infrastructure.JwtAuthentication
{
    internal class JwtBearerException:Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public JwtBearerException(string message) : base(message)
        {

        }

        public JwtBearerException(string message, Exception ex) : base(message, ex)
        {

        }
    }
}
