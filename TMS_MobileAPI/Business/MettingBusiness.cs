using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS_MobileRepository.Entity;

namespace TMS_MobileAPI.Business
{
    /// <summary>
    /// 2018/12/13 zhong
    /// 会议业务逻辑
    /// </summary>
    public class MettingBusiness
    {
        /// <summary>
        /// 生成多条
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RL_Meeting> GetEntityList(RL_Meeting rL_Meeting)
        {
            List<RL_Meeting> rL_Meetings = new List<RL_Meeting>();
            string guid = Guid.NewGuid().ToString();
            var dicList = GetWeeks(Convert.ToDateTime(rL_Meeting.BeginTime),Convert.ToDateTime(rL_Meeting.EndTime)
                , rL_Meeting.Frequency, GetOfWeek(rL_Meeting.Week));
            foreach (var item in dicList)
            {
                RL_Meeting Meetings = new RL_Meeting();
                Meetings.MeetingGuid = guid;
                Meetings.MeetingTitle =rL_Meeting.MeetingTitle;
                Meetings.MeetingType = rL_Meeting.MeetingType;
                Meetings.Frequency = rL_Meeting.Frequency;
                Meetings.Week = rL_Meeting.Week;
                Meetings.BeginTime = item.Key;
                Meetings.EndTime = item.Value;
                Meetings.Place = rL_Meeting.Place;
                Meetings.Participants = rL_Meeting.Participants;
                Meetings.CreateDate = rL_Meeting.CreateDate;
                Meetings.CreateBy = rL_Meeting.CreateBy;
                Meetings.Status = rL_Meeting.Status;
                Meetings.Email = rL_Meeting.Email;
                Meetings.ParticipantsCode = rL_Meeting.ParticipantsCode;


                rL_Meetings.Add(Meetings);
            }
            return rL_Meetings;
        }
       /// <summary>
       /// 根据频率算出会议时间
       /// </summary>
       /// <param name="AStart"></param>
       /// <param name="AEnd"></param>
       /// <param name="frequency"></param>
       /// <param name="AWeek"></param>
       /// <returns></returns>
        private  Dictionary<DateTime,DateTime> GetWeeks(DateTime AStart, DateTime AEnd,string frequency, DayOfWeek AWeek)
        {
            int dividend = 0;
            if (frequency == "每周")
            {
                dividend = 7;
            }
            else if (frequency == "每双周")
            {
                dividend = 14;
            }
            else
            {
                dividend = 28;
            }
            Dictionary<DateTime, DateTime> dic = new Dictionary<DateTime, DateTime>();
            DateTime todayTime = DateTime.Now;//当前时间
            DateTime startWeek = todayTime.AddDays(1 - Convert.ToInt32(todayTime.DayOfWeek.ToString("d")));  //本周周一
            DateTime endWeek = startWeek.AddDays(6);  //本周周日
            TimeSpan vTimeSpanTotal = new TimeSpan(AEnd.Ticks - todayTime.Ticks);//所有天数
            TimeSpan vTimeSpan;

            DateTime startExcuteTime = Convert.ToDateTime("1900-01-01");//开始执行时间
            DateTime endExcuteTime = Convert.ToDateTime("1900-01-01");//结束执行时间


            if (AEnd < endWeek)
            {
                vTimeSpan = new TimeSpan(AEnd.Ticks - todayTime.Ticks);//
            }
            else
            {
                vTimeSpan = new TimeSpan(endWeek.Ticks - todayTime.Ticks);//
            }
            for (int i = 0; i < vTimeSpan.TotalDays; i++)
            {
                if (AStart.AddDays(i).DayOfWeek == AWeek)
                {
                    startExcuteTime = AStart.AddDays(i);//开始执行时间
                    endExcuteTime = Convert.ToDateTime(startExcuteTime.ToString("yyyy-MM-dd") + " " + AEnd.ToString("HH:mm:ss"));
                    dic.Add(startExcuteTime, endExcuteTime);
                }
            }
            if (startExcuteTime == Convert.ToDateTime("1900-01-01"))
            {
                DateTime afterStartWeek = AStart.AddDays(Convert.ToInt32(1 - Convert.ToInt32(AStart.DayOfWeek)) + 7);        //下周一
                DateTime afterEndWeek = AStart.AddDays(Convert.ToInt32(1 - Convert.ToInt32(AStart.DayOfWeek)) + 7).AddDays(6);      //下周末
                if (afterEndWeek > AEnd && afterStartWeek <= AEnd)
                {
                    vTimeSpan = new TimeSpan(AEnd.Ticks - afterStartWeek.Ticks);//
                }
                else if(AEnd< afterStartWeek)
                {
                    vTimeSpan = new TimeSpan(0);//
                }
                else
                {
                    vTimeSpan = new TimeSpan(afterEndWeek.Ticks - afterStartWeek.Ticks);//
                }
                for (int i = 0; i < vTimeSpan.TotalDays; i++)
                {
                    if (afterStartWeek.AddDays(i).DayOfWeek == AWeek)
                    {
                        startExcuteTime = afterStartWeek.AddDays(i);//开始执行时间
                        endExcuteTime = Convert.ToDateTime(startExcuteTime.ToString("yyyy-MM-dd") + " " + AEnd.ToString("HH:mm:ss"));
                        dic.Add(startExcuteTime, endExcuteTime);
                    }
                }
            }
            if (startExcuteTime != Convert.ToDateTime("1900-01-01")) {
                for (int i = 0; i < Math.Ceiling(vTimeSpanTotal.TotalDays / dividend); i++)
                {
                    if (startExcuteTime.AddDays(dividend * (1 + i)).DayOfWeek == AWeek && startExcuteTime.AddDays(dividend * (1 + i)) <= AEnd)
                    {
                        dic.Add(startExcuteTime.AddDays(dividend * (1 + i)), endExcuteTime.AddDays(dividend * (1 + i)));
                    }
                }
            }
        


            return dic;
        }  
        /// <summary>
        /// 周几转换为dayofweek
        /// </summary>
        /// <param name="week"></param>
        /// <returns></returns>
        protected DayOfWeek GetOfWeek(string week)
        {
            DayOfWeek dayWeek;
            switch (week)
            {
                case "周一":dayWeek = DayOfWeek.Monday;
                    break;
                case "周二":
                    dayWeek = DayOfWeek.Tuesday;
                    break;
                case "周三":
                    dayWeek = DayOfWeek.Wednesday;
                    break;
                case "周四":
                    dayWeek = DayOfWeek.Saturday;
                    break;
                case "周五":
                    dayWeek = DayOfWeek.Friday;
                    break;
                default:
                    dayWeek = DayOfWeek.Sunday;
                    break;
            }
            return dayWeek;
        }
        /// <summary>
        /// 将ID加入Metting列中
        /// </summary>
        /// <param name="IdList"></param>
        /// <param name="rL_Meetings"></param>
        /// <returns></returns>
        public IEnumerable<RL_Meeting> AddIDToMettingList(IEnumerable<int> IdList,IEnumerable<RL_Meeting> rL_Meetings)
        {
            List<int> listId = IdList.ToList();
            List<RL_Meeting> l_Meetings = rL_Meetings.ToList();
            for (int i = 0; i < listId.Count; i++)
            {
                l_Meetings[i].ID = listId[i];
            }
            return l_Meetings;
        }

    }
}
