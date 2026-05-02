using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Misc.Polls.Admin.Models;
using Nop.Plugin.Misc.Polls.Domain;

namespace Nop.Plugin.Misc.Polls.Infrastructure;

/// <summary>
/// Represents mapping configuration for plugin models
/// </summary>
public class AutoMapperConfiguration : Profile, IOrderedMapperProfile
{
    #region Ctor

    public AutoMapperConfiguration()
    {
        CreateMap<PollAnswer, PollAnswerModel>();
        CreateMap<PollAnswerModel, PollAnswer>();

        CreateMap<Poll, PollModel>()
            .ForMember(model => model.AvailableLanguages, options => options.Ignore())
            .ForMember(model => model.PollAnswerSearchModel, options => options.Ignore())
            .ForMember(model => model.LanguageName, options => options.Ignore());
        CreateMap<PollModel, Poll>();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Order of this mapper implementation
    /// </summary>
    public int Order => 0;

    #endregion
}