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

    [SerializeField] private Sprite testSprite;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        Test();
    }

    public void InitRecipe(Sprite dishSprite, string dishName, int calorieCount, int prepTimeMinutes, int starRating, List<string> ingredients, List<string> directions)
    {
        // update dish image
        dishImage.sprite = dishSprite;

        // update text elements
        dishNameText.text = dishName;
        ingredientCountText.text = ingredients.Count.ToString("N0");
        calorieCountText.text = calorieCount.ToString("N0");
        prepTimeText.text = prepTimeMinutes.ToString("N0");

        // update star rating
        for (int i = 0; i < starRatingTrans.childCount; i++)
            starRatingTrans.GetChild(i).gameObject.SetActive(false);

        for (int i = 0; i < starRating && i < starRatingTrans.childCount; i++)
            starRatingTrans.GetChild(i).gameObject.SetActive(true);

        // remove any previous Ingredients and directions
        if (verticalGroupTrans.childCount > 1)
        {
            for (int i = 1; i < verticalGroupTrans.childCount; i++)
                Destroy(verticalGroupTrans.GetChild(i).gameObject);
        }

        // update Ingredients
        for (int i = 0; i < ingredients.Count; i++)
        {
            Text infoText = Instantiate(infoPrefab, verticalGroupTrans.transform.position, infoPrefab.transform.rotation,
                verticalGroupTrans).GetComponentInChildren<Text>();

            infoText.text = ingredients[i];
        }

        // create directions label
        Text labelText = Instantiate(labelPrefab, verticalGroupTrans.transform.position, infoPrefab.transform.rotation,
            verticalGroupTrans).GetComponentInChildren<Text>();

        labelText.text = "Directions";

        // update directions
        for (int i = 0; i < directions.Count; i++)
        {
            Text infoText = Instantiate(infoPrefab, verticalGroupTrans.transform.position, infoPrefab.transform.rotation,
                verticalGroupTrans).GetComponentInChildren<Text>();

            infoText.text = directions[i];
        }
    }

    public void Enable()
    {
        canvas.SetActive(true);
    }

    public void Disable()
    {
        canvas.SetActive(false);
    }

    public void Test()
    {
        List<string> ingredients = new List<string>();

        for (int i = 0; i < 10; i++)
        {
            ingredients.Add($"ingredient item {i}");
        }

        List<string> directions = new List<string>();

        for (int i = 0; i < 10; i++)
        {
            directions.Add($"{i}. do the thing");
        }

        InitRecipe(testSprite, "Butter Salmon", 560, 45, 3, ingredients, directions);
    }
}
