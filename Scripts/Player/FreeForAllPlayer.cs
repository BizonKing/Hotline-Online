using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FreeForAllPlayer : Player {

    public UIManager UI;
    public PlayerInformationUICanvas playerInformationUI;

    // Used for initializing the local player (basically the client)
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        LocalPlayerSetUp();
    }

    // Use this for initializing nonlocalPlayer objects
    // Mostly things that cannot use SyncVar
    // If you can use SyncVar, not need to use this
    public override void Start()
    {
        base.Start();

        if (isLocalPlayer)
        {
            return;
        }
        
        SetUpPlayerInformationUI(screenName);
    }

    void LocalPlayerSetUp()
    {
        // Sets up Information UI on Client
        UI = FindObjectOfType<UIManager>();
        UI.TurnOffCharacterSelectionUI();
        UI.SetPlayer(gameObject);
        UI.TurnOnInformationUI();

        // Gets CharacterSelectionMenu
        GameObject characterSelectionMenu = UI.GetCharacterSelectionUI();
        // Gets username choosen from character selection menu
        string username = characterSelectionMenu.GetComponent<CharacterSelectionMenu>().GetUserName();
        // Gets the choosen starter weapon
        choosenStarterWeapon = characterSelectionMenu.GetComponent<CharacterSelectionMenu>().GetWeapon();
        // Sets username
        screenName = username;
        Debug.Log("Username: " + username);
        Debug.Log("Choosen weapon: " + choosenStarterWeapon);

        // Sets up screen name
        CmdSetScreenName(screenName);
        // Sets up PlayerInformationUI
        CmdSetUpPlayerInformationUI(screenName);
        CmdSetWeaponForPlayer(gameObject, choosenStarterWeapon);
    }
    
    [Command]
    void CmdSetUpPlayerInformationUI(string screenName)
    {
        SetUpPlayerInformationUI(screenName);
        RpcSetUpPlayerInformationUI(screenName);
    }

    [ClientRpc]
    void RpcSetUpPlayerInformationUI(string screenName)
    {
        SetUpPlayerInformationUI(screenName);
    }
    
    void SetUpPlayerInformationUI(string screenName)
    {
        // Get PlayerInformationUI
        playerInformationUI = GetComponentInChildren<PlayerInformationUICanvas>();
        // Sets player to player information
        playerInformationUI.SetPlayer(gameObject);
        // Set username of player
        playerInformationUI.SetScreenNameText(screenName);

    }
    
    // Does damage to player
    // Called on server
    public override void TakeDamage(int damage, GameObject playerWhoFiredBullet)
    {
        if (!isServer)
        {
            return;
        }

        health -= damage;
        if (health <= 0 && dead == false)
        {
            dead = true;
            GetComponent<BoxCollider2D>().enabled = false;

            FreeForAllGameManager gameManager = FindObjectOfType<FreeForAllGameManager>();
            gameManager.RegisterDeath(gameObject, playerWhoFiredBullet);
            
            RpcDead();
        }
    }

    [ClientRpc]
    protected override void RpcDead()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        CmdDropWeapon(aimingDirection, gameObject.transform.position);
    }
    
}
