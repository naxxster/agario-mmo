using MLAPI;
using MLAPI.Transports.UNET;
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

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("On SceneLoad - " + scene.name);
        ConnectToServer(GetMatchEndpoint(scene.name));
    }

    public void MoveToWorld(string worldId)
    {
        DisconnectToServer();
        SceneManager.LoadScene(worldId);
        //ConnectToServer(connectionInfo);
    }

    public void ConnectToServer()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        ConnectionInfo connectionInfo = GetMatchEndpoint(activeSceneName);
        Debug.Log("Connect To Server. IP=" + connectionInfo.GetAddress() + ", Port=" + connectionInfo.GetPort());
        NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress = connectionInfo.GetAddress();
        NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectPort = connectionInfo.GetPort();
        NetworkingManager.Singleton.StartClient();
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

    private ConnectionInfo GetMatchEndpoint(string worldId)
    {
        //Temp Code
        string activeSceneName = SceneManager.GetActiveScene().name;
        ConnectionInfo connectionInfo = new ConnectionInfo();

        if (worldId == "Map001")
        {
            connectionInfo.SetAddress("127.0.0.1");
            connectionInfo.SetPort(7777);
        }
        else if (worldId == "Map002")
        {
            connectionInfo.SetAddress("127.0.0.1");
            connectionInfo.SetPort(8888);
        }
        return connectionInfo;
    }
}
