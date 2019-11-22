using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets._Scripts.Misc;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Storage;
using Firebase.Unity.Editor;
using Newtonsoft.Json;
using ReGenSDK;
using ReGenSDK.Model;
using UnityEngine;
using UnityEngine.Networking;

public class DatabaseManager : MonoBehaviour
{
    //Database endpoint
    public static string Endpoint = "https://regen-66cf8.firebaseio.com/";

    //Here is the backend handling most database calls

    public static DatabaseManager Instance;

    //firebase object
    private DatabaseReference databaseReference;

    //bool to notify coroutine when searching is done
    private bool hasAttemptFinished;


    //list objects used to later fill in UI
    private List<string> userFavorites = new List<string>();
    private List<Recipe> currentRecipes = new List<Recipe>();
    private List<Recipe> favoriteRecipes = new List<Recipe>();
    private List<Review> reviewList = new List<Review>();
    //Firebase.Auth object for user and authentication 
    FirebaseAuth auth;
    FirebaseUser user;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(Endpoint);
        // Get the root databaseReference location of the database.
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);

    }


    /**
     * AuthStateChanged used for listening to user login and get that users ID to be later used 
     * for finding the users favorites
     * 
     **/
    void AuthStateChanged(object sender, EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
            }
        }
    }
    public bool checkRecipeAuthorID(Recipe R)
    {

        FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            if (user.UserId == R.AuthorID)
            {
                return true;
            }
        }
        else
        {
            print("No user logged in.");
            return false;
        }

        return false;
    }


    /**
     * Method for getting favorites of a user, used for favorites list and
     * for checking if the user already favorited a recipe
     **/

    public void populateFavorites()
    {
        userFavorites.Clear();
        FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            hasAttemptFinished = false;
            StartCoroutine(WaitForFavoritesPop());

            FirebaseDatabase.DefaultInstance
                           .GetReference("users").Child(auth.CurrentUser.UserId).Child("favorites")
                           .GetValueAsync().ContinueWith(task =>
                           {
                               if (task.IsFaulted)
                               {
                                   print("faulted");
                                   hasAttemptFinished = true;

                               }
                               else if (task.IsCompleted)
                               {
                                   if (task.Result.ChildrenCount == 0)
                                       return;

                                   DataSnapshot snapshot = task.Result;

                                   foreach (var s in snapshot.Children)
                                   {
                                       userFavorites.Add(s.Key);
                                   }


                                   hasAttemptFinished = true;
                               }
                           });
        }
    }

    public void searchFavorites(List<string> favoriteIDs)
    {
        hasAttemptFinished = false;
        favoriteRecipes.Clear();
        StartCoroutine(WaitForFavoritesSearch());
        foreach (string id in favoriteIDs)
        {
            FirebaseDatabase.DefaultInstance
                .GetReference("recipes").Child(id)
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        print("faulted");
                    }
                    else if (task.IsCompleted)
                    {
                        if (task.Result.ChildrenCount == 0)
                            return;

                        DataSnapshot snapshot = task.Result;

                        Recipe newRecipe = JsonUtility.FromJson<Recipe>(snapshot.GetRawJsonValue());
                        newRecipe.Key = task.Result.Key;
                        favoriteRecipes.Add(newRecipe);

                        if (favoriteRecipes.Count == favoriteIDs.Count)
                            hasAttemptFinished = true;
                    }

                });


        }
    }
    private IEnumerator WaitForFavoritesPop()
    {
        yield return new WaitUntil(() => hasAttemptFinished);

        searchFavorites(userFavorites);

    }
    private IEnumerator WaitForFavoritesSearch()
    {
        yield return new WaitUntil(() => hasAttemptFinished);

        SearchManagerUI.Instance.RefreshRecipeList(favoriteRecipes, true);

    }

//
//    public void getFavorites()
//    {
//        userFavorites = new List<string>();
//        FirebaseUser user = auth.CurrentUser;
//        if (user != null)
//        {
//            hasAttemptFinished = false;
//            StartCoroutine(WaitForFavorites());
//
//            FirebaseDatabase.DefaultInstance
//                           .GetReference("users").Child(auth.CurrentUser.UserId).Child("favorites")
//                           .GetValueAsync().ContinueWith(task =>
//                           {
//                               if (task.IsFaulted)
//                               {
//                                   print("faulted");
//                                   hasAttemptFinished = true;
//
//                               }
//                               else if (task.IsCompleted)
//                               {
//                                   if (task.Result.ChildrenCount == 0)
//                                       return;
//
//                                   DataSnapshot snapshot = task.Result;
//
//                                   foreach (var s in snapshot.Children)
//                                   {
//                                       userFavorites.Add(s.Key);
//                                   }
//
//
//                                   hasAttemptFinished = true;
//                               }
//
//                           });
//        }
//    }
//
//    private IEnumerator WaitForFavorites()
//    {
//        yield return new WaitUntil(() => hasAttemptFinished);
//
//        RecipeManagerUI.Instance.SetFavorited(userFavorites);
//
//    }

    /*
     * Method for favoriting a recipe, takes the recipeID of the selected recipe and sends that to the users favorites on firebase
     */
//    public bool favoriteRecipe(string recipeID)
//    {
//
//        FirebaseUser user = auth.CurrentUser;
//
//        if (user != null)
//        {
//            string uid = user.UserId;
//            string json = JsonUtility.ToJson(recipeID);
//            databaseReference.Child("users").Child(uid).Child("favorites").Child(recipeID).SetValueAsync(true);
//            return true;
//        }
//        else
//        {
//            print("No user logged in.");
//            return false;
//        }
//
//    }
    //Same as favorite but removes
//    public bool unfavoriteRecipe(string recipeID)
//    {
//
//        FirebaseUser user = auth.CurrentUser;
//
//        if (user != null)
//        {
//            string uid = user.UserId;
//            string json = JsonUtility.ToJson(recipeID);
//            databaseReference.Child("users").Child(uid).Child("favorites").Child(recipeID).RemoveValueAsync();
//            return true;
//        }
//        else
//        {
//            print("No user logged in.");
//            return false;
//        }
//    }

    /*
     * Method for publishing a recipe to firebase,
     * Takes in the recipe object that has all the inputted info from the recipe publishing page and a photo of the food
     * then sends the photo to storage and the recipe to our DB
     */
//    public void PublishNewRecipe(Recipe recipe, string local_file)
//    {
//        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
//        string key = databaseReference.Child("recipes").Push().Key;
//        // File located on disk
//        StorageReference storage_ref = storage.GetReferenceFromUrl("gs://regen-66cf8.appspot.com/Recipes/" + key);
//        // Create a reference to the file you want to upload
//        storage_ref.PutFileAsync("file://" + local_file)
//          .ContinueWith((Task<StorageMetadata> task) =>
//          {
//              if (task.IsFaulted || task.IsCanceled)
//              {
//                  Debug.Log(task.Exception.ToString());
//              }
//              else
//              {
//                  Debug.Log("Finished uploading...");
//              }
//          });
//        recipe.ImageReferencePath = $"gs://regen-66cf8.appspot.com/Recipes/" + key;
//
//        string json = JsonUtility.ToJson(recipe);
//        databaseReference.Child("recipes").Child(key).SetRawJsonValueAsync(json);
//
//        NotificationManager.Instance.ShowNotification("Publish Successful");
//    }

    //private IEnumerator WaitForElasticSearch(UnityWebRequestAsyncOperation operation)
    //{
    //    yield return 0;
    //}

    //Search function for firebase using ID's found.
    //public void Search(object hits)
    //{
       
    //}
    
    //coroutine that waits for search to be finished fully before updating the UI
    private IEnumerator WaitForRecipes()
    {
        yield return new WaitUntil(() => hasAttemptFinished);

        // if search has yielded results, update the recipe list in the ui
        if (currentRecipes.Count > 0)
            SearchManagerUI.Instance.RefreshRecipeList(currentRecipes);
    }

    public void GetCommunityRating(string recipeKey, Action<double> callback)
    {
        double communityRating = 0;
        FirebaseDatabase.DefaultInstance
                        .GetReference("recipeRatings")
                        .Child($"{recipeKey}")
                        .GetValueAsync()
                        .ContinueWithOnMainThread(task =>
                        {
                            try
                            {
                                if (task.IsFaulted)
                                {

                                }
                                else if (task.IsCompleted)
                                {
                                    var snapshot = task.Result;
                                    if (snapshot.Exists)
                                    {
                                        communityRating = (double)((IDictionary)snapshot.Value)["avgRating"];
                                        //RecipeManagerUI.Instance.DrawCommunityRating()
                                        callback(communityRating);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                var error = e.Message;
                            }
                        });
    }

    public void GetPreviousSurveyRating(string recipeKey, Action<int> callback)
    {
        int rating = 0;
        //ratingNum = 0;
        //hasAttemptFinished = false;


        // Check that user is logged in first
        if (user != null)
        {
            //StartCoroutine(WaitForSurveyRating(ratingNum));
            FirebaseDatabase.DefaultInstance
                            .GetReference("recipeRatings")
                            .Child($"{recipeKey}")
                            .Child("users")
                            .Child($"{user.UserId}")
                            .GetValueAsync()
                            .ContinueWithOnMainThread(task =>
                            {
                                try
                                {
                                    if (task.IsFaulted)
                                    {
                                        Debug.Log("A problem occurred.");
                                    }
                                    else if (task.IsCompleted)
                                    {
                                        DataSnapshot snapshot = task.Result;

                                        if (snapshot.Exists)
                                        {
                                            rating = JsonConvert.DeserializeObject<int>(snapshot.GetRawJsonValue());
                                            callback(rating);
                                        }
                                    }
                                    hasAttemptFinished = true;
                                }
                                catch (Exception e)
                                {
                                    var error = e.Message;
                                }
                            });
        }
    }

    public void GetPreviousReview(string recipeKey, Action<string> callback)
    {
        string content;
        if (user != null)
        {
            try
            {
                FirebaseDatabase.DefaultInstance
                    .GetReference("reviews")
                    .Child($"{recipeKey}")
                    .Child($"{user.UserId}")
                    .Child($"content")
                    .GetValueAsync()
                    .ContinueWithOnMainThread(task =>
                    {
                        try
                        {
                            if (task.IsFaulted)
                            {
                                Debug.Log("A problem occurred.");
                            }
                            else if (task.IsCompleted)
                            {
                                DataSnapshot snapshot = task.Result;

                                if (snapshot.Exists)
                                {
                                    content = JsonConvert.DeserializeObject<string>(snapshot.GetRawJsonValue());
                                    callback(content);
                                }
                            }
                            hasAttemptFinished = true;
                        }
                        catch (Exception e)
                        {
                            var error = e.Message;
                        }
                    });
            }
            catch (Exception e)
            {

            }
        }
        else
        {
            NotificationManager.Instance.ShowNotification("You must be logged in to update or write a review.");
        }
    }

    public void deleteReview(string recipeKey)
    {
        if (user != null)
        {
            try
            {
                FirebaseDatabase.DefaultInstance
                    .GetReference("reviews")
                    .Child($"{recipeKey}")
                    .Child($"{user.UserId}")
                    .RemoveValueAsync();
            }
            catch (Exception e)
            {

            }
        }
        else
        {
            NotificationManager.Instance.ShowNotification("You must be logged in to delete a review.");
        }
    }

    public void UpdateUserRatingForRecipe(string recipeKey, int rating)
    {
        //ratingNum = rating;
        //hasAttemptFinished = false;

        Debug.Log($"Rating the recipe a {rating}...");

        // Check that user is logged in first
        if (user != null)
        {
            try
            {
                string path = $"/recipeRatings/{recipeKey}";
                FirebaseDatabase.DefaultInstance
                                .GetReference("recipeRatings")
                                        .Child($"{recipeKey}")
                                        .GetValueAsync()
                                        .ContinueWithOnMainThread(task =>
                                        {
                                            if (task.IsFaulted)
                                            {
                                                Debug.Log("A problem occurred.");
                                            }
                                            else if (task.IsCompleted)
                                            {
                                                DataSnapshot snapshot = task.Result;

                                                // Update the existing recipe rating metadata
                                                Rating recipeRating;
                                                if (snapshot.Exists)
                                                {
                                                    recipeRating = JsonConvert.DeserializeObject<Rating>(snapshot.GetRawJsonValue());

                                                    // Just return if user tapped the same rating they already gave
                                                    int oldRating = recipeRating?.users?[$"{user.UserId}"] ?? 0;
                                                    if (rating == oldRating)
                                                        return;
                                                }
                                                // Otherwise, create rating metadata for the recipe
                                                else
                                                {
                                                    recipeRating = new Rating();
                                                }

                                                // Update recipe rating then send it back to Firebase as json
                                                recipeRating.UpdateRecipeRating(user.UserId, rating);

                                                // Note: Calls to game objects are only available through the main thread. This section only works because The task extension method Task.ContinueWithOnMainThread() is called.
                                                #region 
                                                //RecipeManagerUI.Instance.UpdateCommunityRating(rating);


                                                RecipeManagerUI.Instance.DrawSurveyRating(rating);
                                                #endregion
                                                databaseReference.Child(path).SetRawJsonValueAsync(JsonConvert.SerializeObject(recipeRating));
                                            }
                                        });
            }
            catch (Exception e)
            {
            }
        }
        else
        {
            NotificationManager.Instance.ShowNotification("You must be logged in to rate recipes.");
        }

    }
}