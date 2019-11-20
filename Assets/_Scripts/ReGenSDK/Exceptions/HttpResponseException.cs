using System.Collections.Generic;
using System.Linq;
using ReGenSDK.Exceptions;
using UnityEngine.Networking;

namespace ReGenSDK.Service
{
    public class HttpResponseException : RegenApiException
    {

        public HttpResponseException(string message, UnityWebRequest request) : this(message, request.responseCode,
            request.error, request.downloadHandler.text,
            request.GetResponseHeaders())
        {
        }

        public HttpResponseException(string message, long code, string error, string content,
            Dictionary<string, string> headers) : base(
            $"{message}\nCode: {code} {error}\nContent: {content}\nHeaders: {headers.Select(pair => $"{pair.Key} : {pair.Value}").Aggregate((a, b) => a + "\n" + b)}")

        {
        }
    }
}