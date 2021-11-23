using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Services.Html;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// Vendor service
    /// </summary>
    public partial class VendorService : IVendorService
    {
        #region Fields

        private readonly INopHtmlHelper _htmlHelper;
        protected IRepository<Customer> CustomerRepository { get; }
        protected IRepository<Product> ProductRepository { get; }
        protected IRepository<Vendor> VendorRepository { get; }
        protected IRepository<VendorNote> VendorNoteRepository { get; }

        #endregion

        #region Ctor

        public VendorService(INopHtmlHelper htmlHelper,
            IRepository<Customer> customerRepository,
            IRepository<Product> productRepository,
            IRepository<Vendor> vendorRepository,
            IRepository<VendorNote> vendorNoteRepository)
        {
            _htmlHelper = htmlHelper;
            CustomerRepository = customerRepository;
            ProductRepository = productRepository;
            VendorRepository = vendorRepository;
            VendorNoteRepository = vendorNoteRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a vendor by vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor
        /// </returns>
        public virtual async Task<Vendor> GetVendorByIdAsync(int vendorId)
        {
            return await VendorRepository.GetByIdAsync(vendorId, cache => default);
        }

        /// <summary>
        /// Gets a vendor by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor
        /// </returns>
        public virtual async Task<Vendor> GetVendorByProductIdAsync(int productId)
        {
            if (productId == 0)
                return null;

            return await (from v in VendorRepository.Table
                    join p in ProductRepository.Table on v.Id equals p.VendorId
                    where p.Id == productId
                    select v).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets vendors by product identifiers
        /// </summary>
        /// <param name="productIds">Array of product identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendors
        /// </returns>
        public virtual async Task<IList<Vendor>> GetVendorsByProductIdsAsync(int[] productIds)
        {
            if (productIds is null)
                throw new ArgumentNullException(nameof(productIds));

            return await (from v in VendorRepository.Table
                    join p in ProductRepository.Table on v.Id equals p.VendorId
                    where productIds.Contains(p.Id) && !v.Deleted && v.Active
                    select v).Distinct().ToListAsync();
        }

        /// <summary>
        /// Gets a vendors by customers identifiers
        /// </summary>
        /// <param name="customerIds">Array of customer identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendors
        /// </returns>
        public virtual async Task<IList<Vendor>> GetVendorsByCustomerIdsAsync(int[] customerIds)
        {
            if (customerIds is null)
                throw new ArgumentNullException(nameof(customerIds));

            return await (from v in VendorRepository.Table
                join c in CustomerRepository.Table on v.Id equals c.VendorId
                where customerIds.Contains(c.Id) && !v.Deleted && v.Active
                select v).Distinct().ToListAsync();
        }

        /// <summary>
        /// Delete a vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteVendorAsync(Vendor vendor)
        {
            await VendorRepository.DeleteAsync(vendor);
        }

        /// <summary>
        /// Gets all vendors
        /// </summary>
        /// <param name="name">Vendor name</param>
        /// <param name="email">Vendor email</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendors
        /// </returns>
        public virtual async Task<IPagedList<Vendor>> GetAllVendorsAsync(string name = "", string email = "", int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var vendors = await VendorRepository.GetAllPagedAsync(query =>
            {
                if (!string.IsNullOrWhiteSpace(name))
                    query = query.Where(v => v.Name.Contains(name));

                if (!string.IsNullOrWhiteSpace(email))
                    query = query.Where(v => v.Email.Contains(email));

                if (!showHidden)
                    query = query.Where(v => v.Active);

                query = query.Where(v => !v.Deleted);
                query = query.OrderBy(v => v.DisplayOrder).ThenBy(v => v.Name).ThenBy(v => v.Email);

                return query;
            }, pageIndex, pageSize);

            return vendors;
        }

        /// <summary>
        /// Inserts a vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertVendorAsync(Vendor vendor)
        {
            await VendorRepository.InsertAsync(vendor);
        }

        /// <summary>
        /// Updates the vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateVendorAsync(Vendor vendor)
        {
            await VendorRepository.UpdateAsync(vendor);
        }

        /// <summary>
        /// Gets a vendor note
        /// </summary>
        /// <param name="vendorNoteId">The vendor note identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor note
        /// </returns>
        public virtual async Task<VendorNote> GetVendorNoteByIdAsync(int vendorNoteId)
        {
            return await VendorNoteRepository.GetByIdAsync(vendorNoteId, cache => default);
        }

        /// <summary>
        /// Gets all vendor notes
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor notes
        /// </returns>
        public virtual async Task<IPagedList<VendorNote>> GetVendorNotesByVendorAsync(int vendorId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = VendorNoteRepository.Table.Where(vn => vn.VendorId == vendorId);

            query = query.OrderBy(v => v.CreatedOnUtc).ThenBy(v => v.Id);

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// Deletes a vendor note
        /// </summary>
        /// <param name="vendorNote">The vendor note</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteVendorNoteAsync(VendorNote vendorNote)
        {
            await VendorNoteRepository.DeleteAsync(vendorNote);
        }

        /// <summary>
        /// Inserts a vendor note
        /// </summary>
        /// <param name="vendorNote">Vendor note</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertVendorNoteAsync(VendorNote vendorNote)
        {
            await VendorNoteRepository.InsertAsync(vendorNote);
        }

        /// <summary>
        /// Formats the vendor note text
        /// </summary>
        /// <param name="vendorNote">Vendor note</param>
        /// <returns>Formatted text</returns>
        public virtual string FormatVendorNoteText(VendorNote vendorNote)
        {
            if (vendorNote == null)
                throw new ArgumentNullException(nameof(vendorNote));

            var text = vendorNote.Note;

            if (string.IsNullOrEmpty(text))
                return string.Empty;

            text = _htmlHelper.FormatText(text, false, true, false, false, false, false);

            return text;
        }

        #endregion
    }
}