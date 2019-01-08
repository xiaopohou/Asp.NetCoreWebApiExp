using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TMS_MobileAPI.Core;
using TMS_MobileRepository.Entity;
using TMS_MobileRepository.IRepository;

namespace TMS_MobileAPI.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("Mobile")]
    [ApiController]
    public class CredentialController : ControllerBase
    {


        private readonly AppSetting appSetting;
        private readonly IMissionRepository missionRepository;
        private readonly ILogger<CredentialController> logger = null;
        public CredentialController(IOptions<AppSetting> options, IMissionRepository _missionRepository,
            ILogger<CredentialController> _logger)
        {
            this.appSetting= options.Value;
            this.missionRepository = _missionRepository;
            this.logger = _logger;
        }
        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="userInfoParam"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Get(UserInfoParam userInfoParam)
        {
            var userDecode = RsaCrypto.Decrypt(userInfoParam.UserCode);
            var res= await missionRepository.GetUserInfo(userDecode);
            if (res != null)
            {
                
                //var discoveryClient = new DiscoveryClient(appSetting.Url) { Policy = { RequireHttps = false } };
                //var disco = await discoveryClient.GetAsync();
                //if (disco.IsError)
                //{
                //    Console.WriteLine(disco.Error);
                //    await HttpContext.Response.Body.WriteAsync(Encoding.Default.GetBytes(disco.Error));
                //    await HttpContext.Response.Body.FlushAsync();
                //}
                //if (string.Equals(TokenClientHelper.tokenEndPoint,null))
                //{
                //   await TokenClientHelper.GetTokenClient();
                //}
                
                var tokenClient = new TokenClient(appSetting.TokenUrl, "tms", "secret");
                
                //var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");
                var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("TMSMOBILE", "1qaz", "api1");
                if (tokenResponse.IsError)
                {
                    return NotFound(tokenResponse.Json);
                }
                
                return Ok(tokenResponse.Json);
            }
            return Unauthorized("没有权限");
        }
    }
}