
using Dapper;
using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS_MobileRepository.Entity;
using TMS_MobileRepository.IRepository;

namespace TMS_MobileAPI.Business
{
    /// <summary>
    /// 2018/12/16 zhong
    /// Email定时业务逻辑接口 
    /// </summary>
    public interface IEmailBusiness
    {
        /// <summary>
        /// 将邮件加入定时队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        Task JoinEmailQueue<T>(IEnumerable<T> obj);
        /// <summary>
        /// 新增邮件日志
        /// </summary>
        /// <param name="b"></param>
        /// <param name="type"></param>
        /// <param name="str"></param>
        void WriteEmailLog(bool b,string type, string str);
    }


    /// <summary>
    /// 2018/12/16 zhong
    /// Email定时业务逻辑
    /// </summary>
    public class EmailBusiness : IEmailBusiness
    {
        private readonly ILogger<EmailBusiness> logger = null;
        private readonly IMissionRepository missionRepository;
        private readonly IMeetingRepository meetingRepository;
        private readonly IEmailRelevance emailRelevance;
        public EmailBusiness(ILogger<EmailBusiness> _logger, IMissionRepository _missionRepository, IEmailRelevance _emailRelevance,
            IMeetingRepository _meetingRepository)
        {
            this.logger = _logger;
            this.missionRepository = _missionRepository;
            this.emailRelevance = _emailRelevance;
            this.meetingRepository = _meetingRepository;
        }
        /// <summary>
        /// 将邮件加入定时队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task JoinEmailQueue<T>(IEnumerable<T> obj)
        {
            Type t = typeof(T);//获得该类的Type
            if (t.Name.Equals("RL_MainItem"))
            {
                RL_MainItem rL_MainItems = ((List<RL_MainItem>)obj)[0];
                var res = await emailRelevance.GetByIdAsync(rL_MainItems.ID);
                if (res != null)
                {
                    if (res.IsPost == 0)
                    {
                        //删除原有的队列
                        BackgroundJob.Delete(res.JobID.ToString());
                        logger.LogInformation("删除任务队列ID：{ID}", res.JobID.ToString());
                    }
                    //删除关联
                    await emailRelevance.DeleteAsync(res.JobID);
                    logger.LogInformation("删除任务队列关联ID：{ID}", res.JobID.ToString());
                }

                DateTime dateTime = DateTime.Now;
                double minutes = new TimeSpan(Convert.ToDateTime(rL_MainItems.EndTime).Ticks-dateTime.Ticks).TotalMinutes;//时间间隔
                if (minutes > 480)
                {
                    DateTime jobTime = rL_MainItems.EndTime.Value.AddMinutes(-480);//离截止时间8小时
                                                                                   //int timeSpan = jobTime.Subtract(dateTime).Minutes;
                    PutMissonJobToQueue(jobTime, rL_MainItems);
                }


            }
            else if (t.Name.Equals("RL_Meeting"))
            {
                List<RL_Meeting> rL_Meetings = (List<RL_Meeting>)obj;
                foreach (var meetingItem in rL_Meetings)
                {
                    var res = await emailRelevance.GetByIdAsync(meetingItem.ID);
                    if (res != null)
                    {
                        if (res.IsPost == 0)
                        {
                            //删除原有的队列
                            BackgroundJob.Delete(res.JobID.ToString());
                            logger.LogInformation("删除会议队列ID：{ID}", res.JobID.ToString());
                        }
                        //删除关联
                        await emailRelevance.DeleteAsync(res.JobID);
                        logger.LogInformation("删除会议队列关联ID：{ID}", res.JobID.ToString());
                    }

                    DateTime dateTime = DateTime.Now;
                    double minutes =new TimeSpan(Convert.ToDateTime(meetingItem.BeginTime).Ticks-dateTime.Ticks).TotalMinutes;//时间间隔
                    if (minutes > 30)
                    {
                        DateTime jobTime = meetingItem.BeginTime.Value.AddMinutes(-30);//离开始时间30分钟

                        PutMettingJobToQueue(jobTime, meetingItem);
                    }
                }
            }


        }
        /// <summary>
        /// 任务队列
        /// </summary>
        /// <param name="JobTime"></param>
        /// <param name="rL_MainItem"></param>
        private void PutMissonJobToQueue(DateTime JobTime, RL_MainItem rL_MainItem)
        {
            //邮件加入队列
            DateTimeOffset dateTimeOffset = new DateTimeOffset(JobTime);
            try
            {
                
                string jobID = BackgroundJob.Schedule<EmailBusiness>((p) => MissonPostEmail(rL_MainItem.ID), dateTimeOffset);
                logger.LogInformation("任务{Title}加入队列ID：{ID}", new string[] { System.Web.HttpUtility.HtmlDecode(rL_MainItem.TaskName), jobID });
                RL_EmailRelevance rL_EmailRelevance = new RL_EmailRelevance();
                rL_EmailRelevance.JobID = int.Parse(jobID);
                rL_EmailRelevance.MissionID = rL_MainItem.ID;
                rL_EmailRelevance.IsPost = 0;
                //邮件队列与任务关联
                emailRelevance.AddEntityAsync(rL_EmailRelevance);

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }
        /// <summary>
        /// 任务定时邮件发送
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public async Task MissonPostEmail(int ID)
        {
            RL_MainItem rL_MainItem = await missionRepository.GetByIdAsync(ID);
            if (rL_MainItem != null)
            {
                if (rL_MainItem.status.Equals("未完成"))
                {
                    string taskName = System.Web.HttpUtility.HtmlDecode(rL_MainItem.TaskName);
                    DateTime dateTime = DateTime.Now;
                    double minutes =new TimeSpan(Convert.ToDateTime(rL_MainItem.EndTime).Ticks-dateTime.Ticks).TotalMinutes;//时间间隔
                    if (minutes <= 480&&minutes>0)
                    {
                        //邮件接口调用
                        var postBool = await missionRepository.EmailpostAsync(rL_MainItem.Email, rL_MainItem, "EndMainItem");
                        var res = await emailRelevance.GetByIdAsync(ID);
                        if (postBool)
                        {
                            if (res != null)
                            {
                                res.IsPost = 1;
                                await emailRelevance.UpdateAsync(res);
                            }
                            logger.LogInformation("任务{Title}定时提醒邮件发送成功", taskName);
                        }
                        else
                        {
                            logger.LogError("任务{Title}定时提醒邮件发送失败", taskName);
                        }


                    }
                    else
                    {
                        logger.LogWarning("任务{Title}定时提醒邮件取消发送", taskName);

                    }

                }
            }

        }
        /// <summary>
        /// 会议队列
        /// </summary>
        /// <param name="JobTime"></param>
        /// <param name="rL_Meeting"></param>
        private void PutMettingJobToQueue(DateTime JobTime, RL_Meeting rL_Meeting)
        {
            //邮件加入队列
            DateTimeOffset dateTimeOffset = new DateTimeOffset(JobTime);
            try
            {
                
                string jobID = BackgroundJob.Schedule<EmailBusiness>((p) => MettingPostEmail(rL_Meeting.ID), dateTimeOffset);
                logger.LogInformation("会议{Title}加入队列ID：{ID}", new string[] { System.Web.HttpUtility.HtmlDecode(rL_Meeting.MeetingTitle), jobID });
                RL_EmailRelevance rL_EmailRelevance = new RL_EmailRelevance();
                rL_EmailRelevance.JobID = int.Parse(jobID);
                rL_EmailRelevance.MettingID = rL_Meeting.ID;
                rL_EmailRelevance.IsPost = 0;
                //邮件队列与任务关联
                emailRelevance.AddEntityAsync(rL_EmailRelevance);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }
        /// <summary>
        /// 会议定时邮件发送
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public async Task MettingPostEmail(int ID)
        {
            RL_Meeting rL_Meeting = await meetingRepository.GetByIdAsync(ID);
            if (rL_Meeting != null)
            {
                if (rL_Meeting.Status.Equals("待开始"))
                {
                    string mettingTitle = System.Web.HttpUtility.HtmlDecode(rL_Meeting.MeetingTitle);
                    DateTime dateTime = DateTime.Now;
                    double minutes =new TimeSpan(Convert.ToDateTime(rL_Meeting.BeginTime).Ticks-dateTime.Ticks).TotalMinutes;//时间间隔
                    if (minutes <= 30&&minutes>0)
                    {
                        //邮件接口调用
                        Emailparameter emailparameter = new Emailparameter();
                        emailparameter.Meeting = rL_Meeting;
                        var postBool = await meetingRepository.EmailpostAsync( emailparameter, "FrontMeeting");
                        var res = await emailRelevance.GetByIdAsync(ID);
                        if (postBool)
                        {
                            if (res != null)
                            {
                                res.IsPost = 1;
                                await emailRelevance.UpdateAsync(res);
                            }
                            logger.LogInformation("会议{Title}定时提醒邮件发送成功", mettingTitle);
                        }
                        else
                        {
                            logger.LogError("会议{Title}定时提醒邮件发送失败", mettingTitle);
                        }


                    }
                    else
                    {
                        logger.LogWarning("会议{Title}定时提醒邮件取消发送", mettingTitle);
                    }

                }
            }

        }
        /// <summary>
        /// 新增邮件日志
        /// </summary>
        /// <param name="b"></param>
        /// <param name="type"></param>
        /// <param name="str"></param>
        public void WriteEmailLog(bool b,string type, string str)
        {
            if (!b)
            {
                string typeStr = type == "Main" ? "任务" : "会议";
                logger.LogError("新建{Type}{TaskName}邮件发送失败",new string[] { typeStr,System.Web.HttpUtility.HtmlDecode(str) });
            }
        }
    }
}
