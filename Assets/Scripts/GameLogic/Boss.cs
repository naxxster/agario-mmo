using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

public class Boss : NetworkedBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Boss Spawned!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        InvokeClientRpcOnEveryone(DoEatClientRpc, other.gameObject);
    }

    [ClientRPC]
    private void DoEatClientRpc(GameObject target)
    {
        // ClientRPC : Invoke by Server, Run on Client.
        if (transform.localScale.magnitude >= target.transform.localScale.magnitude)
        {
            Vector3 newSiz = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            if (target.tag == "Player")
            {
                newSiz += new Vector3(1, 1, 1);
                Destroy(target);
                transform.localScale = newSiz;
                InvokeServerRpc(DoEatServerRpc, target, newSiz);
            }
        }
        else
        {
            //Boss Depeated
            Destroy(gameObject);
            InvokeServerRpc(BossDefeatServerRpc);
        }
    }

    [ServerRPC(RequireOwnership = true)]
    private void DoEatServerRpc(GameObject target, Vector3 size)
    {
        Destroy(target);
        transform.localScale = size;
    }

    [ServerRPC(RequireOwnership = true)]
    private void BossDefeatServerRpc()
    {
        Debug.Log("Boss Defeat Action");
    }
}
