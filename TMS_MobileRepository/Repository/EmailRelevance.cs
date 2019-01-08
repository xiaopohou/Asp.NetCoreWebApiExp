using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using TMS_MobileRepository.Entity;
using TMS_MobileRepository.IRepository;

namespace TMS_MobileRepository.Repository
{
    public class EmailRelevance : IEmailRelevance
    {
        public async Task<bool> AddEntityAsync(RL_EmailRelevance entity)
        {
            using (IDbConnection conn = DataBaseConfig.GetSqlConnection())
            {
                string querySql = @"insert into RL_EmailRelevance values(@JobID,@MissionID,@MettingID,@IsPost)";
                var res = await conn.ExecuteAsync(querySql, entity);
                if (res > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public Task<bool> AddEntityListAsync(IEnumerable<RL_EmailRelevance> entityList)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (IDbConnection conn = DataBaseConfig.GetSqlConnection())
            {
                string querySql = @"Delete RL_EmailRelevance where JobID=@JobID";
                var res = await conn.ExecuteAsync(querySql, new { JobID = id });
                if (res > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public Task<IEnumerable<RL_EmailRelevance>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<RL_EmailRelevance> GetByIdAsync(int id)
        {
            using (IDbConnection conn = DataBaseConfig.GetSqlConnection())
            {
                string querySql = @"select * from RL_EmailRelevance where MissionID=@MissionID";
                var res = await conn.QueryFirstOrDefaultAsync<RL_EmailRelevance>(querySql, new { MissionID=id});
                return res;
            }
        }

        public async Task<bool> UpdateAsync(RL_EmailRelevance entity)
        {
            using (IDbConnection conn = DataBaseConfig.GetSqlConnection())
            {
                string querySql = @"Update RL_EmailRelevance where JobID=@JobID";
                var res = await conn.ExecuteAsync(querySql, new { JobID = entity.JobID });
                if (res > 0)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
