using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using App.Interface;
using Xamarin.Android.Net;

namespace App.Platforms.Android
{
    class AndroidHttpMessageHandler : IPlatfromHttpMessageHandler
    {
        public HttpMessageHandler GetHttpMessageHandler() => new AndroidMessageHandler
        {
            ServerCertificateCustomValidationCallback = (httpRequestMessage, certificate, chain, sslPolicyErrors) => certificate?.Issuer == "CN=localhost" && certificate?.Subject == "CN=localhost" || sslPolicyErrors == SslPolicyErrors.None
        };
    }
}
