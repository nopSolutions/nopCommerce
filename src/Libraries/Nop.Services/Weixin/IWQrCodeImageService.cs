using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Weixin;

namespace Nop.Services.Weixin
{
    /// <summary>
    /// WQrCodeImage Service interface
    /// </summary>
    public partial interface IWQrCodeImageService
    {
        void InsertWQrCodeImage(WQrCodeImage wQrCodeImage);

        void DeleteWQrCodeImage(WQrCodeImage wQrCodeImage, bool delete = false);

        void DeleteWQrCodeImages(IList<WQrCodeImage> wQrCodeImages, bool deleted = false);

        void UpdateWQrCodeImage(WQrCodeImage wQrCodeImage);

        void UpdateWQrCodeImages(IList<WQrCodeImage> wQrCodeImages);

        WQrCodeImage GetWQrCodeImageById(int id);

        List<WQrCodeImage> GetWQrCodeImagesByIds(int[] wQrCodeImageIds);

        IPagedList<WQrCodeImage> GetWQrCodeImages(int pageIndex = 0, int pageSize = int.MaxValue, bool showDeleted = false);

    }
}