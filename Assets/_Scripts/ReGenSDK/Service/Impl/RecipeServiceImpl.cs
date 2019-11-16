using System;
using System.Threading.Tasks;
using ReGenSDK.Service.Api;

namespace ReGenSDK.Service.Impl
{
    class RecipeServiceImpl : RecipeService
    {
        public RecipeServiceImpl(string endpoint, Func<Task<string>> authorizationProvider): base(endpoint, authorizationProvider)
        {
            throw new System.NotImplementedException();
        }

        public override Task<Recipe> Get(string recipeId)
        {
            throw new System.NotImplementedException();
        }

        public override Task Update(string recipeId, Recipe recipe)
        {
            throw new System.NotImplementedException();
        }

        public override Task Delete(string recipeId)
        {
            throw new System.NotImplementedException();
        }

        public override Task Create(Recipe recipe)
        {
            throw new System.NotImplementedException();
        }
    }
}