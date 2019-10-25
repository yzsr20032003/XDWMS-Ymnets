using Apps.Common;
using Apps.Models;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System;
using Apps.Models.EMS;
using Apps.IBLL.EMS;


namespace Apps.BLL.EMS
{
    public partial class EMS_DeviceDetailsBLL
    {

        public  List<EMS_DeviceDetailsModel> GetList(ref GridPager pager, string queryStr, string AreaId)
        {
            IQueryable<EMS_DeviceDetails> queryData = null;
            string pid = AreaId.ToString();
            if (pid != "0")
            {
                queryData = m_Rep.GetList(a => a.AreaId == pid);
            }
            else
            {
                queryData = m_Rep.GetList();
            }
            if (!string.IsNullOrWhiteSpace(queryStr))
            {
                queryData = m_Rep.GetList(
                            a => (
                                    a.Id.Contains(queryStr)
                                   || a.AreaId.Contains(queryStr)
                                   || a.ParentID.Contains(queryStr)
                                   || a.Code.Contains(queryStr)
                                   || a.Name.Contains(queryStr)
                                   || a.Marking.Contains(queryStr)
                                   || a.OEM.Contains(queryStr)
                                   || a.Remark.Contains(queryStr)
                                   || a.CreateUser.Contains(queryStr)
                                   ||a.EMS_DeviceArea.Name.Contains(queryStr)
                                   ||a.EMS_DeviceState.Name.Contains(queryStr)
                                   ||a.EMS_DeviceType.Name.Contains(queryStr)
                                 )
                            );
            }
            pager.totalRows = queryData.Count();
            queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
            return CreateModelList(ref queryData);
        }
        public override List<EMS_DeviceDetailsModel> CreateModelList(ref IQueryable<EMS_DeviceDetails> queryData)
        {

            List<EMS_DeviceDetailsModel> modelList = (from r in queryData
                                                      select new EMS_DeviceDetailsModel
                                                      {
                                                          Id = r.Id,
                                                          AreaId = r.AreaId,
                                                          ParentID = r.ParentID,
                                                          Code = r.Code,
                                                          Name = r.Name,
                                                          Marking = r.Marking,
                                                          OEM = r.OEM,
                                                          Type = r.Type,
                                                          State = r.State,
                                                          Remark = r.Remark,
                                                          locking = r.locking,
                                                          CreateUser = r.CreateUser,
                                                          CreateTime = r.CreateTime,
                                                          DeviceAreaName = r.EMS_DeviceArea.Name,
                                                          DeviceStateName =r.EMS_DeviceState.Name,
                                                          DeviceTypeName = r.EMS_DeviceType.Name,
                                                      }).ToList();
            return modelList;
        }
    }
}

