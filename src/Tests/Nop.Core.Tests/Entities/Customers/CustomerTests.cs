using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Common;

namespace Nop.Core.Tests.Entities.Customers
{
    [TestFixture]
    public class CustomerTests
    {
        [Test]
        public void Can_add_address()
        {
            var customer = new Customer();
            var address = new Address { Id = 1 };

            customer.AddAddress(address);

            customer.Addresses.Count.ShouldEqual(1);
            customer.Addresses.First().Id.ShouldEqual(1);
        }

        [Test]
        public void Can_not_add_duplicate_addresses()
        {
            var customer = new Customer();
            var address = new Address { Id = 1 };
            var address2 = new Address { Id = 2 };

            customer.AddAddress(address);
            customer.AddAddress(address); // should not add
            customer.AddAddress(address2);


            customer.Addresses.Count.ShouldEqual(2);
            var addresses = customer.Addresses.ToList();
            addresses[0].Id.ShouldEqual(1);
            addresses[1].Id.ShouldEqual(2);
        }

        [Test]
        public void Can_remove_address_assigned_as_billing_address()
        {
            var customer = new Customer();
            var address = new Address { Id = 1 };

            customer.AddAddress(address);
            customer.SetBillingAddress(address);

            customer.BillingAddress.ShouldBeTheSameAs(customer.Addresses.First());

            customer.RemoveAddress(address);
            customer.Addresses.Count.ShouldEqual(0);
            customer.BillingAddress.ShouldBeNull();
        }
    }
}
