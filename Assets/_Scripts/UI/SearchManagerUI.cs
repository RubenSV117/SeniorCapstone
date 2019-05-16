using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// Manages UI for searching and sends input to the DatabaseManager instance
/// </summary>
public class SearchManagerUI : MonoBehaviour
{
    public static SearchManagerUI Instance;

    [SerializeField] private Transform recipeListTrans;
    [SerializeField] private Transform recipeListTransFavorites;
    [SerializeField] private GameObject buttonViewPrefab;

    //[Header("Test variables")]
    //[SerializeField] private ToggleGroup test;
    //[SerializeField] private Toggle opt1;
    //[SerializeField] private Toggle opt2;
    //[SerializeField] private Toggle opt3;

    //public List<string> TagsToInclude { get; private set; }
    //public List<string> TagsToExclude { get; private set; }

    public HashSet<string> TagsToInclude { get; private set;}
    public HashSet<string> TagsToExclude { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        TagsToInclude = new HashSet<string>();
        TagsToExclude = new HashSet<string>();
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
        
        Debug.Log($"Performing search for {recipeName} with the following parameters:");
        Debug.Log($"'Exclude' preferences: ({string.Join(", ", TagsToExclude)})");
        Debug.Log($"'Include' preferences: ({string.Join(", ", TagsToInclude)})");

        DatabaseManager.Instance.elasticSearchExclude(recipeName, TagsToExclude.ToArray(), TagsToInclude.ToArray());
    }

    public void RefreshRecipeList(List<Recipe> recipes, bool favoriteSearch = false)
    {
        // remove previous recipes
        if (recipeListTrans.transform.childCount > 0 && !favoriteSearch)
        {
            for (int i = 0; i < recipeListTrans.transform.childCount; i++)
                Destroy(recipeListTrans.transform.GetChild(i).gameObject);
        }
        if (recipeListTransFavorites.transform.childCount > 0 && favoriteSearch)
        {
            for (int i = 0; i < recipeListTransFavorites.transform.childCount; i++)
                Destroy(recipeListTransFavorites.transform.GetChild(i).gameObject);
        }

        // add new recipes
        foreach (var recipe in recipes)
        {
            if (recipe == null)
                continue;

            RecipeButtonView recipeView = 
                Instantiate(buttonViewPrefab, (favoriteSearch ? recipeListTransFavorites : recipeListTrans) )
                    .GetComponent<RecipeButtonView>();

            recipeView.gameObject.SetActive(true);
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

    public void ActivateIncludeTag(string tag)
    {
        
            if (TagsToExclude.Contains(tag))
                TagsToExclude.Remove(tag);

            if (TagsToInclude.Add(tag))
                Debug.Log($"Included tag '{tag}'.");
            else
                Debug.Log($"Tag '{tag}' is already included.");
        
    }       

    public void ActivateExcludeTag(string tag)
    {
        
            if (TagsToInclude.Contains(tag))
                TagsToInclude.Remove(tag);

            if (TagsToExclude.Add(tag))
                Debug.Log($"Excluded tag '{tag}'.");
            else
                Debug.Log($"Tag '{tag}' is already excluded.");
        
    }

    public void ClearPreference(string tag)
    {
        
        if (TagsToInclude.Remove(tag) || TagsToExclude.Remove(tag))
        {
            Debug.Log($"Cleared preference for tag '{tag}'.");
        }
        else
        {
            Debug.Log($"Tag '{tag}' already set to 'no preference'.");
        }
        
    }
}
