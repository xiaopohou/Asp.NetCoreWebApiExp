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
    /// 任务
    /// </summary>
    [ApiVersion("1.0")]
    [Authorize]
    //[ValidateAntiForgeryToken]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("Mobile")]
    public class MissionController : ControllerBase
    {
        private readonly IMissionRepository missionRepository;
        private readonly HtmlEncoder htmlEncoder;
        private readonly IEmailBusiness emailBusiness;
        private readonly ILogger<MissionController> logger = null;

        public MissionController(IMissionRepository _missionRepository, HtmlEncoder _htmlEncoder,
            IEmailBusiness _emailBusiness,ILogger<MissionController> _logger)
        {
            this.missionRepository = _missionRepository;
            this.htmlEncoder = _htmlEncoder;
            this.emailBusiness = _emailBusiness;
            this.logger = _logger;
        }
        /// <summary>
        /// 新增任务，ID默认为0即可
        /// </summary>
        /// <param name="rL_MainItem">任务实体,ID默认为0即可</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] RL_MainItem rL_MainItem)
        {
            if (ModelState.IsValid)
            {
                rL_MainItem.TaskName = htmlEncoder.Encode(rL_MainItem.TaskName);
                rL_MainItem.EditionNum = htmlEncoder.Encode(rL_MainItem.EditionNum);
                rL_MainItem.MissionName = htmlEncoder.Encode(rL_MainItem.MissionName);
                rL_MainItem.ProjectName = htmlEncoder.Encode(rL_MainItem.ProjectName);
                rL_MainItem.UserCode = RsaCrypto.Decrypt(rL_MainItem.UserCode);
                rL_MainItem.CreateBy = RsaCrypto.Decrypt(rL_MainItem.CreateBy);
                var res = await missionRepository.CreateEntityAsync(rL_MainItem);
                if (res > 0)
                {
                    rL_MainItem.ID = res;
                    Task.Run(() => emailBusiness.JoinEmailQueue<RL_MainItem>(new List<RL_MainItem>() { rL_MainItem }));
                    //var emailpost = await missionRepository.EmailpostAsync(rL_MainItem.Email, rL_MainItem, "AddMainItem");
                    //if (!emailpost)
                    //{
                    //    logger.LogError("新建任务{TaskName}邮件发送失败", rL_MainItem.TaskName);
                    //}
                    var missonTask = new Task<bool>(() =>missionRepository.EmailpostAsync(rL_MainItem.Email, rL_MainItem, "AddMainItem").Result);
                    missonTask.ContinueWith(p=>emailBusiness.WriteEmailLog(missonTask.Result, "Main", rL_MainItem.TaskName));
                    missonTask.Start();
                    return Ok("添加任务成功");
                }
                return BadRequest("添加任务失败");
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// 根据MissionID获取任务详情
        /// </summary>
        [HttpPost("TaskDetail")]
        public async Task<IActionResult> getMissionList(TeamDeleteId entity)
        {
            int ID=Convert.ToInt32(RsaCrypto.Decrypt(entity.ID));
            var res = await missionRepository.GetByIdAsync(ID);
            if (ID == -1)
            {
                return NotFound("未找到");

            }
            return Ok(res);
        }

     


        /// <summary>
        /// 根据状态查询任务列表
        /// </summary>
        /// <param name="entity">status任务状态（All为查询所有信息),用户Userid</param>
        /// <returns></returns>
        [HttpPost("TaskList")]
        public async Task<IActionResult> getMissonByType(GetMeetinList entity)
        {
            //路由参数为path得替换
            string UserCodeDecrypt = RsaCrypto.Decrypt(entity.UserCode.Replace("%2F", "/"));
            var res = await missionRepository.GetMissionBystatus(entity.Status, UserCodeDecrypt);
            if (entity.Status == "")
            {
                return NotFound("未找到");
            }
            return Ok(res);
        }

        /// <summary>
        /// 机构树获取
        /// DEPT_CODE为机构编码,DEPT_LEVEL为机构级别,初始化DEPT_LEVEL传1，DEPT_CODE任意值，
        /// 获取下级机构DEPT_CODE入参当前机构的DEPT_CODE，DEPT_LEVEL为当前机构级别+1
        /// </summary>
        /// <param name="rL_InstitutionParameter">DEPT_CODE为机构编码,DEPT_LEVEL为机构级别</param>
        /// <returns></returns>
        [HttpPost("Institution")]
        public async Task<IActionResult> GetInstitution([FromBody] RL_InstitutionParameter rL_InstitutionParameter)
        {
            var res = await missionRepository.GetDynamicsAsync(rL_InstitutionParameter);
            return Ok(res);
        }


        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <param name="Email">收件人邮箱（多个以","分隔）</param>
        /// <param name="entity">邮件内容参数</param>
        /// <param name="EmailType">发送邮件类型（AddMainItem：新建任务、EndMainItem：任务截止8小时）</param>
        /// <returns></returns>
        [HttpPost("Emailpost/{Email}/{EmailType}")]
        public async Task<IActionResult> EmailpostAsyncEmailpostAsync(string Email, RL_MainItem entity, string EmailType)
        {
            if (Email == "")
            {
                return NotFound("未找到");
            }
            var res = await missionRepository.EmailpostAsync(Email, entity, EmailType);
            if (res)
            {
                return Ok("邮件发送成功！");
            }
            return BadRequest("邮件发送失败");
        }
        /// <summary>
        /// 根据输入搜索人员(机构树)
        /// </summary>
        /// <param name="entry">输入字符(姓名，手机，邮箱)</param>
        /// <param name="page">页数</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>
        [HttpPost("{entry}/{page}/{rows}")]
        public async Task<IActionResult> GetUserByEntryAsync(string entry, int page, int rows)
        {
            var res = await missionRepository.GetOaUserByEntryAsync(entry, page, rows);
            if (res == null)
            {
                return NotFound("未找到");
            }
            return Ok(res);
        }
        /// <summary>
        /// 修改任务状态
        /// </summary>
        /// <param name="entity">任务ID</param>
        /// <returns></returns>
        [HttpPost("GetUpdataMissonStatus")]
        public async Task<IActionResult> GetUpdataMissonStatus([FromBody] TeamDeleteId entity)
        {
            int Id = Convert.ToInt32(RsaCrypto.Decrypt(entity.ID.Replace("%2F", "/")));
            var rel = await missionRepository.GetUpdataMissonStatus(Id);
            if (rel)
            {
                return Ok("修改成功");
            }
            return BadRequest("修改失败");
        }
        /// <summary>
        /// 根据工号获取人员信息
        /// </summary>
        /// <param name="userInfoParam"></param>
        /// <returns></returns>
        [HttpPost("UserInfo")]
        public async Task<IActionResult> GetUserInfo([FromBody]UserInfoParam userInfoParam)
        {
            //路由参数为path得替换
            string UserCodeDecrypt = RsaCrypto.Decrypt(userInfoParam.UserCode.Replace("%2F", "/"));
            var res = await missionRepository.GetUserInfo(UserCodeDecrypt);
            if (res == null)
            {
                return NotFound("未找到");
            }
            return Ok(res);
        }
    }
}