using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Transform recipeListTrans;
    [SerializeField] private GameObject buttonViewPrefab;

    /// <summary>
    /// Call database classand receive the list of recipes 
    /// </summary>
    /// <param name="recipeName">Name of the recipe to searched</param>
    public void SearchForRecipes(string recipeName)
    {

    }

    public void RefreshRecipeList(List<Recipe> recipes)
    {
        // remove previous recipes
        for (int i = 0; i < recipeListTrans.childCount; i++)
            Destroy(recipeListTrans.GetChild(i).gameObject);

        // add new recipes
        foreach (var recipe in recipes)
        {
            RecipeButtonView recipeView = Instantiate(buttonViewPrefab, recipeListTrans).GetComponent<RecipeButtonView>();
            recipeView.InitRecipeButton(recipe);
        }
    }
}
