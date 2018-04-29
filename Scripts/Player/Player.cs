using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
/**
 * 
 * SyncVar - Tells the server to sync variable to all clients (syncs from server to client)
 * Command - Code that sends stuff to the server
 * ClientRpc - code that all clients will use
 *
 * */
public class Player : NetworkBehaviour {

    // Player information Fields
    [SyncVar]
    public string screenName;
    [SyncVar]
    public int numberOfEliminations;
    // Player weapon fields
    // NOTE: cannot use SyncVar attribute with weaponScript as SyncVars cannot be used with scripts inheriting NetworkBehaviour
    [SyncVar]
    public GameObject weapon;
    // Player health fields
    [SyncVar]
    public int health;
    [SyncVar]
    public bool dead;
    // Starter weapon
    [SyncVar]
    public string choosenStarterWeapon;
    [SyncVar]
    public float movementSpeed = 2.0f;

    // Movement speed, direction and animator fields

    public Animator animator;
    public Rigidbody2D rigidbody2D;
    public Vector3 aimingDirection;
    // Player camera field
    public CameraScript cameraScript;
    // Added for the escape menu (DO NOT USE THIS FOR STUFF LIKE STUNS!)
    public bool canMove;
    // Added for future stun items
    public bool stunned;

    // USE THIS FOR INITIALIZION
    public override void OnStartLocalPlayer()
    {
        cameraScript = Camera.main.GetComponent<CameraScript>();
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        canMove = true;

        // Sets the transform of the camera to the player
        cameraScript.setTarget(transform);

        // Player now depends on CharacterSelectionMenu in order for screen names to be synced correctly.
        // CharacterSelectionMenu now has a NetworkIdentity so local player instance of Player can find it.
        // CharacterSelectionMenu needs to have SetActive(true) so Player can find it.
        // Refer to: http://www.doofah.com/tutorials/networking/unity-5-network-tutorial-part-3/
        
        Debug.Log("Local player initialized");
    }

    public virtual void Start()
    {
        // Set Rigidbody2D to character prefab already
        if (isServer)
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
        }
    }

    // ONLY USE THIS FOR ANIMATIONS AND WEAPONS
    protected virtual void Update()
    {
		
        if (!isLocalPlayer)
        {
            return;
        }
        if (dead && Input.GetKeyUp(KeyCode.R))
        {
            CmdRespawn();
        }
        if (dead)
        {
            return;
        }
        if (!canMove)
        {
            return;
        }
        
		// Weapon Actions -> Firing weapon, reloading weapon
		// Weapon Animations -> What weapon the player is holding, shows player reloading
		if (weapon != null)
		{
			Weapon weaponScript = weapon.GetComponent<Weapon>();
			
			if (Input.GetKey(KeyCode.Mouse0) && weaponScript.isAuto)
			{
				if (weapon != null)
				{
					CmdFireWeapon();
				}
			}
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				if (weapon != null)
				{
					CmdFireWeapon();
				}
			}
			if (Input.GetKeyUp(KeyCode.R))
			{
				if (weapon != null)
				{
					CmdReloadWeapon();
				}
			}
			if (Input.GetKeyDown(KeyCode.Mouse1))
			{
				if (weapon == null)
				{
					CmdPickUpWeapon(gameObject);
				}
				else
				{
					CmdDropWeapon(aimingDirection, transform.position);
				}
			}
			
			/*** Animation Code ***/
			if (weaponScript.weaponName.Equals("Pistol"))
			{
				animator.SetBool("hasPistol", true);
                animator.SetBool("hasShotgun", false);
                animator.SetBool("hasMac10", false);
			}
			if(weaponScript.weaponName.Equals("Mac 10"))
			{
				animator.SetBool("hasPistol", false);
                animator.SetBool("hasShotgun", false);
                animator.SetBool("hasMac10", true);
			}
			if(weaponScript.weaponName.Equals("Shotgun"))
			{
				animator.SetBool("hasPistol", false);
                animator.SetBool("hasShotgun", true);
                animator.SetBool("hasMac10", false);
			}

            if (Input.GetKey(KeyCode.Mouse0))
            {
                animator.SetBool("attacking", true);
            }
            else
            {
                animator.SetBool("attacking", false);
            }

            if (Input.GetKey(KeyCode.R))
            {
                animator.SetBool("reloading", true);
            }
            else
            {
                animator.SetBool("reloading", false);
            }
		}
		else
		{
			animator.SetBool("hasPistol", false);
            animator.SetBool("hasShotgun", false);
            animator.SetBool("hasMac10", false);
			animator.SetBool("reloading", false);
		}
		
		// Movement animations
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            animator.SetBool("moving", true);
        }
        else
        {
            animator.SetBool("moving", false);
        }

    }

    // ONLY USE THIS FOR PHYSICS (LIKE MOVEMENT)
    protected virtual void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            // if statement is here to prevent player from spinning on server (affects aim as aim is server sided)
            if (isServer)
            {
                if (rigidbody2D.angularVelocity != 0)
                {
                    rigidbody2D.angularVelocity = 0;
                }
                return;
            }
            return;
        }
        if (dead)
        {
            return;
        }
        if (!canMove)
        {
            return;
        }

        aimingDirection = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(aimingDirection.y, aimingDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        /*
         * 
         * SPECIAL NOTE HERE!
         * 
         * IF YOU WANT ACCELERATION/DECELERATION USE GetAxis();
         * OTHERWISE USE GetAxisRaw();
         * 
         * */
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            var currentVelocity = rigidbody2D.velocity;
            currentVelocity.x = Input.GetAxisRaw("Horizontal") * movementSpeed;
            currentVelocity.y = Input.GetAxisRaw("Vertical") * movementSpeed;
            rigidbody2D.velocity = currentVelocity;
        }
        else
        {
            // Fixes physics bug where player would spin rapidly and twitching randomly or after being shot
            rigidbody2D.angularVelocity = 0;
            rigidbody2D.velocity = new Vector2(0, 0);
        }
    }
    
    protected virtual void Animations()
    {
        if (weapon != null)
        {
            Weapon weaponScript = weapon.GetComponent<Weapon>();

            /*** Animation Code ***/
            if (weaponScript.weaponName.Equals("Pistol"))
            {
                animator.SetBool("hasPistol", true);
                animator.SetBool("hasShotgun", false);
                animator.SetBool("hasMac10", false);
            }
            if (weaponScript.weaponName.Equals("Mac 10"))
            {
                animator.SetBool("hasPistol", true);
                animator.SetBool("hasShotgun", false);
                animator.SetBool("hasMac10", false);
            }
            if (weaponScript.weaponName.Equals("Shotgun"))
            {
                animator.SetBool("hasPistol", true);
                animator.SetBool("hasShotgun", false);
                animator.SetBool("hasMac10", false);
            }

            if (Input.GetKey(KeyCode.R))
            {
                animator.SetBool("reloading", true);
            }
            else
            {
                animator.SetBool("reloading", false);
            }
        }
        else
        {
            animator.SetBool("hasPistol", false);
            animator.SetBool("hasShotgun", false);
            animator.SetBool("hasMac10", false);
            animator.SetBool("reloading", false);
        }

        // Movement animations
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            animator.SetBool("moving", true);
        }
        else
        {
            animator.SetBool("moving", false);
        }
    }

    // Sets screen name of the player
    [Command]
    protected virtual void CmdSetScreenName(string username)
    {
        screenName = username;
    }

	/***** Look over CmdSetWeaponForPlayer and SetWeaponForPlayer (the logic does not appear right and will cause bugs) *****/
    [Command]
    public void CmdSetWeaponForPlayer(GameObject player, string weapon)
    {
        SetWeaponForPlayer(player, weapon);
    }

    protected virtual void SetWeaponForPlayer(GameObject player, string weapon)
    {
        if (this.weapon != null)
        {
            choosenStarterWeapon = weapon;
            return;
        }

        FreeForAllGameManager gm = FindObjectOfType<FreeForAllGameManager>();
        gm.SetWeaponForPlayer(player, weapon);
    }

    // Fires the weapon the player is holding
    [Command]
    protected virtual void CmdFireWeapon()
    {
        weapon.GetComponent<Weapon>().Fire();
    }
    
    // Reloads the weapon the player is holding
    [Command]
    protected virtual void CmdReloadWeapon()
    {
        weapon.GetComponent<Weapon>().Reload();
    }

    // Tells server to pick up weapon.
    [Command]
    protected virtual void CmdPickUpWeapon(GameObject player)
    {
        //Array of RaycastHit2D objects that is filled with details on any hits from raycasts made
        RaycastHit2D[] hits = Physics2D.RaycastAll(this.gameObject.transform.position, Vector2.zero);
        Player playerScript = player.GetComponent<Player>();

		// Loop processes the items in the array to see if there is a weapon for the player to pick up
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.tag == "Weapon")
            {
                Debug.Log("Picked up weapon");

                var weapon = hit.collider.gameObject;

                playerScript.weapon = hit.collider.gameObject;
                playerScript.weapon.GetComponent<Weapon>().player = gameObject;
                playerScript.weapon.GetComponent<Weapon>().DeinitializeSelfDestruct();

                // Going to keep these two lines in case players want be hosts (server and client)
                playerScript.weapon.GetComponent<BoxCollider2D>().enabled = false;
                playerScript.weapon.GetComponent<SpriteRenderer>().enabled = false;
                
                break;
            }
        }
    }
    
    // Tells server to drop weapon. Passes in the direction the player is looking at and the player position at the moment of call
    [Command]
    protected virtual void CmdDropWeapon(Vector3 throwingDirection, Vector3 playerPosition)
    {
        DropWeapon(throwingDirection, playerPosition);
    }

    protected virtual void DropWeapon(Vector3 throwingDirection, Vector3 playerPosition)
    {
        // All the physics to throw weapon
        Debug.Log("Throwing Direction Recieved: " + throwingDirection.ToString());
        Debug.Log("Player Position Recieved: " + playerPosition.ToString());

        Rigidbody2D weaponRB = weapon.GetComponent<Rigidbody2D>();
        Transform weaponTransform = weapon.GetComponent<Transform>();
        float throwingForce = 1F;
        weaponTransform.position = playerPosition;

        // Again, keeping these lines in case players can be hosts
        weapon.GetComponent<BoxCollider2D>().enabled = true;
        weapon.GetComponent<SpriteRenderer>().enabled = true;
        
		weapon.GetComponent<Weapon>().InitializeSelfDestruct();

        weaponRB.velocity = Vector3.zero;
        weaponRB.angularVelocity = 0;
        weaponRB.AddTorque(Random.Range(-2f, 2f), ForceMode2D.Impulse);
        weaponRB.AddForce(throwingDirection * throwingForce, ForceMode2D.Impulse);

        weapon.GetComponent<Weapon>().player = null;
        weapon = null;
    }

    // Does damage to player when a bullet hits them
    // NOTE: Since this method occurs on the server, RPCs are allowed but COMMANDs aren't
    public virtual void TakeDamage(int damage, GameObject playerWhoFiredBullet)
    {
        if (!isServer)
        {
            return;
        }

        health -= damage;
        if (health <= 0 && dead == false)
        {
            Player playerWhoFiredBulletInfo = playerWhoFiredBullet.GetComponent<Player>();
            dead = true;
            GetComponent<BoxCollider2D>().enabled = false;
            RpcDead();
        }
    }

    // Sets the player as dead and syncs it to clients
    [ClientRpc]
    protected virtual void RpcDead()
    {
        // Should have some default action in here
    }

    // Tells server to respawn player
    [Command]
    protected virtual void CmdRespawn()
    {
        // Sets new network position first
        NetworkStartPosition[] startPositions = FindObjectsOfType<NetworkStartPosition>();
        transform.position = startPositions[Random.Range(0, startPositions.Length)].transform.position;

        // Fixes physics bug where player would spin rapidly and twitching randomly or after being shot
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.angularVelocity = 0;
        rigidbody2D.velocity = new Vector2(0, 0);
        transform.rotation = Quaternion.identity;

        dead = false;
        health = 1;
        GetComponent<BoxCollider2D>().enabled = true;
        SetWeaponForPlayer(gameObject, choosenStarterWeapon);

        Debug.Log("Respawning " + screenName + "...");
        RpcRespawn(transform.position, rigidbody2D.velocity, transform.rotation);
    }

    // Respawns the player and syncs it to clients
    [ClientRpc]
    protected virtual void RpcRespawn(Vector3 newPosition, Vector2 newRotationVelocity, Quaternion newRotation)
    {
        transform.position = newPosition;

        // Fixes physics bug where player would spin rapidly and twitching randomly or after being shot
        rigidbody2D.velocity = newRotationVelocity;
        transform.rotation = newRotation;

        GetComponent<BoxCollider2D>().enabled = true;
    }

    public string GetScreenName()
    {
        return screenName;
    }

    public int GetNumberOfEliminations()
    {
        return numberOfEliminations;
    }

    public GameObject GetWeapon()
    {
        return weapon;
    }
    
    public int GetHealth()
    {
        return health;
    }
    
    public bool GetDead()
    {
        return dead;
    }

    // Called on server
    public void AddElimination()
    {
        numberOfEliminations++;
    }

    // Says whether the player can or cannot move
    public void SetCanMove(bool result)
    {
        canMove = result;
    }
	
	// Says whether the player is stunned or not
	public void SetStunned(bool result)
	{
		stunned = result;
	}
}
