using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// This class interacts with the database for queries related to Reviews.
/// </summary>
public abstract class ReviewService
{
    /// <summary>
    /// The field contains the ReviewService implementation.
    /// </summary>
    public static ReviewService Instance { get; protected set; } = new ReviewServiceImpl();

    /// <summary>
    /// Submits a review attributed to the currently logged in user.
    /// 
    /// The timestamp of the review is set to when this method is called.
    /// 
    /// Either paramter may be nullable but not both.
    /// </summary>
    /// <param name="recipeId">The id of the recipe to attach this review to.</param>
    /// <param name="content">This parameter represents the review content.</param> 
    /// <returns>A Task that represents the async execution of this database update operation.</returns>
    public abstract Task SubmitReview(string recipeId, string content);

    /// <summary>
    /// Retrieves all reviews for the current recipe
    /// </summary>
    /// <param name="recipeId">the id of the recipe</param>
    /// <returns>A Task that represents the async execution of this query and allows access to the result</returns>
    public abstract Task<List<Review>> GetReviews(string recipeId);

}