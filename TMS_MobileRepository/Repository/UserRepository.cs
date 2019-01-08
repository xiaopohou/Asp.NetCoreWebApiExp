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
    /// <summary>
    /// 人员仓储实现测试用
    /// </summary>
    public class UserRepository : IUserRepository
    {
        public Task<bool> AddEntityAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddEntityListAsync(IEnumerable<User> entityList)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using (IDbConnection conn = DataBaseConfig.GetSqlConnection())
            {
               
                string querySql = @"SELECT * FROM UserInfo_Test ";
                return await conn.QueryAsync<User>(querySql);
            }
        }

        public Task<User> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(User entity)
        {
            throw new NotImplementedException();
        }
    }
}
