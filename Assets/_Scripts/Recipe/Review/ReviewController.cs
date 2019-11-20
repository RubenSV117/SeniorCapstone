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
using ReGenSDK;
using ReGenSDK.Model;
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
    [SerializeField] private GameObject deleteButton;

    #endregion
    

    //firebase object
    private DatabaseReference databaseReference;
    private bool hasAttemptFinished;
    public List<Review> reviewList = new List<Review>();
    #region Public Methods

    public void DidTapSubmit()
    {
        ReGenClient.Instance.Reviews.Create(RecipeManagerUI.currentRecipe.Key, new Review
        {
            Content = inputField.text,
            Rating = stars.GetNumberOfActiveStars()
        });
        gameObject.SetActive(false);
    }

    public void PrefilReview(Review review)
    {
        stars.SetStarValue(review.Rating);
        inputField.text = review.Content;
//        recipeNameText.text = review.Name;
    }


    public void Reset()
    {
        inputField.text = "";
        stars.Reset();
    }

    #endregion
    public void getReviews(string recipeID, Action callback)
    {
       
    }
    
    public void SetDeleteButton(bool isActive)
    {
        deleteButton.SetActive(isActive);
    }

    public void DeleteReview()
    {
        // TODO: wire to database call
    }

}



