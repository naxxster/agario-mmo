using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Move : NetworkBehaviour
{
    [SyncVar(hook = nameof(SetPos))]
    private Vector3 SyncPos;
    public float Speed;

    [SyncVar(hook = nameof(SetPlayerName))]
    private string PlayerName;

    public ClientInstance MasterClient;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Local Player Start");
        MasterClient = GameObject.Find("ClientInstance").GetComponent<ClientInstance>();
        CmdChangeName(MasterClient.PlayerName);
        if (isServer)
        {
            ServerInit();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority) { return ; }

        if (isLocalPlayer)
        {
            PlayerControl();
            KeyControl();
        }
    }

    void OnGUI()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
        int labelWidth = 100;
        int labelHeight = 60;
        GUI.Label(new Rect(screenPos.x - (labelWidth/4), Screen.height - screenPos.y - (labelHeight/4), labelWidth, labelHeight), PlayerName);
    }

    private void ServerInit()
    {
        float z = transform.position.z;
        float x = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector3(0, 0, z)).x, Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, z)).x);
        float y = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector3(0, 0, z)).y, Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, z)).y);
        Vector3 randomPos = new Vector3(x, y, z);
        SyncPos = randomPos;
        Debug.Log("Spawn pos : " + randomPos);

    }

    [Client]
    private void PlayerControl()
    {
        Vector3 Target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Target.z = transform.position.z;

        // When Client moves mouse pointer -> Send command to sync moved position to server
        // Server sync value -> Client hooks position
        Vector3 newPos = Vector3.MoveTowards(transform.position, Target, Speed * Time.deltaTime / transform.localScale.x);
        transform.position = newPos;
    }

    [Client]
    private void KeyControl()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            NetworkManager.singleton.StopClient();      //Disconnect Client (Client)
            //NetworkManager.singleton.StopHost();      //Disconnect Host (Server)
        }
    }

    [Command]
    private void CmdChangeName(string playerName)
    {
        Debug.Log("PlayerName : " + playerName);
        PlayerName = playerName;
    }

    // Client Set GameObject(Prefab) Position from sync value
    private void SetPos(Vector3 oldPos, Vector3 newPos)
    {
        transform.position = newPos;
    }

    private void SetPlayerName(string oldLabel, string newLabel)
    {
        Debug.Log("Sync Callback : " + newLabel);
    }
}
