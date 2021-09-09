using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace DotNet
{
    public static class DurableFunctions
    {
        [FunctionName("DurableFunctions")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>
            {

                // Replace "hello" with the name of your Durable Activity Function.
                await context.CallActivityAsync<string>("DurableFunctions_Hello", "Tokyo"),
                await context.CallActivityAsync<string>("DurableFunctions_Hello", "Seattle"),
                await context.CallActivityAsync<string>("DurableFunctions_Hello", "London")
            };

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName("DurableFunctions_Hello")]
#pragma warning disable CS0618 // Type or member is obsolete
        public static string SayHello([ActivityTrigger] string name, TraceWriter log)
#pragma warning restore CS0618 // Type or member is obsolete
        {
            log.Info($"Saying hello to {name}.");
            return $"Hello {name}!";
        }

        [FunctionName("DurableFunctions_HttpStart")]
        [System.Obsolete]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
#pragma warning disable CS0618 // Type or member is obsolete
            [OrchestrationClient]DurableOrchestrationClientBase starter,
#pragma warning restore CS0618 // Type or member is obsolete
            TraceWriter log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("DurableFunctions", null);

            log.Info($"Started orchestration with ID = '{instanceId}'.");
            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}