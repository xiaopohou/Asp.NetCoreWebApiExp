using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using TMS_MobileRepository.Entity.Validation;

namespace TMS_MobileRepository.Entity
{
    ///<summary>
    ///会议
    ///</summary>
    [CustomValidation(typeof(MettingValidation), "MettingCheck")]
    public partial class RL_Meeting
    {
           public RL_Meeting(){


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
           public string MeetingGuid {get;set;}

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>    
        [Required]
        public string MeetingTitle {get;set;}

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>     
        [Required,Range(typeof(int),"0","1")]
        public int? MeetingType {get;set;}

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>   
        
        public string Frequency {get;set;}

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>     
        
        public string Week {get;set;}

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>    
        [Required]
        public DateTime? BeginTime {get;set;}

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
        public string Place {get;set;}

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>     
        [Required]
        public string Participants {get;set;}

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
        public string Status {get;set;}

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>     
        [Required]
        public string Email {get;set;}

        [Required]
        public string ParticipantsCode { get; set; }

        /// <summary>
        /// 会议内容
        /// </summary>
        [Required]
        public string Meetingcontent { get; set; }

        /// <summary>
        /// 修改ID
        /// </summary>
        public string UpdateID { get; set; }

    }
}
