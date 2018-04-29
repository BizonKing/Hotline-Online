using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings (channel = 1, sendInterval = 0.1f)]
public class NetworkSyncRotation : NetworkBehaviour {

    [SyncVar(hook = "OnPlayerRotSynced")] private float syncPlayerRotation;

    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private float lerpRate = 5;

    private float lastPlayerRot;
    private float threshold = 0.1f;

    private List<float> syncPlayerRotList = new List<float>();
    private float closeEnough = 0.4f;
    [SerializeField]
    private bool useHistoricalInterpolation;

    // Use this for initialization
    void Start()
    {

    }

    void Update()
    {
        LerpRotations();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            TransmitRotations();
        }
    }

    void LerpRotations()
    {
        if (!isLocalPlayer)
        {
            if (useHistoricalInterpolation)
            {
                HistoricalInterpolation();
            }
            else
            {
                OrdinaryLerping();
            }
        }
    }

    void HistoricalInterpolation()
    {
        if (syncPlayerRotList.Count > 0)
        {
            LerpPlayerRotation(syncPlayerRotList[0]);

            if (Mathf.Abs(playerTransform.localEulerAngles.z - syncPlayerRotList[0]) < closeEnough)
            {
                syncPlayerRotList.RemoveAt(0);
            }

            //Debug.Log(syncPlayerRotList.Count.ToString() + " syncPlayerRotList Count");
        }
        
    }

    void OrdinaryLerping()
    {
        LerpPlayerRotation(syncPlayerRotation);
    }

    void LerpPlayerRotation(float rotAngle)
    {
        Vector3 playerNewRot = new Vector3(0, rotAngle, 0);
        playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, Quaternion.Euler(playerNewRot), lerpRate * Time.deltaTime);
    }

    void LerpCamRot(float rotAngle)
    {
        Vector3 camNewRot = new Vector3(rotAngle, 0, 0);
    }

    [Command]
    void CmdProvideRotationsToServer(float playerRot, float camRot)
    {
        syncPlayerRotation = playerRot;
    }

    [Client]
    void TransmitRotations()
    {
        if (isLocalPlayer)
        {
            if (CheckIfBeyondThreshold(playerTransform.localEulerAngles.z, lastPlayerRot))
            {
                lastPlayerRot = playerTransform.localEulerAngles.z;
            }
        }
    }

    bool CheckIfBeyondThreshold(float rot1, float rot2)
    {
        if (Mathf.Abs(rot1 - rot2) > threshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [Client]
    void OnPlayerRotSynced(float latestPlayerRot)
    {
        syncPlayerRotation = latestPlayerRot;
        syncPlayerRotList.Add(syncPlayerRotation);
    }

}
