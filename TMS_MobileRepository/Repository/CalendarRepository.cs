using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using TMS_MobileRepository.Entity;
using TMS_MobileRepository.IRepository;

namespace TMS_MobileRepository.Repository
{
    public class CalendarRepository : ICalendarRepository
    {
        /// <summary>
        /// 获取日历数据
        /// </summary>
        /// <param name="YearMonth"></param>
        /// <param name="UserCode"></param>
        /// <returns></returns>
        public async Task<IEnumerable<RL_Calendar>> GetRL_CalendarResAsync(string YearMonth, string UserCode)
        {
            YearMonth = YearMonth + "-01";
            using (IDbConnection conn = DataBaseConfig.GetSqlConnection())
            {
                string sql = @"select day as Day,case when Main is null then 'false' else 'true' end as HaveMain,
case when Meet is null then 'false' else 'true' end as HaveMeet from (
select *   from(
select day,'Main' as Type,1 as iscount from (select CreateDate,EndTime ,day from RL_MainItem 
left join ( 
select convert(varchar(10),dateadd(DAY,t2.number,t1.day),120) day from 
(select substring(convert(varchar,@YearMonth,120),1,7)+'-01' day) t1, 
(select number from MASTER..spt_values WHERE TYPE='P' AND number>=0 and number<=31) t2 
where convert(varchar(10),dateadd(DAY,t2.number,t1.day),120) like substring(convert(varchar,@YearMonth,120),1,7)+'%')
as days on 1=1 where  (CreateBy=@UserCode or UserCode=@UserCode) 
and CONVERT(varchar(10),EndTime,120)>=days.day) as a where CONVERT(varchar(10),CreateDate,120)<=day
union
select day,'Meet' as Type,1 as iscount from (
select convert(varchar(10),dateadd(DAY,t2.number,t1.day),120) day from 
(select substring(convert(varchar,@YearMonth,120),1,7)+'-01' day) t1, 
(select number from MASTER..spt_values WHERE TYPE='P' AND number>=0 and number<=31) t2 
where convert(varchar(10),dateadd(DAY,t2.number,t1.day),120) like substring(convert(varchar,@YearMonth,120),1,7)+'%')
as a left join (
select * from (select  CHARINDEX(@UserCode+',',ParticipantsCode+',') as include,* from RL_Meeting
) as sel where include>=1 or CreateBy=@UserCode) as b on 1=1 where CONVERT(varchar(10),BeginTime,120)=day and Status!='取消'
group by day) as c pivot(sum(iscount) for c.Type in([Main],[Meet])) as d) as e";
                var res= await conn.QueryAsync<RL_Calendar>(sql, new { YearMonth = YearMonth, UserCode = UserCode });
                if (res.AsList().Count > 0)
                {
                    int resCount = res.AsList().Count;
                    for (int i = 0; i < resCount; i++)
                    {
                        string date = res.AsList()[i].Day;
                        string dataSql = @"select ID, TaskName,MissionName,EndTime from RL_MainItem where (CreateBy=@UserCode or UserCode=@UserCode)
and CONVERT(varchar(10),EndTime,120)>=@YearMonth and CONVERT(varchar(10),CreateDate,120)<=@YearMonth;
select ID, MeetingTitle,BeginTime,EndTime from (select  CHARINDEX(@UserCode+',',ParticipantsCode+',') as include,* from RL_Meeting
where CONVERT(varchar(10),BeginTime,120)=@YearMonth) as sel where Status!='取消' and  (include>=1 or CreateBy=@UserCode)";
                        var dataRes=conn.QueryMultiple(dataSql,new { YearMonth = date, UserCode = UserCode });
                        var mainItemData =await dataRes.ReadAsync<MainItemData>();
                        var meettingData =await dataRes.ReadAsync<MeettingData>();
                        res.AsList()[i].CalendarData.MainData = mainItemData;
                        res.AsList()[i].CalendarData.MeetData = meettingData;
                        if (i == resCount - 1)
                        {
                            dataRes.Dispose();
                        }
                    }
                   
                }
                return res;

            }
        }
    }
}
