using Firebase.Database;
using System.Threading.Tasks;
using Firebase;
using UnityEngine;

public class ReviewServiceImpl : ReviewService
{

    public override Task SubmitReview(string recipeId, string content)
    {
        return FirebaseDatabase.DefaultInstance
               .GetReference("reviews")
               .Child(recipeId)
               .Child(LoginService.Instance.User.UserId)
               .Child("content")
               .SetValueAsync(content.Trim())
               .WithFailure<FirebaseException>(e => Debug.LogException(e))
               .WithSuccess(() => Debug.Log("Review Successfully submitted."));
    }
}
