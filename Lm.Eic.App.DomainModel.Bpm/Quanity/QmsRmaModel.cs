﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lm.Eic.App.DomainModel.Bpm.Quanity
{
    // <summary>
    ///初始Rma单模型
    /// </summary>
    [Serializable]
    public partial class ReportInitiateModel
    {
        public ReportInitiateModel()
        { }
        #region Model
        private string _rmaid;
        /// <summary>
        ///RMA单号
        /// </summary>
        public string RmaId
        {
            set { _rmaid = value; }
            get { return _rmaid; }
        }
        private string _customershortname;
        /// <summary>
        ///客户简称
        /// </summary>
        public string CustomerShortName
        {
            set { _customershortname = value; }
            get { return _customershortname; }
        }
        private string _productname;
        /// <summary>
        ///品名
        /// </summary>
        public string ProductName
        {
            set { _productname = value; }
            get { return _productname; }
        }
        private string _rmaidstatus;
        /// <summary>
        ///单据状态
        /// </summary>
        public string RmaIdStatus
        {
            set { _rmaidstatus = value; }
            get { return _rmaidstatus; }
        }
        private string _rmayear;
        /// <summary>
        ///年份
        /// </summary>
        public string RmaYear
        {
            set { _rmayear = value; }
            get { return _rmayear; }
        }
        private string _rmamonth;
        /// <summary>
        ///月份
        /// </summary>
        public string RmaMonth
        {
            set { _rmamonth = value; }
            get { return _rmamonth; }
        }
        private string _opperson;
        /// <summary>
        ///操作人
        /// </summary>
        public string OpPerson
        {
            set { _opperson = value; }
            get { return _opperson; }
        }
        private DateTime _opdate;
        /// <summary>
        ///操作日期
        /// </summary>
        public DateTime OpDate
        {
            set { _opdate = value; }
            get { return _opdate; }
        }
        private DateTime _optime;
        /// <summary>
        ///操作时间
        /// </summary>
        public DateTime OpTime
        {
            set { _optime = value; }
            get { return _optime; }
        }
        private string _opsign;
        /// <summary>
        ///操作标志
        /// </summary>
        public string OpSign
        {
            set { _opsign = value; }
            get { return _opsign; }
        }
        private decimal _id_key;
        /// <summary>
        ///自增键
        /// </summary>
        public decimal Id_Key
        {
            set { _id_key = value; }
            get { return _id_key; }
        }
        #endregion Model
    }
    /// <summary>
    ///业务Rma处理模型
    /// </summary>
    [Serializable]
    public partial class BussesDescriptionModel
    {
        public BussesDescriptionModel()
        { }
        #region Model
        private string _rmaid;
        /// <summary>
        ///RMA单号
        /// </summary>
        public string RmaId
        {
            set { _rmaid = value; }
            get { return _rmaid; }
        }
        private int _rmaidnumber;
        /// <summary>
        ///序号
        /// </summary>
        public int RmaIdNumber
        {
            set { _rmaidnumber = value; }
            get { return _rmaidnumber; }
        }
        private string _returnhandleorder;
        /// <summary>
        ///销货单号
        /// </summary>
        public string ReturnHandleOrder
        {
            set { _returnhandleorder = value; }
            get { return _returnhandleorder; }
        }
        private string _prodcutid;
        /// <summary>
        ///品号
        /// </summary>
        public string ProdcutId
        {
            set { _prodcutid = value; }
            get { return _prodcutid; }
        }
        private string _productname;
        /// <summary>
        ///品名
        /// </summary>
        public string ProductName
        {
            set { _productname = value; }
            get { return _productname; }
        }
        private string _productspec;
        /// <summary>
        ///规格
        /// </summary>
        public string ProductSpec
        {
            set { _productspec = value; }
            get { return _productspec; }
        }
        private double _productcount;
        /// <summary>
        ///数量
        /// </summary>
        public double ProductCount
        {
            set { _productcount = value; }
            get { return _productcount; }
        }
        private string _customerid;
        /// <summary>
        ///客户编号
        /// </summary>
        public string CustomerId
        {
            set { _customerid = value; }
            get { return _customerid; }
        }
        private string _customername;
        /// <summary>
        ///客户名称
        /// </summary>
        public string CustomerName
        {
            set { _customername = value; }
            get { return _customername; }
        }
        private string _salesorder;
        /// <summary>
        ///订单号
        /// </summary>
        public string SalesOrder
        {
            set { _salesorder = value; }
            get { return _salesorder; }
        }
        private DateTime _productsshipdate;
        /// <summary>
        ///原出货日期
        /// </summary>
        public DateTime ProductsShipDate
        {
            set { _productsshipdate = value; }
            get { return _productsshipdate; }
        }
        private string _baddescrption;
        /// <summary>
        ///不良现象
        /// </summary>
        public string BadDescrption
        {
            set { _baddescrption = value; }
            get { return _baddescrption; }
        }
        private string _customerhandlesuggestion;
        /// <summary>
        ///处理意见
        /// </summary>
        public string CustomerHandleSuggestion
        {
            set { _customerhandlesuggestion = value; }
            get { return _customerhandlesuggestion; }
        }
        private string _feepaymentway;
        /// <summary>
        ///付费方式
        /// </summary>
        public string FeePaymentWay
        {
            set { _feepaymentway = value; }
            get { return _feepaymentway; }
        }
        private string _handlestatus;
        /// <summary>
        ///处理状态
        /// </summary>
        public string HandleStatus
        {
            set { _handlestatus = value; }
            get { return _handlestatus; }
        }
        private string _opperson;
        /// <summary>
        ///操作人
        /// </summary>
        public string OpPerson
        {
            set { _opperson = value; }
            get { return _opperson; }
        }
        private DateTime _opdate;
        /// <summary>
        ///操作日期
        /// </summary>
        public DateTime OpDate
        {
            set { _opdate = value; }
            get { return _opdate; }
        }
        private DateTime _optime;
        /// <summary>
        ///操作时间
        /// </summary>
        public DateTime OpTime
        {
            set { _optime = value; }
            get { return _optime; }
        }
        private string _opsign;
        /// <summary>
        ///操作标志
        /// </summary>
        public string OpSign
        {
            set { _opsign = value; }
            get { return _opsign; }
        }
        private decimal _id_key;
        /// <summary>
        ///自增键
        /// </summary>
        public decimal Id_Key
        {
            set { _id_key = value; }
            get { return _id_key; }
        }
        #endregion Model
    }
    /// <summary>
    ///品保Rma处理模型
    /// </summary>
    [Serializable]
    public partial class InspectionManageModel
    {
        public InspectionManageModel()
        { }
        #region Model
        private string _rmaid;
        /// <summary>
        ///RMA单号
        /// </summary>
        public string RmaId
        {
            set { _rmaid = value; }
            get { return _rmaid; }
        }
        private int _rmaidnumber;
        /// <summary>
        ///序号
        /// </summary>
        public int RmaIdNumber
        {
            set { _rmaidnumber = value; }
            get { return _rmaidnumber; }
        }
        private string _badphenomenon;
        /// <summary>
        ///不良现象
        /// </summary>
        public string BadPhenomenon
        {
            set { _badphenomenon = value; }
            get { return _badphenomenon; }
        }
        private string _baddescription;
        /// <summary>
        ///不良描述
        /// </summary>
        public string BadDescription
        {
            set { _baddescription = value; }
            get { return _baddescription; }
        }
        private string _badreadson;
        /// <summary>
        ///不良原因
        /// </summary>
        public string BadReadson
        {
            set { _badreadson = value; }
            get { return _badreadson; }
        }
        private string _handleway;
        /// <summary>
        ///处理方式
        /// </summary>
        public string HandleWay
        {
            set { _handleway = value; }
            get { return _handleway; }
        }
        private string _responsibleperson;
        /// <summary>
        ///责任人
        /// </summary>
        public string ResponsiblePerson
        {
            set { _responsibleperson = value; }
            get { return _responsibleperson; }
        }
        private string _finishdate;
        /// <summary>
        ///完成日期
        /// </summary>
        public string FinishDate
        {
            set { _finishdate = value; }
            get { return _finishdate; }
        }
        private string _paytime;
        /// <summary>
        ///话费工时
        /// </summary>
        public string PayTime
        {
            set { _paytime = value; }
            get { return _paytime; }
        }
        private string _liabilitybelongto;
        /// <summary>
        ///责任归属
        /// </summary>
        public string LiabilityBelongTo
        {
            set { _liabilitybelongto = value; }
            get { return _liabilitybelongto; }
        }
        private string _handlestatus;
        /// <summary>
        ///处理状态
        /// </summary>
        public string HandleStatus
        {
            set { _handlestatus = value; }
            get { return _handlestatus; }
        }
        private string _opperson;
        /// <summary>
        ///操作人
        /// </summary>
        public string OpPerson
        {
            set { _opperson = value; }
            get { return _opperson; }
        }
        private DateTime _opdate;
        /// <summary>
        ///操作日期
        /// </summary>
        public DateTime OpDate
        {
            set { _opdate = value; }
            get { return _opdate; }
        }
        private DateTime _optime;
        /// <summary>
        ///操作时间
        /// </summary>
        public DateTime OpTime
        {
            set { _optime = value; }
            get { return _optime; }
        }
        private string _opsign;
        /// <summary>
        ///操作标志
        /// </summary>
        public string OpSign
        {
            set { _opsign = value; }
            get { return _opsign; }
        }
        private decimal _id_key;
        /// <summary>
        ///自增键
        /// </summary>
        public decimal Id_Key
        {
            set { _id_key = value; }
            get { return _id_key; }
        }
        #endregion Model
    }

}
