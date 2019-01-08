using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TMS_MobileAPI.Core;
using TMS_MobileRepository.IRepository;

namespace TMS_MobileAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        public ValuesController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        /// <summary>
        /// 获取人员测试用
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get(int id,int s)
        {

            var res = await userRepository.GetAllAsync();
            if (res == null)
            {
                return NotFound();
            }
            return Ok(res);
        }

        
    }
}
