using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FreeForAllGameManager : NetworkBehaviour {

    public KillfeedUI killfeedUI;
    public GameObject pistol;
    public GameObject mac10;
    public GameObject shotgun;

    void Start()
    {
        killfeedUI = FindObjectOfType<UIManager>().GetKillFeed().GetComponent<KillfeedUI>();
    }

    // Registers a death that occurred
    // Called on Server
    public void RegisterDeath(GameObject playerThatDied, GameObject playerThatKilledOtherPlayer)
    {
        if (!isServer)
        {
            return;
        }

        playerThatKilledOtherPlayer.GetComponent<Player>().AddElimination();
        NewKillFeedMessage(playerThatDied, playerThatKilledOtherPlayer);
        RpcNewKillFeedMessage(playerThatDied, playerThatKilledOtherPlayer);
    }

    // Tells client that a new kill feed message needs to be made
    // Called on Client
    [ClientRpc]
    void RpcNewKillFeedMessage(GameObject playerThatDied, GameObject playerThatKilledOtherPlayer)
    {
        NewKillFeedMessage(playerThatDied, playerThatKilledOtherPlayer);
    }

    public void NewKillFeedMessage(GameObject playerThatDied, GameObject playerThatKilledOtherPlayer)
    {
        Player playerThatDiedInfo = playerThatDied.GetComponent<Player>();
        Player playerThatKilledOtherPlayerInfo = playerThatKilledOtherPlayer.GetComponent<Player>();

        killfeedUI.OnKill(playerThatDied, playerThatKilledOtherPlayer);
        Debug.Log(playerThatKilledOtherPlayerInfo.GetScreenName() + " eliminated " + playerThatDiedInfo.GetScreenName());
    }

    // Sets weapon for player
    // Called on Server
    public void SetWeaponForPlayer(GameObject player, string weapon)
    {
        GameObject tempWeapon = null;
        //FreeForAllPlayer playerScript = player.GetComponent<FreeForAllPlayer>();
        Player playerScript = player.GetComponent<Player>();
        playerScript.choosenStarterWeapon = weapon;

        if (weapon.Equals("Pistol"))
        {
            tempWeapon = (GameObject)Instantiate(pistol);
        }
        if (weapon.Equals("Mac 10"))
        {
            tempWeapon = (GameObject)Instantiate(mac10);
        }
        if (weapon.Equals("Shotgun"))
        {
            tempWeapon = (GameObject)Instantiate(shotgun);
        }

        // Need to configure and spawn weapon to network first or else it will be null on the player
        tempWeapon.GetComponent<Weapon>().player = player;
        NetworkServer.Spawn(tempWeapon);
        playerScript.weapon = tempWeapon;
        
    }

}
