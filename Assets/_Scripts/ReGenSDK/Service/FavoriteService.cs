using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReGenSDK.Service.Api;

namespace ReGenSDK.Service
{
    public abstract class FavoriteService : AbstractService, IFavoriteApi
    {
        protected FavoriteService(string endpoint, Func<Task<string>> authorizationProvider) : base(endpoint, authorizationProvider)
        {
        }

        public abstract Task<List<string>> GetList();
        public abstract Task<Dictionary<string, bool>> Get();
        public abstract Task Create(string recipeId);
        public abstract Task Delete(string recipeId);
    }
}