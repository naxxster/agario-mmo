using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Eat : NetworkBehaviour
{
    public string Food;
    public string Enemy;
    public float Increase;
    public Text Letters = null;

    [SyncVar(hook = nameof(SetSize))]
    private Vector3 SyncSize;
    [SyncVar(hook = nameof(SetScore))]
    private int Score = 0;

    private Spawn spawn;

    void Start()
    {
        if (Letters == null)
        {
            Letters = GameObject.Find("ScoreText").GetComponentInChildren<Text>();
        }
        if (isServer)
        {
            ServerInit();
        }
        spawn = GameObject.Find("SpawnManager").GetComponent<Spawn>();
    }

    void OnTriggerEnter(Collider other)
    {
        DoEat(other.gameObject);
    }

    private void SetScore(int oldScore, int newScore)
    {
        Letters.text = "SCORE: " + newScore;
    }

    private void SetSize(Vector3 oldSize, Vector3 newSize)
    {
        // Debug.Log(newSize);
        transform.localScale = newSize;
    }

    [Client]
    private void DoEat(GameObject gameObject)
    {
        if (!isLocalPlayer)
        {
            return ;
        }
        if (transform.localScale.magnitude >= gameObject.transform.localScale.magnitude)
        {
            if (gameObject.tag == Food)
            {
                CmdEatFood(gameObject, 1);
            }
            else if (gameObject.tag == Enemy)
            {
                CmdEatFood(gameObject, 10);
            }
        }
    }

    [Command]
    private void CmdEatFood(GameObject target, float multiply)
    {
        float newIncrease = Increase * multiply;
        SyncSize += new Vector3(newIncrease, newIncrease, newIncrease);

        Score += (10 * (int)multiply);
        //Call server to destroy managed spawned object.
        spawn.DestroySpawnObject(target);
    }

    private void ServerInit()
    {
        Vector3 initSize = new Vector3(1, 1, 1);
        SyncSize = initSize;
    }
}
