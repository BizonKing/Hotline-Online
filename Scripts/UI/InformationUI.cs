using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class InformationUI : NetworkBehaviour {

    public Text weaponNameText;
    public string weaponName;

    public Text eliminationText;
    public int eliminations;

    public Text ammoText;
    public int ammo;

    public GameObject player;

	// Update is called once per frame
	void Update ()
    {
        if (player == null)
        {
            return;
        }
        
        int numberOfEliminations = player.GetComponent<Player>().GetNumberOfEliminations();

        if (player.GetComponent<Player>().GetWeapon() != null)
        {
            weaponName = player.GetComponent<Player>().GetWeapon().GetComponent<Weapon>().weaponName;
            ammo = player.GetComponent<Player>().GetWeapon().GetComponent<Weapon>().ammo;
        }
        else
        {
            weaponName = "No Weapon";
            ammo = 0;
        }

        if (player.GetComponent<Player>().GetWeapon() != null && player.GetComponent<Player>().GetWeapon().GetComponent<Weapon>().isReloading)
        {
            ammoText.text = "Reloading";
        }
        else
        {
            ammoText.text = ammo.ToString();
        }


        eliminations = numberOfEliminations;
        weaponNameText.text = weaponName;
        ammoText.text = ammo.ToString();
        eliminationText.text = "Eliminations: " + eliminations;

	}
    
    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

}
