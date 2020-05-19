using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Core.Domain.Weixin;
using Nop.Core.Html;
using Nop.Data;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;

namespace Nop.Services.Weixin
{
    /// <summary>
    /// WQrCodeImage Service
    /// </summary>
    public partial class WQrCodeImageService : IWQrCodeImageService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<WQrCodeImage> _wQrCodeImageRepository;

        #endregion

        #region Ctor

        public WQrCodeImageService(IEventPublisher eventPublisher,
            IRepository<WQrCodeImage> wQrCodeImageRepository)
        {
            _eventPublisher = eventPublisher;
            _wQrCodeImageRepository = wQrCodeImageRepository;
        }

        #endregion

        #region Methods

        public virtual void InsertWQrCodeImage(WQrCodeImage wQrCodeImage)
        {
            if (wQrCodeImage == null)
                throw new ArgumentNullException(nameof(wQrCodeImage));

            _wQrCodeImageRepository.Insert(wQrCodeImage);

            //event notification
            _eventPublisher.EntityInserted(wQrCodeImage);
        }

        public virtual void DeleteWQrCodeImage(WQrCodeImage wQrCodeImage, bool delete = false)
        {
            if (wQrCodeImage == null)
                throw new ArgumentNullException(nameof(wQrCodeImage));

            wQrCodeImage.Deleted = true;
            UpdateWQrCodeImage(wQrCodeImage);

            //event notification
            _eventPublisher.EntityDeleted(wQrCodeImage);
        }

        public virtual void DeleteWQrCodeImages(IList<WQrCodeImage> wQrCodeImages, bool deleted = false)
        {
            if (wQrCodeImages == null)
                throw new ArgumentNullException(nameof(wQrCodeImages));

            foreach (var wQrCodeImage in wQrCodeImages)
            {
                wQrCodeImage.Deleted = true;
            }

            //delete wUser
            UpdateWQrCodeImages(wQrCodeImages);

            foreach (var wQrCodeImage in wQrCodeImages)
            {
                //event notification
                _eventPublisher.EntityDeleted(wQrCodeImage);
            }
        }

        public virtual void UpdateWQrCodeImage(WQrCodeImage wQrCodeImage)
        {
            if (wQrCodeImage == null)
                throw new ArgumentNullException(nameof(wQrCodeImage));

            _wQrCodeImageRepository.Update(wQrCodeImage);

            //event notification
            _eventPublisher.EntityUpdated(wQrCodeImage);
        }

        public virtual void UpdateWQrCodeImages(IList<WQrCodeImage> wQrCodeImages)
        {
            if (wQrCodeImages == null)
                throw new ArgumentNullException(nameof(wQrCodeImages));

            //update
            _wQrCodeImageRepository.Update(wQrCodeImages);

            //event notification
            foreach (var wQrCodeImage in wQrCodeImages)
            {
                _eventPublisher.EntityUpdated(wQrCodeImage);
            }
        }

        public virtual WQrCodeImage GetWQrCodeImageById(int id)
        {
            if (id == 0)
                return null;

            return _wQrCodeImageRepository.ToCachedGetById(id);
        }

        public virtual List<WQrCodeImage> GetWQrCodeImagesByIds(int[] wQrCodeImageIds)
        {
            if (wQrCodeImageIds is null)
                return new List<WQrCodeImage>();

            var query = from t in _wQrCodeImageRepository.Table
                        where wQrCodeImageIds.Contains(t.Id) &&
                        !t.Deleted
                        select t;

            return query.ToList();
        }

        public virtual IPagedList<WQrCodeImage> GetWQrCodeImages(int pageIndex = 0, int pageSize = int.MaxValue, bool showDeleted = false)
        {
            var query = _wQrCodeImageRepository.Table;

            if (!showDeleted)
                query = query.Where(v => v.Deleted);

            query = query.OrderBy(v => v.CreatTime).ThenBy(v => v.Id);

            return new PagedList<WQrCodeImage>(query, pageIndex, pageSize);
        }


        #endregion
    }
}