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
        while (true)
        {
            var comments = _dataProvider.GetTable<BlogComment>()
                .Join(_dataProvider.GetTable<Customer>(), bc => bc.CustomerId, c => c.Id, (bc, c) => new { bc, c })
                .Where(t => string.IsNullOrEmpty(t.c.Email))
                .Select(t => t.bc).Take(PAGE_SIZE).ToList();

            if (!comments.Any())
                break;

            foreach (var comment in comments) 
                comment.CustomerId = null;

            _dataProvider.UpdateEntities(comments);
        }

        while (true)
        {
            var productReviews = _dataProvider.GetTable<ProductReview>()
                .Join(_dataProvider.GetTable<Customer>(), pr => pr.CustomerId, c => c.Id, (pr, c) => new { pr, c })
                .Where(t => string.IsNullOrEmpty(t.c.Email))
                .Select(t => t.pr).Take(PAGE_SIZE).ToList();

            if (!productReviews.Any())
                break;

            foreach (var pr in productReviews) 
                pr.CustomerId = null;

            _dataProvider.UpdateEntities(productReviews);
        }

        while (true)
        {
            var searchTerms = _dataProvider.GetTable<SearchTerm>()
                .Join(_dataProvider.GetTable<Customer>(), term => term.CustomerId, c => c.Id,
                    (term, c) => new { term, c })
                .Where(t => string.IsNullOrEmpty(t.c.Email))
                .Select(t => t.term).Take(PAGE_SIZE).ToList();

            if (!searchTerms.Any())
                break;

            foreach (var term in searchTerms) 
                term.CustomerId = null;

            _dataProvider.UpdateEntities(searchTerms);
        }
    }

    /// <summary>Collects the DOWN migration expressions</summary>
    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}