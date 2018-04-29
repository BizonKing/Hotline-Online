using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Barebones.MasterServer;

public class NewNetworkManager : NetworkManager
{
    public UIManager UI;
    public UnetGameRoom GameRoom;
    public GameObject henchmen;
    public GameObject pistol;
    public GameObject mac10;
    public GameObject shotgun;
    
    void Awake()
    {
        if (GameRoom == null)
        {
            Debug.LogError("Game Room property is not set on NetworkManager");
            return;
        }

        // Subscribe to events
        GameRoom.PlayerJoined += OnPlayerJoined;
        GameRoom.PlayerLeft += OnPlayerLeft;
    }
    
    // Called on server when client joins server
    private void OnPlayerJoined(UnetMsfPlayer player)
    {
        Debug.Log("Player joined");
        return;
    }

    // Called on server when client joins server
    private void OnPlayerLeft(UnetMsfPlayer player)
    {
        Debug.Log("Player joined");
        return;
    }

    // Called on the client when connected to a server.
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        // Test of "reconstructing" states of items

        if (autoCreatePlayer == true)
        {
            // Spawn Player
            Debug.Log("Auto Create Player was set to true, so player should have spawned in");
        }
        else
        {
            Debug.Log("Auto Create Player was set to false, so player should not had spawned in");
            UI.TurnOnCharacterSelectionUI();
        }

    }

    // Called on server when a client disconnects
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);

        // Don't forget to notify the room that a player disconnected
        GameRoom.ClientDisconnected(conn);
    }

    // Called on the server when a client adds a new player with ClientScene.AddPlayer
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        //base.OnServerAddPlayer(conn, playerControllerId);

        SpawnPlayer(conn, playerControllerId);

        Debug.Log("Should see this from override method");
    }

    // In here, setups the player
    // Called on the server
    void SpawnPlayer(NetworkConnection conn, short playerControllerId)
    {
        // GetStartPosition() sets the transform of the object

        var player = (GameObject)GameObject.Instantiate(playerPrefab, GetStartPosition().position, Quaternion.identity);
        
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

    // Adds Player to Game
    // Called on Client
    public void AddPlayer()
    {
        // Gets result from adding in the player
        CharacterSelectionMenu cs = UI.GetCharacterSelectionUI().GetComponent<CharacterSelectionMenu>();
        string character = cs.GetCharacter();

        if (character.Equals("The Henchmen"))
        {
            playerPrefab = henchmen;
        }
        
        bool playerAdded = ClientScene.AddPlayer(0);
    }

}
