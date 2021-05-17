using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Demo.CreateRoom.Domains;
using Nop.Plugin.Demo.CreateRoom.Services;
using Nop.Services.Catalog;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Demo.CreateRoom.Components
{
    internal class RoomViewComponent : NopViewComponent
    {
        private readonly IProductService _productService;
        private readonly IRoomService _roomService;

        public RoomViewComponent(IProductService productService, IRoomService roomService)
        {
            _productService = productService;
            _roomService = roomService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            if (!(additionalData is ProductDetailsModel model))
                return Content("");

            //Read from the product service
            var productById = await _productService.GetProductByIdAsync(model.Id);
            //If the product exists we will log it
            if (productById != null)
            {
                //Setup the product to save
                var record = new Room
                {
                    Id = model.Id,
                    Name = productById.Name,
                    Description = productById.FullDescription,
                };
                //Map the values we're interested in to our new entity
                _roomService.Log(record);
            }

            return Content("");
        }
    }
}