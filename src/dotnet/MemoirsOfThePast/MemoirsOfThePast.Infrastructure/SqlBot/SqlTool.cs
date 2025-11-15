using MemoirsOfThePast.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace MemoirsOfThePast.Infrastructure.SqlBot
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public class SqlTool(IOptionsSnapshot<DbOptions> options):ISqlTool
    {
       
    }
}
