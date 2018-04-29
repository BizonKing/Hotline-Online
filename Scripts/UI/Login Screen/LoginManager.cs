using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.MasterServer;

public class LoginManager : MonoBehaviour {

    public delegate void OnMatchMakerStartCallback();
    public delegate void OnMatchMakerStopCallback();
    // Methods register in: LoginScreen
    public OnMatchMakerStartCallback OnMatchMakerStart;
    // Methods register in: LoginScreen
    public OnMatchMakerStopCallback OnMatchMakerStop;

    public SpawnRequestController spawnRequest;
    public string status;

    //If the GetAccess call returns properly, everything will be handled automatically, otherwise, we display the error
    protected virtual void OnPassReceived(RoomAccessPacket packet, string errorMessage)
    {
        if (packet == null)
        {
            Msf.Events.Fire(Msf.EventNames.ShowDialogBox, DialogBoxData.CreateError(errorMessage));
            Logs.Error(errorMessage);
            status = "ERROR: No valid room access packet, please try again";
            return;
        }
    }

    // Creates a game
    void CreateGame()
    {

        if (!Msf.Client.Auth.IsLoggedIn)
        {
            Debug.LogError("You're not logged in");
            return;
        }
        else
        {
            Debug.Log("Logged in as guest");
        }

        // These options will be send to spawner, and then passed
        // to spawned process, so that it knows what kind of game server to start.
        // You can add anything to this dictionary
        status = "Setting up game options";
        var spawnOptions = new Dictionary<string, string>
        {
                {MsfDictKeys.MaxPlayers, "5"},
                {MsfDictKeys.RoomName, System.Guid.NewGuid().ToString()},
                {MsfDictKeys.MapName, "Random"},

                // Make sure you set this right, and that this scene
                // is added to the build of your game server
                {MsfDictKeys.SceneName, "Random"}
        };

        var region = "International";
        status = "Sending game creation request";
        // Send the request to spawn a game server
        Msf.Client.Spawners.RequestSpawn(spawnOptions, region, (controller, error) =>
        {
            if (controller == null)
            {
                status = "ERROR: Game request failed, please try again";
                Debug.LogError("Failed: " + error);
                return;
            }

            // If we got here, the request is being handled, but we need
            // to wait until it's done
            // We'll start a coroutine for that (they are perfect for waiting ^_^)
            StartCoroutine(WaitForServerToBeFinalized(controller));
        });
    }

    // Waits for the new game server to be ready
    private IEnumerator WaitForServerToBeFinalized(SpawnRequestController request)
    {
        var currentStatus = request.Status;
        status = "Finalizing game request";
        spawnRequest = request;
        // Keep looping until spawn request is finalized
        // (if spawn request is aborted, this will loop infinitely, 
        // because request will never be finalized, but I think you'll know how to
        // handle it)
        while (request.Status != SpawnStatus.Finalized && request.Status != SpawnStatus.Aborted)
        {
            // Skip a frame, if it's still not finalized
            yield return null;

            // If status has changed
            if (currentStatus != request.Status)
            {
                Debug.Log("Status changed to: " + request.Status);
                if (request.Status == SpawnStatus.Aborted || request.Status == SpawnStatus.Killed)
                {
                    status = "ERROR: Problem when finalizing game spawn request";
                }
                currentStatus = request.Status;
            }
        }

        // If we got here, the spawn request has been finalized

        // When spawned process finalizes, it gives master server some,
        // information about itself, which includes room id

        // We can retrieve this data from master server
        request.GetFinalizationData((data, error) =>
        {
            if (data == null)
            {
                Debug.LogError("Failed to get finalization data: " + error);
                status = "ERROR: Game request failed to finalize, please try again";
                return;
            }

            if (!data.ContainsKey(MsfDictKeys.RoomId))
            {
                Debug.LogError("Spawned server didn't add a room ID to finalization data");
                return;
            }

            // So we've received the roomId of the game server that
            // we've requested to spawn
            var roomId = int.Parse(data[MsfDictKeys.RoomId]);
            status = "Game finalized, requesting access";
            GetRoomAccess(roomId);
        });

        spawnRequest = null;
    }

    // Accesses the server
    void GetRoomAccess(int roomId)
    {
        Msf.Client.Rooms.GetAccess(roomId, (access, error) =>
        {

            if (access == null)
            {
                Debug.LogError("Failed to get room access: " + error);
                status = "ERROR: Failed to get room access, please try again";
                return;
            }
        });
    }
    
    private IEnumerator JoinGame()
    {
        // Checks to see if client is connected to master server
        if (!Msf.Connection.IsConnected)
        {
            Debug.Log("You must be connected to master server");
            status = "ERROR: No connection to master server, please try again";
            yield break;
        }

        // Login as guest
        var promise = Msf.Events.FireWithPromise(Msf.EventNames.ShowLoading, "Logging in");
        bool loggedInAsGuest = false;
        status = "Logging in as guest";
        Msf.Client.Auth.LogInAsGuest((accInfo, error) =>
        {
            promise.Finish();

            loggedInAsGuest = true;

            if (accInfo == null)
            {
                Msf.Events.Fire(Msf.EventNames.ShowDialogBox, DialogBoxData.CreateError(error));
                Logs.Error(error);
                status = "ERROR: Unable to login as guest, please try again";
                return;
            }
        });

        while (loggedInAsGuest == false)
        {
            yield return null;
        }
        
        // Gets list of active games from master server
        Debug.Log("Retrieving gamelist");
        List<GameInfoPacket> gamesList = null;
        bool gotGamesList = false;
        status = "Retrieving game list";
        Msf.Client.Matchmaker.FindGames(games =>
        {
            gotGamesList = true;
            gamesList = games;
        });

        while (gotGamesList == false)
        {
            yield return null;
        }

        Debug.Log("Game list retrieved");
        status = "Game list retrieved";
        // Tries to find an avaliable game.
        // If none are found, make a new game.
        if (gamesList.Count == 0)
        {
            Debug.Log("Game not found, creating a game");
            status = "No avaliable games found, creating a game";
            CreateGame();
        }
        else
        {
            // Trys to find an open game
            Debug.Log("There seems to be avaliable games, hold on a sec");
            status = "Possible avaliable game, please hold";
            GameInfoPacket gameToJoin = null;
            foreach (GameInfoPacket i in gamesList)
            {
                if (i.OnlinePlayers < i.MaxPlayers)
                {
                    gameToJoin = i;
                    break;
                }
            }

            // If there is a game to join, connect to the game
            // Else, create a new game
            if (gameToJoin != null)
            {
                status = "Game found! Joining!";
                Msf.Client.Rooms.GetAccess(gameToJoin.Id, OnPassReceived);
            }
            else
            {
                Debug.Log("Game not found, creating a game");
                status = "No avaliable games found, creating a game";
                CreateGame();
            }

        }
    }

    // For the start button on the first login screen
    public void StartGame()
    {
        OnMatchMakerStart.Invoke();
        StartCoroutine("JoinGame");
    }

    public string GetStatus()
    {
        return status;
    }
    
}
