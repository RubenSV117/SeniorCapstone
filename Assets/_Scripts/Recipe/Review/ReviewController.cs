using System;
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

    #endregion

    #region Public Methods

    public void DidTapSubmit()
    {
        Review review = new Review(stars.GetNumberOfActiveStars(), inputField.text, RecipeManagerUI.currentRecipe);
        gameObject.SetActive(false);
        // TODO: Send review to backend service for publishing
    }

    public void Reset()
    {
        inputField.text = "";
        stars.Reset();
    }

    #endregion
}

struct Review
{
    int starRating;
    string review;
    Recipe recipe;

    public Review(int stars, string review, Recipe recipe)
    {
        this.starRating = stars;
        this.review = review;
        this.recipe = recipe;
    }
}

