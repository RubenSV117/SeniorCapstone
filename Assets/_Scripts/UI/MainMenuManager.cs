using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;

    [SerializeField] private Transform recipeListTrans;
    [SerializeField] private GameObject buttonViewPrefab;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        //CreateButtons();
    }

    /// <summary>
    /// Call database class and receive the list of recipes 
    /// </summary>
    /// <param name="recipeName">Name of the recipe to searched</param>
    public void SearchForRecipes(string recipeName)
    {

    }

    private void CreateButtons()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject obj = Instantiate(buttonViewPrefab, recipeListTrans);

        }
    }

    public void RefreshRecipeList(List<Recipe> recipes)
    {
        foreach (var recipe in recipes)
        {
            GameObject obj = Instantiate(buttonViewPrefab, recipeListTrans);

            RecipeButtonView recipeView = obj.GetComponent<RecipeButtonView>();
            recipeView.transform.SetParent(recipeListTrans);
            recipeView.InitRecipeButton(recipe);
        }

        // remove previous recipes
        //if (recipeListTrans.transform.childCount > 0)
        //{
        //    for (int i = 0; i < recipeListTrans.transform.childCount; i++)
        //        Destroy(recipeListTrans.transform.GetChild(i).gameObject); 
        //}

        // add new recipes
    }
}
