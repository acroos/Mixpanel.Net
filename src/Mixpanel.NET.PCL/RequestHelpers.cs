using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
                Content = new StringContent ($"data={base64String}")
            };
            request.Content.Headers.ContentType = new MediaTypeHeaderValue ("application/x-www-form-urlencoded");

            return request;
        }

        /// <summary>
        /// Makes the http request given a message
        /// </summary>
        /// <returns>True if the request succeeded, false otherwise</returns>
        /// <param name="request">The http request message</param>
        internal static async Task<bool> MakeRequest (HttpRequestMessage request)
        {
            using (var client = new HttpClient ())
            {
                var response = await client.SendAsync (request);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                var responseBody = await response.Content.ReadAsStringAsync ();

                return responseBody == Constants.SuccessResponse;
            }
        }
    }
}
