using System.Collections.Generic;
using Nop.Plugin.Feed.Froogle.Domain;

namespace Nop.Plugin.Feed.Froogle.Services
{
    public partial interface IGoogleService
    {
        void DeleteGoogleProduct(GoogleProductRecord googleProductRecord);

        IList<GoogleProductRecord> GetAll();

        GoogleProductRecord GetById(int googleProductRecordId);

        GoogleProductRecord GetByProductVariantId(int productVariantId);

        void InsertGoogleProductRecord(GoogleProductRecord googleProductRecord);

        void UpdateGoogleProductRecord(GoogleProductRecord googleProductRecord);

        IList<string> GetTaxonomyList();
    }
}
