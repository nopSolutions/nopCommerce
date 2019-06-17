// Copyright [2011] [PagSeguro Internet Ltda.]
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Uol.PagSeguro
{
    internal static class PagSeguroConfiguration
    {
        private const int DEFAULT_REQUEST_TIMEOUT = 10000;

        private static Uri NotificationUri { get; } = new Uri("https://ws.pagseguro.uol.com.br/v2/transactions/notifications");

        private static Uri SandboxNotificationUri { get; } = new Uri("https://ws.sandbox.pagseguro.uol.com.br/v2/transactions/notifications");

        internal static Uri GetNotificationUri(bool isSandbox)
        {
            return isSandbox ? SandboxNotificationUri : NotificationUri;
        }

        private static Uri PaymentUri { get; } = new Uri("https://ws.pagseguro.uol.com.br/v2/checkout");

        private static Uri SandboxPaymentUri { get; } = new Uri("https://ws.sandbox.pagseguro.uol.com.br/v2/checkout");

        internal static Uri GetPaymentUri(bool isSandbox)
        {
            return isSandbox ? SandboxPaymentUri : PaymentUri;
        }

        private static Uri PaymentRedirectUri { get; } = new Uri("https://pagseguro.uol.com.br/v2/checkout/payment.html");

        private static Uri SandboxPaymentRedirectUri { get; } = new Uri("https://sandbox.pagseguro.uol.com.br/v2/checkout/payment.html");

        internal static Uri GetPaymentRedirectUri(bool isSandbox)
        {
            return isSandbox ? SandboxPaymentRedirectUri : PaymentRedirectUri;
        }

        private static Uri SearchUri { get; } = new Uri("https://ws.pagseguro.uol.com.br/v2/transactions");

        private static Uri SandboxSearchUri { get; } = new Uri("https://ws.sandbox.pagseguro.uol.com.br/v2/transactions");

        internal static Uri GetSearchUri(bool isSandbox)
        {
            return isSandbox ? SandboxSearchUri : SearchUri;
        }

        internal static int RequestTimeout => DEFAULT_REQUEST_TIMEOUT;
    }
}
