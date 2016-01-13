// <copyright file="MainViewModel.cs" company="VacuumBreather">
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
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Linq.Expressions;
    using Caliburn.Micro;
    using Helpers;
    using Model;

    #endregion

    /// <summary>
    ///     ITrainSetScreen implementation for the main page of the train set.
    /// </summary>
    [Export(typeof(ITrainSetScreen))]
    public sealed class MainViewModel : Conductor<IMainSubScreen>.Collection.AllActive,
                                        ITrainSetScreen,
                                        IReportModelChanges
    {
        #region Constants

        private const string Name = "Main";

        #endregion

        #region Readonly & Static Fields

        private readonly int _order;
        private readonly Dictionary<string, string> _validationErrors = new Dictionary<string, string>();
        private readonly IEnumerable<IMainSubScreen> _subScreens;

        #endregion

        #region Fields

        private bool _isEditingName;
        private string _trainSetName;
        private TrainSet _trainSet;
        private bool _needsRefresh = true;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// </summary>
        /// <param name="subScreens">The sub screens.</param>
        [ImportingConstructor]
        public MainViewModel([ImportMany] IEnumerable<IMainSubScreen> subScreens)
        {
            this._subScreens = subScreens;
            this._order = 0;

            Items.AddRange(_subScreens.OrderBy(screen => screen.Order));

            foreach (var screen in Items)
            {
                screen.ModelChanged += OnActiveItemModelChanged;
            }

            DisplayName = Name;
        }

        #endregion

        #region Instance Indexers

        /// <summary>
        ///     Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>The error message.</returns>
        public string this[string columnName]
        {
            get
            {
                string errorMessage;

                this._validationErrors.TryGetValue(columnName, out errorMessage);

                return errorMessage ?? string.Empty;
            }
        }

        #endregion

        #region Instance Properties

        /// <summary>
        ///     Gets a value indicating whether this ViewModel can accept the new name of the train set.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this ViewModel can accept the new name of the train set; otherwise, <c>false</c>.
        /// </value>
        public bool CanAcceptRenaming
        {
            get
            {
                return IsEditingName && !HasPropertyValidationError(() => TrainSetName);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this ViewModel can cancel the train set renaming.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this ViewModel can cancel the train set renaming; otherwise, <c>false</c>.
        /// </value>
        public bool CanCancelRenaming
        {
            get
            {
                return IsEditingName;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this ViewModel can start the train set renaming.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this ViewModel can start the train set renaming; otherwise, <c>false</c>.
        /// </value>
        public bool CanStartRenaming
        {
            get
            {
                return !IsEditingName;
            }
        }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string TrainSetName
        {
            get
            {
                return this._trainSetName;
            }
            set
            {
                if (value == this._trainSetName)
                {
                    return;
                }

                if (string.IsNullOrEmpty(value))
                {
                    SetPropertyValidationError(() => TrainSetName, "The name must not be empty.");
                }
                else if (!StringValidator.IsValidString(value))
                {
                    SetPropertyValidationError(() => TrainSetName, "The name contains invalid characters.");
                }
                else
                {
                    ResetPropertyValidationError(() => TrainSetName);
                }

                this._trainSetName = value;
                NotifyOfPropertyChange(() => TrainSetName);
                Refresh();
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the train set name can currently be edited.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the train set name can currently be edited; otherwise, <c>false</c>.
        /// </value>
        public bool IsEditingName
        {
            get
            {
                return this._isEditingName;
            }
            private set
            {
                if (value == IsEditingName)
                {
                    return;
                }

                this._isEditingName = value;
                NotifyOfPropertyChange(() => IsEditingName);
            }
        }

        /// <summary>
        ///     Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <value>An error message indicating what is wrong with this object. The default is an empty string ("").</value>
        /// <returns>An error message indicating what is wrong with this object. The default is an empty string ("").</returns>
        public string Error
        {
            get
            {
                return string.Empty;
            }
        }

        #endregion

        #region Instance Methods

        /// <summary>
        ///     Toggles the renaming.
        /// </summary>
        public void AcceptRenaming()
        {
            if (TrainSet.Name != TrainSetName)
            {
                TrainSet.Name = TrainSetName;
                OnModelChanged();
            }

            IsEditingName = false;
            Refresh();
        }

        /// <summary>
        ///     Toggles the renaming.
        /// </summary>
        public void CancelRenaming()
        {
            TrainSetName = TrainSet.Name;
            IsEditingName = false;
            Refresh();
        }

        /// <summary>
        ///     Toggles the renaming.
        /// </summary>
        public void StartRenaming()
        {
            IsEditingName = true;
            Refresh();
        }

        /// <summary>
        ///     Called when the screen is activated.
        /// </summary>
        protected override void OnActivate()
        {
            base.OnActivate();

            if (this._needsRefresh)
            {
                Refresh();
                this._needsRefresh = false;
            }
        }

        /// <summary>
        ///     Called when the screen is deactivated.
        /// </summary>
        /// <param name="close">if set to <c>true</c> the screen will close.</param>
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (CanCancelRenaming)
            {
                CancelRenaming();
            }
        }

        /// <summary>
        ///     Determines whether the specified property has any validation errors.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property expression.</param>
        /// <returns>
        ///     <c>true</c> if the specified property has any validation errors; otherwise, <c>false</c>.
        /// </returns>
        private bool HasPropertyValidationError<TProperty>(Expression<Func<TProperty>> property)
        {
            string propertyName = property.GetMemberInfo().Name;

            return !string.IsNullOrEmpty(this._validationErrors[propertyName]);
        }

        private void OnActiveItemModelChanged(object sender, EventArgs eventArgs)
        {
            OnModelChanged();
        }

        private void OnModelChanged()
        {
            EventHandler handler = ModelChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Called when the dossier was changed.
        /// </summary>
        private void OnTrainSetChanged()
        {
            if (CanCancelRenaming)
            {
                CancelRenaming();
            }

            TrainSetName = TrainSet.Name;

            foreach (var screen in this._subScreens)
            {
                screen.TrainSet = TrainSet;
            }
        }

        /// <summary>
        ///     Resets any validation errors for the specified property.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property expression.</param>
        private void ResetPropertyValidationError<TProperty>(Expression<Func<TProperty>> property)
        {
            string propertyName = property.GetMemberInfo().Name;

            this._validationErrors[propertyName] = null;
        }

        /// <summary>
        ///     Sets a validation error for the specified property.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property expression.</param>
        /// <param name="errorMessage">The error message.</param>
        private void SetPropertyValidationError<TProperty>(Expression<Func<TProperty>> property, string errorMessage)
        {
            string propertyName = property.GetMemberInfo().Name;

            this._validationErrors[propertyName] = errorMessage;
        }

        #endregion

        #region IReportModelChanges Members

        /// <summary>
        ///     Occurs when any value on the model was changed.
        /// </summary>
        public event EventHandler ModelChanged;

        #endregion

        #region ITrainSetScreen Members

        /// <summary>
        ///     Gets or sets the underlying train set.
        /// </summary>
        /// <value>
        ///     The underlying train set.
        /// </value>
        public TrainSet TrainSet
        {
            get
            {
                return this._trainSet;
            }
            set
            {
                if (value == this._trainSet)
                {
                    return;
                }

                this._trainSet = value;
                OnTrainSetChanged();
                Refresh();
            }
        }

        /// <summary>
        ///     Gets the order of this screen.
        /// </summary>
        /// <value>
        ///     The order of this screen.
        /// </value>
        public int Order
        {
            get
            {
                return this._order;
            }
        }

        /// <summary>
        ///     Requests the refreshing of the screen.
        /// </summary>
        public void RequestRefresh()
        {
            if (!IsActive)
            {
                this._needsRefresh = true;
            }
        }

        #endregion
    }
}