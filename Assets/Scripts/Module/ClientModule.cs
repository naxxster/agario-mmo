using MLAPI;
using MLAPI.Transports.UNET;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientModule : MonoBehaviour
{
    public static ClientModule Singleton { get; protected set; }

    public struct ConnectionInfo
    {
        private string address;
        private int port;

        public string GetAddress()
        {
            return address;
        }
        public void SetAddress(string address)
        {
            this.address = address;
        }
        public int GetPort()
        {
            return port;
        }
        public void SetPort(int port)
        {
            this.port = port;
        }
    }
    public string PlayerName = "";

    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("On SceneLoad - " + scene.name);
    }

    public void MoveToWorld(string worldId)
    {
        LoadWorldAsync(worldId);
    }

    public void ConnectToServer(ConnectionInfo connectionInfo)
    {
        // NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress : Connection Host Address
        // NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectPort : Port that Client Connect
        // NetworkingManager.Singleton.GetComponent<UnetTransport>().ServerListenPort : Port that Server Listen

        Debug.Log("Connect To Server. IP=" + connectionInfo.GetAddress() + ", Port=" + connectionInfo.GetPort());
        NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress = connectionInfo.GetAddress();
        NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectPort = connectionInfo.GetPort();
        NetworkingManager.Singleton.StartClient();
    }

    public void DisconnectToServer()
    {
        if (NetworkingManager.Singleton.IsConnectedClient)
        {
            Debug.Log("Disconnect CLient");
            NetworkingManager.Singleton.StopClient();
        }
    }

    public bool SignIn(string inputName, string inputPassword)
    {
        return true;
    }

    public void SignInProcess(string inputName)
    {
        Debug.Log("Sign In Process");
        PlayerName = inputName;

        MoveToWorld("Map001");
    }

    public bool SignUp(string inputName, string inputPassword)
    {
        return true;
    }

    private async void LoadWorldAsync(string worldId)
    {
        DisconnectToServer();
        SceneManager.LoadScene(worldId);
        Task<HttpModule.HttpModel> httpResponseMessage =
            HttpModule.PostAsyncHttp("https://0sos1lsc4c.execute-api.ap-northeast-2.amazonaws.com/prod/matchrequest", new APIModule.MatchRequestParam(worldId));

        HttpModule.HttpModel response = await httpResponseMessage;
        GameLiftModel.MatchRequest responseJson = JsonUtility.FromJson<GameLiftModel.MatchRequest>(response.body);
        ConnectionInfo connectionInfo = new ConnectionInfo();
        connectionInfo.SetAddress(responseJson.address);
        connectionInfo.SetPort(responseJson.port);
        ConnectToServer(connectionInfo);
    }
}