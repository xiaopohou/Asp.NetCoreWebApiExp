using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace TMS_MobileRepository.Entity
{
    ///<summary>
    ///
    ///</summary>
    public partial class RL_Team
    {
           public RL_Team(){


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
        /// Nullable:True
        /// </summary>   
        [Required]
        public string TeamName {get;set;}

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>  
        [Required]
        public DateTime? CreateDate {get;set;}

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>   
        [Required]
        public string CreateBy {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string TeamGuid {get;set;}


    }
}
