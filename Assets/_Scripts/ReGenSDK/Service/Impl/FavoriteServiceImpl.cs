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

        public override Task<List<string>> Get()
        {
            return HttpGet()
                .RequireAuthentication()
                .Parse<List<string>>()
                .Execute();

        }

        public override Task Create(string recipeId)
        {
            return HttpPut()
                .Path(recipeId)
                .RequireAuthentication()
                .Execute();
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