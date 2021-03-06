﻿using Lm.Eic.App.DbAccess.Bpm.Repository.AstRep;
using Lm.Eic.App.DomainModel.Bpm.Ast;
using Lm.Eic.Uti.Common.YleeDbHandler;
using Lm.Eic.Uti.Common.YleeExtension.Conversion;
using Lm.Eic.Uti.Common.YleeExtension.Validation;
using Lm.Eic.Uti.Common.YleeObjectBuilder;
using Lm.Eic.Uti.Common.YleeOOMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lm.Eic.App.Business.Bmp.Ast
{
    /***********************************************   设备管理CRUD工厂   *****************************
    *
    *  2017-7-27  初版   张明
    **************************************************************************************************/

    /// <summary>
    /// 设备管理模块Crud工厂
    /// </summary>
    internal class EquipmentCrudFactory
    {
        /// <summary>
        /// 设备档案操作Crud
        /// </summary>
        public static EquipmentCrud EquipmentCrud
        {
            get { return OBulider.BuildInstance<EquipmentCrud>(); }
        }

        /// <summary>
        /// 设备校验操作Crud
        /// </summary>
        public static EquipmentCheckCrud EquipmentCheckCrud
        {
            get { return OBulider.BuildInstance<EquipmentCheckCrud>(); }
        }

        /// <summary>
        /// 设备保养操作Crud
        /// </summary>
        public static EquipmentMaintenanceCrud EquipmentMaintenanceCrud
        {
            get { return OBulider.BuildInstance<EquipmentMaintenanceCrud>(); }
        }

        /// <summary>
        /// 设备报废操作Crud
        /// </summary>
        public static EquipmentDiscarCrud EquipmentDiscarCrud
        {
            get { return OBulider.BuildInstance<EquipmentDiscarCrud>(); }
        }

        /// <summary>
        /// 设备维修操作Crud
        /// </summary>
        public static EquipmentRepairedRecordCrud EquipmentRepairedRecordCrud
        { get { return OBulider.BuildInstance<EquipmentRepairedRecordCrud>(); } }
    }

    /***********************************************   设备档案CRUD   *********************************
   *
   *  2017-7-27  初版   张明
   ***************************************************************************************************/

    /// <summary>
    /// 设备档案CRUD
    /// </summary>
    internal class EquipmentCrud
    {
        private IEquipmentRepository irep = null;

        public EquipmentCrud()
        {
            irep = new EquipmentRepository();
        }

        #region Find

        /// <summary>
        /// 查询 
        /// 1.依据财产编号(包含)查询 
        /// 2.依据保管部门查询 
        /// 3.依据录入日期查询
        /// 4.依据录入日期查询待校验设备 
        /// 5.依据录入日期查询待保养设备 
        /// 6.生成设备总览表
        /// </summary>
        /// <param name="qryDto">设备查询数据传输对象 </param>
        /// <returns></returns>
        public List<EquipmentModel> FindBy(QueryEquipmentDto qryDto)
        {
            try
            {
                switch (qryDto.SearchMode)
                {
                    case 1: //依据财产编号查询
                        return irep.Entities.Where(m => m.AssetNumber == qryDto.AssetNumber).ToList();
                    case 2: //依据保管部门查询
                        return irep.Entities.Where(m => m.SafekeepDepartment.Contains(qryDto.Department)).ToList();

                    //return irep.Entities.Where(m => m.SafekeepDepartment.StartsWith(qryDto.Department, StringComparison.CurrentCulture)).ToList();

                    case 3: //依据录入日期查询
                        DateTime inputDate = qryDto.InputDate.ToDate();
                        return irep.Entities.Where(m => m.InputDate == inputDate).ToList();

                    case 4: //依据录入日期查询待校验设备  //结束日期=输入日期加一个月 超期设备等于 计划日期<=当天日期
                        DateTime startPlannedDate = qryDto.PlannedCheckDate.ToDate(), endPlannedDate = startPlannedDate.AddMonths(1), nowDate = DateTime.Now.ToDate();
                        return irep.Entities.Where(m => (m.IsScrapped == "否" && m.IsCheck == "是" && m.PlannedCheckDate >= startPlannedDate && m.PlannedCheckDate <= endPlannedDate) || (m.IsScrapped == "正常" && m.IsCheck == "是" && m.PlannedCheckDate <= nowDate)).ToList();

                    case 5: //依据录入日期查询待保养设备  //按计划保养月查询待保养待设备列表
                        return irep.Entities.Where(m => m.IsMaintenance == "是" && m.PlannedMaintenanceMonth == qryDto.PlannedMaintenanceMonth).ToList();

                    case 6: //查询所有在使用待设备 生成设备总览表
                        return irep.Entities.Where(m => m.IsScrapped == "否").ToList();
                    case 7: //依据财产编号查询
                        return irep.Entities.Where(m => m.AssetNumber.Contains(qryDto.AssetNumber)).ToList();
                    default:
                        return new List<EquipmentModel>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message);
            }
        }

        #endregion Find

        #region Store

        /// <summary>
        /// 修改数据仓库 model.OpSign = add/edit/delete
        /// </summary>
        /// <param name="listModel"></param>
        /// <returns></returns>
        public OpResult Store(List<EquipmentModel> listModel)
        {
            int record = 0;
            string opContext = "设备档案",
                   opSign = string.Empty;
            OpResult opResult = OpResult.SetErrorResult("未执行任何操作！");

            if (listModel == null || listModel.Count <= 0)
                return OpResult.SetErrorResult("集合不能为空！");

            opSign = listModel[0].OpSign;
            if (opSign.IsNullOrEmpty())
                return OpResult.SetErrorResult("操作模式不能为空！");

            try
            {
                switch (opSign)
                {
                    case OpMode.Add: //新增
                        listModel.ForEach(model => { record += irep.Insert(model); });
                        opResult = record.ToOpResult_Add(opContext);
                        break;

                    case OpMode.Edit: //修改
                        listModel.ForEach(model => { record += irep.Update(u => u.Id_Key == model.Id_Key, model); });
                        opResult = record.ToOpResult_Eidt(opContext);
                        break;

                    case OpMode.Delete: //删除
                        listModel.ForEach(model => { record += irep.Delete(model.Id_Key); });
                        opResult = record.ToOpResult_Delete(opContext);
                        break;

                    default:
                        opResult = OpResult.SetErrorResult("操作模式溢出！");
                        break;
                }
            }
            catch (Exception ex) { throw new Exception(ex.InnerException.Message); }
            return opResult;
        }

        /// <summary>
        /// 修改数据仓库 model.OpSign = add/edit/delete
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public OpResult Store(EquipmentModel model)
        {
            OpResult result = OpResult.SetSuccessResult("财产编号不能为空！", false);
            if (model == null || model.AssetNumber.IsNullOrEmpty())
                return result;
            try
            {
                switch (model.OpSign)
                {
                    case OpMode.Add: //新增
                        result = AddEquipmentRecord(model);
                        break;

                    case OpMode.Edit: //修改
                        result = EditEquipmentRecord(model);
                        break;

                    case OpMode.Delete: //删除
                        result = DeleteEquipmentRecord(model);
                        break;

                    default:
                        result = OpResult.SetErrorResult("操作模式溢出");
                        break;
                }
            }
            catch (Exception ex) { throw new Exception(ex.InnerException.Message); }
            return result;
        }

        private OpResult AddEquipmentRecord(EquipmentModel model)
        {
            DateTime defaultDate = DateTime.Now.ToDate();
            //基础设置
            model.DeliveryDate = defaultDate;
            model.InputDate = defaultDate;
            model.OpDate = defaultDate;
            model.OpTime = DateTime.Now;
            model.PlannedCheckDate = defaultDate;
            model.PlannedMaintenanceDate = defaultDate;

            SetEquipmentMaintenanceRule(model);
            SetEquipmentCheckRule(model);
            //设备状态初始化
            if (model.State == null)
                model.State = "运行正常";
            if (model.IsScrapped == null)
                model.IsScrapped = "否";
            //仓储操作
            return irep.Insert(model).ToOpResult_Add("设备档案", model.Id_Key);
        }

        private OpResult EditEquipmentRecord(EquipmentModel model)
        {
            SetEquipmentMaintenanceRule(model);
            SetEquipmentCheckRule(model);

            return irep.Update(u => u.Id_Key == model.Id_Key, model).ToOpResult_Eidt("设备档案");
        }

        private OpResult DeleteEquipmentRecord(EquipmentModel model)
        {
            return irep.Delete(model.Id_Key).ToOpResult_Delete("设备档案");
        }

        #endregion Store

        #region Rule

        /// <summary>
        /// 设置设备校验规则
        /// </summary>
        /// <param name="model"></param>
        private void SetEquipmentCheckRule(EquipmentModel model)
        {
            if (model.EquipmentType != "量测设备")  //如果不等于量测设备 不校验
            {
                model.CheckDate = DateTime.Now.ToDate();//设置为默认日期
                model.CheckInterval = 0;
            }
            //校验处理 设备类别为量测设备才会校验
            model.IsCheck = model.CheckInterval == 0 ? "否" : "是";
            model.CheckDate = model.CheckDate.ToDate();
            model.PlannedCheckDate = model.CheckDate.AddMonths(model.CheckInterval);
            model.CheckState = "在期";
        }

        /// <summary>
        /// 设置设备保养规则
        /// </summary>
        /// <param name="model"></param>
        private void SetEquipmentMaintenanceRule(EquipmentModel model)
        {
            if (model.AssetType == "低质易耗品")  //如果等于低值易耗品 不保养
            {
                model.MaintenanceDate = DateTime.Now.ToDate();
                model.MaintenanceInterval = 0;
            }
            //保养处理
            model.IsMaintenance = model.MaintenanceInterval == 0 ? "否" : "是";
            model.MaintenanceDate = model.MaintenanceDate.ToDate();
            model.PlannedMaintenanceDate = model.MaintenanceDate.AddMonths(model.MaintenanceInterval);
            model.PlannedMaintenanceMonth = model.PlannedMaintenanceDate.ToString("yyyyMM");
            model.MaintenanceState = "在期";
        }

        #endregion Rule
    }

    /***********************************************   设备校验CRUD   *********************************
    *
    *  2017-7-27  初版   张明
    ***************************************************************************************************/

    /// <summary>
    /// 校验管理Crud
    /// </summary>
    internal class EquipmentCheckCrud
    {
        private IEquipmentCheckRepository irep = null;

        public EquipmentCheckCrud()
        {
            irep = new EquipmentCheckRepository();
        }

        #region FindBy

        /// <summary>
        /// 查询 
        /// 1.依据财产编号查询
        /// 2: //依据财产编号精确查询
        /// </summary>
        /// <param name="qryDto">设备查询数据传输对象 </param>
        /// <returns></returns>
        public List<EquipmentCheckRecordModel> FindBy(QueryEquipmentDto qryDto)
        {
            try
            {
                switch (qryDto.SearchMode)
                {
                    case 1: //依据财产编号查询
                        return irep.Entities.Where(m => m.AssetNumber.Contains(qryDto.AssetNumber)).ToList();
                    case 2: //依据财产编号精确查询
                        return irep.Entities.Where(m => m.AssetNumber == qryDto.AssetNumber).ToList();

                    default: return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message);
            }
        }

        #endregion FindBy

        #region Store

        /// <summary>
        /// 修改数据仓库 model.OpSign = add/edit/delete
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public OpResult Store(EquipmentCheckRecordModel model)
        {
            //先查找查找设备 找到后判断校验日期要不要写入 然后写入校验记录 再修改设备信息 如果失败 删除校验记录
            model.OpDate = DateTime.Now.ToDate();
            model.OpTime = DateTime.Now;
            string opContext = "设备校验";
            OpResult opResult = OpResult.SetErrorResult("未执行任何操作！");

            if (model == null)
                return OpResult.SetErrorResult("校验记录不能为空！");

            //设备是否存在
            var equipment = EquipmentCrudFactory.EquipmentCrud.FindBy(new QueryEquipmentDto() { AssetNumber = model.AssetNumber, SearchMode = 1 }).FirstOrDefault();
            if (equipment == null)
                return OpResult.SetErrorResult("未找到保养单上的设备\r\n请确定财产编号是否正确！");

            //设置保养记录 设备名称
            model.EquipmentName = equipment.EquipmentName;

            try
            {
                //判断设备校验日期是否等于校验单上的日期
                if (equipment.CheckDate != model.CheckDate)
                {
                    switch (model.OpSign)
                    {
                        case OpMode.Add: //新增
                            opResult = irep.Insert(model).ToOpResult_Add(opContext, model.Id_Key);
                            opResult.Attach = model;
                            break;

                        case OpMode.Edit: //修改
                            opResult = irep.Update(u => u.Id_Key == model.Id_Key, model).ToOpResult_Eidt(opContext);
                            break;

                        case OpMode.Delete: //删除
                            opResult = irep.Delete(model.Id_Key).ToOpResult_Delete(opContext);
                            break;

                        default:
                            opResult = OpResult.SetErrorResult("操作模式溢出");
                            break;
                    }

                    //如果保存记录成功
                    if (opResult.Result)
                    {
                        opResult = SetEquipmentCheckDateRule(model, equipment);
                        if (!opResult.Result) //如果设备未更新成功
                        {
                            irep.Delete(model.Id_Key).ToOpResult_Delete(opContext);
                            return opResult;
                        }
                    }
                    opResult.Attach = model;
                    return opResult;
                }
                else
                {
                    return OpResult.SetErrorResult("设备校验日期与录入日期相等");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message);
            }
        }

        /// <summary>
        /// 设置设备校验日期规则
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private OpResult SetEquipmentCheckDateRule(EquipmentCheckRecordModel model, EquipmentModel equipment)
        {
            equipment.CheckDate = model.CheckDate;
            equipment.OpSign = OpMode.Edit;
            return OpResult.SetResult("更新设备校验日期成功！", "更新设备校验日期失败！", EquipmentCrudFactory.EquipmentCrud.Store(equipment).Result);
        }

        #endregion Store
    }

    /***********************************************   设备保养CRUD   *********************************
   *
   *  2017-7-27  初版   张明
   ***************************************************************************************************/

    /// <summary>
    /// 保养管理Crud
    /// </summary>
    internal class EquipmentMaintenanceCrud
    {
        private IEquipmentMaintenanceRepositor irep = null;

        public EquipmentMaintenanceCrud()
        {
            irep = new EquipmentMaintenanceRepository();
        }

        #region FindBy

        /// <summary>
        /// 查询
        /// 1.依据财产编号查询
        /// 2.依据财产编号精确查询
        /// </summary>
        /// <param name="qryDto">设备查询数据传输对象 </param>
        /// <returns></returns>
        public List<EquipmentMaintenanceRecordModel> FindBy(QueryEquipmentDto qryDto)
        {
            try
            {
                switch (qryDto.SearchMode)
                {
                    case 1: //依据财产编号查询
                        return irep.Entities.Where(m => m.AssetNumber.StartsWith(qryDto.AssetNumber)).ToList();
                    case 2: //依据财产编号精确查询
                        return irep.Entities.Where(m => m.AssetNumber == qryDto.AssetNumber).ToList();

                    default: return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message);
            }
        }

        #endregion FindBy

        #region Store

        /// <summary>
        /// 修改数据仓库 model.OpSign = add/edit/delete
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public OpResult Store(EquipmentMaintenanceRecordModel model)
        {
            //先查找查找设备 找到后判断校验日期要不要写入 然后写入校验记录 再修改设备信息 如果失败 删除校验记录
            model.OpDate = DateTime.Now.ToDate();
            model.OpTime = DateTime.Now;
            string opContext = "设备保养";
            OpResult opResult = OpResult.SetErrorResult("未执行任何操作！");

            if (model == null)
                return OpResult.SetErrorResult("保养记录不能为空！");

            //设备是否存在
            var equipment = EquipmentCrudFactory.EquipmentCrud.FindBy(new QueryEquipmentDto() { SearchMode = 1, AssetNumber = model.AssetNumber }).FirstOrDefault();
            if (equipment == null)
                return OpResult.SetErrorResult("未找到保养单上的设备\r\n请确定财产编号是否正确！");

            //设置保养记录 设备名称
            model.EquipmentName = equipment.EquipmentName;

            //保存操作记录
            try
            {
                if (equipment.MaintenanceDate != model.MaintenanceDate)
                {
                    switch (model.OpSign)
                    {
                        case OpMode.Add: //新增
                            opResult = irep.Insert(model).ToOpResult_Add(opContext, model.Id_Key);
                            break;

                        case OpMode.Edit: //修改
                            opResult = irep.Update(u => u.Id_Key == model.Id_Key, model).ToOpResult_Eidt(opContext);
                            break;

                        case OpMode.Delete: //删除
                            opResult = irep.Delete(model.Id_Key).ToOpResult_Delete(opContext);
                            break;

                        default:
                            opResult = OpResult.SetErrorResult("操作模式溢出");
                            break;
                    }

                    //如果保存记录成功
                    if (opResult.Result)
                    {
                        opResult = SetEquipmentMaintenanceDateRule(model, equipment);
                        if (!opResult.Result) //如果设备未更新成功
                        {
                            irep.Delete(model.Id_Key).ToOpResult_Delete(opContext);
                            return opResult;
                        }
                    }
                    opResult.Attach = model;
                    return opResult;
                }
                else { return OpResult.SetErrorResult("设备保养日期与录入日期相等"); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message);
            }
        }

        /// <summary>
        /// 设置设备保养日期规则
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private OpResult SetEquipmentMaintenanceDateRule(EquipmentMaintenanceRecordModel model, EquipmentModel equipment)
        {
            equipment.MaintenanceDate = model.MaintenanceDate;
            equipment.OpSign = OpMode.Edit;
            return OpResult.SetResult("更新设备保养日期成功！", "更新设备保养日期失败！", EquipmentCrudFactory.EquipmentCrud.Store(equipment).Result);
        }

        #endregion Store
    }

    /***********************************************   设备报废CRUD   *********************************
  *
  *  2017-7-27  初版   张明
  ***************************************************************************************************/

    /// <summary>
    /// 报废管理Crud
    /// </summary>
    internal class EquipmentDiscarCrud : CrudBase<EquipmentDiscardRecordModel, IEquipmentDiscardRepository>
    {
        public EquipmentDiscarCrud() : base(new EquipmentDiscardRepository(), "设备报废记录")
        {
        }

        /// <summary>
        /// 添加CRUD方法到方法组
        /// </summary>
        protected override void AddCrudOpItems()
        {
            this.AddOpItem(OpMode.Add, AddEquipmentDiscardRecord);
            this.AddOpItem(OpMode.Edit, EditEquipmentDiscardRecord);
            this.AddOpItem(OpMode.UpDate, DeleteEquipmentDiscardRecord);
        }

        /// <summary>
        /// 添加一条设备报废记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private OpResult AddEquipmentDiscardRecord(EquipmentDiscardRecordModel model)
        {
            OpResult result = OpResult.SetErrorResult("未执行任何操作");

            //设备是否存在
            var equipment = EquipmentCrudFactory.EquipmentCrud.FindBy(new QueryEquipmentDto() { AssetNumber = model.AssetNumber, SearchMode = 1 }).FirstOrDefault();

            if (equipment == null)
                return OpResult.SetErrorResult("未找到报废单上的设备\r\n请确定财产编号是否正确！");

            if (equipment.IsScrapped == "是")
                return OpResult.SetErrorResult("操作失败！\r\n设备报废为已报废，不能重复报废！");

            //修改设备报废状态
            equipment.IsScrapped = "是";
            equipment.OpSign = OpMode.Edit;
            var equipmentOpResult = EquipmentCrudFactory.EquipmentCrud.Store(equipment);
            if (!equipmentOpResult.Result)
                return OpResult.SetErrorResult("修改设备报废状态失败！");

            model.DiscardMonth = DateTime.Now.ToString("yyyyMM");
            //存储记录
            model.EquipmentName = equipment.EquipmentName;
            result = irep.Insert(model).ToOpResult_Add(OpContext, model.Id_Key);
            return result;
        }

        /// <summary>
        /// 修改一条设备报废记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private OpResult EditEquipmentDiscardRecord(EquipmentDiscardRecordModel model)
        {
            OpResult result = OpResult.SetErrorResult("咱不提供修改方法");
            return result;
        }

        /// <summary>
        /// 删除一条设备报废记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private OpResult DeleteEquipmentDiscardRecord(EquipmentDiscardRecordModel model)
        {
            OpResult result = OpResult.SetErrorResult("暂不提供删除方法");
            return result;
        }

        /// <summary>
        /// 获取设备报废记录
        /// </summary>
        /// <param name="assetNumber">财产编号</param>
        /// <returns></returns>
        public EquipmentDiscardRecordModel GetEquipmentDiscardRecord(string assetNumber)
        {
            return irep.Entities.FirstOrDefault(m => m.AssetNumber == assetNumber);
        }
        /// <summary>
        /// 获取设备报废记录列表
        /// </summary>
        /// <param name="assetNumber">财产编号</param>
        /// <returns></returns>
        public List<EquipmentDiscardRecordModel> GetEquipmentDiscardRecordList(string assetNumber)
        {
            return irep.Entities.Where(m => m.AssetNumber == assetNumber).ToList();
        }

        /// <summary>
        /// 获取设备报废总览表
        /// </summary>
        /// <returns></returns>
        public List<EquipmentDiscardRecordModel> GetEquipmentDiscardOverView()
        {
            return irep.Entities.ToList();
        }
    }

    /***********************************************   设备维修CRUD   *********************************
   *
   *  2017-7-27  初版   张明
   ***************************************************************************************************/

    /// <summary>
    /// 维修管理Crud
    /// </summary>
    internal class EquipmentRepairedRecordCrud : CrudBase<EquipmentRepairedRecordModel, IEquipmentRepairedRecordRepository>
    {
        public EquipmentRepairedRecordCrud()
            : base(new EquipmentRepairedRecordRepository(), "设备维修记录")
        { }

        protected override void AddCrudOpItems()
        {
            this.AddOpItem(OpMode.Add, AddEquipmentRepairedRecord);
            this.AddOpItem(OpMode.Edit, EditEquipmentRepairedRecord);
        }

        /// <summary>
        /// 添加一条设备维修申请记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private OpResult AddEquipmentRepairedRecord(EquipmentRepairedRecordModel model)
        {
            if (model == null) return new OpResult("实体不能空");
            if (irep.IsExist(e => e.AssetNumber == model.AssetNumber && e.FormId == model.FormId))
            {
                return new OpResult("此编号已存在");
            }
            return irep.Insert(model).ToOpResult_Add(OpContext);
        }

        /// <summary>
        /// 修改一条设备维修申请记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private OpResult EditEquipmentRepairedRecord(EquipmentRepairedRecordModel model)
        {
            return irep.Update(u => u.Id_Key == model.Id_Key, model).ToOpResult_Add(OpContext);
        }
        /// <summary>
        /// 获取设备维修记录
        /// </summary>
        /// <param name="assetNumber">财产编号</param>
        /// <returns></returns>
        public List<EquipmentRepairedRecordModel> GetEquipmentRepairedRecordBy(string assetNumber)
        {
            var data = irep.Entities.Where(m => m.AssetNumber == assetNumber).ToList();
            return data;

        }
        public List<EquipmentRepairedRecordModel> GetEquipmentRepairedRecordFormIdBy(string assetNumber, string formdId)
        {
            return (formdId == string.Empty || formdId == null) ? irep.Entities.Where(m => m.AssetNumber == assetNumber).ToList() :
            irep.Entities.Where(m => m.FormId == formdId && m.AssetNumber == assetNumber).ToList();
        }
        /// <summary>
        /// 获取设备维修总览表
        /// </summary>
        /// <returns></returns>
        public List<EquipmentRepairedRecordModel> GetEquipmentRepairedOverView()
        {
            return irep.Entities.ToList();
        }
    }
}