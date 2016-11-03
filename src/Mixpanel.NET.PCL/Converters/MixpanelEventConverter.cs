using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mixpanel.NET.PCL.Attributes;

namespace Mixpanel.NET.PCL.Converters
{
    internal static class MixpanelEventConverter
    {
        public static Dictionary<string, object> ToDictionary(IMixpanelEvent mixpanelEvent)
        {
            const string eventNamePropertyName = nameof (mixpanelEvent.EventName);

            var dictionary = new Dictionary<string, object> ();
            var eventName = mixpanelEvent.EventName;
            var type = mixpanelEvent.GetType ();
            var fields = type.GetRuntimeFields ().ToArray();
            var properties = type.GetRuntimeProperties ().ToArray ();

            foreach (var p in properties)
            {
                if (p.Name == eventNamePropertyName)
                {
                    continue;
                }

                var ignoreAttribute = p.GetCustomAttribute<MixpanelIgnoreAttribute> ();
                if (ignoreAttribute != null)
                {
                    continue;
                }

                var propertyName = GetPropertyNameFromAttribute(p) ??
                    PropertyNameConverter.ToSpacedString (p.Name);

                var propertyValue = p.GetValue (mixpanelEvent);

                dictionary.Add (propertyName, propertyValue);
            }

            foreach(var f in fields)
            {
                var propertyAttribute = f.GetCustomAttribute<MixpanelPropertyAttribute> ();
                if (propertyAttribute == null)
                {
                    continue;
                }

                var fieldName = GetPropertyNameFromAttribute (f) ??
                    PropertyNameConverter.ToSpacedString (f.Name);

                var fieldValue = f.GetValue (mixpanelEvent);

                dictionary.Add (fieldName, fieldValue);
            }

            return dictionary;
        }

        private static string GetPropertyNameFromAttribute(MemberInfo property)
        {
            var propertyAttribute = property.GetCustomAttribute<MixpanelPropertyAttribute>();
            if (propertyAttribute == null)
            {
                return null;
            }

            return propertyAttribute.PropertyName;
        }
    }
}
