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
    /// WUserService
    /// </summary>
    public partial class WQrCodeLimitService : IWQrCodeLimitService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<WQrCodeLimit> _wQrCodeLimitRepository;

        #endregion

        #region Ctor

        public WQrCodeLimitService(IEventPublisher eventPublisher,
            IRepository<WQrCodeLimit> wQrCodeLimitRepository)
        {
            _eventPublisher = eventPublisher;
            _wQrCodeLimitRepository = wQrCodeLimitRepository;
        }

        #endregion

        #region Methods

        public virtual void InsertWQrCodeLimit(WQrCodeLimit wQrCodeLimit)
        {
            if (wQrCodeLimit == null)
                throw new ArgumentNullException(nameof(wQrCodeLimit));

            _wQrCodeLimitRepository.Insert(wQrCodeLimit);

            //event notification
            _eventPublisher.EntityInserted(wQrCodeLimit);
        }

        public virtual void UpdateWQrCodeLimit(WQrCodeLimit wQrCodeLimit)
        {
            if (wQrCodeLimit == null)
                throw new ArgumentNullException(nameof(wQrCodeLimit));

            _wQrCodeLimitRepository.Update(wQrCodeLimit);

            //event notification
            _eventPublisher.EntityUpdated(wQrCodeLimit);
        }

        public virtual void UpdateWQrCodeLimits(IList<WQrCodeLimit> wQrCodeLimits)
        {
            if (wQrCodeLimits == null)
                throw new ArgumentNullException(nameof(wQrCodeLimits));

            //update
            _wQrCodeLimitRepository.Update(wQrCodeLimits);

            //event notification
            foreach (var wQrCodeLimit in wQrCodeLimits)
            {
                _eventPublisher.EntityUpdated(wQrCodeLimit);
            }
        }

        public virtual WQrCodeLimit GetWQrCodeLimitById(int id)
        {
            if (id == 0)
                return null;

            return _wQrCodeLimitRepository.GetById(id);
        }

        public virtual WQrCodeLimit GetWQrCodeLimitByQrCodeId(int qrCodeId)
        {
            if (qrCodeId == 0)
                return null;

            var query = from t in _wQrCodeLimitRepository.Table
                        orderby t.Id
                        where t.QrCodeId == qrCodeId
                        select t;

            return query.FirstOrDefault();
        }

        public virtual List<WQrCodeLimit> GetWUsersByIds(int[] wQrCodeLimitIds)
        {
            if (wQrCodeLimitIds is null)
                return new List<WQrCodeLimit>();

            var query = from t in _wQrCodeLimitRepository.Table
                        where wQrCodeLimitIds.Contains(t.Id)
                        select t;

            return query.ToList();
        }

        public virtual IPagedList<WQrCodeLimit> GetWQrCodeLimits(int? wConfigId = null, 
            int? wQrCodeCategoryId = null, 
            int? wQrCodeChannelId = null, 
            bool? fixedUse = null, 
            bool? hasCreated = null, 
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _wQrCodeLimitRepository.Table;

            if (wConfigId.HasValue)
                query = query.Where(v => v.WConfigId == wConfigId);

            if (wQrCodeCategoryId.HasValue)
                query = query.Where(v => v.WQrCodeCategoryId == wQrCodeCategoryId);

            if (wQrCodeChannelId.HasValue)
                query = query.Where(v => v.WQrCodeChannelId == wQrCodeChannelId);

            if (hasCreated.HasValue)
            {
                if (hasCreated.Value)
                    query = query.Where(v => !string.IsNullOrEmpty(v.Ticket));
                else
                    query = query.Where(v => string.IsNullOrEmpty(v.Ticket));
            }
                
            if (fixedUse.HasValue)
                query = query.Where(v => v.FixedUse == fixedUse);

            query = query.OrderBy(v => v.Id);

            return new PagedList<WQrCodeLimit>(query, pageIndex, pageSize);
        }


        #endregion
    }
}