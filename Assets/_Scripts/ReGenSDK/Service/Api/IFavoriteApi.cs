using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ReGenSDK.Service.Api
{
    public interface IFavoriteApi
    {
        /// <summary>
        /// Returns a collection of the user's favorite recipes.
        /// </summary>
        /// <returns>A dictionary whose keys are the recipeId and whose value is <c>true</c></returns>
        /// <remarks>[Requires authentication]</remarks>
//        [Get("")]
//        [Headers("Authorization: Bearer")]
        Task<Dictionary<string, bool>> Get();

        /// <summary>
        /// Adds a recipe to the user's favorites.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <returns>A Task representing the async execution of this operation</returns>
        /// <remarks>[Requires authentication]</remarks>
//        [Put("/{recipeId}")]
//        [Headers("Authorization: Bearer")]
        Task Create([NotNull] string recipeId);

        /// <summary>
        /// Deletes a recipe from the user's favorites.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <returns>A Task representing the async execution of this operation</returns>
        /// <remarks>[Requires authentication]</remarks>
//        [Delete("/{recipeId}")]
//        [Headers("Authorization: Bearer")]
        Task Delete([NotNull] string recipeId);
    }
}