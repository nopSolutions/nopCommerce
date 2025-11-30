using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nop.Plugin.Api.DTOs.ShoppingCarts
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum ShoppingCartType
	{
		ShoppingCart = 1,
		Wishlist = 2
	}
}
