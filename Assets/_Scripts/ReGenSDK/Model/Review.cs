using System;

namespace ReGenSDK.Model
{
    [Serializable]
    public class Review
    {
        public string ReviewId;
        public string UserId;
        public string RecipeId;
        public string Content;
        public DateTime Timestamp;
        public int Rating;

        public override string ToString()
        {
            return Content;
        }
    }
}