// <copyright file="TrainSet.cs" company="VacuumBreather">
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

#region Using Directives

#endregion

namespace TrainTool.Model
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Runtime.Serialization;
    using Helpers;

    #endregion

    /// <summary>
    ///     Represents a train set, containing information about all the trains and waggons available to the player.
    /// </summary>
    [DataContract]
    public class TrainSet
    {
        #region Constants

        /// <summary>
        ///     The default name of a newly created train set.
        /// </summary>
        public const string DefaultName = "New Train Set";

        #endregion

        #region Readonly & Static Fields

        [DataMember(Name = "Trains", Order = 1)]
        private readonly List<Train> _trains = new List<Train>();

        [DataMember(Name = "Waggons", Order = 2)]
        private readonly List<Waggon> _waggons = new List<Waggon>();

        #endregion

        #region Fields

        [DataMember(Name = "Name", Order = 0)]
        private string _name;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TrainSet" /> class.
        /// </summary>
        public TrainSet()
        {
            Contract.Assume(StringValidator.IsValidString(DefaultName));

            Name = DefaultName;
        }

        #endregion

        #region Object Invariant

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(StringValidator.IsValidString(Name), "Name must be a valid string.");
            Contract.Invariant(this._trains != null, "Trains must never be null.");
            Contract.Invariant(
                Contract.ForAll(this._trains, train => train != null),
                "Trains must not contain any null values.");
            Contract.Invariant(this._waggons != null, "Waggons must never be null.");
            Contract.Invariant(
                Contract.ForAll(this._waggons, waggon => waggon != null),
                "Waggons must not contain any null values.");
        }

        #endregion

        #region Instance Properties

        /// <summary>
        ///     Gets the trains.
        /// </summary>
        /// <value>
        ///     The trains.
        /// </value>
        public IEnumerable<Train> Trains
        {
            get
            {
                return this._trains;
            }
        }

        /// <summary>
        ///     Gets the waggons.
        /// </summary>
        /// <value>
        ///     The waggons.
        /// </value>
        public IEnumerable<Waggon> Waggons
        {
            get
            {
                return this._waggons;
            }
        }

        /// <summary>
        ///     Gets or sets the name of the train set.
        /// </summary>
        /// <value>The name of the train set.</value>
        /// <exception cref="ArgumentNullException">When null is passed.</exception>
        /// <exception cref="ArgumentException">When a name with an invalid format is assigned.</exception>
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                Contract.Requires<ArgumentException>(StringValidator.IsValidString(value));
                this._name = value;
            }
        }

        #endregion

        #region Instance Methods

        /// <summary>
        ///     Adds a <see cref="Train" /> to the end of the list of trains.
        /// </summary>
        /// <param name="train">The <see cref="Train" /> to add.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="train" /> is null.</exception>
        public virtual void AddNewTrain(Train train)
        {
            Contract.Requires<ArgumentNullException>(train != null);

            this._trains.Add(train);
        }

        /// <summary>
        ///     Adds a <see cref="Waggon" /> to the end of the list of waggons.
        /// </summary>
        /// <param name="waggon">The <see cref="Waggon" /> to add.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="waggon" /> is null.</exception>
        public virtual void AddNewWaggon(Waggon waggon)
        {
            Contract.Requires<ArgumentNullException>(waggon != null);

            this._waggons.Add(waggon);
        }

        /// <summary>
        ///     Moves a train at the specified index one step down.
        /// </summary>
        /// <param name="index">The index of the train.</param>
        public virtual void MoveTrainDownAt(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0 && index < Trains.Count());

            Contract.Assume(index < this._trains.Count);

            Train train = this._trains[index];

            this._trains.RemoveAt(index);
            this._trains.Insert(index + 1, train);
        }

        /// <summary>
        ///     Moves a train at the specified index one step up.
        /// </summary>
        /// <param name="index">The index of the train.</param>
        public virtual void MoveTrainUpAt(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0 && index < Trains.Count());

            Contract.Assume(index < this._trains.Count);

            Train train = this._trains[index];

            this._trains.RemoveAt(index);
            this._trains.Insert(index - 1, train);
        }

        /// <summary>
        ///     Moves a waggon at the specified index one step down.
        /// </summary>
        /// <param name="index">The index of the waggon.</param>
        public virtual void MoveWaggonDownAt(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0 && index < Waggons.Count());

            Contract.Assume(index < this._waggons.Count);

            Waggon waggon = this._waggons[index];

            this._waggons.RemoveAt(index);
            this._waggons.Insert(index + 1, waggon);
        }

        /// <summary>
        ///     Moves a waggon at the specified index one step up.
        /// </summary>
        /// <param name="index">The index of the waggon.</param>
        public virtual void MoveWaggonUpAt(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0 && index < Waggons.Count());

            Contract.Assume(index < this._waggons.Count);

            Waggon waggon = this._waggons[index];

            this._waggons.RemoveAt(index);
            this._waggons.Insert(index - 1, waggon);
        }

        /// <summary>
        ///     Removes the <see cref="Train" /> at the specified index from the list.
        /// </summary>
        /// <param name="index">The index of the train.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is not a valid index.</exception>
        [SuppressMessage("Microsoft.Contracts", "RequiresAtCall-index >= 0 && index < this.ScenarioReports.Count()")]
        public virtual void RemoveTrainAt(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0 && index < Trains.Count());

            Contract.Assume(index < Trains.Count());

            this._trains.RemoveAt(index);
        }

        /// <summary>
        ///     Removes the <see cref="Waggon" /> at the specified index from the list.
        /// </summary>
        /// <param name="index">The index of the waggon.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is not a valid index.</exception>
        [SuppressMessage("Microsoft.Contracts", "RequiresAtCall-index >= 0 && index < this.ScenarioReports.Count()")]
        public virtual void RemoveWaggonAt(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0 && index < Waggons.Count());

            Contract.Assume(index < Waggons.Count());

            this._waggons.RemoveAt(index);
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            Contract.Assume(Name != null);

            return Name;
        }

        #endregion
    }
}