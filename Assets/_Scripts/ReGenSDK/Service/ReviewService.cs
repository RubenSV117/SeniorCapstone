using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ReGenSDK.Model;
using ReGenSDK.Service.Api;

namespace ReGenSDK.Service
{
    /// <inheritdoc />
    public abstract class ReviewService : AbstractService, IReviewApi
    {
        protected ReviewService(string endpoint, Func<Task<string>> authorizationProvider) : base(endpoint, authorizationProvider)
        {
        }

        /// <inheritdoc cref="ReGenSDK.Service.Api.IReviewApi.Create"/>
        Task Create([NotNull] string recipeId, [NotNull] string content, int rating)
        {
            if (recipeId == null) throw new ArgumentNullException(nameof(recipeId));
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (rating < 0 || rating > 5) throw new ArgumentOutOfRangeException(nameof(rating));
            return Create(recipeId, new Review
            {
                Content = content,
                Rating = rating
            });
        }

        public abstract Task<ReviewsPage> GetPage(string recipeId, string start, int size = 5);
        public abstract Task<Review> Get(string recipeId);
        public abstract Task Create(string recipeId, Review review);
        public abstract Task Update(string recipeId, Review review);
        public abstract Task Delete(string recipeId);
    }
}