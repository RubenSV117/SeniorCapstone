using System;
using System.Threading.Tasks;
using ReGenSDK.Service.Api;

namespace ReGenSDK.Service.Impl
{
    class RatingServiceImpl : RatingService
    {
        public RatingServiceImpl(string endpoint, Func<Task<string>> authorizationProvider) : base(endpoint, authorizationProvider)
        {
            throw new NotImplementedException();
        }

        public override Task<double> GetAverage(string recipeId)
        {
            throw new NotImplementedException();
        }

        public override Task<int> Get(string recipeId)
        {
            throw new System.NotImplementedException();
        }

        public override Task Create(string recipeId, int rating)
        {
            throw new System.NotImplementedException();
        }

        public override Task Update(string recipeId, int rating)
        {
            throw new System.NotImplementedException();
        }
    }
}