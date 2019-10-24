using System.Collections.Generic;
using System.Linq;
using Apps.Web.Core;
using Apps.IBLL.EMS;
using Apps.Locale;
using System.Web.Mvc;
using Apps.Common;
using Apps.IBLL;
using Apps.Models.EMS;
using Unity.Attributes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Data;

namespace Apps.Web.Areas.EMS.Controllers
{
    public class DeviceDetailsController : BaseController
    {
        [Dependency]
        public IEMS_DeviceDetailsBLL m_BLL { get; set; }
        [Dependency]
        public IEMS_DeviceAreaBLL m_DeviceAreaBLL { get; set; }

        [Dependency]
        public IEMS_DeviceStateBLL m_DeviceStateBll { get; set; }

        [Dependency]
        public IEMS_DeviceTypeBLL m_DeviceTypeBll { get; set; }
        ValidationErrors errors = new ValidationErrors();

        [SupportFilter]
        public ActionResult Index()
        {
            CommonHelper commonHelper = new CommonHelper();
            ViewBag.GetDeviceAreaTree = commonHelper.GetDeviceAreaTree(true);
            return View();
        }
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetList(GridPager pager, string queryStr, string areaId)
        {
            List<EMS_DeviceDetailsModel> list = m_BLL.GetList(ref pager, queryStr, areaId);
            GridRows<EMS_DeviceDetailsModel> grs = new GridRows<EMS_DeviceDetailsModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {
            ViewBag.DeviceArea = new SelectList(m_DeviceAreaBLL.GetList(ref setNoPagerAscById, ""), "Id", "Name");
            ViewBag.DeviceState = new SelectList(m_DeviceStateBll.GetList(ref setNoPagerAscById, ""), "Id", "Name");
            ViewBag.DeviceType = new SelectList(m_DeviceTypeBll.GetList(ref setNoPagerAscById, ""), "Id", "Name");
            return View();
        }
       
        [HttpPost]
        [SupportFilter]
        public JsonResult Create(EMS_DeviceDetailsModel model)
        {
            model.Id = ResultHelper.NewId;
            model.CreateTime = ResultHelper.NowTime;
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",AreaId" + model.AreaId, "成功", "创建", "EMS_DeviceDetails");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",AreaId" + model.AreaId + "," + ErrorCol, "失败", "创建", "EMS_DeviceDetails");
                    return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail));
            }
        }
        #endregion

        #region 修改
        [SupportFilter]
        public ActionResult Edit(string id)
        {
            EMS_DeviceDetailsModel entity = m_BLL.GetById(id);
            ViewBag.DeviceArea = new SelectList(m_DeviceAreaBLL.GetList(ref setNoPagerAscById, ""), "Id", "Name", entity.AreaId);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(EMS_DeviceDetailsModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",AreaId" + model.AreaId, "成功", "修改", "EMS_DeviceDetails");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",AreaId" + model.AreaId + "," + ErrorCol, "失败", "修改", "EMS_DeviceDetails");
                    return Json(JsonHandler.CreateMessage(0, Resource.EditFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.EditFail));
            }
        }
        #endregion

        #region 详细
        [SupportFilter]
        public ActionResult Details(string id)
        {
            EMS_DeviceDetailsModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public ActionResult Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (m_BLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "EMS_DeviceDetails");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "EMS_DeviceDetails");
                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail));
            }
        }
        #endregion
        #region 导出导入
        [HttpPost]
        [SupportFilter]
        public ActionResult Import(string filePath)
        {
            var list = new List<EMS_DeviceDetailsModel>();
            bool checkResult = m_BLL.CheckImportData(Utils.GetMapPath(filePath), list, ref errors);
            //校验通过直接保存
            if (checkResult)
            {
                m_BLL.SaveImportData(list);
                LogHandler.WriteServiceLog(GetUserId(), "导入成功", "成功", "导入", "EMS_DeviceDetails");
                return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
            }
            else
            {
                string ErrorCol = errors.Error;
                LogHandler.WriteServiceLog(GetUserId(), ErrorCol, "失败", "导入", "EMS_DeviceDetails");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));
            }
        }
        [HttpPost]
        [SupportFilter(ActionName = "Export")]
        public JsonResult CheckExportData()
        {
            List<EMS_DeviceDetailsModel> list = m_BLL.GetList(ref setNoPagerAscById, "");
            if (list.Count().Equals(0))
            {
                return Json(JsonHandler.CreateMessage(0, "没有可以导出的数据"));
            }
            else
            {
                return Json(JsonHandler.CreateMessage(1, "可以导出"));
            }
        }
        [SupportFilter]
        public ActionResult Export()
        {
            List<EMS_DeviceDetailsModel> list = m_BLL.GetList(ref setNoPagerAscById, "");
            JArray jObjects = new JArray();
            foreach (var item in list)
            {
                var jo = new JObject();
                jo.Add("Id", item.Id);
                jo.Add("AreaId", item.AreaId);
                jo.Add("ParentID", item.ParentID);
                jo.Add("Code", item.Code);
                jo.Add("Name", item.Name);
                jo.Add("Marking", item.Marking);
                jo.Add("OEM", item.OEM);
                jo.Add("Type", item.Type);
                jo.Add("State", item.State);
                jo.Add("Remark", item.Remark);
                jo.Add("locking", item.locking);
                jo.Add("CreateUser", item.CreateUser);
                jo.Add("CreateTime", item.CreateTime);
                jObjects.Add(jo);
            }
            var dt = JsonConvert.DeserializeObject<DataTable>(jObjects.ToString());
            var exportFileName = string.Concat(
                "File",
                DateTime.Now.ToString("yyyyMMddHHmmss"),
                ".xlsx");
            return new ExportExcelResult
            {
                SheetName = "Sheet1",
                FileName = exportFileName,
                ExportData = dt
            };
        }
        #endregion
        #region 创建
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetListParent(GridPager pager, string queryStr)
        {
            List<EMS_DeviceAreaModel> list = m_DeviceAreaBLL.GetList(ref pager, queryStr);
            GridRows<EMS_DeviceAreaModel> grs = new GridRows<EMS_DeviceAreaModel>();
            grs.rows = list;
            grs.total = pager.totalRows;
            return Json(grs);
        }
        
        [SupportFilter(ActionName = "Create")]
        public ActionResult CreateParent()
        {
            return View();
        }

        [HttpPost]
        [SupportFilter(ActionName = "Create")]
        public JsonResult CreateParent(EMS_DeviceAreaModel model)
        {
            model.Id = ResultHelper.NewId;
            model.CreateTime = ResultHelper.NowTime;
            if (model != null && ModelState.IsValid)
            {

                if (m_DeviceAreaBLL.Create(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",ParentId" + model.ParentId, "成功", "创建", "EMS_DeviceArea");
                    return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",ParentId" + model.ParentId + "," + ErrorCol, "失败", "创建", "EMS_DeviceArea");
                    return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail));
            }
        }
        #endregion

        #region 修改
        [SupportFilter(ActionName = "Edit")]
        public ActionResult EditParent(string id)
        {
            EMS_DeviceAreaModel entity = m_DeviceAreaBLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter(ActionName = "Edit")]
        public JsonResult EditParent(EMS_DeviceAreaModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_DeviceAreaBLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",ParentId" + model.ParentId, "成功", "修改", "EMS_DeviceDetails");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",ParentId" + model.ParentId + "," + ErrorCol, "失败", "修改", "EMS_DeviceDetails");
                    return Json(JsonHandler.CreateMessage(0, Resource.EditFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.EditFail));
            }
        }
        #endregion

        #region 详细
        [SupportFilter(ActionName = "Details")]
        public ActionResult DetailsParent(string id)
        {
            EMS_DeviceAreaModel entity = m_DeviceAreaBLL.GetById(id);
            return View(entity);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter(ActionName = "Delete")]
        public ActionResult DeleteParent(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (m_DeviceAreaBLL.Delete(ref errors, id))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "EMS_DeviceDetails");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "EMS_DeviceDetails");
                    return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail + ErrorCol));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Resource.DeleteFail));
            }
        }
        #endregion
    }
}

