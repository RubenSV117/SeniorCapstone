using Firebase;
using Firebase.Auth;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static Tests.Constants;

namespace Tests
{
    public class UserStory7 : UITest
    {   
        [UnityTest]
        public IEnumerator PublishRecipeWithValidInputs()
        {
            var data = new TestData();
            data.Name = TEST_RECIPE_NAME;
            data.Calories = TEST_RECIPE_CALORIES;
            data.Minutes = TEST_RECIPE_MINUTES;
            data.IngredientsList = TEST_RECIPE_INGREDIENTS.ToList();
            data.Directions = TEST_RECIPE_DIRECTIONS.ToList();
            data.Tags = TEST_RECIPE_TAGS.ToList();
            data.NotificationText = "Recipe Uploaded";
            yield return RunPublishRecipeTest(data);
        }

        [UnityTest]
        public IEnumerator PublishRecipeWithoutImage()
        {
            var data = new TestData();
            var err = "An image must be provided.";
            data.Name = TEST_RECIPE_NAME;
            data.Calories = TEST_RECIPE_CALORIES;
            data.Minutes = TEST_RECIPE_MINUTES;
            data.IngredientsList = TEST_RECIPE_INGREDIENTS.ToList();
            data.Directions = TEST_RECIPE_DIRECTIONS.ToList();
            data.ErrorCode = 404;
            data.ErrorMsg = err;
            yield return RunPublishRecipeTest(data);
        }

        [UnityTest]
        public IEnumerator PublishRecipeWithoutIngredients()
        {
            var data = new TestData();
            var err = "At least one ingredient must be provided.";
            data.Name = TEST_RECIPE_NAME;
            data.Calories = TEST_RECIPE_CALORIES;
            data.Minutes = TEST_RECIPE_MINUTES;
            data.Directions = TEST_RECIPE_DIRECTIONS.ToList();
            data.ErrorCode = 404;
            data.ErrorMsg = err;
            yield return RunPublishRecipeTest(data);
        }

        [UnityTest]
        public IEnumerator PublishRecipeWithoutSteps()
        {
            var data = new TestData();
            var err = "At least one step must be provided.";
            data.Name = TEST_RECIPE_NAME;
            data.Calories = TEST_RECIPE_CALORIES;
            data.Minutes = TEST_RECIPE_MINUTES;
            data.IngredientsList = TEST_RECIPE_INGREDIENTS.ToList();
            data.ErrorCode = 404;
            data.ErrorMsg = err;
            yield return RunPublishRecipeTest(data);
        }

        internal IEnumerator RunPublishRecipeTest(TestData data)
        {
            yield return LoadScene("ReGen");

            yield return WaitFor(new ObjectAppeared<LoginManagerUI>());
            yield return WaitFor(new ObjectAppeared("SkipButton"));

            yield return Press("SkipButton");

            yield return WaitFor(new ObjectAppeared<MainMenuManagerUI>());
            yield return WaitFor(new ObjectAppeared("NavigationBar"));

            yield return Press("PublishButton");

            yield return WaitFor(new ObjectAppeared<PublishingManagerUI>());
            yield return WaitFor(new ObjectAppeared("PublishButton"));

            yield return Press("AddImageButton");
            yield return Press("CameraRollButton");
            yield return TypeInto("NameInputField", data.Name);
            yield return TypeInto("CaloriesInputField", data.Calories);
            yield return TypeInto("PrepTimeInputField", data.Minutes);
            foreach (string k in data.IngredientsList)
            {
                string[] temp = k.Split(' ');
                string ingrAmount = temp[0] + ' ' + temp[1];
                string ingrName = temp[2];
                yield return TypeInto("AmountInputField", ingrAmount);
                yield return TypeInto("IngredientBuilder/NameInputField", ingrName);
                yield return Press("IngredientBuilder/AddButton");              
            }
            foreach (string k in data.Directions)
            {
                yield return TypeInto("InputField", k);
                yield return Press("DirectionBuilder/AddButton");              
            }
            foreach(string k in data.Tags)
            {
                if (string.Compare(k, "vegetarian") == 0)
                    yield return Press("VegetarianToggle/Background/Checkmark");
                else if (string.Compare(k, "vegan") == 0)
                    yield return Press("VeganToggle/Background/Checkmark");
                else if (string.Compare(k, "glutenFree") == 0)
                    yield return Press("GlutenFreeToggle/Background/Checkmark");
                else if (string.Compare(k, "dairyFree") == 0)
                    yield return Press("DairyFreeToggle/Background/Checkmark");
                else if (string.Compare(k, "ketogenic") == 0)
                    yield return Press("KetogenicToggle/Background/Checkmark");
            }

            //yield return Press("PublishButton");
        }
    
        internal class TestData
        {
            public string Name = "";
            public string Calories = "";
            public string Minutes = "";
            public List<string> IngredientsList;
            public List<string> Directions;
            public List<string> Tags;
            public string NotificationText;
            public int ErrorCode = 0;
            public string ErrorMsg;

            
        }
    }
}
