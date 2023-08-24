﻿using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.GD_Model
{
    public class SendMachineCommandModel
    {
        public SteelBeltStampingStatusEnum SteelBeltStampingStatus { get; set; }

        /// <summary>
        /// 鐵片流水號
        /// </summary>
        public int WorkNumber { get; set; }

        
        /// <summary>
        /// 工作移動到下一個工站的相對距離(由計算得出 輪到他加工時他需移動的距離)
        /// </summary>
        public double RelativeMoveDistance { get; set; }
        /// <summary>
        /// 工作需要移動的絕對距離(目前位置離加工位置多遠)
        /// </summary>
        public double AbsoluteMoveDistance { get; set; }

        /// <summary>
        /// 鐵牌
        /// </summary>
        public StampPlateSettingModel StampPlateSetting { get; set; }

        /// <summary>
        /// 鐵牌寬度
        /// </summary>
        public  double StampWidth { get; set; }



        /// <summary>
        /// 加工需求 QR加工 true會進行加工
        /// </summary>
        public bool WorkScheduler_QRStamping { get; set; }
        //public double WorkScheduler_QRStamping_XOffset { get; set; }


        /// <summary>
        /// 加工需求 鋼印加工 true會進行加工
        /// </summary>
        public bool WorkScheduler_FontStamping { get; set; }
        //public double WorkScheduler_FontStamping_XOffset { get; set; }

        /// <summary>
        /// 加工需求 剪斷 true會進行加工
        /// </summary>
        public bool WorkScheduler_Shearing { get; set; }
        //public double WorkScheduler_Shearing_XOffset { get; set; }when

        public bool IsFinish { get; set; }

        public SteelBeltStampingStatusEnum MachiningStatus { get; set; }


    }




}