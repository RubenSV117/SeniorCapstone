using System;
using System.Threading.Tasks;
using ReGenSDK.Service.Api;

namespace ReGenSDK.Service
{
    public abstract class RatingService : AbstractService, IRatingApi
    {
        protected RatingService(string endpoint, Func<Task<string>> authorizationProvider) : base(endpoint, authorizationProvider)
        {
        }

        public abstract Task<double> GetAverage(string recipeId);
        public abstract Task<int> Get(string recipeId);
        public abstract Task Create(string recipeId, int rating);
        public abstract Task Update(string recipeId, int rating);
    }
}