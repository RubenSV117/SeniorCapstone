using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ReGenSDK.Model;

namespace ReGenSDK.Service.Impl
{
    internal class ReviewServiceImpl : ReviewService
    {
        public ReviewServiceImpl(string endpoint, Func<Task<string>> authorizationProvider) : base(endpoint,
            authorizationProvider)
        {
        }


        public override Task<ReviewsPage> GetPage(string recipeId, string start = "", int size = 5)
        {
            if (recipeId == null) throw new ArgumentNullException(nameof(recipeId));
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
            return HttpGet()
                .Path(recipeId)
                .Query("start", start)
                .Query("size", size.ToString())
                .Parse<ReviewsPage>()
                .Execute();
        }

        public override Task<Review> Get(string recipeId)
        {
            return HttpGet()
                .Path(recipeId)
                .Path("self")
                .RequireAuthentication()
                .Parse<Review>()
                .Execute();
        }

        public override Task Create(string recipeId, Review review)
        {
            ValidateReview(review.Content, review.Rating);
            return HttpPut()
                .Path(recipeId)
                .RequireAuthentication()
                .Body(review)
                .Execute();
        }

        public override Task Update(string recipeId, Review review)
        {
            ValidateReview(review.Content, review.Rating);
            return HttpPost()
                .Path(recipeId)
                .RequireAuthentication()
                .Body(review)
                .Execute();
        }

        private void ValidateReview([NotNull] string reviewContent, int reviewRating)
        {
            if (reviewContent == null) throw new ArgumentNullException(nameof(reviewContent));
            if (reviewRating <= 0 || reviewRating > 5) throw new ArgumentOutOfRangeException(nameof(reviewRating));
        }

        public override Task Delete(string recipeId)
        {
            return HttpDelete()
                .Path(recipeId)
                .RequireAuthentication()
                .Execute();
        }
    }
}