using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public static class HttpModule
{
    public struct HttpModel
    {
        public string statusCode;
        public string body;
    }

    private static readonly HttpClient httpClient = new HttpClient();

    public static async Task<HttpModel> PostAsyncHttp(string url, object param)
    {
        var jsonParam = JsonUtility.ToJson(param);
        Debug.Log("Json Param : " + jsonParam);
        var stringContent = new StringContent(jsonParam, System.Text.Encoding.UTF8, "application/json");

        using (var response = await httpClient.PostAsync(url, stringContent).ConfigureAwait(false))
        {
            string json = await response.Content.ReadAsStringAsync();
            return JsonUtility.FromJson<HttpModel>(json);
        }
    }
}