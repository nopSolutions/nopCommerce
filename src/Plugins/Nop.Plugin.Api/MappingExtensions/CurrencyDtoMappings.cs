using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Api.DTO;
using Nop.Plugin.Api.AutoMapper;

namespace Nop.Plugin.Api.MappingExtensions
{
	public static class CurrencyDtoMappings
	{
        public static CurrencyDto ToDto(this Currency currency)
        {
            return currency.MapTo<Currency, CurrencyDto>();
        }
    }
}
