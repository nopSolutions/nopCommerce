using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.CustomAPI.Models;
using Nop.Plugin.CustomAPI.Models.DTO;
using Nop.Plugin.CustomAPI.Repository;
using Nop.Plugin.CustomAPI.Repository.IRepository;

namespace Nop.Plugin.CustomAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShoppingCartItemsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        protected APIResponse _response;
        private readonly IMapper _mapper;

        public ShoppingCartItemsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            this._response = new APIResponse();
        }

        [HttpGet(Name = "GetAllCartItems")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetAllCartItems()
        {
            try
            {
                var shoppingCart = await _unitOfWork.ProductRepository.GetCartItemAsync();
                if (shoppingCart == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = true;
                    return NoContent();
                }

                _response.Result = shoppingCart;
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>
                {
                    ex.ToString()
                };
            }

            return _response;

        }

        [HttpPost("AddProductTocart/{id}", Name = "AddProductTocart")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> AddProductTocart([FromRoute] int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = true;
                    return BadRequest(_response);
                }
                var products = await _unitOfWork.ProductRepository.GetByIdAsync(id);
                if (products == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>
                      {
                         "product not found"
                      };
                    return NotFound(_response);
                }

                var shoppingcart = await _unitOfWork.ProductRepository.AddCartItemAsync(products);

                _response.Result = shoppingcart;
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>
                {
                    ex.ToString()
                };
            }

            return _response;

        }
    }
}