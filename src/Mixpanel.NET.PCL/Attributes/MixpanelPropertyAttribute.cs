using System;
namespace Mixpanel.NET.PCL.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class MixpanelPropertyAttribute : Attribute
    {
        private readonly string _propertyName;
        public string PropertyName => _propertyName;

        public MixpanelPropertyAttribute()
        {
            _propertyName = null;
        }

        public MixpanelPropertyAttribute (string propertyName)
        {
            _propertyName = propertyName;
        }
    }
}
