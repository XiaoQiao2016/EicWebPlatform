﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lm.Eic.Framework.ProductMaster.Model.ITIL
{
   public class ItilDevelopModuleManageChangeRecordModel
    {
        public ItilDevelopModuleManageChangeRecordModel()
        { }

        #region Model
        private string _parameterkey;
        /// <summary>
        ///模块名&类名&方法名
        /// </summary>
        public string ParameterKey
        {
            set { _parameterkey = value; }
            get { return _parameterkey; }
        }
        private string _changeprogress;
        /// <summary>
        ///开发进度
        /// </summary>
        public string ChangeProgress
        {
            set { _changeprogress = value; }
            get { return _changeprogress; }
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
        private string _opperson;
        /// <summary>
        ///操作人
        /// </summary>
        public string OpPerson
        {
            set { _opperson = value; }
            get { return _opperson; }
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
        ///自增减
        /// </summary>
        public decimal Id_Key
        {
            set { _id_key = value; }
            get { return _id_key; }
        }
        #endregion Model
    }
}
