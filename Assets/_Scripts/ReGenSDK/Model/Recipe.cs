using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReGenSDK.Model
{
    [Serializable]
    public class Recipe : RecipeLite
    {
        public string AuthorId;
        public int Calories;
        public int PrepTimeMinutes;
        public List<string> Steps;
        public string ImageReferencePath;
        public string RootImagePath;
        
        [NonSerialized] public Sprite ImageSprite;

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