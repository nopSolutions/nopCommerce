using AwesomeAssertions;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Customers;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Common;

[TestFixture]
public class SearchTermServiceTests : ServiceTest
{
    private const int STORE_ID = 1;

    private ISearchTermService _searchTermService;
    private IRepository<SearchTerm> _searchTermRepository;
    private ICustomerService _customerService;
    private Customer _customer;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        _searchTermService = GetService<ISearchTermService>();
        _searchTermRepository = GetService<IRepository<SearchTerm>>();
        _customerService = GetService<ICustomerService>();

        _customer = new Customer
        {
            CustomerGuid = Guid.NewGuid(),
            CreatedOnUtc = DateTime.UtcNow,
            LastActivityDateUtc = DateTime.UtcNow
        };
        await _customerService.InsertCustomerAsync(_customer);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _customerService.DeleteCustomerAsync(_customer);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _searchTermRepository.DeleteAsync(term => term.CustomerId == _customer.Id);
    }

    private async Task InsertSearchTermAsync(string keyword)
    {
        await _searchTermService.InsertSearchTermAsync(new SearchTerm
        {
            Keyword = keyword,
            CustomerId = _customer.Id,
            StoreId = STORE_ID,
            CreatedOnUtc = DateTime.UtcNow
        });
    }

    [Test]
    public async Task DeleteSearchTermsByKeywordShouldDeleteAllRecordsOfKeyword()
    {
        //the same keyword is searched several times, so several records are stored
        await InsertSearchTermAsync("shoes");
        await InsertSearchTermAsync("shoes");
        await InsertSearchTermAsync("shoes");
        await InsertSearchTermAsync("shirt");

        await _searchTermService.DeleteSearchTermsByKeywordAsync("shoes", _customer.Id, STORE_ID);

        var history = await _searchTermService.SearchTermHistoryItemsAsync(_customer.Id, STORE_ID);
        history.Should().BeEquivalentTo("shirt");

        var terms = await _searchTermRepository.GetAllAsync(query => query.Where(term => term.CustomerId == _customer.Id && term.Keyword == "shoes"));
        terms.Should().HaveCount(3).And.OnlyContain(term => term.Deleted);
    }
}
