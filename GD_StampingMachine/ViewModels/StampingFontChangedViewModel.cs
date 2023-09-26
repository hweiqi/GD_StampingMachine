﻿using DevExpress.CodeParser;
using DevExpress.Data.Extensions;
using DevExpress.Mvvm.Native;
using GD_CommonLibrary.Extensions;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Method;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GD_CommonLibrary;
using Newtonsoft.Json;
using System.Threading;
using CommunityToolkit.Mvvm.Input;
using DevExpress.XtraPrinting.Preview;
using GD_StampingMachine.Singletons;

namespace GD_StampingMachine.ViewModels
{
    public class StampingFontChangedViewModel : GD_CommonLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_StampingFontChangedViewModel");

        public StampMachineDataSingleton StampMachineData { get; } = StampMachineDataSingleton.Instance;


        private ObservableCollection<StampingTypeViewModel> _stampingTypeVMObservableCollection;
        public ObservableCollection<StampingTypeViewModel> StampingTypeVMObservableCollection
        {
            get
            {
                _stampingTypeVMObservableCollection ??= new ObservableCollection<StampingTypeViewModel>();
                return _stampingTypeVMObservableCollection;
            }
            set
            {
                _stampingTypeVMObservableCollection = value;
                OnPropertyChanged();
            }
        }


        private ObservableCollection<StampingTypeViewModel> _unusedStampingTypeVMObservableCollection;
        public ObservableCollection<StampingTypeViewModel> UnusedStampingTypeVMObservableCollection
        {
            get
            {
                _unusedStampingTypeVMObservableCollection ??= new ObservableCollection<StampingTypeViewModel>();
                return _unusedStampingTypeVMObservableCollection;
            }
            set
            {
                _unusedStampingTypeVMObservableCollection = value;
                OnPropertyChanged(nameof(UnusedStampingTypeVMObservableCollection));
            }
        }


        private StampingTypeViewModel _stampingFontSelected;
        /// <summary>
        /// 鋼印機上的字模
        /// </summary>
        
        public StampingTypeViewModel StampingFontSelected
        {
            get
            {
                return _stampingFontSelected;
            }
            set
            {
                _stampingFontSelected = value;
                OnPropertyChanged(nameof(StampingFontSelected));
            }
        }
        /// <summary>
        /// 被新建出來還沒放上去的字模/被換下來的字模
        /// </summary>
        public StampingTypeViewModel UnusedStampingFontSelected { get; set; }

        public RelayCommand StampingFontReplaceCommand
        {
            get => new RelayCommand(() =>
            {
                if (StampingFontSelected != null && UnusedStampingFontSelected != null)
                {
                    StampingTypeModelExchanged();
                }
            });
        }

        private void StampingTypeModelExchanged()
        {
            var FontString = StampingFontSelected.StampingTypeString;
            var FontStringNumber = StampingFontSelected.StampingTypeNumber;
            var FontStringUseCount = StampingFontSelected.StampingTypeUseCount;

            var UnusedFontString = UnusedStampingFontSelected.StampingTypeString;
            var UnusedFontStringNumber = UnusedStampingFontSelected.StampingTypeNumber;
            var UnusedFontStringUseCount = UnusedStampingFontSelected.StampingTypeUseCount;

            var ST_index = StampingTypeVMObservableCollection.FindIndex(x => x == StampingFontSelected);
            var UST_index = UnusedStampingTypeVMObservableCollection.FindIndex(x => x == UnusedStampingFontSelected);

            var UsedS = StampingTypeVMObservableCollection[ST_index].DeepCloneByJson();
            var UnusedS = UnusedStampingTypeVMObservableCollection[UST_index].DeepCloneByJson();

            StampingTypeVMObservableCollection[ST_index] = UnusedS;
            UnusedStampingTypeVMObservableCollection[UST_index] = UsedS;

        }


        private ObservableCollection<StampingTypeViewModel> _newUnusedStampingFont;
        [JsonIgnore]
        public ObservableCollection<StampingTypeViewModel> NewUnusedStampingFont
          {
              get
              {
                  if (_newUnusedStampingFont == null)
                  {
                      _newUnusedStampingFont = new ObservableCollection<StampingTypeViewModel>();
                  }

                  if (_newUnusedStampingFont.Count == 0)
                  {
                      _newUnusedStampingFont.Add(new StampingTypeViewModel(new GD_Model.StampingTypeModel
                      {
                          StampingTypeNumber = 0,
                          StampingTypeUseCount = 0,
                          StampingTypeString = null,
                          IsNewAddStamping = true,
                      }));
                  };
                /*if (_newUnusedStampingFont.Count > 1)
                {
                    _newUnusedStampingFont.remove
                }*/

                return _newUnusedStampingFont;
              }
              set
              {
                  _newUnusedStampingFont = value;
                  OnPropertyChanged(nameof(NewUnusedStampingFont));
              }
          }


        public ICommand UnusedStampingFontAddCommand
        {
            get => new RelayCommand(() =>
            {
                
                var FirstFont = NewUnusedStampingFont.FirstOrDefault().DeepCloneByJson();
                FirstFont.IsNewAddStamping = false;
                UnusedStampingTypeVMObservableCollection.Add(FirstFont);
            });
        }

        public ICommand UnusedStampingFontDelCommand
        {
            get => new RelayCommand(() =>
            {
                if (UnusedStampingFontSelected != null)
                {
                    //下列配合DragDrop會導致介面異常 已停用
                    UnusedStampingTypeVMObservableCollection.Remove(UnusedStampingFontSelected);




                }

            });
        }



        private double _stampingFontTurntable_RorateAngle;
        public double StampingFontTurntable_RorateAngle
        {
            get => _stampingFontTurntable_RorateAngle;
            set
            {
                _stampingFontTurntable_RorateAngle = value;
                OnPropertyChanged();
            }
        }

        private StampingTypeModelMartixViewModel _stampingTypeModelMartix= new();

        public StampingTypeModelMartixViewModel StampingTypeModelMartix
        {
            get => _stampingTypeModelMartix; 
            set
            {
                _stampingTypeModelMartix = value;
                OnPropertyChanged(nameof(StampingTypeModelMartix));
            }
        }


       
     


        private SweepDirection? _direction;
        [JsonIgnore]
        public SweepDirection? Direction
        {
            get=> _direction; 
            set 
            { 
                _direction = value;
                OnPropertyChanged(); 
            }
        }

        CancellationTokenSource cancellationToken = new CancellationTokenSource();

        private StampingTypeViewModel _stampingTypeModel_readyStamping;
        [JsonIgnore]
        public StampingTypeViewModel StampingTypeModel_ReadyStamping
        {
            get
            {
                if (_stampingTypeModel_readyStamping == null)
                {
                    if (_stampingTypeVMObservableCollection.Count != 0)
                    {
                        _stampingTypeModel_readyStamping = _stampingTypeVMObservableCollection.FirstOrDefault();
                    }
                }
                return _stampingTypeModel_readyStamping;
            }
            set
            {
                //如果沒動則不須刷新
                _stampingTypeModel_readyStamping = value;
                OnPropertyChanged();
                Task.Run(async () =>
                {
                    cancellationToken.Cancel();
                    await Task.Delay(100);
                    cancellationToken = new CancellationTokenSource();
                    CancellationToken token = cancellationToken.Token;
                    await Task.Run(async () =>
                    {
                        var FIndex = StampingTypeVMObservableCollection.ToList().FindIndex(x => x.Equals(_stampingTypeModel_readyStamping));
                        if (FIndex != -1)
                        {
                            StampingTypeModelMartix = new();
                            //離原點差距的角度-以逆時針計算

                            double TargetAngle = -360 * FIndex / StampingTypeVMObservableCollection.Count;
                            int ClockDirection = 1;
                            if (Direction == null)
                            {
                                int ReverseInt = 1;
                                if (Math.Abs(TargetAngle - StampingFontTurntable_RorateAngle) > 180)
                                {
                                    ReverseInt = -1;
                                }

                                if (TargetAngle - StampingFontTurntable_RorateAngle > 0)
                                    ClockDirection = 1 * ReverseInt;
                                else
                                    ClockDirection = -1 * ReverseInt;
                            }
                            if (Direction == SweepDirection.Clockwise)
                            {
                                ClockDirection = -1;
                            }
                            if (Direction == SweepDirection.Counterclockwise)
                            {
                                ClockDirection = 1;
                            }
                            try
                            {
                                StampingTypeVMObservableCollection.ForEach(x => { x.StampingIsUsing = false; });
                                StampingTypeModelMartix.BottomStampingTypeModel = StampingTypeVMObservableCollection[FIndex];
                                double RotateGap = StampingTypeVMObservableCollection.Count / 4;

                                var LeftIndex = FIndex + RotateGap * 1;
                                while (LeftIndex >= StampingTypeVMObservableCollection.Count)
                                {
                                    LeftIndex -= StampingTypeVMObservableCollection.Count;
                                }
                                StampingTypeModelMartix.LeftStampingTypeModel = StampingTypeVMObservableCollection[(int)LeftIndex];

                                var TopIndex = FIndex + RotateGap * 2;
                                while (TopIndex >= StampingTypeVMObservableCollection.Count)
                                {
                                    TopIndex -= StampingTypeVMObservableCollection.Count;
                                }
                                StampingTypeModelMartix.TopStampingTypeModel = StampingTypeVMObservableCollection[(int)TopIndex];

                                var RightIndex = FIndex + RotateGap * 3;
                                while (RightIndex >= StampingTypeVMObservableCollection.Count)
                                {
                                    RightIndex -= StampingTypeVMObservableCollection.Count;
                                }
                                StampingTypeModelMartix.RightStampingTypeModel = StampingTypeVMObservableCollection[(int)RightIndex];


                            }
                            catch (Exception ex)
                            {

                            }

                            while (true)
                            {
                                if (token.IsCancellationRequested)
                                    break;
                                if (StampingFontTurntable_RorateAngle > 360)
                                {
                                    StampingFontTurntable_RorateAngle -= 360;
                                }
                                if (StampingFontTurntable_RorateAngle < -360)
                                {
                                    StampingFontTurntable_RorateAngle += 360;
                                }


                                if (Math.Abs(TargetAngle - StampingFontTurntable_RorateAngle) < 1.2 ||
                                    Math.Abs(TargetAngle + 360 - StampingFontTurntable_RorateAngle) < 1.2)
                                {
                                    break;
                                }
                                StampingFontTurntable_RorateAngle += ClockDirection * 0.5;
                                //Thread.Sleep(1);
                                await Task.Delay(1);
                            }

                            StampingFontTurntable_RorateAngle = TargetAngle;
                            StampingTypeVMObservableCollection.ForEach(x => { x.StampingIsUsing = false; });
                            StampingTypeVMObservableCollection[FIndex].StampingIsUsing = true;

                        }
                    }, token);
                });
            }
        }


        private AsyncRelayCommand<SelectionChangedEventArgs> _stamping_SelectionChangedCommand;
        [JsonIgnore]
        public AsyncRelayCommand<SelectionChangedEventArgs> Stamping_SelectionChangedCommand
        {
            get => _stamping_SelectionChangedCommand ??= new AsyncRelayCommand<SelectionChangedEventArgs>(async(e, token) =>
            {
                try
                {
                    
                    /*
                    var FIndex = StampingTypeVMObservableCollection.ToList().FindIndex(x => x.Equals(_stampingTypeModel_readyStamping));
                    if (FIndex != -1)
                    {
                        StampingTypeModelMartix = new();
                        //離原點差距的角度-以逆時針計算

                        double TargetAngle = -360 * FIndex / StampingTypeVMObservableCollection.Count;
                        int ClockDirection = 1;
                        if (Direction == null)
                        {
                            int ReverseInt = 1;
                            if (Math.Abs(TargetAngle - StampingFontTurntable_RorateAngle) > 180)
                            {
                                ReverseInt = -1;
                            }

                            if (TargetAngle - StampingFontTurntable_RorateAngle > 0)
                                ClockDirection = 1 * ReverseInt;
                            else
                                ClockDirection = -1 * ReverseInt;
                        }
                        if (Direction == SweepDirection.Clockwise)
                        {
                            ClockDirection = -1;
                        }
                        if (Direction == SweepDirection.Counterclockwise)
                        {
                            ClockDirection = 1;
                        }
                        try
                        {
                            StampingTypeVMObservableCollection.ForEach(x => { x.StampingIsUsing = false; });
                            StampingTypeModelMartix.BottomStampingTypeModel = StampingTypeVMObservableCollection[FIndex];
                            double RotateGap = StampingTypeVMObservableCollection.Count / 4;

                            var LeftIndex = FIndex + RotateGap * 1;
                            while (LeftIndex >= StampingTypeVMObservableCollection.Count)
                            {
                                LeftIndex -= StampingTypeVMObservableCollection.Count;
                            }
                            StampingTypeModelMartix.LeftStampingTypeModel = StampingTypeVMObservableCollection[(int)LeftIndex];

                            var TopIndex = FIndex + RotateGap * 2;
                            while (TopIndex >= StampingTypeVMObservableCollection.Count)
                            {
                                TopIndex -= StampingTypeVMObservableCollection.Count;
                            }
                            StampingTypeModelMartix.TopStampingTypeModel = StampingTypeVMObservableCollection[(int)TopIndex];

                            var RightIndex = FIndex + RotateGap * 3;
                            while (RightIndex >= StampingTypeVMObservableCollection.Count)
                            {
                                RightIndex -= StampingTypeVMObservableCollection.Count;
                            }
                            StampingTypeModelMartix.RightStampingTypeModel = StampingTypeVMObservableCollection[(int)RightIndex];


                        }
                        catch (Exception ex)
                        {

                        }

                        while (true)
                        {
                            token.ThrowIfCancellationRequested();
                            if (StampingFontTurntable_RorateAngle > 360)
                            {
                                StampingFontTurntable_RorateAngle -= 360;
                            }
                            if (StampingFontTurntable_RorateAngle < -360)
                            {
                                StampingFontTurntable_RorateAngle += 360;
                            }


                            if (Math.Abs(TargetAngle - StampingFontTurntable_RorateAngle) < 1.2 ||
                                Math.Abs(TargetAngle + 360 - StampingFontTurntable_RorateAngle) < 1.2)
                            {
                                break;
                            }
                            StampingFontTurntable_RorateAngle += ClockDirection * 0.5;
                            await Task.Delay(1);
                        }


                        StampingFontTurntable_RorateAngle = TargetAngle;
                        StampingTypeVMObservableCollection.ForEach(x => { x.StampingIsUsing = false; });
                        StampingTypeVMObservableCollection[FIndex].StampingIsUsing = true;

                    }*/
                }
                catch (OperationCanceledException)
                {

                }
                catch (Exception ex)
                {

                }
            }
            , e=> !_stamping_SelectionChangedCommand.IsRunning);
        }



        /*動起來很好看 所以保留在這裡*/
        /*private bool StampingTypeModel_ReadyStamping_IsRotating = false;
        private StampingTypeModel _stampingTypeModel_readyStamping;
        public StampingTypeModel StampingTypeModel_ReadyStamping
        {
            get => _stampingTypeModel_readyStamping;
            set
            {
                _stampingTypeModel_readyStamping = value;
                OnPropertyChanged(nameof(StampingTypeModel_ReadyStamping));
                var FIndex = StampingTypeVMObservableCollection.ToList().FindIndex(x => x.Equals(_stampingTypeModel_readyStamping));
                if (FIndex != -1)
                {
                    //離原點差距的角度-以逆時針計算
                    double TargetAngle = -360 * FIndex / StampingTypeVMObservableCollection.Count;

                    var AngleGap = Math.Abs(TargetAngle - StampingFontTurntable_RorateAngle);
                    if (TargetAngle > StampingFontTurntable_RorateAngle)
                    {
                        AngleGap += 360;
                    }
                    int SleepTime = 0;
                    if (AngleGap != 0)
                    {
                        SleepTime = Convert.ToInt32(360 / AngleGap);
                        if (SleepTime >= 5)
                            SleepTime = 5;
                    }

                    //延遲旋轉
                    Task.Run(() =>
                    {
                        if (StampingTypeModel_ReadyStamping_IsRotating)
                        {
                            StampingTypeModel_ReadyStamping_IsRotating = false;
                        }

                        //先取得目前角度->方向逆時針
                        lock (balanceLock)
                        {
                            var CurrentAngle = StampingFontTurntable_RorateAngle;
                            StampingTypeModel_ReadyStamping_IsRotating = true;
                            while (StampingTypeModel_ReadyStamping_IsRotating)
                            {
                                if (StampingFontTurntable_RorateAngle < -360)
                                {
                                    StampingFontTurntable_RorateAngle += 360;
                                }
                                if (StampingFontTurntable_RorateAngle > 360)
                                {
                                    StampingFontTurntable_RorateAngle -= 360;
                                }
                                //以0.1度進行逆時針旋轉  超過360度需要處理?

                                if (Math.Abs(TargetAngle - StampingFontTurntable_RorateAngle) < 1.2)
                                {
                                    break;
                                }
                                StampingFontTurntable_RorateAngle -= 0.5;
                                System.Threading.Thread.Sleep(SleepTime);
                            }
                            StampingFontTurntable_RorateAngle = TargetAngle;
                            StampingTypeModel_ReadyStamping_IsRotating = false;
                        }

                    });
                }
            }
        }*/





        [JsonIgnore]
        /// <summary>
        /// DropDarg
        /// </summary>
        public StampingTypeDropTarget DragStampingTypeDropTarget { get; set; } = new StampingTypeDropTarget();

    }

    public class StampingTypeModelMartixViewModel : GD_CommonLibrary.BaseViewModel
    {
        [JsonIgnore]
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_StampingFontChangedViewModel");
        private StampingTypeViewModel _bottomStampingTypeModel;
        private StampingTypeViewModel _rightStampingTypeModel;
        private StampingTypeViewModel _topStampingTypeModel;
        private StampingTypeViewModel _leftStampingTypeModel;

        [JsonIgnore]
        public StampingTypeViewModel BottomStampingTypeModel
        {
            get=> _bottomStampingTypeModel;
            set
            {
                _bottomStampingTypeModel = value;
                OnPropertyChanged(nameof(BottomStampingTypeModel));
            }
        }
        [JsonIgnore]
        public StampingTypeViewModel RightStampingTypeModel
        {
            get => _rightStampingTypeModel;
            set
            {
                _rightStampingTypeModel = value;
                OnPropertyChanged(nameof(RightStampingTypeModel));
            }
        }
        [JsonIgnore]
        public StampingTypeViewModel TopStampingTypeModel
        {
            get => _topStampingTypeModel;
            set
            {
                _topStampingTypeModel = value;
                OnPropertyChanged(nameof(TopStampingTypeModel));
            }
        }
        [JsonIgnore]
        public StampingTypeViewModel LeftStampingTypeModel
        {
            get => _leftStampingTypeModel; set
            {
                _leftStampingTypeModel = value;
                OnPropertyChanged(nameof(LeftStampingTypeModel));
            }
        }
    }



}
