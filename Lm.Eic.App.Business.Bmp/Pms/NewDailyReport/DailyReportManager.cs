﻿using Lm.Eic.App.DomainModel.Bpm.Pms.NewDailyReport;
using Lm.Eic.Uti.Common.YleeObjectBuilder;
using Lm.Eic.Uti.Common.YleeOOMapper;
using System.Collections.Generic;
using Lm.Eic.App.Erp.Bussiness.QuantityManage;
using Lm.Eic.App.Erp.Domain.QuantityModel;
using System;
using Lm.Eic.Uti.Common.YleeExtension.FileOperation;
using System.Linq;
using Lm.Eic.Uti.Common.YleeExtension.Conversion;
using Lm.Eic.App.Business.Bmp.Pms.NewDailyReport.DailyReportConfig;

namespace Lm.Eic.App.Business.Bmp.Pms.NewDailyReport
{
    public class DailyProductionReportManager
    {
        /// <summary>
        /// 产品生产工艺设置
        /// </summary>
        public StandardProductionFlowManager ProductionFlowSet
        {
            get { return OBulider.BuildInstance<StandardProductionFlowManager>(); }
        }
        /// <summary>
        /// 生产日报订单分配管理
        /// </summary>
        public ProductOrderDispatchManager ProductOrderDispatch
        {
            get { return OBulider.BuildInstance<ProductOrderDispatchManager>(); }
        }
        public DailyReportManager DailyReport
        {
            get { return OBulider.BuildInstance<DailyReportManager>(); }
        }
        /// <summary>
        /// 日报表生产编码管理
        /// </summary>
        public DailyProductionCodeConfigManager DailyProductionCodeConfig
        {
            get { return OBulider.BuildInstance<DailyProductionCodeConfigManager>(); }
        }
    }
    public class ProductOrderDispatchManager
    {
        /// <summary>
        /// 获得当前已经分配的订单
        /// </summary>
        /// <param name="department">部门</param>
        /// <returns></returns>

        public List<ProductOrderDispatchModel> GetHaveDispatchOrderBy(string department, string dicpatchStatus)
        {
            return DailyReportCrudFactory.ProductOrderDispatch.GetHaveDispatchOrderBy(department, dicpatchStatus);
        }
        public List<ProductOrderDispatchModel> GetVirtualOrderDataBy(string department)
        {
            return DailyReportCrudFactory.ProductOrderDispatch.GetVirtualOrderDataBy(department);
        }
        public List<ProductOrderDispatchModel> GetNeedDispatchOrderBy(string department)
        {
            DateTime getDate =DateTime.Now.Date.ToDate();
            List<ProductOrderDispatchModel> returndatas = new List<ProductOrderDispatchModel>();
            ProductOrderDispatchModel model = null;
            ///ERP在制生产订单
            var erpInProductiondatas = QualityDBManager.OrderIdInpectionDb.GetProductionOrderIdInfoBy(department, "在制");
            ///今日生产已确认分配的订单

            if (erpInProductiondatas == null || erpInProductiondatas.Count == 0) return returndatas;
            erpInProductiondatas.ForEach(e =>
            {
                model = new ProductOrderDispatchModel();
                OOMaper.Mapper<ProductionOrderIdInfo, ProductOrderDispatchModel>(e, model);
                ////如果订单在数据库中不存在
                var haveDispatchOrder = DailyReportCrudFactory.ProductOrderDispatch.GetOrderInfoBy(e.OrderId);
                if (haveDispatchOrder == null)
                {
                    model.ProductionDate = e.PlanStartProductionDate;
                    model.ValidDate = e.PlanEndProductionDate;
                    model.DicpatchStatus = "未分配";
                    model.IsValid = "false";
                }
                else
                {
                    model = haveDispatchOrder;
                    if (model.ValidDate <= getDate)
                    {
                        model.DicpatchStatus = "已失效";
                    }
                    model.ProductStatus = e.ProductStatus;
                }
                if (!returndatas.Contains(model))
                    returndatas.Add(model);
            });
            TreatmentNoProductOrderId(department, returndatas);
            return returndatas.OrderByDescending(e => e.OrderId).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public List<ProductOrderDispatchModel> GetQueryOrderBy(string orderId)
        {
        
            List<ProductOrderDispatchModel> returndatas = new List<ProductOrderDispatchModel>();
            ProductOrderDispatchModel model = null;
            ///ERP在制生产订单
            var erpInProductiondatas = QualityDBManager.OrderIdInpectionDb.GetProductionOrderIdInfoBy(orderId);
            ///今日生产已确认分配的订单

            if (erpInProductiondatas == null || erpInProductiondatas.Count == 0) return returndatas;
            erpInProductiondatas.ForEach(e =>
            {
                model = new ProductOrderDispatchModel();
                OOMaper.Mapper<ProductionOrderIdInfo, ProductOrderDispatchModel>(e, model);
                ////如果订单在数据库中不存在
                var haveDispatchOrder = DailyReportCrudFactory.ProductOrderDispatch.GetOrderInfoBy(e.OrderId);
                if (haveDispatchOrder == null)
                {
                    model.ProductionDate = e.PlanStartProductionDate;
                    model.ValidDate = e.PlanEndProductionDate;
                    model.DicpatchStatus = "未分配";
                    model.IsValid = "false";
                }
                else model = haveDispatchOrder;
                
                if (!returndatas.Contains(model))
                    returndatas.Add(model);
            });
            return returndatas.OrderByDescending(e => e.OrderId).ToList();
        }
        /// <summary>
        /// 处理已经分配的但是完工的订单
        /// </summary>
        /// <param name="todayHaveDispatchProductionOrderDatas"></param>
        /// <param name="erpInProductiondatas"></param>
        public void TreatmentNoProductOrderId(string department, List<ProductOrderDispatchModel> erpInProductiondatas)
        {
            var todayHaveDispatchProductionOrderDatas = DailyReportCrudFactory.ProductOrderDispatch.GetHaveDispatchOrderBy(department);
            if (todayHaveDispatchProductionOrderDatas == null || todayHaveDispatchProductionOrderDatas.Count == 0) return;
            todayHaveDispatchProductionOrderDatas.ForEach(e =>
            {
                ///如果工单不在ERP中的在制工单中 那么此分配的工单失效 状态变为已经完工 如果是虚拟工单不用改变
                var dates = erpInProductiondatas.FirstOrDefault(f => f.OrderId == e.OrderId && e.IsVirtualOrderId == 0);
                if (dates == null)
                {
                    e.IsValid = "false";
                    e.ProductStatus = "已完工";
                    e.OpSign = OpMode.Edit;
                    StoreOrderDispatchData(e);
                }
            });
        }
        /// <summary>
        /// 保存订单数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public OpResult StoreOrderDispatchData(ProductOrderDispatchModel model)
        {
            return DailyReportCrudFactory.ProductOrderDispatch.Store(model, true);
        }
    }

    public class DailyReportManager
    {
        public OpResult StoreDailyReport(DailyProductionReportModel model)
        {
            return DailyReportCrudFactory.DailyProductionReport.Store(model, true);
        }
        public OpResult StoreDailyReport(List<DailyProductionReportModel> DailyReportList)
        {
            return DailyReportCrudFactory.DailyProductionReport.SavaDailyReportList(DailyReportList);
        }
        public OpResult StoreDailyReport(DailyProductionReportModel model, List<UserInfoVm> groupUserInfos, out List<DailyProductionReportModel> storeListDatas)
        {
            List<DailyProductionReportModel> DailyReportList = new List<DailyProductionReportModel>();
            if (model != null)
            {
                DailyProductionReportModel newModel = null;
                double sumProductCount = model.TodayProductionCount;
                double sumProductBadCount = model.TodayBadProductCount;
                if (groupUserInfos == null || groupUserInfos.Count == 0)
                {
                    storeListDatas = null;
                    return OpResult.SetErrorResult("人员数据为空，操作失败!");
                }
                double sumWorkerProductionTime = groupUserInfos.Sum(e => e.WorkerProductionTime);
                ///总工时不能为空
                if (sumWorkerProductionTime == 0)
                {
                    storeListDatas = null;
                    return OpResult.SetErrorResult("人员统计的工时为空，操作失败!");
                }
                ///对多人信息进行分摊
                groupUserInfos.ForEach(m =>
                {
                    newModel = new DailyProductionReportModel();
                    OOMaper.Mapper<DailyProductionReportModel, DailyProductionReportModel>(model, newModel);
                    OOMaper.Mapper<UserInfoVm, DailyProductionReportModel>(m, newModel);
                    newModel.TodayProductionCount = Math.Round((sumProductCount / sumWorkerProductionTime) * m.WorkerProductionTime, 2, MidpointRounding.AwayFromZero);
                    //不良数也按工时分分摊
                    newModel.TodayBadProductCount = Math.Round((sumProductBadCount / sumWorkerProductionTime) * m.WorkerProductionTime, 2, MidpointRounding.AwayFromZero);
                    if (!DailyReportList.Contains(newModel))
                        DailyReportList.Add(newModel);
                });
            }
            OpResult resultDatas = DailyReportCrudFactory.DailyProductionReport.SavaDailyReportList(DailyReportList);
            storeListDatas = DailyReportList;
            return resultDatas;
        }

        /// <summary>
        /// 处理机台输入分摊日报表数据返回
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="disposeSign"></param>
        /// <returns></returns>
        public List<DailyProductionReportModel> DisposeDailyReportdMachineDatas(List<DailyProductionReportModel> datas, double workerLookMachineSumTime,double workerNoProductionSumTime)
        {
            List<DailyProductionReportModel> DailyReportList = new List<DailyProductionReportModel>();
            DailyProductionReportModel newModel = null;
            if (datas != null)
            {
                double machineRunSumTime = datas.Sum(e => e.MachineProductionTime);
                
                // 人员投入工时 分摊到每个工单上  依据 出勤时数=（人员工时/ 机台生产总时间）* 当前机台生产时间
                // WorkerProductionTime= （WorkerProductionTime/ MachineProductionTimeSum） *MachineProductionTime
                datas.ForEach(e =>
                {
                    newModel = new DailyProductionReportModel();
                    OOMaper.Mapper<DailyProductionReportModel, DailyProductionReportModel>(e, newModel);
                    newModel.InPutDate = e.InPutDate.ToDate();
                    ///人工生产产量 ==机台生产产量
                    newModel.TodayProductionCount = e.MachineProductionCount;
                    /// 人工不良产量= 机台不良产量
                    newModel.TodayBadProductCount = e.MachineProductionBadCount;
                    ///计算人员的标准工时
                   if(e.MachineProductionCount!=0&& e.StandardProductionTime!=0)
                        newModel.GetProductionTime = Math.Round(e.MachineProductionCount * e.StandardProductionTime / 3600, 2);
                    /// 机台录入分摊人工工时
                    if (machineRunSumTime != 0)
                    {
                        newModel.WorkerProductionTime = Math.Round((workerLookMachineSumTime / machineRunSumTime) * e.MachineProductionTime, 2);
                        newModel.WorkerNoProductionTime = Math.Round((workerNoProductionSumTime / machineRunSumTime) * e.MachineProductionTime, 2);
                    }
                    newModel.MachineUnproductiveTime = e.MachineSetProductionTime - e.MachineProductionTime;    
                    if (!DailyReportList.Contains(newModel))
                        DailyReportList.Add(newModel);
                });
            }
            return DailyReportList;
        }
        public List<ProductFlowCountDatasVm> GetProductionFlowCountDatas(string department, string orderId, string productName)
        {
            List<ProductFlowCountDatasVm> retrundatas = new List<ProductFlowCountDatasVm>();
            ProductFlowCountDatasVm modelVm = null;
            var datas = DailyProductionReportService.ProductionConfigManager.ProductionFlowSet.GetProductFlowInfoBy(new QueryDailyProductReportDto()
            { Department = department, ProductName = productName, SearchMode = 2 });
            if (datas == null || datas.Count == 0) return retrundatas;
            datas.ForEach(e =>
            {
                double orderHavePutInNumber = DailyReportCrudFactory.DailyProductionReport.GetDailyProductionCountBy(orderId, e.ProcessesName);
                modelVm = new ProductFlowCountDatasVm()
                { OrderId = orderId, OrderHavePutInNumber = orderHavePutInNumber };
                OOMaper.Mapper<StandardProductionFlowModel, ProductFlowCountDatasVm>(e, modelVm);
                var orderInfo = DailyReportCrudFactory.ProductOrderDispatch.GetOrderInfoBy(orderId);
                if (orderInfo != null)
                {
                    modelVm.ProductName = orderInfo.ProductName;
                    modelVm.OrderProductNumber = Math.Round(orderInfo.ProduceNumber, 2);
                    modelVm.OrderNeedPutInNumber = orderInfo.ProduceNumber * e.ProductCoefficient;
                }
                if (!retrundatas.Contains(modelVm))
                    retrundatas.Add(modelVm);
            });
            return retrundatas;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="orderId"></param>
        /// <param name="processesName"></param>
        /// <returns></returns>
        public List<DailyProductionReportModel> GetDailyDataBy(DateTime date, string orderId, string processesName)
        {
            return DailyReportCrudFactory.DailyProductionReport.GetDailyDatasBy(date, orderId, processesName);
        }
        public DailyProductionReportModel GetWorkerDailyDatasBy(string workerId)
        {
            var datas = DailyReportCrudFactory.DailyProductionReport.GetWorkerDailyDatasBy(workerId);
            if (datas == null) return null;
            return datas.FirstOrDefault();
        }
    }
}
