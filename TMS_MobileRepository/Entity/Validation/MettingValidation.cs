using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TMS_MobileRepository.Entity;

namespace TMS_MobileRepository.Entity.Validation
{
    /// <summary>
    /// 2018/12/13 zhong
    /// 会议自定义类验证
    /// </summary>
    public class MettingValidation
    {

        public static ValidationResult MettingCheck(object value, ValidationContext validationContext)
        {
            RL_Meeting rL_Meeting = new RL_Meeting();
            if (validationContext.ObjectInstance.ToString() == "TMS_MobileRepository.Entity.Emailparameter")
            {
                Emailparameter emailparameters = (Emailparameter)validationContext.ObjectInstance;
                rL_Meeting = emailparameters.Meeting;
            }
            else
            {
                 rL_Meeting = (RL_Meeting)validationContext.ObjectInstance;

            }
            if (rL_Meeting.Frequency != "每周" && rL_Meeting.Frequency != "每双周" && rL_Meeting.Frequency != "每四周"&&rL_Meeting.MeetingType==1)
            {
                return new ValidationResult("Frequency Must in '每周'、'每双周'、'每四周'");
            } else if (rL_Meeting.Week != "周一" && rL_Meeting.Week != "周二" && rL_Meeting.Week != "周三" && rL_Meeting.Week != "周四"
                && rL_Meeting.Week != "周五"&&rL_Meeting.MeetingType == 1)
            {
                return new ValidationResult("Week Must in '周一'、'周二'、'周三'、'周四'、'周五'");
            }
            else if (Convert.ToDateTime(rL_Meeting.BeginTime)>=Convert.ToDateTime(rL_Meeting.EndTime)|| 
                Convert.ToDateTime(rL_Meeting.BeginTime)<DateTime.Now||
                Convert.ToDateTime(Convert.ToDateTime(rL_Meeting.BeginTime).ToString("HH:mm:ss")) >=
                Convert.ToDateTime(Convert.ToDateTime(rL_Meeting.EndTime).ToString("HH:mm:ss")))
            {
                return new ValidationResult("The BeginTime and EndTime is irrational ");
            }
            return ValidationResult.Success;
        }
    }
}
