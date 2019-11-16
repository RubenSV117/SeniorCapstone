using System;

namespace ReGenSDK.Model
{
    [Serializable]
    public class Review
    {
        public string ReviewId {get; set;}
        public string UserId {get; set;}
        public string RecipeId {get; set;}
        public string Content {get; set;}
        public DateTime Timestamp {get; set;}
        public int Rating {get; set;}

        public override string ToString()
        {
            return Content;
        }
    }
}