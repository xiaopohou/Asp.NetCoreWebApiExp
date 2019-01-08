using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using TMS_MobileRepository.Entity;
using TMS_MobileRepository.IRepository;
using Dapper;
using System.Net.Mail;
using System.Net;

namespace TMS_MobileRepository.Repository
{
    /// <summary>
    /// 会议数据获取
    /// </summary>
    public class MeetingRepository : IMeetingRepository
    {
        /// <summary>
        /// 会议新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> AddEntityAsync(RL_Meeting entity)
        {
            string guid = Guid.NewGuid().ToString();
            entity.MeetingGuid = guid;
            entity.Frequency = "";
            entity.Week = "";
            using (IDbConnection conn = DataBaseConfig.GetSqlConnection())
            {
                string querySql = @"insert into RL_Meeting values(@MeetingGuid,@MeetingTitle,@MeetingType,@Frequency,@Week,
                                 @BeginTime,@EndTime,@Place,@Participants,@CreateDate,@CreateBy,@Status,@Email,@ParticipantsCode,@Meetingcontent)";
                var res = await conn.ExecuteAsync(querySql, entity);
                if (res > 0)
                {
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 会议新增，返回主键
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> CreateEntityAsync(RL_Meeting entity)
        {
            string guid = Guid.NewGuid().ToString();
            entity.MeetingGuid = guid;
            entity.Frequency = "";
            entity.Week = "";
            using (IDbConnection conn = DataBaseConfig.GetSqlConnection())
            {
                string querySql = @"insert into RL_Meeting values(@MeetingGuid,@MeetingTitle,@MeetingType,@Frequency,@Week,
                                 @BeginTime,@EndTime,@Place,@Participants,@CreateDate,@CreateBy,@Status,@Email,@ParticipantsCode,@Meetingcontent);
                                 SELECT CAST(SCOPE_IDENTITY() as int)";
                var res = await conn.QueryFirstOrDefaultAsync<int>(querySql, entity);
                return res;
            }
        }

        /// <summary>
        /// 会议新增多条
        /// </summary>
        /// <param name="entityList"></param>
        /// <returns></returns>
        public async Task<bool> AddEntityListAsync(IEnumerable<RL_Meeting> entityList)
        {
            using (IDbConnection conn = DataBaseConfig.GetSqlConnection())
            {
                string querySql = @"insert into RL_Meeting values(@MeetingGuid,@MeetingTitle,@MeetingType,@Frequency,@Week,
                                 @BeginTime,@EndTime,@Place,@Participants,@CreateDate,@CreateBy,@Status,@Email,@ParticipantsCode,@Meetingcontent)";
                var res = await conn.ExecuteAsync(querySql, entityList);
                if (res > 0)
                {
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 会议新增多条返回主键列
        /// </summary>
        /// <param name="entityList"></param>
        /// <returns></returns>
        public async Task<IEnumerable<int>> CreateEntityListAsync(IEnumerable<RL_Meeting> entityList)
        {
            List<RL_Meeting> rL_Meetings = entityList.AsList();
            List<int> listId = new List<int>();
            using (IDbConnection conn = DataBaseConfig.GetSqlConnection())
            {
                string querySql = @"insert into RL_Meeting values(@MeetingGuid,@MeetingTitle,@MeetingType,@Frequency,@Week,
                                 @BeginTime,@EndTime,@Place,@Participants,@CreateDate,@CreateBy,@Status,@Email,@ParticipantsCode,@Meetingcontent);
                                 SELECT CAST(SCOPE_IDENTITY() as int)";
                foreach (var item in rL_Meetings)
                {
                    var res = await conn.QueryFirstOrDefaultAsync<int>(querySql, item);
                    listId.Add(res);
                }
                
                return listId;
            }
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RL_Meeting>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据ID获取会议详情与修改当前会议获取详情
        /// </summary>
        /// <param name="id">会议主键ID</param>
        /// <returns></returns>
        public  async Task<RL_Meeting> GetByIdAsync(int id)
        {
            using (IDbConnection con = DataBaseConfig.GetSqlConnection())
            {
                string sql = "select * from RL_Meeting where ID=@id";
                return await con.QueryFirstOrDefaultAsync<RL_Meeting>(sql, new { id = id });
            }
        }
        /// <summary>
        /// 修改单条会议记录信息
        /// </summary>
        /// <param name="entity">会议实体</param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(RL_Meeting entity)
        {
            using (IDbConnection con = DataBaseConfig.GetSqlConnection())
            {
                string sql = @"UPDATE [RL_Meeting]
                               SET [MeetingTitle] =@MeetingTitle
                                  ,[MeetingType] = @MeetingType
                                  ,[Frequency] = @Frequency
                                  ,[Week] = @Week
                                  ,[BeginTime] = @BeginTime
                                  ,[EndTime] =@EndTime
                                  ,[Place] = @Place
                                  ,[Participants] = @Participants
                                  ,[Status] = @Status
                                  ,[Meetingcontent]=@Meetingcontent
                             WHERE ID =@ID";
                var rel = await con.ExecuteAsync(sql, entity);
                if (rel > 0)
                {
                    return true;
                }
                return false;

            }
        }


        /// <summary>
        /// 会议修改，根据MeetingGuid获取修改页面数据详情（修改整个循环）
        /// </summary>
        /// <param name="MeetingGuid">GUid</param>
        /// <returns></returns>
        public async Task<RL_Meeting> GetMeetinUpdateDetail(string MeetingGuid)
        {
            using (IDbConnection con = DataBaseConfig.GetSqlConnection())
            {
                string sql = @"select MeetingGuid,MeetingTitle,MeetingType,Frequency,Week,MIN(BeginTime)as BeginTime,MAX(EndTime) as EndTime,Place,Participants,CreateBy,CreateDate,Status,Email from RL_Meeting where MeetingGuid=@MeetingGuid
group by MeetingGuid,MeetingTitle,MeetingType,Frequency,Week,Place,Participants,CreateBy,CreateDate,Status,Email";
                return await con.QueryFirstAsync<RL_Meeting>(sql, new { MeetingGuid = MeetingGuid });
            }
        }

        /// <summary>
        /// 根据状态,UserCode获取会议列表
        /// </summary>
        /// <param name="UserCode">登录人USERCODE</param>
        /// <param name="Status">查询状态（待办传递参数"待开始会议",全部状态为"我发起的会议","历史会议","待开始会议"）</param>
        /// <returns></returns>
        public async Task<IEnumerable<RL_Meeting>> GetMeetinList(string  UserCode,string Status)
        {
           
            using (IDbConnection connect = DataBaseConfig.GetSqlConnection())
            {
                string sql = "";
                if (Status == "我发起的会议")
                {
                    sql = @"select * from RL_Meeting where CreateBy=@UserCode  order by CreateDate desc";

                }
                if(Status=="历史会议")
                {
                    sql = @"select * from (
                               select  CHARINDEX(','+ '" + @UserCode + @"' +','  ,  ','+Participants+',') as include,* from RL_Meeting
                               ) as sel where include>=1 or CreateBy=@UserCode and BeginTime<GETDATE() and Status='完成' order by BeginTime desc ";
                }
                if (Status == "待开始会议")
                {
                    sql = @"select * from (
                               select  CHARINDEX(','+ '" + @UserCode + @"' +','  ,  ','+Participants+',') as include,* from RL_Meeting
                               ) as sel where include>=1 or CreateBy=@UserCode and Status='待开始'   order by BeginTime  desc ";

                }
                if (sql.Equals(""))
                {
                    sql = @"select * from RL_Meeting where CreateBy='0'   order by BeginTime desc ";
                }
                return await connect.QueryAsync<RL_Meeting>(sql,new { UserCode = UserCode });

            }
        }

        /// <summary>
        /// 取消当前会议
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public async Task<bool> MeetinCancel( int id)
        {
            using (IDbConnection con = DataBaseConfig.GetSqlConnection())
            {
                string sql = "update RL_Meeting set Status='取消' where ID=@id";
                var rel= await con.ExecuteAsync(sql,new { id=id});
                if (rel > 0)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 取消整个循环会议
        /// </summary>
        /// <param name="MeetingGuid">GUid</param>
        /// <returns></returns>
        public async Task<bool> MeetinCancelAll(string MeetingGuid)
        {
            using (IDbConnection con=DataBaseConfig.GetSqlConnection())
            {
                string sql = "update RL_Meeting set Status='取消' where MeetingGuid=@MeetingGuid";
                var rel= await con.ExecuteAsync(sql, new { MeetingGuid = MeetingGuid });
                if (rel > 0)
                {
                    return true;
                }
                return false;
            }

        }

       /// <summary>
       /// 修改所有循环会议内容
       /// </summary>
       /// <param name="entity">会议实体</param>
       /// <returns></returns>
        public async Task<bool> MeetinUpdate(RL_Meeting entity)
        {
            using (IDbConnection con=DataBaseConfig.GetSqlConnection())
            {
                string sql = @"delete RL_Meeting where MeetingGuid=@MeetingGuid";
                var rel = await con.ExecuteAsync(sql,new { MeetingGuid =entity.MeetingGuid});
                if (rel > 0)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 修改会议状态为完成
        /// </summary>
        /// <param name="id">会议ID</param>
        /// <returns></returns>
        public async Task<bool> GetUpdateRL_MeetingStatus(int id)
        {
            using (IDbConnection con=DataBaseConfig.GetSqlConnection())
            {
                string sql = @"update RL_Meeting set Status='已完成' where ID=@id";
                var rel = await con.ExecuteAsync(sql, new { id = id });
                if (rel > 0)
                {
                    return true;
                }
                return false;
            }
        }



        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <param name="Email">收件人邮箱（多个以","分隔）</param>
        /// <param name="entity">邮件内容参数</param>
        /// <param name="EmailType">发送邮件类型（AddMeeting：新建会议,AddMeetinPeople:会议新增人员、UpdataMeeting：修改会议、CancelMeeting：会议取消、FrontMeeting：会议前半小时）</param>
        /// <returns></returns>
        public async Task<bool> EmailpostAsync(Emailparameter entity, string EmailType)
        {
            //SqlParameter[] ps = new SqlParameter[entity.Count];
            //int index = 0;
            //foreach (var item in entity)
            //{
            //    ps[index++] = new SqlParameter(item.Key, item.Value);
            //}
           
            try
            {
                string AddresseeEmail = "";
                //获取发件人邮箱
                string UserName = DataBaseConfig.GetUserName();
                string SmtpServer = DataBaseConfig.GetSmtpServer();
                string Pwd = DataBaseConfig.GetPwd();
                MailMessage mailObj = new MailMessage();
                mailObj.From = new MailAddress(UserName); //发送人邮箱地址
                if(EmailType== "AddMeeting"|| EmailType== "AddMeetinPeople")
                {
                    string OnEmail = "";

                    using (IDbConnection con = DataBaseConfig.GetSqlConnection())
                    {
                        string sql = "select * from RL_EmailRecord where MeetingId=@ID and CreateData=(select  MAX(CreateData) from RL_EmailRecord where MeetingId=@ID)";
                        var List = con.QueryAsync<RL_EmailRecord>(sql, new { id = entity.Meeting.ID });
                        var EmailList = List.Result.AsList();
                        if(EmailList.Count>0)
                        { 
                        var Meetingid = EmailList.AsList()[0].MeetingId;
                        var UserCode = EmailList.AsList()[0].UserCode;
                        var EmaiRecord = EmailList.AsList()[0].EmaiRecord;
                        string[] sArray = entity.Meeting.Email.Split(',');
                        foreach (var item in sArray)
                        {
                            if (!EmaiRecord.Contains(item))
                            {
                                OnEmail += item + ",";
                            }
                        }
                            AddresseeEmail = OnEmail.TrimEnd(',');
                        }
                        else
                        {
                            AddresseeEmail = entity.Meeting.Email;

                        }

                    }

                 
                }
                else
                {
                    AddresseeEmail = entity.Meeting.Email;
                }
                mailObj.To.Add(AddresseeEmail);   //收件人邮箱地址

                mailObj.IsBodyHtml = true;
                mailObj.SubjectEncoding = Encoding.UTF8;
                mailObj.BodyEncoding = Encoding.UTF8;
                string htmlbody = "";    
                //新建会议&会议新增人员 参数 会议时间，会议地点，会议内容
                if (EmailType == "AddMeeting")
                {
                   

                    mailObj.Subject = "会议提醒";    //主题
                    htmlbody = @"<html>   
                                <body>
                                <p>您好！<p/>
                                 <p style='margin-left:45px;'>
                                邀请您于" + entity.Meeting.BeginTime + @"," + entity.Meeting.MeetingTitle + @"参加会议。会议内容：" + entity.Meeting.MeetingTitle + @"<br/><br/>谢谢！ <p/>
                                <body/> 
                                <html/>";
                }
                if (EmailType == "AddMeetinPeople")
                {
                    mailObj.Subject = "会议提醒";    //主题
                    htmlbody = @"<html>   
                                <body>
                                <p>您好！<p/>
                                 <p style='margin-left:45px;'>
                                邀请您于" + entity.Meeting.BeginTime + @"," + entity.Meeting.MeetingTitle + @"参加会议。会议内容：" + entity.Meeting.MeetingTitle + @"<br/><br/>谢谢！ <p/>
                                <body/> 
                                <html/>";

                }
                //修改会议 参数 会议名称，新会议类型，新会议时间，新会议地点
                if (EmailType == "UpdataMeeting")
                {
                    mailObj.Subject = "会议提醒";    //主题
                    htmlbody = @"<html>   
                                <body>
                                <p>您好！<p/>
                                 <p style='margin-left:45px;'>
                                   " + entity.Meeting.MeetingTitle + @"，修改为" + entity.Meeting.Frequency+entity.Meeting.Week + @"," + entity.Meeting.BeginTime + @"," + entity.Meeting.Place + @"。请准时参加。<br/><br/>谢谢！<p/>
                               <body/> 
                                <html/>";
                }
                //会议取消 参数：会议时间，会议名称
                if (EmailType == "CancelMeeting")
                {
                    mailObj.Subject = "会议提醒";    //主题
                    htmlbody = @"<html>   
                                <body>
                                <p>您好！<p/>
                                 <p style='margin-left:45px;'>
                                    原定于" + entity.Meeting.BeginTime + @"的" + entity.Meeting.MeetingTitle + @"取消。请知悉。<br/><br/>谢谢！<p/>
                                 <body/>    
                                 <html/>";
                }
                //会议提前半小时 会议名称，会议时间，会议地点
                if (EmailType == "FrontMeeting")
                {
                    mailObj.Subject = "会议提醒";    //主题
                    htmlbody = @"<html>   
                                <body>
                                <p>您好！<p/>
                                 <p style='margin-left:45px;'>
                                    " + entity.Meeting.MeetingTitle + @"距" + entity.Meeting.BeginTime + @"会议开始还有半小时，会议地点" + entity.Meeting.Place + @"请准时参加。<br/><br/>谢谢！<p/>
                                <body/>    
                                <html/>";
                }


                mailObj.Body = htmlbody;    //正文
                SmtpClient smtp = new SmtpClient();
                smtp.Host = SmtpServer; //"smtp.163.com";         //smtp服务器名称
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = new NetworkCredential(UserName, Pwd);  //发送人的登录名和密码
                smtp.Send(mailObj);
                
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
                throw;
            }


        }
    }
}
