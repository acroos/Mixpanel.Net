using System;
namespace Mixpanel.NET.PCL
{
    public static class Client
    {
        private static string _token;
        internal static string Token => _token;

        public static void Initialize(string token)
        {
            _token = token;
        }
    }
}
