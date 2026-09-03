using FluentMigrator;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.UpgradeTo510;

[NopUpdateMigration("2026-10-28 00:00:00", "5.10", UpdateMigrationType.Data)]
public class DataMigration : Migration
{
    private const int PAGE_SIZE = 500;
    private readonly INopDataProvider _dataProvider;

    public DataMigration(INopDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        //#8224
        var bcPageIndex = 0;
        while (true)
        {
            var comments =
                (from bc in _dataProvider.GetTable<BlogComment>()
                 join c in _dataProvider.GetTable<Customer>() on bc.CustomerId equals c.Id
                 where string.IsNullOrEmpty(c.Email)
                 select bc).Skip(bcPageIndex++ * PAGE_SIZE).Take(PAGE_SIZE).ToList();

            if (!comments.Any())
                break;

            foreach (var comment in comments)
            {
                comment.CustomerId = null;
                _dataProvider.UpdateEntity(comment);
            }
        }

        var prPageIndex = 0;
        while (true)
        {
            var productReviews =
                (from pr in _dataProvider.GetTable<ProductReview>()
                 join c in _dataProvider.GetTable<Customer>() on pr.CustomerId equals c.Id
                 where string.IsNullOrEmpty(c.Email)
                 select pr).Skip(prPageIndex++ * PAGE_SIZE).Take(PAGE_SIZE).ToList();

            if (!productReviews.Any())
                break;

            foreach (var pr in productReviews)
            {
                pr.CustomerId = null;
                _dataProvider.UpdateEntity(pr);
            }
        }

        var stPageIndex = 0;
        while (true)
        {
            var searchTerms =
                (from term in _dataProvider.GetTable<SearchTerm>()
                 join c in _dataProvider.GetTable<Customer>() on term.CustomerId equals c.Id
                 where string.IsNullOrEmpty(c.Email)
                 select term).Skip(stPageIndex++ * PAGE_SIZE).Take(PAGE_SIZE).ToList();

            if (!searchTerms.Any())
                break;

            foreach (var term in searchTerms)
            {
                term.CustomerId = null;
                _dataProvider.UpdateEntity(term);
            }
        }
    }

    /// <summary>Collects the DOWN migration expressions</summary>
    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}