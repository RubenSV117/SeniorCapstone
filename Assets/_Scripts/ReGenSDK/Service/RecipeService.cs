﻿using System;
using System.Threading.Tasks;
using ReGenSDK.Service.Api;

namespace ReGenSDK.Service
{
    public abstract class RecipeService : AbstractService, IRecipeApi
    {
        protected RecipeService(string endpoint, Func<Task<string>> authorizationProvider) : base(endpoint, authorizationProvider)
        {
        }

        public abstract Task<Recipe> Get(string recipeId);
        public abstract Task Update(string recipeId, Recipe recipe);
        public abstract Task Delete(string recipeId);
        public abstract Task Create(Recipe recipe);
    }
}