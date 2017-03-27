﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lm.Eic.App.DbAccess.Bpm.Mapping;
using Lm.Eic.Uti.Common.YleeDbHandler;
using Lm.Eic.App.DomainModel.Bpm.Quanity;
namespace Lm.Eic.App.DbAccess.Bpm.Repository.QmsRep
{
    public interface IInspectionModeConfigRepository : IRepository<InspectionModeConfigModel>
    {
       
    }
    public class InspectionModeConfigRepository : BpmRepositoryBase<InspectionModeConfigModel>, IInspectionModeConfigRepository
    {
     

        
    }

    public interface IInspectionModeSwitchConfigRepository : IRepository<InspectionModeSwitchConfigModel>
    {

    }
    public class InspectionModeSwitchConfigRepository : BpmRepositoryBase<InspectionModeSwitchConfigModel>, IInspectionModeSwitchConfigRepository
    {
    }




    #region IQC

    public interface IIqcInspectionItemConfigRepository : IRepository<InspectionIqcItemConfigModel>
    {
       
    }
    public class IqcInspectionItemConfigRepository : BpmRepositoryBase<InspectionIqcItemConfigModel>, IIqcInspectionItemConfigRepository
    {
     
    }

    public interface IIqcInspectionMasterRepository : IRepository<InspectionIqcMasterModel>
    {
        int UpAuditMaterData(string orderId, string materialId,string upAuditData);
        int UpAuditDetailData(string orderId, string materialId, string upAuditData);


    }
    public class IqcInspectionMasterRepository : BpmRepositoryBase<InspectionIqcMasterModel>, IIqcInspectionMasterRepository
    {
        public int UpAuditDetailData(string orderId, string materialId, string inspectionItemStatus)
        {
            string upDetailsql = string.Format("Update   Qms_IqcInspectionDetail   Set  InspectionItemStatus='{0}'  Where OrderId='{1}' and  MaterialId='{2}'", inspectionItemStatus, orderId, materialId);
            return DbHelper.Bpm.ExecuteNonQuery(upDetailsql);
        }

        public int  UpAuditMaterData(string orderId, string materialId,string upAduitData)
        {
            string upMaterSql = string.Format("Update   Qms_IqcInspectionMaster   Set InspectionStatus='{0}'  Where OrderId='{1}' and  MaterialId='{2}'", upAduitData, orderId, materialId);
            return DbHelper.Bpm.ExecuteNonQuery(upMaterSql);
        }
    }

    public interface IIqcInspectionDetailRepository : IRepository<InspectionIqcDetailModel> { }
    public class IqcInspectionDetailRepository : BpmRepositoryBase<InspectionIqcDetailModel>, IIqcInspectionDetailRepository
    { }
    #endregion

    #region  FQC

    public interface IFqcInspectionItemConfigRepository : IRepository<InspectionFqcItemConfigModel> { }

    public class FqcInspectionItemConfigRepository : BpmRepositoryBase<InspectionFqcItemConfigModel>, IFqcInspectionItemConfigRepository
    {

    }
    public interface IFqcInspectionDatailRepository : IRepository<InspectionFqcDetailModel> { }

    public class FqcInspectionDatailRepository : BpmRepositoryBase<InspectionFqcDetailModel>, IFqcInspectionDatailRepository
    { }


    public interface IFqcInspectionMasterRepository : IRepository<InspectionFqcMasterModel>
    {
        int UpAuditMaterData(string orderId, int orderIdNumber, string upAuditData);
        int UpAuditDetailData(string orderId, int orderIdNumber, string upAuditData);
    }

    public class FqcInspectionMasterRepository : BpmRepositoryBase<InspectionFqcMasterModel>, IFqcInspectionMasterRepository
    {
        public int UpAuditDetailData(string orderId, int orderIdNumber, string upAuditData)
        {
            string upDetailsql = string.Format("Update   Qms_FqcInspectionDetail   Set   InspectionItemStatus='{0}'  Where OrderId='{1}' and  OrderIdNumber='{2}'", upAuditData, orderId, orderIdNumber);
            return DbHelper.Bpm.ExecuteNonQuery(upDetailsql);
        }

        public int UpAuditMaterData(string orderId, int orderIdNumber, string upAuditData)
        {
            string upDetailsql = string.Format("Update   Qms_FqcInspectionMaster   Set   InspectionStatus='{0}'  Where OrderId='{1}' and  OrderIdNumber='{2}'", upAuditData, orderId, orderIdNumber);
            return DbHelper.Bpm.ExecuteNonQuery(upDetailsql);
        }
    }

    #endregion



}
