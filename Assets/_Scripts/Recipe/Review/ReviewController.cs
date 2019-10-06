using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using Object = System.Object;
using RestSharp;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using Firebase.Storage;
using System.Threading.Tasks;
using Firebase.Auth;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;
/// <summary>
/// Manages recipe reviews
///
/// Ruben Sanchez
/// </summary>
public class ReviewController : MonoBehaviour
{

    #region Properties

    public Recipe recipe
    {
        set => recipeNameText.text = value.Name;
    }

    [SerializeField] private StarController stars;
    [SerializeField] private InputField inputField;
    [SerializeField] private Text recipeNameText;

    #endregion
    //Database endpoint
    public static string Endpoint = "https://regen-66cf8.firebaseio.com/";

    //Here is the backend handling most database calls

    public static DatabaseManager Instance;

    //firebase object
    private DatabaseReference databaseReference;
    private bool hasAttemptFinished;
    //list<Review> reviewList = new list<Review>();
    #region Public Methods

    public void DidTapSubmit()
    {
        Review review = new Review(stars.GetNumberOfActiveStars(), inputField.text, RecipeManagerUI.currentRecipe);
        gameObject.SetActive(false);
        // TODO: Send review to backend service for publishing
    }

    public void Reset()
    {
        inputField.text = "";
        stars.Reset();
    }

    #endregion
    public void getReviews(string recipeID)
    {
        //hasAttemptFinished = false;
        //favoriteRecipes.Clear();
        //StartCoroutine(WaitForReviews());
        //FirebaseDatabase.DefaultInstance
        //       .GetReference("recipes").Child("reviews").Child(recipeID)
        //       .GetValueAsync().ContinueWith(task =>
        //       {
        //           if (task.IsFaulted)
        //           {
        //               print("faulted");
        //               hasAttemptFinished = true;

        //           }
        //           else if (task.IsCompleted)
        //           {
        //               if (task.Result.ChildrenCount == 0)
        //                   return;

        //               DataSnapshot snapshot = task.Result;

        //               foreach (var s in snapshot.Children)
        //               {
        //                   Review newReview = JsonUtility.FromJson<Recipe>(s.GetRawJsonValue());
        //                   reviewList.Add(newReview);
        //               }


        //           }
        //       });
    }
    private IEnumerator WaitForReviews()
    {
        yield return new WaitUntil(() => hasAttemptFinished);

        //searchFavorites(userFavorites);

    }
}

struct Review
{
    int starRating;
    string review;
    Recipe recipe;

    public Review(int stars, string review, Recipe recipe)
    {
        this.starRating = stars;
        this.review = review;
        this.recipe = recipe;
    }
}

