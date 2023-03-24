﻿using DevExpress.Data;
using DevExpress.DataAccess.Json;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.WindowsUI;
using GD_StampingMachine.Extensions;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Method;
using GD_StampingMachine.Model;
using GD_StampingMachine.ViewModels.ParameterSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ProductSetting
{
    public class ProductProjectViewModel : ViewModelBase
    {
        public ProductProjectViewModel() 
        {
            Task.Run(() =>
            { 
                while (true)
                {
                    Thread.Sleep(100);
                    OnPropertyChanged(nameof(ProductProjectFinishProcessing));
                    //OnPropertyChanged(nameof(PartsParameterVMObservableCollection));
                } 
            });
        }



        public ProductProjectModel ProductProject { get; set; } = new ProductProjectModel();
     
        /// <summary>
        /// 進度條 會以專案內的資料為準
        /// </summary>
        public double ProductProjectFinishProcessing
        {
            get
            {
                ProductProject.FinishProgress = 0;
                if (PartsParameterVMObservableCollection.Count > 0)
                {
                    double AverageProgress = 0;
                    PartsParameterVMObservableCollection.ForEach(p =>
                    {
                        AverageProgress += p.FinishProgress / PartsParameterVMObservableCollection.Count;
                    });

                    ProductProject.FinishProgress = AverageProgress;

                }
                return ProductProject.FinishProgress;
            }
            set
            {
                ProductProject.FinishProgress = value;
                OnPropertyChanged(nameof(ProductProject));
            }
        }



        private RelayParameterizedCommand _projectEditCommand;
        public RelayParameterizedCommand ProjectEditCommand
        {
            get
            {
                if (_projectEditCommand == null)
                {
                    _projectEditCommand = new RelayParameterizedCommand(obj =>
                    {

                    });
                }
                return _projectEditCommand;
            }
            set
            {
                _projectEditCommand = value;
                OnPropertyChanged(nameof(ProjectEditCommand));
            }
        }
        private RelayParameterizedCommand _projectDeleteCommand;
        public RelayParameterizedCommand ProjectDeleteCommand
        {
            get
            {
                if (_projectDeleteCommand == null)
                {
                    _projectDeleteCommand = new RelayParameterizedCommand(obj =>
                    {
                        if (obj is GridControl ObjGridControl)
                        {
                            if (ObjGridControl.ItemsSource is ObservableCollection<ProductProjectViewModel> GridItemSource)
                            {
                                var MessageBoxReturn = WinUIMessageBox.Show(null,
                                    (string)Application.Current.TryFindResource("Text_AskDelProject") +
                                    "\r\n" +
                                    $"{this.ProductProject.Number} - {this.ProductProject.Name}" +
                                    "?"
                                    ,
                                    (string)Application.Current.TryFindResource("Text_notify"),
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Exclamation,
                                    MessageBoxResult.None,
                                    MessageBoxOptions.None,
                                    DevExpress.Xpf.Core.FloatingMode.Window);

                                if (MessageBoxReturn == MessageBoxResult.Yes)
                                    GridItemSource.Remove(this);
                            }
                        }
                    });
                }
                return _projectDeleteCommand;
            }
            /*set
            {
                _projectDeleteCommand = value;
                OnPropertyChanged(nameof(ProjectDeleteCommand));
            }*/
        }










        private bool _isInParameterPage;
        public bool IsInParameterPage { get=> _isInParameterPage; set { _isInParameterPage = value;OnPropertyChanged(); } }
        public PartsParameterViewModel AddNewPartsParameterVM { get; set; } = new PartsParameterViewModel();
        public NumberSettingModelBase NumberSettingModelSavedComboSelected
        {
            get => AddNewPartsParameterVM.NumberSetting;
            set
            {
                AddNewPartsParameterVM.NumberSetting = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<NumberSettingModelBase> _numberSettingModelSavedCollection;
        /// <summary>
        /// 建立零件POPUP-加工型態combobox用
        /// </summary>
        public ObservableCollection<NumberSettingModelBase> NumberSettingModelSavedCollection
        {
            get
            {
                if (_numberSettingModelSavedCollection == null)
                {
                    _numberSettingModelSavedCollection = new ObservableCollection<NumberSettingModelBase>();
                    var CsvHM = new GD_CsvHelperMethod();
                    if (ProductProject.SheetStampingTypeForm == SheetStampingTypeFormEnum.NormalSheetStamping)
                    {
                        CsvHM.ReadNumberSetting(out var SavedCollection);
                        foreach (var asd in SavedCollection)
                        {
                            _numberSettingModelSavedCollection.Add(asd);
                        }
                    }
                    if (ProductProject.SheetStampingTypeForm == SheetStampingTypeFormEnum.QRSheetStamping)
                    {
                        CsvHM.ReadQRNumberSetting(out var QRSavedCollection);
                        foreach (var asd in QRSavedCollection)
                        {
                            _numberSettingModelSavedCollection.Add(asd);
                        }
                    }
                }
                return _numberSettingModelSavedCollection;
            }
            set
            {
                _numberSettingModelSavedCollection = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SettingViewModelBase> _numberSettingModelSavedCollection_Pic;
        public ObservableCollection<SettingViewModelBase> NumberSettingModelSavedCollection_Pic
        {
            get
            {
                if (_numberSettingModelSavedCollection_Pic == null)
                {
                    _numberSettingModelSavedCollection_Pic = new ObservableCollection<SettingViewModelBase>();
                    var CsvHM = new GD_CsvHelperMethod();
                    if (ProductProject.SheetStampingTypeForm == SheetStampingTypeFormEnum.NormalSheetStamping)
                    {
                        CsvHM.ReadNumberSetting(out var SavedCollection);
                        foreach (var asd in SavedCollection)
                        {
                            _numberSettingModelSavedCollection_Pic.Add(new NumberSettingViewModel(asd));
                        }
                    }
                    if (ProductProject.SheetStampingTypeForm == SheetStampingTypeFormEnum.QRSheetStamping)
                    {
                        CsvHM.ReadQRNumberSetting(out var QRSavedCollection);
                        foreach (var asd in QRSavedCollection)
                        {
                            _numberSettingModelSavedCollection_Pic.Add(new NumberSettingViewModel(asd));
                        }
                    }
                }
                return _numberSettingModelSavedCollection_Pic;
            }
            set
            {
                _numberSettingModelSavedCollection_Pic = value;
                OnPropertyChanged();
            }
        }
        private SettingViewModelBase _settingVM;
        /// <summary>
        /// 上方的排列示意圖(純顯示)
        /// </summary>
        public SettingViewModelBase SettingVM
        {
            get => _settingVM;
            set
            {
                _settingVM = value;
                OnPropertyChanged();
            }
        }
        private PartsParameterViewModel _partsParameterViewModelSelectItem;
        /// <summary>
        /// 參數gridcontrol選擇
        /// </summary>
        public PartsParameterViewModel PartsParameterViewModelSelectItem
        {
            get
            {
                if (_partsParameterViewModelSelectItem != null)
                {
                    SettingVM = new NumberSettingViewModel()
                    {
                        NumberSetting = _partsParameterViewModelSelectItem.NumberSetting
                    };
                }
                return _partsParameterViewModelSelectItem;
            }
            set
            {
                _partsParameterViewModelSelectItem = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<PartsParameterViewModel> _partsParameterVMObservableCollection;
        /// <summary>
        /// GridControl
        /// </summary>
        public ObservableCollection<PartsParameterViewModel> PartsParameterVMObservableCollection
        {
            get
            {
               if (_partsParameterVMObservableCollection == null)
                    _partsParameterVMObservableCollection = new ObservableCollection<PartsParameterViewModel>();


                ProductProject.PartsParameterObservableCollection = new ObservableCollection<Model.ProductionSetting.PartsParameterModel>();
                _partsParameterVMObservableCollection.ForEach(obj =>
                {
                        ProductProject.PartsParameterObservableCollection.Add(obj.PartsParameter);
                });
                return _partsParameterVMObservableCollection;
            }
            set
            {
                _partsParameterVMObservableCollection = value;
                OnPropertyChanged(nameof(PartsParameterVMObservableCollection));
            }
        }




        /// <summary>
        /// 建立零件
        /// </summary>
        public ICommand CreatePartCommand
        {
            get => new RelayCommand(() =>
            {
                PartsParameterVMObservableCollection.Add(AddNewPartsParameterVM.DeepCloneByJson());
                //儲存 ProductProject
                SaveProductProject();
            });
        }
        public ICommand SaveProductProjectCommand 
        {
            get => new RelayCommand(() =>
            {
                if (string.IsNullOrWhiteSpace(ProductProject.ProjectPath) || string.IsNullOrWhiteSpace(Path.GetFileNameWithoutExtension(ProductProject.ProjectPath)))
                {
                    //跳出彈跳式視窗
                    using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                    {
                        System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                        if (result == System.Windows.Forms.DialogResult.OK)
                            ProductProject.ProjectPath = dialog.SelectedPath;
                    }
                }

                SaveProductProject();
            });
        }
        public bool SaveProductProject()
        {

            if (ProductProject.ProjectPath != null)
            {
                if (!Path.HasExtension(ProductProject.ProjectPath))
                {
                    ProductProject.ProjectPath = Path.Combine(ProductProject.ProjectPath, ProductProject.Name , ".csv");
                    ProductProject.EditTime= DateTime.Now;
                }

                return new GD_CsvHelperMethod().WriteProductProject(ProductProject.ProjectPath, ProductProject);


            }

            return false;
        }

        private bool _addParameterDarggableIsPopup;
        public bool AddParameterDarggableIsPopup
        {
            get
            {
                return _addParameterDarggableIsPopup;
            }
            set
            {
                _addParameterDarggableIsPopup = value;
                OnPropertyChanged(nameof(AddParameterDarggableIsPopup));
            }
        }


        private bool _importParameterDarggableIsPopup;
        public bool ImportParameterDarggableIsPopup
        {
            get
            {
                return _importParameterDarggableIsPopup;
            }
            set
            {
                _importParameterDarggableIsPopup = value;
                OnPropertyChanged(nameof(ImportParameterDarggableIsPopup));
            }
        }
        private bool _exportParameterDarggableIsPopup;
        public bool ExportParameterDarggableIsPopup
        {
            get
            {
                return _exportParameterDarggableIsPopup;
            }
            set
            {
                _exportParameterDarggableIsPopup = value;
                OnPropertyChanged(nameof(ExportParameterDarggableIsPopup));
            }
        }








    }
}
