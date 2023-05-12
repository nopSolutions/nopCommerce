using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.CustomAPI.Models;
using Nop.Plugin.CustomAPI.Models.DTO;
using Nop.Plugin.CustomAPI.Repository;
using Nop.Plugin.CustomAPI.Repository.IRepository;
using Nop.Services.Catalog;

namespace Nop.Plugin.CustomAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        protected APIResponse _response;
        private readonly IMapper _mapper;

        public ProductController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            this._response = new APIResponse();
        }


        [HttpGet(Name = "GetAllProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetAllProduct()
        {
            try
            {
                var products = _unitOfWork.ProductRepository.GetAll();

                if (products == null)
                {
                    _response.StatusCode = HttpStatusCode.NoContent;
                    _response.IsSuccess = true;
                    return NoContent();
                }
                _response.Result = _mapper.Map<List<ProductDTO>>(products);
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

        [HttpGet("GetProductByName/{search}", Name = "GetProductByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetProductByString(string? search)
        {
            try
            {
                if (search == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = true;
                    return BadRequest(_response);
                }

                var products = await _unitOfWork.ProductRepository.GetAllAsync(search);

                if (products.Count == 0)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = true;
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<List<ProductDTO>>(products);
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

        [HttpGet("{id}", Name = "GetProductById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetProductById([FromRoute] int id)
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
                    _response.IsSuccess = true;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<ProductDTO>(products);
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