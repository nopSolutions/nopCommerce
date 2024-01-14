using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.POS.Kaching.Models.ReconciliationModels;

namespace Nop.Plugin.POS.Kaching.Services
{
    public interface IReconciliationService
    {
        Task<bool> HandleReconciliationsAsync(Dictionary<string, Reconciliation> reconciliation, string accountingFileName);
    }
}