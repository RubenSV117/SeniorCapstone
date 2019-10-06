using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;


public class RecipeManagerUI : MonoBehaviour
{
    public static Recipe thisRecipe;
    public static RecipeManagerUI Instance;
    [SerializeField] private GameObject canvas;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject labelPrefab;
    [SerializeField] private GameObject infoPrefab;

    [Header("Dish Info")]
    [SerializeField] private Image dishImage;
    [SerializeField] private Text dishNameText;
    [SerializeField] private Text ingredientCountText;
    [SerializeField] private Text calorieCountText;
    [SerializeField] private Text prepTimeText;

    [SerializeField] private Transform starRatingTrans;
    [SerializeField] private Transform verticalGroupTrans;

    [SerializeField] private GameObject loadingObject;

    [SerializeField] private GameObject rateStars;

    [SerializeField] private GameObject favoriteButton;
    [SerializeField] private GameObject unfavoriteButton;


    [SerializeField] private GameObject ingredients;
    [SerializeField] private GameObject directions;

    private Sprite currentRecipeSprite;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }


    public void SetSprite(Sprite newSprite)
    {
        currentRecipeSprite = newSprite;
    }

    public void InitRecipeUI(Recipe newRecipe)
    {
        thisRecipe = newRecipe;
        dishImage.sprite = newRecipe.ImageSprite;

        // update text elements
        dishNameText.text = newRecipe.Name;
        ingredientCountText.text = newRecipe.Ingredients.Length.ToString("N0");
        calorieCountText.text = newRecipe.Calories.ToString("N0");
        prepTimeText.text = newRecipe.PrepTimeMinutes.ToString("N0");

        // update star rating
        for (int i = 0; i < starRatingTrans.childCount; i++)
            starRatingTrans.GetChild(i).gameObject.SetActive(false);

        for (int i = 0; i < newRecipe.StarRating && i < starRatingTrans.childCount; i++)
            starRatingTrans.GetChild(i).gameObject.SetActive(true);

        //// remove any previous ingredients and directions
        //if (verticalGroupTrans.childCount > 1)
        //{
        //    for (int i = 1; i < verticalGroupTrans.childCount; i++)
        //        Destroy(verticalGroupTrans.GetChild(i).gameObject);
        //}

        //foreach (Transform child in ingredients.transform)
        //{
        //    Destroy(child.gameObject);
        //}

        //foreach (Transform child in directions.transform)
        //{
        //    Destroy(child.gameObject);
        //}

        if (ingredients.transform.childCount > 1)
        {
            for (int i = 1; i < ingredients.transform.childCount; i++)
            {
                Destroy(ingredients.transform.GetChild(i).gameObject);
            }
        }

        if (directions.transform.childCount > 1)
        {
            for (int i = 1; i < directions.transform.childCount; i++)
            {
                Destroy(directions.transform.GetChild(i).gameObject);
            }
        }

        // update ingredients
        for (int i = 0; i < newRecipe.Ingredients.Length; i++)
        {
            Text infoText = Instantiate(infoPrefab, ingredients.transform.position, infoPrefab.transform.rotation,
                ingredients.transform).GetComponentInChildren<Text>();

            infoText.text = newRecipe.Ingredients[i].ToString();
        }

        //// create directions label
        //Text labelText = Instantiate(labelPrefab, verticalGroupTrans.transform.position, infoPrefab.transform.rotation,
        //    verticalGroupTrans).GetComponentInChildren<Text>();

        //labelText.text = "Directions";

        // update directions
        for (int i = 0; i < newRecipe.Steps.Length; i++)
        {
            Text infoText = Instantiate(infoPrefab, directions.transform.position, infoPrefab.transform.rotation,
                directions.transform).GetComponentInChildren<Text>();

            infoText.text = newRecipe.Steps[i];
        }

        //// create rating prompt
        //Text ratingText = Instantiate(labelPrefab, verticalGroupTrans.transform.position, infoPrefab.transform.rotation,
        //    verticalGroupTrans).GetComponentInChildren<Text>();

        //ratingText.text = "What did you think?";

        //Instantiate(ratingPrefab, verticalGroupTrans.transform.position, ratingPrefab.transform.rotation, verticalGroupTrans);

        // if user already rated this, remember rating
        // ...



        loadingObject.SetActive(true);
        StartCoroutine(WaitForImage());

        DatabaseManager.Instance.getFavorites();
        // To-do: Call drawing methods in this class instead of creating a circular reference 
        //(RecipeManagerUI calls DatabaseManager which calls RecipeManagerUI again).
        DatabaseManager.Instance
            .GetPreviousSurveyRating(thisRecipe.Key, rating =>
            {
                DrawSurveyRating(rating);
            });
        DatabaseManager.Instance
            .GetCommunityRating(thisRecipe.Key, rating => 
            {
                DrawCommunityRating( (int)rating );
            });

        canvas.SetActive(true);
    }

    public void Enable()
    {
        canvas.SetActive(true);
    }

    public void Disable()
    {
        canvas.SetActive(false);
    }

    private IEnumerator WaitForImage()
    {
        yield return new WaitWhile(() => dishImage == null);
        loadingObject.SetActive(false);
    }

    public void SetFavorited(List<string> favorites)
    {
        if(favorites.Contains(thisRecipe.Key))
            HandleFavorite();

        else
            HandleUnfavorite();
    }



    public void HandleFavorite()
    {
        bool worked = DatabaseManager.Instance.favoriteRecipe(thisRecipe.Key);

        if (worked)
        {
            unfavoriteButton.SetActive(true);
            //NotificationManager.Instance.ShowNotification("Favorited");
        }
        else
        {
            unfavoriteButton.SetActive(false);
            //NotificationManager.Instance.ShowNotification("Failed to favorite.");
        }
    }

    public void HandleUnfavorite()
    {
        bool worked = DatabaseManager.Instance.unfavoriteRecipe(thisRecipe.Key);
        if (worked)
        {
            unfavoriteButton.SetActive(false);
            //NotificationManager.Instance.ShowNotification("Unfavoriting.");

        }
        else
        {
            //NotificationManager.Instance.ShowNotification("Failed to unfavorite.");
        }
    }

    /// <summary>
    /// Updates the rating survey UI with the number of stars tapped.
    /// </summary>
    /// <param name="ratingStar">The star tapped.</param>
    public void RateRecipe(GameObject ratingStar)
    {
        try
        {
            int rating = ratingStar.transform.GetSiblingIndex() + 1;

            // The DB method makes a circular reference to this class and runs UpdateSurveyRating()
            // to update the survey UI.
            DatabaseManager.Instance.UpdateUserRatingForRecipe(thisRecipe.Key, rating);
        }
        catch (System.Exception)
        {
        }
    }

    /// <summary>
    /// Draws the gold stars on the rating survey.
    /// </summary>
    /// <param name="rating">The number of stars to enable.</param>
    public void DrawSurveyRating(int rating)
    {
        if (rating > rateStars.transform.childCount)
            throw new UnityException($"Rating {rating} was higher than stars available ({rateStars.transform.childCount}).");

        // Clear previous rating
        foreach (Transform child in rateStars.transform)
            child.gameObject.SetActive(false);

        // Display new rating
        for (int i = 0; i < rating; i++)
        {
            var star = rateStars.transform.GetChild(i);
            star.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Draws the community star rating in the info header.
    /// </summary>
    /// <param name="rating"></param>
    public void DrawCommunityRating(int rating)
    {
        if (rating > starRatingTrans.childCount)
            throw new UnityException($"Rating {rating} was higher than stars available ({rateStars.transform.childCount}).");

        // Clear previous rating
        foreach (Transform child in starRatingTrans)
            child.gameObject.SetActive(false);

        // Display new rating
        for (int i = 0; i < rating; i++)
        {
            var star = starRatingTrans.GetChild(i);
            star.gameObject.SetActive(true);
        }
    }

    public void RetrievePreviousRating()
    {
           
    }

    public void Test()
    {
        List<Ingredient> ingredients = new List<Ingredient>();

        for (int i = 0; i < 10; i++)
        {
            ingredients.Add(new Ingredient($"Ingredient {i}", "1/2 cup"));
        }

        List<string> directions = new List<string>();

        for (int i = 0; i < 50; i++)
        {
            directions.Add($"{i}. do the thing");
        }

        List<string> tags = new List<string>() {"Fish"};

        List<string> reviews =
            new List<string>()
            {
                "pretty good",
                "pretty good",
                "pretty good",
                "pretty good",
                "pretty good",
                "pretty good",
                "pretty good",
                "pretty good"
            };

        Recipe recipe = new Recipe("Butter Salmon", "", 560, 45, tags, ingredients, directions, reviews, 3);

        InitRecipeUI(recipe);
    }
}
