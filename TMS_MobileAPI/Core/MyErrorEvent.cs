using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NLog;
using System.Text;
using System.Threading.Tasks;

namespace TMS_MobileAPI.Core
{
    public static class MyErrorEvent
    {
        public static Task ErrorEvent(HttpContext context)
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();
            var error = feature?.Error;
            LogManager.GetCurrentClassLogger().Error(error.Message,error.StackTrace);
            return context.Response.WriteAsync(JsonConvert.SerializeObject(new MyHttpResult() { code=500,data="",msg="出现未知错误，请联系管理员"}), Encoding.UTF8);
            
        }
    }
}
