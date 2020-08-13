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

        // This is a newer concept in .NET Core.  There is this concept of Providers.  There are file providers, logging providers, etc.
        // Most of the documentation shows these being set in the Controllers initializer.

        public HandleJSONController(ILogger<HandleJSONController> _logger)
        {
            logger = _logger;
        }

        // GET: /<controller>/
        [HttpGet]
        public IActionResult Index()
        {
            // return View();
            logger.LogInformation("Starting Error Logging", null);
            Guid guid = Guid.NewGuid();
            var guidString = Convert.ToString(guid);

            return Ok("Index Return Value: " + guidString);
        }

        [HttpPost]
        public IActionResult PostJSON() 
        {
            logger.LogInformation("Starting PostJSON Logging", null);
            var requestBody = HttpContext.Request.Body;
            var requestLength = HttpContext.Request.ContentLength;
            string jsonContent = getJSONContent(requestBody);
            logger.LogInformation(jsonContent);

            BotUpdateRequest allRequests = new BotUpdateRequest();
            Result result = new Result();
            UpdateResultHandler updateResultHandler = new UpdateResultHandler();
            if (jsonContent.Contains("result"))
            {
                JsonConvert.PopulateObject(jsonContent, allRequests);
                foreach (Result currentResult in allRequests.result)
                {
                    var currentMessage = currentResult.message;
                    updateResultHandler.ProcessUpdateRequest(currentMessage);
                }
            }
            else
            {
                JsonConvert.PopulateObject(jsonContent, result);
                updateResultHandler.ProcessUpdateRequest(result.message);
            }

            return Ok("Hello From Chuckbot");
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
                // jsonString = getJSONFromFile();
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

        string getJSONContent(Stream streamBody)
        {
            string returnString = String.Empty;
            var bodyStream = new StreamReader(streamBody);
            try
            {
                returnString = bodyStream.ReadToEnd();
            }
            catch (Exception ex)
            {
                string exception = ex.Message;
            }
            finally
            {
                if (bodyStream != null)
                {
                    bodyStream.Close();
                }
            }

            return returnString;
        }

        // TODO: This is a debug function that allows me to pull a predefined JSON file.  Need to find out how to get the physical path.
        string getJSONFromFile()
        {
            string jsonToReturn = "No file to return";
            /*
            IFileProvider fileProvider = new PhysicalFileProvider(rootPath);
            IDirectoryContents files = fileProvider.GetDirectoryContents("wwwroot");
            IFileInfo fileInfo = fileProvider.GetFileInfo("wwwroot/sampleUpdate.json");

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
            */
            return jsonToReturn;
        }
    }
}
