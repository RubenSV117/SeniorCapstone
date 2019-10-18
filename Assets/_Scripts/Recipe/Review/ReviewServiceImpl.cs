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
        values["timestamp"] = System.DateTime.UtcNow;

        return FirebaseDatabase.DefaultInstance
                .GetReference("reviews")
                .Child(recipeId)
                .Child(LoginService.Instance.User.UserId)
                .SetValueAsync(values)
                .WithFailure<FirebaseException>(e => Debug.LogException(e))
                .WithSuccess(() => Debug.Log("Review Successfully submitted."));
    }
}
