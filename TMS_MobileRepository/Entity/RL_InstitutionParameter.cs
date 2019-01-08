using System;
using System.Collections.Generic;
using System.Text;

namespace TMS_MobileRepository.Entity
{
    /// <summary>
    /// 机构树参数
    /// </summary>
    public class RL_InstitutionParameter
    {
       /// <summary>
       /// 机构编码
       /// </summary>
       public string DEPT_CODE { get; set; }
       /// <summary>
       /// 机构级别
       /// </summary>
       public int DEPT_LEVEL { get; set; }
       //public int StartIndex { get; set; }
       //public int EndIndex { get; set; }
    }
}
