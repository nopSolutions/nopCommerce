using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Weixin;

namespace Nop.Services.Weixin
{
    /// <summary>
    /// WMessageBind Service interface
    /// </summary>
    public partial interface IWMessageBindService
    {
        void InsertEntity(WMessageBindMapping messageBind);

        void DeleteEntity(WMessageBindMapping messageBind);

        void DeleteEntities(IList<WMessageBindMapping> messageBinds);

        void UpdateEntity(WMessageBindMapping messageBind);

        void UpdateEntities(IList<WMessageBindMapping> messageBinds);

        WMessageBindMapping GetEntityById(int id);

        List<WMessageBindMapping> GetEntitiesByIds(int[] messageBindIds);

        List<int> GetMessageBindIds(int bindSceneId, WMessageBindSceneType messageBindSceneType);

        List<WMessageBindMapping> GetEntities(int messageId = 0, int bindSceneId = 0, WMessageBindSceneType? messageBindSceneType = null, bool? published = null);

        IPagedList<WMessageBindMapping> GetEntities(int messageId = 0, int bindSceneId = 0, WMessageBindSceneType? messageBindSceneType = null, bool? published = null, int pageIndex = 0, int pageSize = int.MaxValue);

    }
}