using Nop.Core.Domain.Localization;
using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.DTOs.Languages;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class LanguageDtoMappings
    {
        public static LanguageDto ToDto(this Language language)
        {
            return language.MapTo<Language, LanguageDto>();
        }
    }
}
