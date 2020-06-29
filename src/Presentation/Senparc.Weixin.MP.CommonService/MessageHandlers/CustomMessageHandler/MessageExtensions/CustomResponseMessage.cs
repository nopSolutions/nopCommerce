using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Infrastructure;
using Nop.Core.Domain.Weixin;
using Nop.Services.Weixin;
using Senparc.NeuChar.Entities;
using Senparc.Weixin.MP.Entities;
using Senparc.NeuChar.MessageHandlers;
using Nop.Services.Catalog;
using Nop.Services.Suppliers;
using Nop.Services.Marketing;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Marketing;

namespace Senparc.Weixin.MP.CommonService.CustomMessageHandler
{
    public partial class CustomMessageHandler
    {
        public IResponseMessageBase GetResponseMessagesByIds(List<int> messageIds, WResponseType? wResponseType = null)
        {
            IResponseMessageBase responseMessage = null;

            var wmessageService = EngineContext.Current.Resolve<IWMessageService>();
            var wmessages = wmessageService.GetWMessagesByIds(messageIds.ToArray());

            if (wmessages != null && wmessages.Count > 0)
            {
                foreach (var messageEntity in wmessages)
                {
                    WResponseType? currentResponseType = null;

                    //强制执行指定回复方式
                    if (wResponseType.HasValue)
                        currentResponseType = wResponseType;
                    else
                        currentResponseType = messageEntity.ResponseType;


                    if (currentResponseType == WResponseType.Custom)//客服方式回复
                    {
                        CustomResponseMessage(messageEntity, out responseMessage);
                    }
                    else if (currentResponseType == WResponseType.Passive)//被动方式回复
                    {
                        PassiveResponseMessage(messageEntity, out responseMessage);
                    }
                }
            }

            return responseMessage;
        }

        protected void CustomResponseMessage(WMessage message, out IResponseMessageBase responseMessage)
        {
            responseMessage = null;

            switch (message.MessageType)
            {
                case WMessageType.Text:
                    {
                        if (!string.IsNullOrEmpty(message.Content))
                        {
                            AdvancedAPIs.CustomApi.SendText(_senparcWeixinSetting.WeixinAppId,
                                OpenId, message.Content, kfAccount: message.KfAccount);
                        }
                        break;
                    }
                case WMessageType.Image:
                    {
                        var messageService = EngineContext.Current.Resolve<IWMessageService>();
                        //是否永久素材ID
                        if (!message.MaterialMsg)
                        {
                            if (string.IsNullOrEmpty(message.MediaId) ||  /*没有生成MediaId*/
                                message.CreatTime + 259200 - 3600 < Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now)  /*3天过期*/
                                )
                            {
                                if (!string.IsNullOrEmpty(message.PicUrl))
                                {
                                    //上传素材
                                    var uploadResult = AdvancedAPIs.MediaApi.UploadTemporaryMedia(_senparcWeixinSetting.WeixinAppId,
                                        UploadMediaFileType.image,
                                        _fileProvider.MapPath(message.PicUrl));

                                    //更新
                                    if (string.IsNullOrEmpty(uploadResult.errmsg))
                                    {
                                        message.MediaId = uploadResult.media_id;
                                        message.CreatTime = (int)uploadResult.created_at;
                                        messageService.UpdateWMessage(message);
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(message.MediaId))
                        {
                            AdvancedAPIs.CustomApi.SendImage(_senparcWeixinSetting.WeixinAppId,
                                OpenId, message.MediaId, kfAccount: message.KfAccount);
                        }
                        break;
                    }
                case WMessageType.Voice:
                    {
                        if (!string.IsNullOrEmpty(message.MediaId))
                        {
                            AdvancedAPIs.CustomApi.SendVoice(_senparcWeixinSetting.WeixinAppId,
                                OpenId, message.MediaId, kfAccount: message.KfAccount);
                        }
                        break;
                    }
                case WMessageType.Video:
                    {
                        if (!string.IsNullOrEmpty(message.MediaId) &&
                            !string.IsNullOrEmpty(message.Title) &&
                            !string.IsNullOrEmpty(message.Description) &&
                            !string.IsNullOrEmpty(message.ThumbMediaId)
                            )
                        {
                            AdvancedAPIs.CustomApi.SendVideo(_senparcWeixinSetting.WeixinAppId,
                                OpenId, message.MediaId, message.Title, message.Description,
                                kfAccount: message.KfAccount, thumb_media_id: message.ThumbMediaId);
                        }
                        break;
                    }
                case WMessageType.Music:
                    {
                        if (!string.IsNullOrEmpty(message.ThumbMediaId))
                        {
                            AdvancedAPIs.CustomApi.SendMusic(_senparcWeixinSetting.WeixinAppId,
                                OpenId, message.Title, message.Description,
                                message.MusicUrl, message.HqMusicUrl,
                                message.ThumbMediaId, kfAccount: message.KfAccount);
                        }
                        break;
                    }
                case WMessageType.News:
                    {
                        var articles = new List<Article>();

                        if (!string.IsNullOrEmpty(message.Title) &&
                            !string.IsNullOrEmpty(message.Description) &&
                            !string.IsNullOrEmpty(message.PicUrl) &&
                            !string.IsNullOrEmpty(message.Url)
                            )
                        {
                            articles.Add(new Article
                            {
                                Title = message.Title,
                                Description = message.Description,
                                PicUrl = articles.Count == 0 ? message.PicUrl : message.ThumbPicUrl,  //第二条开始为小图
                                Url = message.Url
                            });
                        }

                        if (articles.Count > 0)
                        {
                            AdvancedAPIs.CustomApi.SendNews(_senparcWeixinSetting.WeixinAppId,
                            OpenId, articles, kfAccount: message.KfAccount);
                        }
                        break;
                    }
                case WMessageType.MpNews:
                    {
                        if (!string.IsNullOrEmpty(message.MediaId))
                        {
                            AdvancedAPIs.CustomApi.SendMpNews(_senparcWeixinSetting.WeixinAppId,
                                OpenId, message.MediaId, kfAccount: message.KfAccount);
                        }
                        break;
                    }
                case WMessageType.WxCard:
                    {
                        AdvancedAPIs.CardExt cardExt = null;
                        AdvancedAPIs.CustomApi.SendCard(_senparcWeixinSetting.WeixinAppId,
                            OpenId, message.MediaId, cardExt);
                        break;
                    }
                case WMessageType.MiniProgramPage:
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// 被动消息，每次只能有1条被动消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="responseMessage"></param>
        protected void PassiveResponseMessage(WMessage message, out IResponseMessageBase responseMessage)
        {
            responseMessage = null;

            switch (message.MessageType)
            {
                case WMessageType.Text:
                    {
                        if (!string.IsNullOrEmpty(message.Content))
                        {
                            var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                            strongResponseMessage.Content = message.Content;
                            responseMessage = strongResponseMessage;
                        }
                        break;
                    }
                case WMessageType.Image:
                    {
                        var strongResponseMessage = CreateResponseMessage<ResponseMessageImage>();

                        var messageService = EngineContext.Current.Resolve<IWMessageService>();
                        //是否永久素材ID
                        if (!message.MaterialMsg)
                        {
                            if (string.IsNullOrEmpty(message.MediaId) ||  /*没有生成MediaId*/
                                message.CreatTime + 259200 - 3600 < Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now)  /*3天过期*/
                                )
                            {
                                if (!string.IsNullOrEmpty(message.PicUrl))
                                {
                                    //上传素材
                                    var uploadResult = AdvancedAPIs.MediaApi.UploadTemporaryMedia(_senparcWeixinSetting.WeixinAppId,
                                        UploadMediaFileType.image,
                                        _fileProvider.MapPath(message.PicUrl));

                                    //更新
                                    if (string.IsNullOrEmpty(uploadResult.errmsg))
                                    {
                                        message.MediaId = uploadResult.media_id;
                                        message.CreatTime = (int)uploadResult.created_at;
                                        messageService.UpdateWMessage(message);
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(message.MediaId))
                        {
                            strongResponseMessage.Image.MediaId = message.MediaId;
                            responseMessage = strongResponseMessage;
                        }

                        break;
                    }
                case WMessageType.Voice:
                    {
                        var strongResponseMessage = CreateResponseMessage<ResponseMessageVoice>();

                        var messageService = EngineContext.Current.Resolve<IWMessageService>();
                        //是否永久素材ID
                        if (!message.MaterialMsg)
                        {
                            if (string.IsNullOrEmpty(message.MediaId) ||  /*没有生成MediaId*/
                                message.CreatTime + 259200 - 3600 < Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now)  /*3天过期*/
                                )
                            {
                                if (!string.IsNullOrEmpty(message.MusicUrl))
                                {
                                    //上传素材
                                    var uploadResult = AdvancedAPIs.MediaApi.UploadTemporaryMedia(_senparcWeixinSetting.WeixinAppId,
                                        UploadMediaFileType.voice,
                                        _fileProvider.MapPath(message.MusicUrl));

                                    //更新
                                    if (string.IsNullOrEmpty(uploadResult.errmsg))
                                    {
                                        message.MediaId = uploadResult.media_id;
                                        message.CreatTime = (int)uploadResult.created_at;
                                        messageService.UpdateWMessage(message);
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(message.MediaId))
                        {
                            strongResponseMessage.Voice.MediaId = message.MediaId;
                            responseMessage = strongResponseMessage;
                        }
                        break;
                    }
                case WMessageType.Video:
                    {
                        var strongResponseMessage = CreateResponseMessage<ResponseMessageVideo>();

                        var messageService = EngineContext.Current.Resolve<IWMessageService>();
                        //是否永久素材ID
                        if (!message.MaterialMsg)
                        {
                            if (string.IsNullOrEmpty(message.MediaId) ||  /*没有生成MediaId*/
                                message.CreatTime + 259200 - 3600 < Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now)  /*3天过期*/
                                )
                            {
                                if (!string.IsNullOrEmpty(message.MusicUrl))
                                {
                                    //上传素材
                                    var uploadResultMediaId = AdvancedAPIs.MediaApi.UploadTemporaryMedia(_senparcWeixinSetting.WeixinAppId,
                                        UploadMediaFileType.video,
                                        _fileProvider.MapPath(message.MusicUrl));

                                    if (string.IsNullOrEmpty(uploadResultMediaId.errmsg))
                                    {
                                        message.MediaId = uploadResultMediaId.media_id;
                                        message.CreatTime = (int)uploadResultMediaId.created_at;
                                    }

                                    //上传缩微图
                                    if (!string.IsNullOrEmpty(message.ThumbPicUrl))
                                    {
                                        var uploadResultThumbId = AdvancedAPIs.MediaApi.UploadTemporaryMedia(_senparcWeixinSetting.WeixinAppId,
                                        UploadMediaFileType.thumb,
                                        _fileProvider.MapPath(message.ThumbPicUrl));

                                        if (string.IsNullOrEmpty(uploadResultThumbId.errmsg))
                                        {
                                            message.ThumbMediaId = uploadResultMediaId.media_id;
                                        }
                                    }

                                    //更新
                                    if (string.IsNullOrEmpty(uploadResultMediaId.errmsg))
                                    {
                                        messageService.UpdateWMessage(message);
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(message.MediaId))
                        {
                            strongResponseMessage.Video.Title = message.Title;
                            strongResponseMessage.Video.Description = message.Description;
                            strongResponseMessage.Video.MediaId = message.MediaId;
                            responseMessage = strongResponseMessage;
                        }
                        break;
                    }
                case WMessageType.Music:
                    {
                        var strongResponseMessage = CreateResponseMessage<ResponseMessageMusic>();

                        var messageService = EngineContext.Current.Resolve<IWMessageService>();
                        //是否永久素材ID
                        if (!message.MaterialMsg)
                        {
                            if (string.IsNullOrEmpty(message.ThumbMediaId) ||  /*没有生成ThumbMediaId*/
                                message.CreatTime + 259200 - 3600 < Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now)  /*3天过期*/
                                )
                            {
                                if (!string.IsNullOrEmpty(message.ThumbPicUrl))
                                {
                                    //上传素材
                                    var uploadResult = AdvancedAPIs.MediaApi.UploadTemporaryMedia(_senparcWeixinSetting.WeixinAppId,
                                        UploadMediaFileType.thumb,
                                        _fileProvider.MapPath(message.ThumbPicUrl));

                                    //更新
                                    if (string.IsNullOrEmpty(uploadResult.errmsg))
                                    {
                                        message.MediaId = uploadResult.media_id;
                                        message.ThumbMediaId = uploadResult.media_id;
                                        message.CreatTime = (int)uploadResult.created_at;
                                        messageService.UpdateWMessage(message);
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(message.ThumbMediaId))
                        {
                            strongResponseMessage.Music.Title = message.Title;
                            strongResponseMessage.Music.Description = message.Description;
                            strongResponseMessage.Music.MusicUrl = message.MusicUrl;
                            strongResponseMessage.Music.HQMusicUrl = message.HqMusicUrl;
                            strongResponseMessage.Music.ThumbMediaId = message.ThumbMediaId;

                            responseMessage = strongResponseMessage;
                        }
                        break;
                    }
                case WMessageType.News:
                    {
                        var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();

                        if (!string.IsNullOrEmpty(message.Title) &&
                            !string.IsNullOrEmpty(message.Description) &&
                            !string.IsNullOrEmpty(message.PicUrl) &&
                            !string.IsNullOrEmpty(message.Url)
                            )
                        {
                            strongResponseMessage.Articles.Add(new Article
                            {
                                Title = message.Title,
                                Description = message.Description,
                                PicUrl = strongResponseMessage.Articles.Count == 0 ? message.PicUrl : message.ThumbPicUrl,  //第二条开始为小图
                                Url = message.Url
                            });
                        }

                        //TODO: 多条消息的ID存储在MediaId中，MediaId对应id消息类型只能是News类型

                        if (strongResponseMessage.Articles.Count > 0)
                            responseMessage = strongResponseMessage;

                        break;
                    }
                case WMessageType.MpNews:
                    {
                        break;
                    }
                case WMessageType.WxCard:
                    {
                        break;
                    }
                case WMessageType.MiniProgramPage:
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }


        public IResponseMessageBase GetQrCodeLimitPassiveResponseMessage(QrCodeLimitBindingSource source)
        {
            IResponseMessageBase responseMessage = null;

            var content = string.Empty;

            var title = string.Empty;
            var description = string.Empty;
            var url = string.Empty;
            var imageUrl = string.Empty;
            var thumbImageUrl = string.Empty;
            var adverImageUrl = string.Empty;

            switch (source.WSceneType)
            {
                case WSceneType.Adver:
                    {
                        //可以为广告竖图
                        if (source.ProductId > 0)
                        {
                            var productService = EngineContext.Current.Resolve<IProductService>();
                            var product = productService.GetProductById(source.ProductId);
                            if (product != null)
                            {
                                title = product.Name;
                                description = product.ShortDescription;
                                imageUrl = product.CoverImageUrl;
                                thumbImageUrl = product.CoverThumbImageUrl;
                                url = source.UseFixUrl ? source.Url : "http://www.autosetyourproducturl.com/";
                            }

                            if (product != null)
                            {
                                //获取产品绑定的首张图片
                                var productAdvertImageService = EngineContext.Current.Resolve<IProductAdvertImageService>();
                                var productAdvertImages = productAdvertImageService.GetEntitiesByProductId(product.Id, 1);
                                if (productAdvertImages.Count > 0)
                                {
                                    adverImageUrl = productAdvertImages[0].ImageUrl;
                                }
                            }

                        }
                        break;
                    }
                case WSceneType.Message:
                    {
                        content = source.Content;
                        break;
                    }
                case WSceneType.Product:
                    {
                        if (source.ProductId > 0)
                        {
                            var productService = EngineContext.Current.Resolve<IProductService>();
                            var product = productService.GetProductById(source.ProductId);
                            if (product != null)
                            {
                                title = product.Name;
                                description = product.ShortDescription;
                                imageUrl = product.CoverImageUrl;
                                thumbImageUrl = product.CoverThumbImageUrl;
                                url = source.UseFixUrl ? source.Url : "http://www.autosetyourproducturl.com/";
                            }

                            if (product != null)
                            {
                                //获取产品绑定的首张图片
                                var productAdvertImageService = EngineContext.Current.Resolve<IProductAdvertImageService>();
                                var productAdvertImages = productAdvertImageService.GetEntitiesByProductId(product.Id, 1);
                                if (productAdvertImages.Count > 0)
                                {
                                    adverImageUrl = productAdvertImages[0].ImageUrl;
                                }
                            }
                        }
                        break;
                    }
                case WSceneType.Supplier:
                    {
                        if (source.SupplierId > 0 && source.SupplierShopId > 0)
                        {
                            var supplierShopService = EngineContext.Current.Resolve<ISupplierShopService>();
                            var supplierShop = supplierShopService.GetEntityById(source.SupplierShopId);
                            if (supplierShop != null)
                            {
                                title = supplierShop.Name;
                                description = supplierShop.Description;
                                imageUrl = supplierShop.ImageUrl;
                                thumbImageUrl = supplierShop.ThumbImageUrl;
                                url = source.UseFixUrl ? source.Url : "http://www.autosetyoursuppliershopurl.com/";
                            }
                        }
                        break;
                    }
                case WSceneType.Command:
                case WSceneType.GiftCard://卡券领取操作放到二维码扫码事件或二维码关注事件中，同永久二维码一起处理
                case WSceneType.IDCard:
                case WSceneType.Verify:
                case WSceneType.Vote:
                case WSceneType.None:
                default:
                    {
                        return null;
                    }
            }

            switch (source.MessageType)
            {
                case WMessageType.Text:
                    {
                        if (!string.IsNullOrEmpty(content))
                        {
                            var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                            strongResponseMessage.Content = content;
                            responseMessage = strongResponseMessage;
                        }
                        break;
                    }
                case WMessageType.Image:
                    {
                        if (!string.IsNullOrEmpty(adverImageUrl))
                        {
                            //上传素材
                            var uploadResult = AdvancedAPIs.MediaApi.UploadTemporaryMedia(_senparcWeixinSetting.WeixinAppId,
                                UploadMediaFileType.image,
                                _fileProvider.MapPath(adverImageUrl));

                            //更新
                            if (string.IsNullOrEmpty(uploadResult.errmsg) && !string.IsNullOrEmpty(uploadResult.media_id))
                            {
                                var strongResponseMessage = CreateResponseMessage<ResponseMessageImage>();

                                strongResponseMessage.Image.MediaId = uploadResult.media_id;
                                responseMessage = strongResponseMessage;
                            }
                        }

                        break;
                    }
                case WMessageType.Voice:
                    {
                        break;
                    }
                case WMessageType.Video:
                    {
                        break;
                    }
                case WMessageType.Music:
                    {
                        break;
                    }
                case WMessageType.News:
                    {
                        var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();

                        if (!string.IsNullOrEmpty(title) &&
                            !string.IsNullOrEmpty(description) &&
                            !string.IsNullOrEmpty(imageUrl) &&
                            !string.IsNullOrEmpty(url)
                            )
                        {
                            strongResponseMessage.Articles.Add(new Article
                            {
                                Title = title,
                                Description = description,
                                PicUrl = strongResponseMessage.Articles.Count == 0 ? imageUrl : thumbImageUrl,  //第二条开始为小图
                                Url = url
                            });


                        }

                        //TODO: 多条消息的ID存储在MediaId中，MediaId对应id消息类型只能是News类型

                        if (strongResponseMessage.Articles.Count > 0)
                            responseMessage = strongResponseMessage;

                        break;
                    }
                case WMessageType.MpNews:
                    {
                        break;
                    }
                case WMessageType.WxCard:
                    {
                        break;
                    }
                case WMessageType.MiniProgramPage:
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return responseMessage;
        }

        public IResponseMessageBase GetQrCodeTempPassiveResponseMessage(Nop.Services.Weixin.QrCodeSceneString.QrCodeSceneParam source)
        {
            IResponseMessageBase responseMessage = null;

            switch (source.SceneType)
            {
                case WSceneType.Adver:
                    {
                        int.TryParse(source.Value, out var adverId);
                        //可以为广告竖图
                        if (adverId > 0)
                        {
                            //获取广告图片
                            var productAdvertImageService = EngineContext.Current.Resolve<IProductAdvertImageService>();
                            var productAdvertImages = productAdvertImageService.GetEntityById(adverId);
                            if (productAdvertImages != null)
                            {
                                if (!string.IsNullOrEmpty(productAdvertImages.ImageUrl))
                                {
                                    //上传素材
                                    var uploadResult = AdvancedAPIs.MediaApi.UploadTemporaryMedia(_senparcWeixinSetting.WeixinAppId,
                                        UploadMediaFileType.image,
                                        _fileProvider.MapPath(productAdvertImages.ImageUrl));

                                    if (string.IsNullOrEmpty(uploadResult.errmsg) && !string.IsNullOrEmpty(uploadResult.media_id))
                                    {
                                        var strongResponseMessage = CreateResponseMessage<ResponseMessageImage>();
                                        strongResponseMessage.Image.MediaId = uploadResult.media_id;
                                        responseMessage = strongResponseMessage;
                                    }
                                }
                            }

                        }
                        break;
                    }
                case WSceneType.Message:
                    {
                        if (!string.IsNullOrEmpty(source.Value))
                        {
                            var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                            strongResponseMessage.Content = source.Value;
                            responseMessage = strongResponseMessage;
                        }
                        break;
                    }
                case WSceneType.Product:
                    {
                        int.TryParse(source.Value, out var productId);
                        if (productId > 0)
                        {
                            var productService = EngineContext.Current.Resolve<IProductService>();
                            var product = productService.GetProductById(productId);
                            if (product != null)
                            {
                                var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();

                                if (!string.IsNullOrEmpty(product.Name) &&
                                    !string.IsNullOrEmpty(product.ShortDescription) &&
                                    !string.IsNullOrEmpty(product.CoverThumbImageUrl)
                                    )
                                {
                                    strongResponseMessage.Articles.Add(new Article
                                    {
                                        Title = product.Name,
                                        Description = product.ShortDescription,
                                        PicUrl = product.CoverImageUrl,    //第二条开始应为小图
                                        Url = "http://www.autosetyourproducturl.com/"
                                    });
                                }

                                //TODO: 多条消息的ID存储在MediaId中，MediaId对应id消息类型只能是News类型

                                if (strongResponseMessage.Articles.Count > 0)
                                    responseMessage = strongResponseMessage;
                            }
                        }
                        break;
                    }
                case WSceneType.Supplier:
                    {
                        int.TryParse(source.Value, out var supplierId);
                        int.TryParse(source.Value1, out var supplierShopId);
                        if (supplierId > 0 && supplierShopId > 0)
                        {
                            var supplierShopService = EngineContext.Current.Resolve<ISupplierShopService>();
                            var supplierShop = supplierShopService.GetEntityById(supplierShopId);
                            if (supplierShop != null)
                            {
                                var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();

                                if (!string.IsNullOrEmpty(supplierShop.Name) &&
                                   !string.IsNullOrEmpty(supplierShop.Description) &&
                                   !string.IsNullOrEmpty(supplierShop.ImageUrl)
                                   )
                                {
                                    strongResponseMessage.Articles.Add(new Article
                                    {
                                        Title = supplierShop.Name,
                                        Description = supplierShop.Description,
                                        PicUrl = supplierShop.ImageUrl,  //第二条开始应为小图
                                        Url = "http://www.autosetyourproducturl.com/"
                                    });
                                }

                                //TODO: 多条消息的ID存储在MediaId中，MediaId对应id消息类型只能是News类型

                                if (strongResponseMessage.Articles.Count > 0)
                                    responseMessage = strongResponseMessage;
                            }
                        }
                        break;
                    }
                case WSceneType.Verify:
                    {
                        if (!string.IsNullOrEmpty(source.Value))
                        {
                            var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                            strongResponseMessage.Content = source.Value;
                            responseMessage = strongResponseMessage;
                        }
                        break;
                    }
                case WSceneType.Command:
                    {
                        if (!string.IsNullOrEmpty(source.Value))
                        {
                            var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                            strongResponseMessage.Content = "命令执行完成";
                            responseMessage = strongResponseMessage;
                        }
                        break;
                    }
                case WSceneType.Vote:
                    {
                        if (!string.IsNullOrEmpty(source.Value))
                        {
                            var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                            strongResponseMessage.Content = "投票成功";
                            responseMessage = strongResponseMessage;
                        }
                        break;
                    }
                case WSceneType.GiftCard: //卡券领取操作放到二维码扫码事件或二维码关注事件中，同永久二维码一起处理
                case WSceneType.IDCard:
                case WSceneType.None:
                default:
                    {
                        break;
                    }
            }

            return responseMessage;
        }

        public IResponseMessageBase GetAndSetSupplierVoucherCouponPassiveResponseMessage(List<int> cardIds, Nop.Services.Weixin.QrCodeSceneString.QrCodeSceneParam qrCodeSceneParam, string currentOpenId)
        {
            IResponseMessageBase responseMessage = null;
            var receiveNumber = 0; //本次领取总数

            var userAssetIncomeHistoryService = EngineContext.Current.Resolve<IUserAssetIncomeHistoryService>();
            var wuserService = EngineContext.Current.Resolve<IWUserService>();
            var supplierVoucherCouponService = EngineContext.Current.Resolve<ISupplierVoucherCouponService>();

            var wuser = wuserService.GetWUserByOpenId(currentOpenId);
            if (wuser == null)
                return null;

            //当前用户Id赋值
           var wuserId = wuser.Id;

            var supplierVoucherCoupons = supplierVoucherCouponService.GetEntitiesByIds(cardIds.ToArray());

            //循环给扫码用户赠送卡券到账户（判断领取条件和是否重复）

            foreach (var item in supplierVoucherCoupons)
            {
                if (item.LimitUsableNumber > 0)
                {
                    var userAssetIncomeHistory = userAssetIncomeHistoryService.GetEntitiesBySupplierVoucherCouponId(wuserId, item.Id, true);
                    if (userAssetIncomeHistory.Count >= item.LimitUsableNumber)
                        continue;
                }
                if (item.LimitReceiveNumber > 0)
                {
                    var userAssetIncomeHistory = userAssetIncomeHistoryService.GetEntitiesBySupplierVoucherCouponId(wuserId, item.Id);
                    if (userAssetIncomeHistory.Count >= item.LimitUsableNumber)
                        continue;
                }

                //为当前用户添加卡券资产
                userAssetIncomeHistoryService.InsertEntityBysupplierVoucherCouponParams(item, wuserId, 0, null, qrCodeSceneParam.SceneType);

                receiveNumber++; //成功领取计数
            }

            string descripion;
            if (receiveNumber > 0)
                descripion = string.Format("您已成功领取{0}张卡券，请前往卡券管理页面查看或使用！", receiveNumber);
            else
                descripion = "该卡券已领取，同一账号请勿重复领取！(前往卡券管理页面查看/使用)";

            if (supplierVoucherCoupons.Count == 0)
                descripion = "抱歉，该卡券已领完，请关注其他优惠活动！";

            //回复卡券领取成功消息
            var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
            strongResponseMessage.Articles.Add(new Article
            {
                Title = "卡券领取通知",
                Description = descripion,
                PicUrl = "http://www.giftcardsuccess.com/giftcard.jpg",
                Url = "http://www.giftcardsuccess.com/"
            });

            //TODO: 多条消息的ID存储在MediaId中，MediaId对应id消息类型只能是News类型

            if (strongResponseMessage.Articles.Count > 0)
                responseMessage = strongResponseMessage;

            return responseMessage;
        }

    }
}
