/*----------------------------------------------------------------
    Copyright (C) 2020 Senparc
    
    文件名：LocationService.cs
    文件功能描述：地理位置信息处理
    
    
    创建标识：Senparc - 20150312
----------------------------------------------------------------*/

using System.Collections.Generic;
using Senparc.Weixin.MP.Entities;
using Senparc.CO2NET.Helpers.BaiduMap;
using Senparc.CO2NET.Helpers.GoogleMap;
using Senparc.Weixin.MP.Helpers;
using Senparc.CO2NET.Helpers;
using Senparc.NeuChar.Entities;

namespace Senparc.Weixin.MP.CommonService
{
    public class LocationService
    {
        public ResponseMessageNews GetResponseMessage(RequestMessageLocation requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage);

            #region 百度地图

            {
                var markersList = new List<BaiduMarkers>();
                markersList.Add(new BaiduMarkers()
                {
                    Longitude = requestMessage.Location_X,
                    Latitude = requestMessage.Location_Y,
                    Color = "red",
                    Label = "S",
                    Size = BaiduMarkerSize.m
                });

                var mapUrl = BaiduMapHelper.GetBaiduStaticMap(requestMessage.Location_X, requestMessage.Location_Y, 1, 6, markersList);
                responseMessage.Articles.Add(new Article()
                {
                    Description = string.Format("【来自百度地图】您刚才发送了地理位置信息。Location_X：{0}，Location_Y：{1}，Scale：{2}，标签：{3}",
                               requestMessage.Location_X, requestMessage.Location_Y,
                               requestMessage.Scale, requestMessage.Label),
                    PicUrl = mapUrl,
                    Title = "定位地点周边地图",
                    Url = mapUrl
                });
            }

            #endregion

            #region GoogleMap

            {
                var markersList = new List<GoogleMapMarkers>();
                markersList.Add(new GoogleMapMarkers()
                {
                    X = requestMessage.Location_X,
                    Y = requestMessage.Location_Y,
                    Color = "red",
                    Label = "S",
                    Size = GoogleMapMarkerSize.Default,
                });
                var mapSize = "480x600";
                var mapUrl = GoogleMapHelper.GetGoogleStaticMap(19 /*requestMessage.Scale*//*微信和GoogleMap的Scale不一致，这里建议使用固定值*/,
                                                                                markersList, mapSize);
                responseMessage.Articles.Add(new Article()
                {
                    Description = string.Format("【来自GoogleMap】您刚才发送了地理位置信息。Location_X：{0}，Location_Y：{1}，Scale：{2}，标签：{3}",
                                  requestMessage.Location_X, requestMessage.Location_Y,
                                  requestMessage.Scale, requestMessage.Label),
                    PicUrl = mapUrl,
                    Title = "定位地点周边地图",
                    Url = mapUrl
                });
            }

            #endregion


            responseMessage.Articles.Add(new Article()
            {
                Title = "微信公众平台SDK 官网链接",
                Description = "Senparc.Weixin.MK SDK地址",
                PicUrl = "https://sdk.weixin.senparc.com/images/logo.jpg",
                Url = "https://sdk.weixin.senparc.com"
            });

            return responseMessage;
        }
    }
}