using System;

namespace Nop.Services.Orders.CustomExceptions
{
    public class InvalidOrderShippingStatusException : Exception
    {
        public InvalidOrderShippingStatusException() { }

        public InvalidOrderShippingStatusException(string message)
        : base(message) { }
    }
}
