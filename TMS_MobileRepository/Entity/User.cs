using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TMS_MobileRepository.Entity
{
    public class User
    {
        [Required]
        public string UserId { get; set; }
        [Display(Name = "姓名"), Required, MaxLength(5, ErrorMessage = "{0}长度不可超过{1}")]
        public string UserName { get; set; }
        [Display(Name = "性别"), Required, Range(typeof(string), "男", "女")]
        public string Sex { get; set; }
        [Display(Name = "年龄"), Required, Range(typeof(int), "20", "60")]
        public int Age { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}
