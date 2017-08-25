﻿using Lm.Eic.App.DomainModel.Bpm.Quanity;
using Lm.Eic.App.Erp.Bussiness.QuantityManage;
using Lm.Eic.App.Erp.Domain.QuantityModel;
using Lm.Eic.Uti.Common.YleeOOMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lm.Eic.App.Business.Bmp.Quality.InspectionManage
{
    public class InspectionFqcFormManager
    {
        public List<InspectionFqcMasterModel> GetInspectionFormManagerListBy(string formStatus, DateTime dateFrom, DateTime dateTo)
        {
            //查询ERP中所有物料和单号 
            var list = InspectionManagerCrudFactory.FqcMasterCrud.GetFqcInspectionMasterModelListBy(formStatus, dateFrom, dateTo);

            switch (formStatus)
            {
                case "待检测":
                    return null;
                case "未完成":
                    return list.Where(e => e.InspectionResult == "未完成").ToList();
                case "全部":
                    return GetERPOrderAndMaterialBy(dateFrom, dateTo, "MS7");
                case "待审核":
                    return list.Where(e => e.InspectionStatus == "待审核").ToList();
                case "已审核":
                    return list.Where(e => e.InspectionStatus == "已审核").ToList();
                default:
                    return new List<InspectionFqcMasterModel>();
            }


        }

        public List<InspectionFqcDetailModel> GetInspectionDatailListBy(string orderId, int orderIdNumber)
        {
            return InspectionManagerCrudFactory.FqcDetailCrud.GetFqcInspectionDetailDatasBy(orderId, orderIdNumber);
        }
        /// <summary>
        ///审核主表数据
        /// </summary>
        /// <returns></returns>
        public OpResult AuditFqcInspectionMasterModel(InspectionFqcMasterModel model)
        {
            try
            {
                if (model == null) return OpResult.SetErrorResult("FQC主表不能为空"); ;
                //先改变主表的状态
                var retrunResult = InspectionManagerCrudFactory.FqcMasterCrud.Store(model, true);
                if (!retrunResult.Result) return OpResult.SetErrorResult("FQC主表审核状态更新失败");
                //主要更新成功 再   更新详细表的信息
                retrunResult = InspectionManagerCrudFactory.FqcMasterCrud.UpAuditDetailData(model.OrderId, model.OrderIdNumber, "Done");
                if (!retrunResult.Result) return OpResult.SetErrorResult("FQC详细表审核状态更新失败");
                return retrunResult;
            }
            catch (Exception ex)
            {
                return OpResult.SetErrorResult(ex.Message);
                throw new Exception(ex.Message);
            }

        }

        List<InspectionFqcMasterModel> GetERPOrderAndMaterialBy(DateTime startTime, DateTime endTime, string department)
        {
            List<InspectionFqcMasterModel> retrunList = new List<InspectionFqcMasterModel>();
            var OrderIdList = GetOrderIdList(startTime, endTime, department);
            if (OrderIdList == null || OrderIdList.Count <= 0) return retrunList;
            OrderIdList.ForEach(e =>
            {
                retrunList.Add(MaterialModelToInspectionFqcMasterModel(e));
            });
            return retrunList.OrderByDescending(e => e.MaterialInDate).ToList();
        }


        InspectionFqcMasterModel MaterialModelToInspectionFqcMasterModel(MaterialModel model)
        {
            return model != null ? (new InspectionFqcMasterModel()
            {
                OrderId = model.OrderID,
                MaterialName = model.ProductName,
                MaterialSpec = model.ProductStandard,
                MaterialSupplier = model.ProductSupplier,
                MaterialDrawId = model.ProductDrawID,
                MaterialId = model.ProductID,
                InspectionStatus = "未完成",
                MaterialCount = model.ProduceNumber,
                MaterialInDate = model.ProduceInDate,
                InspectionResult = string.Empty,
                InspectionItems = "还没有抽检",
                InspectionMode = "正常"
            }) : new InspectionFqcMasterModel();
        }
        List<MaterialModel> GetOrderIdList(DateTime starDate, DateTime endDate, string department)
        {
            return QualityDBManager.OrderIdInpectionDb.FindErpAllMasterilBy(starDate, endDate, department);
        }
    }
}
