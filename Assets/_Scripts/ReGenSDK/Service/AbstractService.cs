using System;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace ReGenSDK.Service
{
    public class AbstractService
    {
        private readonly string endpoint;
        private readonly Func<Task<string>> authorizationProvider;

        public AbstractService(string endpoint, Func<Task<string>> authorizationProvider)
        {
            this.endpoint = endpoint;
            this.authorizationProvider = authorizationProvider;
        }
        
        protected RequestBuilder<UnityWebRequest> HttpRequest()
        {
            return new RequestBuilder<UnityWebRequest>(endpoint, authorizationProvider, op => op);
        }

        protected RequestBuilder<UnityWebRequest> HttpGet()
        {
            return HttpRequest().Method("GET");
        }
        
        protected RequestBuilder<UnityWebRequest> HttpPost()
        {
            return HttpRequest().Method("POST");
        }
        
        protected RequestBuilder<UnityWebRequest> HttpPut()
        {
            return HttpRequest().Method("PUT");
        }
        
        protected RequestBuilder<UnityWebRequest> HttpDelete()
        {
            return HttpRequest().Method("DELETE");
        }
    }
}