using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMS_MobileRepository.Entity;

namespace TMS_MobileRepository.IRepository
{
    public interface IMissionRepository:IBaseRepository<RL_MainItem>
    {
        /// <summary>
        /// 根据条件获取取全部数据
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<IEnumerable<RL_MainItem>> GetMissionBystatus(string status,string UserCode);

        /// <summary>
        /// 机构数获取
        /// </summary>
        /// <param name="Dept_Code">主键</param>
        /// <param name="Odept_Code">下一级</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="EndIndex">结束位置</param>
        /// <returns></returns>
        Task<dynamic> GetDynamicsAsync(RL_InstitutionParameter rL_Institution);


        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <param name="Email">收件人邮箱（多个以","分隔）</param>
        /// <param name="entity">邮件内容参数</param>
        /// <param name="EmailType">发送邮件类型（AddMainItem：新建任务、EndMainItem：任务截止8小时）</param>
        /// <returns></returns>
        Task<bool> EmailpostAsync(string Email, RL_MainItem entity, string EmailType);

        Task<IEnumerable<dynamic>> GetOaUserByEntryAsync(string entry, int page, int rows);

        Task<int> CreateEntityAsync(RL_MainItem entity);

        /// <summary>
        /// 修改任务状态
        /// </summary>
        /// <param name="Id">任务ID</param>
        /// <returns></returns>
        Task<bool> GetUpdataMissonStatus(int Id);

        Task<dynamic> GetUserInfo(string UserCode);

    }
}
