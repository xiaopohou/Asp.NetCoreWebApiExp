using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS_MobileAPI.Core
{
    /// <summary>
    /// 2018/12/11 zhong
    /// appsetting类
    /// </summary>
    public class AppSetting
    {
        /// <summary>
        /// sqlserver数据库连接字符串
        /// </summary>
        public string SqlServerConnection { get; set; }
        /// <summary>
        /// 发送邮件配置
        /// </summary>
        public string Emailpost { get; set; }

        /// <summary>
        /// Token服务地址
        /// </summary>
        public string Url { get; set; }

        ///TokenEndPoint地址
        public string TokenUrl { get; set; }

    }
}
