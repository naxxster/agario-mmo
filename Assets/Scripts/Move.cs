using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;
using Mirror;

public class Move : NetworkBehaviour
{
    [SyncVar(hook = nameof(SetPos))]
    private Vector3 SyncPos;
    public float Speed;

    // Start is called before the first frame update
    void Start()
    {
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

    [Client]
    private void PlayerControl()
    {
        Vector3 Target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Target.z = transform.position.z;

        // When Client moves mouse pointer -> Send command to sync moved position to server
        // Server sync value -> Client hooks position
        Vector3 newPos = Vector3.MoveTowards(transform.position, Target, Speed * Time.deltaTime / transform.localScale.x);
        transform.position = newPos;
        // CmdMovePos(newPos);
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

    // Client Set GameObject(Prefab) Position from sync value
    private void SetPos(Vector3 oldPos, Vector3 newPos)
    {
        transform.position = newPos;
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
}
