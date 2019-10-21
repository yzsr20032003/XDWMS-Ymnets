using Apps.Common;
using Apps.Models;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System;
using Apps.Models.EMS;

namespace Apps.BLL.EMS
{
    public  partial class EMS_DeviceTypeBLL
    {

        public override List<EMS_DeviceTypeModel> CreateModelList(ref IQueryable<EMS_DeviceType> queryData)
        {

            List<EMS_DeviceTypeModel> modelList = (from r in queryData
                                              select new EMS_DeviceTypeModel
                                              {
                                                  Id = r.Id,
                                                  Code = r.Code,
                                                  Name = r.Name,
                                                  CreateTime = r.CreateTime,
                                              }).ToList();
            return modelList;
        }
    }
 }

