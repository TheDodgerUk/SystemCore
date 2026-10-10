using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public delegate void ResponseHandler(JToken response);

public class RestRequest
{
    public readonly string Url;

    private readonly MonoBehaviour m_Host;

    public RestRequest(MonoBehaviour host, string url)
    {
        Url = url;
        m_Host = host;
    }

    public void Get(ResponseHandler callback)
    {
        WaitForResponse(UnityWebRequest.Get(Url), callback);
    }

    private void WaitForResponse(UnityWebRequest request, ResponseHandler callback)
    {
        m_Host.WaitForRequest(request, response =>
        {
            if (response.isNetworkError || response.isHttpError)
            {
                Debug.Log($"{response.error}\nfrom \"{Url}\"");
                callback(null);
            }
            else
            {
                string text = response.downloadHandler.text;
                Debug.Log($"Received response \"{Url}\"\n\"{text}\"");
                callback(JToken.Parse(text));
            }
        });
    }
}