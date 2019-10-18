using UnityEngine;
using NUnit.Framework;
using Firebase.Database;
using System.Linq;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests
{
    public class ReviewUI
    {
        [SetUp]
        public void SetUp()
        {
            Debug.Log("Setting up");
            Common.Database.Setup();
        }

        private static readonly ReviewData[] reviewData;

    /*    [UnityTest]
        IEnumerable TestReviewPublishing([ValueSource("reviewData")] ReviewData data)
        {
            Task<List<Recipe>> recipeTask = FirebaseDatabase.DefaultInstance.GetReference("recipe").GetValueAsync()
                .WithSuccess(data => data.Children.ToList().Select(recipeData => JsonUtility.FromJson<Recipe>(recipeData.GetRawJsonValue())))
            await ;
               
            ReviewService.Instance.SubmitReview("","");

            return 0;
        } */
    }

    public class Review
    {
        [SetUp]
        public void SetUp()
        {
            Debug.Log("Setting up");
            Common.Database.Setup();
        }

        [UnityTest]
        public IEnumerator AddReview()
        {
            var content = "This recipe is great";

            Common.Auth.EnsureUserExists(true, Constants.TEST_EMAIL, Constants.TEST_PASSWORD);
            var userid = LoginService.Instance.User.UserId;
            var task = ReviewService.Instance.SubmitReview("0", content);
            yield return task.AsCoroutine();
            Assert.AreEqual(TaskStatus.RanToCompletion, task.Status);
            Assert.AreNotEqual(TaskStatus.Faulted, task.Status);
            var validation = FirebaseDatabase.DefaultInstance.GetReference("reviews").Child("0").Child(userid).GetValueAsync().AsCoroutine();
            yield return validation;
            var data = validation.Result;
            Assert.IsTrue(validation.IsCompleted);
            Assert.IsFalse(validation.IsFaulted);

            Assert.IsTrue(data.HasChild("content"));
            Assert.IsTrue(data.HasChild("timestamp"));
            Assert.AreEqual(data.Child("content").GetValue(false), content);


        }
    }

    class ReviewData
    {
        public int recipe = 0;
        public string content = "This is a review";

    }
}