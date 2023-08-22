﻿using DevExpress.Mvvm.Native;
using GD_CommonLibrary.Extensions;
using GD_CommonLibrary.Method;
using GD_CommonLibrary;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.Method;
using GD_StampingMachine.ViewModels.ProductSetting;
using GD_StampingMachine.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Singletons
{

    public class StampingMachineSingleton : BaseSingleton<StampingMachineSingleton>
    {
        protected override void Init()
        {
            StampingMachineJsonHelper JsonHM = new StampingMachineJsonHelper();

            MachanicalSpecificationVM = new MachanicalSpecificationViewModel(new MachanicalSpecificationModel()
            {
                AllowMachiningSize = new AllowMachiningSizeModel()
                {
                    WebHeightLowerLimited = 75,
                    WebHeightUpperLimited = 500,
                    FlangeWidthLowerLimited = 150,
                    FlangeWidthUpperLimited = 1050,
                    MachiningMinLength = 2400,
                    MachiningMaxLength = 99999
                },
                MachiningProperty = new MachiningPropertyModel()
                {
                    HorizontalDrillCount = 1,
                    VerticalDrillCount = 2,
                    Each_HorizontalDrill_SpindleCount = 1,
                    Each_VerticalDrill_SpindleCount = 1,
                    AuxiliaryAxisEffectiveTravelMax = 300,
                    MaxDrillDiameter = 40,
                    MaxDrillThickness = 80,
                    SpindleMaxPower = 15,
                    SpindleToolHolder = SpindleToolHolderEnum.BT40,
                    SpindleRotationalFrequencyMin = 180,
                    SpindleRotationalFrequencyMax = 400,
                    SpindleFeedSpeedMin = 40,
                    SpindleFeedSpeedMax = 1000,
                    SpindleMoveSpeed = 24
                },
                MachineSize = new MachineSizeModel()
                {
                    Length = 5450,
                    Width = 2000,
                    Height = 2000,
                    Weight = 14.5
                },
            })
            {

            };

            if (JsonHM.ReadMachineSettingJson(StampingMachineJsonHelper.MachineSettingNameEnum.StampingFont, out StampingFontChangedViewModel SReadSFC))
            {
                StampingFontChangedVM = SReadSFC;
            }
            else
            {
                StampingFontChangedVM = new StampingFontChangedViewModel
                {
                    StampingTypeVMObservableCollection = new ObservableCollection<StampingTypeViewModel>()
                };
                for (int i = 1; i <= 40; i++)
                {
                    // char

                    StampingFontChangedVM.StampingTypeVMObservableCollection.Add(
                        new StampingTypeViewModel(
                            new StampingTypeModel()
                            {
                                StampingTypeNumber = i,
                                StampingTypeString = (64 + i).ToChar().ToString(),
                                StampingTypeUseCount = 0
                            })
                        );
                }
                StampingFontChangedVM.UnusedStampingTypeVMObservableCollection = new ObservableCollection<StampingTypeViewModel>()
                {
                    new StampingTypeViewModel(new StampingTypeModel(){ StampingTypeNumber =0 , StampingTypeString = "ㄅ" , StampingTypeUseCount=0}) ,
                    new StampingTypeViewModel(new StampingTypeModel(){ StampingTypeNumber =0 , StampingTypeString = "ㄆ" , StampingTypeUseCount=0}),
                    new StampingTypeViewModel(new StampingTypeModel(){ StampingTypeNumber =0, StampingTypeString = "ㄇ" , StampingTypeUseCount=0})
                };
            }
            AxisSettingModel AxisSetting = new();
            if (JsonHM.ReadParameterSettingJsonSetting(StampingMachineJsonHelper.ParameterSettingNameEnum.AxisSetting, out AxisSettingModel JsonAxisSetting))
            {
                AxisSetting = JsonAxisSetting;
            }

            TimingSettingModel TimingSetting = new();
            if (JsonHM.ReadParameterSettingJsonSetting(StampingMachineJsonHelper.ParameterSettingNameEnum.TimingSetting, out TimingSettingModel JsonTimingSetting))
            {
                TimingSetting = JsonTimingSetting;
            }

            EngineerSettingModel EngineerSetting = new();
            if (JsonHM.ReadParameterSettingJsonSetting(StampingMachineJsonHelper.ParameterSettingNameEnum.EngineerSetting, out EngineerSettingModel JsonEngineerSetting))
            {
                EngineerSetting = JsonEngineerSetting;
            }

            SeparateSettingModel SeparateSetting = new();
            if (JsonHM.ReadParameterSettingJsonSetting(StampingMachineJsonHelper.ParameterSettingNameEnum.SeparateSetting, out SeparateSettingModel JsonSeparateSetting))
            {
                SeparateSetting = JsonSeparateSetting;
            }

            ParameterSettingModel ParameterSetting = new()
            {
                AxisSetting = AxisSetting,
                TimingSetting = TimingSetting,
                SeparateSetting = SeparateSetting,
                InputOutput = new(),
                EngineerSetting = EngineerSetting
            };

            ParameterSettingVM = new(ParameterSetting);

            ProductSettingVM = new();

            if (JsonHM.ReadProjectSettingJson(out List<ProjectModel> PathList))
            {
                PathList.ForEach(EPath =>
                {
                    //加工專案為到處放的形式 沒有固定位置
                    if (JsonHM.ReadJsonFile(System.IO.Path.Combine(EPath.ProjectPath, EPath.Name), out ProductProjectModel PProject))
                    {
                        ProductSettingVM.ProductProjectVMObservableCollection.Add(new ProductProjectViewModel(PProject));
                    }
                    else
                    {
                        //需註解找不到檔案!
                        MessageBoxResultShow.ShowOK("", $"Can't find file {System.IO.Path.Combine(EPath.ProjectPath, EPath.Name)}");
                        ProductSettingVM.ProductProjectVMObservableCollection.Add(new ProductProjectViewModel(new ProductProjectModel()
                        {
                            ProjectPath = EPath.ProjectPath,
                            Name = EPath.Name,
                            FileIsNotExisted = true
                        })); ;
                    }
                });
            }

            TypeSettingSettingVM = new();


            if (JsonHM.ReadProjectDistributeListJson(out var RPDList))
            {
                RPDList.ForEach(PDistribute =>
                {
                    //PDistribute.ProductProjectNameList
                    PDistribute.ProductProjectVMObservableCollection = ProductSettingVM.ProductProjectVMObservableCollection;
                    PDistribute.SeparateBoxVMObservableCollection = ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection;
                    //將製品清單拆分成兩份
                    TypeSettingSettingVM.ProjectDistributeVMObservableCollection.Add(new ProjectDistributeViewModel(PDistribute)
                    {
                        IsInDistributePage = false
                        //重新繫結
                    });

                });
            }


            MachineMonitorVM = new MachineMonitorViewModel(new MachineMonitorModel())
            {

            };


            MachineFunctionVM = new MachineFunctionViewModel()
            {

            };
        }



        public MachanicalSpecificationViewModel MachanicalSpecificationVM { get; set; }
        public StampingFontChangedViewModel StampingFontChangedVM { get; set; }
        public ParameterSettingViewModel ParameterSettingVM { get; set; }
        public ProductSettingViewModel ProductSettingVM { get; set; }
        public TypeSettingSettingViewModel TypeSettingSettingVM { get; set; }
        public MachineMonitorViewModel MachineMonitorVM
        {
            get;
            set;
        }
        public MachineFunctionViewModel MachineFunctionVM
        {
            get;
            set;
        }
        public DXObservableCollection<OperatingLogViewModel> LogDataObservableCollection
        {
            get
            {
                return Singletons.LogDataSingleton.Instance.DataObservableCollection;
            }
        }
    }
}
