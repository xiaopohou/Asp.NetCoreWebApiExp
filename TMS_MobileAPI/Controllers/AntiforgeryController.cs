using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TMS_MobileAPI.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [EnableCors("Csrf")]
    [ApiController]
    public class AntiforgeryController : ControllerBase
    {


        private readonly IHttpContextAccessor accessor;
        private readonly IAntiforgery antiforgery;

        public AntiforgeryController(IHttpContextAccessor _accessor,IAntiforgery _antiforgery)
        {
            this.accessor = _accessor;
            this.antiforgery = _antiforgery;
        }
        /// <summary>
        /// 获取XSRF-TOKEN
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetTeamMemberDelete()
        {

            var tokens = antiforgery.GetAndStoreTokens(accessor.HttpContext);
            return Ok(tokens.RequestToken);
        }
    }
}