using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/**
 * 
 * THIS WILL BE SERVER SIDED
 * 
 **/
public class WeaponSpawner : NetworkBehaviour {

    public GameObject weapon;
    public float SpawnTime;
    private float lastSpawnTime;

    // When the server starts, spawns in all weapons
    public override void OnStartServer()
    {
        GameObject tempWeapon = (GameObject)Instantiate(weapon, gameObject.transform.position, gameObject.transform.rotation);
        lastSpawnTime = Time.time;
        NetworkServer.Spawn(tempWeapon);
        Debug.Log("Spawning a weapon");
    }

    // Update is called once per frame
    void Update () {

        if (isServer)
        {
            if ((Time.time - lastSpawnTime) > SpawnTime)
            {
                SpawnWeapon();
            }
        }
	}
    
    // Spawns weapon when ready
    void SpawnWeapon()
    {
        GameObject tempWeapon = (GameObject)Instantiate(weapon, gameObject.transform.position, gameObject.transform.rotation);
        lastSpawnTime = Time.time;
        NetworkServer.Spawn(tempWeapon);
        Debug.Log("Spawning a weapon");
    }
}
