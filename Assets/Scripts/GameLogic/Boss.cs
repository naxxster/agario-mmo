using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using UnityEngine;

public class Boss : NetworkedBehaviour
{
    private Vector3 BossVector;

    // Start is called before the first frame update
    void Start()
    {
    }

    public override void NetworkStart()
    {
        // This is called when the object is spawned. Once this gets invoked. The object is ready for RPC and var changes.
        Debug.Log("Boss Spawned!");
        PickPosition();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDestroy()
    {
        ClientModule.Singleton.PlayerStatus = ClientModule.PlayStatus.WIN;      //Boss Destroy => Player Win
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            InvokeClientRpcOnEveryone(DoEatClientRpc, other.gameObject);
        }
    }

    private void PickPosition()
    {
        float z = transform.position.z;
        float x = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector3(0, 0, z)).x, Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, z)).x);
        float y = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector3(0, 0, z)).y, Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, z)).y);

        BossVector = new Vector3(x, y, z);
        StartCoroutine(Wandering());
    }

    IEnumerator Wandering()
    {
        float i = 0.0f;
        float rate = 1.0f / 1.0f;
        Vector3 currentPos = transform.position;

        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            transform.position = Vector3.Lerp(currentPos, BossVector, i);
            yield return null;
        }

        float randomFloat = Random.Range(0.0f, 1.0f); // Create %50 chance to wait
        if (randomFloat < 0.5f)
            StartCoroutine(WaitForSomeTime());
        else
            PickPosition();
    }

    IEnumerator WaitForSomeTime()
    {
        yield return new WaitForSeconds(5.0f);
        PickPosition();
    }


    [ClientRPC]
    private void DoEatClientRpc(GameObject target)
    {
        // ClientRPC : Invoke by Server, Run on Client.
        if (target != null)
        {
            if (transform.localScale.magnitude >= target.transform.localScale.magnitude)
            {
                //Boss Destroy Everything!
                Destroy(target);
                InvokeServerRpc(DoEatServerRpc, target);
            }
            else
            {
                //Boss Defeated
                Destroy(gameObject);
                InvokeServerRpc(BossDefeatServerRpc);
            }
        }
    }

    [ServerRPC(RequireOwnership = true)]
    private void DoEatServerRpc(GameObject target)
    {
        Destroy(target);
    }

    [ServerRPC(RequireOwnership = true)]
    private void BossDefeatServerRpc()
    {
        Debug.Log("Boss Defeat Action");
    }
}
