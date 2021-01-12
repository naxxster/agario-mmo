using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : NetworkedBehaviour
{
    private float Speed = 5.0f;
    public string Food = "Food";
    public string Enemy = "Player";
    public float Increase = 0.1f;

    [SyncedVar]
    private int Score = 0;

    public Text Letters;

    // Allows you to change the channel position updates are sent on. Its prefered to be UnreliableSequenced for fast paced.
    public string Channel = "MLAPI_DEFAULT_MESSAGE";

    public override void NetworkStart()
    {
        // This is called when the object is spawned. Once this gets invoked. The object is ready for RPC and var changes.
        if (Letters == null)
        {
            Letters = GameObject.Find("ScoreText").GetComponentInChildren<Text>();
        }
        ServerInit();

        // TODO : 원격지로부터 데이터를 로드해서 기본 객체를 만드는데 사용할 
    }

    private void Update()
    {
        if (IsClient)
        {
            PlayerControl();
            PlayerCommand();
        }
    }

    private void ServerInit()
    {
        float z = transform.position.z;
        float x = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector3(0, 0, z)).x, Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, z)).x);
        float y = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector3(0, 0, z)).y, Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, z)).y);
        Vector3 randomPos = new Vector3(x, y, z);
        Debug.Log("Spawn pos : " + randomPos);
        transform.position = randomPos;
    }

    void OnTriggerEnter(Collider other)
    {
        InvokeServerRpc(DoEatServerRpc, other.gameObject);
    }

    private void PlayerControl()
    {
        float keyHorizontal = Input.GetAxis("Horizontal");
        float keyVertical = Input.GetAxis("Vertical");
        Vector3 moveVector =
            new Vector3(keyHorizontal * Speed * Time.deltaTime / transform.localScale.x, keyVertical * Speed * Time.deltaTime / transform.localScale.y, 0);
        transform.Translate(moveVector);
    }

    private void PlayerCommand()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space Enter");
            string activeSceneName = SceneManager.GetActiveScene().name;
            if (activeSceneName == "Map001")
            {
                ClientModule.Singleton.MoveToWorld("Map002");
            }
            else if (activeSceneName == "Map002")
            {
                ClientModule.Singleton.MoveToWorld("Map001");
            }
        }
    }

    [ServerRPC(RequireOwnership = true)]
    private void DoEatServerRpc(GameObject target)
    {
        // ServerRPC : Invoke by Client, Run on Server.
        if (transform.localScale.magnitude >= target.transform.localScale.magnitude)
        {
            Vector3 newSiz = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            if (target.tag == Food)
            {
                newSiz += new Vector3(Increase * 1, Increase * 1, Increase * 1);
                Score += 10;
            }
            else if (target.tag == Enemy)
            {
                newSiz += new Vector3(Increase * 10, Increase * 10, Increase * 10);
                Score += 100;
            }
            Destroy(target);
            transform.localScale = newSiz;
            InvokeClientRpcOnEveryone(DoEatClientRpc, target, newSiz);
        }
    }

    [ClientRPC]
    public void DoEatClientRpc(GameObject target, Vector3 size)
    {
        // This code gets ran on the clients at the request of the server.
        // ClientRPC : Invoke by server, Run on Client.
        Destroy(target);
        transform.localScale = size;
        Letters.text = "SCORE: " + Score;
    }
}
