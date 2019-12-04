using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReGenSDK;
using ReGenSDK.Model;
using ReGenSDK.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Manages UI for searching and sends input to the DatabaseManager instance
/// </summary>
public class SearchManagerUI : MonoBehaviour
{
    public static SearchManagerUI Instance;

    [SerializeField] private Transform recipeListTrans;
    [SerializeField] private Transform recipeListTransFavorites;
    [SerializeField] private GameObject buttonViewPrefab;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject favoritesPanel;

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

        FillWithRandomSearch();
    }

    private void FillWithRandomSearch()
    {
        SearchForRecipes(RandomLetter());
    }

    private string RandomLetter()
    {
        int num = Random.Range(0, 25);
        char randomLetter = (char)('a' + num);
        return "" + randomLetter; 
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

        ReGenClient.Instance.Search.Builder().Add(recipeName).ExcludeTag(TagsToExclude)
            .IncludeTag(TagsToInclude).Execute().Success(async list =>
            {
                Debug.Log("SearchResults Received " + list);
                var tasks = new List<Task<Recipe>>();
                foreach (var result in list) {
                    Debug.Log("Retrieving recipe for result: " + result);
                    if (result.Key == null)
                    {
                        Debug.LogWarning("Search results has no key: " + result);
                        continue;
                    }
                    try
                    {
                        tasks.Add(ReGenClient.Instance.Recipes.Get(result.Key));
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        throw;
                    }

                }
                var recipes = new List<Recipe>();
                foreach (var task in tasks)
                {
                    recipes.Add(await task);
                }
                RefreshRecipeList(recipes);
            });
    }

    public void RefreshRecipeList(List<Recipe> recipes, bool favoriteSearch = false)
    {
        // remove previous recipes
        if (recipeListTrans.transform.childCount > 0 && !favoriteSearch)
        {
            Debug.Log("Adding NONFAVORITES to UI");
            for (int i = 0; i < recipeListTrans.transform.childCount; i++)
                Destroy(recipeListTrans.transform.GetChild(i).gameObject);
        }
        if (recipeListTransFavorites.transform.childCount > 0 && favoriteSearch)
        {
            Debug.Log("Adding favorites to UI");
            for (int i = 0; i < recipeListTransFavorites.transform.childCount; i++)
                Destroy(recipeListTransFavorites.transform.GetChild(i).gameObject);
        }

        // add new recipes
        foreach (var recipe in recipes)
        {
            RecipeButtonView recipeView = 
                Instantiate(buttonViewPrefab, (favoriteSearch ? recipeListTransFavorites : recipeListTrans) )
                    .GetComponent<RecipeButtonView>();

            recipeView.InitRecipeButton(recipe);
        }

        loadingPanel.SetActive(false);
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

    public void EnableFavoritesPanel() 
    {
        List<String> favorites = new List<String>();
        List<Recipe> favRecipes = new List<Recipe>();
        favoritesPanel.SetActive(true);
        ReGenClient.Instance.Favorites.Get().Success(list =>
        {
            favorites = list;
            foreach(var fav in favorites)
            {
                ReGenClient.Instance.Recipes.Get(fav).Success(newList =>
                {
                    favRecipes.Add(newList);
                    RefreshRecipeList(favRecipes, true);
                });
            }
            loadingPanel.SetActive(false); 
        });

    }

}
