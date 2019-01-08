using System;
using System.Collections.Generic;
using System.Text;

namespace TMS_MobileRepository.Entity
{
 public   class RL_Teamlist
    {
        public IEnumerable<RL_Team> Team { get; set; }
        public IEnumerable<RL_TeamMember> TeamMembers { get; set; }

        ///// <summary>
        ///// Desc:
        ///// Default:
        ///// Nullable:False
        ///// </summary>           
        //public int ID { get; set; }

        ///// <summary>
        ///// Desc:
        ///// Default:
        ///// Nullable:True
        ///// </summary>   
        //public string TeamName { get; set; }

        ///// <summary>
        ///// Desc:
        ///// Default:
        ///// Nullable:True
        ///// </summary>  
        //public DateTime? CreateDate { get; set; }

        ///// <summary>
        ///// Desc:
        ///// Default:
        ///// Nullable:True
        ///// </summary>   
        //public string CreateBy { get; set; }

        ///// <summary>
        ///// Desc:
        ///// Default:
        ///// Nullable:False
        ///// </summary>           
        //public string TeamGuid { get; set; }

        //public RL_TeamMember TeamMembers { get; set; }
    }

}
