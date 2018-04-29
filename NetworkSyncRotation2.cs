using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings (channel = 1, sendInterval = 0.1f)]
public class NetworkSyncRotation2 : NetworkBehaviour {

    [SyncVar]
    private Quaternion syncRotation;
    [SerializeField]
    private Transform objectTransform;
    [SerializeField]
    private float lerpRate = 5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        LerpRotation();
	}

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            TransmitRotation();
        }
    }

    void LerpRotation()
    {
        if (!isLocalPlayer)
        {
            OrdinaryLerping();
        }
    }

    void OrdinaryLerping()
    {
        LerpPlayerRotation(syncRotation);
    }

    void LerpPlayerRotation(Quaternion rotAngle)
    {
        //Vector3 playerNewRot = new Vector3(0, rotAngle, 0);
        objectTransform.rotation = Quaternion.Lerp(objectTransform.rotation, rotAngle, lerpRate * Time.deltaTime);
    }

    [Command]
    void CmdTransmitRotation(Quaternion rot)
    {
        syncRotation = rot;
    }

    [ClientCallback]
    void TransmitRotation()
    {
        if (isLocalPlayer)
        {
            CmdTransmitRotation(objectTransform.rotation);
        }
    }

    [Client]
    void OnPlayerRotSynced(Quaternion latestObjectRot)
    {
        syncRotation = latestObjectRot;
    }
}
