using System;
using System.Threading.Tasks;
using ReGenSDK.Model;
using UnityEngine;

namespace ReGenSDK.Service.Impl
{
    class RecipeServiceImpl : RecipeService
    {
        public RecipeServiceImpl(string endpoint, Func<Task<string>> authorizationProvider): base(endpoint, authorizationProvider)
        {
        }

        public override Task<Recipe> Get(string recipeId)
        {
            return Get()
                .Path(recipeId)
                .Parse<Recipe>()
                .Execute();
        }

        public override Task Update(string recipeId, Recipe recipe)
        {
            throw new NotImplementedException();
        }

        public override Task Delete(string recipeId)
        {
            throw new NotImplementedException();
        }

        public override Task Create(Recipe recipe)
        {
            throw new NotImplementedException();
        }
    }
}