using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Infrastructure;
using Nop.Core.Domain.Weixin;
using Nop.Services.Weixin;
using Senparc.NeuChar.Entities;
using Senparc.Weixin.MP.Entities;
using Senparc.NeuChar.MessageHandlers;

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

                        if(articles.Count>0)
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

                                    if(string.IsNullOrEmpty(uploadResultMediaId.errmsg))
                                    {
                                        message.MediaId = uploadResultMediaId.media_id;
                                        message.CreatTime = (int)uploadResultMediaId.created_at;
                                    }

                                    //上传缩微图
                                    if(!string.IsNullOrEmpty(message.ThumbPicUrl))
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

    }
}
