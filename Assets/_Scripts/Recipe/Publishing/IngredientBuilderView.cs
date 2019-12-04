using ReGenSDK.Model;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages ingredient building for PublishingManager
///
/// Ruben Sanchez
/// </summary>
public class IngredientBuilderView : MonoBehaviour
{
    #region Private Fields

    [SerializeField] private GameObject addButton;

    private int index; // index of this ingredient in the PublishingManagerList

    private Ingredient ingredient;

    private string amount;
    private new string name;
    private bool hasAddedIngredient; 

    #endregion

    #region Public Methods

    public Ingredient GetIngredient()
    {
        return ingredient;
    }

    public void UpdateAmount(string newAmount)
    {
        amount = newAmount;
        BuildIngredient();
    }

    public void UpdateName(string newName)
    {
        name = newName;
        BuildIngredient();
    }

    public void BuildIngredient()
    {
        // update the ingredient
        ingredient = new Ingredient
        {
            IngredientName = name,
            IngredientAmount = amount
        };

        // if ingredient is complete, auto add another builder
        if (!string.IsNullOrEmpty(amount) && !string.IsNullOrEmpty(name) && !hasAddedIngredient)
        {
            hasAddedIngredient = true;
            AddNewIngredient();
        }
    }

    public void AddNewIngredient()
    {
        // return if either field has not been populated
        if (string.IsNullOrEmpty(amount) || string.IsNullOrEmpty(name))
            return;

        // add another instance of this builder to the list
        PublishingManagerUI.Instance.AddIngredientBuilder();

        // disabled add button to expose delete button
        addButton.SetActive(false);
    }

    public void RemoveIngredient()
    {
        PublishingManagerUI.Instance.RemoveBuilder(gameObject);
    }
    #endregion
}
