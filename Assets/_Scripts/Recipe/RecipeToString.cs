using UnityEngine;

/// <summary>
/// Overrides ToString
///
/// </summary>
public partial class Recipe
{
    [SerializeField] private int maxLength = 2000;

    public override string ToString()
    {
        string recipeString = "";

        recipeString += $"\nhttps://firebasestorage.googleapis.com/v0/b/regen-66cf8.appspot.com/o/Recipes%2F{Key}.jpg?alt=media&token=ebb93ded-d515-45e4-a719-169c7876faa1\n\n";

        recipeString += $"Name: {Name}\n"
                        + $"Calories: {Calories}\n"
                        + $"Prep Time: {PrepTimeMinutes}\n\n"
                        + $"Ingredients:\n";

        foreach (var i in Ingredients)
            recipeString += i.ToString() + "\n";

        recipeString += "\n\n";

        foreach (var s in Steps)
            recipeString += s + "\n\n";

        recipeString += "\n\n";

        //if (recipeString.Length > maxLength)
        //{
        //    recipeString = recipeString.Substring(0, maxLength - 3);
        //    recipeString += "...";
        //}

        return recipeString;
    }
}
