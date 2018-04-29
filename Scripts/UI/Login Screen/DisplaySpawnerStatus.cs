using System.Collections;
using System.Collections.Generic;
using Barebones.MasterServer;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySpawnerStatus : MonoBehaviour
{
    public Text Text;

    // Use this for initialization
    void Start()
    {
        Text = gameObject.GetComponent<Text>();
        Msf.Server.Spawners.SpawnerRegistered += OnSpawnerRegistered;
    }

    private void OnSpawnerRegistered(SpawnerController obj)
    {
        Text.text = "Started Spawner";
    }

    void OnDestroy()
    {
        Msf.Server.Spawners.SpawnerRegistered -= OnSpawnerRegistered;
    }
}
