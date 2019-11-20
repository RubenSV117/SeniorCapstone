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


        public override Task<ReviewsPage> GetPage(string recipeId, string start, int size = 5)
        {
            if (recipeId == null) throw new ArgumentNullException(nameof(recipeId));
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
            return Get()
                .Path(recipeId)
                .Query("start", start)
                .Query("size", size.ToString())
                .Parse<ReviewsPage>()
                .Execute();
        }

        public override Task<Review> Get(string recipeId)
        {
            return Get()
                .Path(recipeId)
                .Path("self")
                .RequireAuthentication()
                .Parse<Review>()
                .Execute();
        }

        public override Task Create(string recipeId, Review review)
        {
            return Put()
                .Path(recipeId)
                .RequireAuthentication()
                .Body(review)
                .Execute();
        }

        public override Task Update(string recipeId, Review review)
        {
            return Post()
                .Path(recipeId)
                .RequireAuthentication()
                .Body(review)
                .Execute();
        }

        public override Task Delete(string recipeId)
        {
            return Delete()
                .Path(recipeId)
                .RequireAuthentication()
                .Execute();
        }
    }
}