using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMS_MobileRepository.Entity;

namespace TMS_MobileRepository.IRepository
{
    public interface ICalendarRepository
    {
        Task<IEnumerable<RL_Calendar>> GetRL_CalendarResAsync(string YearMonth, string UserCode);
    }
}
