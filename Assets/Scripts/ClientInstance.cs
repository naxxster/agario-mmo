using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

public class ClientInstance : MonoBehaviour
{
    public struct LoginMessage : NetworkMessage
    {
        public string id;
        public string password;
    }

    public Button StartHostingBtn;
    public Button StartServerBtn;
    public Button StartClientBtn;
    public Button StopHostingBtn;
    public Button StopServerBtn;
    public Button StopClientBtn;

    [SerializeField]
    public InputField NameInputField;

    public NetworkManager networkManager;

    public string PlayerName;           //Update Through Server Login Callback using Network Message.

    // Start is called before the first frame update
    void Start()
    {
        // SendLoginRequest("test", "");
        SetNetworkHUD(0);
        PlayerName = "Player-Test";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendLoginRequest(string id, string password)
    {
        LoginMessage msg = new LoginMessage()
        {
            id = "test",
            password = ""
        };
        NetworkServer.SendToAll(msg);
    }

    private void SetNetworkHUD(int status)
    {
        if (status == 0)
        {
            //Init
            StartHostingBtn.gameObject.SetActive(true);
            StartServerBtn.gameObject.SetActive(true);
            StartClientBtn.gameObject.SetActive(true);
            StopHostingBtn.gameObject.SetActive(false);
            StopServerBtn.gameObject.SetActive(false);
            StopClientBtn.gameObject.SetActive(false);
            NameInputField.gameObject.SetActive(true);
        }
        else if (status == 1)
        {
            //Hosting
            StartHostingBtn.gameObject.SetActive(false);
            StartServerBtn.gameObject.SetActive(false);
            StartClientBtn.gameObject.SetActive(false);
            StopHostingBtn.gameObject.SetActive(true);
            StopServerBtn.gameObject.SetActive(false);
            StopClientBtn.gameObject.SetActive(false);
            NameInputField.gameObject.SetActive(false);
        }
        else if (status == 2)
        {
            //Server
            StartHostingBtn.gameObject.SetActive(false);
            StartServerBtn.gameObject.SetActive(false);
            StartClientBtn.gameObject.SetActive(false);
            StopHostingBtn.gameObject.SetActive(false);
            StopServerBtn.gameObject.SetActive(true);
            StopClientBtn.gameObject.SetActive(false);
            NameInputField.gameObject.SetActive(false);
        }
        else if (status == 3)
        {
            //Client
            StartHostingBtn.gameObject.SetActive(false);
            StartServerBtn.gameObject.SetActive(false);
            StartClientBtn.gameObject.SetActive(false);
            StopHostingBtn.gameObject.SetActive(false);
            StopServerBtn.gameObject.SetActive(false);
            StopClientBtn.gameObject.SetActive(true);
            NameInputField.gameObject.SetActive(false);
        }
    }

    public void OpenHost()
    {
        networkManager.StartHost();
        SetNetworkHUD(1);
    }

    public void CloseHost()
    {
        networkManager.StopHost();
        SetNetworkHUD(0);
    }

    public void OpenServer()
    {
        networkManager.StartServer();
        SetNetworkHUD(2);
    }

    public void CloseServer()
    {
        networkManager.StopServer();
        SetNetworkHUD(0);
    }

    public void ConnectClientToServer()
    {
        networkManager.StartClient();
        SetNetworkHUD(3);
    }

    public void DisconnectClientFromServer()
    {
        networkManager.StopClient();
        SetNetworkHUD(0);
    }

}
