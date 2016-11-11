using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mixpanel.NET.PCL
{
    internal static class RequestHelpers
    {
        /// <summary>
        /// Gets the request message from given dictionary and endpoint.
        /// </summary>
        /// <returns>The request message.</returns>
        /// <param name="endpoint">The endpoint where the request will be sent.</param>
        /// <param name="dict">The dictionary of values to send.</param>
        internal static HttpRequestMessage GetRequestMessageFromDictionaryAndEndpoint (string endpoint, Dictionary<string, object> dict)
        {
            var json = JsonConvert.SerializeObject (dict);
            var base64String = Convert.ToBase64String (Encoding.UTF8.GetBytes (json));

            var request = new HttpRequestMessage (HttpMethod.Post, endpoint)
            {
                Content = new StringContent ($"data={base64String}&verbose=1")
            };
            request.Content.Headers.ContentType = new MediaTypeHeaderValue ("application/x-www-form-urlencoded");

            return request;
        }

        /// <summary>
        /// Makes the http request given a message
        /// </summary>
        /// <returns>The response from mixpanel</returns>
        /// <param name="request">The http request message</param>
        internal static async Task<MixpanelResponse> GetRequestOutput(HttpRequestMessage request)
        {
            using (var client = new HttpClient ())
            {
                var response = await client.SendAsync (request);
                if (!response.IsSuccessStatusCode)
                {
                    return new MixpanelResponse(false, $"Request status code: {response.StatusCode}");
                }

                var responseBody = await response.Content.ReadAsStringAsync ();

                var mixpanelResponse = JsonConvert.DeserializeObject<MixpanelResponse> (responseBody);

                return mixpanelResponse;
            }
        }
    }
}
