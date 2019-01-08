using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TMS_MobileAPI.Core;
using TMS_MobileAPI.HangFire;
using TMS_MobileRepository.Entity;
using TMS_MobileRepository.IRepository;

namespace TMS_MobileAPI.Controllers
{
    //[ValidateAntiForgeryToken]
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [EnableCors("Mobile")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ITeamRepository teamRepository;
        private readonly ILogger<TeamController> _logger = null;
        private readonly HtmlEncoder htmlEncoder;

        public TeamController(ITeamRepository _teamRepository,ILogger<TeamController> logger,HtmlEncoder _htmlEncoder)
        {
            this.teamRepository = _teamRepository;
            this._logger = logger;
            htmlEncoder = _htmlEncoder;
        }

        /// <summary>
        /// 新建团队
        /// </summary>
        /// <param name="rL_TeamRes">Team中ID默认传0，TeamGuid空，TeamMembers中ID默认传0，TeamGuid空</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync(RL_TeamRes rL_TeamRes)
        {
            if (ModelState.IsValid)
            {
                rL_TeamRes.Team.TeamName = htmlEncoder.Encode(rL_TeamRes.Team.TeamName);

                if (rL_TeamRes.TeamMembers.Count() > 0)
                {
                    foreach (var item in rL_TeamRes.TeamMembers)
                    {
                        item.UserCode = RsaCrypto.Decrypt(item.UserCode);
                    }
                }

                rL_TeamRes.Team.CreateBy = RsaCrypto.Decrypt(rL_TeamRes.Team.CreateBy.Replace("%2F", "/"));

                var res = await teamRepository.AddEntityAsync(rL_TeamRes);
                if (res)
                {
                    //_logger.LogInformation("新建团队成功");
                    return Ok("新建团队成功");
                }
                //_logger.LogInformation("新建团队失败");
                return BadRequest("新建团队失败");
            }
            
            return BadRequest(ModelState);
        }

        /// <summary>
        /// 所在团队页加载获取全部团队
        /// </summary>
        /// <returns></returns>
        [HttpPost("TeamList")]
        public async Task<IActionResult> GetTeamList()
        {
           
            var rel = await teamRepository.GetTeamList();
            return Ok(rel);
        }
        /// <summary>
        /// 获取我创建的团队
        /// </summary>
        /// <param name="userInfoParam">登录人userCode</param>
        /// <returns></returns>
        [HttpPost("CreateTeamList")]
        public async Task<IActionResult> GetMyCreateTeamList([FromBody]UserInfoParam userInfoParam)
        {
            //路由参数为path得替换
            string UserCodeDecrypt = RsaCrypto.Decrypt(userInfoParam.UserCode.Replace("%2F", "/"));
            var rel = await teamRepository.GetMyCreateTeamList(UserCodeDecrypt);
            if (userInfoParam.UserCode == null)
            {
                return NotFound("未找到");
            }
            if (rel == null)
            {
                return NotFound("未找到");
            }
            return Ok(rel);
        }

        /// <summary>
        /// 获取我加入的团队
        /// </summary>
        /// <param name="UserCode"></param>
        /// <returns></returns>
        [HttpPost("JoinTeamList")]
        public async Task<IActionResult> GetMyJoinTeamList([FromBody]UserInfoParam userInfoParam)
        {
            //路由参数为path得替换
            string UserCodeDecrypt = RsaCrypto.Decrypt(userInfoParam.UserCode.Replace("%2F", "/"));
            var rel = await teamRepository.GetMyJoinTeamList(UserCodeDecrypt);
            if (userInfoParam.UserCode == null)
            {
                return NotFound("未找到");
            }
            return Ok(rel);
        }

        /// <summary>
        /// 根据主键删除团队成员
        /// </summary>
        /// <param name="entity">团队成员主键ID</param>
        /// <returns></returns>
        [HttpPost("TeamMember")]
        public async Task<IActionResult> GetTeamMemberDelete(TeamDeleteId entity)
        {
            entity.ID = RsaCrypto.Decrypt(entity.ID);
            var rel = await teamRepository.GetTeamMemberDelete(entity);
            if (rel)
            {

                return Ok("删除成功！");
            }
            return BadRequest("删除失败！");
        }


        /// <summary>
        /// 删除团队及团队下所有成员
        /// </summary>
        /// <param name="entity">团队ID</param>
        /// <returns></returns>
        [HttpPost("TeamDelete")]
        public async Task<IActionResult> GetTeamDelete(TeamDeleteId entity)
        {
            entity.ID = RsaCrypto.Decrypt(entity.ID);
            var rel = await teamRepository.GetTeamDelete(entity);
            if (rel)
            {
                return Ok("删除成功！");
            }
            return BadRequest("删除失败！");
        }

        /// <summary>
        /// 根据姓名查询数据
        /// </summary>
        /// <param name="Name">姓名</param>
        /// <returns></returns>
        [HttpPost("GetByNameList")]
        public async Task<IActionResult> GetByNameList(string Name)
        {
            var rel = await teamRepository.GetByNameList(Name);
            if (Name == null)
            {
                return NotFound("未找到");
            }
            return Ok(rel);
        }

       
    }
}