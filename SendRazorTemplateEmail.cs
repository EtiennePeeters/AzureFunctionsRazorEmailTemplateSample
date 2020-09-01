using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.Threading;
using AzureFunctionsRazorEmailTemplateSample.Views;

namespace AzureFunctionsRazorEmailTemplateSample.Function
{
    public class SendRazorTemplateEmail
    {
        private readonly RazorViewToStringRenderer razorViewToStringRenderer;

        public SendRazorTemplateEmail(RazorViewToStringRenderer razorViewToStringRenderer)
        {
            this.razorViewToStringRenderer = razorViewToStringRenderer;
        }

        [FunctionName("SendRazorTemplateEmail")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [SendGrid] IAsyncCollector<SendGridMessage> messageCollector, 
            ILogger log, CancellationToken cancellationToken)
        {
            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            var message = new SendGridMessage();

            message.SetFrom("from@example.com");
            message.AddTo("to@example.com");
            message.SetSubject("Subject");

            var model = new SampleEmailTemplateModel()
            {
                Name = name
            };
            var htmlContent = await razorViewToStringRenderer.RenderViewToStringAsync(model);
            message.AddContent(MimeType.Html, htmlContent);

            await messageCollector.AddAsync(message, cancellationToken);

            log.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult(htmlContent);
        }
    }
}
