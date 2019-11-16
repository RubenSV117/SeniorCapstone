using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ReGenSDK.Model
{
    [Serializable]
    public class ReviewsPage
    {
        public List<Review> Reviews
        { get; set; }

        public string NextKey
        { get; set; }

        [NonSerialized]
        public string RecipeId;
    }
}
