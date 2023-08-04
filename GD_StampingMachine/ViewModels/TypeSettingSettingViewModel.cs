﻿using DevExpress.CodeParser;
using DevExpress.Mvvm.DataAnnotations;
using GD_CommonLibrary.Extensions;
using GD_StampingMachine.ViewModels.ProductSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using GD_CommonLibrary;
using Newtonsoft.Json;
using DevExpress.Mvvm.Native;
using DevExpress.Data.Extensions;
using GD_CommonLibrary.Method;
using GD_StampingMachine.Method;

namespace GD_StampingMachine.ViewModels
{
    //public class TypeSettingSettingModel
    //{

        /// <summary>
        /// 製品清單
        /// </summary>
        //public ObservableCollection<ProductProjectViewModel> ProductProjectVMObservableCollection { get; set; } 
        /// <summary>
        /// 盒子列表
        /// </summary>
        //public ObservableCollection<ParameterSetting.SeparateBoxViewModel> SeparateBoxVMObservableCollection { get; set; } 
    //}
    /// <summary>
    /// 排版設定
    /// </summary>
    public class TypeSettingSettingViewModel : BaseViewModelWithLog
    {

        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_TypeSettingSettingViewModel");

        public TypeSettingSettingViewModel()
        {

        }


        /// <summary>
        /// 建立用的model
        /// </summary>
        public ProjectDistributeModel NewProjectDistribute
        {
            get; set;
        } = new();


        public ProductSettingViewModel ProductSettingVM { get; set; }=new();
        public ParameterSettingViewModel ParameterSettingVM { get; set; } = new();


        [JsonIgnore]
        public ICommand CreateProjectDistributeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    AddLogData("btnAddProject");

                    if(NewProjectDistribute.ProjectDistributeName == null)
                    {
                        MethodWinUIMessageBox.CanNotCreateProjectFileNameIsEmpty();
                        return;
                    }
               
                    if(ProjectDistributeVMObservableCollection.FindIndex(x=>x.ProjectDistributeName == NewProjectDistribute.ProjectDistributeName) !=-1)
                    {
                        MethodWinUIMessageBox.CanNotCreateProject(NewProjectDistribute.ProjectDistributeName); 
                        return;
                    }

                    NewProjectDistribute.CreatedDate = DateTime.Now;
                    var Clone = NewProjectDistribute.DeepCloneByJson();
                    Clone.ProductProjectVMObservableCollection = ProductSettingVM.ProductProjectVMObservableCollection;
                    Clone.SeparateBoxVMObservableCollection = ParameterSettingVM.SeparateSettingVM.SeparateBoxVMObservableCollection;
                    ProjectDistributeVMObservableCollection.Add(new ProjectDistributeViewModel(Clone));
                    var Model_IEnumerable = ProjectDistributeVMObservableCollection.Select(x => x.ProjectDistribute).ToList();
                    //存檔
                    JsonHM.WriteProjectDistributeListJson(Model_IEnumerable);

                });
            }
        }

        /// <summary>
        /// 切換工程號
        /// </summary>
        [JsonIgnore]
        public ICommand ChangeProjectDistributeCommand{get;set;}

        /// <summary>
        /// 刪除排版專案 並將盒子內的所有東西釋放回專案
        /// </summary>
        [JsonIgnore]
        public ICommand DeleteProjectDistributeCommand
        {
            get => new RelayCommand(() =>
            {
                var Removed_List = new List<int>();
                foreach(var _selectItem in ProjectDistributeSelectedItems)
                {
                    _selectItem.StampingBoxPartsVM.BoxPartsParameterVMObservableCollection.ForEach(obj =>
                    {
                        if (_selectItem.ProjectDistributeName == _selectItem.StampingBoxPartsVM.ProjectDistributeName)
                        {
                            obj.DistributeName = null;
                            obj.BoxIndex = null;
                        }
                    });
                    _selectItem.SaveProductProjectVMObservableCollection();

                    var F_Index = ProjectDistributeVMObservableCollection.FindIndex(x => x == _selectItem);
                    Removed_List.Add(F_Index);
                }

                var DescendingRemoved_List = Removed_List.OrderByDescending(x => x);
                foreach(var DescendingRemovedIndex in DescendingRemoved_List)
                {
                    if(DescendingRemovedIndex != -1)
                        ProjectDistributeVMObservableCollection.RemoveAt(DescendingRemovedIndex);
                }


            });
        }



        private bool _addProjectDistributeDarggableIsPopup;
        public bool AddProjectDistributeDarggableIsPopup { get=> _addProjectDistributeDarggableIsPopup; set { _addProjectDistributeDarggableIsPopup = value;OnPropertyChanged(); } }



        public ObservableCollection<ProjectDistributeViewModel> ProjectDistributeSelectedItems { get; set; } = new();

        public ObservableCollection<ProjectDistributeViewModel> ProjectDistributeVMObservableCollection { get; set; }= new();
        [JsonIgnore]
        public DevExpress.Mvvm.ICommand<DevExpress.Mvvm.Xpf.RowClickArgs> RowDoubleClickCommand
        {
            get => new DevExpress.Mvvm.DelegateCommand<DevExpress.Mvvm.Xpf.RowClickArgs>((DevExpress.Mvvm.Xpf.RowClickArgs args) =>
            {
                if (args.Item is GD_StampingMachine.ViewModels.ProjectDistributeViewModel ProjectItem)
                {
                    if (ProjectDistributeVM != ProjectItem)
                        ProjectDistributeVM = ProjectItem;
                    ProjectDistributeVM.IsInDistributePage = true;
                    ProjectDistributeVM.PartsParameterVMObservableCollectionRefresh();
                   
                }
            });
        }

        private ProjectDistributeViewModel _projectDistributeVM;
        /// <summary>
        /// 準備加工的排版專案
        /// </summary>
        public ProjectDistributeViewModel ProjectDistributeVM { get=> _projectDistributeVM; set { _projectDistributeVM = value; OnPropertyChanged(); } }


    }
}
