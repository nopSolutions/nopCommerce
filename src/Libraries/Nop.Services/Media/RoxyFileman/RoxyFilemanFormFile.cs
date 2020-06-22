using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Media;

namespace Nop.Services.Media.RoxyFileman
{
    /// <summary>
    /// Represents the roxyFileman fake HttpRequest file
    /// </summary>
    public class RoxyFilemanFormFile : IFormFile
    {
        #region Fields

        private readonly Picture _picture;
        private readonly PictureBinary _pictureBinary;
        private readonly string _fileExtension;

        #endregion

        #region Ctor

        public RoxyFilemanFormFile(Picture picture, PictureBinary pictureBinary, string fileExtension)
        {
            _picture = picture;
            _pictureBinary = pictureBinary;
            _fileExtension = fileExtension;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Opens the request stream for reading the uploaded file.
        /// </summary>
        public Stream OpenReadStream()
        {
            return new MemoryStream(_pictureBinary.BinaryData);
        }

        /// <summary>
        /// Copies the contents of the uploaded file to the <paramref name="target" /> stream.
        /// </summary>
        /// <param name="target">The stream to copy the file contents to.</param>
        public void CopyTo(Stream target)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronously copies the contents of the uploaded file to the <paramref name="target" /> stream.
        /// </summary>
        /// <param name="target">The stream to copy the file contents to.</param>
        /// <param name="cancellationToken"></param>
        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the raw Content-Type header of the uploaded file.
        /// </summary>
        public string ContentType => _picture.MimeType;

        /// <summary>
        /// Gets the raw Content-Disposition header of the uploaded file.
        /// </summary>
        public string ContentDisposition => string.Empty;

        /// <summary>
        /// Gets the header dictionary of the uploaded file.
        /// </summary>
        public IHeaderDictionary Headers => null;

        /// <summary>
        /// Gets the file length in bytes.
        /// </summary>
        public long Length => _pictureBinary.BinaryData.Length;

        /// <summary>
        /// Gets the form field name from the Content-Disposition header.
        /// </summary>
        public string Name => string.Empty;

        /// <summary>
        /// Gets the file name from the Content-Disposition header.
        /// </summary>
        public string FileName => $"{_picture.SeoFilename}{_fileExtension}";

        #endregion
    }
}