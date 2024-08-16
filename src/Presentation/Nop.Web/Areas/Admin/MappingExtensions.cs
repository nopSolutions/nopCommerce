using AutoMapper;
using Nop.Core;
using Nop.Core.Domain.Books;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure.Mapper;
using Nop.Web.Areas.Admin.Infrastructure;
using Nop.Web.Areas.Admin.Models.Books;
using Nop.Web.Framework.Models;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Nop.Web.Areas.Admin;

public static class MappingExtensions
{
    public static BookModel ToModel(this Book entity)
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Book, BookModel>();
        });
        IMapper iMapper = config.CreateMapper();
        return iMapper.Map<Book, BookModel>(entity);
    }

    public static Book ToEntity(this BookModel model)
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Book, BookModel>();
            cfg.CreateMap<BookModel, Book>();
        });
        IMapper iMapper = config.CreateMapper();
        return iMapper.Map<BookModel, Book>(model);
    }
}
