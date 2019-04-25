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
public class DatabaseManager : MonoBehaviour
{
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

    //Firebase.Auth object for user and authentication 
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://regen-66cf8.firebaseio.com/");
        // Get the root databaseReference location of the database.
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        Debug.Log("Setting up Firebase Auth");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }


    /**
     * AuthStateChanged used for listening to user login and get that users ID to be later used 
     * for finding the users favorites
     * 
     **/
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
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


    /**
     * Method for getting favorites of a user, used for favorites list and
     * for checking if the user already favorited a recipe
     **/

    public void populateFavorites()
    {
        userFavorites = new List<string>();
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
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

        SearchManagerUI.Instance.RefreshRecipeList(favoriteRecipes,true);

    }


    public void getFavorites()
    {
        userFavorites = new List<string>();
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            hasAttemptFinished = false;
            StartCoroutine(WaitForFavorites());

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

    private IEnumerator WaitForFavorites()
    {
        yield return new WaitUntil(() => hasAttemptFinished);

        RecipeManagerUI.Instance.SetFavorited(userFavorites);

    }

    /*
     * Method for favoriting a recipe, takes the recipeID of the selected recipe and sends that to the users favorites on firebase
     */
    public bool favoriteRecipe(string recipeID)
    {

        Firebase.Auth.FirebaseUser user = auth.CurrentUser;

        if (user != null)
        {
            string uid = user.UserId;
            string json = JsonUtility.ToJson(recipeID);
            databaseReference.Child("users").Child(uid).Child("favorites").Child(recipeID).SetValueAsync(true);
            return true;
        }
        else
        {
            print("No user logged in.");
            return false;
        }
        
    }
    //Same as favorite but removes
    public bool unfavoriteRecipe(string recipeID)
    {

        Firebase.Auth.FirebaseUser user = auth.CurrentUser;

        if (user != null)
        {
            string uid = user.UserId;
            string json = JsonUtility.ToJson(recipeID);
            databaseReference.Child("users").Child(uid).Child("favorites").Child(recipeID).RemoveValueAsync();
            return true;
        }
        else
        {
            print("No user logged in.");
            return false;
        }
    }

    /*
     * Method for publishing a recipe to firebase,
     * Takes in the recipe object that has all the inputted info from the recipe publishing page and a photo of the food
     * then sends the photo to storage and the recipe to our DB
     */
    public void PublishNewRecipe(Recipe recipe, string local_file)
    {
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        string key = databaseReference.Child("recipes").Push().Key;
        // File located on disk
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://regen-66cf8.appspot.com/Recipes/" + key);
        // Create a reference to the file you want to upload
        storage_ref.PutFileAsync("file://" + local_file)
          .ContinueWith((Task<StorageMetadata> task) => {
              if (task.IsFaulted || task.IsCanceled)
              {
                  Debug.Log(task.Exception.ToString());
              }
              else
              {
                  Debug.Log("Finished uploading...");
              }
          });
        recipe.ImageReferencePath = $"gs://regen-66cf8.appspot.com/Recipes/" + key;

        string json = JsonUtility.ToJson(recipe);
        databaseReference.Child("recipes").Child(key).SetRawJsonValueAsync(json);

        NotificationManager.Instance.ShowNotification("Publish Successful");
    }

    /*
     * Our search function, checks an elastic search VM that holds minor information about recipes by building
     * an HTTP get request using RestSharp, then the resulting json is then parsed and the IDs are sent to the search() function
     * there it takes the full information of those recipes
     */
    public void elasticSearchExclude(string name,string[] includeTags, string[] excludeTags)
    {
        //clear the UI
        currentRecipes.Clear();
        //initializing restclient
        var client = new RestClient("http://35.192.138.105/elasticsearch/_search/template");
        //POST is the only request allowed to send with a body however it can be used to Get information as well in this case
        var request = new RestRequest(Method.POST);

        //Here is where we start building a query with the tags
        string param = "{\"source\":{\"query\": {\"bool\": {";
        string must = "\"must\":[";
        string must_not = "\"must_not\":[";
        string searchTag = "{\"term\": {\"tags.keyword\": \"";
        string should = "\"should\": [\n{\n\"wildcard\": {\n\"name\": \"*" + name +"*\"\n}\n}\n,\n{\n\"fuzzy\": {\n\"name\": {\n\"value\": \""+name + "\"\n}\n}\n}\n]\n}\n},\n\"size\": 10";
        request.AddHeader("Postman-Token", "f1918e1d-0cbd-4373-b9e6-353291796dd6");
        request.AddHeader("cache-control", "no-cache");
        request.AddHeader("Authorization", "Basic dXNlcjpYNE1keTVXeGFrbVY=");
        request.AddHeader("Content-Type", "application/json");
        //checks if tags are being used
        if (excludeTags.Length > 0 || includeTags.Length > 0)
        {
            //if exclude tags are being used
            if (excludeTags.Length > 0)
            {
                param = param + must;
                for (int i = 0; i < excludeTags.Length; i++)
                {
                    if (i != 0)
                    {
                        param += ",";
                    }
                    param = param + searchTag + excludeTags[i] + "\"}}";
                }
                param += "],";
            }
            //if include tags are being used
            if (includeTags.Length > 0)
            {
                param = param + must_not;
                for (int i = 0; i < includeTags.Length; i++)
                {
                    if (i != 0)
                    {
                        param += ",";

                    }
                    param = param + searchTag + includeTags[i] + "\"}}";

                }
                param += "],";
            }
            //add the should clause for the names 
            param = param + should + "}}";
        }
        //This area is for if no tags 
        else
        {
            param = "{\"source\": { \"query\": {\"bool\": {\"should\": [ {\"wildcard\": " +
                "{\"{{my_field1}}\": \"*{{my_value}}*\"}},{\"fuzzy\": {\"{{my_field1}}\": \"{{my_value}}\"}}, {\"wildcard\": " +
                "{\"{{my_field2}}\": \"*{{my_value}}*\"}},{\"fuzzy\": {\"{{my_field2}}\": \"{{my_value}}\"}},{\"wildcard\": {\"{{my_field3}}\": " +
                "\"*{{my_value}}*\"}},{\"fuzzy\": {\"{{my_field3}}\": \"{{my_value}}\"}}]}},\"size\": \"{{my_size}}\"},\"params\": {\"my_field1\": " +
                "\"name\",\"my_field2\": \"ingredients.IngredientName\",\"my_field3\": \"tags\",\"my_value\": \"" + name +
                "\",\"my_size\": 100}}";
        }
        //this is for testing purposed to see the Parameter for the request
        request.AddParameter("application/json",param, ParameterType.RequestBody);
        //save the response
        IRestResponse response = client.Execute(request);
        //if the response is not empty
        if (!response.Content.Contains("\"total\":0"))
        { 
            //convert it to rootObject
            Rootobject rootObject = JsonConvert.DeserializeObject<Rootobject>(response.Content);
            //send the hits of IDs to the search function
            Search(rootObject.hits.hits);
        }
        else
        {
           //if the response is empty, just refresh the list, should just refresh the list making it empty
        SearchManagerUI.Instance.RefreshRecipeList(currentRecipes);
        }

    }

    //Search function for firebase using ID's found.
    public void Search(Hit[] hits)
    {
        hasAttemptFinished = false;
        currentRecipes.Clear();
        StartCoroutine(WaitForRecipes());
        for (int i = 0; i < hits.Length; i++)
        {
            FirebaseDatabase.DefaultInstance
                .GetReference("recipes").Child(hits[i]._id)
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
                        currentRecipes.Add(newRecipe);

                        if (currentRecipes.Count == hits.Length)
                            hasAttemptFinished = true;
                    }

                });
         
           
        }
    }

    //coroutine that waits for search to be finished fully before updating the UI
    private IEnumerator WaitForRecipes ()
    {
        yield return new WaitUntil(() => hasAttemptFinished);

        // if search has yielded results, update the recipe list in the ui
        if(currentRecipes.Count > 0)
            SearchManagerUI.Instance.RefreshRecipeList(currentRecipes);
    }

 
}
