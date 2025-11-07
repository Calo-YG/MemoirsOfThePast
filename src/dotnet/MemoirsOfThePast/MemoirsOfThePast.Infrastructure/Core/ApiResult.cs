namespace MemoirsOfThePast.Infrastructure.Core
{
    [Serializable]
    public class ApiResultBase
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; }
    }

    [Serializable]
    public class ApiResult<TResult> : ApiResultBase
    {
        public TResult Data { get; set; }

        public ApiResult(TResult result)
        {
            Data = result;
            Code = 200;
        }

        public ApiResult(string errorMessage = "", int code = 500)
        {
            Message = errorMessage;
            Code = code;
        }
    }

    [Serializable]
    public class ApiResult : ApiResult<object>
    {
        public ApiResult(object result) : base(result)
        {
        }

        public ApiResult(string errorMessage, int code = 500) : base(errorMessage, code)
        {
        }

        public ApiResult() : base()
        {
        }

        public static ApiResult Success(object result)
        {
            return new ApiResult(result);
        }

        public static ApiResult Fail(string errorMessage, int errorCode)
        {
            return new ApiResult(errorMessage, errorCode);
        }
    }
}
