using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchWeaponMenu : MonoBehaviour {

    public UIManager uiManager;
    public Dropdown weaponSelection;
    
    
    public void Confirm()
    {
        string weapon = weaponSelection.options[weaponSelection.value].text;
        GameObject player = uiManager.player;
        Player playerScript = player.GetComponent<Player>();

        playerScript.CmdSetWeaponForPlayer(player, weapon);
        uiManager.TurnOffSwitchWeaponMenu();
    }

    public void Cancel()
    {
        uiManager.TurnOffSwitchWeaponMenu();
        uiManager.TurnOnEscapeMenu();
    }
	
}
