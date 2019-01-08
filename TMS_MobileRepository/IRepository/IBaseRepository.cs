using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TMS_MobileRepository.IRepository
{
    /// <summary>
    /// 2018/12/11 zhong
    /// 基础数据仓储接口(CRUD)
    /// </summary>
    public interface IBaseRepository<T> where T:class
    {
        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> AddEntityAsync(T entity);

        /// <summary>
        /// 批量添加实体
        /// </summary>
        /// <param name="entityList"></param>
        /// <returns></returns>
        Task<bool> AddEntityListAsync(IEnumerable<T> entityList);

        /// <summary>
        /// 根据主键Id获取一个实体
        /// </summary>
        /// <param name="id">主键Id</param>
        /// <returns></returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync();


        /// <summary>
        /// 修改一个实体
        /// </summary>
        /// <param name="entity">要修改的实体</param>
        /// <returns></returns>
        Task<bool> UpdateAsync(T entity);

        /// <summary>
        /// 根据主键Id删除一个实体
        /// </summary>
        /// <param name="id">主键Id</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(int id);
        
    }
}
