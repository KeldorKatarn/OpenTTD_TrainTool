// <copyright file="TrainsViewModel.cs" company="VacuumBreather">
//      Copyright © 2014 VacuumBreather. All rights reserved.
// </copyright>
// <license type="X11/MIT">
//      Permission is hereby granted, free of charge, to any person obtaining a copy
//      of this software and associated documentation files (the "Software"), to deal
//      in the Software without restriction, including without limitation the rights
//      to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//      copies of the Software, and to permit persons to whom the Software is
//      furnished to do so, subject to the following conditions:
//      The above copyright notice and this permission notice shall be included in
//      all copies or substantial portions of the Software.
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//      IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//      FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//      AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//      LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//      OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//      THE SOFTWARE.
// </license>

namespace TrainTool.ViewModel.TrainSetScreens
{
    #region Using Directives

    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows.Data;
    using Helpers;
    using Model;
    using Services;

    #endregion

    /// <summary>
    ///     ITrainToolScreen implementation for the trains subscreen of the train set.
    /// </summary>
    [Export(typeof(IMainSubScreen))]
    public sealed class TrainsViewModel : TrainToolScreenBase, IMainSubScreen
    {
        #region Fields

        private ListCollectionView _trainsView;
        private string _trainName;
        private int _year;
        private int _mass;
        private int _power;
        private int _maxSpeed;
        private int _maxTractiveEffort;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TrainsViewModel" /> class.
        /// </summary>
        /// <param name="dialogService">The dialog service.</param>
        [ImportingConstructor]
        public TrainsViewModel(IDialogService dialogService)
            : base(0, dialogService)
        {
            DisplayName = "Trains";
        }

        #endregion

        #region Instance Properties

        /// <summary>
        ///     Deletes the selected train.
        /// </summary>
        public bool CanDeleteTrain
        {
            get
            {
                return TrainsView.CurrentItem != null;
            }
        }

        /// <summary>
        ///     Moves the selected train down.
        /// </summary>
        public bool CanMoveTrainDown
        {
            get
            {
                return TrainsView.CurrentPosition < (TrainsView.Cast<Train>().Count() - 1);
            }
        }

        /// <summary>
        ///     Moves the selected train up.
        /// </summary>
        public bool CanMoveTrainUp
        {
            get
            {
                return TrainsView.CurrentPosition > 0;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether a train is selected.
        /// </summary>
        /// <value>
        ///     <c>true</c> if a train is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsTrainSelected
        {
            get
            {
                return TrainsView.CurrentItem != null;
            }
        }

        /// <summary>
        ///     Gets or sets the name of the train.
        /// </summary>
        /// <value>The name of the train.</value>
        public string TrainName
        {
            get
            {
                return this._trainName;
            }
            set
            {
                if (SelectedTrain == null)
                {
                    return;
                }

                if (string.IsNullOrEmpty(value))
                {
                    SetPropertyValidationError(() => TrainName, "The train name must not be empty.");
                }
                else if (!StringValidator.IsValidString(value))
                {
                    SetPropertyValidationError(() => TrainName, "The train name contains invalid characters.");
                }
                else
                {
                    ResetPropertyValidationError(() => TrainName);

                    if (!value.Equals(SelectedTrain.Name))
                    {
                        SelectedTrain.Name = value;
                        _trainsView.Refresh();
                        OnModelChanged();
                    }
                }

                this._trainName = value;
                NotifyOfPropertyChange(() => TrainName);
                NotifyOfPropertyChange(() => TrainsView);
            }
        }

        /// <summary>
        ///     Gets or sets the year of introduction of the train.
        /// </summary>
        /// <value>
        ///     The year of introduction of the train.
        /// </value>
        public int Year
        {
            get
            {
                return this._year;
            }
            set
            {
                if (SelectedTrain == null)
                {
                    return;
                }

                if (!value.Equals(SelectedTrain.Year))
                {
                    SelectedTrain.Year = value;
                    OnModelChanged();
                }

                this._year = value;
                NotifyOfPropertyChange(() => Year);
                NotifyOfPropertyChange(() => TrainsView);
            }
        }

        /// <summary>
        ///     Gets or sets the mass of the train in metric tonnes.
        /// </summary>
        /// <value>
        ///     The mass of the train in metric tonnes.
        /// </value>
        public int Mass
        {
            get
            {
                return this._mass;
            }
            set
            {
                if (SelectedTrain == null)
                {
                    return;
                }

                if (value <= 0)
                {
                    SetPropertyValidationError(() => Mass, "The train mass must be a positive number.");
                }
                else
                {
                    ResetPropertyValidationError(() => Mass);

                    if (!value.Equals(SelectedTrain.Mass))
                    {
                        SelectedTrain.Mass = value;
                        OnModelChanged();
                    }
                }

                this._mass = value;
                NotifyOfPropertyChange(() => Mass);
            }
        }

        /// <summary>
        ///     Gets or sets the power of the train in kW.
        /// </summary>
        /// <value>
        ///     The power of the train in kW.
        /// </value>
        public int Power
        {
            get
            {
                return this._power;
            }
            set
            {
                if (SelectedTrain == null)
                {
                    return;
                }

                if (value <= 0)
                {
                    SetPropertyValidationError(() => Power, "The train power must be a positive number.");
                }
                else
                {
                    ResetPropertyValidationError(() => Power);

                    if (!value.Equals(SelectedTrain.Power))
                    {
                        SelectedTrain.Power = value;
                        OnModelChanged();
                    }
                }

                this._power = value;
                NotifyOfPropertyChange(() => Power);
            }
        }

        /// <summary>
        ///     Gets or sets the max speed of the train in km/h.
        /// </summary>
        /// <value>
        ///     The max speed of the train in km/h.
        /// </value>
        public int MaxSpeed
        {
            get
            {
                return this._maxSpeed;
            }
            set
            {
                if (SelectedTrain == null)
                {
                    return;
                }

                if (value <= 0)
                {
                    SetPropertyValidationError(() => MaxSpeed, "The train maximum speed must be a positive number.");
                }
                else
                {
                    ResetPropertyValidationError(() => MaxSpeed);

                    if (!value.Equals(SelectedTrain.MaxSpeed))
                    {
                        SelectedTrain.MaxSpeed = value;
                        OnModelChanged();
                    }
                }

                this._maxSpeed = value;
                NotifyOfPropertyChange(() => MaxSpeed);
            }
        }

        /// <summary>
        ///     Gets or sets the max tractive effort of the train in kN.
        /// </summary>
        /// <value>
        ///     The max tractive effort of the train in kN.
        /// </value>
        public int MaxTractiveEffort
        {
            get
            {
                return this._maxTractiveEffort;
            }
            set
            {
                if (SelectedTrain == null)
                {
                    return;
                }

                if (value <= 0)
                {
                    SetPropertyValidationError(
                        () => MaxTractiveEffort,
                        "The train maximum TE must be a positive number.");
                }
                else
                {
                    ResetPropertyValidationError(() => MaxTractiveEffort);

                    if (!value.Equals(SelectedTrain.MaxTractiveEffort))
                    {
                        SelectedTrain.MaxTractiveEffort = value;
                        OnModelChanged();
                    }
                }

                this._maxTractiveEffort = value;
                NotifyOfPropertyChange(() => MaxTractiveEffort);
            }
        }

        public ICollectionView TrainsView
        {
            get
            {
                return this._trainsView;
            }
        }

        private Train SelectedTrain
        {
            get
            {
                return (Train)TrainsView.CurrentItem;
            }
        }

        #endregion

        #region Instance Methods

        /// <summary>
        ///     Adds a new train.
        /// </summary>
        public void AddTrain()
        {
            var train = new Train();

            this._trainsView.AddNewItem(train);
            this._trainsView.CommitNew();
            this._trainsView.MoveCurrentTo(train);
        }

        /// <summary>
        ///     Deletes the selected train.
        /// </summary>
        public void DeleteTrain()
        {
            var currentTrain = (Train)this._trainsView.CurrentItem;

            this._trainsView.Remove(currentTrain);
        }

        /// <summary>
        ///     Moves the selected train down.
        /// </summary>
        public void MoveTrainDown()
        {
            object movedTrain = this._trainsView.CurrentItem;
            int index = this._trainsView.CurrentPosition;

            Contract.Assume(index >= 0 && index < this._trainsView.Count - 1);

            TrainSet.MoveTrainDownAt(index);

            this._trainsView.Refresh();
            this._trainsView.MoveCurrentTo(movedTrain);
        }

        /// <summary>
        ///     Moves the selected train up.
        /// </summary>
        public void MoveTrainUp()
        {
            object movedTrain = this._trainsView.CurrentItem;
            int index = this._trainsView.CurrentPosition;

            Contract.Assume(index > 0 && index < this._trainsView.Count);

            TrainSet.MoveTrainUpAt(index);

            this._trainsView.Refresh();
            this._trainsView.MoveCurrentTo(movedTrain);
        }

        /// <summary>
        ///     Called when the train set was changed.
        /// </summary>
        protected override void OnTrainSetChanged()
        {
            base.OnTrainSetChanged();

            this._trainsView = CollectionViewSource.GetDefaultView(TrainSet.Trains) as ListCollectionView;

            Contract.Assert(TrainsView != null);

            TrainsView.CollectionChanged += OnTrainsCollectionChanged;
            TrainsView.CurrentChanged += OnTrainsCurrentChanged;
            TrainsView.MoveCurrentToFirst();
            TrainsView.Refresh();

            ResetAllValidationErrors();
            UpdateProperties();
        }

        private void OnModelChanged()
        {
            EventHandler handler = ModelChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnTrainsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnModelChanged();
            Refresh();
        }

        private void OnTrainsCurrentChanged(object sender, EventArgs e)
        {
            ResetAllValidationErrors();
            UpdateProperties();
            Refresh();
        }

        private void UpdateProperties()
        {
            TrainName = (SelectedTrain != null) ? SelectedTrain.Name : string.Empty;
            Year = (SelectedTrain != null) ? SelectedTrain.Year : 1900;
            Mass = (SelectedTrain != null) ? SelectedTrain.Mass : 1;
            Power = (SelectedTrain != null) ? SelectedTrain.Power : 1;
            MaxSpeed = (SelectedTrain != null) ? SelectedTrain.MaxSpeed : 1;
            MaxTractiveEffort = (SelectedTrain != null) ? SelectedTrain.MaxTractiveEffort : 1;
        }

        #endregion

        #region IReportModelChanges Members

        /// <summary>
        ///     Occurs when any value on the model was changed.
        /// </summary>
        public event EventHandler ModelChanged;

        #endregion
    }
}