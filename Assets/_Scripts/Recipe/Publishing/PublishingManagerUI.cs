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

    public delegate void UIEvent();
    public event UIEvent OnUIElementAdded;
    public event UIEvent OnUIElementRemoved;

    [SerializeField] private InputField caloriesInputField;
    [SerializeField] private InputField prepTimeInputField;
    [SerializeField] private GameObject ingedientBuilderPrefab;
    [SerializeField] private GameObject directionBuilderPrefab;
    [SerializeField] private Transform ingedientVerticalGroupTrans;
    [SerializeField] private Transform directionVerticalGroupTrans;
    [SerializeField] private Transform tagGridroupTrans;

    private List<Ingredient> ingredients = new List<Ingredient>();
    private List<string> directions = new List<string>();
    private List<string> tags = new List<string>();

    private string recipeName;
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

    public void UpdateName(string newName)
    {
        if (string.IsNullOrEmpty(newName))
            return;

        recipeName = newName;
    }

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

    public void SendRecipe()
    {
    }

    public void AddIngredientBuilder()
    {
        // make a new ingredient builder item
        Instantiate(ingedientBuilderPrefab, ingedientVerticalGroupTrans);

        // fire event for ui element change
        OnUIElementAdded?.Invoke();
    }

    public void AddDirectiontBuilder()
    {
        // make a new ingredient builder item
        Instantiate(directionBuilderPrefab, directionVerticalGroupTrans);

        // fire event for ui element change
        OnUIElementAdded?.Invoke();
    }

    public void RemoveBuilder(GameObject obj)
    {
        Destroy(obj);

        // fire event for ui element change
        OnUIElementRemoved?.Invoke();
    }

    public void BuildRecipe()
    {
        // get ingredients from list
        IngredientBuilderView[] ingredientBuilders = ingedientVerticalGroupTrans.GetComponentsInChildren<IngredientBuilderView>();

        foreach (var i in ingredientBuilders)
            if(i.GetIngredient() != null)
                ingredients.Add(i.GetIngredient());

        // get directions from list
        DirectionBuilderView[] directionBuilders = directionVerticalGroupTrans.GetComponentsInChildren<DirectionBuilderView>();

        foreach (var d in directionBuilders)
            if (!string.IsNullOrEmpty(d.GetDirection()))
                directions.Add(d.GetDirection());

        // get tags from the list
        Toggle[] tagToggles = tagGridroupTrans.GetComponentsInChildren<Toggle>();

        foreach (var t in tagToggles)
            if(t.isOn)
                tags.Add(t.GetComponentInChildren<Text>().text);
    }

    #endregion
}
