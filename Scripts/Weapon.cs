using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Weapon : NetworkBehaviour {

    public bool isScatter;
    public bool isAuto;
    public bool canFire;
    public bool isReloading;
    public int numOfBulletsInShot;
    public float bulletSpeed;
    public float reloadTime;
    public float upperSpreadAngle;
    public float lowerSpreadAngle;
    public float fireRate;
    public GameObject bullet;
    public string weaponName;
    
    //Things to Sync
    [SyncVar]
    private float lastShot;
    [SyncVar (hook = "PlayerSyncVarHook")]
    public GameObject player;
    [SyncVar]
    public int ammo;
    [SyncVar]
    private int ammoToRestore;
    
    void Start()
    {
        ammoToRestore = ammo;
        lastShot = Time.time;

        // Solves sync problem where "new" players see weapons that "old" players picked up
        PlayerSyncVarHook(player);

        if (isServer)
        {
            if (player == null)
            {
                InitializeSelfDestruct();
            }
            else
            {
                DeinitializeSelfDestruct();
            }
        }
    }

    // Fires a bullet
    // Called on Server
    public void Fire()
    {
        if (ammo != 0 && canFire)
        {
            Transform bulletSpawn = player.transform.Find("Bullet Spawn");
            
            // Possible solution to fix server-sided self rotation and twitching
            //Transform bulletSpawn = new GameObject().transform;
            //bulletSpawn.position = bulletSpawnPosition;
            //bulletSpawn.rotation = bulletSpawnRotation;

            if ((Time.time - lastShot) > fireRate)
            {
                if (isScatter)
                {
                    Debug.Log("Firing weapon");
                    for (int i = 0; i < numOfBulletsInShot; i++)
                    {
                        // Spawns bullet
                        var tempBullet = (GameObject)Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);

                        // Sets Player who shot the bullet into the bullet
                        tempBullet.GetComponent<Bullet>().SetPlayerWhoFiredBullet(player);

                        // Gets Rigidbody2D compoenent from spawned bullet
                        Rigidbody2D tempBulletRB = tempBullet.GetComponent<Rigidbody2D>();

                        // Randomize angle variation between bullets
                        float spreadAngle = Random.Range(lowerSpreadAngle, upperSpreadAngle);

                        // Take the random angle variation and add it to the initial
                        // desiredDirection (which we convert into another angle), which in this case is the players aiming direction
                        var x = bulletSpawn.position.x - player.transform.position.x;
                        var y = bulletSpawn.position.y - player.transform.position.y;
                        float rotateAngle = spreadAngle + (Mathf.Atan2(y, x) * Mathf.Rad2Deg);

                        // Calculate the new direction we will move in which takes into account 
                        // the random angle generated
                        var MovementDirection = new Vector2(Mathf.Cos(rotateAngle * Mathf.Deg2Rad), Mathf.Sin(rotateAngle * Mathf.Deg2Rad)).normalized;

                        tempBulletRB.velocity = MovementDirection * bulletSpeed;
                        
                        NetworkServer.Spawn(tempBullet);
                        Destroy(tempBullet, 5.0f);
                    }
                    // Reduce current ammo by one
                    ammo--;
                    lastShot = Time.time;
                    
                }
                else
                {
                    Debug.Log("Firing weapon");
                    // Spawns bullet
                    var tempBullet = (GameObject)Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);

                    // Sets Player who shot the bullet into the bullet
                    tempBullet.GetComponent<Bullet>().SetPlayerWhoFiredBullet(player);

                    // Gets Rigidbody2D compoenent from spawned bullet
                    Rigidbody2D tempBulletRB = tempBullet.GetComponent<Rigidbody2D>();

                    // Have the bullet move in the direction the player is facing when it spawned
                    var x = bulletSpawn.position.x - player.transform.position.x;
                    var y = bulletSpawn.position.y - player.transform.position.y;
                    tempBulletRB.velocity = new Vector2(x, y) * bulletSpeed;

                    NetworkServer.Spawn(tempBullet);
                    Destroy(tempBullet, 5.0f);
                    
                    // Reduce current ammo by one
                    ammo--;
                    lastShot = Time.time;

                }
            }
        }
    }

    // Reloads Weapon
    // Called on Server
    public void Reload()
    {
        if (ammo < ammoToRestore && !isReloading)
        {
            canFire = false;
            isReloading = true;
            StartCoroutine(ReloadWeapon());
        }
    }

    private IEnumerator ReloadWeapon()
    {
        yield return new WaitForSeconds(reloadTime);

        ammo = ammoToRestore;
        canFire = true;
        isReloading = false;
        Debug.Log("Weapon reloaded: " + ammo);
    }

    public void PlaySound()
    {

    }

    public int GetAmmoToRestore()
    {
        return ammoToRestore;
    }

    public void PlayerSyncVarHook(GameObject player)
    {
        if (player != null)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    // IF weapon has no user, THEN invoke self destruct
    public void InitializeSelfDestruct()
    {
        Invoke("SelfDestruct", 30f);
    }
    
    // IF weapon has no user, THEN cancel self destruct
    public void DeinitializeSelfDestruct()
    {
        CancelInvoke();
    }

    void SelfDestruct()
    {
        NetworkServer.Destroy(gameObject);
    }

}