using LinqToDB;
using LinqToDB.Data;
using Nop.Data;
using System.Data;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcCore.Delivery
{
    public class DeliveryService : IDeliveryService
    {
        private readonly INopDataProvider _nopDataProvider;

        public DeliveryService(
            INopDataProvider nopDataProvider
        ) {
            _nopDataProvider = nopDataProvider;
        }

        public async Task<bool> CheckZipcodeAsync(string zip)
        {
            var returnCode = new DataParameter { Name = "ReturnCode", DataType = DataType.Int32, Direction = ParameterDirection.Output };
            var parameters = new DataParameter[] { returnCode, new DataParameter { Name = "zip", DataType = DataType.Int32, Value = zip } };
            await _nopDataProvider.ExecuteNonQueryAsync("EXEC @ReturnCode = ZipIsHomeDelivery @zip", dataParameters: parameters);

            return returnCode.Value.Equals(1);
        }
    }
}
