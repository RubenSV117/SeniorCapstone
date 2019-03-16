using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Data container for an ingredient
/// </summary>
[Serializable]
public class Ingredient
{
    public string IngredientName;
    public string IngredientAmount;

    public Ingredient(string name, string amount)
    {
        IngredientName = name;
        IngredientAmount = amount;
    }

    public override string ToString()
    {
        return $"{IngredientAmount} {IngredientName}";
    }
}

