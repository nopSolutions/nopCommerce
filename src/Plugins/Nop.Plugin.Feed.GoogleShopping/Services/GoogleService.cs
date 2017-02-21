using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nop.Core.Data;
using Nop.Plugin.Feed.GoogleShopping.Domain;

namespace Nop.Plugin.Feed.GoogleShopping.Services
{
    public partial class GoogleService : IGoogleService
    {
        #region Fields

        private readonly IRepository<GoogleProductRecord> _gpRepository;

        #endregion

        #region Ctor

        public GoogleService(IRepository<GoogleProductRecord> gpRepository)
        {
            this._gpRepository = gpRepository;
        }

        #endregion

        #region Utilities

        private string GetEmbeddedFileContent(string resourceName)
        {
            string fullResourceName = string.Format("Nop.Plugin.Feed.GoogleShopping.Files.{0}", resourceName);
            var assem = this.GetType().Assembly;
            using (var stream = assem.GetManifestResourceStream(fullResourceName))
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        #endregion

        #region Methods

        public virtual void DeleteGoogleProduct(GoogleProductRecord googleProductRecord)
        {
            if (googleProductRecord == null)
                throw new ArgumentNullException("googleProductRecord");

            _gpRepository.Delete(googleProductRecord);
        }

        public virtual IList<GoogleProductRecord> GetAll()
        {
            var query = from gp in _gpRepository.Table
                        orderby gp.Id
                        select gp;
            var records = query.ToList();
            return records;
        }

        public virtual GoogleProductRecord GetById(int googleProductRecordId)
        {
            if (googleProductRecordId == 0)
                return null;

            return _gpRepository.GetById(googleProductRecordId);
        }

        public virtual GoogleProductRecord GetByProductId(int productId)
        {
            if (productId == 0)
                return null;

            var query = from gp in _gpRepository.Table
                        where gp.ProductId == productId
                        orderby gp.Id
                        select gp;
            var record = query.FirstOrDefault();
            return record;
        }

        public virtual void InsertGoogleProductRecord(GoogleProductRecord googleProductRecord)
        {
            if (googleProductRecord == null)
                throw new ArgumentNullException("googleProductRecord");

            _gpRepository.Insert(googleProductRecord);
        }

        public virtual void UpdateGoogleProductRecord(GoogleProductRecord googleProductRecord)
        {
            if (googleProductRecord == null)
                throw new ArgumentNullException("googleProductRecord");

            _gpRepository.Update(googleProductRecord);
        }

        public virtual IList<string> GetTaxonomyList()
        {
            var fileContent = GetEmbeddedFileContent("taxonomy.txt");
            if (String.IsNullOrEmpty((fileContent)))
                return new List<string>();

            //parse the file
            var result = fileContent.Split(new [] {"\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries)

                .ToList();
            return result;
        }

        #endregion
    }
}
