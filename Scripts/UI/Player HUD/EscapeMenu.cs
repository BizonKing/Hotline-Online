using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EscapeMenu : MonoBehaviour {

    public NewNetworkManager networkManager;
    public UIManager uiManager;

	// Use this for initialization
	void Start ()
    {
        networkManager = FindObjectOfType<NewNetworkManager>();
        uiManager = FindObjectOfType<UIManager>();
	}

    public void SwitchWeapon()
    {
        uiManager.TurnOffEscapeMenu();
        uiManager.TurnOnSwitchWeaponMenu();
    }
	
	public void Disconnect()
    {
        networkManager.StopClient();
        gameObject.SetActive(false);
    }
}
