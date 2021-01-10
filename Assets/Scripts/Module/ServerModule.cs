using MLAPI;
using MLAPI.Messaging;
using MLAPI.Transports.UNET;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ServerModule : NetworkedBehaviour
{
    public GameObject FoodPrefab;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        NetworkingManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkingManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        string activeSceneName = SceneManager.GetActiveScene().name;
        if (activeSceneName == "Map001")
        {
            //Connect to server from Client
            NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress = "127.0.0.1";
            NetworkingManager.Singleton.GetComponent<UnetTransport>().ServerListenPort = 7777;

            if (Application.isBatchMode)
            {
                Debug.Log("Server Module Start at Port :7777 ");
                // Only run on Server mode
                NetworkingManager.Singleton.StartServer();
            }
        }
        else if (activeSceneName == "Map002")
        {
            //Connect to server from Client
            NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress = "127.0.0.1";
            NetworkingManager.Singleton.GetComponent<UnetTransport>().ServerListenPort = 8888;

            if (Application.isBatchMode)
            {
                Debug.Log("Server Module Start at Port :8888 ");
                // Only run on Server mode
                NetworkingManager.Singleton.StartServer();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnServerStarted()
    {
        //This runs at Server side
        Debug.Log("On Server Started");
#if UNITY_EDITOR
        //SpawnPlayer();
#endif
        StartCoroutine(SpawnFood());
    }

    private void OnClientConnected(ulong clientId)
    {
        //This runs at Client Side
        Debug.Log("On Client Connected - " + clientId);
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
}
