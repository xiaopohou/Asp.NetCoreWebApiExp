using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.SqlClient;
using TMS_MobileRepository.Helpers;

namespace TMS_MobileRepository
{
    /// <summary>
    /// 2018/12/11 zhong
    /// 数据库配置信息获取类
    /// </summary>
    public class DataBaseConfig
    {
        /// <summary>
        /// Sql Server链接字符串
        /// </summary>
        public static IDbConnection GetSqlConnection()
        {
            string connect = ConfigurationHelper.GetValue("ConnectionStrings:SqlServerConnection");
            IDbConnection conn = new SqlConnection(connect);
            conn.Open();
            return conn;
        }
        /// <summary>
        /// 获取oracle连接字符串
        /// </summary>
        /// <returns></returns>
        public static IDbConnection GetOracleConnection()
        {
            string connect = ConfigurationHelper.GetValue("ConnectionStrings:OracleConnection");
            IDbConnection conn = new OracleConnection(connect);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// 获取发件人邮箱
        /// </summary>
        /// <returns></returns>
        public static string GetUserName()
        {
            string Connect = ConfigurationHelper.GetValue("Emailpost:UserName");
            return Connect;
        }

        /// <summary>
        /// 获取邮件发送服务器地址
        /// </summary>
        /// <returns></returns>
        public static string GetSmtpServer()
        {
            string Connect = ConfigurationHelper.GetValue("Emailpost:SmtpServer");
            return Connect;
        }

        /// <summary>
        /// 获取发件人邮箱密码
        /// </summary>
        /// <returns></returns>
        public static string GetPwd()
        {
            string Connect = ConfigurationHelper.GetValue("Emailpost:Pwd");
            return Connect;
        }
    }
}
