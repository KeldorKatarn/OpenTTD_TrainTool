// <copyright file="WaggonsViewModel.cs" company="VacuumBreather">
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
    ///     ITrainToolScreen implementation for the waggons subscreen of the train set.
    /// </summary>
    [Export(typeof(IMainSubScreen))]
    public sealed class WaggonsViewModel : TrainToolScreenBase, IMainSubScreen
    {
        #region Fields

        private ListCollectionView _waggonsView;
        private string _waggonName;
        private int _massEmpty;
        private int _massFull;
        private int _maxSpeed;
        private double _length;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WaggonsViewModel" /> class.
        /// </summary>
        /// <param name="dialogService">The dialog service.</param>
        [ImportingConstructor]
        public WaggonsViewModel(IDialogService dialogService)
            : base(1, dialogService)
        {
            DisplayName = "Waggons";
        }

        #endregion

        #region Instance Properties

        /// <summary>
        ///     Deletes the selected waggon.
        /// </summary>
        public bool CanDeleteWaggon
        {
            get
            {
                return WaggonsView.CurrentItem != null;
            }
        }

        /// <summary>
        ///     Moves the selected waggon down.
        /// </summary>
        public bool CanMoveWaggonDown
        {
            get
            {
                return WaggonsView.CurrentPosition < (WaggonsView.Cast<Waggon>().Count() - 1);
            }
        }

        /// <summary>
        ///     Moves the selected waggon up.
        /// </summary>
        public bool CanMoveWaggonUp
        {
            get
            {
                return WaggonsView.CurrentPosition > 0;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether a waggon is selected.
        /// </summary>
        /// <value>
        ///     <c>true</c> if a waggon is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsWaggonSelected
        {
            get
            {
                return WaggonsView.CurrentItem != null;
            }
        }

        /// <summary>
        ///     Gets or sets the name of the waggon.
        /// </summary>
        /// <value>The name of the waggon.</value>
        public string WaggonName
        {
            get
            {
                return this._waggonName;
            }
            set
            {
                if (SelectedWaggon == null)
                {
                    return;
                }

                if (string.IsNullOrEmpty(value))
                {
                    SetPropertyValidationError(() => WaggonName, "The waggon name must not be empty.");
                }
                else if (!StringValidator.IsValidString(value))
                {
                    SetPropertyValidationError(() => WaggonName, "The waggon name contains invalid characters.");
                }
                else
                {
                    ResetPropertyValidationError(() => WaggonName);

                    if (!value.Equals(SelectedWaggon.Name))
                    {
                        SelectedWaggon.Name = value;
                        _waggonsView.Refresh();
                        OnModelChanged();
                    }
                }

                this._waggonName = value;
                NotifyOfPropertyChange(() => WaggonName);
                NotifyOfPropertyChange(() => WaggonsView);
            }
        }

        /// <summary>
        ///     Gets or sets the empty mass of the waggon in metric tonnes.
        /// </summary>
        /// <value>
        ///     The empty mass of the waggon in metric tonnes.
        /// </value>
        public int MassEmpty
        {
            get
            {
                return this._massEmpty;
            }
            set
            {
                if (SelectedWaggon == null)
                {
                    return;
                }

                if (value <= 0)
                {
                    SetPropertyValidationError(() => MassEmpty, "The waggon empty mass must be a positive number.");
                }
                else
                {
                    ResetPropertyValidationError(() => MassEmpty);

                    if (!value.Equals(SelectedWaggon.MassEmpty))
                    {
                        SelectedWaggon.MassEmpty = value;
                        OnModelChanged();
                    }
                }

                this._massEmpty = value;
                NotifyOfPropertyChange(() => MassEmpty);
            }
        }

        /// <summary>
        ///     Gets or sets the full mass of the waggon in metric tonnes.
        /// </summary>
        /// <value>
        ///     The full mass of the waggon in metric tonnes.
        /// </value>
        public int MassFull
        {
            get
            {
                return this._massFull;
            }
            set
            {
                if (SelectedWaggon == null)
                {
                    return;
                }

                if (value <= 0)
                {
                    SetPropertyValidationError(() => MassFull, "The waggon full mass must be a positive number.");
                }
                else
                {
                    ResetPropertyValidationError(() => MassFull);

                    if (!value.Equals(SelectedWaggon.MassFull))
                    {
                        SelectedWaggon.MassFull = value;
                        OnModelChanged();
                    }
                }

                this._massFull = value;
                NotifyOfPropertyChange(() => MassFull);
            }
        }

        /// <summary>
        ///     Gets or sets the max speed of the waggon in km/h.
        /// </summary>
        /// <value>
        ///     The max speed of the waggon in km/h.
        /// </value>
        public int MaxSpeed
        {
            get
            {
                return this._maxSpeed;
            }
            set
            {
                if (SelectedWaggon == null)
                {
                    return;
                }

                if (value <= 0)
                {
                    SetPropertyValidationError(() => MaxSpeed, "The waggon maximum speed must be a positive number.");
                }
                else
                {
                    ResetPropertyValidationError(() => MaxSpeed);

                    if (!value.Equals(SelectedWaggon.MaxSpeed))
                    {
                        SelectedWaggon.MaxSpeed = value;
                        OnModelChanged();
                    }
                }

                this._maxSpeed = value;
                NotifyOfPropertyChange(() => MaxSpeed);
            }
        }

        /// <summary>
        ///     Gets or sets the length of the waggon.
        /// </summary>
        /// <value>
        ///     The length of the waggon.
        /// </value>
        public double Length
        {
            get
            {
                return this._length;
            }
            set
            {
                if (SelectedWaggon == null)
                {
                    return;
                }

                if (value <= 0)
                {
                    SetPropertyValidationError(() => Length, "The waggon length must be a positive number.");
                }
                else
                {
                    ResetPropertyValidationError(() => Length);

                    if (!value.Equals(SelectedWaggon.Length))
                    {
                        SelectedWaggon.Length = value;
                        OnModelChanged();
                    }
                }

                this._length = value;
                NotifyOfPropertyChange(() => Length);
            }
        }

        /// <summary>
        ///     Gets all the waggons.
        /// </summary>
        /// <value>
        ///     All the waggons.
        /// </value>
        public ICollectionView WaggonsView
        {
            get
            {
                return this._waggonsView;
            }
        }

        /// <summary>
        ///     Gets the selected waggon.
        /// </summary>
        /// <value>
        ///     The selected waggon.
        /// </value>
        private Waggon SelectedWaggon
        {
            get
            {
                return (Waggon)WaggonsView.CurrentItem;
            }
        }

        #endregion

        #region Instance Methods

        /// <summary>
        ///     Adds a new waggon.
        /// </summary>
        public void AddWaggon()
        {
            var waggon = new Waggon();

            this._waggonsView.AddNewItem(waggon);
            this._waggonsView.CommitNew();
            this._waggonsView.MoveCurrentTo(waggon);
        }

        /// <summary>
        ///     Deletes the selected waggon.
        /// </summary>
        public void DeleteWaggon()
        {
            var currentWaggon = (Waggon)this._waggonsView.CurrentItem;

            this._waggonsView.Remove(currentWaggon);
        }

        /// <summary>
        ///     Moves the selected waggon down.
        /// </summary>
        public void MoveWaggonDown()
        {
            object movedWaggon = this._waggonsView.CurrentItem;
            int index = this._waggonsView.CurrentPosition;

            Contract.Assume(index >= 0 && index < this._waggonsView.Count - 1);

            TrainSet.MoveWaggonDownAt(index);

            this._waggonsView.Refresh();
            this._waggonsView.MoveCurrentTo(movedWaggon);
        }

        /// <summary>
        ///     Moves the selected waggon up.
        /// </summary>
        public void MoveWaggonUp()
        {
            object movedWaggon = this._waggonsView.CurrentItem;
            int index = this._waggonsView.CurrentPosition;

            Contract.Assume(index > 0 && index < this._waggonsView.Count);

            TrainSet.MoveWaggonUpAt(index);

            this._waggonsView.Refresh();
            this._waggonsView.MoveCurrentTo(movedWaggon);
        }

        /// <summary>
        ///     Called when the train set was changed.
        /// </summary>
        protected override void OnTrainSetChanged()
        {
            base.OnTrainSetChanged();

            this._waggonsView = CollectionViewSource.GetDefaultView(TrainSet.Waggons) as ListCollectionView;

            Contract.Assert(WaggonsView != null);

            WaggonsView.CollectionChanged += OnWaggonsCollectionChanged;
            WaggonsView.CurrentChanged += OnWaggonsCurrentChanged;
            WaggonsView.MoveCurrentToFirst();
            WaggonsView.Refresh();

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

        private void OnWaggonsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnModelChanged();
            Refresh();
        }

        private void OnWaggonsCurrentChanged(object sender, EventArgs e)
        {
            ResetAllValidationErrors();
            UpdateProperties();
            Refresh();
        }

        private void UpdateProperties()
        {
            WaggonName = (SelectedWaggon != null) ? SelectedWaggon.Name : string.Empty;
            MassEmpty = (SelectedWaggon != null) ? SelectedWaggon.MassEmpty : 1;
            MassFull = (SelectedWaggon != null) ? SelectedWaggon.MassFull : 1;
            MaxSpeed = (SelectedWaggon != null) ? SelectedWaggon.MaxSpeed : 1;
            Length = (SelectedWaggon != null) ? SelectedWaggon.Length : 0.5;
        }

        #endregion

        #region IMainSubScreen Members

        /// <summary>
        ///     Occurs when any value on the model was changed.
        /// </summary>
        public event EventHandler ModelChanged;

        #endregion
    }
}