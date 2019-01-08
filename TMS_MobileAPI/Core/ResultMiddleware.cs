using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS_MobileAPI.Core
{
    /// <summary>
    /// 2018/12/11 zhong
    /// 格式化返回
    /// </summary>
    public class ResultMiddleware: ActionFilterAttribute
    {
        private readonly ILogger<ResultMiddleware> _logger = null;
        public ResultMiddleware(ILogger<ResultMiddleware> logger)
        {
            this._logger = logger;
        }
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult)
            {
                var objectResult = context.Result as ObjectResult;
                if (objectResult.Value == null)
                {
                    //_logger.LogInformation("未找到资源");
                    context.Result = new ObjectResult(new { code = 404, msg = "未找到资源", data = "" });
                }
                else
                {
                    if (context.Result is BadRequestObjectResult)
                    {
                        context.Result = new ObjectResult(new { code = 400, msg = "", data = objectResult.Value });
                    }
                    else if (context.Result is UnsupportedMediaTypeResult)
                    {
                        context.Result = new ObjectResult(new { code = 415, msg = "", data = objectResult.Value });

                    }else if(context.Result is NotFoundObjectResult)
                    {
                        context.Result = new ObjectResult(new { code = 200, msg = "", data = new object[] { } });
                    }
                    else
                    {
                        context.Result = new ObjectResult(new { code = 200, msg = "", data = objectResult.Value });
                    }
                }
            }
            else if (context.Result is EmptyResult)
            {
                context.Result = new ObjectResult(new { code = 404, msg = "未找到资源", data = "" });
            }
            else if (context.Result is ContentResult)
            {
                context.Result = new ObjectResult(new { code = 200, msg = "", data = (context.Result as ContentResult).Content });
            }
            else if (context.Result is StatusCodeResult)
            {
                context.Result = new ObjectResult(new { code = (context.Result as StatusCodeResult).StatusCode, msg = "", data = "" });
            }
        }
    }
}
