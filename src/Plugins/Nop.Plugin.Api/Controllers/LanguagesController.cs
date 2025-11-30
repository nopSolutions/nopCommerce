using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTO.Errors;
using Nop.Plugin.Api.DTO.Languages;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Services.Authentication;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
	public class LanguagesController : BaseApiController
	{
		private readonly IDTOHelper _dtoHelper;
		private readonly ICustomerApiService _customerApiService;
		private readonly IAuthenticationService _authenticationService;
		private readonly ILanguageService _languageService;

		public LanguagesController(
			IJsonFieldsSerializer jsonFieldsSerializer,
			IAclService aclService,
			ICustomerService customerService,
			IStoreMappingService storeMappingService,
			IStoreService storeService,
			IDiscountService discountService,
			ICustomerActivityService customerActivityService,
			ILocalizationService localizationService,
			IPictureService pictureService,
			ILanguageService languageService,
			IDTOHelper dtoHelper,
			ICustomerApiService customerApiService,
			IAuthenticationService authenticationService)
			: base(jsonFieldsSerializer,
				   aclService,
				   customerService,
				   storeMappingService,
				   storeService,
				   discountService,
				   customerActivityService,
				   localizationService,
				   pictureService)
		{
			_languageService = languageService;
			_dtoHelper = dtoHelper;
			_customerApiService = customerApiService;
			_authenticationService = authenticationService;
		}

		/// <summary>
		///     Retrieve all languages
		/// </summary>
		/// <param name="fields">Fields from the language you want your json to contain</param>
		/// <response code="200">OK</response>
		/// <response code="401">Unauthorized</response>
		[HttpGet]
		[Route("/api/languages", Name = "GetAllLanguages")]
		[ProducesResponseType(typeof(LanguagesRootObject), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
		[ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
		[GetRequestsErrorInterceptorActionFilter]
		public async Task<IActionResult> GetAllLanguages([FromQuery] int? storeId = null, [FromQuery] string fields = null)
		{
			// no permissions required

			var allLanguages = await _languageService.GetAllLanguagesAsync(storeId: storeId ?? 0);

			IList<LanguageDto> languagesAsDto = await allLanguages.SelectAwait(async language => await _dtoHelper.PrepareLanguageDtoAsync(language)).ToListAsync();

			var languagesRootObject = new LanguagesRootObject
			{
				Languages = languagesAsDto
			};

			var json = JsonFieldsSerializer.Serialize(languagesRootObject, fields ?? "");

			return new RawJsonActionResult(json);
		}

		[HttpGet]
		[Route("/api/languages/current", Name = "GetCurrentLanguage")]
		[ProducesResponseType(typeof(LanguageDto), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
		[ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.Unauthorized)]
		public async Task<IActionResult> GetCurrentLanguage()
		{
			var customer = await _authenticationService.GetAuthenticatedCustomerAsync();
			if (customer is null)
				return Error(HttpStatusCode.Unauthorized);
			// no permissions required
			var language = await _customerApiService.GetCustomerLanguageAsync(customer);
			if (language is null)
				return NoContent();
			var languageDto = await _dtoHelper.PrepareLanguageDtoAsync(language);
			return Ok(languageDto);
		}

		[HttpPost]
		[Route("/api/languages/current", Name = "SetCurrentLanguage")]
		[ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
		[ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.NotFound)]
		[ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.Unauthorized)]
		public async Task<IActionResult> SetCurrentLanguage([FromQuery] int id)
		{
			var customer = await _authenticationService.GetAuthenticatedCustomerAsync();
			if (customer is null)
				return Error(HttpStatusCode.Unauthorized);
			// no permissions required
			var language = await _languageService.GetLanguageByIdAsync(id);
			if (language is null)
				return NotFound();
			await _customerApiService.SetCustomerLanguageAsync(customer, language);
			return NoContent();
		}
	}
}
