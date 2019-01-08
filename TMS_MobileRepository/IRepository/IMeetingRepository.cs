using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMS_MobileRepository.Entity;


namespace TMS_MobileRepository.IRepository
{
    public interface IMeetingRepository:IBaseRepository<RL_Meeting>
    {
        /// <summary>
        /// <summary>
        /// 根据状态,UserCode获取会议列表
        /// </summary>
        /// <param name="UserCode">登录人USERCODE</param>
        /// <param name="Status">查询状态（待办传递参数"待开始会议",全部状态为"我发起的会议","历史会议","待开始会议"）</param>
        /// <returns></returns>
        Task<IEnumerable<RL_Meeting>> GetMeetinList( string UserCode, string Status);
        /// <summary>
        ///  会议修改，根据MeetingGuid获取修改页面数据详情（修改整个循环）
        /// </summary>
        /// <param name="MeetingGuid">GUid</param>
        /// <returns></returns>
        Task<RL_Meeting> GetMeetinUpdateDetail(string MeetingGuid);
        /// <summary>
        /// 取消当前会议
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        Task<bool> MeetinCancel(int id);
        /// <summary>
        /// 取消整个循环会议
        /// </summary>
        /// <param name="MeetingGuid">Guid</param>
        /// <returns></returns>
        Task<bool> MeetinCancelAll(string MeetingGuid);

        /// <summary>
        /// 修改所有循环会议内容
        /// </summary>
        /// <param name="entity">会议实体</param>
        /// <returns></returns>
        Task<bool> MeetinUpdate(RL_Meeting entity);



        /// <summary>
        /// 修改会议状态为完成
        /// </summary>
        /// <param name="id">会议ID</param>
        /// <returns></returns>
        Task<bool> GetUpdateRL_MeetingStatus(int id);
        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <param name="Email">收件人邮箱（多个以","分隔）</param>
        /// <param name="entity">邮件内容参数</param>
        /// <param name="EmailType">发送邮件类型（AddMeeting：新建会议,AddMeetinPeople:会议新增人员、UpdataMeeting：修改会议、CancelMeeting：会议取消、FrontMeeting：会议前半小时）</param>
        /// <returns></returns>
        Task<bool> EmailpostAsync( Emailparameter entity, string EmailType);

        Task<int> CreateEntityAsync(RL_Meeting entity);
        Task<IEnumerable<int>> CreateEntityListAsync(IEnumerable<RL_Meeting> entityList);
    }
}
