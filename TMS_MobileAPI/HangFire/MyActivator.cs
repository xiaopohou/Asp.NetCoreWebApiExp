using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS_MobileAPI.HangFire
{
    /// <summary>
    /// 2018/12/14 zhong
    /// 定时任务启动器
    /// </summary>
    public class MyActivator: JobActivator
    {
        private readonly IServiceProvider _serviceProvider;
        public MyActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override object ActivateJob(Type jobType)
        {
            return _serviceProvider.GetService(jobType);
        }
    }
}
