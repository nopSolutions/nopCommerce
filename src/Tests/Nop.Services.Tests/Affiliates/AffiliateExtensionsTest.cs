using NUnit.Framework;
using Nop.Services.Affiliates;
using System;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Common;
using Nop.Tests;

namespace Nop.Services.Tests.Affiliates
{
    [TestFixture]
    public class AffiliateExtensionsTest : ServiceTest
    {
        [Test]
        public void Test_GetFullName_WhenLastNameIsEmpty_ShouldReturnOnlyFirstName()
        {
            const string firstName = "John";

            // arrange
            Affiliate affiliate = new Affiliate();
            affiliate.Address = new Address() { FirstName = firstName, LastName = String.Empty };

            // act
            string fullName = affiliate.GetFullName();

            // assert
            fullName.ShouldEqual(firstName);
        }

        [Test]
        public void Test_GetFullName_WhenFirstNameIsEmpty_ShouldReturnOnlyLastName()
        {
            // arrange
            Affiliate affiliate = new Affiliate();
            affiliate.Address = new Address() { FirstName = String.Empty, LastName = "Doe" };

            // act
            string fullName = affiliate.GetFullName();

            // assert
            fullName.ShouldEqual("Doe");
        }

        [Test]
        public void Test_GetFullName_WhenFirstAndLastNameAreSet_ShouldReturnBothNamesSeparatedBySpace()
        {
            // arrange
            Affiliate affiliate = new Affiliate();
            affiliate.Address = new Address() { FirstName = "John", LastName = "Doe" };

            // act
            string fullName = affiliate.GetFullName();

            // assert
            fullName.ShouldEqual("John Doe");
        }
    }
}
