using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Nop.Core.Infrastructure;
using Nop.Data;

using Senparc.CO2NET.AspNet.HttpUtility;
using Senparc.NeuChar.MessageHandlers;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.CommonService;
using Senparc.Weixin.MP.CommonService.CustomMessageHandler;
using Senparc.Weixin.MP.CommonService.Mvc.Extension.Results;

namespace Senparc.Weixin.MP.CommonService.Controllers
{

    public partial class WeixinBaseController : Controller
    {

    }
}