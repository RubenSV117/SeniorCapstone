using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReGenSDK.Service.Impl
{
    class FavoriteServiceImpl : FavoriteService
    {
        public FavoriteServiceImpl(string endpoint, Func<Task<string>> authorizationProvider) : base(endpoint,
            authorizationProvider)
        {
        }

        public override async Task<List<string>> GetList()
        {
            var dictionary = await Get();
            return dictionary.Keys.ToList();
        }

        public override Task<Dictionary<string, bool>> Get()
        {
//            UnityWebRequest.Get(endpoint).SetRequestHeader("Authorization", authorizationProvider);
            throw new NotImplementedException();

        }

        public override Task Create(string recipeId)
        {
            throw new NotImplementedException();
        }

        public override Task Delete(string recipeId)
        {
            throw new NotImplementedException();
        }
    }
}