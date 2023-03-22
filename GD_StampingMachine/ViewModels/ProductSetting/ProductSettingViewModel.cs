﻿using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.WindowsUI;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Model;
using GD_StampingMachine.SplashScreenWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ProductSetting
{
    public class ProductSettingViewModel : ViewModelBase
    {
        private bool _addProjectDarggableIsPopup;
        public bool AddProjectDarggableIsPopup
        {
            get
            {
                return _addProjectDarggableIsPopup;
            }
            set
            {
                _addProjectDarggableIsPopup = value;
                OnPropertyChanged(nameof(AddProjectDarggableIsPopup));
            }
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


        public string ProjectPathText
        {
            get
            {
                return CreatedProjectModel.ProjectPath;
            }
            set
            {
                CreatedProjectModel.ProjectPath = value;
                OnPropertyChanged(nameof(ProjectPathText));
            }
        }


        private ObservableCollection<ProductProjectViewModel> _productProjectVMObservableCollection;
        public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection
        {
            get
            {
                if (_productProjectVMObservableCollection == null)
                    _productProjectVMObservableCollection = new ObservableCollection<ProductProjectViewModel>();
                return _productProjectVMObservableCollection;
            }
            set
            {
                _productProjectVMObservableCollection = value;
                OnPropertyChanged(nameof(ProductProjectVMObservableCollection));
            }
        }





        public ICommand SetProjectFolder
        {
            get => new RelayCommand(() =>
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    DialogResult result = dialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        ProjectPathText = dialog.SelectedPath;
                    }
                }
            });
         }

        public ICommand CreateProjectCommand
        {
            get=> new RelayCommand(() =>
            {
                //若不clone會導致資料互相繫結
                ProductProjectVMObservableCollection.Add(new ProductProjectViewModel()
                {
                    ProductProject = (ProductProjectModel)CreatedProjectModel.Clone()
                });
                OnPropertyChanged(nameof(ProductProjectVMObservableCollection));
              //  OnPropertyChanged(nameof(Search_ProductProjectVMObservableCollection));
                /*
                ProductProjectVMObservableCollection.Add(new ProductProjectViewModel()
                {
                    ProductProject = new ProductProjectModel()
                    { 
                        Name="創典科技總公司基地(new)",
                        Number = "newAS001",
                        Form ="newQR",
                        CreateTime=DateTime.Now
                    }

                });
                */
            });
        }

        //   public ProductProjectModel CreatedProjectModel { get; set;} = new ProductProjectModel();
        public ProductProjectModel CreatedProjectModel { get; set;} = new ProductProjectModel()
           {
               Name = "NewProject",
               Number = "newAS001",
               CreateTime = DateTime.Now
           };

        public Array SheetStampingTypeEnumCollection
        {
            get
            {
                return System.Enum.GetValues(typeof(SheetStampingTypeFormEnum));
            }
        }

        public DevExpress.Mvvm.ICommand<DevExpress.Mvvm.Xpf.RowClickArgs> RowDoubleClickCommand
        {
            get => new DevExpress.Mvvm.DelegateCommand<DevExpress.Mvvm.Xpf.RowClickArgs>((DevExpress.Mvvm.Xpf.RowClickArgs args) =>
            {
                if(args.Item is GD_StampingMachine.ViewModels.ProductSetting.ProductProjectViewModel ProjectItem)
                {
                    //ProjectItem.

                }


            });
        }

        //public ObservableCollection<> Parameter { get; set; }






















    }
}
