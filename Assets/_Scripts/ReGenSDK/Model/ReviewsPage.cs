using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ReGenSDK.Model
{
    [Serializable]
    public class ReviewsPage
    {
        public List<Review> Reviews;


        public string NextKey;

        [NonSerialized]
        public string RecipeId;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }
}
