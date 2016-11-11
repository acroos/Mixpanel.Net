using System;
using Newtonsoft.Json;

namespace Mixpanel.NET.PCL
{
    public class MixpanelResponse
    {
        public bool Status { get; }
        public string Error { get; }

        [JsonConstructor]
        public MixpanelResponse (bool status, string message = null)
        {
            Status = status;
            Error = message;
        }
    }
}
