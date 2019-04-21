using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the UI to publish recipes
///
/// Ruben Sanchez
/// </summary>
public class PublishingManagerUI : MonoBehaviour, IPanel
{
    public static PublishingManagerUI Instance;

    public delegate void UIEvent();
    public event UIEvent OnUIElementAdded;
    public event UIEvent OnUIElementRemoved;
    public event UIEvent OnUIRefresh;

    [SerializeField] private GameObject canvas;
    [SerializeField] private InputField nameInputField;
    [SerializeField] private InputField caloriesInputField;
    [SerializeField] private InputField prepTimeInputField;
    [SerializeField] private GameObject ingedientBuilderPrefab;
    [SerializeField] private GameObject directionBuilderPrefab;
    [SerializeField] private Transform ingedientVerticalGroupTrans;
    [SerializeField] private Transform directionVerticalGroupTrans;
    [SerializeField] private Transform tagGridroupTrans;
    [SerializeField] private Text ingredientAmount;

    private List<Ingredient> ingredients = new List<Ingredient>();
    private List<string> directions = new List<string>();
    private List<string> tags = new List<string>();

    private string recipeName;
    private int calories;
    private int minutesPrep;
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

        // update the ingredient count text
        Invoke("UpdateIngredientCount", .1f);
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
        // remove the given builder
        Destroy(obj);

        // fire event for ui element change
        OnUIElementRemoved?.Invoke();

        // update the ingredient count text, apparently theres a tiny delay before the child count is properly updated
       Invoke("UpdateIngredientCount", .1f);
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

        // close canvas and refresh its content
        Disable();
        Refresh();
    }

    #endregion

    #region Private Methods

    private void UpdateIngredientCount()
    {
        ingredientAmount.text = (ingedientVerticalGroupTrans.childCount - 1).ToString();
    }

    private void RemoveAllIngredients()
    {
        foreach (Transform child in ingedientVerticalGroupTrans)
            Destroy(child.gameObject);
    }

    private void RemoveAllDirections()
    {
        foreach (Transform child in directionVerticalGroupTrans)
            Destroy(child.gameObject);
    }

    private void ResetAllTags()
    {
        Toggle[] tagToggles = tagGridroupTrans.GetComponentsInChildren<Toggle>();

        foreach (var t in tagToggles)
            t.isOn = false;
    }

    #endregion

    #region IPanel Implementation
    public void Enable()
    {
        canvas.SetActive(true);
    }

    public void Disable()
    {
        canvas.SetActive(false);
    }

    public void Init()
    {
    }

    public void Refresh()
    {
        // refresh ingredients
        RemoveAllIngredients();
        AddIngredientBuilder();

        // refresh directions
        RemoveAllDirections();
        AddDirectiontBuilder();

        // refresh tags
        ResetAllTags();

        // refresh recipe info
        nameInputField.text = string.Empty;
        caloriesInputField.text = string.Empty;
        prepTimeInputField.text = string.Empty;

        OnUIRefresh?.Invoke();
    } 
    #endregion
}
