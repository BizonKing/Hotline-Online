using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UIManager : MonoBehaviour {

    public bool autoCreateCharacter;
    public GameObject EscapeMenu;
    public GameObject CharacterSelectionUI;
    public GameObject SwitchWeaponUI;
    public GameObject InformationUI;
    public GameObject KillFeed;

    public GameObject player;
    public Player playerScript;

    public GameObject eliminated;

    private void Update()
    {
        if (playerScript != null && playerScript.dead)
        {
            eliminated.SetActive(true);
        }
        else
        {
            eliminated.SetActive(false);
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (!EscapeMenu.activeSelf)
            {
                TurnOnEscapeMenu();
            }
            else
            {
                TurnOffEscapeMenu();
            }
        }
    }

    public void TurnOnEscapeMenu()
    {
        EscapeMenu.SetActive(true);
        player.GetComponent<Player>().SetCanMove(false);
    }

    public void TurnOffEscapeMenu()
    {
        EscapeMenu.SetActive(false);
        player.GetComponent<Player>().SetCanMove(true);
    }

    public void TurnOnCharacterSelectionUI()
    {
        CharacterSelectionUI.SetActive(true);
    }

    public void TurnOffCharacterSelectionUI()
    {
        CharacterSelectionUI.SetActive(false);
    }

    public void TurnOnInformationUI()
    {
        InformationUI.GetComponent<InformationUI>().SetPlayer(player);
        InformationUI.SetActive(true);
    }
    
    public void TurnOffInformationUI()
    {
        InformationUI.SetActive(false);
    }

    public void TurnOnKillfeed()
    {
        KillFeed.SetActive(true);
    }

    public void TurnOffKillfeed()
    {
        KillFeed.SetActive(false);
    }

    public void TurnOnSwitchWeaponMenu()
    {
        SwitchWeaponUI.SetActive(true);
        player.GetComponent<Player>().SetCanMove(false);
    }

    public void TurnOffSwitchWeaponMenu()
    {
        SwitchWeaponUI.SetActive(false);
        player.GetComponent<Player>().SetCanMove(true);
    }

    public GameObject GetCharacterSelectionUI()
    {
        return CharacterSelectionUI;
    }

    public GameObject GetInformationUI()
    {
        return InformationUI;
    }

    public GameObject GetKillFeed()
    {
        return KillFeed;
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
        playerScript = player.GetComponent<Player>();
    }
    
}
