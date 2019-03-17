using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using Object = System.Object;
using Restsharp;


public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance;
    
    private DatabaseReference databaseReference;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://regen-66cf8.firebaseio.com/");
        // Get the root databaseReference location of the database.
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        TestPublish("Chicken Burrito");
        TestPublish("Spring Roll");


        TestRead("Hawaiian");
    }

    private void PublishNewRecipe(Recipe recipe)
    {
        string json = JsonUtility.ToJson(recipe);

        databaseReference.Child("recipes").Child(recipe.Name).SetRawJsonValueAsync(json);
    }
    private void testWebrequestName(string name)
    {
        var client = new RestClient("http://35.192.138.105/elasticsearch/_search/template");
        var request = new RestRequest(Method.GET);
        request.AddHeader("Postman-Token", "e4f474bc-ca7c-4853-a2ec-27f7e5748c88");
        request.AddHeader("cache-control", "no-cache");
        request.AddHeader("Authorization", "Basic dXNlcjpYNE1keTVXeGFrbVY=");
        request.AddHeader("Content-Type", "application/json");
        request.AddParameter("undefined", "{\"source\": { \"query\": {\"bool\": {\"must_not\": [ {\"wildcard\": " +
            "{\"{{my_field1}}\": \"*{{my_value}}*\"}},{\"fuzzy\": {\"{{my_field1}}\": \"{{my_value}}\"}}, {\"wildcard\": " +
            "{\"{{my_field2}}\": \"*{{my_value}}*\"}},{\"fuzzy\": {\"{{my_field2}}\": \"{{my_value}}\"}},{\"wildcard\": {\"{{my_field3}}\": " +
            "\"*{{my_value}}*\"}},{\"fuzzy\": {\"{{my_field3}}\": \"{{my_value}}\"}}]}},\"size\": \"{{my_size}}\"},\"params\": {\"my_field1\": " +
            "\"name\",\"my_field2\": \"ingredients\",\"my_field3\": \"tags\",\"my_value\": \"endies\",\"my_size\": 100}}", ParameterType.RequestBody);
        IRestResponse response = client.Execute(request);
        Console.WriteLine(response.Content);
    }
    private void TestRead(string name)
    {
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
                    DataSnapshot snapshot = task.Result;
                    // Do something with snapshot...
                }
            });
    }


    private void TestPublish(string name)
    {
        List<Ingredient> ingredients = new List<Ingredient>()
        {
            new Ingredient("flour", "1/2 cup"),
            new Ingredient("marinara", "1/2 cup"),
            new Ingredient("mozzerella", "2 cups"),
            new Ingredient("ham", "1/3 cup"),
            new Ingredient("pineapple", "1/4 cup")
        };

        List<string> steps = new List<string>()
        {
            "Knead the dough.",
            "Add the marinara sauce.",
            "Add the mozerrella cheese.",
            "Add the ham.",
            "Add the pineapple.",
            "Bake at 360F for 45 minutes."
        };

        List<string> tags = new List<string>()
        {
            "dairy"
        };

        List<string> reviews = new List<string>()
        {
            "This was pretty ok."
        };

        Recipe newRecipe = new Recipe(name, 450, 50, tags, ingredients, steps, reviews, 4);

        PublishNewRecipe(newRecipe);
    }
}
