using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS_MobileAPI.Core
{
    ///zhong 2018-12-19
    /// <summary>
    /// 请求头验证中间件
    /// </summary>
    public class RequestHeaderMiddleware
    {
        private RequestDelegate next;
        public RequestHeaderMiddleware(RequestDelegate requestDelegate)
        {
            this.next = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            
            var applicationUrl = $"{context.Request.Scheme}://{context.Request.Host.Value}";
            var headersDictionary = context.Request.Headers;
            var urlReferrer = headersDictionary[HeaderNames.Referer].ToString();
            if (string.IsNullOrEmpty(urlReferrer))
            {
                
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new MyHttpResult() { code = 401, data = "", msg = "不允许访问" }), Encoding.UTF8);
                return;
            }
            else if (!urlReferrer.StartsWith(applicationUrl))
            {
                
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new MyHttpResult() { code = 401, data = "", msg = "不允许访问" }), Encoding.UTF8);
                return;
            }
            await next.Invoke(context);
            
        }
    }

    public static class RequestHeaderMiddlewareExtensions
    {
        ///zhong 2018-12-19
        /// <summary>
        /// 请求头验证
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRequestHeaderMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestHeaderMiddleware>();
        }
    }
}
