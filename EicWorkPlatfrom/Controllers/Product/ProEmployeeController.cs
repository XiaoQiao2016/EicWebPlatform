﻿using Lm.Eic.App.Business.Bmp.Hrm.Archives;
using Lm.Eic.App.Business.Bmp.Pms.LeaveAsk;
using Lm.Eic.App.Business.Mes.Optical.Authen;
using Lm.Eic.App.DomainModel.Bpm.Hrm.Archives;
//using Lm.Eic.App.DomainModel.Bpm.Pms.LeaveAskManager;
using Lm.Eic.Framework.Authenticate.Business;
using Lm.Eic.Framework.ProductMaster.Business.Config;
using Lm.Eic.Framework.ProductMaster.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EicWorkPlatfrom.Controllers.Product
{
    public class ProEmployeeController : EicBaseController
    {
        //
        // GET: /ProEmployee/

        public ActionResult Index()
        {
            return View();
        }
        #region WorkerInfo
        /// <summary>
        /// 人员信息管理
        /// </summary>
        /// <returns></returns>
        public ActionResult RegistWorkerInfo()
        {
            return View();
        }

        [NoAuthenCheck]
        public JsonResult GetWorkers()
        {
            var workers = AuthenManager.User.GetWorkers();
            LoginUser currentUser = OnLineUser;
            var currentWorker = workers.FirstOrDefault(e => e.WorkerId == currentUser.UserId);
            if (currentWorker != null)
                currentUser.Department = currentWorker.Department;
            var departments = ArchiveService.ArchivesManager.DepartmentMananger.Departments;
            var roles = AuthenService.RoleManager.Roles.Where(e => e.RoleLevel > currentUser.Role.RoleLevel);
            var datas = new { user = currentUser, workers = workers, departments = departments, roles = roles };
            return Json(datas, JsonRequestBehavior.AllowGet);
        }

        [NoAuthenCheck]
        public JsonResult GetWorkerBy(string workerId)
        {
            var workers = ArchiveService.ProWorkerManager.GetWorkerBy(workerId);
            return Json(workers, JsonRequestBehavior.AllowGet);
        }

        [NoAuthenCheck]
        public JsonResult RegistWorker(ProWorkerInfo worker)
        {
            var result = ArchiveService.ProWorkerManager.RegistUser(worker);
            return Json(result);
        }
        #endregion


        #region 考勤管理

        #region 加班管理
        public ActionResult ProWorkOverHoursManage()
        {
            return View();
        }
        #endregion



        #region 请假管理
        public ActionResult ProAskLeaveManage()
        {
            return View();
        }
        /// <summary>
        /// 获取请假类别
        /// </summary>
        /// <returns></returns>
        [NoAuthenCheck]
        public  JsonResult GetLeaveTypesConfigs()

        {
            List<ConfigDataDictionaryModel> leaveConfigTypes = PmConfigService.DataDicManager.LoadConfigDatasBy("AttendanceConfig", "AskForLeaveType");
            return Json(leaveConfigTypes, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [NoAuthenCheck]
        public JsonResult StoreLeaveAskManagerDatas(LeaveAskModels models)
        {
            try
            {
                var opresult = LeaveAskService.LeaveAskManager.StoreLeaveAskDatas(models);
                
                return Json(opresult);
            }
            catch (System.Exception ex)
            {

                throw new Exception(ex.Message);
            }


        }
        
        #endregion

        #endregion
    }
}