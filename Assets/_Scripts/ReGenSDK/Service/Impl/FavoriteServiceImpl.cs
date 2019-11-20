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
            throw new NotImplementedException();
            //TODO
        }

        public override Task Create(string recipeId)
        {
            return Put()
                .Path(recipeId)
                .RequireAuthentication()
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