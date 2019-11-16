using System;
using System.Collections.Generic;

namespace ReGenSDK.Model
{
    [Serializable]
    public class Recipe : RecipeLite
    {
        public string AuthorId;
        public int? Calories;
        public int? PrepTimeMinutes;
        public List<string> Steps;
        public string ImageReferencePath;
        public string RootImagePath;

//        public Recipe(string key, string authorId, string name, int? calories, int? prepTimeMinutes,
//            List<Ingredient> ingredients, List<string> steps, List<string> tags,
//            string imageReferencePath, string rootImagePath)
//        {
//            Key = key;
//            AuthorId = authorId;
//            Name = name;
//            Calories = calories;
//            PrepTimeMinutes = prepTimeMinutes;
//            Ingredients = ingredients;
//            Steps = steps;
//            Tags = tags;
//            ImageReferencePath = imageReferencePath;
//            RootImagePath = rootImagePath;
//        }


        public override string ToString()
        {
            String recipeString = "";

            recipeString += "Name: " + Name;
            recipeString += "\nCalories: " + Calories;
            recipeString += "\nPrep Time: " + PrepTimeMinutes + " minutes";
            recipeString += "\n\nIngredients: ";

            foreach (var ingredient in Ingredients)
                recipeString += "\n" + ingredient;

            recipeString += "\n\nSteps: ";

            foreach (var step in Steps)
                recipeString += "\n\n" + step;

            recipeString += "\n\n" + RootImagePath;

            return recipeString;
        }
    }
}