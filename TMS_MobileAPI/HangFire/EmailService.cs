using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS_MobileAPI.HangFire
{
    /// <summary>
    /// 2018/12/15 zhong
    /// 邮件定时任务测试
    /// </summary>
    public interface IEmailService
    {
        void Test();
    }
    /// <summary>
    /// 邮件服务测试
    /// </summary>
    public class EmailService:IEmailService
    {
        
        private readonly ILogger<EmailService> _logger;
           
        public EmailService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<EmailService>();
           
        }
        /// <summary>
        /// 测试
        /// </summary>
        public void Test()
        {
            _logger.LogInformation($"check service start checking, now is {DateTime.Now}");
            BackgroundJob.Schedule(() => TestLog(), TimeSpan.FromMilliseconds(30));
            _logger.LogInformation($"check is end, now is {DateTime.Now}");
        }
        public void TestLog()
        {
            _logger.LogInformation("HangFire Test()");
        }
    }

        
    
}
