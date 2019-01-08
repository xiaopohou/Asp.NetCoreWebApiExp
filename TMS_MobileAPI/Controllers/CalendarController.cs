using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TMS_MobileAPI.Core;
using TMS_MobileRepository.Entity;
using TMS_MobileRepository.IRepository;

namespace TMS_MobileAPI.Controllers
{
    /// <summary>
    /// 2018/12/15 zhong
    /// 日历
    /// </summary>
    //[ValidateAntiForgeryToken]
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [EnableCors("Mobile")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarRepository calendarRepository;
        public CalendarController(ICalendarRepository _calendarRepository)
        {
            this.calendarRepository = _calendarRepository;
        }
        /// <summary>
        /// 获取日历数据
        /// 返回结果HaveMain表示当天是否有任务，HaveMeet表示当天是否有会议，MainData为当天任务的数据，
        /// MeetData为当天会议的数据。
        /// </summary>
        /// <param name="YearMonth">年月(YYYY-MM，2018-12)</param>
        /// <param name="UserCode">用户UserCode</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetCalendarDataAsync(GetCalendarDataAsync entity)
        {
            DateTime date;
            if(!DateTime.TryParse(entity.YearMonth+"+01",out date)){
                return BadRequest("The Parameter YearMonth Cannot Be Converted To Time ");
            }
            //路由参数为path得替换
            string UserCodeDecrypt = RsaCrypto.Decrypt(entity.UserCode.Replace("%2F", "/"));
            var res= await calendarRepository.GetRL_CalendarResAsync(entity.YearMonth, UserCodeDecrypt);
            return Ok(res);
        }
    }
}