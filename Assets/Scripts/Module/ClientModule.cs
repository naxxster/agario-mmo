using MLAPI;
using MLAPI.Transports.UNET;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientModule : MonoBehaviour
{
    public static ClientModule Singleton { get; protected set; }

    public enum UIType
    {
        NORMAL,
        PLAY
    };
    public class ConnectionInfo
    {
        public string Addess = "127.0.0.1";
        public int Port = 7777;
        public string WorldId = "Map001";

        public string PlayerSessionId = "";
        public string TicketId = "";
        public bool MatchSuccess = false;
        public int PollingCount = 0;
    }
    public string PlayerId = "";

    public ConnectionInfo ClientConnection = new ConnectionInfo();

    void Awake()
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


    #region UI Handle

    [SerializeField]
    public InputField NameInputField;
    [SerializeField]
    public InputField PasswordInputField;
    public Button SignInBtn;
    public Button SignUpBtn;
    public Text ConnectionStatus;

    public void SetMainUI(UIType uIType)
    {
        if (uIType == UIType.NORMAL)
        {
            //Init
            NameInputField.gameObject.SetActive(true);
            PasswordInputField.gameObject.SetActive(true);
            SignInBtn.gameObject.SetActive(true);
            SignUpBtn.gameObject.SetActive(true);
            ConnectionStatus.gameObject.SetActive(true);
        }
        else if (uIType == UIType.PLAY)
        {
            //Hosting
            NameInputField.gameObject.SetActive(false);
            PasswordInputField.gameObject.SetActive(false);
            SignInBtn.gameObject.SetActive(false);
            SignUpBtn.gameObject.SetActive(false);
            ConnectionStatus.gameObject.SetActive(false);
        }
    }
    public void OnClickSignInBtn()
    {
        string inputName = NameInputField.text;
        string inputPassword = PasswordInputField.text;

        Debug.Log("Onclick - " + ClientModule.Singleton);
        ConnectionStatus.text = "Entering the world";

        if (ClientModule.Singleton.SignIn(inputName, inputPassword))
        {
            // Login existing server
            //SetNetworkHUD(UIType.PLAY);
            ClientModule.Singleton.SignInProcess(inputName);
        }
        else
        {
            Debug.Log("Sign In Failed. inputName-" + inputName + ", inputPassword-" + inputPassword);
        }
    }

    public void OnClickSignUpBtn()
    {
        string inputName = NameInputField.text;
        string inputPassword = PasswordInputField.text;

        if (ClientModule.Singleton.SignUp(inputName, inputPassword))
        {
            Debug.Log("Sign Up Succeeded!");
        }
        else
        {
            Debug.Log("Sign Up Failed. inputName-" + inputName + ", inputPassword-" + inputPassword);
        }
    }
    #endregion


    #region Auth Process
    public bool SignIn(string inputName, string inputPassword)
    {
        // Implement SignIn Validation
        return true;
    }

    public void SignInProcess(string inputName)
    {
        Debug.Log("Sign In Process");
        PlayerId = inputName;

        MoveToWorld("Map001");
    }

    public bool SignUp(string inputName, string inputPassword)
    {
        return true;
    }
    #endregion

    #region ClientNetwork
    public void MoveToWorld(string worldId)
    {
        ConnectionStatus.text = "Searching World . . .";
        StartCoroutine(HttpModule.PutRequest(APIModule.GAMELIFT_MATCHREQUEST, new APIModule.MatchmakingRequest(this.PlayerId, worldId), MatchRequestCallback));
    }

    public void ConnectToServer()
    {
        // NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress : Connection Host Address
        // NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectPort : Port that Client Connect
        // NetworkingManager.Singleton.GetComponent<UnetTransport>().ServerListenPort : Port that Server Listen

        Debug.Log("Connect To Server. IP=" + ClientConnection.Addess + ", Port=" + ClientConnection.Port);
        NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress = ClientConnection.Addess;
        NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectPort = ClientConnection.Port;
        NetworkingManager.Singleton.NetworkConfig.ConnectionApproval = true;
        NetworkingManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes(this.ClientConnection.PlayerSessionId);
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

    private void MatchRequestCallback(string matchmakingResponse)
    {
        Debug.Log("Match Request Callback - " + matchmakingResponse);
        APIModule.MatchmakingResponse matchmakingResult = JsonUtility.FromJson<APIModule.MatchmakingResponse>(matchmakingResponse);
        this.ClientConnection.TicketId = matchmakingResult.ticketId;

        StartCoroutine(MatchStatusPolling());
    }

    IEnumerator MatchStatusPolling()
    {
        Debug.Log("Send Polling with TicketId - " + this.ClientConnection.TicketId);
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(HttpModule.PutRequest(APIModule.GAMELIFT_MATCHSTATUS, new APIModule.MatchstatusRequest(this.ClientConnection.TicketId), MatchStatusCallback));
    }

    private void MatchStatusCallback(string matchstatusResponse)
    {
        Debug.Log("MatchStatus Callback : " + matchstatusResponse);
        APIModule.MatchstatusResponse matchstatusResult = JsonUtility.FromJson<APIModule.MatchstatusResponse>(matchstatusResponse);
        if (matchstatusResult.port > 0)
        {
            this.ClientConnection.MatchSuccess = true;
        }
        if (this.ClientConnection.MatchSuccess)
        {
            SetMainUI(UIType.PLAY);
            DisconnectToServer();
            SceneManager.LoadScene(this.ClientConnection.WorldId);
            ConnectToServer();
        }
        else
        {
            if (this.ClientConnection.PollingCount < 120)
            {
                // Send requests around 2minutes
                StartCoroutine(MatchStatusPolling());
            }
            else
            {
                Debug.Log("Connection Error. Start later");
                ConnectionStatus.text = "Connection Error";
            }
        }
    }
    #endregion
}