﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lm.Eic.App.HwCollaboration.Business;
using Lm.Eic.App.HwCollaboration.Model;
using Lm.Eic.Uti.Common.YleeOOMapper;

namespace Lm.Eic.App.HwCollaboration.Business.ManPower
{
    /// <summary>
    /// 人力管理器
    /// </summary>
    public class ManPowerManager : HwCollaborationDataBase<ManPowerDto>
    {
        public ManPowerManager() : base(HwModuleName.ManPower, HwAccessApiUrl.ManPowerApiUrl)
        { }

        protected override bool CanSendDto(ManPowerDto dto)
        {
            return dto.manpowerMainList != null && dto.manpowerMainList.Count > 0;
        }

        protected override ManPowerDto HandleDto(ManPowerDto dto)
        {
            return dto;
        }
    }
}
