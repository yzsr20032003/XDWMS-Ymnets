using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.EMS
{
    public partial class EMS_DeviceDetailsModel
    {
        [Display(Name = "类别名称")]
        public string DeviceAreaName { get; set; }
     }
}

