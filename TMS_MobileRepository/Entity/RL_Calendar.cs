using System;
using System.Collections.Generic;
using System.Text;

namespace TMS_MobileRepository.Entity
{

    //public class RL_CalendarRes
    //{
    //    public IEnumerable<RL_Calendar> CalendarResData { get; set; }
    //}
    public class RL_Calendar
    {
        public RL_Calendar()
        {
            CalendarData = new RL_CalendarData();
        }
        public string Day { get; set; }
        public bool HaveMain { get; set; }
        public bool HaveMeet { get; set; }
        public RL_CalendarData CalendarData { get; set; }
    }

    public class RL_CalendarData
    {
        public RL_CalendarData()
        {
            MainData = new List<MainItemData>();
            MeetData = new List<MeettingData>();
        }
        public IEnumerable<MainItemData> MainData { get; set; }
        public IEnumerable<MeettingData> MeetData { get; set; }
    }

    public class MainItemData
    {
        public int ID { get; set; }
        public string TaskName { get; set; }
        public string MissionName { get; set; }
        public DateTime? EndTime { get; set; }
    }
    public class MeettingData
    {
        public int ID { get; set; }
        public string MeetingTitle { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
