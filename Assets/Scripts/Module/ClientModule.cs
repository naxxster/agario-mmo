using MLAPI;
using MLAPI.Transports.UNET;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientModule : MonoBehaviour
{
    struct ConnectionInfo
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

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        string activeSceneName = SceneManager.GetActiveScene().name;
        if (activeSceneName == "Map001" || activeSceneName == "Map002")
        {
#if UNITY_EDITOR
            if (NetworkingManager.Singleton.IsConnectedClient)
            {
                DisconnectToServer();
            }
            ConnectToServer();
#else
            if (!Application.isBatchMode)
            {
                ConnectToServer();
            }
#endif
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }

    public void ConnectToServer()
    {
        // NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress : Connection Host Address
        // NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectPort : Port that Client Connect
        // NetworkingManager.Singleton.GetComponent<UnetTransport>().ServerListenPort : Port that Server Listen
        Debug.Log("Connect To Server");

        ConnectionInfo connectionInfo = GetMatchEndpoint();
        NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress = connectionInfo.GetAddress();
        NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectPort = connectionInfo.GetPort();
        NetworkingManager.Singleton.StartClient();
    }

    public void DisconnectToServer()
    {
        NetworkingManager.Singleton.StopClient();
    }

    public bool SignIn(string inputName, string inputPassword)
    {
        return true;
    }

    public void SignInProcess(string inputName)
    {
        Debug.Log("Sign In Process");
    }

    public bool SignUp(string inputName, string inputPassword)
    {
        return true;
    }

    private ConnectionInfo GetMatchEndpoint()
    {
        //Temp Code
        string activeSceneName = SceneManager.GetActiveScene().name;
        ConnectionInfo connectionInfo = new ConnectionInfo();

        if (activeSceneName == "Map001")
        {
            connectionInfo.SetAddress("127.0.0.1");
            connectionInfo.SetPort(7777);
        }
        else if (activeSceneName == "Map002")
        {
            connectionInfo.SetAddress("127.0.0.1");
            connectionInfo.SetPort(8888);
        }
        return connectionInfo;
    }
}
