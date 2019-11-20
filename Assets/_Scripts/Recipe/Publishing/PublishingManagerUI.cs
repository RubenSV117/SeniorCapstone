using System;
using System.Collections.Generic;
using Firebase.Storage;
using ReGenSDK;
using ReGenSDK.Model;
using ReGenSDK.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the UI to publish recipes
///
/// Ruben Sanchez
/// </summary>
public class PublishingManagerUI : MonoBehaviour, IPanel
{
    #region Public Fields
    public static PublishingManagerUI Instance;

    public delegate void UIEvent();
    public static event UIEvent OnUIElementAdded;
    public static event UIEvent OnUIElementRemoved;
    public static event UIEvent OnUIRefresh; 
    #endregion

    #region Private Fields
    [SerializeField] private GameObject canvas;
    [SerializeField] private int startOrder = -1;
    [SerializeField] private int finalOrder = 0;

    [SerializeField] private Image recipeImage;
    [SerializeField] private InputField nameInputField;
    [SerializeField] private InputField caloriesInputField;
    [SerializeField] private InputField prepTimeInputField;
    [SerializeField] private GameObject ingedientBuilderPrefab;
    [SerializeField] private GameObject directionBuilderPrefab;
    [SerializeField] private Transform ingedientVerticalGroupTrans;
    [SerializeField] private Transform directionVerticalGroupTrans;
    [SerializeField] private Transform tagGridroupTrans;
    [SerializeField] private Text ingredientAmount;
    [SerializeField] private GameObject largeCameraButtons;
    [SerializeField] private GameObject smallCameraButtons;

    private List<Ingredient> ingredients = new List<Ingredient>();
    private List<string> directions = new List<string>();
    private List<string> tags = new List<string>();
    private List<Review> reviews = null;
    private int starRating = 0;

    private string recipeName;
    private int calories;
    private int minutesPrep; 
    #endregion

    #region MonoBehaviour Callbacks
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        canvas.GetComponent<Canvas>().sortingOrder = startOrder;
    }

    private void OnEnable()
    {
        // subscribe to camera events to update the recipe image
        CameraManager.OnPictureTaken += UpdateRecipeImage;
        CameraManager.OnCameraRollPictureChosen += UpdateRecipeImage;
    }

    private void OnDisable()
    {
        CameraManager.OnPictureTaken -= UpdateRecipeImage;
        CameraManager.OnCameraRollPictureChosen -= UpdateRecipeImage;
    }
    #endregion

    #region Public Methods

    public void prefillWithRecipe(Recipe recipe)
    {
        recipeImage.sprite = recipe.ImageSprite;
        caloriesInputField.text = recipe.Calories.ToString();
        prepTimeInputField.text = recipe.PrepTimeMinutes.ToString();
        ingredientAmount.text = recipe.Ingredients.Count.ToString();

        foreach (var ingredient in recipe.Ingredients)
        {
            IngredientBuilderView view = AddIngredientBuilderView();
            view.UpdateName(ingredient.IngredientName);
            view.UpdateAmount(ingredient.IngredientAmount);
        }

        foreach (var step in recipe.Steps)
        {
            DirectionBuilderView view = AddDirectiontBuilderView();
            view.SetDirection(step);
        }
    }

    public void UpdateRecipeImage(Sprite newSprite)
    {
        if (newSprite == null)
            return;

        recipeImage.sprite = newSprite;
        
        // deactivate big camera button, activate the small corner one
        largeCameraButtons.SetActive(false);
        smallCameraButtons.SetActive(true);
    }

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

    public void AddIngredientBuilder()
    {
        // make a new ingredient builder item
        Instantiate(ingedientBuilderPrefab, ingedientVerticalGroupTrans);

        // fire event for ui element change
        OnUIElementAdded?.Invoke();

        // update the ingredient count text
        Invoke("UpdateIngredientCount", .1f);
    }

    public IngredientBuilderView AddIngredientBuilderView()
    {
        // make a new ingredient builder item
        IngredientBuilderView ingredientView = Instantiate(ingedientBuilderPrefab, ingedientVerticalGroupTrans).GetComponent<IngredientBuilderView>();

        // fire event for ui element change
        OnUIElementAdded?.Invoke();

        // update the ingredient count text
        Invoke("UpdateIngredientCount", .1f);

        return ingredientView;
    }

    public void AddDirectiontBuilder()
    {
        // make a new ingredient builder item
        Instantiate(directionBuilderPrefab, directionVerticalGroupTrans);

        // fire event for ui element change
        OnUIElementAdded?.Invoke();
    }

    public DirectionBuilderView AddDirectiontBuilderView()
    {
        // make a new ingredient builder item
        DirectionBuilderView directionView = Instantiate(directionBuilderPrefab, directionVerticalGroupTrans).GetComponent<DirectionBuilderView>();

        // fire event for ui element change
        OnUIElementAdded?.Invoke();

        return directionView;
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
                tags.Add(t.GetComponentInChildren<RecipeTag>().GetTag());

        if (!CheckForFinishedRecipe())
            return;

        // create recipe 
        Recipe newRecipe = new Recipe
        {
            Name = recipeName,
            Calories = calories,
            PrepTimeMinutes = minutesPrep,
            Tags = tags,
            Ingredients = ingredients,
            Steps = new List<string>()
        };
        
        // send for publish
        var recipeApi = ReGenClient.Instance.Recipes;
        recipeApi.Create(newRecipe).Success(recipe =>
        {
            var path = $"gs://regen-66cf8.appspot.com/Recipes/{recipe.Key}";
            FirebaseStorage.DefaultInstance
                .GetReferenceFromUrl(path)
                .PutFileAsync($"file://{CameraManager.PathOfCurrentImage}")
                .Success(data =>
                {
                    Debug.Log("Finished uploading...");
                    recipe.ImageReferencePath = path;
                    recipeApi.Update(recipe);
                }).Failure(Debug.LogException);
        });

        // close canvas and refresh its content
        Disable();
        Refresh();
    }

    #endregion

    #region Private Methods

    private void SetCanvasOrderToStart()
    {
        canvas.GetComponent<Canvas>().sortingOrder = startOrder;
    }

    private void SetCanvasOrderToFinal()
    {
        canvas.GetComponent<Canvas>().sortingOrder = finalOrder;
    }

    private bool CheckForFinishedRecipe()
    {
        bool isFinished = true;

        // check if name has been seet
        if (string.IsNullOrEmpty(nameInputField.text))
        {
            NotificationManager.Instance.ShowNotification("An name must be set for the recipe.");
            return false;
        }

        // check if calories has been seet
        if (string.IsNullOrEmpty(caloriesInputField.text))
        {
            NotificationManager.Instance.ShowNotification("An calorie amount must be set for the recipe.");
            return false;
        }

        // check if prep time has been seet
        if (string.IsNullOrEmpty(prepTimeInputField.text))
        {
            NotificationManager.Instance.ShowNotification("An prep time must be set for the recipe.");
            return false;
        }

        // check if ingredients have been given
        if (ingedientVerticalGroupTrans.childCount <= 1)
        {
            NotificationManager.Instance.ShowNotification("Ingredients must be set for the recipe.");
            return false;
        }

        // check if directions have been given
        if (directionVerticalGroupTrans.childCount <= 1)
        {
            NotificationManager.Instance.ShowNotification("Directions must be set for the recipe.");
            return false;
        }

        return isFinished;
    }

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
        SetCanvasOrderToStart();

        canvas.SetActive(true);
        Invoke("SetCanvasOrderToFinal", .2f);
    }

    public void Disable()
    {
        canvas.SetActive(false);
        SetCanvasOrderToStart();
    }

    public void Init() { }

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

        // reset recipe image sprite
        recipeImage.sprite = null;

        // reset camera buttons
        largeCameraButtons.SetActive(true);
        smallCameraButtons.SetActive(false);

        OnUIRefresh?.Invoke();
    } 
    #endregion
}
