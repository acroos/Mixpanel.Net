using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Mixpanel.NET.PCL.Converters;
using Newtonsoft.Json;

namespace Mixpanel.NET.PCL
{
    public class Tracker
    {
        private readonly string _token;
        private string _distinctId;

        public Tracker (string token)
        {
            if (token == null)
            {
                throw new ArgumentNullException (nameof (token));
            }
            _token = token;
        }

        /// <summary>
        /// Permanently delete the profile from Mixpanel, along with all of its properties. 
        /// Note: You must call <see cref="Engage"/> before using this method
        /// </summary>
        /// <returns>True if the profile was deleted, false otherwise</returns>
        public Task<bool> Delete()
        {
            if (_distinctId == null)
            {
                throw new Exception ("Cannot delete a profile when distinct id is not set");
            }
            var id = _distinctId;
            _distinctId = null;

            return Delete (id);
        }

        /// <summary>
        /// Permanently delete the profile from Mixpanel, along with all of its properties. 
        /// </summary>
        /// <returns>True if the profile was deleted, false otherwise</returns>
        /// <param name="distinctId">Distinct identifier.</param>
        public Task<bool>Delete(string distinctId)
        {
            var dataDictionary = new Dictionary<string, object>
            {
                { "$token", _token },
                { "$distinct_id", _distinctId },
                { "$delete", string.Empty }
            };

            var request = GetRequestMessageFromDictionaryAndEndpoint (Constants.EngageUri, dataDictionary);

            return MakeRequest (request);
        }

        /// <summary>
        /// Engage a new user with a distinct id.
        /// This will not add any further information to a profile.
        /// </summary>
        /// <returns>True if the engage worked, false otherwise</returns>
        /// <param name="distinctId">Distinct identifier.</param>
        public Task<bool> Engage(string distinctId)
        {
            if (distinctId == null)
            {
                throw new ArgumentNullException (nameof (distinctId));
            }

            _distinctId = distinctId;

            var dataDictionary = new Dictionary<string, object>
            {
                { "$token", _token },
                { "$distinct_id", _distinctId }
            };

            var request = GetRequestMessageFromDictionaryAndEndpoint (Constants.EngageUri, dataDictionary);

            return MakeRequest (request);
        }

        /// <summary>
        /// Takes a dictionary containing names and values of profile properties. 
        /// If the profile does not exist, it creates it with these properties. 
        /// If it does exist, it sets the properties to these values, overwriting existing values.
        /// Note: You must call <see cref="Engage"/> before using this method
        /// </summary>
        /// /// <returns>True if the set worked, false otherwise</returns>
        /// <param name="properties">Properties of the profile</param>
        public Task<bool> Set(Dictionary<string, object> properties)
        {
            if (_distinctId == null)
            {
                throw new Exception ("Cannot set values on a profile with no distinct id set");
            }

            if (properties == null)
            {
                throw new ArgumentNullException (nameof (properties));
            }

            var dataDictionary = new Dictionary<string, object>
            {
                { "$token", _token },
                { "$distinct_id", _distinctId },
                { "$set", properties }
            };

            var request = GetRequestMessageFromDictionaryAndEndpoint (Constants.EngageUri, dataDictionary);

            return MakeRequest (request);
        }

        /// <summary>
        /// Works the same as <see cref="Set"/>
        /// Will only set the value the firs time
        /// Note: You must call <see cref="Engage"/> before using this method
        /// </summary>
        /// <returns>True if the set worked, false otherwise</returns>
        /// <param name="properties">Properties of the profile</param>
        public Task<bool> SetOnce(Dictionary<string, object> properties)
        {
            if (_distinctId == null)
            {
                throw new Exception ("Cannot set values on a profile with no distinct id set");
            }

            if (properties == null)
            {
                throw new ArgumentNullException (nameof (properties));
            }

            var dataDictionary = new Dictionary<string, object>
            {
                { "$token", _token },
                { "$distinct_id", _distinctId },
                { "$set_once", properties }
            };

            var request = GetRequestMessageFromDictionaryAndEndpoint (Constants.EngageUri, dataDictionary);

            return MakeRequest (request);
        }

        /// <summary>
        /// Track the specified event
        /// </summary>
        /// <returns>True if the event was tracked, false otherwise</returns>
        /// <param name="mixpanelEvent">A mixpanel event</param>
        public Task<bool> Track(IMixpanelEvent mixpanelEvent)
        {
            if (mixpanelEvent == null)
            {
                throw new ArgumentNullException (nameof (mixpanelEvent));
            }

            var eventName = mixpanelEvent.EventName;
            var properties = MixpanelEventConverter.ToDictionary (mixpanelEvent);

            return Track (eventName, properties);
        }

        /// <summary>
        /// Tracks a named event with a set of properties
        /// </summary>
        /// <returns>True if the event was tracked, false otherwise</returns>
        /// <param name="eventName">Event name.</param>
        /// <param name="properties">Properties.</param>
        public Task<bool> Track(string eventName, Dictionary<string, object> properties = null)
        {
            if (eventName == null)
            {
                throw new ArgumentNullException (nameof (eventName));
            }

            if (properties == null)
            {
                properties = new Dictionary<string, object> ();
            }

            properties.Add ("token", _token);

            if (_distinctId != null)
            {
                properties.Add ("distinct_id", _distinctId);
            }

            var dataDictionary = new Dictionary<string, object>
            {
                { "event", eventName },
                { "properties", properties ?? new Dictionary<string, object> () }
            };

            var request = GetRequestMessageFromDictionaryAndEndpoint (Constants.TrackUri, dataDictionary);

            return MakeRequest (request);
        }

        /// <summary>
        /// Gets the request message from given dictionary and endpoint.
        /// </summary>
        /// <returns>The request message.</returns>
        /// <param name="endpoint">The endpoint where the request will be sent.</param>
        /// <param name="dict">The dictionary of values to send.</param>
        private HttpRequestMessage GetRequestMessageFromDictionaryAndEndpoint(string endpoint, Dictionary<string, object> dict)
        {
            var json = JsonConvert.SerializeObject (dict);
            var base64String = Convert.ToBase64String (Encoding.UTF8.GetBytes (json));

            var data = new []
            {
                new KeyValuePair<string, string> ("data", base64String)
            };

            var request = new HttpRequestMessage (HttpMethod.Post, endpoint)
            {
                Content = new FormUrlEncodedContent (data)
            };
            request.Content.Headers.ContentType = new MediaTypeHeaderValue ("application/x-www-form-urlencoded");

            return request;
        }
    
        /// <summary>
        /// Makes the http request given a message
        /// </summary>
        /// <returns>True if the request succeeded, false otherwise</returns>
        /// <param name="request">The http request message</param>
        private async Task<bool> MakeRequest(HttpRequestMessage request)
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
