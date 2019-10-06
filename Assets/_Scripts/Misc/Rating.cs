using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Scripts.Misc
{
    public class Rating
    {
        public double avgRating { get; set; }
        public int numRatings { get; set; }
        public Dictionary<string, int> users { get; set; }

        /// <summary>
        /// Construct a new rating with default values.
        /// </summary>
        public Rating()
        {
            avgRating = 0;
            numRatings = 0;
            users = new Dictionary<string, int>();
        }

        /// <summary>
        /// Updates the recipe rating for the given user.
        /// 3 cases:
        /// 1.) Recipe has no ratings.
        /// 2.) Recipe has ratings but user hasn't rated the recipe before.
        /// 3.) Recipe has ratings and user has already rated the recipe.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="rating"></param>
        public void UpdateRecipeRating(string userId, int rating)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be null or whitespace.");
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating cannot be less than 1 or greater than 5.");
            if (!string.IsNullOrWhiteSpace(userId) && rating >= 1 && rating <= 5)
            {
                // Case 1
                if (numRatings == 0 && users.Count == 0)
                {
                    numRatings = 1;
                }

                // Case 2
                else if (numRatings > 0 && !users.ContainsKey(userId))
                {
                    numRatings++;
                }

                // Case 3
                else if (numRatings > 0 && users.ContainsKey(userId))
                {
                }
                users[userId] = rating;
                avgRating = (double)users.Sum(x => x.Value) / (double)(numRatings);
            }
        }
    }
}
