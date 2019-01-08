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
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace TMS_MobileRepository.Repository
{
    /// <summary>
    /// 任务数据获取
    /// </summary>
    public class MissonRepository : IMissionRepository
    {
        public Task<bool> AddEntityAsync(RL_MainItem entity)
        {
            //using (IDbConnection conn = DataBaseConfig.GetSqlConnection())
            //{
            //    string querySql = @"insert into RL_MainItem values(@TaskName,@MissionName,@ProjectName,@EditionNum,
            //                     @ProjectType,@Executor,@EndTime,@CreateDate,@CreateBy,@Status,@UserCode,@Email);
            //                     SELECT CAST(SCOPE_IDENTITY() as int)";
            //    var res = await conn.ExecuteAsync(querySql, entity);
            //    return res;
            //}
            throw new NotImplementedException();
        }
        public async Task<int> CreateEntityAsync(RL_MainItem entity)
        {
            using (IDbConnection conn = DataBaseConfig.GetSqlConnection())
            {
                string querySql = @"insert into RL_MainItem values(@TaskName,@MissionName,@ProjectName,@EditionNum,
                                 @ProjectType,@Executor,@EndTime,@CreateDate,@CreateBy,@Status,@UserCode,@Email);
                                 SELECT CAST(SCOPE_IDENTITY() as int)";
                var res = await conn.QueryFirstOrDefaultAsync<int>(querySql, entity);
                return res;
            }
        }

        public Task<bool> AddEntityListAsync(IEnumerable<RL_MainItem> entityList)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RL_MainItem>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<RL_MainItem> GetByIdAsync(int id)
        {
            using (IDbConnection conn = DataBaseConfig.GetSqlConnection())
            {
                string SQL = @"select * from RL_MainItem where ID=@ID";
                return await conn.QueryFirstOrDefaultAsync<RL_MainItem>(SQL, new { ID = id });
            }

        }

        public Task<bool> UpdateAsync(RL_MainItem entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RL_MainItem>> GetMissionBystatus(string status, string UserCode)
        {
            using (IDbConnection conn = DataBaseConfig.GetSqlConnection())
            {
                string sql = "";
                if (status == "我发起的任务")
                {
                    sql = "select * from RL_MainItem where ISNULL(status,'All')=case when ISNULL('All','All')='All' then ISNULL(status,'All') else 'All' end and CreateBy=@UserCode order by CreateDate desc";
                }
                else
                {
                    sql = @"select * from (select * from RL_MainItem where ISNULL(status,'All')=case when ISNULL(@status,'All')='All' then ISNULL(status,'All') else @status end and CreateBy=@UserCode 
                            union
                            select * from RL_MainItem where ISNULL(status,'All')=case when ISNULL(@status,'All')='All' then ISNULL(status,'All') else @status end and UserCode=@UserCode) as rl order by CreateDate desc";
                }
                return await conn.QueryAsync<RL_MainItem>(sql, new { status = status, UserCode = UserCode });

            }
        }
        /// <summary>
        /// 机构数
        /// </summary>
        /// <param name="rL_Institution"></param>
        /// <returns></returns>
        public async Task<dynamic> GetDynamicsAsync(RL_InstitutionParameter rL_Institution)
        {
            using (IDbConnection conn = DataBaseConfig.GetOracleConnection())
            {
                string sql = "";
                RL_InstitutionResult rL_InstitutionResult = new RL_InstitutionResult();
                ///第一级目录
                if (rL_Institution.DEPT_LEVEL == 1)
                {
                    sql = @"select Dept_Code,Dept_Name,Dept_PCode,Dept_Level from SY_ORG_DEPT where Dept_Level=1";
                    var res = await conn.QueryAsync<RL_Institution>(sql, rL_Institution);
                    rL_InstitutionResult.Institutions = res;

                }
                else
                {
                    sql = @"select Dept_Code,Dept_Name,Dept_PCode,Dept_Level from SY_ORG_DEPT where  dept_pcode=:DEPT_CODE and Dept_Level=:DEPT_LEVEL";
                    //string sql1= @"select User_Code,User_Name,User_Email from SY_ORG_USER where Dept_Code=:DEPT_CODE";
                    //                    string sql1 = @"select User_Code,User_Name,User_Email from SY_ORG_USER where Dept_Code in 
                    //(select Dept_PCode from SY_ORG_DEPT where  dept_pcode=:DEPT_CODE and Dept_Level=:DEPT_LEVEL)";
                    string sql1 = @"select User_Code,User_Name,User_Email from SY_ORG_USER where Dept_Code=:DEPT_CODE";
                    var institutions =await conn.QueryAsync<RL_Institution>(sql, rL_Institution);
                    var users =await conn.QueryAsync<RL_OAUser>(sql1, rL_Institution);
                                            
                    rL_InstitutionResult.Institutions = institutions;
                    rL_InstitutionResult.OAUsers = users;



                }
                return rL_InstitutionResult;




            }
        }

        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <param name="Email">收件人邮箱（多个以","分隔）</param>
        /// <param name="entity">邮件内容参数</param>
        /// <param name="EmailType">发送邮件类型（AddMainItem：新建任务、EndMainItem：任务截止8小时）</param>
        /// <returns></returns>
        public async Task<bool> EmailpostAsync(string Email, RL_MainItem entity, string EmailType)
        {
            //SqlParameter[] ps = new SqlParameter[entity.Count];
            //int index = 0;
            //foreach (var item in entity)
            //{
            //    ps[index++] = new SqlParameter(item.Key, item.Value);
            //}
            try
            {
                //获取发件人邮箱
                string UserName = DataBaseConfig.GetUserName();
                string SmtpServer = DataBaseConfig.GetSmtpServer();
                string Pwd = DataBaseConfig.GetPwd();
                MailMessage mailObj = new MailMessage();
                mailObj.From = new MailAddress(UserName); //发送人邮箱地址
                mailObj.To.Add(Email);   //收件人邮箱地址
                mailObj.IsBodyHtml = true;
                mailObj.SubjectEncoding = Encoding.UTF8;
                mailObj.BodyEncoding = Encoding.UTF8;
                string htmlbody = "";
                //新建任务 参数 任务名，截止时间，任务类型，相关任务书，相关项目，相关版本
                if (EmailType == "AddMainItem")
                {
                    mailObj.Subject = "任务待办提醒";    //主题
                    htmlbody = @"<html>   
                                <body>
                                <p>您好！<p/>
                                 <p style='margin-left:45px;'>
                                 任务：" + entity.TaskName + @"发送至您，截止时间" + entity.EndTime + @"。<br/>任务相关信息：<br/>任务类型：" + entity.ProjectType + @"<br/>任务书：" +entity.MissionName + @"<br/>项目：" + entity.ProjectName + @"<br/>版本：" + entity.EditionNum + @"<br/><br/>请登录云助理->国寿研发频道->IT 管理平台统一视图查看<br/><br/>谢谢！<p/>
                                <body/> 
                                <html/>";
                }
                //任务截止8小时 参数：任务名，截止时间
                if (EmailType == "EndMainItem")
                {
                    mailObj.Subject = "任务待办提醒";    //主题
                    htmlbody = @"<html>   
                                <body>
                                <p>您好！<p/>
                                 <p style='margin-left:45px;'>
                                 任务：" + entity.TaskName + @"截止时间" + entity.EndTime + @"。请及时完成，<br/>如已完成请登录云助理->国寿研发频道->IT 管理平台统一视图设置任务状态为完成。<body/> 
                                <p/>
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
            catch (Exception)
            {
                return false;
                throw;
            }


        }
        /// <summary>
        /// 根据输入获取人员 
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public async Task<IEnumerable<dynamic>> GetOaUserByEntryAsync(string entry, int page, int rows)
        {
            int rowsend = page * rows + 1;
            int rowsstart = (page - 1) * rows;
            using (IDbConnection conn = DataBaseConfig.GetOracleConnection())
            {
                string sql = "";
                if (Regex.IsMatch(entry, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))//Email
                {
                    sql = @"select D.Dept_Name as GRANDFATHER_DEPT_NAME, C.Dept_Name as FATHER_DEPT_NAME,User_Code,User_Name,User_Email from (select * from (select A.*,rownum rn from 
(select User_Code,User_Name,User_Email,Dept_Code from SY_ORG_USER where User_Email like '%'||:entry||'%') A 
where rownum<:rowsend) where rn>:rowsstart) B left join SY_ORG_DEPT C on C.Dept_Code=B.Dept_Code left join 
SY_ORG_DEPT D on D.Dept_Code=C.Dept_Pcode";
                }
                else if (Regex.IsMatch(entry, @"^[+-]?\d*$"))//phone
                {
                    sql = @"select D.Dept_Name as GRANDFATHER_DEPT_NAME, C.Dept_Name as FATHER_DEPT_NAME,User_Code,User_Name,User_Email from (select * from (select A.*,rownum rn from 
(select User_Code,User_Name,User_Email,Dept_Code from SY_ORG_USER where User_Mobile like '%'||:entry||'%') A 
where rownum<:rowsend) where rn>:rowsstart) B left join SY_ORG_DEPT C on C.Dept_Code=B.Dept_Code left join 
SY_ORG_DEPT D on D.Dept_Code=C.Dept_Pcode";
                }
                else
                {
                    sql = @"select D.Dept_Name as GRANDFATHER_DEPT_NAME, C.Dept_Name as FATHER_DEPT_NAME,User_Code,User_Name,User_Email from (select * from (select A.*,rownum rn from 
(select User_Code,User_Name,User_Email,Dept_Code from SY_ORG_USER where User_Name like '%'||:entry||'%') A 
where rownum<:rowsend) where rn>:rowsstart) B left join SY_ORG_DEPT C on C.Dept_Code=B.Dept_Code left join 
SY_ORG_DEPT D on D.Dept_Code=C.Dept_Pcode";
                }
                var res = await conn.QueryAsync<RL_OAUser>(sql, new
                {
                    entry = entry
                    ,
                    rowsend = rowsend,
                    rowsstart = rowsstart,

                });
                return res;




            }
        }

        /// <summary>
        /// 修改任务状态
        /// </summary>
        /// <param name="Id">任务ID</param>
        /// <returns></returns>
        public async Task<bool> GetUpdataMissonStatus(int Id)
        {
            using (IDbConnection con = DataBaseConfig.GetSqlConnection())
            {
                string sql = "update RL_MainItem set status='已完成' where ID=@Id";
                var rel = await con.ExecuteAsync(sql, new { Id = Id });
                if (rel > 0)
                {
                    return true;
                }
                return false;


            }
        }

        public async Task<dynamic> GetUserInfo(string UserCode)
        {
            using (IDbConnection conn = DataBaseConfig.GetOracleConnection())
            {

                string sql = @"select User_Code,User_Name,User_Email from SY_ORG_USER where User_Code=:UserCode";
                var res =await conn.QueryFirstOrDefaultAsync<dynamic>(sql, new { UserCode = UserCode });
                return res;

            }
        }

    }
}
