using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Console;

using Newtonsoft.Json;
using ChuckBotAPI.Classes;

namespace ChuckBotAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HandleJSONController : ControllerBase
    {
        readonly ILogger logger;
        readonly private IWebHostEnvironment environment;

        // This is a newer concept in .NET Core.  There is this concept of Providers.  There are file providers, logging providers, etc.
        // Most of the documentation shows these being set in the Controllers initializer.

        public HandleJSONController(ILogger<HandleJSONController> _logger, IWebHostEnvironment _environment)
        {
            logger = _logger;
            environment = _environment;
        }

        // GET: /<controller>/
        // This is all just debug code to make sure the app is running and logging is working.
        // POSTMAN Command GET:  https://localhost:5001/handlejson/
        [HttpGet]
        public IActionResult Index()
        {
            // return View();
            logger.LogInformation("Starting Error Logging", null);
            Guid guid = Guid.NewGuid();
            var guidString = Convert.ToString(guid);

            return Ok("Index Return Value: " + guidString);
        }

        // This is the endpoint that Telegram expects to send the request to.  This is set with the following POST
        // https://api.telegram.org/bot273811417:AAE_nPFxQ1REjc1Lx20UDVAn-bZfbKsgtIQ/setWebHook with JSON body looking like
        // {"url": "https://cwchuckbotapi.azurewebsites.net/handlejson"}
        [HttpPost]
        public IActionResult PostJSON() 
        {
            logger.LogInformation("Starting PostJSON Logging", null);
            var requestBody = HttpContext.Request.Body;
           
            var requestLength = HttpContext.Request.ContentLength;
            Task<string> jsonContentAsync = getJSONContent(requestBody);
            var jsonContent = jsonContentAsync.Result;
            logger.LogInformation("JSON Content: " + jsonContent);
            logger.LogInformation("Body Length: " + requestLength.ToString());

            BotUpdateRequest allRequests = new BotUpdateRequest();
            Result result = new Result();
            UpdateResultHandler updateResultHandler = new UpdateResultHandler();
            var returnString = "Hello from Chuckbot";
            if (jsonContent.Contains("result"))
            {
                logger.LogInformation("JSON Contains RESULT!  Total Results: " + allRequests.result.Count.ToString());
                JsonConvert.PopulateObject(jsonContent, allRequests);
                foreach (Result currentResult in allRequests.result)
                {
                    var currentMessage = currentResult.message;
                    updateResultHandler.ProcessUpdateRequest(currentMessage);
                }
            }
            else
            {
                try {
                    logger.LogInformation("Simple Update Request - Attempting to Serialize JSON");
                    JsonConvert.PopulateObject(jsonContent, result);
                    updateResultHandler.ProcessUpdateRequest(result.message);
                } catch (JsonException ex)
                {
                    returnString = ex.Message;
                    logger.LogError("Error Serializing SimpleJSON: " + ex.Message);
                }
            }

            return Ok(returnString);
        }

        private BotUpdateRequest getUpdateObject(Stream streamBody)
        {
            BotUpdateRequest result = new BotUpdateRequest();
            var streamBodyLength = streamBody.CanRead;
            logger.LogWarning("CanRead Value: {0}", streamBodyLength);

            var bodyStream = new StreamReader(streamBody);
            string jsonString = String.Empty;
            try
            {
                bodyStream = new StreamReader(streamBody);
                jsonString = bodyStream.ReadToEnd();
                logger.LogWarning("JSON String: {0}", jsonString);
                // Debug: Should be converted to a test
                jsonString = getJSONFromFile();
                JsonConvert.PopulateObject(jsonString, result);
            }
            catch (Exception ex)
            {
                string exception = ex.Message;
                logger.LogWarning("Error Message: {0}", exception);
            }
            finally
            {
                if (bodyStream != null)
                {
                    bodyStream.Close();
                }
            }


            return result;
        }

        async Task<string> getJSONContent(Stream streamBody)
        {
            string returnString = String.Empty;
            var bodyStream = new StreamReader(streamBody);
            try
            {
                returnString = await bodyStream.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                string exception = ex.Message;
                logger.LogInformation("Exception Reading Body: " + ex.Message);
            }
            finally
            {
                if (bodyStream != null)
                {
                    bodyStream.Close();
                }
            }

            // Debug Code: Gets a sample from a file.  Commented out if things are working
            // returnString = getJSONFromFile();
            return returnString;
        }

        // TODO: This is a debug function that allows me to pull a predefined JSON file.  Need to find out how to get the physical path.
        string getJSONFromFile()
        {
            string jsonToReturn = "No file to return";

            string rootPath = environment.ContentRootPath;
            var fileProvider = new PhysicalFileProvider(rootPath);
            IDirectoryContents files = fileProvider.GetDirectoryContents("wwwroot");
            IFileInfo fileInfo = fileProvider.GetFileInfo("sampleUpdate.json");

            if (fileInfo.Exists)
            {
                StreamReader reader = null;
                try
                {
                    reader = new StreamReader(fileInfo.CreateReadStream());
                    jsonToReturn = reader.ReadToEnd();
                }
                catch (Exception ex)
                {
                    jsonToReturn = ex.Message;
                }
                finally
                {
                    reader.Close();
                }
            }
           
            return jsonToReturn;
        }
    }
}
