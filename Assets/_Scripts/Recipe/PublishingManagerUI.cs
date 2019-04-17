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
    [SerializeField] private GameObject infoPrefab;
    [SerializeField] private InputField caloriesInputField;
    [SerializeField] private InputField prepTimeInputField;

    private List<Ingredient> ingredients;
    private List<string> steps;

    private int calories;
    private int minutesPrep;
    private string ingredientAmount;
    private string ingredientName;

    public void UpdateCalories(string caloriesString)
    {
        // check if input is numeric
        foreach (var c in caloriesString)
        {
            // non-numeric, clear the input field
            if(!Char.IsDigit(c))
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
}
