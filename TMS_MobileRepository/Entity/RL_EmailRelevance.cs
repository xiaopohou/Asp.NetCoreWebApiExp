using System;
using System.Collections.Generic;
using System.Text;

namespace TMS_MobileRepository.Entity
{
    /// <summary>
    /// 邮件JobID与任务，会议ID关联表
    /// </summary>
    public class RL_EmailRelevance
    {
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int ID { get; set; }

        public int JobID { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        public int MissionID { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary> 
        public int MettingID { get; set; }
        public int IsPost { get; set; }
    }
}
