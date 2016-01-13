// <copyright file="Waggon.cs" company="VacuumBreather">
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
    ///     Represents a waggon in the game.
    /// </summary>
    [DataContract]
    public class Waggon : INotifyPropertyChanged
    {
        #region Constants

        public const string DefaultName = "New Waggon";

        #endregion

        #region Fields

        [DataMember(Name = "Name", Order = 0, IsRequired = true)]
        private string _name;

        [DataMember(Name = "MassEmpty", Order = 1, IsRequired = true)]
        private int _massEmpty;

        [DataMember(Name = "FullMass", Order = 2, IsRequired = true)]
        private int _massFull;

        [DataMember(Name = "MaxSpeed", Order = 3, IsRequired = true)]
        private int _maxSpeed;

        [DataMember(Name = "Length", Order = 4)]
        private double _length;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Waggon" /> class.
        /// </summary>
        public Waggon()
        {
            Contract.Assume(StringValidator.IsValidString(DefaultName));

            this._name = DefaultName;
            this._massEmpty = 1;
            this._massFull = 1;
            this._maxSpeed = 1;
            this._length = 0.5;
        }

        #endregion

        #region Object Invariant

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(StringValidator.IsValidString(this._name), "Name must be a valid name string.");
            Contract.Invariant(this._massEmpty > 0, "MassEmpty must be a positive integer.");
            Contract.Invariant(this._massEmpty > 0, "MassFull must be a positive integer.");
            Contract.Invariant(this._maxSpeed > 0, "MaxSpeed must be a positive integer.");
            Contract.Invariant(this._length > 0.0, "Length must be a positive number.");
        }

        #endregion

        #region Instance Properties

        /// <summary>
        ///     Gets or sets the name of the waggon.
        /// </summary>
        /// <value>The name of the waggon.</value>
        /// <exception cref="ArgumentNullException">When the waggon name is set to null.</exception>
        /// <exception cref="ArgumentException">When the waggon name is set to an invalid string.</exception>
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
                Contract.Requires<ArgumentException>(value > 0);
                this._massEmpty = value;

                OnPropertyChanged("MassEmpty");
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
                Contract.Requires<ArgumentException>(value > 0);
                this._massFull = value;

                OnPropertyChanged("MassFull");
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
                Contract.Requires<ArgumentException>(value > 0);
                this._maxSpeed = value;

                OnPropertyChanged("MaxSpeed");
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
                Contract.Requires<ArgumentException>(value > 0.0);
                this._length = value;

                OnPropertyChanged("Length");
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

        [OnDeserializing]
        private void OnDeserializing(StreamingContext c)
        {
            this._name = DefaultName;
            this._massEmpty = 1;
            this._massFull = 1;
            this._maxSpeed = 1;
            this._length = 0.5;
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