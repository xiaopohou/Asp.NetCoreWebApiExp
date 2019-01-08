using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace TMS_MobileRepository.Entity
{
    ///<summary>
    ///
    ///</summary>
    public partial class RL_TeamMember
    {
           public RL_TeamMember(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string TeamGuid {get;set;}
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>   
        
        public string UserCode { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>   
        [Required]
        public string Name {get;set;}

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>    
        [Required]
        public string Email {get;set;}

        

    }
}
