using Hangfire.Annotations;
using Hangfire.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS_MobileAPI.HangFire
{
    ///2018-12-18 zhong
    /// <summary>
    /// 自定义hangfire认证
    /// </summary>
    public class HangFireAuthorizeFilter : IDashboardAuthorizationFilter
    {
        
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}
