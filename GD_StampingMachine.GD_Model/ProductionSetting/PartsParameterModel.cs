﻿using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.GD_Model.ProductionSetting
{
    public class PartsParameterModel
    {
        /// <summary>
        /// 分配加工專案
        /// </summary>
        public string DistributeName { get; set; }
        /// <summary>
        /// 加工專案名稱
        /// </summary>
        public string ProjectID { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 鐵牌字串
        /// </summary>
        public string IronPlateString { get; set; }
        /// <summary>
        /// QR內容
        /// </summary>
        public string QrCodeContent { get; set; }
        /// <summary>
        /// 側邊字串(橫著打)
        /// </summary>
        public string QR_Special_IronPlateString { get; set; }


        /// <summary>
        /// 參數A
        /// </summary>
        public string ParamA { get; set; }
        /// <summary>
        /// 參數B
        /// </summary>
        public string ParamB { get; set; }
        /// <summary>
        /// 參數C
        /// </summary>
        public string ParamC { get; set; }
        /// <summary>
        /// 進度條
        /// </summary>
        public float Processing { get; set; }
        /// <summary>
        /// 加工狀態
        /// </summary>
        public MachiningStatusEnum MachiningStatus { get; set; }
        /// <summary>
        /// 分料盒
        /// </summary>
        public int? BoxIndex { get; set; }
        /// <summary>
        /// 鐵牌參數
        /// </summary>
        public StampPlateSettingModel StampingPlate { get; set; } = new StampPlateSettingModel();
   
        /// <summary>
        /// 機台命令
        /// </summary>
        public SendMachineCommandModel SendMachineCommand { get; set; } = new SendMachineCommandModel();




    }






}
