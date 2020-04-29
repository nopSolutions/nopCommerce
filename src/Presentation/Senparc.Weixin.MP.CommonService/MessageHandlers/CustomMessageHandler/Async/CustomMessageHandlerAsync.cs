/*----------------------------------------------------------------
    Copyright (C) 2020 Senparc
    
    文件名：CustomMessageHandlerAsync.cs
    文件功能描述：自定义MessageHandler（异步方法）
    
    
    创建标识：Senparc - 20191003
----------------------------------------------------------------*/

//DPBMARK_FILE MP
using System.Threading;
using System.Threading.Tasks;

#if NET45
using System.Web;
#else
#endif

namespace Senparc.Weixin.MP.CommonService.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// </summary>
    public partial class CustomMessageHandler
    {
        public override async Task OnExecutingAsync(CancellationToken cancellationToken)
        {
            //演示：MessageContext.StorageData

            var currentMessageContext = await base.GetUnsafeMessageContext();//为了在分布式缓存下提高读写效率，使用此方法，如果需要获取实时数据，应该使用 base.GetCurrentMessageContext()
            if (currentMessageContext.StorageData == null || !(currentMessageContext.StorageData is int))
            {
                currentMessageContext.StorageData = (int)0;
                //await GlobalMessageContext.UpdateMessageContextAsync(currentMessageContext);//储存到缓存
            }
            await base.OnExecutingAsync(cancellationToken);
        }

        public override async Task OnExecutedAsync(CancellationToken cancellationToken)
        {
            //演示：MessageContext.StorageData

            var currentMessageContext = await base.GetUnsafeMessageContext();//为了在分布式缓存下提高读写效率，使用此方法，如果需要获取实时数据，应该使用 base.GetCurrentMessageContext()
            currentMessageContext.StorageData = ((int)currentMessageContext.StorageData) + 1;
            GlobalMessageContext.UpdateMessageContext(currentMessageContext);//储存到缓存
            await base.OnExecutedAsync(cancellationToken);
        }
    }
}