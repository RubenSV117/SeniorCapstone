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

    public List<string> TagsToInclude{ get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        TagsToInclude = new List<string>()
        {
        };

    }

    /// <summary>
    /// Call database class and receive the list of recipes 
    /// </summary>
    /// <param name="recipeName">Name of the recipe to searched</param>
    public void SearchForRecipes(string recipeName)
    {
        if (string.IsNullOrWhiteSpace(recipeName))
        {
            return;
        }
<<<<<<< HEAD
        string[] excludeTags = TagsToExlude.ToArray();
        Debug.Log($"Searching {recipeName} with {TagsToExlude.Count} tags to exclude...");

        //DatabaseManager.Instance.Search(recipeName);
<<<<<<< HEAD
        DatabaseManager.Instance.elasticSearchExclude(recipeName, excludeTags);
=======
        DatabaseManager.Instance.elasticSearchExclude(recipeName, TagsToExlude.ToArray());
=======

        Debug.Log($"Searching {recipeName} with {TagsToInclude.Count} tags to exclude...");

        //DatabaseManager.Instance.Search(recipeName);
        DatabaseManager.Instance.elasticSearchExclude(recipeName, TagsToInclude.ToArray());
>>>>>>> z
    }

    public void SearchForRecipesSimple(string recipeName)
    {
        // DatabaseManager.Instance.Search(recipeName);
>>>>>>> fixed typo in code
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
            RecipeButtonView recipeView = 
                Instantiate(buttonViewPrefab, recipeListTrans)
                    .GetComponent<RecipeButtonView>();

            recipeView.InitRecipeButton(recipe);
        }
    }

    public void ToggleTag(string newTag)
    {
        if (!TagsToInclude.Contains(newTag))
        {
            TagsToInclude.Add(newTag);
        }
        else if (TagsToInclude.Contains(newTag))
        {
            TagsToInclude.Remove(newTag);
        }
    }
}
