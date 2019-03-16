using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the recipe view button to be instantiated and initialized in the main screen
/// </summary>
public class RecipeButtonView : MonoBehaviour
{
    [SerializeField] private Image recipeImage;
    [SerializeField] private Text recipeName;
    [SerializeField] private GameObject loadingPanelObject;

    private string recipeImagePath;
    private Recipe recipe;

    /// <summary>
    /// Initialize the recipe button
    /// </summary>
    /// <param name="newRecipe">Recipe whose info will be used to initialize the button</param>
    public void InitRecipeButton(Recipe newRecipe)
    {
        // turn on loading panel until the image is retrieved
        loadingPanelObject.SetActive(true);
        recipeImagePath = newRecipe.imageReferencePath;
        recipeName.text = name;
        recipe = newRecipe;

        GetSprite();

        GetComponent<Button>().onClick.AddListener(OpenRecipe);
    }

    private void GetSprite()
    {
        //recipeImage.sprite = recipeSprite;

        loadingPanelObject.SetActive(false);
    }

    /// <summary>
    /// Reference the recipe manager to open the recipe UI and initialize the components with this recipe
    /// </summary>
    public void OpenRecipe()
    {
        RecipeManagerUI.Instance.InitRecipeUI(recipe);
    }
}
