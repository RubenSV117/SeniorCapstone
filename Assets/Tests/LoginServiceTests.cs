using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class LoginServiceTest
    {
        private static readonly string TEST_EMAIL = "TestEmail@FakeDomain.com";
        private static readonly string TEST_PASSWORD = "NotARealPassword";


        private LoginService auth;

        [SetUp]
        protected void SetUp()
        {
            auth = new LoginService();
            auth.SignOut();
        }

        [Test]
        public void CreateAccountEmail()
        {
            var handler = new Mock<(typeof(ILoginServiceEventHandler)); 
            auth.RegisterUserWithEmail(TEST_EMAIL, TEST_PASSWORD).WithSuccess(user =>
            {
                Assert.NotNull(user);
            });

            auth.RegisterUserWithEmail(TEST_EMAIL, TEST_PASSWORD).ContinueWith(user =>
            {
                Assert.IsNull(user);
            });
        }

        [Test]
        public void TEST()
        {
            TaskResult<string, Exception> result = new TaskResult<string, Exception>();
            result.is
        }

    }
}
