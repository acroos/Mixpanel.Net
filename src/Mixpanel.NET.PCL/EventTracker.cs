using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mixpanel.NET.PCL.Converters;

namespace Mixpanel.NET.PCL
{
    public static class EventTracker
    {
        /// <summary>
        /// Track a new event
        /// </summary>
        /// <returns>The response from mixpanel</returns>
        /// <param name="profile">Profile.</param>
        /// <param name="mixpanelEvent">Mixpanel event.</param>
        public static Task<MixpanelResponse> Track (Profile profile, IMixpanelEvent mixpanelEvent)
        {
            if (mixpanelEvent == null)
            {
                throw new ArgumentNullException (nameof (mixpanelEvent));
            }

            var eventName = mixpanelEvent.EventName;
            var properties = MixpanelEventConverter.ToDictionary (mixpanelEvent);

            return Track (profile, eventName, properties);
        }

        /// <summary>
        /// Track a new event
        /// </summary>
        /// <returns>The response from mixpanel</returns>
        /// <param name="mixpanelEvent">Mixpanel event.</param>
        public static Task<MixpanelResponse> Track (IMixpanelEvent mixpanelEvent)
        {
            return Track (null, mixpanelEvent);
        }

        /// <summary>
        /// Track a new event
        /// </summary>
        /// <returns>The response from mixpanel</returns>
        /// <param name="profile">Profile.</param>
        /// <param name="eventName">Event name.</param>
        /// <param name="properties">Properties</param>
        public static Task<MixpanelResponse> Track (Profile profile, string eventName,
                                        Dictionary<string, object> properties)
        {
            if (Client.Token == null)
            {
                throw new Exception ("Cannot track events without initializing Client first");
            }

            if (eventName == null)
            {
                throw new ArgumentNullException (nameof (eventName));
            }

            if (properties == null)
            {
                properties = new Dictionary<string, object> ();
            }

            properties.Add ("token", Client.Token);

            if (profile != null)
            {
                properties.Add ("distinct_id", profile.DistinctId);
            }

            var dataDictionary = new Dictionary<string, object>
            {
                { "event", eventName },
                { "properties", properties ?? new Dictionary<string, object> () }
            };

            var request = RequestHelpers.GetRequestMessageFromDictionaryAndEndpoint (Constants.TrackUri, dataDictionary);

            return RequestHelpers.GetRequestOutput (request);
        }

        /// <summary>
        /// Track a new event
        /// </summary>
        /// <returns>The response from mixpanel</returns>
        /// <param name="eventName">Event name.</param>
        /// <param name="properties">Properties</param>
        public static Task<MixpanelResponse> Track (string eventName,
                                       Dictionary<string, object> properties)
        {
            return Track (null, eventName, properties);
        }

        /// <summary>
        /// Track a new event
        /// </summary>
        /// <returns>The response from mixpanel</returns>
        /// <param name="eventName">Event name.</param>
        public static Task<MixpanelResponse> Track (string eventName)
        {
            return Track (null, eventName, null);
        }
    }
}
