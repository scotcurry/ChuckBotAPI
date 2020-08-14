using RestSharp;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace ChuckBotAPI.Classes
{
    public class ChuckJokeHandler
    {
      
        static string chuckJokeURL = "https://api.icndb.com";
        ILogger logger;

        public enum JokeType
        {
            Any,
            Nerdy,
            Explicit
        }

        public string GetChuckJoke()
        {
            buildLogger();
            logger.LogError("Starting UpdateResultHandler");
            // log.Info("Processing Generic Chuck Joke");
            return getJokeWithOptions(null, JokeType.Any);
        }

        public string GetChuckJoke(string replacementText)
        {
            buildLogger();
            replacementText = replacementText.Trim();
            if (replacementText.ToLower() == "victor" || replacementText.ToLower() == "victor gonzalez" || replacementText.ToLower() == "neha")
                getJokeWithOptions(replacementText, JokeType.Nerdy);
            if (replacementText.ToLower() == "eric" || replacementText.ToLower() == "eric szewczyk" || replacementText.ToLower() == "jen" || replacementText.ToLower() == "jenifer"
               || replacementText.ToLower() == "jennifer")
                getJokeWithOptions(replacementText, JokeType.Nerdy);
            else
                getJokeWithOptions(replacementText, JokeType.Any);

            string messageText = "Processing Chuck Joke with Replacement Text: " + replacementText;
            logger.LogInformation(messageText);
            return getJokeWithOptions(replacementText, JokeType.Any);
        }

        public string GetJokeWithReplacement(string replacementText, string jokeText)
        {
            jokeText = jokeText.Replace("Chuck Norris", replacementText);
            return jokeText;
        }

        private string getJokeWithOptions(string replacementText, JokeType jokeType)
        {
            string jokeToReturn = string.Empty;
            RestClient restClient = new RestClient(chuckJokeURL);
            string initialJokeEndpoint = "jokes/random";
            if ((int)jokeType == (int)(JokeType.Any))
                initialJokeEndpoint = "jokes/random";
            if ((int)jokeType == (int)JokeType.Nerdy)
                initialJokeEndpoint += "jokes/random?limitTo=[nerdy]";
            if ((int)jokeType == (int)JokeType.Explicit)
                initialJokeEndpoint += "jokes/random?limitTo=[explicit]";

            logger.LogInformation("Joke Endpoint: " + initialJokeEndpoint);
            RestRequest request = new RestRequest(initialJokeEndpoint, Method.GET);
            IRestResponse response = restClient.Execute(request);

            string jokeText = string.Empty;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                logger.LogInformation("Return from ChuckDB = 200 OK");
                ChuckJokeInfo jokeInfo = new ChuckJokeInfo();
                JsonConvert.PopulateObject(response.Content, jokeInfo);
                jokeText = jokeInfo.value.joke;
            }
            else
            {
                jokeText = "Something went wrong. Chuck is going to kick someone's ass";
            }

            if (replacementText != null)
            {
                jokeText = GetJokeWithReplacement(replacementText, jokeText);
            }

            if (jokeText.Contains("&quot;"))
                jokeText = HandleJokeWithQuote(jokeText);

            return jokeText;
        }

        // This is public so I can run a test case against it.
        public string HandleJokeWithQuote(string jokeWithQuote)
        {
            char quoteChar = '"';
            string quoteCharString = quoteChar.ToString();
            string jokeText = jokeWithQuote.Replace("&quot;", quoteCharString);

            return jokeText;
        }

        private void buildLogger()
        {
            if (logger == null)
            {
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                });
                logger = loggerFactory.CreateLogger("ChuckBotAPI.Classes.ChuckJokeHandler");
            }
        }
    }
}
