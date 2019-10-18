using Firebase.Database;
using System.Threading.Tasks;
using Firebase;
using UnityEngine;
using System.Collections.Generic;

public class ReviewServiceImpl : ReviewService
{
    public override Task SubmitReview(string recipeId, string content)
    {
        var values = new Dictionary<string, object>();
        values["content"] = content.Trim();
        values["timestamp"] = System.DateTime.UtcNow.ToBinary();

        return FirebaseDatabase.DefaultInstance
                .GetReference("reviews")
                .Child(recipeId)
                .Child(LoginService.Instance.User.UserId)
                .SetValueAsync(values)
                .WithFailure<FirebaseException>(e => Debug.LogException(e))
                .WithSuccess(() => Debug.Log("Review Successfully submitted."));
    }

    public override Task<List<Review>> GetReviews(string recipeId)
    {
        return FirebaseDatabase.DefaultInstance
            .GetReference("reviews")
            .Child(recipeId)
            .GetValueAsync()
            .WithFailure<DataSnapshot, FirebaseException>(e => Debug.LogException(e))
            .WithSuccess<DataSnapshot, List<Review>>(data =>
            {
                var reviews = new List<Review>((int)data.ChildrenCount);
                foreach (var r in data.Children)
                {
                    reviews.Add(new Review(
                        (string)r.Child("content").GetValue(false),
                        System.DateTime.FromBinary((long)r.Child("timestamp").GetValue(false)),
                        r.Key));
                }
                return reviews;
            });
    }
}
