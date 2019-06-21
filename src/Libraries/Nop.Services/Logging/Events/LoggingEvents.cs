using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Logging.Events
{
    public class LoggingEvents
    {
#pragma warning disable IDE1006 // Naming Styles
        public const int DefaultLogger = 2000;

        public const int GeoLookupGetInformation = 4000;

        public const int EventPublisherError = 5000;

        public const int ImportManagerPictureInsert = 6000;

        public const int ImportManagerDownloadFile = 6001;

        public const int LocalizationGetLocaleString = 7000;

        public const int LocalizationGetResource = 7001;

        public const int OrderProcessingPlaceOrder = 8000;

        public const int OrderProcessingProcessNextRecurringPayment = 8001;

        public const int OrderProcessingProcessNextRecurringPaymentCancel = 8002;

        public const int OrderProcessingCancelRecurringPayment = 8003;

        public const int OrderProcessingCapture = 8004;

        public const int OrderProcessingRefund = 8005;

        public const int OrderProcessingPartiallyRefund = 8006;

        public const int OrderProcessingVoid = 8007;

        public const int NotificationServiceNotify = 9000;

#pragma warning restore IDE1006 // Naming Styles
    }
}
