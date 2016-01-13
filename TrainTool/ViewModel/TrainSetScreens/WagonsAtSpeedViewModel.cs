// <copyright file="WagonsAtSpeedViewModel.cs" company="VacuumBreather">
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
    ///     ITrainToolScreen implementation for the Waggons at speed page of the train set.
    /// </summary>
    [Export(typeof(ITrainSetScreen))]
    public class WaggonsAtSpeedViewModel : Screen, ITrainSetScreen
    {
        #region Enums

        /// <summary>
        ///     An enum with values describing the hilliness of the terrain along a train line.
        /// </summary>
        public enum Hilliness
        {
            NoSlopes,
            OneSlope,
            MultipleSlopes
        }

        #endregion

        #region Constants

        private const string Name = "Waggons at speed";

        #endregion

        #region Readonly & Static Fields

        private readonly IEnumerable<KeyValuePair<int, int>> _noValues = new List<KeyValuePair<int, int>>();
        private readonly int _order;

        private readonly List<KeyValuePair<string, Hilliness>> _hillinessValues =
            new List<KeyValuePair<string, Hilliness>>
            {
                new KeyValuePair<string, Hilliness>(
                    "No Slopes",
                    Hilliness.NoSlopes),
                new KeyValuePair<string, Hilliness>(
                    "One Slope",
                    Hilliness.OneSlope),
                new KeyValuePair<string, Hilliness>(
                    "Multiple Slopes",
                    Hilliness.MultipleSlopes)
            };

        #endregion

        #region Fields

        private bool _needsRefresh = true;
        private TrainSet _trainSet;
        private Train _selectedTrain;
        private Waggon _selectedWaggon;
        private int _cargoFactor = 15;
        private Train _selectedAlternativeTrain;
        private int _slopePercentage = 1;
        private int _tilesBetweenSlopes;
        private Hilliness _selectedHilliness;
        private int _numLocomotives = 1;
        private int _numAltLocomotives = 1;
        private bool _useStaticFriction;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TractiveEffortChartsViewModel" /> class.
        /// </summary>
        [ImportingConstructor]
        public WaggonsAtSpeedViewModel()
        {
            this._order = 2;
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
        ///     Gets the available waggons.
        /// </summary>
        /// <value>
        ///     The available waggons.
        /// </value>
        public IEnumerable<Waggon> AvailableWaggons
        {
            get
            {
                return TrainSet.Waggons.OrderBy(waggon => waggon.Name);
            }
        }

        /// <summary>
        ///     Gets or sets the slope percentage.
        /// </summary>
        /// <value>
        ///     The slope percentage.
        /// </value>
        public int SlopePercentage
        {
            get
            {
                return this._slopePercentage;
            }
            set
            {
                if (value == this._slopePercentage)
                {
                    return;
                }

                this._slopePercentage = value;
                NotifyOfPropertyChange(() => SlopePercentage);
                RefreshCharts();
            }
        }

        /// <summary>
        ///     Gets or sets the number of locomotives.
        /// </summary>
        /// <value>
        ///     The number of locomotives.
        /// </value>
        public int NumLocomotives
        {
            get
            {
                return this._numLocomotives;
            }
            set
            {
                if (value == this._numLocomotives)
                {
                    return;
                }

                this._numLocomotives = value;
                NotifyOfPropertyChange(() => NumLocomotives);
                RefreshCharts();
            }
        }

        /// <summary>
        ///     Gets or sets the number of alternative locomotives.
        /// </summary>
        /// <value>
        ///     The number of alternative locomotives.
        /// </value>
        public int NumAltLocomotives
        {
            get
            {
                return this._numAltLocomotives;
            }
            set
            {
                if (value == this._numAltLocomotives)
                {
                    return;
                }

                this._numAltLocomotives = value;
                NotifyOfPropertyChange(() => NumAltLocomotives);
                RefreshCharts();
            }
        }

        /// <summary>
        ///     Gets or sets the minimum number of tiles between two slopes.
        /// </summary>
        /// <value>
        ///     The minimum number of tiles between two slopes.
        /// </value>
        public int TilesBetweenSlopes
        {
            get
            {
                return this._tilesBetweenSlopes;
            }
            set
            {
                if (value == this._tilesBetweenSlopes)
                {
                    return;
                }

                this._tilesBetweenSlopes = value;
                NotifyOfPropertyChange(() => TilesBetweenSlopes);
                RefreshCharts();
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
                RefreshCharts();
            }
        }

        /// <summary>
        ///     Gets or sets the cargo factor.
        /// </summary>
        /// <value>
        ///     The cargo factor.
        /// </value>
        public int CargoFactor
        {
            get
            {
                return this._cargoFactor;
            }
            set
            {
                if (value == this._cargoFactor)
                {
                    return;
                }

                this._cargoFactor = value;
                NotifyOfPropertyChange(() => CargoFactor);
                RefreshCharts();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether static friction is used.
        /// </summary>
        /// <value>
        ///     <c>true</c> if static friction is used; otherwise, <c>false</c>.
        /// </value>
        public bool UseStaticFriction
        {
            get
            {
                return this._useStaticFriction;
            }
            set
            {
                if (value == this._useStaticFriction)
                {
                    return;
                }

                this._useStaticFriction = value;
                NotifyOfPropertyChange(() => UseStaticFriction);
                RefreshCharts();
            }
        }

        /// <summary>
        ///     Gets or sets the selected waggon.
        /// </summary>
        /// <value>
        ///     The selected waggon.
        /// </value>
        public Waggon SelectedWaggon
        {
            get
            {
                return this._selectedWaggon;
            }
            set
            {
                if (ReferenceEquals(value, this._selectedWaggon))
                {
                    return;
                }

                this._selectedWaggon = value;
                NotifyOfPropertyChange(() => SelectedWaggon);
                RefreshCharts();
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
                RefreshCharts();
            }
        }

        /// <summary>
        ///     Gets the tractive effort values for the selected train.
        /// </summary>
        /// <value>
        ///     The tractive effort values for the selected train.
        /// </value>
        public IEnumerable<KeyValuePair<int, int>> SpeedToWaggonsValues
        {
            get
            {
                if (SelectedTrain == null || SelectedWaggon == null)
                {
                    return this._noValues;
                }

                return GetSpeedToWaggonsValues(
                    SelectedTrain,
                    NumLocomotives,
                    SelectedWaggon,
                    CargoFactor,
                    SlopePercentage,
                    SelectedHilliness,
                    TilesBetweenSlopes,
                    UseStaticFriction);
            }
        }

        /// <summary>
        ///     Gets the tractive effort values for the selected alternative train.
        /// </summary>
        /// <value>
        ///     The tractive effort values for the selected alternative train.
        /// </value>
        public IEnumerable<KeyValuePair<int, int>> SpeedToWaggonsValuesAlt
        {
            get
            {
                if (SelectedAlternativeTrain == null || SelectedWaggon == null)
                {
                    return this._noValues;
                }

                return GetSpeedToWaggonsValues(
                    SelectedAlternativeTrain,
                    NumAltLocomotives,
                    SelectedWaggon,
                    CargoFactor,
                    SlopePercentage,
                    SelectedHilliness,
                    TilesBetweenSlopes,
                    UseStaticFriction);
            }
        }

        /// <summary>
        ///     Gets the tractive effort values for the selected train.
        /// </summary>
        /// <value>
        ///     The tractive effort values for the selected train.
        /// </value>
        public IEnumerable<KeyValuePair<int, int>> WaggonsToSpeedValues
        {
            get
            {
                if (SelectedTrain == null || SelectedWaggon == null)
                {
                    return this._noValues;
                }

                return GetWaggonsToSpeedValues(
                    SelectedTrain,
                    NumLocomotives,
                    SelectedWaggon,
                    CargoFactor,
                    SlopePercentage,
                    SelectedHilliness,
                    TilesBetweenSlopes,
                    UseStaticFriction);
            }
        }

        /// <summary>
        ///     Gets the tractive effort values for the selected alternative train.
        /// </summary>
        /// <value>
        ///     The tractive effort values for the selected alternative train.
        /// </value>
        public IEnumerable<KeyValuePair<int, int>> WaggonsToSpeedValuesAlt
        {
            get
            {
                if (SelectedAlternativeTrain == null || SelectedWaggon == null)
                {
                    return this._noValues;
                }

                return GetWaggonsToSpeedValues(
                    SelectedAlternativeTrain,
                    NumAltLocomotives,
                    SelectedWaggon,
                    CargoFactor,
                    SlopePercentage,
                    SelectedHilliness,
                    TilesBetweenSlopes,
                    UseStaticFriction);
            }
        }

        /// <summary>
        ///     Gets or sets the selected hilliness.
        /// </summary>
        /// <value>
        ///     The selected hilliness.
        /// </value>
        public Hilliness SelectedHilliness
        {
            get
            {
                return this._selectedHilliness;
            }
            set
            {
                if (value == this._selectedHilliness)
                {
                    return;
                }

                this._selectedHilliness = value;
                NotifyOfPropertyChange(() => SelectedHilliness);
                NotifyOfPropertyChange(() => HasMultipleSlopes);
                RefreshCharts();
            }
        }

        /// <summary>
        ///     Gets a value indicating whether [has multiple slopes].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [has multiple slopes]; otherwise, <c>false</c>.
        /// </value>
        public bool HasMultipleSlopes
        {
            get
            {
                return SelectedHilliness == Hilliness.MultipleSlopes;
            }
        }

        /// <summary>
        ///     Gets all possible hilliness values.
        /// </summary>
        /// <value>
        ///     All possible hilliness values.
        /// </value>
        public IEnumerable<KeyValuePair<string, Hilliness>> HillinessValues
        {
            get
            {
                return this._hillinessValues;
            }
        }

        #endregion

        #region Class Methods

        private static IEnumerable<KeyValuePair<int, int>> GetSpeedToWaggonsValues(Train train,
            int numLocomotives,
                                                                                   Waggon waggon,
                                                                                   int cargoFactor,
                                                                                   int slopePercentage,
                                                                                   Hilliness selectedHilliness,
                                                                                   int tilesBetweenSlopes,
                                                                                   bool useStaticFriction)
        {
            // Create speed values from 0 to 200 km/h in 5 km/h steps.
            IEnumerable<int> speedValues = Enumerable.Range(0, 42).Select(value => value * 5).ToList();

            // Compute waggon weight based on cargo factor
            int waggonMass = waggon.MassEmpty + (waggon.MassFull - waggon.MassEmpty) * cargoFactor;

            // Calculate the tractive effort at each speed step and cap at TE max.
            IEnumerable<int> numWaggonsPerSpeed = speedValues.Select(
                speed =>
                {
                    var tractiveEffort = (int)((train.Power * numLocomotives) / (speed / 3.6));
                    tractiveEffort = ((speed > train.MaxSpeed) || (speed > waggon.MaxSpeed)) ? 0 : tractiveEffort;
                    tractiveEffort = Math.Min((train.MaxTractiveEffort * numLocomotives), tractiveEffort);
                    tractiveEffort = (speed == 0 ? (train.MaxTractiveEffort * numLocomotives) : tractiveEffort);

                    double tonnage = (tractiveEffort * 1000.0) / (useStaticFriction ? 27.0 : 17.0);
                    tonnage = Math.Max(0.0, tonnage - (train.Mass * numLocomotives));

                    var maxNumWaggons = (int)(tonnage / waggonMass);

                    IEnumerable<int> numberOfWaggons = Enumerable.Range(0, maxNumWaggons + 1);

                    IEnumerable<KeyValuePair<int, double>> numWaggonsToTeValue = numberOfWaggons.Select(
                        numWaggons =>
                        {
                            double numWaggonsOnSlope = 0;

                            switch (selectedHilliness)
                            {
                                case Hilliness.OneSlope:
                                {
                                    numWaggonsOnSlope = Math.Min(numWaggons, 1.0 / waggon.Length);
                                }
                                    break;

                                case Hilliness.MultipleSlopes:
                                {
                                    double trainTileLength = numWaggons * waggon.Length;
                                    double tilesWithWaggons = Math.Ceiling(trainTileLength / (tilesBetweenSlopes + 1));

                                    numWaggonsOnSlope = Math.Min(numWaggons, tilesWithWaggons / waggon.Length);
                                }
                                    break;
                            }

                            // 1/10 is Short for 9.81 / 100. It just works and is very close to the correct
                            // trigonometry based values for small slope percentages.
                            double teSlopeResistance = (numWaggonsOnSlope * waggonMass * slopePercentage) / 10.0;

                            // Compute rolling resistance efforts
                            double trainTonnage = (numWaggons * waggonMass + (train.Mass * numLocomotives));

                            double teRollingRestistance = (trainTonnage * (useStaticFriction ? 27.0 : 17.0)) / 1000.0;

                            double teTotal = (teSlopeResistance + teRollingRestistance);

                            return new KeyValuePair<int, double>(numWaggons, teTotal);
                        });

                    int numPossibleWaggons = numWaggonsToTeValue.LastOrDefault(pair => pair.Value <= tractiveEffort).Key;

                    return numPossibleWaggons;
                });

            return speedValues.Zip(
                numWaggonsPerSpeed,
                (speed, numWaggons) => new KeyValuePair<int, int>(speed, numWaggons));
        }

        private static IEnumerable<KeyValuePair<int, int>> GetWaggonsToSpeedValues(Train train,
            int numLocomotives,
                                                                                   Waggon waggon,
                                                                                   int cargoFactor,
                                                                                   int slopePercentage,
                                                                                   Hilliness selectedHilliness,
                                                                                   int tilesBetweenSlopes,
                                                                                   bool useStaticFriction)
        {
            // Create waggon numbers from 1 to 19.
            IEnumerable<int> waggonNumbers = Enumerable.Range(1, 19).ToList();

            // Compute waggon weight based on cargo factor
            int waggonMass = waggon.MassEmpty + (waggon.MassFull - waggon.MassEmpty) * cargoFactor;

            // Compute total tonnages.
            IEnumerable<double> tonnages =
                waggonNumbers.Select(numWaggons => (double)(numWaggons * waggonMass) + (train.Mass * numLocomotives));

            // Compute rolling resistance efforts
            IEnumerable<double> tractiveEfforts =
                tonnages.Select(tonnage => (tonnage * (useStaticFriction ? 27.0 : 17.0)) / 1000.0);

            // Compute slope resistance efforts
            IEnumerable<double> slopeTractiveEfforts = waggonNumbers.Select(
                numWaggons =>
                {
                    double numWaggonsOnSlope = 0;

                    switch (selectedHilliness)
                    {
                        case Hilliness.OneSlope:
                        {
                            numWaggonsOnSlope = Math.Min(numWaggons, 1.0 / waggon.Length);
                        }
                            break;

                        case Hilliness.MultipleSlopes:
                        {
                            double trainTileLength = numWaggons * waggon.Length;
                            double tilesWithWaggons = Math.Ceiling(trainTileLength / (tilesBetweenSlopes + 1));

                            numWaggonsOnSlope = Math.Min(numWaggons, tilesWithWaggons / waggon.Length);
                        }
                            break;
                    }

                    // 1/10 is Short for 9.81 / 100. It just works and is very close to the correct
                    // trigonometry based values for small slope percentages.
                    double teSlopeResistance = (numWaggonsOnSlope * waggonMass * slopePercentage) / 10.0;

                    return teSlopeResistance;
                });

            // Compute total tractive efforts
            IEnumerable<double> totalTractiveEfforts = tractiveEfforts.Zip(
                slopeTractiveEfforts,
                (resistanceTe, slopeTe) => resistanceTe + slopeTe);

            // Compute the speed for each tonnage.
            IEnumerable<double> speeds =
                totalTractiveEfforts.Select(
                    tractiveEffort => (tractiveEffort > (train.MaxTractiveEffort * numLocomotives)) ? 0 : ((train.Power * numLocomotives) / tractiveEffort));

            // Convert to km/h.
            speeds = speeds.Select(speed => (speed * 3.6));

            // Cap at max speed.
            int maxSpeed = Math.Min(train.MaxSpeed, waggon.MaxSpeed);

            speeds = speeds.Select(speed => Math.Min(maxSpeed, speed));

            // Zip them together.
            IEnumerable<KeyValuePair<int, int>> values = waggonNumbers.Zip(
                speeds,
                (numWaggons, speed) => new KeyValuePair<int, int>(numWaggons, (int)speed));

            return values;
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

        /// <summary>
        ///     Called when the train set was changed.
        /// </summary>
        private void OnTrainSetChanged()
        {
            RequestRefresh();
        }

        private void RefreshCharts()
        {
            NotifyOfPropertyChange(() => SpeedToWaggonsValues);
            NotifyOfPropertyChange(() => SpeedToWaggonsValuesAlt);
            NotifyOfPropertyChange(() => WaggonsToSpeedValues);
            NotifyOfPropertyChange(() => WaggonsToSpeedValuesAlt);
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