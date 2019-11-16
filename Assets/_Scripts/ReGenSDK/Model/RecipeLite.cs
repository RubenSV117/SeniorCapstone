using System;
using System.Collections.Generic;

namespace ReGenSDK.Model
{
    [Serializable]
    public class RecipeLite
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public List<string> Tags { get; set; }
    }
}