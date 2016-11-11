using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mixpanel.NET.PCL
{
    public class Profile
    {
        private readonly string _distinctId;
        internal string DistinctId => _distinctId;

        public Profile()
        {
            if (Client.Token == null)
            {
                throw new Exception ("Cannot create a new profile without initializing Client");
            }

            _distinctId = Guid.NewGuid ().ToString ();
        }

        public Profile (string distinctId)
        {
            if (Client.Token == null)
            {
                throw new Exception ("Cannot create a new profile without initializing Client");
            }

            if (distinctId == null)
            {
                throw new ArgumentNullException (nameof (distinctId));
            }

            _distinctId = distinctId;
        }

        /// <summary>
        /// Takes a dictionary containing keys and numerical values. 
        /// When processed, the property values are added to the existing values of the properties on the profile. 
        /// If the property is not present on the profile, the value will be added to 0. 
        /// It is possible to decrement by calling Add with negative values.
        /// This is useful for maintaining the values of properties like "Number of Logins" or "Files Uploaded".
        /// </summary>
        /// <returns>The response from mixpanel</returns>
        /// <param name="properties">Properties.</param>
        public Task<MixpanelResponse> Add(Dictionary<string, int> properties)
        {
            return Engage ("$add", properties);
        }

        /// <summary>
        /// Takes a dictionary containing keys and values, and appends each to a list associated with the corresponding property name. 
        /// Appending to a property that doesn't exist will result in assigning a list with one element to that property.
        /// </summary>
        /// <returns>The response from mixpanel</returns>
        /// <param name="properties">Properties.</param>
        public Task<MixpanelResponse> Append (Dictionary<string, string> properties)
        {
            return Engage ("$append", properties);
        }

        /// <summary>
        /// Permanently delete the profile from Mixpanel, along with all of its properties. 
        /// </summary>
        /// <returns>The response from mixpanel</returns>
        public Task<MixpanelResponse> Delete ()
        {
            return Engage ("$delete", string.Empty);
        }

        /// <summary>
        /// Takes a dictionary containing keys and values. 
        /// The value in the request is removed from the existing list on the user profile. 
        /// If it does not exist, no updates are made.
        /// </summary>
        /// <returns>The response from mixpanel</returns>
        /// <param name="properties">Properties.</param>
        public Task<MixpanelResponse> Remove (Dictionary<string, string> properties)
        {
            return Engage ("$remove", properties);
        }


        /// <summary>
        /// Takes a dictionary containing names and values of profile properties. 
        /// If the profile does not exist, it creates it with these properties. 
        /// If it does exist, it sets the properties to these values, overwriting existing values.
        /// </summary>
        /// <returns>The response from mixpanel</returns>
        /// <param name="properties">Properties of the profile</param>
        public Task<MixpanelResponse> Set (Dictionary<string, object> properties)
        {
            return Engage ("$set", properties);
        }

        /// <summary>
        /// Works the same as <see cref="Set"/>
        /// Will only set the value the first time
        /// </summary>
        /// <returns>The response from mixpanel</returns>
        /// <param name="properties">Properties of the profile</param>
        public Task<MixpanelResponse> SetOnce (Dictionary<string, object> properties)
        {
            return Engage ("$set_once", properties);
        }

        /// <summary>
        /// Takes a dictionary containing keys and list values. 
        /// The list values in the request are merged with the existing list on the user profile, ignoring duplicate list values.
        /// </summary>
        /// <returns>The response from mixpanel</returns>
        /// <param name="properties">Properties.</param>
        public Task<MixpanelResponse> Union (Dictionary<string, string []> properties)
        {
            return Engage ("$union", properties);
        }

        /// <summary>
        /// Takes an array of string property names, and permanently removes the properties and their values from a profile.
        /// </summary>
        /// <returns>The response from mixpanel</returns>
        /// <param name="propertyNames">Property names.</param>
        public Task<MixpanelResponse> Unset (string[] propertyNames)
        {
            return Engage ("$unset", propertyNames);
        }

        /// <summary>
        /// Engage a new user with a distinct id.
        /// This will not add any further information to a profile.
        /// </summary>
        /// <returns>The response from the mixpanel server</returns>
        private Task<MixpanelResponse> Engage (string operation, object properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException (nameof (properties));
            }

            var dataDictionary = new Dictionary<string, object>
            {
                { "$token", Client.Token },
                { "$distinct_id", _distinctId },
                { operation, properties }
            };

            var request =
                RequestHelpers.GetRequestMessageFromDictionaryAndEndpoint (Constants.EngageUri,
                                                                           dataDictionary);
            return RequestHelpers.MakeRequest (request);
        }
    }
}
