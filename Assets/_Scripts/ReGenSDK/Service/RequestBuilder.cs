using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ReGenSDK.Exceptions;
using ReGenSDK.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ReGenSDK.Service
{
    public class RequestBuilder<T>
    {
        private string method = "GET";
        private string endpoint;
        private bool isQuery;
        private readonly Func<Task<string>> authorizationProvider;
        private Func<UnityWebRequest, T> mapping;
        private string body;
        private bool needsAuth;

        public RequestBuilder(string endpoint, Func<Task<string>> authorizationProvider,
            Func<UnityWebRequest, T> op)
        {
            this.endpoint = endpoint;
            this.authorizationProvider = authorizationProvider;
        }

        public RequestBuilder<T> Path([NotNull] string param)
        {
            if (string.IsNullOrWhiteSpace(param)) throw new ArgumentNullException(nameof(param));
            if (isQuery)
            {
                throw new InvalidOperationException("all path parameters must be inserted before query parameters");
            }

            endpoint += "/" + UnityWebRequest.EscapeURL(param);
            return this;
        }

        public RequestBuilder<T> Method([NotNull] string method)
        {
            if (string.IsNullOrWhiteSpace(method)) throw new ArgumentNullException(nameof(method));
            this.method = method;
            return this;
        }

        public RequestBuilder<T> Query([NotNull] string name, [CanBeNull] string value)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (value == null) return this;
            if (!isQuery)
            {
                isQuery = true;
                endpoint += "?" + UnityWebRequest.EscapeURL(name) + "=" + UnityWebRequest.EscapeURL(value);
            }
            else
            {
                endpoint += "," + UnityWebRequest.EscapeURL(name) + "=" + UnityWebRequest.EscapeURL(value);
            }

            return this;
        }

        public RequestBuilder<T> Body([NotNull] object content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (content is string s)
            {
                body = s;
            }
            else
            {
                body = JsonUtility.ToJson(content);
            }

            return this;
        }

        public RequestBuilder<T> RequireAuthentication()
        {
            needsAuth = true;
            return this;
        }

        public RequestBuilder<U> Map<U>([NotNull] Func<T, U> block)
        {
            if (block == null) throw new ArgumentNullException(nameof(block));
            Func<UnityWebRequest, U> map = request => block(mapping(request));
            return new RequestBuilder<U>(endpoint, authorizationProvider, map.Memoized())
            {
                method = method,
                isQuery = isQuery,
                body = body,
                needsAuth = needsAuth
            };
        }

        public RequestBuilder<T> Status(int code, [NotNull] Action<T> callback)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            mapping = op =>
            {
                var map = mapping(op);
                if (op.responseCode == code)
                {
                    callback(map);
                }

                return map;
            };
            return this;
        }

        public Task<T> Execute()
        {
            var task = new TaskCompletionSource<UnityWebRequest>();
            return MainThreadTask.Run(async () =>
            {
                var unityWebRequest = new UnityWebRequest(endpoint, method);
                unityWebRequest.SetRequestHeader("Accept", "application/json");
                if (needsAuth)
                {
                    var token = await authorizationProvider();
                    if (string.IsNullOrEmpty(token))
                    {
                        throw new RegenAuthenticationException("token was not retrieved for endpoint " + endpoint);
                    }

                    unityWebRequest.SetRequestHeader("Authorization", $"Bearer: {token}");
                }

                if (body != null)
                {
                    unityWebRequest.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
                }

                unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
                Debug.Log($"Sending Web Request: {method} {endpoint}");
                var request = unityWebRequest.SendWebRequest();
                request.completed += operation =>
                {
                    var r = request.webRequest;
                    Debug.Log($"Web Request Complete: {method} {endpoint} {r.responseCode} {r.downloadHandler.text}");
                    if (r.isNetworkError)
                    {
                        task.SetException(new HttpRequestException("Encountered Network Error" + r.error));
                    }
                    else if (r.isHttpError)
                    {
                        task.SetException(new HttpResponseException(r.responseCode, r.error, r.GetResponseHeaders()));
                    }
                    else
                    {
                        task.SetResult(r);
                    }
                };
                await task.Task;
                return mapping(task.Task.Result);
            });
        }
    }

    public class HttpResponseException : RegenApiException
    {
        public readonly long Code;
        public readonly Dictionary<string, string> Headers;

        public HttpResponseException(long code, string message, Dictionary<string, string> headers) : base(message)
        {
            Code = code;
            Headers = headers;
        }
    }

    public static class RequestBuilderExtension
    {
        public static RequestBuilder<T> ParseJson<T>(this RequestBuilder<UnityWebRequest> builder)
        {
            return builder.Map(x =>
                {
                    Debug.Log("Attempting to parse network response: " + x.responseCode);
                    if (x.isNetworkError || x.isHttpError)
                    {
                        Debug.Log("Error detected with status code: " + x.responseCode);
                        return default;
                    }
                    if (x.responseCode == 204) 
                        return default;
                    var text = x.downloadHandler.text;
                    if (string.IsNullOrWhiteSpace(text))
                        return default;
                    Debug.Log($"Parsing results to {typeof(T)}");
                    return JsonUtility.FromJson<T>(text);
                }
            );
        }
    }
}