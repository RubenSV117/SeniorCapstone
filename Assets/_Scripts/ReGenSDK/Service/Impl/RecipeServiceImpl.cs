using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.Core.Internal;
using JetBrains.Annotations;
using ReGenSDK.Model;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace ReGenSDK.Service.Impl
{
    class RecipeServiceImpl : RecipeService
    {
        public RecipeServiceImpl(string endpoint, Func<Task<string>> authorizationProvider): base(endpoint, authorizationProvider)
        {
        }

        public override Task<Recipe> Get(string recipeId)
        {
            return HttpGet()
                .Path(recipeId)
                .Parse<Recipe>()
                .Execute();
        }

        public override Task Update(string recipeId, Recipe recipe)
        {
            ValidateRecipe(recipe.Name, recipe.Ingredients, recipe.Steps,
                recipe.Tags);
            return HttpPost()
                .Path(recipeId)
                .RequireAuthentication()
                .Body(recipe)
                .Execute();

        }

        public override Task Delete(string recipeId)
        {
            return HttpDelete()
                .Path(recipeId)
                .RequireAuthentication()
                .Execute();
        }

        public override Task<Recipe> Create(Recipe recipe)
        {
            ValidateRecipe(recipe.Name, recipe.Ingredients, recipe.Steps,
                recipe.Tags);
            return HttpPut()
                .RequireAuthentication()
                .Body(recipe)
                .Parse<Recipe>()
                .Execute();
        }

        private void ValidateRecipe([NotNull] string recipeName,
            [NotNull] List<Ingredient> recipeIngredients, [NotNull] List<string> recipeSteps, [NotNull] List<string> recipeTags)
        {
            if (recipeName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(recipeName));
            if (recipeIngredients == null) throw new ArgumentNullException(nameof(recipeIngredients));
            if (recipeSteps == null) throw new ArgumentNullException(nameof(recipeSteps));
            if (recipeTags == null) throw new ArgumentNullException(nameof(recipeTags));
        }
    }
}