using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages input field and sends new ingredient to the PublishingManager instance
///
/// Ruben Sanchez
/// </summary>
public class IngredientBuilderView : MonoBehaviour
{
    [SerializeField] private Button addButton;
    [SerializeField] private GameObject deleteButton;

    private int index; // index of this ingredient in the PublishingManagerList

    private Ingredient ingredient;

    private string amount;
    private string name;
    private bool hasAddedIngredient;

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
        ingredient = new Ingredient(name, amount);
    }

    public void AddNewIngredient()
    {
        // return if either field has not been populated
        if (string.IsNullOrEmpty(amount) || string.IsNullOrEmpty(name))
            return;

        // add another instance of this builder to the list
        PublishingManagerUI.Instance.AddIngredientBuilder();

        // disabled add button to expose delete button
        addButton.gameObject.SetActive(false);
    }

    public void RemoveIngredient()
    {
        Destroy(gameObject);
    }
    #endregion
}
