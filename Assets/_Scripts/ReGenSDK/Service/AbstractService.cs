using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.Core.Internal;
using RestSharp.Serializers;
using UnityEditor;
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
        
        protected RequestBuilder<UnityWebRequest> WebRequest()
        {
            return new RequestBuilder<UnityWebRequest>(endpoint, authorizationProvider, op => op);
        }

        protected RequestBuilder<UnityWebRequest> Get()
        {
            return new RequestBuilder<UnityWebRequest>(endpoint, authorizationProvider, op => op).Method("GET");
        }
        
        protected RequestBuilder<UnityWebRequest> Post()
        {
            return new RequestBuilder<UnityWebRequest>(endpoint, authorizationProvider, op => op).Method("POST");
        }
        
        protected RequestBuilder<UnityWebRequest> Put()
        {
            return new RequestBuilder<UnityWebRequest>(endpoint, authorizationProvider, op => op).Method("PUT");
        }
        
        protected RequestBuilder<UnityWebRequest> Delete()
        {
            return new RequestBuilder<UnityWebRequest>(endpoint, authorizationProvider, op => op).Method("DELETE");
        }
    }
}