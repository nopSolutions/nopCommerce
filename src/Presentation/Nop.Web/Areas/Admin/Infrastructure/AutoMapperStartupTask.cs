using AutoMapper;
using Nop.Core.Domain.Books;
using Nop.Web.Areas.Admin.Models.Books;

namespace Nop.Web.Areas.Admin.Infrastructure;

public class AutoMapperStartupTask : Profile
{
    public AutoMapperStartupTask()
    {
        CreateMap<Book, BookModel>()
            .ForMember(entity => entity.Name, mo => mo.MapFrom(src => src.Name))
                .ForMember(entity => entity.CreatedOn, mo => mo.MapFrom(src => src.CreatedOn));

        CreateMap<BookModel, Book>()
            .ForMember(entity => entity.Name, mo => mo.MapFrom(src => src.Name))
                .ForMember(entity => entity.CreatedOn, mo => mo.Ignore());
    }
}
