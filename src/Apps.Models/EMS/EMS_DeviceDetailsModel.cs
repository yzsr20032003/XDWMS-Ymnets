using System;
using System.ComponentModel.DataAnnotations;
using Apps.Models;
namespace Apps.Models.EMS
{
    public partial class EMS_DeviceDetailsModel
    {
        [Display(Name = "区域名称")]
        public string DeviceAreaName { get; set; }
        [Display(Name ="设备类型")]
        public string DeviceTypeName { get; set; }
        [Display(Name ="设备状态")]
        public string DeviceStateName { get; set; }
    }
}

