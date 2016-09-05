﻿using Lm.Eic.Uti.Common.YleeObjectBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lm.Eic.App.Business.Bmp.Pms.DailyReport
{
    public class DailyReportService
    {
        /// <summary>
        /// 工序管理
        /// </summary>
        public static ProductFlowManager MaterialBoardManager
        {
            get { return OBulider.BuildInstance<ProductFlowManager>(); }
        }
    }
}
