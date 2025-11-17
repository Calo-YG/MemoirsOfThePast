namespace MemoirsOfThePast.Infrastructure.Exceptions
{
    internal class BusinessException:Exception
    {
        public int Cod { get;private set; }

        public BusinessException()
        {
        }

        public BusinessException(string message) : base(message)
        {
        }

        public BusinessException(int code,string message) : base(message)
        {
        }
    }
}
