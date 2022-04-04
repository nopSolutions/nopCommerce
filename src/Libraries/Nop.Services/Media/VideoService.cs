using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Data;
using Nop.Services.Catalog;

namespace Nop.Services.Media
{
    /// <summary>
    /// Video service
    /// </summary>
    public partial class VideoService : IVideoService
    {
        #region Fields

        private readonly IRepository<ProductVideo> _productVideoRepository;
        private readonly IRepository<Video> _videoRepository;

        #endregion

        #region Ctor

        public VideoService(IRepository<ProductVideo> productVideoRepository,
            IRepository<Video> videoRepository)
        {
            _productVideoRepository = productVideoRepository;
            _videoRepository = videoRepository;
        }

        #endregion

        #region CRUD methods

        /// <summary>
        /// Gets a video
        /// </summary>
        /// <param name="videoId">Video identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video
        /// </returns>
        public virtual async Task<Video> GetVideoByIdAsync(int videoId)
        {
            return await _videoRepository.GetByIdAsync(videoId, cache => default);
        }

        /// <summary>
        /// Gets videos by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the videos
        /// </returns>
        public virtual async Task<IList<Video>> GetVideosByProductIdAsync(int productId)
        {
            if (productId == 0)
                return new List<Video>();

            var query = from v in _videoRepository.Table
                        join pv in _productVideoRepository.Table on v.Id equals pv.VideoId
                        orderby pv.DisplayOrder, pv.Id
                        where pv.ProductId == productId
                        select v;

            var videos = await query.ToListAsync();

            return videos;
        }

        /// <summary>
        /// Inserts a video
        /// </summary>
        /// <param name="video">Video</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video
        /// </returns>
        public virtual async Task<Video> InsertVideoAsync(Video video)
        {
            await _videoRepository.InsertAsync(video);
            return video;
        }

        /// <summary>
        /// Updates the video
        /// </summary>
        /// <param name="video">Video</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video
        /// </returns>
        public virtual async Task<Video> UpdateVideoAsync(Video video)
        {
            await _videoRepository.UpdateAsync(video);
            
            return video;
        }

        /// <summary>
        /// Deletes a video
        /// </summary>
        /// <param name="video">Video</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteVideoAsync(Video video)
        {
            if (video == null)
                throw new ArgumentNullException(nameof(video));

            await _videoRepository.DeleteAsync(video);
        }

        #endregion
    }
}
