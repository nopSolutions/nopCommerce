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
    /// WUserService service
    /// </summary>
    public partial class WLocationService : IWLocationService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<WLocation> _wLocationRepository;

        #endregion

        #region Ctor

        public WLocationService(IEventPublisher eventPublisher,
            IRepository<WLocation> wLocationRepository)
        {
            _eventPublisher = eventPublisher;
            _wLocationRepository = wLocationRepository;
        }

        #endregion

        #region Methods

        public virtual void InsertLocation(WLocation wlocation)
        {
            if (wlocation == null)
                throw new ArgumentNullException(nameof(wlocation));

            _wLocationRepository.Insert(wlocation);

            //event notification
            _eventPublisher.EntityInserted(wlocation);
        }

        public virtual void DeleteLocation(WLocation location)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            _wLocationRepository.Delete(location);

            //event notification
            _eventPublisher.EntityDeleted(location);
        }

        public virtual void DeleteLocations(IList<WLocation> locations)
        {
            if (locations == null)
                throw new ArgumentNullException(nameof(locations));

            //delete wUser
            _wLocationRepository.Delete(locations);

            foreach (var location in locations)
            {
                //event notification
                _eventPublisher.EntityDeleted(location);
            }
        }

        public virtual void UpdateLocation(WLocation location)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            _wLocationRepository.Update(location);

            //event notification
            _eventPublisher.EntityUpdated(location);
        }

        public virtual WLocation GetLocationById(int id)
        {
            if (id == 0)
                return null;

            return _wLocationRepository.GetById(id);
        }

        public virtual WLocation GetLocationByUserId(int userId)
        {
            if (userId == 0)
                return null;

            var query = from location in _wLocationRepository.Table
                        where location.UserId == userId
                        orderby location.Id descending
                        select location;

            return query.FirstOrDefault();
        }

        public virtual IPagedList<WLocation> GetLocations(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _wLocationRepository.Table;

            query = query.OrderBy(l => l.Id);

            return new PagedList<WLocation>(query, pageIndex, pageSize);
        }

        #endregion
    }
}