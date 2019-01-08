using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace TMS_MobileRepository.Helpers
{
    /// <summary>
    /// 获取配置帮助类
    /// </summary>
    public class ConfigurationHelper
    {
        /// <summary>
        /// 配置信息
        /// </summary>
        public static IConfiguration config { get; set; }
        /// <summary>
        /// 根据key得到配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValue(string key)
        {
            var res = config.GetSection(key).Value;
            return res;
        }
    }
}
