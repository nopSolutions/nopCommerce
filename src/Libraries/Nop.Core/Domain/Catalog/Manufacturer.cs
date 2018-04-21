using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a manufacturer
    /// </summary>
    public partial class Manufacturer : BaseEntity, ILocalizedEntity, ISlugSupported, IAclSupported, IStoreMappingSupported, IDiscountSupported
    {
        private ICollection<Discount> _appliedDiscounts;
        private ICollection<Discount_AppliedToManufacturers> _discount_AppliedToManufacturers;

        public Manufacturer()
        {
            _appliedDiscounts = new ObservableCollection<Discount>();
            _discount_AppliedToManufacturers = new ObservableCollection<Discount_AppliedToManufacturers>();
            ((ObservableCollection<Discount>)_appliedDiscounts).CollectionChanged += appliedDiscountsChanged;
            ((ObservableCollection<Discount_AppliedToManufacturers>)_discount_AppliedToManufacturers).CollectionChanged += appliedDiscountsManufacturerChanged;
        }

        private void appliedDiscountsManufacturerChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (internalmodify == true) return;
            if (e.NewItems != null)
            {
                foreach (Discount_AppliedToManufacturers newitem in e.NewItems)
                {
                    internalmodify = true;
                    try
                    {
                        _appliedDiscounts.Add(newitem.Discount);
                    }
                    catch (Exception) { }
                    internalmodify = false;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Discount_AppliedToManufacturers olditem in e.OldItems)
                {
                    var item = ((List<Discount>)_appliedDiscounts).Find(p => p.Id == olditem.DiscountId);
                    internalmodify = true;
                    _appliedDiscounts.Remove(item);
                    internalmodify = false;
                }
            }
        }

        private void appliedDiscountsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (internalmodify == true) return;
            if (e.NewItems != null)
            {
                foreach (Discount newitem in e.NewItems)
                {
                    var ca = new Discount_AppliedToManufacturers()
                    {
                        Discount = newitem,
                        DiscountId = newitem.Id,
                        Manufacturer = this,
                        ManufacturerId = this.Id
                    };
                    internalmodify = true;
                    Discount_AppliedToManufacturers.Add(ca);
                    internalmodify = false;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Discount olditem in e.OldItems)
                {
                    var item = ((List<Discount_AppliedToManufacturers>)Discount_AppliedToManufacturers).Find(p =>
                        p.DiscountId == olditem.Id && p.ManufacturerId == this.Id);
                    internalmodify = true;
                    Discount_AppliedToManufacturers.Remove(item);
                    internalmodify = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value of used manufacturer template identifier
        /// </summary>
        public int ManufacturerTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public string MetaTitle { get; set; }

        /// <summary>
        /// Gets or sets the parent picture identifier
        /// </summary>
        public int PictureId { get; set; }

        /// <summary>
        /// Gets or sets the page size
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers can select the page size
        /// </summary>
        public bool AllowCustomersToSelectPageSize { get; set; }

        /// <summary>
        /// Gets or sets the available customer selectable page size options
        /// </summary>
        public string PageSizeOptions { get; set; }

        /// <summary>
        /// Gets or sets the available price ranges
        /// </summary>
        public string PriceRanges { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is subject to ACL
        /// </summary>
        public bool SubjectToAcl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        public bool LimitedToStores { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the collection of applied discounts
        /// </summary>
        /// 
        public virtual ICollection<Discount_AppliedToManufacturers> Discount_AppliedToManufacturers
        {
            get { return _discount_AppliedToManufacturers; }
            protected set { _discount_AppliedToManufacturers = value; }
        }
        private bool internalmodify = false;
        public virtual ICollection<Discount> AppliedDiscounts
        {
            get
            {
                if (_appliedDiscounts.Contains(null) || (_appliedDiscounts.Count != Discount_AppliedToManufacturers.Count))
                {
                    //regenerate
                    internalmodify = true;
                    _appliedDiscounts.Clear();
                    foreach (var item in Discount_AppliedToManufacturers)
                    {
                        _appliedDiscounts.Add(item.Discount);
                    }
                    internalmodify = false;
                }
                return _appliedDiscounts;
            }
            set
            {
                internalmodify = true;
                Discount_AppliedToManufacturers.Clear();
                foreach (var item in value)
                {
                    Discount_AppliedToManufacturers dac = new Discount_AppliedToManufacturers()
                    {
                        Manufacturer = this,
                        ManufacturerId = Id,
                        Discount = item,
                        DiscountId = item.Id
                    };
                    Discount_AppliedToManufacturers.Add(dac);
                }
                internalmodify = false;
            }
        }
    }
}
