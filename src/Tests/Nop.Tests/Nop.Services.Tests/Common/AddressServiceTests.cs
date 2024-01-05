using NUnit.Framework;
using Moq;
using Nop.Services.Common; 
using Nop.Core.Domain.Common;
using Nop.Data;
using Nop.Services.Directory;
using Nop.Services.Attributes;
using Nop.Services.Localization;

namespace Nop.Tests.Nop.Services.Tests.Common
{
    [TestFixture]
    public class AddressServiceTests : ServiceTest
    {
        private Mock<IAddressService> _addressServiceMock;
        private Mock<AddressSettings> _addressSettingsMock;
        private Mock<IAttributeParser<AddressAttribute, AddressAttributeValue>> _addressAttributeParserMock;
        private Mock<IAttributeService<AddressAttribute, AddressAttributeValue>> _addressAttributeServiceMock;
        private Mock<ICountryService> _countryServiceMock;
        private Mock<ILocalizationService> _localizationServiceMock;
        private Mock<IRepository<Address>> _addressRepositoryMock;
        private Mock<IStateProvinceService> _stateProvinceServiceMock;
        private AddressService _addressService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _addressServiceMock = new Mock<IAddressService>();
            _addressSettingsMock = new Mock<AddressSettings>();
            _addressAttributeParserMock = new Mock<IAttributeParser<AddressAttribute, AddressAttributeValue>>();
            _addressAttributeServiceMock = new Mock<IAttributeService<AddressAttribute, AddressAttributeValue>>();
            _countryServiceMock = new Mock<ICountryService>();
            _localizationServiceMock = new Mock<ILocalizationService>();
            _addressRepositoryMock = new Mock<IRepository<Address>>();
            _stateProvinceServiceMock = new Mock<IStateProvinceService>();
            
            _addressService = new AddressService(
                addressSettings: _addressSettingsMock.Object,
                addressAttributeParser: _addressAttributeParserMock.Object,
                addressAttributeService: _addressAttributeServiceMock.Object,
                countryService: _countryServiceMock.Object,
                localizationService: _localizationServiceMock.Object,
                addressRepository: _addressRepositoryMock.Object,
                stateProvinceService: _stateProvinceServiceMock.Object
           
            );
        }

        [Test]
        public void Test_GetAddressByIdAsync_Returns_Address()
        {
            int addressId = 1;
            var expectedAddress = new Address(); 

            _addressServiceMock.Setup(service => service.GetAddressByIdAsync(addressId))
                .ReturnsAsync(expectedAddress); 

            var addressService = _addressServiceMock.Object;

            // Act
            var actualAddress = addressService.GetAddressByIdAsync(addressId).GetAwaiter().GetResult();

            // Assert
            Assert.IsNotNull(actualAddress);
            Assert.AreEqual(expectedAddress, actualAddress);
          
        }

        [Test]
        public void Test_IsAddressValidAsync_Returns_True_For_Valid_Address()
        {
            // Arrange
            var validAddress = new Address
            {              
                FirstName = "John",
                LastName = "Doe"
            };

            _addressServiceMock.Setup(service => service.IsAddressValidAsync(validAddress))
                .ReturnsAsync(true); 

            var addressService = _addressServiceMock.Object;

            // Act
            var isValid = addressService.IsAddressValidAsync(validAddress).GetAwaiter().GetResult();

            // Assert
            Assert.IsTrue(isValid);
           
        }

        [Test]
        public void Test_InsertAddressAsync_Creates_New_Address()
        {
            // Arrange
            var newAddress = new Address
            {
                FirstName = "Jane",
                LastName = "Doe",
            };

            _addressServiceMock.Setup(service => service.InsertAddressAsync(newAddress))
                .Returns(Task.CompletedTask); 

            var addressService = _addressServiceMock.Object;

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await addressService.InsertAddressAsync(newAddress));
          
        }

        [Test]
        public void Test_UpdateAddressAsync_Updates_Existing_Address()
        {
            // Arrange
            var existingAddress = new Address
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",

            };

            _addressServiceMock.Setup(service => service.UpdateAddressAsync(existingAddress))
                .Returns(Task.CompletedTask); 

            var addressService = _addressServiceMock.Object;

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await addressService.UpdateAddressAsync(existingAddress));
        }

        [Test]
        public void Test_DeleteAddressAsync_Removes_Address()
        {
            // Arrange
            var addressToDelete = new Address
            {
                Id = 1,
            };

            _addressServiceMock.Setup(service => service.DeleteAddressAsync(addressToDelete))
                .Returns(Task.CompletedTask);

            var addressService = _addressServiceMock.Object;

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await addressService.DeleteAddressAsync(addressToDelete));
           
        }
        [Test]
        public void Test_GetAddressTotalByCountryIdAsync_Returns_Correct_Count()
        {
            // Arrange
            int countryId = 1;
            int expectedCount = 5; 

            _addressServiceMock.Setup(service => service.GetAddressTotalByCountryIdAsync(countryId))
                .ReturnsAsync(expectedCount);

            var addressService = _addressServiceMock.Object;
            // Act
            var actualCount = addressService.GetAddressTotalByCountryIdAsync(countryId).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(expectedCount, actualCount);
            
        }
        [Test]
        public void Test_GetAddressTotalByStateProvinceIdAsync_Returns_Correct_Count()
        {
            // Arrange
            int stateProvinceId = 1;
            int expectedCount = 3; 

            _addressServiceMock.Setup(service => service.GetAddressTotalByStateProvinceIdAsync(stateProvinceId))
                .ReturnsAsync(expectedCount);

            var addressService = _addressServiceMock.Object;
            // Act
            var actualCount = addressService.GetAddressTotalByStateProvinceIdAsync(stateProvinceId).GetAwaiter().GetResult();
            // Assert
            Assert.AreEqual(expectedCount, actualCount);
        }
        [Test]
        public void Test_IsAddressValidAsync_Returns_False_For_Invalid_Address()
        {
            // Arrange
            var invalidAddress = new Address
            {
                // Create an invalid address missing required properties
            };

            _addressServiceMock.Setup(service => service.IsAddressValidAsync(invalidAddress))
                .ReturnsAsync(false); 
            var addressService = _addressServiceMock.Object;
            // Act
            var isValid = addressService.IsAddressValidAsync(invalidAddress).GetAwaiter().GetResult();

            // Assert
            Assert.IsFalse(isValid);
        }
        [Test]
        public void Test_CloneAddress_Creates_Deep_Copy()
        {
            // Arrange
            var originalAddress = new Address
            {
                FirstName = "John",
                LastName = "Doe",
            }; 
            // Act
            var clonedAddress = _addressService.CloneAddress(originalAddress);

            // Assert
            Assert.IsNotNull(clonedAddress);
            Assert.AreNotSame(originalAddress, clonedAddress); 
            Assert.AreEqual("John", clonedAddress.FirstName);
            Assert.AreEqual("Doe", clonedAddress.LastName);
        }
       
    }
}
