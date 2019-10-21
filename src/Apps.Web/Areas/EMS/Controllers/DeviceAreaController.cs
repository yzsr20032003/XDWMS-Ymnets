using System.Collections.Generic;
using System.Linq;
using Apps.Web.Core;
using Apps.IBLL.EMS;
using Apps.Locale;
using System.Web.Mvc;
using Apps.Common;
using Apps.IBLL;
using Apps.Models.EMS;
using Apps.Models.Sys;
using Unity.Attributes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Text;

namespace Apps.Web.Areas.EMS.Controllers
{
    public class DeviceAreaController : BaseController
    {
        [Dependency]
        public IEMS_DeviceAreaBLL m_BLL { get; set; }
        ValidationErrors errors = new ValidationErrors();

        [SupportFilter]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetList(string id)
        {
            if (id == null)
                id = "0";
            List<EMS_DeviceAreaModel> list = m_BLL.GetList(id);
            var json = from r in list
                       select new EMS_DeviceAreaModel()
                       {
                           Id = r.Id,
                           ParentId = r.ParentId,
                           Name = r.Name,
                           CreateTime = r.CreateTime,
                           state = (m_BLL.GetList(r.Id).Count > 0) ? "closed" : "open"
                       };
            return Json(json);
        }

        public JsonResult GetListByComTree(string id)
        {
            List<EMS_DeviceAreaModel> list = m_BLL.GetList(id);
            var json = from r in list
                       select new SysTreeModel()
                       {
                           id = r.Id,
                           text = r.Name,
                           state = (m_BLL.GetList(r.Id).Count > 0) ? "closed" : "open"
                       };


            return Json(json);
        }

        [HttpPost]
        public JsonResult GetListByParentId(string id)
        {
            if (id == null)
                id = "0";
            List<EMS_DeviceAreaModel> list = m_BLL.GetList(id);
            StringBuilder sb = new StringBuilder("");
            foreach (var i in list)
            {
                sb.AppendFormat("<option value='{0}'>{1}</option>", i.Id, i.Name);
            }

            return Json(sb.ToString());
        }

        #region 创建
        [SupportFilter]
        public ActionResult Create(string id)
        {
            EMS_DeviceAreaModel entity = new EMS_DeviceAreaModel()
            {
                ParentId = id,
           
            };
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(EMS_DeviceAreaModel model)
        {
            model.Id = ResultHelper.NewId;
            model.CreateTime = ResultHelper.NowTime;
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Create(ref errors, model))
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
        [SupportFilter]
        public ActionResult Edit(string id)
        {
            EMS_DeviceAreaModel entity = m_BLL.GetById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(EMS_DeviceAreaModel model)
        {
            if (model != null && ModelState.IsValid)
            {

                if (m_BLL.Edit(ref errors, model))
                {
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",ParentId" + model.ParentId, "成功", "修改", "EMS_DeviceArea");
                    return Json(JsonHandler.CreateMessage(1, Resource.EditSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + model.Id + ",ParentId" + model.ParentId + "," + ErrorCol, "失败", "修改", "EMS_DeviceArea");
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
            EMS_DeviceAreaModel entity = m_BLL.GetById(id);
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
                    LogHandler.WriteServiceLog(GetUserId(), "Id:" + id, "成功", "删除", "EMS_DeviceArea");
                    return Json(JsonHandler.CreateMessage(1, Resource.DeleteSucceed));
                }
                else
                {
                    string ErrorCol = errors.Error;
                    LogHandler.WriteServiceLog(GetUserId(), "Id" + id + "," + ErrorCol, "失败", "删除", "EMS_DeviceArea");
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
            var list = new List<EMS_DeviceAreaModel>();
            bool checkResult = m_BLL.CheckImportData(Utils.GetMapPath(filePath), list, ref errors);
            //校验通过直接保存
            if (checkResult)
            {
                m_BLL.SaveImportData(list);
                LogHandler.WriteServiceLog(GetUserId(), "导入成功", "成功", "导入", "EMS_DeviceArea");
                return Json(JsonHandler.CreateMessage(1, Resource.InsertSucceed));
            }
            else
            {
                string ErrorCol = errors.Error;
                LogHandler.WriteServiceLog(GetUserId(), ErrorCol, "失败", "导入", "EMS_DeviceArea");
                return Json(JsonHandler.CreateMessage(0, Resource.InsertFail + ErrorCol));
            }
        }
        [HttpPost]
        [SupportFilter(ActionName = "Export")]
        public JsonResult CheckExportData()
        {
            List<EMS_DeviceAreaModel> list = m_BLL.GetList(ref setNoPagerAscById, "");
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
            List<EMS_DeviceAreaModel> list = m_BLL.GetList(ref setNoPagerAscById, "");
            JArray jObjects = new JArray();
            foreach (var item in list)
            {
                var jo = new JObject();
                jo.Add("Id", item.Id);
                jo.Add("ParentId", item.ParentId);
                jo.Add("Name", item.Name);
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
    }
}
