using System;
using System.Collections.Generic;
using System.Text;

namespace TMS_MobileRepository.Entity
{
    public class RL_InstitutionResult
    {
       public IEnumerable<RL_Institution> Institutions { get; set; }
       public IEnumerable<RL_OAUser> OAUsers { get; set; }
    }
}
