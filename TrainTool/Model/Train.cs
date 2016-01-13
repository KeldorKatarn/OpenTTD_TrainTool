// <copyright file="Train.cs" company="VacuumBreather">
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

namespace TrainTool.Model
{
    #region Using Directives

    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;
    using Annotations;
    using Helpers;

    #endregion

    /// <summary>
    ///     Represents a train in the game.
    /// </summary>
    [DataContract]
    public class Train : INotifyPropertyChanged
    {
        #region Constants

        public const string DefaultName = "New Train";

        #endregion

        #region Fields

        [DataMember(Name = "Name", Order = 0, IsRequired = true)]
        private string _name;

        [DataMember(Name = "Year", Order = 1, IsRequired = true)]
        private int _year;

        [DataMember(Name = "Mass", Order = 2, IsRequired = true)]
        private int _mass;

        [DataMember(Name = "Power", Order = 3, IsRequired = true)]
        private int _power;

        [DataMember(Name = "MaxSpeed", Order = 4, IsRequired = true)]
        private int _maxSpeed;

        [DataMember(Name = "MaxTractiveEffort", Order = 5, IsRequired = true)]
        private int _maxTractiveEffort;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Train" /> class.
        /// </summary>
        public Train()
        {
            Contract.Assume(StringValidator.IsValidString(DefaultName));

            this._name = DefaultName;
            this._year = 1900;
            this._mass = 1;
            this._power = 1;
            this._maxSpeed = 1;
            this._maxTractiveEffort = 1;
        }

        #endregion

        #region Object Invariant

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(StringValidator.IsValidString(this._name), "Name must be a valid name string.");
            Contract.Invariant(this._mass > 0, "MassFull must be a positive integer.");
            Contract.Invariant(this._power > 0, "Power must be a positive integer.");
            Contract.Invariant(this._maxSpeed > 0, "MaxSpeed must be a positive integer.");
            Contract.Invariant(this._maxTractiveEffort > 0, "MaxTractiveEffort must be a positive integer.");
        }

        #endregion

        #region Instance Properties

        /// <summary>
        ///     Gets or sets the name of the train.
        /// </summary>
        /// <value>The name of the train.</value>
        /// <exception cref="ArgumentNullException">When the train name is set to null.</exception>
        /// <exception cref="ArgumentException">When the train name is set to an invalid string.</exception>
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

                OnPropertyChanged("Name");
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
                this._year = value;

                OnPropertyChanged("Year");
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
                Contract.Requires<ArgumentException>(value > 0);
                this._mass = value;

                OnPropertyChanged("Mass");
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
                Contract.Requires<ArgumentException>(value > 0);
                this._power = value;

                OnPropertyChanged("Power");
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
                Contract.Requires<ArgumentException>(value > 0);
                this._maxSpeed = value;

                OnPropertyChanged("MaxSpeed");
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
                Contract.Requires<ArgumentException>(value > 0);
                this._maxTractiveEffort = value;

                OnPropertyChanged("MaxTractiveEffort");
            }
        }

        #endregion

        #region Instance Methods

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        ///     Called when a property value changes.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}