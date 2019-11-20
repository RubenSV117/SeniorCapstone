using System;
using System.Threading.Tasks;
using ReGenSDK.Service.Api;

namespace ReGenSDK.Service.Impl
{
    class RatingServiceImpl : RatingService
    {
        public RatingServiceImpl(string endpoint, Func<Task<string>> authorizationProvider) : base(endpoint, authorizationProvider)
        {
        }

        public override Task<double> GetAverage(string recipeId)
        {
            return Get()
                .Path(recipeId)
                .Path("average")
                .Parse<double>()
                .Execute();
        }

        public override Task<int> Get(string recipeId)
        {
            return Get()
                .Path(recipeId)
                .RequireAuthentication()
                .Parse<int>()
                .Execute();
        }

        public override Task Create(string recipeId, int rating)
        {
            return Put()
                .Path(recipeId)
                .RequireAuthentication()
                .Body(new RatingBody
                {
                    Rating = rating
                })
                .Execute();
        }

        public override Task Update(string recipeId, int rating)
        {
            return Post()
                .Path(recipeId)
                .RequireAuthentication()
                .Body(new RatingBody
                {
                    Rating = rating
                })
                .Execute();
        }

        [Serializable]
        class RatingBody
        {
            public int Rating;
        }
    }
}