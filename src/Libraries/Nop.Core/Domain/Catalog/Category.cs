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
    /// Represents a category
    /// </summary>
    public partial class Category : BaseEntity, ILocalizedEntity, ISlugSupported, IAclSupported, IStoreMappingSupported, IDiscountSupported
    {
        private ICollection<Discount> _appliedDiscounts;
        private ICollection<Discount_AppliedToCategories> _discount_AppliedToCategories;

        public Category()
        {
            _appliedDiscounts = new ObservableCollection<Discount>();
            _discount_AppliedToCategories = new ObservableCollection<Discount_AppliedToCategories>();
            ((ObservableCollection<Discount>)_appliedDiscounts).CollectionChanged += appliedDiscountsChanged;
            ((ObservableCollection<Discount_AppliedToCategories>)_discount_AppliedToCategories).CollectionChanged += appliedDiscountsCategoryChanged;
        }

        private void appliedDiscountsCategoryChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (internalmodify == true) return;
            if (e.NewItems != null)
            {
                foreach (Discount_AppliedToCategories newitem in e.NewItems)
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
                foreach (Discount_AppliedToCategories olditem in e.OldItems)
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
                    var ca = new Discount_AppliedToCategories()
                    {
                        Discount = newitem,
                        DiscountId = newitem.Id,
                        Category = this,
                        CategoryId = this.Id
                    };
                    internalmodify = true;
                    Discount_AppliedToCategories.Add(ca);
                    internalmodify = false;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Discount olditem in e.OldItems)
                {
                    var item = ((List<Discount_AppliedToCategories>)Discount_AppliedToCategories).Find(p =>
                        p.DiscountId == olditem.Id && p.CategoryId == this.Id);
                    internalmodify = true;
                    Discount_AppliedToCategories.Remove(item);
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
        /// Gets or sets a value of used category template identifier
        /// </summary>
        public int CategoryTemplateId { get; set; }

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
        /// Gets or sets the parent category identifier
        /// </summary>
        public int ParentCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the picture identifier
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
        /// Gets or sets a value indicating whether to show the category on home page
        /// </summary>
        public bool ShowOnHomePage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include this category in the top menu
        /// </summary>
        public bool IncludeInTopMenu { get; set; }
        
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

        public virtual ICollection<Discount_AppliedToCategories> Discount_AppliedToCategories
        {
            get { return _discount_AppliedToCategories; }
            protected set { _discount_AppliedToCategories = value; }
        }
        private bool internalmodify = false;
        public virtual ICollection<Discount> AppliedDiscounts
        {
            get
            {
                if (_appliedDiscounts.Contains(null) || (_appliedDiscounts.Count != Discount_AppliedToCategories.Count))
                {
                    //regenerate
                    internalmodify = true;
                    _appliedDiscounts.Clear();
                    foreach (var item in Discount_AppliedToCategories)
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
                Discount_AppliedToCategories.Clear();
                foreach (var item in value)
                {
                    Discount_AppliedToCategories dac = new Discount_AppliedToCategories()
                    {
                        Category = this,
                        CategoryId = Id,
                        Discount = item,
                        DiscountId = item.Id
                    };
                    Discount_AppliedToCategories.Add(dac);
                }
                internalmodify = false;
            }
        }
    }
}
