//using Firebase;
//using Firebase.Auth;
//using Moq;
//using NUnit.Framework;
//using System;
//using System.Linq;
//using System.Text;
//using System.Collections;
//using System.Threading.Tasks;
//using UnityEngine;
//using UnityEngine.TestTools;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using static Tests.Constants;
//
//namespace Tests
//{
//    public class UserStory7 : UITest
//    {
//        bool onetime = false;
//
//        [UnitySetUp]
//        public IEnumerator SetUp()
//        {
//            if (!onetime)
//            {
//                Debug.Log("Already Set Up!");
//            }
//            else
//            {
//                onetime = true;
//                Debug.Log("Setting up");
//                yield return Common.Database.Setup();
//            }
//            FirebaseAuth.DefaultInstance.SignOut();
//            yield return 0;
//        }
//
//
//        [UnityTest]
//        public IEnumerator PublishRecipeWithValidInputs()
//        {
//            var data = new TestData();
//            data.Name = TEST_RECIPE_NAME;
//            data.Calories = TEST_RECIPE_CALORIES;
//            data.Minutes = TEST_RECIPE_MINUTES;
//            data.IngredientsList = TEST_RECIPE_INGREDIENTS.ToList();
//            data.Directions = TEST_RECIPE_DIRECTIONS.ToList();
//            data.Tags = TEST_RECIPE_TAGS.ToList();
//            data.Message = "Publish Successful";
//            yield return RunPublishRecipeTest(data);
//        }
//
//        [UnityTest]
//        public IEnumerator PublishRecipeWithoutImage()
//        {
//            var data = new TestData();
//            var err = "An image must be set for the recipe.";
//            data.HasImage = false;
//            data.Name = TEST_RECIPE_NAME;
//            data.Calories = TEST_RECIPE_CALORIES;
//            data.Minutes = TEST_RECIPE_MINUTES;
//            data.IngredientsList = TEST_RECIPE_INGREDIENTS.ToList();
//            data.Directions = TEST_RECIPE_DIRECTIONS.ToList();
//            data.Message = err;
//            yield return RunPublishRecipeTest(data);
//        }
//
//        [UnityTest]
//        public IEnumerator PublishRecipeWithoutIngredients()
//        {
//            var data = new TestData();
//            var err = "Ingredients must be set for the recipe.";
//            data.Name = TEST_RECIPE_NAME;
//            data.Calories = TEST_RECIPE_CALORIES;
//            data.Minutes = TEST_RECIPE_MINUTES;
//            data.Directions = TEST_RECIPE_DIRECTIONS.ToList();
//            data.Message = err;
//            yield return RunPublishRecipeTest(data);
//        }
//
//        [UnityTest]
//        public IEnumerator PublishRecipeWithoutSteps()
//        {
//            var data = new TestData();
//            var err = "Directions must be set for the recipe.";
//            data.Name = TEST_RECIPE_NAME;
//            data.Calories = TEST_RECIPE_CALORIES;
//            data.Minutes = TEST_RECIPE_MINUTES;
//            data.IngredientsList = TEST_RECIPE_INGREDIENTS.ToList();
//            data.Message = err;
//            yield return RunPublishRecipeTest(data);
//        }
//
//        internal IEnumerator RunPublishRecipeTest(TestData data)
//        {
//            yield return LoadScene("ReGen");
//
//            yield return WaitFor(new ObjectAppeared<LoginManagerUI>());
//            yield return WaitFor(new ObjectAppeared("SkipButton"));
//
//            yield return Press("SkipButton");
//
//            yield return WaitFor(new ObjectAppeared<MainMenuManagerUI>());
//            yield return WaitFor(new ObjectAppeared("NavigationBar"));
//
//            yield return Press("PublishButton");
//
//            yield return WaitFor(new ObjectAppeared<PublishingManagerUI>());
//            yield return WaitFor(new ObjectAppeared("PublishButton"));
//
//
//            if (data.HasImage)
//            {
//                //yield return Press("AddImageButton");
//                //yield return Press("CameraRollButton");
//
//                TestImages.CameraPickBlackTexture();
//            }
//
//
//            yield return TypeInto("NameInputField", data.Name);
//            yield return TypeInto("CaloriesInputField", data.Calories);
//            yield return TypeInto("PrepTimeInputField", data.Minutes);
//            for(int i = 0; i < data.IngredientsList.ToList().Count(); i++)
//            {
//                string[] temp = data.IngredientsList.ToList()[i].Split(' ');
//                string ingrAmount = temp[0] + ' ' + temp[1];
//                string ingrName = temp[2];
//                int objIndex = 0;
//                var parent = "IngredientBuilder";
//                if (i != 0)
//                    parent += "(Clone)";
//                else
//                    objIndex -= 1;
//                yield return TypeInto($"{parent}/AmountInputField", ingrAmount);
//                yield return TypeInto($"{parent}/NameInputField", ingrName);
//            }
//        
//            foreach (string k in data.Directions)
//            {
//                yield return TypeInto("DirectionBuilder/InputField", k);              
//            }
//            foreach(string k in data.Tags)
//            {
//                if (string.Compare(k, "vegetarian") == 0)
//                    yield return Press("VegetarianToggle/Background/Checkmark");
//                else if (string.Compare(k, "vegan") == 0)
//                    yield return Press("VeganToggle/Background/Checkmark");
//                else if (string.Compare(k, "glutenFree") == 0)
//                    yield return Press("GlutenFreeToggle/Background/Checkmark");
//                else if (string.Compare(k, "dairyFree") == 0)
//                    yield return Press("DairyFreeToggle");
//                else if (string.Compare(k, "ketogenic") == 0)
//                    yield return Press("KetogenicToggle/Background/Checkmark");
//            }
//
//
//            yield return Press("PublishingManager/Canvas/RecipePanel/Scroll View/Viewport/Content/PublishButton");
//            //PublishingManagerUI.Instance.BuildRecipe();
//
//            yield return WaitFor(new ObjectAppeared("NotificationManager/Canvas/PopUpPanel"));
//            yield return AssertLabel("NotificationManager/Canvas/PopUpPanel/Text", data.Message);
//
//
//        }
//    
//        internal class TestData
//        {
//            public string Name = "";
//            public string Calories = "";
//            public string Minutes = "";
//            public List<string> IngredientsList = new List<string>();
//            public List<string> Directions = new List<string>();
//            public List<string> Tags = new List<string>() ;
//            public bool HasImage = true;
//            public string Message;
//
//            
//        }
//    }
//}
