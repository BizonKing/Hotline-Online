using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillfeedUIItem : MonoBehaviour {

    public Text text;

    public void SetUp(GameObject playerThatDied, GameObject playerThatKill)
    {
        Player playerThatDiedInfo = playerThatDied.GetComponent<Player>();
        Player playerThatKillInfo = playerThatKill.GetComponent<Player>();

        text.text = playerThatKillInfo.GetScreenName() + " eliminated " + playerThatDiedInfo.GetScreenName();
    }
	
}
