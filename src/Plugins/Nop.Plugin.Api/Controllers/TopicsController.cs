using Nop.Plugin.Api.JSON.Serializers;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Services.Topics;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Stores;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTO.Errors;
using Nop.Plugin.Api.DTO.Orders;
using Nop.Core;
using Nop.Plugin.Api.DTOs.Topics;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.Models.TopicsParameters;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.Factories;
using Nop.Core.Domain.Topics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Plugin.Api.Authorization.Attributes;
using Nop.Plugin.Api.DTOs.Taxes;

namespace Nop.Plugin.Api.Controllers
{
    [AuthorizePermission("ManageTopics")]
    public class TopicsController : BaseApiController
    {
        private readonly ITopicService _topicService;
        private readonly IStoreContext _storeContext;
        private readonly IDTOHelper _dtoHelper;
        private readonly IFactory<Topic> _factory;

        public TopicsController(
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ITopicService topicService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IDTOHelper dtoHelper,
            IPictureService pictureService,
            IFactory<Topic> factory

            ) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService,
                   storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _topicService = topicService;
            _storeContext = storeContext;
            _dtoHelper = dtoHelper;
            _factory = factory;
        }

        /// <summary>
        ///     Receive a list of all Topics
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/topics", Name = "GetTopics")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetTopics([FromQuery] TopicsParametersModel parameters)
        {
            var storeId = _storeContext.GetCurrentStore().Id;

            var topics = await _topicService.GetAllTopicsAsync(storeId);

            IList<TopicDto> topicsAsDtos = topics.Select(x => _dtoHelper.PrepareTopicDTO(x)).ToList();

            var topicsRootObject = new TopicsRootObject
            {
                Topics = topicsAsDtos
            };

            var json = JsonFieldsSerializer.Serialize(topicsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Retrieve topic by specified id
        /// </summary>
        /// ///
        /// <param name="id">Id of the topic</param>
        /// <param name="fields">Fields from the topic you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/topics/{id}", Name = "GetTopicById")]
        [ProducesResponseType(typeof(TopicsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetTopicById([FromRoute] int id, [FromQuery] string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var topic = await _topicService.GetTopicByIdAsync(id);

            if (topic == null)
            {
                return Error(HttpStatusCode.NotFound, "topic", "not found");
            }

            var topicsRootObject = new TopicsRootObject();

            var topicDto = _dtoHelper.PrepareTopicDTO(topic);
            topicsRootObject.Topics.Add(topicDto);

            var json = JsonFieldsSerializer.Serialize(topicsRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/topics", Name = "CreateTopic")]
        [ProducesResponseType(typeof(TopicsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> CreateTopic(
            [FromBody]
            TopicDto dto)
        {
            var jsonString = JsonConvert.SerializeObject(dto);
            var jsonProperties = JObject.Parse(jsonString).Properties().ToList();
            var objectPropertyNameValuePairs = jsonProperties.ToDictionary<JProperty, string, object>(jsonProperty => jsonProperty.Name, jsonProperty => jsonProperty.Value);

            var topicDelta = new Delta<TopicDto>(objectPropertyNameValuePairs);
            topicDelta.Dto = dto;
            
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            var newTopic = await _factory.InitializeAsync();
            newTopic.Title = topicDelta.Dto.Title;
            newTopic.Body = topicDelta.Dto.Body;
            newTopic.SystemName = topicDelta.Dto.SystemName;

            await _topicService.InsertTopicAsync(newTopic);

            var topicsRootObject = new TopicsRootObject();

            var topicDto = _dtoHelper.PrepareTopicDTO(newTopic);

            topicsRootObject.Topics.Add(topicDto);

            var json = JsonFieldsSerializer.Serialize(topicsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/topics/{id}", Name = "UpdateTopic")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> UpdateTopic(
            [FromBody]
            TopicDto dto)
        {
            var jsonString = JsonConvert.SerializeObject(dto);
            var jsonProperties = JObject.Parse(jsonString).Properties().ToList();
            var objectPropertyNameValuePairs = jsonProperties.ToDictionary<JProperty, string, object>(jsonProperty => jsonProperty.Name, jsonProperty => jsonProperty.Value);

            var topicDelta = new Delta<TopicDto>(objectPropertyNameValuePairs);
            topicDelta.Dto = dto;

            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            var currentTopic = await _topicService.GetTopicByIdAsync(topicDelta.Dto.Id);

            if (currentTopic == null)
            {
                return Error(HttpStatusCode.NotFound, "topic", "not found");
            }

            topicDelta.Merge(currentTopic);

            await _topicService.UpdateTopicAsync(currentTopic);

            var topicsRootObject = new TopicsRootObject();

            var topicDto = _dtoHelper.PrepareTopicDTO(currentTopic);

            topicsRootObject.Topics.Add(topicDto);

            var json = JsonFieldsSerializer.Serialize(topicsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/topics/{id}", Name = "DeleteTopic")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeleteTopic([FromRoute] int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var topicToDelete = await _topicService.GetTopicByIdAsync(id);

            if (topicToDelete == null)
            {
                return Error(HttpStatusCode.NotFound, "topic", "not found");
            }

            await _topicService.DeleteTopicAsync(topicToDelete);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteTopic", await LocalizationService.GetResourceAsync("ActivityLog.DeleteTopic"), topicToDelete);

            return Json(new { status = "ok" });
        }
    }
}
