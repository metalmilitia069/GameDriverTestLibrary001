using System;
using System.Diagnostics;
using NUnit.Framework;
using gdio.unity_api;
using gdio.unity_api.v2;
using gdio.common.objects;

namespace ClassLibrary1
{
    [TestFixture]
    public class Class1
    {

        //These parameters can be used to override settings used to test when running from the NUnit command line
        public string testMode = TestContext.Parameters.Get("Mode", "IDE");
        //public string pathToExe = TestContext.Parameters.Get("pathToExe", "L:\\Unity Projects\\GameDriverTest001\\GameDriverTest001\\GameDriverTest001Build001"); // replace null with the path to your executable as needed
        public string pathToExe = TestContext.Parameters.Get("pathToExe", null); // replace null with the path to your executable as needed

        public const string ELEMENTS_LIB = "//*[@name='GameDriverManager']/fn:component('ElementLib')";

        ApiClient api;

        [OneTimeSetUp]
        public void Connect()
        {
            try
            {
                // First we need to create an instance of the ApiClient
                api = new ApiClient();

                // If an executable path was supplied, we will launch the standalone game
                if (pathToExe != null)
                {
                    ApiClient.Launch(pathToExe);
                    api.Connect("localhost", 19734, false, 30);
                }

                // If no executable path was given, we will attempt to connect to the Unity editor and initiate Play mode
                else if (testMode == "IDE")
                {
                    api.Connect("localhost", 19734, true, 30);
                }
                // Otherwise, attempt to connect to an already playing game
                else api.Connect("localhost", 19734, false, 30);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            // Enable input hooking
            api.EnableHooks(HookingObject.ALL);

            var buttonName = api.GetObjectFieldValue<string>(ELEMENTS_LIB, "MESSAGE_BUTTON_ID", 30);
            api.WaitForObject(buttonName);
            api.ClickObject(MouseButtons.LEFT, buttonName, 30);

            api.Wait(3000);
        }

        //[Test]
        //public void Test1()
        //{
        //    var buttonName = api.GetObjectFieldValue<string>(ELEMENTS_LIB, "MESSAGE_BUTTON_ID", 30);
        //    api.WaitForObject(buttonName);
        //    api.ClickObject(MouseButtons.LEFT, buttonName, 10);
        //}

        [Test]
        public void Test2()
        {
            var messageID = api.GetObjectFieldValue<string>(ELEMENTS_LIB, "MESSAGE_TEXT_ID", 30);
            var messageName = api.GetObjectFieldValue<string>(messageID + "/fn:component('UnityEngine.Behaviour')", "text", 30);
            Console.WriteLine(messageName);
        }

        [OneTimeTearDown]
        public void Disconnect()
        {
            // Disconnect the GameDriver client from the agent
            api.DisableHooks(HookingObject.ALL);
            api.Wait(2000);
            api.Disconnect();
            api.Wait(2000);
        }
    }


}

