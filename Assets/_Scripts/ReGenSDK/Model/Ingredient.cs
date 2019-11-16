using System;

namespace ReGenSDK.Model
{
    [Serializable]
    public class Ingredient
    {
//        [Required]
//        [StringLength(30, MinimumLength = 1)]
        public string IngredientName;

//        [Required]
//        [StringLength(30, MinimumLength = 1)]
        public string IngredientAmount;

        public override string ToString()
        {
            return $"{IngredientAmount} {IngredientName}";
        }
    }
}