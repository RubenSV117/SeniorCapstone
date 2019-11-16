using System.Threading.Tasks;
using JetBrains.Annotations;
using ReGenSDK.Model;

namespace ReGenSDK.Service.Api
{
    public interface IReviewApi
    {
        /// <summary>
        /// Returns a single page of reviews for a recipe.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <param name="start">The key to start at.</param>
        /// <param name="size">The number of reviews to return. (min: 1, max: 100, default: 5)</param>
        /// <returns>A Task representing the async execution of this operation</returns>
//        [Get("/{recipeId}")]
//        [Headers("Authorization: Bearer")]
        Task<ReviewsPage> GetPage([NotNull] string recipeId, [CanBeNull] string start, int size = 5);

        
        /// <summary>
        /// Returns the review for the current user. 
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <returns>A Task representing the async execution of this operation.
        /// The result will be null if the user does not have a review.</returns>
//        [Get("/{recipeId}/self")]
//        [Headers("Authorization: Bearer")]
        Task<Review> Get([NotNull] string recipeId);
        
//        /// <summary>
//        /// Returns the review for the current user.
//        /// </summary>
//        /// <param name="recipeId">The recipe ID.</param>
//        /// <returns>A Task representing the async execution of this operation</returns>
//        [Get("/{recipeId}/self")]
//        [Headers("Authorization: Bearer")]
//        Task<HttpResponse> GetHttp([NotNull] string recipeId);

        /// <summary>
        /// Adds the user's review for a recipe to ReGen.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <param name="review">A Review with Content and Rating</param>
        /// <returns>A Task representing the async execution of this operation</returns>
//        [Put("/{recipeId}")]
//        [Headers("Authorization: Bearer")]
        Task Create([NotNull] string recipeId, [NotNull] Review review);

        /// <summary>
        /// Updates the user's review for a recipe to ReGen.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <param name="review">A Review with Content and Rating</param>
        /// <returns>A Task representing the async execution of this operation</returns>
//        [Post("/{recipeId}")]
//        [Headers("Authorization: Bearer")]
        Task Update([NotNull] string recipeId, [NotNull] Review review);

        /// <summary>
        /// Deletes the user's review for a recipe in ReGen.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <returns>A Task representing the async execution of this operation</returns>
//        [Delete("/{recipeId}")]
//        [Headers("Authorization: Bearer")]
        Task Delete([NotNull] string recipeId);
    }
}