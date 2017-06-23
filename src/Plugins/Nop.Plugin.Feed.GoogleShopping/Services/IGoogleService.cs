using System.Collections.Generic;
using Nop.Plugin.Feed.GoogleShopping.Domain;

namespace Nop.Plugin.Feed.GoogleShopping.Services
{
    public partial interface IGoogleService
    {
        void DeleteGoogleProduct(GoogleProductRecord googleProductRecord);

        IList<GoogleProductRecord> GetAll();

        GoogleProductRecord GetById(int googleProductRecordId);

        GoogleProductRecord GetByProductId(int productId);

        void InsertGoogleProductRecord(GoogleProductRecord googleProductRecord);

        void UpdateGoogleProductRecord(GoogleProductRecord googleProductRecord);

        IList<string> GetTaxonomyList();
    }
}