using UnityEngine;

/// <summary>
/// This class interacts with the database for queries related to Reviews.
/// This GameObject contains no visible elements.
/// </summary>
public abstract class ReviewService : MonoBehaviour
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
    /// <param name="content">This parameter represents the review content. If this parameter is null, no review will be submitted but a rating still may.</param>
    /// <param name="stars">This is the integer rating of the recipe submitted by the user. If this parameter is -1, no rating will be submited but a review still may.</param>
    public abstract void SubmitReview(string content, int stars);

}