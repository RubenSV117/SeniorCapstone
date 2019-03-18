using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class SearchDB : MonoBehaviour
{
    private const string URL = "http://api.edamam.com/search?q=";
    private const string BURL = "bing.com";
    private const string API_ID = "&app_id=${a38ae966}";
    private const string API_KEY = "&app_key=${afad9bdb0b584a2cccbd34180335b1ab	—}";
    
    public Text responseText;

    public void Request()
    {
        // A correct website page.
        StartCoroutine(GetRequest(URL + API_ID + API_KEY));

        // A non-existing page.
        StartCoroutine(GetRequest(BURL));
    }

    private IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                responseText.text = webRequest.downloadHandler.text;
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
            }
        }
    }
}
