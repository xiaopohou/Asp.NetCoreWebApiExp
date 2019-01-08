using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS_MobileRepository.Entity;
using TMS_MobileRepository.IRepository;

namespace TMS_MobileRepository.Repository
{
    public class TeamRepository : ITeamRepository
    {
        private readonly ILogger<TeamRepository> _logger = null;
        public TeamRepository(ILogger<TeamRepository> logger)
        {
            this._logger = logger;
        }
        public async Task<bool> AddEntityAsync(RL_TeamRes entity)
        {
            using (IDbConnection conn = DataBaseConfig.GetSqlConnection())
            {
                string teamGuid = Guid.NewGuid().ToString();
                RL_Team rL_Team = entity.Team;
                List<RL_TeamMember> teamMembers = entity.TeamMembers.AsList();
                rL_Team.TeamGuid = teamGuid;
                for (int i = 0; i < teamMembers.Count; i++)
                {
                    teamMembers[i].TeamGuid = teamGuid;
                }
                IDbTransaction transaction = conn.BeginTransaction();
                try
                {
                    string querySql = @"insert into RL_Team values(@TeamName,@CreateDate,@CreateBy,@TeamGuid)";
                    string querySql1 = @"insert into RL_TeamMember values(@TeamGuid,@UserCode,@Name,@Email)";
                    var res = await conn.ExecuteAsync(querySql, rL_Team,transaction);
                    var res1 = await conn.ExecuteAsync(querySql1, teamMembers, transaction);
                    transaction.Commit();
                    if (res > 0 && res1 > 0)
                    {
                        return true;
                    }
                    return false;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public Task<bool> AddEntityListAsync(IEnumerable<RL_TeamRes> entityList)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RL_TeamRes>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<RL_TeamRes> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(RL_TeamRes entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 所在团队页加载获取全部团队
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetTeamList()
        {
            using (IDbConnection con=DataBaseConfig.GetSqlConnection())
            {
                RL_Teamlist rl = new RL_Teamlist();
                string sql = @"select * from RL_team";
                string sqls = @"select * from RL_TeamMember  where TeamGuid in(select TeamGuid from RL_Team )";
                //  var data = con.QueryAsync<RL_TeamRes, RL_Team, RL_TeamRes>(sql, (RL_TeamRes, RL_Teams) => { RL_TeamRes.Team = RL_Teams; return RL_TeamRes; });
                // var data =con.QueryAsync<RL_Teamlist, RL_Team,RL_TeamMember, RL_Teamlist>(sql, (RL_Teamlist, RL_Team, RL_TeamMember) => { RL_Teamlist.RL_Team = RL_Team; RL_Teamlist.teammember = RL_TeamMember; ; return RL_Teamlist; });
                var data = await con.QueryAsync<RL_Team>(sql);//con.QueryAsync<RL_Teamlist, RL_TeamMember, RL_Teamlist>(sql, (RL_Teamlist, RL_TeamMember) => { RL_Teamlist.TeamMembers = RL_TeamMember; return RL_Teamlist; });
                var datas = await con.QueryAsync<RL_TeamMember>(sqls);
                rl.Team = data;
                rl.TeamMembers = datas;
                return rl;
            }
        }

        /// <summary>
        /// 获取我创建的团队
        /// </summary>
        /// <param name="UserCode">登录人userCode</param>
        /// <returns></returns>
        public async Task<dynamic> GetMyCreateTeamList(String UserCode)
        {
            using (IDbConnection conn=DataBaseConfig.GetSqlConnection())
            {
                RL_Teamlist rl = new RL_Teamlist();
                string sql = @"select * from RL_team as team  where team.CreateBy =@UserCode";
                //  var data = conn.QueryAsync<RL_Teamlist, RL_TeamMember, RL_Teamlist>(sql, (RL_Teamlist, RL_TeamMember) => { RL_Teamlist.TeamMembers = RL_TeamMember; return RL_Teamlist; });
                string sqls = @"select * from RL_TeamMember  where TeamGuid in(select TeamGuid from RL_Team where CreateBy=@UserCode)";
                rl.Team = await conn.QueryAsync<RL_Team>(sql,new { UserCode = UserCode });
                rl.TeamMembers = await conn.QueryAsync<RL_TeamMember>(sqls,new { UserCode = UserCode });
                return  rl;
            }
        }

        /// <summary>
        /// 获取我加入的团队
        /// </summary>
        /// <param name="UserCode">登录人userCode</param>
        /// <returns></returns>
        public async Task<dynamic> GetMyJoinTeamList(String UserCode)
        {
            using (IDbConnection conn=DataBaseConfig.GetSqlConnection())
            {
                RL_Teamlist rl = new RL_Teamlist();
                string sql = @"select * from RL_TeamMember where TeamGuid in(select TeamGuid from RL_Team where TeamGuid in(select TeamGuid from RL_TeamMember where UserCode=@UserCode))";
                string sqls = @"select * from RL_Team where TeamGuid in(select TeamGuid from RL_TeamMember where UserCode=@UserCode)";
                var data = await conn.QueryAsync<RL_Team>(sqls,new { UserCode = UserCode });//conn.QueryAsync<RL_Teamlist, RL_TeamMember, RL_Teamlist>(sql, (RL_Teamlist, RL_TeamMember) => { RL_Teamlist.TeamMembers = RL_TeamMember; return RL_Teamlist; });
                var datas = await conn.QueryAsync<RL_TeamMember>(sql, new { UserCode = UserCode });
                rl.Team = data;
                rl.TeamMembers = datas;
                return rl;
            }
        }
        /// <summary>
        /// 团队成员删除
        /// </summary>
        /// <param name="entity">成员主键ID</param>
        /// <returns></returns>
        public async Task<bool> GetTeamMemberDelete(TeamDeleteId entity)
        {
            using (IDbConnection con=DataBaseConfig.GetSqlConnection())
            {
                string sql = @"delete RL_TeamMember where ID in("+ entity.ID +")";
                var rel = await con.ExecuteAsync(sql);
                if (rel>0)
                {
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 根据姓名查询数据
        /// </summary>
        /// <param name="Name">姓名</param>
        /// <returns></returns>
        public async Task<IEnumerable<TeamList>> GetByNameList(String Name)
        {
            using (IDbConnection con=DataBaseConfig.GetSqlConnection())
            {
                RL_Teamlist rl = new RL_Teamlist();
                string sql = "select * from RL_TeamMember rltm left join RL_Team rlt on rltm.TeamGuid=rlt.TeamGuid  where Name like @Name";
                return await con.QueryAsync<TeamList>(sql, new { Name = "%"+Name+"%" });
                 

            }
        }

        /// <summary>
        /// 删除团队及团队下所有成员
        /// </summary>
        /// <param name="entity">团队ID</param>
        /// <returns></returns>
        public async Task<bool> GetTeamDelete(TeamDeleteId entity)
        {
            using (IDbConnection con=DataBaseConfig.GetSqlConnection())
            {
                string sql = @"delete RL_Team where ID in(" + entity.ID + ")";
                string sqles = "select * from RL_TeamMember where TeamGuid in(select TeamGuid from RL_Team where ID in(" + entity.ID + "))";
                string sqls = @"delete RL_TeamMember where TeamGuid in(select TeamGuid from RL_Team where ID in(" + entity.ID + "))";
                var sel = await con.QueryAsync<RL_TeamMember>(sqles);

                IDbTransaction transaction = con.BeginTransaction();
                try
                {
                    if(sel.Count()>0)
                    {
                        var rels = await con.ExecuteAsync(sqls,entity.ID=entity.ID , transaction);
                        var rel = await con.ExecuteAsync(sql,entity.ID=entity.ID, transaction);
                        transaction.Commit();
                        if (rel > 0 && rels > 0)
                        {
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        var rel = await con.ExecuteAsync(sql, transaction);
                        transaction.Commit();
                        if (rel > 0)
                        {
                            return true;
                        }
                        return false;
                    }


                }
                catch (Exception e)
                {
                    transaction.Rollback();

                    return false;
                }
            }
        }
    }
}
