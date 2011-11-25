using System;
using System.Collections.Generic;
using FluentValidation.Attributes;

namespace Nop.Web.MVC.Tests.Public.Validators
{
    [Validator(typeof(TestValidator))]
    public class Person
    {
        public string NameField;
        public int Id { get; set; }
        public string Surname { get; set; }
        public string Forename { get; set; }

        public List<Person> Children { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int? NullableInt { get; set; }

        public Person()
        {
            Children = new List<Person>();
            Orders = new List<Order>();
        }

        public int CalculateSalary()
        {
            return 20;
        }

        public Address Address { get; set; }
        public IList<Order> Orders { get; set; }

        public string Email { get; set; }
        public decimal Discount { get; set; }
        public double Age { get; set; }

        public int AnotherInt { get; set; }

        public string CreditCard { get; set; }
    }


    public class Address
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public Country Country { get; set; }
    }

    public class Country
    {
        public string Name { get; set; }
    }

    public class Order
    {
        public string ProductName { get; set; }
        public decimal Amount { get; set; }
    }
}
