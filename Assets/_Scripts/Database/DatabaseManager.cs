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

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance;
    
    private DatabaseReference databaseReference;

    private bool hasAttemptFinished;

    private List<Recipe> currentRecipes = new List<Recipe>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://regen-66cf8.firebaseio.com/");
        // Get the root databaseReference location of the database.
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

    }

    private void PublishNewRecipe(Recipe recipe, string local_file)
    {
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        string key = databaseReference.Child("recipes").Push().Key;
        // File located on disk
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://regen-66cf8.appspot.com");
        // Create a reference to the file you want to upload
        Firebase.Storage.StorageReference image_ref = storage_ref.Child("Recipes");

        image_ref.PutFileAsync(local_file)
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
    public void elasticSearchExclude(string name,string[] excludeTags, IEnumerable<string> includeTags)
    {
        currentRecipes.Clear();
        var client = new RestClient("http://35.192.138.105/elasticsearch/_search/template");
        var request = new RestRequest(Method.POST);
        string param = "{\"source\":{\"query\": {\"bool\": {";
        string must = "\"must\":[";
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

    private void TestPublish(string name)
    {
        List<Ingredient> ingredients = new List<Ingredient>()
        {
            new Ingredient("Chicken", "1"),
            new Ingredient("Breading", "Some"),
            new Ingredient("Cooking oil","enough for frying")

        };

        List<string> steps = new List<string>()
        {
            "Bread the chicken",
            "Cook oil in a fryer until boiling",
            "Dunk breaded chicken in oil until fried"
        };

        List<string> tags = new List<string>()
        {
            "poultry",
            "wheat"
        };

        List<string> reviews = new List<string>()
        {
            "This was pretty ok."
        };

        Recipe newRecipe = new Recipe(name, "", 450, 50, tags, ingredients, steps, reviews, 4);

        PublishNewRecipe(newRecipe,"hi");
    }
}
