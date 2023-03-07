using FluentAssertions;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Services.Security;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Security
{
    [TestFixture]
    public class PermissionServiceTests : ServiceTest
    {
        private IPermissionService _permissionService;
        private IRepository<PermissionRecord> _permissionRecordRepository;

        [OneTimeSetUp]
        public void SetUp()
        {
            _permissionService = GetService<IPermissionService>();
            _permissionRecordRepository = GetService<IRepository<PermissionRecord>>();
        }

        [Test]
        public async Task TestCrud()
        {
            var insertItem = new PermissionRecord
            {
                Name = "Test name",
                SystemName = "Test system name",
                Category = "Test category"
            };

            var updateItem = new PermissionRecord
            {
                Name = "Test name 1",
                SystemName = "Test system name",
                Category = "Test category"
            };

            await TestCrud(insertItem, _permissionService.InsertPermissionRecordAsync, updateItem, _permissionService.UpdatePermissionRecordAsync, _permissionService.GetPermissionRecordByIdAsync, (item, other) => item.Name.Equals(other.Name), _permissionService.DeletePermissionRecordAsync);
        }

        [Test]
        public async Task CanInstalUninstallPermissions()
        {
            async Task<IList<PermissionRecord>> getRecordsAsync()
            {
                return await _permissionRecordRepository.GetAllAsync(query =>
                    query.Where(p => p.SystemName.Equals("Test permission record system name")));
            }

            var records = await getRecordsAsync();

            records.Count.Should().Be(0);

            await _permissionService.InstallPermissionsAsync(new TestPermissionProvider());
            records = await getRecordsAsync();
            records.Count.Should().Be(1);
            await _permissionService.UninstallPermissionsAsync(new TestPermissionProvider());
            records = await getRecordsAsync();
            records.Count.Should().Be(0);
        }

        public class TestPermissionProvider : IPermissionProvider
        {
            private readonly PermissionRecord _permissionRecord = new()
            {
                Name = "Test name",
                SystemName = "Test permission record system name",
                Category = "Test category"
            };

            public IEnumerable<PermissionRecord> GetPermissions()
            {
                return new[] { _permissionRecord };
            }

            public HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
            {
                return new()
                {
                    (
                        NopCustomerDefaults.AdministratorsRoleName,
                        new[] {_permissionRecord}
                    )
                };
            }
        }
    }
}
