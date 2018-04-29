using System.Collections;
using System.Collections.Generic;
using Barebones.MasterServer;
using Barebones.Networking;
using UnityEngine;
using UnityEngine.UI;

public class DisplayConnectionStatus : MonoBehaviour
{

    private IClientSocket _connection;

    public Text Text;

    void Start()
    {
        Text = gameObject.GetComponent<Text>();
        _connection = GetConnection();
        _connection.StatusChanged += OnStatusChanged;
        OnStatusChanged(_connection.Status);
    }

    protected void OnStatusChanged(ConnectionStatus status)
    {
        switch (status)
        {
            case ConnectionStatus.Connected:
                Text.text = "Connected";
                break;
            case ConnectionStatus.Disconnected:
                Text.text = "Offline";
                break;
            case ConnectionStatus.Connecting:
                Text.text = "Connecting";
                break;
            default:
                Text.text = "Unknown";
                break;
        }
    }

    protected IClientSocket GetConnection()
    {
        return Msf.Connection;
    }

    protected void OnDestroy()
    {
        _connection.StatusChanged -= OnStatusChanged;
    }
}
