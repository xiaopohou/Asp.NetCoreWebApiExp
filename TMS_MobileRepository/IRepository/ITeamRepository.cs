using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMS_MobileRepository.Entity;

namespace TMS_MobileRepository.IRepository
{
    public interface ITeamRepository:IBaseRepository<RL_TeamRes>
    {
         /// <summary>
         /// 所在团队页加载获取全部团队
         /// </summary>
         /// <returns></returns>
         Task<dynamic> GetTeamList();
        /// <summary>
        /// 获取我创建的团队
        /// </summary>
        /// <param name="UserCode">登录人userCode</param>
        /// <returns></returns>
        Task<dynamic> GetMyCreateTeamList(String UserCode);
        /// <summary>
        /// 获取我加入的团队
        /// </summary>
        /// <param name="UserCode">登录人userCode</param>
        /// <returns></returns>
        Task<dynamic> GetMyJoinTeamList(String UserCode);

        /// <summary>
        ///  团队成员删除
        /// </summary>
        /// <param name="entity">成员主键ID</param>
        /// <returns></returns>
        Task<bool> GetTeamMemberDelete(TeamDeleteId entity);

        /// <summary>
        /// 删除团队及团队下所有成员
        /// </summary>
        /// <param name="Id">团队ID</param>
        /// <returns></returns>
        Task<bool> GetTeamDelete(TeamDeleteId entity);
        /// <summary>
        /// 根据姓名查询数据
        /// </summary>
        /// <param name="Name">姓名</param>
        /// <returns></returns>
        Task<IEnumerable<TeamList>> GetByNameList(String Name);
    }
}
 