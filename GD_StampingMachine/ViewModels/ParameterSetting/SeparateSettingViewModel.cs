﻿using DevExpress.Mvvm.Native;
using GD_CommonLibrary;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.GD_Model;
using GD_StampingMachine.GD_Model;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{



    public class SeparateSettingViewModel : ParameterSettingBaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("NameSeparateSettingViewModel");
        public SeparateSettingViewModel(SeparateSettingModel _SeparateSetting)
        {
            SeparateSetting= _SeparateSetting;
            initSeparateSetting();
        }

        public SeparateSettingModel SeparateSetting = new();

        private ObservableCollection<SeparateBoxViewModel> _separateBoxVMObservableCollection;
        public ObservableCollection<SeparateBoxViewModel> SeparateBoxVMObservableCollection
        {
            get
            {
                if (_separateBoxVMObservableCollection == null)
                {
                    _separateBoxVMObservableCollection = new ObservableCollection<SeparateBoxViewModel>();
                    SeparateSetting.UnifiedSetting_SeparateBoxObservableCollection.ForEach(obj =>
                    {
                        _separateBoxVMObservableCollection.Add(new SeparateBoxViewModel(obj));
                    });
                }
                return _separateBoxVMObservableCollection;
            }
            set
            {
                _separateBoxVMObservableCollection = value;
                SeparateSetting.UnifiedSetting_SeparateBoxObservableCollection = new ObservableCollection<SeparateBoxModel>();
                if (_separateBoxVMObservableCollection != null)
                {
                    _separateBoxVMObservableCollection.ForEach(obj =>
                    {
                        SeparateSetting.UnifiedSetting_SeparateBoxObservableCollection.Add(obj._separateBox);
                    });
                }
                OnPropertyChanged();
            }
        }

        private SeparateBoxViewModel _singleSetting_SeparateBoxModel;
        public SeparateBoxViewModel SingleSetting_SeparateBoxModel
        {
            get=> _singleSetting_SeparateBoxModel??=new SeparateBoxViewModel(SeparateSetting.SingleSetting_SeparateBox);

            set
            {
                _singleSetting_SeparateBoxModel = value;
                SeparateSetting.SingleSetting_SeparateBox = null;
                if (_singleSetting_SeparateBoxModel != null)
                {
                    SeparateSetting.SingleSetting_SeparateBox = _singleSetting_SeparateBoxModel._separateBox;
                }
                OnPropertyChanged();
            }
        }


        public double SingleSetting_SeparateBoxValue
        {
            get
            {
                if(SettingType == SettingTypeEnum.UnifiedSetting)
                {
                    SeparateBoxVMObservableCollection.ForEach(x => x.BoxSliderValue = SeparateSetting.SingleSetting_SeparateBox.BoxSliderValue);
                }
                return SeparateSetting.SingleSetting_SeparateBox.BoxSliderValue;
            }
            set
            {
                SeparateSetting.SingleSetting_SeparateBox.BoxSliderValue = value;
                OnPropertyChanged(nameof(SingleSetting_SeparateBoxValue));
            }
        }

        public SettingTypeEnum SettingType
        {
            get => SeparateSetting.SettingType;
            set
            {
                SeparateSetting.SettingType = value;
                if (SeparateSetting.SettingType == SettingTypeEnum.SingleSetting)
                {
                    SeparateBoxVMObservableCollection.ForEach(x =>
                    {
                        x.BoxSliderIsEnabled = true;
                    });
                }
                if (SeparateSetting.SettingType == SettingTypeEnum.UnifiedSetting)
                {
                    SeparateBoxVMObservableCollection.ForEach(x =>
                    {
                        x.BoxSliderIsEnabled = false;
                    });
                }


                OnPropertyChanged();
            }
        }



        public override ICommand RecoverSettingCommand
        {
            get => new RelayCommand(() =>
            {
                SeparateSetting = new SeparateSettingModel();
                initSeparateSetting();
            });
        }

        public override ICommand SaveSettingCommand
        {
            get => new RelayCommand(() =>
            {
                JsonHM.WriteParameterSettingJsonSetting(Method.GD_JsonHelperMethod.ParameterSettingNameEnum.SeparateSetting, SeparateSetting, true);       
            });
        }
        public override ICommand LoadSettingCommand
        {
            get => new RelayCommand(() =>
            {
                if (JsonHM.ReadParameterSettingJsonSetting(Method.GD_JsonHelperMethod.ParameterSettingNameEnum.SeparateSetting, out SeparateSettingModel SSetting, true))
                {
                    SeparateSetting = SSetting;
                    OnPropertyChanged(nameof(SettingType));
                    OnPropertyChanged(nameof(SingleSetting_SeparateBoxValue));
                    OnPropertyChanged(nameof(SeparateBoxVMObservableCollection));
                }
            });
        }

        public override ICommand DeleteSettingCommand => throw new NotImplementedException();

        public ICommand SetAllSeparateBoxIsEnabled
        {
            get => new RelayParameterizedCommand(Parameter =>
            {
                if(Parameter is bool ParameterBoolean)
                {
                    SeparateBoxVMObservableCollection.ForEach(obj =>
                    {
                        obj.BoxIsEnabled = ParameterBoolean;
                    });
                }

            });
        }



        private void initSeparateSetting()
        {
            SingleSetting_SeparateBoxValue = 0;
            if (SeparateSetting.UnifiedSetting_SeparateBoxObservableCollection.Count == 0)
            {
                for (int i = 1; i <= 10; i++)
                {
                    SeparateBoxVMObservableCollection.Add(new SeparateBoxViewModel()
                    {
                        BoxIndex = i,
                        BoxSliderValue = 0,
                        BoxIsEnabled = true,
                    }) ;
                }
            }
        }





    }
}
