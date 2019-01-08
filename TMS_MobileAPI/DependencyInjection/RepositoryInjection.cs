using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS_MobileAPI.Business;
using TMS_MobileAPI.HangFire;
using TMS_MobileRepository.IRepository;
using TMS_MobileRepository.Repository;

namespace TMS_MobileAPI.DependencyInjection
{
    /// <summary>
    /// 2018/12/12 zhong
    /// 简单DDD模式仓储注入 
    /// tips：增加的其他模型仓储在方法里注入
    /// </summary>
    public class RepositoryInjection
    {
        public static void ConfigureRepository(IServiceCollection services)
        {
            
            services.AddSingleton<IUserRepository, UserRepository>();//人员测试用
            services.AddSingleton<IMissionRepository, MissonRepository>();
            services.AddSingleton<IMeetingRepository, MeetingRepository>();
            services.AddSingleton<ITeamRepository, TeamRepository>();
            services.AddSingleton<ICalendarRepository, CalendarRepository>();
            services.AddSingleton<IEmailRelevance, EmailRelevance>();
            //HangFire注入
            services.AddScoped<IEmailService, EmailService>();
            //邮件定时
            services.AddSingleton<IEmailBusiness, EmailBusiness>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
        }
    }
}
