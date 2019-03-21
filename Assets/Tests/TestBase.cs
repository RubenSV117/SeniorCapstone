using NUnit.Framework;
using System;
using System.Collections;
using System.Threading.Tasks;
using Tests.Util;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class Constants
    {
        public static readonly string TEST_EMAIL = "TestEmail@FakeDomain.com";
        public static readonly string TEST_PASSWORD = "NotARealPassword";

        public static readonly string TEST_EMAIL_OTHER = "OtherTestEmail@FakeDomain.com";
        public static readonly string TEST_PASSWORD_OTHER = "DefinitelyNotARealPassword";
    }
    
    [TestFixture]
    public class BaseTest
    {
        private Func<Task> setup;
        private Func<Task> teardown;

        protected CustomYieldInstruction PreCondition(Func<Task> condition)
        {
            Task t = condition();
            return new WaitWhile(() => t.IsRunning());
        }

        protected void SetUp(Func<Task> setupTask)
        {
            if (setup != null) {
                setup = () =>
                {
                    Task t1 = setup();
                    Task t2 = setupTask();
                    return Task.WhenAll(t1, t2);
                };
            } else
            {
                setup = setupTask;
            }
        }

        protected void TearDown(Func<Task> teardownTask)
        {
            if (teardown != null)
            {
                teardown = () =>
                {
                    Task t1 = teardown();
                    Task t2 = teardownTask();
                    return Task.WhenAll(t1, t2);
                };
            }
            else
            {
                teardown = teardownTask;
            }
        }

        protected void SetUp(Action setupAction)
        {
            if (setup != null)
            {
                setup = () =>
                {
                    Task t1 = setup();
                    Task t2 = Task.Run(setupAction);
                    return t1;
                };
            }
            else
            {
                setup = () => Task.Run(setupAction);
            }
        }

        protected void TearDown(Action teardownAction)
        {
            if (teardown != null)
            {
                teardown = () =>
                {
                    Task t1 = teardown();
                    Task t2 = Task.Run(teardownAction);
                    return Task.WhenAll(t1, t2);
                };
            }
            else
            {
                teardown = () => Task.Run(teardownAction);
            }
        }

        [UnitySetUp]
        public IEnumerator BaseSetUp()
        {
            if (setup != null)
            {
                Task t = setup();
                while (t.IsRunning())
                    yield return null;
            }
        }

        [UnityTearDown]
        public IEnumerator BaseTearDown()
        {
            if (teardown != null)
            {
                Task t = teardown();
                while (t.IsRunning())
                    yield return null;
            }
        }

    }
}
