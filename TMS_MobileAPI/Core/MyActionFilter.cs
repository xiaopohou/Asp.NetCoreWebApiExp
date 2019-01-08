using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS_MobileAPI.Core
{
    /// <summary>
    /// 2018/12/11 zhong
    /// 自定义过滤器
    /// </summary>
    public class MyActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                MyHttpResult result = new MyHttpResult { code = 103 };
                foreach (var item in context.ModelState.Values)
                {
                    foreach (var err in item.Errors)
                    {
                        result.msg += err.ErrorMessage + "|";

                    }
                }
                context.Result = new JsonResult(result);
            }
            
        }
    }
}
