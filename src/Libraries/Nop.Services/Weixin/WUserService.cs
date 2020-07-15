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
    public partial class WUserService : IWUserService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<WUser> _wUserRepository;

        #endregion

        #region Ctor

        public WUserService(IEventPublisher eventPublisher,
            IRepository<WUser> wUserRepository)
        {
            _eventPublisher = eventPublisher;
            _wUserRepository = wUserRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 添加新用户
        /// </summary>
        /// <param name="wuser"></param>
        public virtual void InsertWUser(WUser wuser)
        {
            if (wuser == null)
                throw new ArgumentNullException(nameof(wuser));

            _wUserRepository.Insert(wuser);

            //event notification
            _eventPublisher.EntityInserted(wuser);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="wUser"></param>
        /// <param name="delete">是否真删除，否则只更改删除标志</param>
        public virtual void DeleteWUser(WUser wUser, bool delete = false)
        {
            if (wUser == null)
                throw new ArgumentNullException(nameof(wUser));

            wUser.Deleted = true;
            UpdateWUser(wUser);

            //event notification
            _eventPublisher.EntityDeleted(wUser);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wUsers"></param>
        /// <param name="deleted">是否真删除，否则只更改删除标志</param>
        public virtual void DeleteWUsers(IList<WUser> wUsers, bool deleted = false)
        {
            if (wUsers == null)
                throw new ArgumentNullException(nameof(wUsers));

            foreach (var wuser in wUsers)
            {
                wuser.Deleted = true;
            }

            //delete wUser
            UpdateWUsers(wUsers);

            foreach (var wuser in wUsers)
            {
                //event notification
                _eventPublisher.EntityDeleted(wuser);
            }
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="wuser"></param>
        public virtual void UpdateWUser(WUser wuser)
        {
            if (wuser == null)
                throw new ArgumentNullException(nameof(wuser));

            _wUserRepository.Update(wuser);

            //event notification
            _eventPublisher.EntityUpdated(wuser);
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="wuser"></param>
        public virtual void UpdateWUsers(IList<WUser> wUsers)
        {
            if (wUsers == null)
                throw new ArgumentNullException(nameof(wUsers));

            //update
            _wUserRepository.Update(wUsers);

            //event notification
            foreach (var wuser in wUsers)
            {
                _eventPublisher.EntityUpdated(wuser);
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="wUserId"></param>
        /// <returns></returns>
        public virtual WUser GetWUserById(int id)
        {
            if (id == 0)
                return null;

            return _wUserRepository.ToCachedGetById(id);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public virtual WUser GetWUserByOpenId(string openId)
        {
            if (string.IsNullOrEmpty(openId))
                return null;

            openId = openId.Trim();

            var query = from t in _wUserRepository.Table
                        where t.OpenId == openId
                        orderby t.Id
                        select t;

            return query.FirstOrDefault();
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="openIdHash"></param>
        /// <returns></returns>
        public virtual WUser GetWUserByOpenIdHash(long openIdHash)
        {
            if (openIdHash == 0)
                return null;

            var query = from user in _wUserRepository.Table
                        orderby user.Id
                        where !user.Deleted &&
                        user.OpenIdHash == openIdHash
                        select user;

            return query.FirstOrDefault();
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="wUserIds"></param>
        /// <returns></returns>
        public virtual List<WUser> GetWUsersByIds(int[] wUserIds)
        {
            if (wUserIds is null)
                return new List<WUser>();

            var query = from user in _wUserRepository.Table
                        where wUserIds.Contains(user.Id) &&
                        !user.Deleted
                        select user;

            return query.ToList();
        }

        /// <summary>
        /// 获取用户基本信息
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public virtual WUserBaseInfo GetWUserBaseInfo(string openId, bool containAllOpenid = false)
        {
            WUserBaseInfo wuserBaseInfo = null;

            if (string.IsNullOrEmpty(openId))
                return wuserBaseInfo;

            openId = openId.Trim();

            var query = from t in _wUserRepository.Table
                        where t.OpenId == openId &&
                        !t.Deleted
                        orderby t.Id
                        select t;

            var wuser = query.FirstOrDefault();

            if (wuser != null)
            {
                wuserBaseInfo = new WUserBaseInfo
                {
                    OpenId = wuser.OpenId,
                    UnionId = wuser.UnionId,
                    NickName = wuser.NickName,
                    HeadImgUrl = wuser.HeadImgUrl,
                    Subscribe = wuser.Subscribe,
                    SubscribeTime = wuser.SubscribeTime,
                    UnSubscribeTime = wuser.UnSubscribeTime
                };
            }
            else if (containAllOpenid)
            {
                wuserBaseInfo = new WUserBaseInfo
                {
                    OpenId = openId,
                    NickName = string.Empty,
                    HeadImgUrl = string.Empty,
                    Subscribe = false,
                    SubscribeTime = 0,
                    UnSubscribeTime = 0
                };
            }

            return wuserBaseInfo;
        }

        /// <summary>
        /// 获取用户基本信息
        /// </summary>
        /// <param name="openIds"></param>
        /// <returns></returns>
        public virtual List<WUserBaseInfo> GetWUserBaseInfoByOpenIds(string[] openIds, bool containAllOpenid = false)
        {
            List<WUserBaseInfo> wuserBaseInfos = new List<WUserBaseInfo>();

            if (openIds is null)
                return wuserBaseInfos;

            var query = from t in _wUserRepository.Table
                        where openIds.Contains(t.OpenId) &&
                        !t.Deleted
                        select t;

            var wusers = query.ToList();

            foreach (var openId in openIds)
            {
                if (string.IsNullOrEmpty(openId))
                    continue;

                var wuser = wusers.FirstOrDefault(v => v.OpenId == openId);
                if (wuser != null)
                {
                    wuserBaseInfos.Add(new WUserBaseInfo
                    {
                        OpenId = openId,
                        UnionId = wuser.UnionId,
                        NickName = wuser.NickName,
                        HeadImgUrl = wuser.HeadImgUrl,
                        Subscribe = wuser.Subscribe,
                        SubscribeTime = wuser.SubscribeTime,
                        UnSubscribeTime = wuser.UnSubscribeTime
                    });
                }
                else if (containAllOpenid)
                {
                    wuserBaseInfos.Add(new WUserBaseInfo
                    {
                        OpenId = openId,
                        NickName = string.Empty,
                        HeadImgUrl = string.Empty,
                        Subscribe = false,
                        SubscribeTime = 0,
                        UnSubscribeTime = 0
                    });
                }
            }
            return wuserBaseInfos;
        }

        /// <summary>
        /// 获取全部用户
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="showDeleted"></param>
        /// <returns></returns>
        public virtual IPagedList<WUser> GetAllUsers(
            string nickName = null,
            string remark = null,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool showDeleted = false)
        {
            var query = _wUserRepository.Table;

            if (!string.IsNullOrEmpty(nickName))
                query = query.Where(v => v.NickName.Contains(nickName));
            if (!string.IsNullOrEmpty(remark))
                query = query.Where(v => v.Remark.Contains(remark));

            if (!showDeleted)
                query = query.Where(v => v.Deleted);

            query = query.OrderBy(v => v.CreatTime).ThenBy(v => v.Id);

            return new PagedList<WUser>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// 获取自己推荐的用户列表信息
        /// </summary>
        /// <param name="refereeId"></param>
        /// <param name="wConfigId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="showDeleted"></param>
        /// <returns></returns>
        public virtual IPagedList<WUser> GetRefereeUsers(int refereeId, int wConfigId = 0, int pageIndex = 0, int pageSize = int.MaxValue, bool showDeleted = false)
        {
            var query = _wUserRepository.Table;
            query = query.Where(user => user.RefereeId == refereeId);
            query = query.OrderBy(v => v.CreatTime).ThenBy(v => v.Id);

            return new PagedList<WUser>(query, pageIndex, pageSize);
        }

        #endregion
    }
}