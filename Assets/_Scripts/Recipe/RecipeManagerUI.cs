using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class RecipeManagerUI : MonoBehaviour
{
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
        dishImage.sprite = newRecipe.ImageSprite;

        // update text elements
        dishNameText.text = newRecipe.Name;
        ingredientCountText.text = newRecipe.Ingredients.Count.ToString("N0");
        calorieCountText.text = newRecipe.Calories.ToString("N0");
        prepTimeText.text = newRecipe.PrepTimeMinutes.ToString("N0");

        // update star rating
        for (int i = 0; i < starRatingTrans.childCount; i++)
            starRatingTrans.GetChild(i).gameObject.SetActive(false);

        for (int i = 0; i < newRecipe.StarRating && i < starRatingTrans.childCount; i++)
            starRatingTrans.GetChild(i).gameObject.SetActive(true);

        // remove any previous ingredients and directions
        if (verticalGroupTrans.childCount > 1)
        {
            for (int i = 1; i < verticalGroupTrans.childCount; i++)
                Destroy(verticalGroupTrans.GetChild(i).gameObject);
        }

        // update ingredients
        for (int i = 0; i < newRecipe.Ingredients.Count; i++)
        {
            Text infoText = Instantiate(infoPrefab, verticalGroupTrans.transform.position, infoPrefab.transform.rotation,
                verticalGroupTrans).GetComponentInChildren<Text>();

            infoText.text = newRecipe.Ingredients[i].ToString();
        }

        // create directions label
        Text labelText = Instantiate(labelPrefab, verticalGroupTrans.transform.position, infoPrefab.transform.rotation,
            verticalGroupTrans).GetComponentInChildren<Text>();

        labelText.text = "Directions";

        // update directions
        for (int i = 0; i < newRecipe.Steps.Count; i++)
        {
            Text infoText = Instantiate(infoPrefab, verticalGroupTrans.transform.position, infoPrefab.transform.rotation,
                verticalGroupTrans).GetComponentInChildren<Text>();

            infoText.text = newRecipe.Steps[i];
        }

        loadingObject.SetActive(true);
        StartCoroutine(WaitForImage());

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

    public void Test()
    {
        List<Ingredient> ingredients = new List<Ingredient>();

        for (int i = 0; i < 10; i++)
        {
            ingredients.Add(new Ingredient($"Ingredient {i}", "1/2 cup"));
        }

        List<string> directions = new List<string>();

        for (int i = 0; i < 10; i++)
        {
            directions.Add($"{i}. do the thing");
        }

        List<string> tags = new List<string>() {"Fish"};

        Recipe recipe = new Recipe("Butter Salmon", "", 560, 45, tags, ingredients, directions, null, 3);

        InitRecipeUI(recipe);
    }
}
