using System;
using System.Collections.Generic;
using ReGenSDK;
using ReGenSDK.Model;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages recipe reviews
///
/// Ruben Sanchez
/// </summary>
public class ReviewController : MonoBehaviour
{

    #region Properties

    public Recipe recipe
    {
        set => recipeNameText.text = value.Name;
    }

    [SerializeField] private StarController stars;
    [SerializeField] private InputField inputField;
    [SerializeField] private Text recipeNameText;
    [SerializeField] private GameObject deleteButton;

    #endregion

    private bool hasExistingReview = false;
    public List<Review> reviewList = new List<Review>();
    #region Public Methods

    public void DidTapSubmit()
    {
        var starCount = stars.GetNumberOfActiveStars();
        Debug.Log("Stars: " + starCount);
        if (hasExistingReview)
        {
            ReGenClient.Instance.Reviews.Update(RecipeManagerUI.currentRecipe.Key, new Review
            {
                Content = inputField.text,
                Rating = starCount
            });            
        }
        else
        {
            ReGenClient.Instance.Reviews.Create(RecipeManagerUI.currentRecipe.Key, new Review
            {
                Content = inputField.text,
                Rating = starCount
            });
        }

        gameObject.SetActive(false);
    }

    public void PrefilReview(Review review)
    {
        Debug.Log($"Prefilled stars: {review.Rating}");
        hasExistingReview = true;
        stars.SetStarValue(review.Rating);
        inputField.text = review.Content;
    }


    public void Reset()
    {
        inputField.text = "";
        stars.Reset();
    }

    #endregion
    public void getReviews(string recipeID, Action callback)
    {
       
    }
    
    public void SetDeleteButton(bool isActive)
    {
        deleteButton.SetActive(isActive);
    }

    public void DeleteReview()
    {
        // TODO: wire to database call
    }

}



