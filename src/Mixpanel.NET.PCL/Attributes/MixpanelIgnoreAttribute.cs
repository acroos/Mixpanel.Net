using System;
namespace Mixpanel.NET.PCL.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class MixpanelIgnoreAttribute : Attribute
    {
    }
}
