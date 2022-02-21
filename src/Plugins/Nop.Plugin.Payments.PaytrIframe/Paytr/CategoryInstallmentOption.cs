using System.Collections.Generic;

namespace Nop.Plugin.Payments.PaytrIframe.Paytr
{
    public class CategoryInstallmentOption
    {
        public List<CategoryOption> CategoryOptions { get; set; }
        public List<InstallmentOption> InstallmentOptions { get; set; }
        public string CategoryInstallment { get; set; }
    }
}
