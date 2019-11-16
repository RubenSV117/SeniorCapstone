//using UnityEngine;
//using NUnit.Framework;
//using Firebase.Database;
//using System.Linq;
//using UnityEngine.TestTools;
//using System.Collections;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//
//namespace Tests
//{
//    public class ReviewUI
//    {
//        bool onetime = false;
//
//        [UnitySetUp]
//        public IEnumerator SetUp()
//        {
//            if (!onetime)
//            {
//                Debug.Log("Already Set Up!");
//            } else
//            {
//
//                onetime = true;
//                Debug.Log("Setting up");
//                yield return Common.Database.Setup();
//            }
//            yield return 0;
//        }
//
//        private static readonly ReviewData[] reviewData;
//
//    /*    [UnityTest]
//        IEnumerable TestReviewPublishing([ValueSource("reviewData")] ReviewData data)
//        {
//            Task<List<Recipe>> recipeTask = FirebaseDatabase.DefaultInstance.GetReference("recipe").GetValueAsync()
//                .WithSuccess(data => data.Children.ToList().Select(recipeData => JsonUtility.FromJson<Recipe>(recipeData.GetRawJsonValue())))
//            await ;
//               
//            ReviewService.Instance.SubmitReview("","");
//
//            return 0;
//        } */
//    }
//
//    public class ReviewApi
//    {
//        bool onetime = false;
//
//        [UnitySetUp]
//        public IEnumerator SetUp()
//        {
//            if (onetime)
//            {
//                Debug.Log("Already Set Up!");
//            }
//            else
//            {
//
//                onetime = true;
//                Debug.Log("Setting up");
//                yield return Common.Database.Setup();
//            }
//            yield return 0;
//        }
//
//        [UnityTest]
//        public IEnumerator AddReview()
//        {
//            var content = "This recipe is great";
//
//            Common.Auth.EnsureUserExists(true, Constants.TEST_EMAIL, Constants.TEST_PASSWORD);
//            var userid = LoginService.Instance.User.UserId;
//            var task = ReviewService.Instance.SubmitReview("0", content);
//            yield return task.AsCoroutine();
//            Assert.AreEqual(TaskStatus.RanToCompletion, task.Status);
//            Assert.AreNotEqual(TaskStatus.Faulted, task.Status);
//            var validation = FirebaseDatabase.DefaultInstance.GetReference("reviews").Child("0").Child(userid).GetValueAsync().AsCoroutine();
//            yield return validation;
//            var data = validation.Result;
//            Assert.IsTrue(validation.IsCompleted);
//            Assert.IsFalse(validation.IsFaulted);
//
//            Assert.IsTrue(data.HasChild("content"));
//            Assert.IsTrue(data.HasChild("timestamp"));
//            Assert.AreEqual(data.Child("content").GetValue(false), content);
//            yield return 0;
//        }
//
//        [UnityTest]
//        public IEnumerator GetReview()
//        {
//            /* Common.Auth.EnsureUserExists(true, Constants.TEST_EMAIL, Constants.TEST_PASSWORD);
//            var review = Constants.REVIEWS[0];
//            var task = ReviewService.Instance.GetReviews("0");
//            yield return task.AsCoroutine();
//            Assert.AreEqual(TaskStatus.RanToCompletion, task.Status);
//            Assert.AreNotEqual(TaskStatus.Faulted, task.Status);
//
//            var list = task.Result;
//            Assert.NotNull(list);
//            Assert.AreEqual(1, list.Count, "Expected 1 review");
//
//            var actual = list[0];
//            Assert.NotNull(actual);
//            Assert.Equals(review.userId, actual.userId);
//            Assert.Equals(review.content, actual.content);
//            Assert.NotNull(review.timestamp); */
//            yield return 0;
//
//        }
//    }
//
//    class ReviewData
//    {
//        public int recipe = 0;
//        public string content = "This is a review";
//
//    }
//}