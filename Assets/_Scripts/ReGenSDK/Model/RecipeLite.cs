using System;
using System.Collections.Generic;

namespace ReGenSDK.Model
{
    [Serializable]
    public class RecipeLiteJson
    {
        public string Key;
        public string AuthorID;
        public string name;
        public List<Ingredient> ingredients;
        public List<string> tags;

        public override string ToString()
        {
            return $"{nameof(Key)}: {Key}, {nameof(AuthorID)}: {AuthorID}, {nameof(name)}: {name}, {nameof(ingredients)}: {ingredients}, {nameof(tags)}: {tags}";
        }

        public RecipeLite ToActual()
        {
            return new RecipeLite
            {
                Key = Key,
                AuthorID = AuthorID,
                Name = name,
                Ingredients = ingredients,
                Tags = tags
            };
        }
    }
    
    public class RecipeLite
    {
        public string Key;
        public string AuthorID;
        public string Name;
        public List<Ingredient> Ingredients;
        public List<string> Tags;

        public override string ToString()
        {
            return $"{nameof(Key)}: {Key}, {nameof(AuthorID)}: {AuthorID}, {nameof(Name)}: {Name}, {nameof(Ingredients)}: {Ingredients}, {nameof(Tags)}: {Tags}";
        }
    }
}