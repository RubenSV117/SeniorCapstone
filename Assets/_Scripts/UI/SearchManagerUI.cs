using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages UI for searching and sends input to the DatabaseManager instance
/// </summary>
public class SearchManagerUI : MonoBehaviour
{
    public static SearchManagerUI Instance;

    [SerializeField] private Transform recipeListTrans;
    [SerializeField] private GameObject buttonViewPrefab;

    public List<string> TagsToExlude{ get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        TagsToExlude = new List<string>();
    }

    /// <summary>
    /// Call database class and receive the list of recipes 
    /// </summary>
    /// <param name="recipeName">Name of the recipe to searched</param>
    public void SearchForRecipes(string recipeName)
    {

        string[] excludeTags = TagsToExlude.ToArray();
        DatabaseManager.Instance.elasticSearchExclude(recipeName, excludeTags);
    }

    public void RefreshRecipeList(List<Recipe> recipes)
    {
        // remove previous recipes
        if (recipeListTrans.transform.childCount > 0)
        {
            for (int i = 0; i < recipeListTrans.transform.childCount; i++)
                Destroy(recipeListTrans.transform.GetChild(i).gameObject);
        }

        // add new recipes
        foreach (var recipe in recipes)
        {
            RecipeButtonView recipeView = Instantiate(buttonViewPrefab, recipeListTrans).GetComponent<RecipeButtonView>();

            recipeView.InitRecipeButton(recipe);
        }
    }

    public void ToggleTag(string newTag)
    {
        if (!TagsToExlude.Contains(newTag))
        {
            TagsToExlude.Add(newTag);
        }

        else
        {
            TagsToExlude.Remove(newTag);
        }
    }
}
