using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace DotNet.Test
{
    [TestClass]
    public class DurableFunctionTest : FunctionTestHelper.FunctionTest
    {
        [TestMethod]
        public async Task Run_Orchectrator()
        {
            var contextMock = GetContextMock();
            contextMock.Setup(context => context.CallActivityAsync<string>("DurableFunctions_Hello", "Tokyo")).Returns(Task.FromResult<string>("Hello Tokyo!"));
            contextMock.Setup(context => context.CallActivityAsync<string>("DurableFunctions_Hello", "Seattle")).Returns(Task.FromResult<string>("Hello Seattle!"));
            contextMock.Setup(context => context.CallActivityAsync<string>("DurableFunctions_Hello", "London")).Returns(Task.FromResult<string>("Hello London!"));
            var result = await DotNet.DurableFunctions.RunOrchestrator(contextMock.Object);
            Assert.AreEqual("Hello Tokyo!", result[0]);
            Assert.AreEqual("Hello Seattle!", result[1]);
            Assert.AreEqual("Hello London!", result[2]);
        }

#pragma warning disable CS0618 // Type or member is obsolete
        private static Mock<DurableOrchestrationContextBase> GetContextMock()
#pragma warning restore CS0618 // Type or member is obsolete
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return new Mock<DurableOrchestrationContextBase>();
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [TestMethod]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task Run_Hello_Activity()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var result = DotNet.DurableFunctions.SayHello("Amsterdam", log);
            Assert.AreEqual("Hello Amsterdam!", result);
        }

        [TestMethod]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task Run_Orchectrator_Client()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var clientMock = new Mock<DurableOrchestrationClientBase>();
#pragma warning restore CS0618 // Type or member is obsolete
                              // https://github.com/Azure/azure-functions-durable-extension/blob/0345b369ffa1745c24ffbacfaf8a43fb62dd2572/src/WebJobs.Extensions.DurableTask/DurableOrchestrationClient.cs#L46
            var requestMock = new Mock<HttpRequestMessage>();
            var id = "8e503c5e-19de-40e1-932d-298c4263115b";
            clientMock.Setup(client => client.StartNewAsync("DurableFunctions", null)).Returns(Task.FromResult<string>(id));
            var request = requestMock.Object;
            _ = clientMock.Setup(client => NewMethod(client, id, request));
#pragma warning disable CS0612 // Type or member is obsolete
            var result = DurableFunctions.HttpStart(request, clientMock.Object, log);
#pragma warning restore CS0612 // Type or member is obsolete
            try
            {

                clientMock.Verify(client => client.StartNewAsync("DurableFunctions", null));
                clientMock.Verify(client => NewMethod1(client, id, request));
            }
            catch (MockException)
            {
                Assert.Fail();
            }
        }

#pragma warning disable CS0618 // Type or member is obsolete
        private static HttpResponseMessage NewMethod1(DurableOrchestrationClientBase client, string id, HttpRequestMessage request)
#pragma warning restore CS0618 // Type or member is obsolete
        {
            return client.CreateCheckStatusResponse(request, id);
        }

#pragma warning disable CS0618 // Type or member is obsolete
        private static HttpResponseMessage NewMethod(DurableOrchestrationClientBase client, string id, HttpRequestMessage request)
#pragma warning restore CS0618 // Type or member is obsolete
        {
            return client.CreateCheckStatusResponse(request, id);
        }
    }
}
