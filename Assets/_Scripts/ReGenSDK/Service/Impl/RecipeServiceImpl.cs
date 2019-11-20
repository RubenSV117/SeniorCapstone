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
            return Post()
                .Path(recipeId)
                .RequireAuthentication()
                .Body(recipe)
                .Execute();

        }

        public override Task Delete(string recipeId)
        {
            return Delete()
                .Path(recipeId)
                .RequireAuthentication()
                .Execute();
        }

        public override Task Create(Recipe recipe)
        {
            return Put()
                .RequireAuthentication()
                .Body(recipe)
                .Execute();
        }
    }
}