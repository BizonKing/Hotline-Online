using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionMenu : MonoBehaviour {

    public InputField screenNameInput;
    public Dropdown characterSelectionDropdown;
    public Dropdown starterWeaponSelectionDropdown;
    public NewNetworkManager newNetworkManager;

    public void EnterGame()
    {
        newNetworkManager.AddPlayer();
    }

    public string GetUserName()
    {
        return screenNameInput.text;
    }

    public string GetWeapon()
    {
        return starterWeaponSelectionDropdown.options[starterWeaponSelectionDropdown.value].text;
    }

    public string GetCharacter()
    {
        return characterSelectionDropdown.options[characterSelectionDropdown.value].text;
    }

}
