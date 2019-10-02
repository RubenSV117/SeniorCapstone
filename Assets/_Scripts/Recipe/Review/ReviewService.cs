using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// This class interacts with the database for queries related to Reviews.
/// This GameObject contains no visible elements.
/// </summary>
public abstract class ReviewService
{
    private ReviewService instance;
    
    /// <summary>
    /// The field contains the ReviewManager instance that is active in the scene.
    /// </summary>
    public ReviewService Instance { get => instance; protected set => instance = value; }

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

}