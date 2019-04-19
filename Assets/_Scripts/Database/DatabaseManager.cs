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

    public static DatabaseManager Instance;
    
    private DatabaseReference databaseReference;

    private bool hasAttemptFinished;

    private List<string> userFavorites = new List<string>();

    private List<Recipe> currentRecipes = new List<Recipe>();

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
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    public void getFavorites()
    {

        FirebaseDatabase.DefaultInstance
                       .GetReference("users").Child(auth.CurrentUser.UserId)
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
                               string recipeID;
                               recipeID = JsonUtility.FromJson<string>(snapshot.GetRawJsonValue());
                               userFavorites.Add(recipeID);
                           }

                       });


        
    }

    public void favoriteRecipe(string recipeID)
    {

        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            string uid = user.UserId;
            string key = databaseReference.Child("users").Push().Key;
            string json = JsonUtility.ToJson(uid + "," + recipeID);
            databaseReference.Child("users").Child(uid).SetRawJsonValueAsync(json);
        }
        else
        {
            print("No user logged in.");
        }
        
    }

    private void PublishNewRecipe(Recipe recipe, string local_file)
    {
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        string key = databaseReference.Child("recipes").Push().Key;
        // File located on disk
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://regen-66cf8.appspot.com/Recipes/" + key);
        // Create a reference to the file you want to upload
        storage_ref.PutFileAsync(local_file)
          .ContinueWith((Task<StorageMetadata> task) => {
              if (task.IsFaulted || task.IsCanceled)
              {
                  Debug.Log(task.Exception.ToString());
          // Uh-oh, an error occurred!
              }
              else
              {
                  Debug.Log("Finished uploading...");
              }
          });
        recipe.ImageReferencePath = $"gs://regen-66cf8.appspot.com/Recipes/" + key;

        string json = JsonUtility.ToJson(recipe);
        print(json);
        databaseReference.Child("recipes").Child(key).SetRawJsonValueAsync(json);
    }
    public void elasticSearchExclude(string name,string[] excludeTags, string[] includeTags)
    {
        currentRecipes.Clear();
        var client = new RestClient("http://35.192.138.105/elasticsearch/_search/template");
        var request = new RestRequest(Method.POST);
        string param = "{\"source\":{\"query\": {\"bool\": {";
        string must = "\"must\":[";
        string must_not = "\"must\":[";
        string Excludetag = "{\"term\": {\"tags\": \"";
        string should = "\"should\": [\n{\n\"wildcard\": {\n\"name\": \"*" + name +"*\"\n}\n}\n,\n{\n\"fuzzy\": {\n\"name\": {\n\"value\": \""+name + "\"\n}\n}\n}\n]\n}\n},\n\"size\": 10";
        request.AddHeader("Postman-Token", "f1918e1d-0cbd-4373-b9e6-353291796dd6");
        request.AddHeader("cache-control", "no-cache");
        request.AddHeader("Authorization", "Basic dXNlcjpYNE1keTVXeGFrbVY=");
        request.AddHeader("Content-Type", "application/json");
        if (excludeTags.Length > 0)
        {
            param = param + must;
            for(int i=0; i < excludeTags.Length; i++)
            {
                if(i !=0) {
                    param += ",";
                }
                param = param + Excludetag + excludeTags[i] + "\"}}";
            }
            param += "],";
            param = param + must_not;
            for(int i= 0; i < includeTags.Length; i++)
            {
                if (i != 0)
                {
                    param += ",";

                }
                param = param + Excludetag + includeTags[i] + "\"}}";

            }
            param = param + should + "}}";
        }
        else
        {
            param = "{\"source\": { \"query\": {\"bool\": {\"should\": [ {\"wildcard\": " +
                "{\"{{my_field1}}\": \"*{{my_value}}*\"}},{\"fuzzy\": {\"{{my_field1}}\": \"{{my_value}}\"}}, {\"wildcard\": " +
                "{\"{{my_field2}}\": \"*{{my_value}}*\"}},{\"fuzzy\": {\"{{my_field2}}\": \"{{my_value}}\"}},{\"wildcard\": {\"{{my_field3}}\": " +
                "\"*{{my_value}}*\"}},{\"fuzzy\": {\"{{my_field3}}\": \"{{my_value}}\"}}]}},\"size\": \"{{my_size}}\"},\"params\": {\"my_field1\": " +
                "\"name\",\"my_field2\": \"ingredients.IngredientName\",\"my_field3\": \"tags\",\"my_value\": \"" + name +
                "\",\"my_size\": 100}}";
        }
		print(param);
        request.AddParameter("application/json",param, ParameterType.RequestBody);
        IRestResponse response = client.Execute(request);

        if (!response.Content.Contains("\"total\":0"))
        {
            print(response.Content);

            Rootobject rootObject = JsonConvert.DeserializeObject<Rootobject>(response.Content);
            Search(rootObject.hits.hits);
        }
        else
        {
        SearchManagerUI.Instance.RefreshRecipeList(currentRecipes);
        }

    }
    
    //Searching by name, old version of the search function
    public void Search(string name)
    {
        hasAttemptFinished = false;
        currentRecipes.Clear();

        StartCoroutine(WaitForRecipes());

        FirebaseDatabase.DefaultInstance
            .GetReference("recipes").OrderByChild("Name")
            .StartAt(name)
            .EndAt(name + "\uf8ff")
            .GetValueAsync().ContinueWith(task => {
                if (task.IsFaulted)
                {
                    // Handle the error...
                }
                else if (task.IsCompleted)
                {
                    if (task.Result.ChildrenCount == 0)
                        return;

                    DataSnapshot snapshot = task.Result;
                    print(snapshot.GetRawJsonValue());

                    foreach (var recipe in snapshot.Children)
                    {
                        Recipe newRecipe = JsonUtility.FromJson<Recipe>(recipe.GetRawJsonValue());
                        currentRecipes.Add(newRecipe);
                    }
                }

                hasAttemptFinished = true;
            });
    }

    //Search function for firebase using ID's found.
    public void Search(Hit[] hits)
    {
        hasAttemptFinished = false;
        currentRecipes.Clear();
        StartCoroutine(WaitForRecipes());
        for (int i = 0; i < hits.Length; i++)
        {
            print(hits[i]._id);
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
                        currentRecipes.Add(newRecipe);

                        if (currentRecipes.Count == hits.Length)
                            hasAttemptFinished = true;
                    }

                });
         
           
        }
    }

    private IEnumerator WaitForRecipes ()
    {
        yield return new WaitUntil(() => hasAttemptFinished);

        // if search has yielded results, update the recipe list in the ui
        if(currentRecipes.Count > 0)
            SearchManagerUI.Instance.RefreshRecipeList(currentRecipes);
    }

 
}
