using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace TMS_MobileRepository.Entity
{
    ///<summary>
    ///
    ///</summary>
    public partial class RL_MainItem
    {
           public RL_MainItem(){


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
           public string TaskName {get;set;}

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary> 
        [Required]
        public string MissionName {get;set;}

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary> 
        [Required]
        public string ProjectName {get;set;}

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary> 
        [Required]
        public string EditionNum {get;set;}

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>   
        [Required]
        public string ProjectType {get;set;}

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>    
        [Required]
        public string Executor {get;set;}

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>    
        [Required]
        public DateTime? EndTime {get;set;}

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
           /// Nullable:True
           /// </summary>   
        [Required]
        public string status {get;set;}

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>  
        [Required]
        public string UserCode {get;set;}

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>    
        [Required]
        public string Email {get;set;}

    }
}
