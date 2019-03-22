using Firebase;
using Firebase.Auth;
using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;
using static Tests.Common.Auth;
using static Tests.Constants;

namespace Tests
{
    public class UserStory1 : UITest
    {   
        private Mock<ILoginService> InjectMockLoginService()
        {
            var f = typeof(LoginService).GetField("lazy", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            var m = new Mock<ILoginService>();
            f.SetValue(null, new Lazy<ILoginService>(() => m.Object));
            return m;
        }


        [UnityTest]
        public IEnumerator CreateAccountWithValidEmailAndValidPassword()
        {
            var data = new TestData();
            data.Email = TEST_EMAIL;
            data.Password = TEST_PASSWORD;
            data.NotificationText = "Account registered";
            yield return RunCreateEmailAccountTest(data);
        }

        [UnityTest]
        public IEnumerator CreateAccountWithInvalidEmailAndValidPassword()
        {
            var data = new TestData();
            var err = "The email address is badly formatted.";
            data.Email = "not!an?email/address";
            data.Password = TEST_PASSWORD;
            data.ErrorCode = 11;
            data.ErrorMsg = err;
            data.NotificationText = err;
            yield return RunCreateEmailAccountTest(data);
        }

        [UnityTest]
        public IEnumerator CreateAccountWithValidEmailAndInvalidPassword()
        {
            var data = new TestData();
            var err = "The given password is invalid.";
            data.Email = TEST_EMAIL;
            data.Password = "weak";
            data.ErrorCode = 23;
            data.ErrorMsg = err;
            data.NotificationText = err;
            yield return RunCreateEmailAccountTest(data);
        }

        [UnityTest]
        public IEnumerator CreateAccountWithBlankEmailAndBlankPassword()
        {
            var data = new TestData();
            var err = "An email address must be provided.";
            data.ErrorCode = 37;
            data.ErrorMsg = err;
            data.NotificationText = err;
            yield return RunCreateEmailAccountTest(data);
        }

        [UnityTest]
        public IEnumerator CreateAccountWithValidEmailAndBlankPassword()
        {
            var data = new TestData();
            var err = "A password must be provided.";
            data.Email = TEST_EMAIL;
            data.ErrorCode = 38;
            data.ErrorMsg = err;
            data.NotificationText = err;
            yield return RunCreateEmailAccountTest(data);
        }

        [UnityTest]
        public IEnumerator CreateAccountWithValidEmailAndMisMatchedPassword()
        {
            var data = new TestData();
            var err = "Confirmation password must match first password.";
            data.Email = TEST_EMAIL;
            data.Password = TEST_PASSWORD;
            data.ConfirmPassword = TEST_PASSWORD + "x";
            data.LoginServiceCalled = false;
            data.NotificationText = err;
            yield return RunCreateEmailAccountTest(data);
        }
        
        internal IEnumerator RunCreateEmailAccountTest(TestData data)
        {
            var m = InjectMockLoginService();
            Assert.AreEqual(m.Object, LoginService.Instance);

            m.Setup(o => o.RegisterUserWithEmail(data.Email, data.Password))
                .Returns((string email, string pass) =>
                {
                    return Task.Delay(1000).ContinueWith<FirebaseUser>(data.RegisterTaskResult);
                });
            //yield return EnsureTestUserDoesNotExist();

            yield return LoadScene("ReGen");

            yield return WaitFor(new ObjectAppeared<LoginManagerUI>());
            yield return WaitFor(new ObjectAppeared("InitialOptionsGroup"));

            yield return Press("SignUpOptionButton");

            yield return WaitFor(new ObjectAppeared("SignupOptionsGroup"));

            yield return Press("EmailSignupOptionButton");

            yield return TypeInto("EmailInputField", data.Email);
            yield return TypeInto("EmailRegisterGroup/PasswordField", data.Password);
            yield return TypeInto("ConfirmPasswordField", data.ConfirmPassword ?? data.Password);

            yield return Press("EmailRegisterButton");
            if (data.LoginServiceCalled)
            {
                yield return WaitFor(new ObjectAppeared("NotificationManager/Canvas/LoadingPanel"));
                yield return WaitFor(new ObjectDisappeared("NotificationManager/Canvas/LoadingPanel"));
            }
            yield return WaitFor(new ObjectAppeared("NotificationManager/Canvas/PopUpPanel"));
            yield return AssertLabel("NotificationManager/Canvas/PopUpPanel/Text", data.NotificationText);

            if (data.LoginServiceCalled)
            {
                m.Verify(o => o.RegisterUserWithEmail(data.Email, data.Password), Times.Once);
            }
            m.VerifyNoOtherCalls();
        }
    
        internal class TestData
        {
            public string Email = "";
            public string Password = "";
            public string ConfirmPassword;
            public Func<Task, FirebaseUser> RegisterTaskResult;
            public string NotificationText;
            public bool LoginServiceCalled = true;
            public int ErrorCode = 0;
            public string ErrorMsg;

            public TestData()
            {
                RegisterTaskResult = t =>
                {
                    if (ErrorMsg != null)
                    {
                        throw new FirebaseException(ErrorCode, ErrorMsg);
                    }
                    return null;
                };
            }
        }
    }
}
