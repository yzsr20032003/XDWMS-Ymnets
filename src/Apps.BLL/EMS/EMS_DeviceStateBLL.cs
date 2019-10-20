using Apps.Common;
using Apps.Models;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System;
using Apps.Models.EMS;

namespace Apps.BLL.EMS
{
    public  partial class EMS_DeviceStateBLL
    {

        public override List<EMS_DeviceStateModel> CreateModelList(ref IQueryable<EMS_DeviceState> queryData)
        {

            List<EMS_DeviceStateModel> modelList = (from r in queryData
                                              select new EMS_DeviceStateModel
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

