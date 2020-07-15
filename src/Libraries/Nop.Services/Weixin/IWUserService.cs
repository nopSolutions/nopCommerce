using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Weixin;

namespace Nop.Services.Weixin
{
    /// <summary>
    /// WUser service interface
    /// </summary>
    public partial interface IWUserService
    {
        /// <summary>
        /// 添加新用户
        /// </summary>
        /// <param name="wuser"></param>
        void InsertWUser(WUser wuser);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="wUser"></param>
        /// <param name="delete">是否真删除，否则只更改删除标志</param>
        void DeleteWUser(WUser wUser, bool delete = false);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wUsers"></param>
        /// <param name="deleted">是否真删除，否则只更改删除标志</param>
        void DeleteWUsers(IList<WUser> wUsers, bool deleted = false);

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="wuser"></param>
        void UpdateWUser(WUser wuser);

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="wuser"></param>
        void UpdateWUsers(IList<WUser> wUsers);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="wUserId"></param>
        /// <returns></returns>
        WUser GetWUserById(int id);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        WUser GetWUserByOpenId(string openId);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="openIdHash"></param>
        /// <returns></returns>
        WUser GetWUserByOpenIdHash(long openIdHash);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="wUserIds"></param>
        /// <returns></returns>
        List<WUser> GetWUsersByIds(int[] wUserIds);

        /// <summary>
        /// 获取用户基本信息
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        WUserBaseInfo GetWUserBaseInfo(string openId, bool containAllOpenid = false);

        /// <summary>
        /// 获取用户基本信息
        /// </summary>
        /// <param name="openIds"></param>
        /// <returns></returns>
        List<WUserBaseInfo> GetWUserBaseInfoByOpenIds(string[] openIds, bool containAllOpenid = false);

        /// <summary>
        /// 获取全部用户
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="showDeleted"></param>
        /// <returns></returns>
        IPagedList<WUser> GetAllUsers(string nickName = null, string remark = null, int pageIndex = 0, int pageSize = int.MaxValue, bool showDeleted = false);

        /// <summary>
        /// 获取自己推荐的用户列表信息
        /// </summary>
        /// <param name="refereeId"></param>
        /// <param name="wConfigId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="showDeleted"></param>
        /// <returns></returns>
        IPagedList<WUser> GetRefereeUsers(int refereeId, int wConfigId = 0, int pageIndex = 0, int pageSize = int.MaxValue, bool showDeleted = false);

    }
}