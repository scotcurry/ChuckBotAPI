using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ChuckBotAPI.Classes
{
    public class UpdateResultHandler
    {
        string telegramRESTAPIKey = "273811417:AAE_nPFxQ1REjc1Lx20UDVAn-bZfbKsgtIQ";
        string telegramBaseURL = "https://api.telegram.org";
        string telegramUpdateEndpoint = "sendMessage";

        readonly ILogger logger;

        // TODO:  Need to research new logger functionality.
        public UpdateResultHandler()
        {
            logger.LogWarning("Starting UpdateResultHandler", null);
        }

        public enum JokeType
        {
            Any,
            Nerdy,
            Explicit
        }

        public void ProcessUpdateRequest(Message message)
        {
            int chatID = message.chat.id;
            string messageText = message.text;
            logger.LogWarning("Message Text: {0}", messageText);
            Task<string> jsonToSend = processUpdateRequest(messageText, chatID);

            string returnedString = jsonToSend.Result;
        }

        async Task<string> processUpdateRequest(string messageText, int chatID)
        {
            bool returnMessage = true;
            string orginalText = messageText;
            MessageToReturn messageToReturn = new MessageToReturn();
            ChuckJokeHandler jokeHandler = new ChuckJokeHandler();
            messageToReturn.chat_id = chatID;

            messageText = messageText.ToLower();

            if (messageText == "/help")
                messageToReturn.text = "Try /chuck";
            else if (messageText == "/start")
                messageToReturn.text = "Get a Chuck Norris joke by entering /chuck";
            else if (messageText == "/chuck")
                messageToReturn.text = jokeHandler.GetChuckJoke();
            else if (messageText == "/chuck@censecnwebbot")
                messageToReturn.text = jokeHandler.GetChuckJoke();
            else if (messageText == "/replace@censecnwebbot")
            {
                string replaceString = orginalText.Substring(8);
                replaceString.Trim();
                messageToReturn.text = jokeHandler.GetChuckJoke(replaceString);
            }
            else if (messageText.StartsWith("/replace"))
            {
                string replaceString = orginalText.Substring(8);
                replaceString.Trim();
                messageToReturn.text = jokeHandler.GetChuckJoke(replaceString);
            }
            else
                returnMessage = false;

            string serializedMessage = string.Empty;
            if (returnMessage)
            {
                serializedMessage = JsonConvert.SerializeObject(messageToReturn);
                await sendUpdateReply(serializedMessage);
            }
            else 
            {
                serializedMessage = "Problem";
            }
            return serializedMessage;
        }


        async Task<string> sendUpdateReply(string jsonToPost)
        {
            string endpointString = "bot" + telegramRESTAPIKey + "/" + telegramUpdateEndpoint;
            endpointString = telegramBaseURL + "/" + endpointString;
           
            var contentType = new MediaTypeHeaderValue("application/json");
            var postRequest = new HttpRequestMessage(HttpMethod.Post, endpointString);

            HttpContent content = new StringContent(jsonToPost);
            postRequest.Content = content;
            postRequest.Content.Headers.ContentType = contentType;

            string returnString = "Success";
            HttpResponseMessage responseMessage = null;
            try
            {

                // responseMessage = await client.SendAsync(postRequest);
            }
            catch (Exception ex)
            {
                returnString = ex.Message;
            }

            return returnString;
        }
    }
}
