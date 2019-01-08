using System;
using System.Collections.Generic;
using System.Text;

namespace TMS_MobileRepository.Entity
{
     public  class RL_TeamRes
    {
        public RL_Team Team { get; set; }
        public IEnumerable<RL_TeamMember> TeamMembers { get; set; }
    }
}
