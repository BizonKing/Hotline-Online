using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerInformationUICanvas : MonoBehaviour {
    
    public Text screenNameText;
    public Text eliminationsText;
    public GameObject player;
    Quaternion rotation;

    void Awake()
    {
        rotation = transform.rotation;
    }

    private void LateUpdate()
    {
        transform.rotation = rotation;

        if (player == null)
            return;

        eliminationsText.text = player.GetComponent<Player>().GetNumberOfEliminations().ToString();
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    public void SetScreenNameText(string screenName)
    {
        screenNameText.text = screenName;
    }

}
