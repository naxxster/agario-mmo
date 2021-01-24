﻿using MLAPI;
using MLAPI.Messaging;
using MLAPI.Transports.UNET;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ServerModule : NetworkedBehaviour
{
    public static ServerModule Singleton { get; protected set; }
    public GameObject FoodPrefab;
    private GameLiftServer GameLift;

    private Dictionary<string, string> PlayerSessionMap = new Dictionary<string, string>();

    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Singleton = this;
        GameLift = new GameLiftServer();
    }

    // Start is called before the first frame update
    void Start()
    {
        NetworkingManager.Singleton.NetworkConfig.ConnectionApproval = true;
        NetworkingManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkingManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkingManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        NetworkingManager.Singleton.ConnectionApprovalCallback += OnConnectionApproved;

#if UNITY_EDITOR

#else
        if (Application.isBatchMode)
        {
            string activeSceneName = SceneManager.GetActiveScene().name;
            if (activeSceneName == "Map001")
            {
                //Connect to server from Client
                NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress = "0.0.0.0";
                NetworkingManager.Singleton.GetComponent<UnetTransport>().ServerListenPort = 7777;

                const int port = 7777;
                LogModule.WriteToLogFile("[ServerModule] Server Module Start at Port :7777 ");

                // Only run on Server mode
                if (GameLift.GameLiftStart(port))
                {
                    NetworkingManager.Singleton.StartServer();
                }
            }
            else if (activeSceneName == "Map002")
            {
                //Connect to server from Client
                NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress = "0.0.0.0";
                NetworkingManager.Singleton.GetComponent<UnetTransport>().ServerListenPort = 8888;

                const int port = 8888;
                LogModule.WriteToLogFile("[ServerModule] Server Module Start at Port :8888 ");

                // Only run on Server mode
                if (GameLift.GameLiftStart(port))
                {
                    NetworkingManager.Singleton.StartServer();
                }
            }
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnServerStarted()
    {
        //This runs at Server side
        LogModule.WriteToLogFile("[ServerModule] On Server Started");
#if UNITY_EDITOR
#endif
        StartCoroutine(SpawnFood());
    }

    private void OnClientConnected(ulong clientId)
    {
        //Called when Client connected
        LogModule.WriteToLogFile("[ServerModule] On Client Connected - " + clientId);
    }

    private void OnClientDisconnect(ulong clientId)
    {
        //Called when Client disconnected
        if (PlayerSessionMap.ContainsKey("" + clientId))
        {
            string playerSessionId = PlayerSessionMap["" + clientId];
            PlayerSessionMap.Remove("" + clientId);
#if UNITY_EDITOR
#else
            GameLift.RemovePlayer(playerSessionId);
#endif
        }
    }

    private void OnConnectionApproved(byte[] connectionData, ulong clientId, MLAPI.NetworkingManager.ConnectionApprovedDelegate callback)
    {
        LogModule.WriteToLogFile("[ServerModule] On Connection Approved");

        string connectionString = System.Text.Encoding.UTF8.GetString(connectionData);
        LogModule.WriteToLogFile("[ServerModule] Connection String - " + connectionString);

        string playerSessionId = connectionString;

        //If approve is true, the connection will be added. If it is false, the client gets disconnected
        bool approve = !string.IsNullOrEmpty(connectionString);

        if (!approve || !GameLift.AcceptPlayer(playerSessionId))
        {
            LogModule.WriteToLogFile("[ServerModule] Disconnect Client from server - clientId : " + clientId + ", playerSessionId : " + playerSessionId);
            approve = false;
        }
        else
        {
            approve = true;
        }
        bool createPlayerObject = approve;

        float z = 0;
        float x = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector3(0, 0, z)).x, Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, z)).x);
        float y = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector3(0, 0, z)).y, Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, z)).y);
        Vector3 randomPos = new Vector3(x, y, z);

        Vector3 spawnPosition = randomPos;
        if (approve)
        {
            LogModule.WriteToLogFile("[ServerModule] Spawn Position from Server - " + spawnPosition);
            PlayerSessionMap["" + clientId] = playerSessionId;
        }

        callback(createPlayerObject, null, approve, spawnPosition, null);
    }

    private IEnumerator SpawnFood()
    {
        for (int i = 0; i < 1000; i++)
        {
            int x = UnityEngine.Random.Range(0, Camera.main.pixelWidth);
            int y = UnityEngine.Random.Range(0, Camera.main.pixelHeight);

            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 0));
            pos.z = 0;

            //Implement Custom Prefab Spawning
            GameObject foodObj = Instantiate(FoodPrefab, pos, Quaternion.identity);
            //Spawn Food Management
            foodObj.GetComponent<NetworkedObject>().Spawn();
            if (i < 10)
            {
                yield return new WaitForSeconds(2.0f);
            }
            else if (i < 200)
            {
                yield return new WaitForSeconds(5.0f);
            }
            else
            {
                yield return new WaitForSeconds(10.0f);
            }
        }
    }

    public void OnApplicationQuit()
    {
#if UNITY_EDITOR
#else
        if (Application.isBatchMode)
        {
            GameLift.EndProcess();
        }
#endif
    }
}