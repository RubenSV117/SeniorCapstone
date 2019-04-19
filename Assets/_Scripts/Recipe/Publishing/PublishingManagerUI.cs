using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the UI to publish recipes
///
/// Ruben Sanchez
/// </summary>
public class PublishingManagerUI : MonoBehaviour
{
    public static PublishingManagerUI Instance;

    [SerializeField] private InputField caloriesInputField;
    [SerializeField] private InputField prepTimeInputField;
    [SerializeField] private GameObject ingedientBuilderPrefab;
    [SerializeField] private Transform ingedientVerticalGroupTrans;
    [SerializeField] private NestedContentSizeFitter contentFitter;

    private List<Ingredient> ingredients = new List<Ingredient>();
    private List<string> steps = new List<string>();

    private int calories;
    private int minutesPrep;
    private string ingredientAmount;
    private string ingredientName;

    #region MonoBehaviour Callbacks
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    } 
    #endregion

    #region Public Methods
    public void UpdateCalories(string caloriesString)
    {
        // check if input is numeric
        foreach (var c in caloriesString)
        {
            // non-numeric, clear the input field
            if (!Char.IsDigit(c))
            {
                caloriesInputField.text = "";
                return;
            }

        }

        // parse the value
        calories = Convert.ToInt32(caloriesString);
    }

    public void UpdatePrepTime(string prepTimeString)
    {
        // check if input is numeric
        foreach (var c in prepTimeString)
        {
            // non-numeric, clear the input field
            if (!Char.IsDigit(c))
            {
                prepTimeInputField.text = "";
                return;
            }

        }

        // parse the value
        minutesPrep = Convert.ToInt32(prepTimeString);
    }

    public void UpdateIngredientAmount(string amount)
    {
        ingredientAmount = amount;
    }

    public void UpdateIngredientName(string name)
    {
        ingredientAmount = name;
    }

    public void SendRecipe()
    {
    }

    public void AddIngredientBuilder()
    {
        // make a new ingredient builder item
        Instantiate(ingedientBuilderPrefab, ingedientVerticalGroupTrans);
        contentFitter.Grow();
    }

    public void BuildRecipe()
    {
        IngredientBuilderView[] ingredientBuilders = ingedientVerticalGroupTrans.GetComponentsInChildren<IngredientBuilderView>();

        foreach (var i in ingredientBuilders)
        {
            if(i.GetIngredient() != null)
                ingredients.Add(i.GetIngredient());
        }
    }

    #endregion
}
