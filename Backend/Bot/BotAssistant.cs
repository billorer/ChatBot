using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace Backend.Bot
{
    public class QnABotAssistant : IBotAssistant
    {
        public string Answer(string message)
        {
            return MakeRequest(message);        
        }

        private string MakeRequest(string messageQuery)
        {
            string responseString = string.Empty;

            var query = messageQuery;
            var knowledgebaseId = "a5b649c6-8719-4302-b6d3-8c803a0b14de"; // Use knowledge base id created.
            var qnamakerSubscriptionKey = "a9a20432eb88462c973b1c63ce3385b1"; //Use subscription key assigned to you.

            //Build the URI
            Uri qnamakerUriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v2.0");
            var builder = new UriBuilder($"{qnamakerUriBase}/knowledgebases/{knowledgebaseId}/generateAnswer");

            //Add the question as part of the body
            var postBody = $"{{\"question\": \"{query}\"}}";

            //Send the POST request
            using (WebClient client = new WebClient())
            {
                //Set the encoding to UTF8
                client.Encoding = System.Text.Encoding.UTF8;

                //Add the subscription key header
                client.Headers.Add("Ocp-Apim-Subscription-Key", qnamakerSubscriptionKey);
                client.Headers.Add("Content-Type", "application/json");
                responseString = client.UploadString(builder.Uri, postBody);
            }
            JObject json = JObject.Parse(responseString);
            return json["answers"][0]["answer"].ToString();
        }

    }
}