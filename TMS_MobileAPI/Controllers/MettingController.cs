using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TMS_MobileAPI.Business;
using TMS_MobileAPI.Core;
using TMS_MobileRepository.Entity;
using TMS_MobileRepository.IRepository;

namespace TMS_MobileAPI.Controllers
{
    /// <summary>
    /// 会议
    /// </summary>
    [ApiVersion("1.0")]
    [Authorize]
    //[ValidateAntiForgeryToken]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [EnableCors("Mobile")]
    [ApiController]
    public class MettingController : ControllerBase
    {
        private readonly IMeetingRepository meetingRepository;
        private readonly HtmlEncoder htmlEncoder;
        private readonly ILogger<MettingController> logger = null;
        private readonly IEmailBusiness emailBusiness;
        /// <summary>
        /// 会议构造函数
        /// </summary>
        /// <param name="_meetingRepository"></param>
        /// <param name="_htmlEncoder"></param>
        /// <param name="_logger"></param>
        /// <param name="_emailBusiness"></param>
        public MettingController(IMeetingRepository _meetingRepository, HtmlEncoder _htmlEncoder,
            ILogger<MettingController> _logger,IEmailBusiness _emailBusiness)
        {
            this.meetingRepository = _meetingRepository;
            this.htmlEncoder = _htmlEncoder;
            this.logger = _logger;
            this.emailBusiness = _emailBusiness;
        }
        /// <summary>
        /// 根据状态,UserCode获取会议列表
        /// </summary>
        /// <param name="entity">UserCode：登录人USERCODEStatus：</param>
        /// <param name="Status">查询状态（待办传递参数"待开始会议",
        /// 全部状态为"我发起的会议","历史会议","待开始会议"）</param>
        /// <returns></returns>
        [HttpPost("GetMeetinList")]
        public async Task<IActionResult> GetMeetinList(GetMeetinList entity)
        {
            //路由参数为path得替换
            entity.UserCode = RsaCrypto.Decrypt(entity.UserCode.Replace("%2F", "/"));
            var rel = await meetingRepository.GetMeetinList(entity.UserCode,entity.Status);
            if (rel.Count() <= 0)
            {
                return NotFound("未找到");
            }
            //foreach (var item in rel)
            //{
            //    item.Place = System.Web.HttpUtility.HtmlDecode(item.Place);
            //    item.MeetingTitle = System.Web.HttpUtility.HtmlDecode(item.MeetingTitle);
            //}
            if (entity.UserCode.ToString() == null)
            {
                return NotFound("未找到");
            }
            return Ok(rel);
        }

        /// <summary>
        /// 根据ID获取会议详情与修改当前会议获取详情
        /// </summary>
        /// <param name="entity">会议主键ID</param>
        /// <returns></returns>
        [HttpPost("GetByIdAsync")]
        public async Task<IActionResult> GetByIdAsync(TeamDeleteId entity)
        {
            int id = Convert.ToInt32(RsaCrypto.Decrypt(entity.ID));
            var rel = await meetingRepository.GetByIdAsync(id);


            if (rel == null)
            {
                return NotFound("未找到");
            }
            //rel.Place = System.Web.HttpUtility.HtmlDecode(rel.Place);
            //rel.MeetingTitle = System.Web.HttpUtility.HtmlDecode(rel.MeetingTitle);

            return Ok(rel);
        }
        /// <summary>
        ///  会议修改，根据MeetingGuid获取修改页面数据详情（修改整个循环用）
        /// </summary>
        /// <param name="entity">GUid</param>
        /// <returns></returns>
        [HttpPost("GetMeetinUpdateDetail")]
        public async Task<IActionResult> GetMeetinUpdateDetail(string MeetingGuid)
        {
         
            var rel = await meetingRepository.GetMeetinUpdateDetail(MeetingGuid);
            if (rel == null)
            {
                return NotFound("未找到");
            }
            if (MeetingGuid == null)
            {
                return NotFound("未找到");
            }
            //rel.Place = System.Web.HttpUtility.HtmlDecode(rel.Place);
            //rel.MeetingTitle = System.Web.HttpUtility.HtmlDecode(rel.MeetingTitle);
            return Ok(rel);
        }

        /// <summary>
        /// 取消当前会议
        /// </summary>
        /// <param name="entity">主键ID</param>
        /// <returns></returns>
        [HttpPost("MeetinCancel")]
        public async Task<IActionResult> MeetinCancel(TeamDeleteId entity)
        {
            int id = Convert.ToInt32(RsaCrypto.Decrypt(entity.ID));
            var rel = await meetingRepository.MeetinCancel(id);
            if (rel)
            {
                //获取邮件所需信息
                var ins = await meetingRepository.GetByIdAsync(id);
                //ins.Place = System.Web.HttpUtility.HtmlDecode(ins.Place);
                //ins.MeetingTitle = System.Web.HttpUtility.HtmlDecode(ins.MeetingTitle);
                Emailparameter em = new Emailparameter();
                NewMethod(ins, em);
                //var Emailrel = await meetingRepository.EmailpostAsync(em, "CancelMeeting");
                //if (Emailrel)
                //{
                //    return Ok("取消成功，邮件发送成功");
                //}
                //return Ok("取消成功，邮件发送失败");

                var meetTask = new Task<bool>(() => meetingRepository.EmailpostAsync(em, "CancelMeeting").Result);
                meetTask.ContinueWith(p => emailBusiness.WriteEmailLog(meetTask.Result, "Meet", ins.MeetingTitle));
                meetTask.Start();
                return Ok("取消成功");
            }
            return BadRequest("取消失败");
        }

        /// <summary>
        /// 取消全部循环会议
        /// </summary>
        /// <param name="MeetingGuid">GUid</param>
        /// <returns></returns>
        [HttpPost("MeetinCancelAll")]
        public async Task<IActionResult> MeetinCancelAll(string MeetingGuid)
        {
            var rel = await meetingRepository.MeetinCancelAll(MeetingGuid);
            if (rel)
            {   //获取邮件所需信息
                var ins = await meetingRepository.GetMeetinUpdateDetail(MeetingGuid);
                //ins.Place = System.Web.HttpUtility.HtmlDecode(ins.Place);
                //ins.MeetingTitle = System.Web.HttpUtility.HtmlDecode(ins.MeetingTitle);
                Emailparameter em = new Emailparameter();
                NewMethod(ins, em);
                //var Emailrel = await meetingRepository.EmailpostAsync(em, "CancelMeeting");
                //if (Emailrel)
                //{
                //    return Ok("取消成功，邮件发送成功");
                //}
                //return Ok("取消成功，邮件发送失败");

                var meetTask = new Task<bool>(() => meetingRepository.EmailpostAsync(em, "CancelMeeting").Result);
                meetTask.ContinueWith(p => emailBusiness.WriteEmailLog(meetTask.Result, "Meet", ins.MeetingTitle));
                meetTask.Start();
                return Ok("取消成功");
            }
            return BadRequest("取消失败");
        }

        /// <summary>
        /// 根据ID修改会议
        /// </summary>
        /// <param name="entity">会议实体</param>
        /// <returns></returns>
        [HttpPost("UpdateThis")]
        public async Task<IActionResult> UpdateAsync(RL_Meeting entity)
        {
            entity.CreateBy = RsaCrypto.Decrypt(entity.CreateBy.Replace("%2F", "/"));
            entity.ID =Convert.ToInt32(RsaCrypto.Decrypt(entity.UpdateID));
            var rel = await meetingRepository.UpdateAsync(entity);
            if (rel)
            {
                Emailparameter em = new Emailparameter();
                NewMethod(entity, em);
                //var Emailrel = await meetingRepository.EmailpostAsync(em, "UpdataMeeting");
                //if (Emailrel)
                //{
                //    return Ok("修改成功，邮件发送成功");
                //}
                //return Ok("修改成功,邮件发送失败");

                var meetTask = new Task<bool>(() => meetingRepository.EmailpostAsync(em, "UpdataMeeting").Result);
                meetTask.ContinueWith(p => emailBusiness.WriteEmailLog(meetTask.Result, "Meet", entity.MeetingTitle));
                meetTask.Start();
                return Ok("修改成功");
            }
            return BadRequest("修改失败");
        }

        private static void NewMethod(RL_Meeting entity, Emailparameter em)
        {
            em.Meeting = entity;
        }

        /// <summary>
        /// 修改所有循环会议内容
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("MeetinUpdate")]
        public async Task<IActionResult> MeetinUpdate(RL_Meeting entity)
        {
            entity.CreateBy = RsaCrypto.Decrypt(entity.CreateBy.Replace("%2F", "/"));
            if (ModelState.IsValid)
            {
                var rel = await meetingRepository.MeetinUpdate(entity);

                MettingBusiness mettingBusiness = new MettingBusiness();
                var listRes = mettingBusiness.GetEntityList(entity);
                if (listRes.ToList().Count == 0)
                {
                    return BadRequest("会议循环无法执行");
                }
                var res = await meetingRepository.AddEntityListAsync(listRes);
                if (res)
                {

                    Emailparameter em = new Emailparameter();
                    NewMethod(entity, em);
                    //var Emailrel = await meetingRepository.EmailpostAsync(em, "UpdataMeeting");
                    //if (Emailrel)
                    //{
                    //    return Ok("修改成功，邮件发送成功");
                    //}
                    //return Ok("修改成功,邮件发送失败");
                    var meetTask = new Task<bool>(() => meetingRepository.EmailpostAsync(em, "UpdataMeeting").Result);
                    meetTask.ContinueWith(p => emailBusiness.WriteEmailLog(meetTask.Result, "Meet", entity.MeetingTitle));
                    meetTask.Start();
                    return Ok("修改成功");

                }
                return BadRequest("修改建会议失败");

            }
            return BadRequest(ModelState);
        }
        /// <summary>
        /// 新建会议
        /// </summary>
        /// <param name="rL_Meeting">Id默认为0，MeetingGuid默认为空， meetingType为0或1，frequency为"每周"，"每双周","每四周"，week为周一到周五
        /// 注意开始结束时间逻辑,ParticipantsCode、Participants、Email多个以逗号分隔</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync(RL_Meeting rL_Meeting)
        {
            if (ModelState.IsValid)
            {
                rL_Meeting.MeetingTitle = htmlEncoder.Encode(rL_Meeting.MeetingTitle);
                rL_Meeting.Place = htmlEncoder.Encode(rL_Meeting.Place);
                rL_Meeting.CreateBy = RsaCrypto.Decrypt(rL_Meeting.CreateBy);
                rL_Meeting.ParticipantsCode = RsaCrypto.Decrypt(rL_Meeting.ParticipantsCode);
                if (rL_Meeting.MeetingType == 0)
                {
                    //var res = await meetingRepository.AddEntityAsync(rL_Meeting);
                    var res = await meetingRepository.CreateEntityAsync(rL_Meeting);
                    if (res>0)
                    {
                        rL_Meeting.ID = res;
                        Task.Run(()=>  emailBusiness.JoinEmailQueue<RL_Meeting>(new List<RL_Meeting>() { rL_Meeting }));

                        Emailparameter emailparameter = new Emailparameter();
                        emailparameter.Meeting = rL_Meeting;
                        //var emailpost=await meetingRepository.EmailpostAsync(emailparameter, "AddMeeting");
                        //if (!emailpost)
                        //{
                        //    logger.LogError("新建会议{MeetName}邮件发送失败", rL_Meeting.MeetingTitle);
                        //}
                        var mettingTask = new Task<bool>(() => meetingRepository.EmailpostAsync(emailparameter, "AddMeeting").Result);
                        mettingTask.ContinueWith(p => emailBusiness.WriteEmailLog(mettingTask.Result, "Meeting", rL_Meeting.MeetingTitle));
                        mettingTask.Start();

                        return Ok("新建会议成功");
                    }
                    return BadRequest("新建会议失败");
                }
                else
                {
                    MettingBusiness mettingBusiness = new MettingBusiness();
                    var listRes = mettingBusiness.GetEntityList(rL_Meeting);
                    if (listRes.ToList().Count == 0)
                    {
                        return BadRequest("会议循环无法执行");
                    }
                    //var res = await meetingRepository.AddEntityListAsync(listRes);
                    var res = await meetingRepository.CreateEntityListAsync(listRes);
                    if (res.ToList().Count>0)
                    {
                        var mettingList = mettingBusiness.AddIDToMettingList(res, listRes);
                        Task.Run(() => emailBusiness.JoinEmailQueue<RL_Meeting>(mettingList));

                        Emailparameter emailparameter = new Emailparameter();
                        emailparameter.Meeting = rL_Meeting;
                        //var emailpost = await meetingRepository.EmailpostAsync(emailparameter, "AddMeeting");
                        //if (!emailpost)
                        //{
                        //    logger.LogError("新建会议{MeetName}邮件发送失败", rL_Meeting.MeetingTitle);
                        //}

                        var mettingTask = new Task<bool>(() => meetingRepository.EmailpostAsync(emailparameter, "AddMeeting").Result);
                        mettingTask.ContinueWith(p => emailBusiness.WriteEmailLog(mettingTask.Result, "Meeting", rL_Meeting.MeetingTitle));
                        mettingTask.Start();

                        return Ok("新建会议成功");
                    }
                    return BadRequest("添新建会议失败");
                }
            }
            return BadRequest(ModelState);

        }

        /// <summary>
        /// 修改会议状态为完成
        /// </summary>
        /// <param name="entity">会议ID</param>
        /// <returns></returns>
        [HttpPost("GetUpdateRL_MeetingStatus")]
        public async Task<IActionResult> GetUpdateRL_MeetingStatus(TeamDeleteId entity)
        {
            int id = Convert.ToInt32(RsaCrypto.Decrypt(entity.ID));
            var rel = await meetingRepository.GetUpdateRL_MeetingStatus(id);
            if (rel)
            {
                return Ok("修改成功！");
            }
            return BadRequest("修改失败！");
        }

        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <param name="Email">收件人邮箱（多个以","分隔）</param>
        /// <param name="entity">邮件内容参数</param>
        /// <param name="EmailType">发送邮件类型（AddMeeting：新建会议,AddMeetinPeople:会议新增人员、UpdataMeeting：修改会议、CancelMeeting：会议取消、FrontMeeting：会议前半小时）</param>
        /// <returns></returns>
        [HttpPost("Emailpost/{Email}/{EmailType}")]
        public async Task<IActionResult> EmailpostAsyncEmailpostAsync(Emailparameter entity, string EmailType)
        {
            if (entity.Meeting.Email == "")
            {
                return NotFound("未找到");
            }
            var res = await meetingRepository.EmailpostAsync(entity, EmailType);
            if (res)
            {
                return Ok("邮件发送成功");
            }
            return BadRequest("邮件发送失败");
        }
    }
}
