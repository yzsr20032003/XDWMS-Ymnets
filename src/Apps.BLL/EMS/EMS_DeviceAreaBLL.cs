using Apps.Common;
using Apps.Models;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System;
using Apps.Models.EMS;

namespace Apps.BLL.EMS
{
    public  partial class EMS_DeviceAreaBLL
    {

        public List<EMS_DeviceAreaModel> GetList(string parentId)
        {
            IQueryable <EMS_DeviceArea> queryData = null;
            queryData = m_Rep.GetList(a => a.ParentId == parentId).OrderBy(a => a.CreateTime);
            return CreateModelList(ref queryData);
        }
        public override List<EMS_DeviceAreaModel> CreateModelList(ref IQueryable<EMS_DeviceArea> queryData)
        {

            List<EMS_DeviceAreaModel> modelList = (from r in queryData
                                              select new EMS_DeviceAreaModel
                                              {
                                                  Id = r.Id,
                                                  ParentId = r.ParentId,
                                                  Name = r.Name,
                                                  CreateTime = r.CreateTime,
                                              }).ToList();
            return modelList;
        }
    }
 }

