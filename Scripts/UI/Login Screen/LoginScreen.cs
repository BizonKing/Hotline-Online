using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : MonoBehaviour
{
    public Button startButton;
    public Button quitButton;
    public GameObject progressPanel;
    public Text progressStatus;
    public Button progressAbortButton;
    public LoginManager loginManager;

    void Start()
    {
        loginManager.OnMatchMakerStart += OnMatchMakerStart;
        loginManager.OnMatchMakerStop += OnMatchMakerStop;
        progressStatus.text = loginManager.GetStatus();
    }

    void Update()
    {
        progressStatus.text = loginManager.GetStatus();
    }

    public void JoinGame()
    {
        loginManager.StartGame();
    }

    public void CancelMatch()
    {
        loginManager.status = "Canceling";

        if (loginManager.spawnRequest == null)
        {
            // If there's no  request to abort, just hide the window
            loginManager.OnMatchMakerStop.Invoke();
            return;
        }

        // Start a timer which will close the window
        // after timeout, in case abortion fails
        StartCoroutine(CloseAfterRequest(10F, loginManager.spawnRequest.SpawnId));

        // Disable abort button
        progressAbortButton.interactable = false;

        loginManager.spawnRequest.Abort((isHandled, error) =>
        {
            // If request is not handled, enable the button abort button
            if (!isHandled)
            {
                progressAbortButton.interactable = true;
            }
            else
            {
                progressAbortButton.interactable = true;
                loginManager.OnMatchMakerStop.Invoke();
            }
        });
    }

    public IEnumerator CloseAfterRequest(float seconds, int spawnId)
    {
        yield return new WaitForSeconds(seconds);

        if ((loginManager.spawnRequest != null) && (loginManager.spawnRequest.SpawnId == spawnId))
        {
            loginManager.OnMatchMakerStop.Invoke();

            // Send another abort request just in case
            // (maybe something unstuck?)
            loginManager.spawnRequest.Abort();
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OnMatchMakerStart()
    {
        loginManager.status = "Starting Matchmaking";
        startButton.gameObject.SetActive(false);
        progressPanel.SetActive(true);
    }

    public void OnMatchMakerStop()
    {
        loginManager.status = "Stopping Matchmaking";
        startButton.gameObject.SetActive(true);
        progressPanel.SetActive(false);
    }
    

}