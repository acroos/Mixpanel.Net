using System;
namespace Mixpanel.NET.PCL
{
    public static class Client
    {
        internal static string Token { get; private set; }

        public static void Initialize(string token)
        {
            Token = token;
        }
    }
}
