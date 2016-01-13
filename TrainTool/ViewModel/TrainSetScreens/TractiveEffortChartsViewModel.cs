// <copyright file="TractiveEffortChartsViewModel.cs" company="VacuumBreather">
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
    using Caliburn.Micro;
    using Model;

    #endregion

    /// <summary>
    ///     ITrainToolScreen implementation for the TE Charts page of the train set.
    /// </summary>
    [Export(typeof(ITrainSetScreen))]
    public class TractiveEffortChartsViewModel : Screen, ITrainSetScreen
    {
        #region Constants

        private const string Name = "TE Charts";

        #endregion

        #region Readonly & Static Fields

        private readonly IEnumerable<KeyValuePair<int, int>> _noValues = new List<KeyValuePair<int, int>>();
        private readonly int _order;

        #endregion

        #region Fields

        private bool _needsRefresh = true;
        private TrainSet _trainSet;
        private Train _selectedTrain;
        private Train _selectedAlternativeTrain;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TractiveEffortChartsViewModel" /> class.
        /// </summary>
        [ImportingConstructor]
        public TractiveEffortChartsViewModel()
        {
            this._order = 1;
            DisplayName = Name;
        }

        #endregion

        #region Instance Properties

        /// <summary>
        ///     Gets the available trains.
        /// </summary>
        /// <value>
        ///     The available trains.
        /// </value>
        public IEnumerable<Train> AvailableTrains
        {
            get
            {
                return TrainSet.Trains.OrderBy(train => train.Year);
            }
        }

        /// <summary>
        ///     Gets or sets the selected train.
        /// </summary>
        /// <value>
        ///     The selected train.
        /// </value>
        public Train SelectedTrain
        {
            get
            {
                return this._selectedTrain;
            }
            set
            {
                if (ReferenceEquals(value, this._selectedTrain))
                {
                    return;
                }

                this._selectedTrain = value;
                NotifyOfPropertyChange(() => SelectedTrain);
                NotifyOfPropertyChange(() => TractiveEffortValues);
                NotifyOfPropertyChange(() => TrainTonnageValues);
            }
        }

        /// <summary>
        ///     Gets or sets the selected alternative train.
        /// </summary>
        /// <value>
        ///     The selected alternative train.
        /// </value>
        public Train SelectedAlternativeTrain
        {
            get
            {
                return this._selectedAlternativeTrain;
            }
            set
            {
                if (ReferenceEquals(value, this._selectedAlternativeTrain))
                {
                    return;
                }

                this._selectedAlternativeTrain = value;
                NotifyOfPropertyChange(() => SelectedAlternativeTrain);
                NotifyOfPropertyChange(() => TractiveEffortValuesAlt);
                NotifyOfPropertyChange(() => TrainTonnageValuesAlt);
            }
        }

        /// <summary>
        ///     Gets the tractive effort values for the selected train.
        /// </summary>
        /// <value>
        ///     The tractive effort values for the selected train.
        /// </value>
        public IEnumerable<KeyValuePair<int, int>> TractiveEffortValues
        {
            get
            {
                return (SelectedTrain == null) ? this._noValues : GetValuesFor(SelectedTrain);
            }
        }

        /// <summary>
        ///     Gets the tractive effort values for the selected train.
        /// </summary>
        /// <value>
        ///     The tractive effort values for the selected train.
        /// </value>
        public IEnumerable<KeyValuePair<int, int>> TractiveEffortValuesAlt
        {
            get
            {
                return (SelectedTrain == null) ? this._noValues : GetValuesFor(SelectedAlternativeTrain);
            }
        }

        /// <summary>
        ///     Gets the maximum train weight values for the selected train.
        /// </summary>
        /// <value>
        ///     The maximum train weight values for the selected train.
        /// </value>
        public IEnumerable<KeyValuePair<int, int>> TrainTonnageValues
        {
            get
            {
                return (SelectedTrain == null) ? this._noValues : GetTonnageValues();
            }
        }

        /// <summary>
        ///     Gets the maximum train weight values for the selected alternative train.
        /// </summary>
        /// <value>
        ///     The maximum train weight values for the selected alternative train.
        /// </value>
        public IEnumerable<KeyValuePair<int, int>> TrainTonnageValuesAlt
        {
            get
            {
                return (SelectedTrain == null) ? this._noValues : GetTonnageValues(true);
            }
        }

        #endregion

        #region Instance Methods

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

        private IEnumerable<KeyValuePair<int, int>> GetTonnageValues(bool useAlternativeTrain = false)
        {
            IEnumerable<KeyValuePair<int, int>> values = useAlternativeTrain
                                                             ? TractiveEffortValuesAlt
                                                             : TractiveEffortValues;

            return values.Select(pair => new KeyValuePair<int, int>(pair.Key, (int)((pair.Value * 1000.0) / 17.0)));
        }

        private IEnumerable<KeyValuePair<int, int>> GetValuesFor(Train train)
        {
            // Create speed values from 0 to 200 km/h in 5 km/h steps.
            IEnumerable<int> speedValues = Enumerable.Range(0, 42).Select(val => val * 5);

            // Calculate the tractive effort at each speed step and cap at TE max.
            IEnumerable<KeyValuePair<int, int>> values = speedValues.Select(
                speed =>
                {
                    var tractiveEffort = (int)((train.Power) / (speed / 3.6));
                    tractiveEffort = speed > train.MaxSpeed ? 0 : tractiveEffort;
                    tractiveEffort = Math.Min(train.MaxTractiveEffort, tractiveEffort);

                    return new KeyValuePair<int, int>(speed, speed == 0 ? train.MaxTractiveEffort : tractiveEffort);
                });

            return values;
        }

        /// <summary>
        ///     Called when the train set was changed.
        /// </summary>
        private void OnTrainSetChanged()
        {
            RequestRefresh();
        }

        #endregion

        #region ITrainSetScreen Members

        /// <summary>
        ///     Gets or sets the underlying train set.
        /// </summary>
        /// <value>
        ///     The underlying train set.
        /// </value>
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
            else
            {
                Refresh();
            }
        }

        #endregion
    }
}