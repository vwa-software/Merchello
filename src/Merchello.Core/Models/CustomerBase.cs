﻿namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Serialization;
	using ETC.B2B.PriceGroup.Services;
	using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a customer base class
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class CustomerBase : Entity, ICustomerBase
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        /// <summary>
        /// The last activity date.
        /// </summary>
        private DateTime _lastActivityDate;

        /// <summary>
        /// The _extended data.
        /// </summary>
        private ExtendedDataCollection _extendedData;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerBase"/> class.
        /// </summary>
        /// <param name="isAnonymous">
        /// The is anonymous.
        /// </param>
        protected CustomerBase(bool isAnonymous)
            : this(isAnonymous, new ExtendedDataCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerBase"/> class.
        /// </summary>
        /// <param name="isAnonymous">
        /// The is anonymous.
        /// </param>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        protected CustomerBase(bool isAnonymous, ExtendedDataCollection extendedData)
        {
            IsAnonymous = isAnonymous;
            _extendedData = extendedData;
        }
        
        /// <summary>
        /// Gets or sets date the customer was last active on the site
        /// </summary>
        [DataMember]
        public DateTime LastActivityDate
        {
            get
            {
                return _lastActivityDate;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _lastActivityDate, _ps.Value.LastActivityDateSelector);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this customer is an anonymous customer 
        /// </summary>
        [IgnoreDataMember]
        public bool IsAnonymous { get; private set; }

        /// <summary>
        /// Gets a collection to store custom/extended data for the customer
        /// </summary>
        [DataMember]
        public ExtendedDataCollection ExtendedData
        {
            get
            {
                return _extendedData;
            }

            internal set
            {
                _extendedData = value;
                _extendedData.CollectionChanged += ExtendedDataChanged;
            }
        }


        /// <summary>
        /// Asserts that the last activity date is set to the current date time
        /// </summary>
        internal override void AddingEntity()
        {
            base.AddingEntity();
            _lastActivityDate = DateTime.Now;
        }

        /// <summary>
        /// Asserts that the last activity date is set to the current date time
        /// </summary>
        internal override void UpdatingEntity()
        {
            base.UpdatingEntity();
            _lastActivityDate = DateTime.Now;
        }

        /// <summary>
        /// The extended data changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExtendedDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(_ps.Value.ExtendedDataChangedSelector);
        }


		private ETC.B2B.PriceGroup.Model.PriceGroup _priceGroup = null;

		/// <summary>
		/// eTC Price group
		/// </summary>
		[IgnoreDataMember]
		public ETC.B2B.PriceGroup.Model.PriceGroup PriceGroup
		{
			get
			{
				if (this._priceGroup == null || (this._priceGroup.IsEmpty && !this.IsAnonymous) || this.IsDirty())
					this._priceGroup = PriceGroupService.GetPriceGroupForMember();
				return this._priceGroup;
			}
		}
				
		/// <summary>
		/// Property selectors.
		/// </summary>
		private class PropertySelectors
        {
            /// <summary>
            /// The last activity date selector.
            /// </summary>
            public readonly PropertyInfo LastActivityDateSelector = ExpressionHelper.GetPropertyInfo<CustomerBase, DateTime>(x => x.LastActivityDate);

            /// <summary>
            /// The extended data changed selector.
            /// </summary>
            public readonly PropertyInfo ExtendedDataChangedSelector = ExpressionHelper.GetPropertyInfo<CustomerBase, ExtendedDataCollection>(x => x.ExtendedData);
        }
    }
}