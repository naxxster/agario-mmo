using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Mirror;

public class Spawn : NetworkBehaviour
{
    public GameObject food;
    public float speed;
    private NetworkManager networkManager;
    private int spawnerCount = 0;

    void Start()
    {
        networkManager = NetworkManager.singleton;
        Debug.Log("Spawner On");
        StartCoroutine(SpawnFood());
    }

    private IEnumerator SpawnFood()
    {
        while(spawnerCount < 100)
        {
            int x = UnityEngine.Random.Range(0, Camera.main.pixelWidth);
            int y = UnityEngine.Random.Range(0, Camera.main.pixelHeight);

            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 0));
            pos.z = 0;

            //Implement Custom Prefab Spawning
            GameObject foodObj = Instantiate(food, pos, Quaternion.identity);
            //Spawn Food Management
            NetworkServer.Spawn(foodObj);
            spawnerCount += 1;
            yield return new WaitForSeconds(1.0f);
        }
    }

    public void DestroySpawnObject(GameObject target)
    {
        spawnerCount -= 1;
        NetworkServer.Destroy(target);
    }
}
